using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMS_Viewer
{
    public class DMSParser
    {
        DMSFile _file;
        ParserState state = ParserState.BASE_INFO;
        Regex versionRegex = new Regex(@"SET VERSION_DAM\s+(.*)");
        Regex databaseRegex = new Regex(@"REM Database: (.*)");
        Regex startedRegex = new Regex(@"REM Started: (.*)");

        Regex tableNameRegex = new Regex(@"EXPORT\s+(.*?)\.(.*?)\s+WHERE");
        Regex columnRegex = new Regex(@"(([A-Z0-9_]+):([A-Z]+)\((\d+)\)~~~).*?");
        public DMSFile ParseFile(string filename)
        {
            _file = new DMSFile();
            using (StreamReader reader = new StreamReader(filename))
            {
                var line = "";
                var previousLine = "";
                while (reader.EndOfStream == false)
                {
                    previousLine = line;
                    line = reader.ReadLine();
                    ProcessLine(previousLine, line, reader);
                }
            }
            return _file;
        }


        private void ProcessLine(string previous, string line, StreamReader reader)
        {
            if (state == ParserState.BASE_INFO)
            {
                ProcessBaseInfoLine(line);
            } else if (state == ParserState.LOOKING_EXPORT)
            {
                ProcessExportLine(previous, line, reader);
            } else if (state == ParserState.LOOKING_COLUMNS)
            {
                ProcessColumnsLine(previous, line, reader);
            } else if (state == ParserState.LOOKING_ROW_DATA)
            {
                ProcessRowData(previous, line, reader);
            }
        }

        private void ProcessRowData(string previous, string line, StreamReader reader)
        {
            if  (line[0] == '/')
            {
                /* reached end of row data */
                state = ParserState.LOOKING_EXPORT;
                return;
            }
            string rowData = line;
            List<string> rows = new List<string>();

            /* get all the row data here */
            while (line.Equals("/") == false)
            {
                line = reader.ReadLine();
                if (line.Equals("/"))
                {
                    break;
                }
                if (line.Equals("//"))
                {
                    rows.Add(rowData);
                    rowData = "";
                } else
                {
                    rowData += line;
                }
            }
            state = ParserState.LOOKING_EXPORT;
            int i = 3;
        }

        private void ProcessColumnsLine(string previous, string line, StreamReader reader)
        {
            if (state == ParserState.LOOKING_COLUMNS && previous.Equals("/"))
            {
                var columnText = "";
                /* check this line for the column regex */
                if (columnRegex.IsMatch(line))
                {
                    /* we have columns, read them all */
                    columnText += line;
                    while (reader.Peek() != '/') {
                        line = reader.ReadLine();
                        if (line[0] != '/')
                        {
                            columnText += line;
                        }
                    }
                    columnText = columnText.Replace("\r", "").Replace("\n","");
                    var matches = columnRegex.Matches(columnText);

                    foreach (Match m in matches)
                    {
                        DMSTableColumn col = new DMSTableColumn();
                        col.Name = m.Groups[2].Value;
                        col.Type = m.Groups[3].Value;
                        col.Size = m.Groups[4].Value;

                        _file.Tables.Last().Columns.Add(col);
                    }
                    /* eat the final / line and set update state */
                    reader.ReadLine();
                    state = ParserState.LOOKING_ROW_DATA;
                    
                }
            }
        }
        private void ProcessExportLine(string previous, string line, StreamReader reader)
        {
            if (state == ParserState.LOOKING_EXPORT && previous.Equals("/") && line.StartsWith("EXPORT"))
            {
                /* here is the start of an export statement */

                var match = tableNameRegex.Match(line);

                DMSTable table = new DMSTable();
                table.Name = match.Groups[1].Value;
                table.DBName = match.Groups[2].Value;
                table.WhereClause = "";

                while (reader.Peek() != '/')
                {
                    table.WhereClause += reader.ReadLine();
                }
                _file.Tables.Add(table);

                /* set state to "looking for columns" */
                state = ParserState.LOOKING_COLUMNS;
            }
        }

        private void ProcessBaseInfoLine(string line)
        {
            /* we are looking for the following lines */
            /* SET VERSION_DAM  8.5:9:0 */
            /* REM Database: IEP91DEV */
            /* REM Started: Mon Oct 20 17:13:50 2014 */
            var match = versionRegex.Match(line);
            if (match.Success)
            {
                _file.Version = match.Groups[1].Value;
            }

            match = databaseRegex.Match(line);
            if (match.Success)
            {
                _file.Database = match.Groups[1].Value;
            }

            match = startedRegex.Match(line);
            if (match.Success)
            {
                _file.Started = match.Groups[1].Value;

                /* once we have found "started", switch parser to next state */
                state = ParserState.LOOKING_EXPORT;
            }
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

        public List<DMSTable> Tables = new List<DMSTable>();

    }

    public class DMSTable
    {
        public string Name;
        public string DBName;
        public string WhereClause;

        public List<DMSTableColumn> Columns = new List<DMSTableColumn>();

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

    
}
