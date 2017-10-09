using DMSLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS_Viewer
{
    public class DMSRebuilder
    {
        public static void RebuildSingleFile(string datFile, string outputFolder, string scriptName)
        {
            List<string> files = new List<string>();
            files.Add(datFile);
            RebuildFiles(files, outputFolder, scriptName);
        }

        public static void RebuildDirectory(string datFolder, string outputFolder, string scriptName)
        {
            var files = Directory.EnumerateFiles(datFolder, "*.dat", SearchOption.AllDirectories).ToList<string>();
            RebuildFiles(files, outputFolder, scriptName);
        }

        private static void RebuildFiles(List<string> files, string outputFolder, string scriptName)
        {
            Console.WriteLine("  Beginning rebuild of " + files.Count + " files.");

            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
            FileStream fs = new FileStream(outputFolder + Path.DirectorySeparatorChar + scriptName + "_EXP.dms", FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(fs);
            foreach (var file in files)
            {

                var dmsFile = DMSReader.Read(file);
                Console.WriteLine("  Processing file: " + dmsFile.FileName);
                Console.WriteLine("    Table count: " + dmsFile.Tables.Count);
                Console.WriteLine("    Total rows: " + dmsFile.Tables.Sum(t => t.Rows.Count));
                foreach (var table in dmsFile.Tables)
                {

                    if (table.Rows.Count == 0)
                    {
                        continue;
                    }

                    if (results.ContainsKey(table.Name) == false)
                    {
                        results.Add(table.Name, new List<string>());
                    }
                    if (results[table.Name].Contains(table.WhereClause) == false)
                    {
                        results[table.Name].Add(table.WhereClause);
                    }
                }

            }
            Console.WriteLine("  All Files processed, generating unified scripts.");

            sw.WriteLine("-- Rebuilt with DMSUtils -- ");
            sw.WriteLine();
            sw.WriteLine("SET OUTPUT " + scriptName + "_EXP.dat;");
            sw.WriteLine("SET LOG " + scriptName + "_EXP.log;");
            sw.WriteLine("");
            foreach (var key in results.Keys)
            {
                var whereList = results[key];
                whereList.Sort();
                foreach (var where in whereList)
                {
                    if (results[key].Contains("") && where.Equals("") == false)
                    {
                        sw.Write("-- EXPORT " + key);
                    } else
                    {
                        sw.Write("EXPORT " + key);
                    }
                    
                    if (where.Trim().Length != 0)
                    {
                        sw.WriteLine(" WHERE " + where + ";");
                    }
                    else
                    {
                        sw.WriteLine(";");
                    }
                }
                sw.WriteLine();
            }
            sw.Flush();
            sw.Close();

            sw = new StreamWriter(new FileStream(outputFolder + Path.DirectorySeparatorChar + scriptName + "_IMP.dms", FileMode.Create, FileAccess.Write, FileShare.None));
            sw.WriteLine("-- Rebuilt with DMSUtils -- ");
            sw.WriteLine();
            sw.WriteLine("SET INPUT " + scriptName + "_EXP.dat;");
            sw.WriteLine("SET LOG " + scriptName + "_IMP.log;");
            sw.WriteLine();
            foreach (var key in results.Keys)
            {
                sw.WriteLine("IMPORT " + key + ";");
                sw.WriteLine();
            }
            sw.Flush();
            sw.Close();

            Console.WriteLine("Script generation complete.");
        }
    }
}
