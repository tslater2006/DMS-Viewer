using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSFile
    {
        public string FileName;

        public string Version;
        public string Endian;
        public string BaseLanguage;
        public string Database;
        public string Started;
        public List<string> Namespaces = new List<string>();

        public byte[] FileMetadata;

        public List<DMSTable> Tables = new List<DMSTable>();
        public string Ended;

        
    }
}
