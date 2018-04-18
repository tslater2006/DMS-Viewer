using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSFile
    {
        public string FileName;
        public string BlankLine;
        public string Version;
        public string Endian;
        public string BaseLanguage;
        public string Database;
        public string Started;
        public List<string> Namespaces = new List<string>();

        public byte[] FileMetadata;

        public List<DMSTable> Tables = new List<DMSTable>();
        public string Ended;

        public void WriteToStream(StreamWriter sw)
        {
            /* Write out the header */
            sw.WriteLine($"SET VERSION_DAM  {Version}");
            sw.WriteLine(BlankLine);
            sw.WriteLine($"SET ENDIAN {Endian}");
            sw.WriteLine($"SET BASE_LANGUAGE {BaseLanguage}");
            sw.WriteLine($"REM Database: {Database}");
            sw.WriteLine($"REM Started: {Started}");

            /* Write out the namespaces */
            sw.WriteLine("EXPORT  RECORD/SPACE.x");
            foreach (var space in Namespaces)
            {
                sw.WriteLine(space);
            }
            sw.WriteLine("/");

            var metadataLines = DMSEncoder.EncodeDataToLines(FileMetadata);
            foreach(var line in metadataLines)
            {
                sw.WriteLine(line);
            }
            sw.WriteLine("/");
            foreach (var table in Tables)
            {
                table.WriteToStream(sw);
            }
            sw.WriteLine($"REM Ended: {Ended}");

        }

    }
}
