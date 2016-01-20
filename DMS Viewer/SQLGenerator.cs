using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS_Viewer
{
    public class SQLGenerator
    {
        public static void GenerateSQLFile(DMSFile dms, string filepath, bool padColumns)
        {
            StreamWriter sw = new StreamWriter(filepath);

            /* Write the header */

            sw.WriteLine("/* ****** DMS 2 SQL v1.0 ***********");
            sw.WriteLine(String.Format(" * Database: {0}", dms.Database));
            sw.WriteLine(String.Format(" * Date: {0}", dms.Started));
            sw.WriteLine(String.Format(" * Table Count: {0}", dms.Tables.Count));
            sw.WriteLine(String.Format(" * Total Rows: {0}", dms.Tables.Sum(t => t.Rows.Count)));
            sw.WriteLine("*/");


            foreach( DMSTable table in dms.Tables)
            {
                /* Write table header */
                sw.WriteLine(String.Format("/* Begin Table: {0}({1})", table.Name, table.DBName));
                sw.WriteLine(String.Format(" * Where Clause: {0}", table.WhereClause.Replace("\r", "").Replace("\n", "")));
                sw.WriteLine(String.Format(" * Row Count: {0}", table.Rows.Count));
                sw.WriteLine("*/\r\n");

                var columnText = new StringBuilder();
                var columnNames = new StringBuilder();
                var columnValues = new StringBuilder();

                foreach (DMSTableRow row in table.Rows)
                {
                    columnNames.Clear();
                    columnValues.Clear();

                    for (var x = 0; x < row.Values.Count; x++)
                    {
                        var colType = table.Columns[x].Type;
                        var colName = table.Columns[x].Name;

                        var printValue = "";
                        switch(colType)
                        {
                            case "CHAR":
                                printValue = SafeString(row.Values[x]);
                                break;
                            case "DATE":

                            default:
                                printValue = row.Values[x] == null ? "NULL" : row.Values[x].ToString();
                                break;
                        }

                        /* print them out with padding */
                        var valuePad = 0;
                        var colPad = 0;
                        if (padColumns)
                        {
                            if (colName.Length > printValue.Length)
                            {
                                valuePad = colName.Length - printValue.Length;
                                colPad = 0;
                            }
                            else if (colName.Length < printValue.Length)
                            {
                                colPad = printValue.Length - colName.Length;
                                valuePad = 0;
                            }
                            else
                            {
                                colPad = 0;
                                valuePad = 0;
                            }
                        }

                        columnNames.Append(colName).Append(new string(' ', colPad)).Append(", ");
                        columnValues.Append(printValue).Append(new string(' ', valuePad)).Append(", ");
                    }

                    var sqlStatement = String.Format("INSERT INTO {0} \r\n({1}) \r\nVALUES \r\n({2});", table.DBName, columnNames.ToString(),columnValues.ToString());
                    sw.WriteLine(sqlStatement);
                }

                sw.WriteLine();
            }


            sw.Flush();
            sw.Close();
        }

        private static string SafeString(string s)
        {
            return "'" + s + "'";
        }
    }
}
