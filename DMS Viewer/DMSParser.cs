using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    public class DMSParser
    {
        DMSFile _file;
        ParserState state = ParserState.BASE_INFO;
        Regex versionRegex = new Regex(@"SET VERSION_DAM\s+(.*)");
        Regex databaseRegex = new Regex(@"REM Database: (.*)");
        Regex startedRegex = new Regex(@"REM Started: (.*)");
        Regex endedRegex = new Regex(@"REM Ended: (.*)");

        Regex tableNameRegex = new Regex(@"EXPORT\s+(.*?)\.(.*?)\s+(WHERE)?");
        Regex columnRegex = new Regex(@"(([A-Z0-9_]+):(\d?[A-Z]+)\((\d+)(,\d*)?\)~~~).*?");
        int lineNumber = 0;

        string previousLine = "";
        string currentLine = "";
        StreamReader reader;
        bool captureHeader = true;

        public DMSFile ParseFile(string filename)
        {
            _file = new DMSFile();
            _file.FileName = new FileInfo(filename).Name;
            using (StreamReader reader = new StreamReader(filename))
            {
                this.reader = reader;
                var currentLine = "";
                var previousLine = "";
                while (reader.EndOfStream == false)
                {
                    previousLine = currentLine;
                    currentLine = GetNextLine();
                    ProcessLine();
                }
            }

            /* Decode the metadata for the records */
            foreach (var table in _file.Tables)
            {
                StringBuilder sb = new StringBuilder();
                foreach(var line in table.MetadataLines)
                {
                    sb.Append(line);
                }

                var f = DecodeMetadata(sb.ToString());
                table.Metadata = new DMSRecordMetadata(f);
            }

            return _file;
        }


        private void ProcessLine()
        {
            if (captureHeader)
            {
                _file.HeaderLines.Add(currentLine);
            }


            if (state == ParserState.BASE_INFO)
            {
                ProcessBaseInfoLine();
            } else if (state == ParserState.LOOKING_EXPORT)
            {
                var match = endedRegex.Match(currentLine);
                if (match.Success)
                {
                    _file.Ended = match.Groups[1].Value;
                }
                else
                {
                    ProcessExportLine();
                }
            } else if (state == ParserState.LOOKING_COLUMNS)
            {
                ProcessColumnsLine();
            } else if (state == ParserState.LOOKING_ROW_DATA)
            {
                ProcessRowData();
            }

        }
        int leftOverValue = 0; 

        private byte[] DecodeMetadata(string data)
        {
            MemoryStream ms = new MemoryStream();
            var curIndex = 0;
            while (curIndex < data.Length - 1)
            {
                if (data[curIndex] == 'A' && data[curIndex + 1] == '(')
                {
                    curIndex += 2;
                    /* ascii */
                    while (data[curIndex] != ')')
                    {
                        if (data[curIndex] == '\\')
                        {
                            /* escape char */
                            curIndex++;
                            ms.WriteByte((byte)data[curIndex++]);
                        }
                        else
                        {
                            ms.WriteByte((byte)data[curIndex++]);
                        }
                    }
                    /* skip closing paren */
                    curIndex++;
                    continue;
                }
                if (data[curIndex] == 'B' && data[curIndex + 1] == '(')
                {
                    curIndex += 2;
                    /* binary */
                    StringBuilder sb = new StringBuilder();
                    while (data[curIndex] != ')')
                    {
                        sb.Append(data[curIndex++]);
                    }
                    curIndex++;
                    var foo = DecodeBinaryData(sb.ToString());
                    var bytes = UTF8Encoding.UTF8.GetBytes(foo);
                    ms.Write(bytes,0,bytes.Length);
                }
            }

            return ms.ToArray();
        }

        private string DecodeBinaryData(string data)
        {
            bool isUsingLeftover = leftOverValue != 0;

            var chars = data.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (var x = 0; x < chars.Length - 1; x += 2)
            {
                var decodedValue = 0;
                if (isUsingLeftover)
                {
                    decodedValue = leftOverValue;
                } else
                {
                    decodedValue = (chars[x] - 'A') << 4 | (chars[x + 1] - 'A');
                }
                
                if (decodedValue >= 194)
                {
                    if (isUsingLeftover == false)
                    {
                        x += 2;
                    } else
                    {
                        isUsingLeftover = false;
                        leftOverValue = 0;
                    }
                    if (x == chars.Length)
                    {
                        leftOverValue = decodedValue;
                        break;
                    }
                    var secondValue = (chars[x] - 'A') << 4 | (chars[x + 1] - 'A');

                    if (decodedValue > 194)
                    {
                        secondValue += (64 * (decodedValue - 194));
                    }
                    decodedValue = secondValue;
                }
                sb.Append((char)decodedValue);
            }
            return sb.ToString();
        }

        private void DecodeRowData(DMSTableRow row, string data)
        {
            var curIndex = 0;
            var columnValue = "";
            while (curIndex < data.Length - 1)
            {
                if (data[curIndex] == 'A' && data[curIndex+1] == '(')
                {
                    curIndex += 2;
                    /* ascii */
                    while(data[curIndex] != ')')
                    {
                        if (data[curIndex] == '\\')
                        {
                            /* escape char */
                            curIndex++;
                            columnValue += data[curIndex++];
                        } else
                        {
                            columnValue += data[curIndex++];
                        }                        
                    }
                    /* skip closing paren */
                    curIndex++;
                    continue;
                }
                if (data[curIndex] == 'B' && data[curIndex + 1] == '(')
                {
                    curIndex += 2;
                    /* binary */
                    StringBuilder sb = new StringBuilder();
                    while (data[curIndex] != ')')
                    {
                        sb.Append(data[curIndex++]);
                    }
                    curIndex++;

                    columnValue += DecodeBinaryData(sb.ToString());
                }
                if (data[curIndex] == ',')
                {
                    /* save this column value */
                    row.Values.Add(columnValue);
                    columnValue = "";
                    curIndex++;
                }
            }
            if (columnValue.Length > 0)
            {
                row.Values.Add(columnValue);
            }
            columnValue = "";
        }

        private void ProcessRowData()
        {
            if  (currentLine[0] == '/')
            {
                /* reached end of row data */
                state = ParserState.LOOKING_EXPORT;
                return;
            }
            string rowData = currentLine;

            /* get all the row data here */
            while (currentLine.Equals("/") == false)
            {
                currentLine = GetNextLine();
                if (currentLine.Equals("/"))
                {
                    break;
                }
                if (currentLine.Equals("//"))
                {
                    DMSTableRow row = new DMSTableRow();
                    DecodeRowData(row, rowData);
                    _file.Tables[_file.Tables.Count - 1].Rows.Add(row);
                    rowData = "";
                } else
                {
                    rowData += currentLine;
                }
            }
            state = ParserState.LOOKING_EXPORT;
        }

        private void ProcessColumnsLine()
        {
            if (state == ParserState.LOOKING_COLUMNS && previousLine.Equals("/"))
            {
                var columnText = "";
                /* check this currentLine for the column regex */
                if (columnRegex.IsMatch(currentLine))
                {
                    /* we have columns, read them all */
                    columnText += currentLine;
                    while (reader.Peek() != '/')
                    {
                        currentLine = GetNextLine();
                        if (currentLine[0] != '/')
                        {
                            columnText += currentLine;
                        }
                    }
                    columnText = columnText.Replace("\r", "").Replace("\n", "");
                    var matches = columnRegex.Matches(columnText);

                    foreach (Match m in matches)
                    {
                        DMSTableColumn col = new DMSTableColumn();
                        col.Name = m.Groups[2].Value;
                        col.Type = m.Groups[3].Value;
                        col.Size = m.Groups[4].Value;

                        _file.Tables.Last().Columns.Add(col);
                    }
                    /* eat the final / currentLine and set update state */
                    GetNextLine();
                    state = ParserState.LOOKING_ROW_DATA;

                } else
                {
                    _file.Tables.Last().MetadataLines.Add(currentLine);
                }
            }
            else
            {
                if (currentLine != "/")
                {
                    _file.Tables.Last().MetadataLines.Add(currentLine);
                }
            }
        }
        private void ProcessExportLine()
        {
            if (state == ParserState.LOOKING_EXPORT && previousLine.Equals("/") && currentLine.StartsWith("EXPORT"))
            {
                captureHeader = false;
                /* remove the last 2 lines, which are not part of the header */
                _file.HeaderLines.RemoveAt(_file.HeaderLines.Count - 1);
                _file.HeaderLines.RemoveAt(_file.HeaderLines.Count - 1);
                /* here is the start of an export statement */

                var match = tableNameRegex.Match(currentLine);

                DMSTable table = new DMSTable();
                table.Name = match.Groups[1].Value;
                table.DBName = match.Groups[2].Value;
                table.WhereClause = "";

                while (reader.Peek() != '/')
                {
                    table.WhereClause += GetNextLine();
                }
                _file.Tables.Add(table);

                /* set state to "looking for columns" */
                state = ParserState.LOOKING_COLUMNS;
            }
        }

        private void ProcessBaseInfoLine()
        {
            /* we are looking for the following lines */
            /* SET VERSION_DAM  8.5:9:0 */
            /* REM Database: IEP91DEV */
            /* REM Started: Mon Oct 20 17:13:50 2014 */
            var match = versionRegex.Match(currentLine);
            if (match.Success)
            {
                _file.Version = match.Groups[1].Value;
            }

            match = databaseRegex.Match(currentLine);
            if (match.Success)
            {
                _file.Database = match.Groups[1].Value;
            }

            match = startedRegex.Match(currentLine);
            if (match.Success)
            {
                _file.Started = match.Groups[1].Value;

                /* once we have found "started", switch parser to next state */
                state = ParserState.LOOKING_EXPORT;
            }
        }

        private string GetNextLine()
        {
            previousLine = currentLine;
            currentLine = reader.ReadLine();
            lineNumber++;
            return currentLine;
        }
    }

    enum ParserState
    {
        BASE_INFO,LOOKING_EXPORT, LOOKING_COLUMNS, LOOKING_ROW_DATA
    }

    public class DMSFile
    {
        public string Version;
        public string Database;
        public string Started;
        public string Ended;
        public string FileName;
        public List<string> HeaderLines = new List<string>();
        public List<DMSTable> Tables = new List<DMSTable>();

    }

    public class DMSTable
    {
        public string Name;
        public string DBName;
        public string WhereClause;
        public List<string> MetadataLines = new List<string>();
        public DMSRecordMetadata Metadata;
        public List<DMSTableColumn> Columns = new List<DMSTableColumn>();
        public List<DMSTableRow> Rows = new List<DMSTableRow>();
        public override string ToString()
        {
            return Name;
        }
    }

    public class DMSTableColumn
    {
        public string Name;
        public string Type;
        public string Size;

        public override string ToString()
        {
            return Name + ":" + Type + "(" + Size + ")";
        }
    }

    public class DMSTableRow
    {
        public List<string> Values = new List<string>();
    }
    
    public class DMSRecordMetadata
    {
        public string RecordLanguage;
        public string OwnerID;
        public string AnalyticDeleteRecord;
        public string ParentRecord;
        public string RecordName;
        public string RelatedLanguageRecord;
        public string RecordDBName;
        /* Unknown 76 bytes, seems to be all 0's */
        public string OptimizationTriggers;
        /* 10 bytes unknown 00 00 01 00 00 00 00 00 00 00 */
        public int VersionNumber;
        public int FieldCount;
        public int BuildSequence;
        public int IndexCount;
        /* 4 bytes unknown */
        public int VersionNumber2;
        /* 22 bytes unknown*/

        /* List of Field Info structures */

        private string FromUnicodeBytes(byte[] data)
        {
            var str = Encoding.Unicode.GetString(data);
            var nullIndex = str.IndexOf('\0');
            str = str.Substring(0, nullIndex);

            return str;
        }

        public DMSRecordMetadata(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));
            RecordLanguage = FromUnicodeBytes(br.ReadBytes(8));
            OwnerID = FromUnicodeBytes(br.ReadBytes(10));
            AnalyticDeleteRecord = FromUnicodeBytes(br.ReadBytes(32));
            ParentRecord = FromUnicodeBytes(br.ReadBytes(32));
            RecordName = FromUnicodeBytes(br.ReadBytes(32));
            RelatedLanguageRecord = FromUnicodeBytes(br.ReadBytes(32));
            RecordDBName = FromUnicodeBytes(br.ReadBytes(38));

            /* Unknwon 76 bytes */
            br.ReadBytes(76);
            OptimizationTriggers = FromUnicodeBytes(br.ReadBytes(4));

            /* Unknown 10 bytes */
            br.ReadBytes(10);

            VersionNumber = BitConverter.ToInt32(br.ReadBytes(4), 0);
            FieldCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
            BuildSequence = BitConverter.ToInt32(br.ReadBytes(4), 0);
            IndexCount = BitConverter.ToInt32(br.ReadBytes(4), 0);

            /* Unknown 8 bytes */
            br.ReadBytes(4);
            
            VersionNumber2 = BitConverter.ToInt32(br.ReadBytes(4), 0);

            /* Unknown 22 bytes */
            br.ReadBytes(22);
        }
    }
}
