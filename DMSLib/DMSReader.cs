using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            var memSizeBefore = Process.GetCurrentProcess().PrivateMemorySize64;
            DMSFile file = null;
            DMSRowDecoder rowDecoder = new DMSRowDecoder();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (File.Exists(path))
            {
                file = new DMSFile();
                using (StreamReader sr = new StreamReader(File.OpenRead(path)))
                {
                    /* Read the version */
                    file.Version = sr.ReadLine().Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries)[2];

                    /* Blank line */
                    file.BlankLine = sr.ReadLine();

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
                    StringBuilder sb = new StringBuilder();
                    /* Read big metadata blob */
                    MemoryStream ms = new MemoryStream();
                    byte[] lineBytes = null;
                    currentLine = "";
                    while (currentLine != "/")
                    {
                        lineBytes = DMSDecoder.DecodeString(currentLine);
                        ms.Write(lineBytes, 0, lineBytes.Length);
                        currentLine = sr.ReadLine();
                    }
                    var ddlBlob = ms.ToArray();
                    file.DDLs = new DDLDefaults(ddlBlob);
                    ms.Close();
                    
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
                        ms = new MemoryStream();
                        while (currentLine != "/")
                        {
                            lineBytes = DMSDecoder.DecodeString(currentLine);
                            ms.Write(lineBytes, 0, lineBytes.Length);
                            currentLine = sr.ReadLine();
                        }

                        table.Metadata = new DMSRecordMetadata(ms.ToArray());
                        ms.Close();
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
                            if (m.Groups.Count == 6)
                            {
                                col.Size += m.Groups[5].Value;
                            }
                            table.Columns.Add(col);
                        }

                        currentLine = sr.ReadLine();
                        DMSRow row = new DMSRow();
                        sb.Clear();
                        while (currentLine != "/")
                        {
                            if (currentLine == "//")
                            {
                                var nextRow = new DMSRow();
                                rowDecoder.Finish(nextRow);
                                table.Rows.Add(nextRow);
                                rowDecoder.Reset();
                            }
                            else
                            {
                                rowDecoder.DecodeLine(currentLine);
                            }
                            currentLine = sr.ReadLine();
                        }
                        currentLine = sr.ReadLine();
                    }

                    /* Parse "Ended" */
                     file.Ended = currentLine.Replace("REM Ended: ", "");

                }
            }
            sw.Stop();
            Console.WriteLine("Total Read Time: " + sw.Elapsed.TotalSeconds + " seconds.");
            Console.WriteLine("DAT File Size: " + new FileInfo(path).Length / 1024.0 / 1024.0 + "MB");
            Console.WriteLine("Memory Size Increase: " + ((Process.GetCurrentProcess().PrivateMemorySize64 - memSizeBefore) / 1024.0) / 1024.0 + "MB");
            var totalRows = file.Tables.Sum(t => t.Rows.Count);
            Console.WriteLine("Total Row Count: " + totalRows);
            return file;
        }
    }
}
