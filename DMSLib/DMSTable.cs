using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSTable
    {
        public string Name;
        public string DBName;
        public string WhereClause;
        public List<string> MetadataLines = new List<string>();
        public DMSRecordMetadata Metadata;
        public List<DMSColumn> Columns = new List<DMSColumn>();
        public List<DMSRow> Rows = new List<DMSRow>();
        public override string ToString()
        {
            return Name;
        }
    }
}
