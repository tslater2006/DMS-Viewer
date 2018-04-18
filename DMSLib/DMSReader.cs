using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DMSLib
{
    public class DMSReader
    {
        static Regex tableNameRegex = new Regex(@"EXPORT\s+(.*?)\.(.*?)\s+(WHERE)?");
        static Regex columnRegex = new Regex(@"(([A-Z0-9_]+):(\d?[A-Z]+)\((\d+)(,\d*)?\)~~~).*?");

        public static DMSFile Read(string path)
        {
            DMSFile file = null;

            if (File.Exists(path))
            {
                file = new DMSFile();
                using (StreamReader sr = new StreamReader(File.OpenRead(path)))
                {
                    /* Read the version */
                    file.Version = sr.ReadLine().Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries)[2];

                    /* Blank line */
                    sr.ReadLine();

                    file.Endian = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    file.BaseLanguage = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    file.Database = sr.ReadLine().Replace("REM Database: ", "");
                    file.Started = sr.ReadLine().Replace("REM Started: ", "");

                    /* Read the EXPORT RECROD/SPACE.x */
                    sr.ReadLine();


                    /* Read namespaces */
                    var currentLine = "";
                    while (currentLine != "/")
                    {
                        currentLine = sr.ReadLine();
                        if (currentLine != "/")
                        {
                            file.Namespaces.Add(currentLine);
                        }
                    }

                    /* Read big metadata blob */
                    StringBuilder sb = new StringBuilder();
                    currentLine = "";
                    while (currentLine != "/")
                    {
                        currentLine = sr.ReadLine();
                        if (currentLine != "/")
                        {
                            sb.Append(currentLine);
                        }
                    }

                    file.FileMetadata = DMSDecoder.DecodeString(sb.ToString());

                    currentLine = sr.ReadLine();

                    while (currentLine.StartsWith("EXPORT"))
                    {
                        /* Handle a table export */
                        DMSTable table = new DMSTable();
                        file.Tables.Add(table);
                        /* Record name */
                        var match = tableNameRegex.Match(currentLine);
                        table.Name = match.Groups[1].Value;
                        table.DBName = match.Groups[2].Value;
                        table.WhereClause = "";

                        /* Record where clause */
                        currentLine = sr.ReadLine();
                        while (currentLine != "/")
                        {
                            if (table.WhereClause.Length > 0)
                            {
                                table.WhereClause += "\r\n";
                            }
                            table.WhereClause += currentLine ;
                            currentLine = sr.ReadLine();
                        }

                        /* Record metadata */
                        sb.Clear();
                        currentLine = sr.ReadLine();
                        while (currentLine != "/")
                        {
                            sb.Append(currentLine);
                            currentLine = sr.ReadLine();
                        }

                        table.Metadata = new DMSRecordMetadata(DMSDecoder.DecodeString(sb.ToString()));

                        /* Record Columns */
                        currentLine = sr.ReadLine();
                        sb.Clear();
                        while (currentLine != "/")
                        {
                            sb.Append(currentLine);
                            currentLine = sr.ReadLine();
                        }

                        /* Parse the columns */
                        var matches = columnRegex.Matches(sb.ToString());

                        foreach (Match m in matches)
                        {
                            DMSColumn col = new DMSColumn();
                            col.Name = m.Groups[2].Value;
                            col.Type = m.Groups[3].Value;
                            col.Size = m.Groups[4].Value;

                            table.Columns.Add(col);
                        }

                        currentLine = sr.ReadLine();
                        DMSRow row = new DMSRow();
                        sb.Clear();
                        while (currentLine != "/")
                        {
                            if (currentLine == "//")
                            {
                                /* Parse the row */
                                var rowText = sb.ToString();

                                List<string> fieldData = new List<string>();
                                var openParens = 0;
                                sb.Clear();
                                foreach (char c in rowText)
                                {
                                    if (c == '(' && sb[sb.Length - 1] != '\\')
                                    {
                                        openParens++;
                                        sb.Append(c);
                                    }
                                    else if (c == ')' && sb[sb.Length - 1] != '\\')
                                    {
                                        openParens--;
                                        sb.Append(c);
                                    }
                                    else if (c == ',' && openParens == 0)
                                    {
                                        fieldData.Add(sb.ToString());
                                        sb.Clear();
                                    }
                                    else
                                    {
                                        sb.Append(c);
                                    }
                                }

                                if (sb.Length > 0)
                                {
                                    fieldData.Add(sb.ToString());
                                }

                                foreach (var str in fieldData)
                                {
                                    byte[] data = DMSDecoder.DecodeString(str);
                                    row.Values.Add(data);
                                }

                                table.Rows.Add(row);
                                row = new DMSRow();
                                sb.Clear();
                            }
                            else
                            {
                                sb.Append(currentLine);
                            }
                            currentLine = sr.ReadLine();
                        }
                        currentLine = sr.ReadLine();
                    }

                    /* Parse "Ended" */
                    file.Ended = currentLine.Replace("REM Ended: ", "");

                }
            }

            return file;
        }
    }
}
