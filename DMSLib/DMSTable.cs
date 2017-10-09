using System;
using System.Collections.Generic;
using System.IO;
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
        public DMSRecordMetadata Metadata;
        public List<DMSColumn> Columns = new List<DMSColumn>();
        public List<DMSRow> Rows = new List<DMSRow>();
        public override string ToString()
        {
            return Name;
        }

        public void WriteToStream(StreamWriter sw)
        {
            sw.WriteLine("/");
            sw.WriteLine($"EXPORT  {Name}.{DBName} WHERE ");
            WriteWhereClause(sw, WhereClause);
            sw.WriteLine("/");

            /* Write table metadata */
            Metadata.WriteToStream(sw);

            /* Write the column info */
            WriteColumns(sw, Columns);

            /* Write each row of data */
            foreach (DMSRow row in Rows)
            {
                row.WriteToStream(sw);
                /* WriteTableRow(stream, row);
                stream.WriteLine("//");*/
            }
            sw.WriteLine("/");
        }

        private void WriteWhereClause(StreamWriter stream, string where)
        {
            /* TODO: Fix this? */
            stream.WriteLine(where);
        }

        private void WriteColumns(StreamWriter stream, List<DMSColumn> columns)
        {
            var maxLength = 70;
            var curLineLength = 0;
            stream.WriteLine("/");
            foreach (DMSColumn col in columns)
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
