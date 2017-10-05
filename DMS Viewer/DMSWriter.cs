using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS_Viewer
{
    public class DMSWriter
    {
        private DMSFile dmsFile;
        public DMSWriter(DMSFile dmsFile)
        {
            this.dmsFile = dmsFile;
        }

        public void Write(string outputPath)
        {

            using (StreamWriter stream = new StreamWriter(outputPath,false))
            {
                /* Drop the header to disk */
                foreach (var headerLine in dmsFile.HeaderLines)
                {
                    stream.WriteLine(headerLine);
                }

                /* Drop each table */
                foreach (var table in dmsFile.Tables)
                {
                    stream.WriteLine("/");
                    stream.WriteLine($"EXPORT  {table.Name}.{table.DBName} WHERE ");
                    WriteWhereClause(stream, table.WhereClause);
                    stream.WriteLine("/");

                    /* Write table metadata */
                    foreach(var metadataLine in table.Metadata)
                    {
                        stream.WriteLine(metadataLine);
                    }


                    /* Write the column info */
                    WriteColumns(stream, table.Columns);


                    /* Write each row of data */
                    foreach(DMSTableRow row in table.Rows)
                    {
                        WriteTableRow(stream, row);
                        stream.WriteLine("//");
                    }
                    stream.WriteLine("/");
                    stream.WriteLine($"REM Ended: {dmsFile.Ended}");
                }
            }
        }

        private void WriteTableRow(StreamWriter stream, DMSTableRow row)
        {
            //throw new NotImplementedException();
            StringBuilder sb = new StringBuilder();
            var LINE_LENGTH = 70;
            foreach(var v in row.Values)
            {
                var strEnc = EncodeString(v);
                sb.Append(strEnc).Append(",");
            }

            /* Split this into lines of LINE_LENGTH */
            string fixedLine = "";
            List<string> lines = new List<string>();
            while (sb.Length > LINE_LENGTH)
            {
                LINE_LENGTH = 70;

                var lineLength = LINE_LENGTH;
                /* Need to find if we are in an A tag or a B tag */
                var startPos = LINE_LENGTH - 1;

                var endingChar = sb[startPos];

                if (endingChar == ',')
                {
                    fixedLine = sb.ToString(0, LINE_LENGTH);
                    lines.Add(fixedLine);
                    sb.Remove(0, LINE_LENGTH);
                } else if (endingChar == ')')
                {
                    fixedLine = sb.ToString(0, LINE_LENGTH + 1);
                    lines.Add(fixedLine);
                    sb.Remove(0, LINE_LENGTH + 1);
                }
                else if (endingChar == '(')
                {
                    fixedLine = sb.ToString(0, LINE_LENGTH - 2);
                    lines.Add(fixedLine);
                    sb.Remove(0, LINE_LENGTH - 2);
                }
                else
                {
                    if ((endingChar == 'A' || endingChar == 'B') && sb[startPos - 1] == ',')
                    {
                        fixedLine = sb.ToString(0, LINE_LENGTH - 1);
                        lines.Add(fixedLine);
                        sb.Remove(0, LINE_LENGTH - 1);
                    }
                    else
                    {
                        while (sb[startPos] != '(')
                        {
                            startPos--;
                        }

                        var tagType = sb[startPos - 1] == 'A' ? EncodeTags.ASCII : EncodeTags.BINARY;
                        var lengthAdd = 0;
                        if (sb[LINE_LENGTH - 2] == '(')
                        {
                            /* we dont want to end with A() */
                            lengthAdd = (tagType == EncodeTags.ASCII ? 1 : 2);
                        }
                        if (sb[LINE_LENGTH] == ')')
                        {
                            /* Allow it to close the encoding */
                            lengthAdd++;
                            tagType = EncodeTags.NONE;
                        }
                        if (sb[LINE_LENGTH + 1] == ',')
                        {
                            /* Allow trailing commas on the line */
                            lengthAdd++;
                            tagType = EncodeTags.NONE;
                        }

                        LINE_LENGTH += lengthAdd;
                        fixedLine = sb.ToString(0, LINE_LENGTH - 1) + (tagType != EncodeTags.NONE ? ")" : "");
                        lines.Add(fixedLine);
                        sb.Remove(0, LINE_LENGTH - 1);
                        if (tagType != EncodeTags.NONE)
                        {
                            sb.Insert(0, tagType == EncodeTags.ASCII ? "A(" : "B(");
                        }
                    }
                }
                
            }
            lines.Add(sb.ToString());

            foreach(var line in lines)
            {
                stream.WriteLine(line);
            }

        }
        private string EncodeString(string s)
        {
            return EncodeData(UTF8Encoding.UTF8.GetBytes(s));
        }

        private string EncodeData(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            EncodeTags currentTag = EncodeTags.NONE;
            
            foreach (byte b in data)
            {
                if (b >= 32 && b <=127)
                {
                    /* we'll treat this as an ascii character */
                    switch (currentTag)
                    {
                        case EncodeTags.NONE:
                            sb.Append("A(");
                            currentTag = EncodeTags.ASCII;
                            break;
                        case EncodeTags.BINARY:
                            sb.Append(")A(");
                            currentTag = EncodeTags.ASCII;
                            break;
                        case EncodeTags.ASCII:
                            break;
                    }
                    sb.Append((char)b);
                } else
                {
                    /* we'll treat this as a binary */
                    switch (currentTag)
                    {
                        case EncodeTags.NONE:
                            sb.Append("B(");
                            currentTag = EncodeTags.BINARY;
                            break;
                        case EncodeTags.ASCII:
                            sb.Append(")B(");
                            currentTag = EncodeTags.BINARY;
                            break;
                        case EncodeTags.BINARY:
                            break;
                    }
                    sb.Append(EncodeByte(b));
                }

            }

            if (currentTag != EncodeTags.NONE)
            {
                sb.Append(")");
            }
            
            return sb.ToString();
        }

        private string EncodeByte(byte b)
        {
            StringBuilder sb = new StringBuilder();
            if (b < 194)
            {
                var lowerHalf = (char)((b & 0xF) + 65);
                var upperHalf = (char)(((b & 0xF0) >> 4) + 65);
                sb.Append(upperHalf).Append(lowerHalf);
            } else
            {
                var firstNumber = 194 + (b / 64);
                var secondNumber = b - (64 * (firstNumber - 194));

                var firstLower = (char)((firstNumber & 0xF) + 65);
                var firstUpper = (char)(((firstNumber & 0xF0) >> 4) + 65);

                var secondLower = (char)((secondNumber & 0xF) + 65);
                var secondUpper = (char)(((secondNumber & 0xF0) >> 4) + 65);

                sb.Append(firstUpper).Append(firstLower).Append(secondUpper).Append(secondLower);
            }

            return sb.ToString();
        }

        enum EncodeTags
        {
            ASCII,BINARY,NONE
        }

        private void WriteWhereClause(StreamWriter stream, string where)
        {
            /* TODO: Fix this? */
            stream.WriteLine(where);
        }

        private void WriteColumns(StreamWriter stream, List<DMSTableColumn> columns)
        {
            var maxLength = 70;
            var curLineLength = 0;
            stream.WriteLine("/");
            foreach (DMSTableColumn col in columns)
            {
                var nextColumn = $"{col.Name}:{col.Type}({col.Size})~~~";
                if (curLineLength + nextColumn.Length < maxLength)
                {
                    stream.Write(nextColumn);
                    curLineLength += nextColumn.Length;
                }
                else
                {
                    stream.WriteLine();
                    stream.Write(nextColumn);
                    curLineLength = nextColumn.Length;
                }
            }
            stream.WriteLine();
            stream.WriteLine("/");
        }
    }
}
