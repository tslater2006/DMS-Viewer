using DMS_Viewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export_Reconstructor
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();

            Console.Write("Enter the path to search for .DAT files: ");
            var dir = Console.ReadLine();

            var fileList = Directory.EnumerateFiles(dir, "*.dat",SearchOption.AllDirectories);

            FileStream fs = new FileStream("export.dms", FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(fs);
            DMSParser parser = new DMSParser();
            foreach (var file in fileList)
            {
                var dmsFile = parser.ParseFile(file);

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
            foreach (var key in results.Keys)
            {
                foreach (var where in results[key])
                {
                    sw.Write("EXPORT " + key);
                    if (where.Trim().Length != 0)
                    {
                        sw.WriteLine(" WHERE " + where + ";");
                    }else
                    {
                        sw.WriteLine(";");
                    }
                }
                sw.WriteLine();
            }
            sw.Flush();
            sw.Close();

            sw = new StreamWriter(new FileStream("import.dms", FileMode.Create, FileAccess.Write, FileShare.None));
            foreach (var key in results.Keys)
            {
                sw.WriteLine("IMPORT " + key + ";");
                sw.WriteLine();
            }
            sw.Flush();
            sw.Close();
        }
    }
}
