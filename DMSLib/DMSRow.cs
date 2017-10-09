using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSRow
    {
        public List<string> Values = new List<string>();

        internal void WriteToStream(StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in Values)
            {
                /* For now, assume everything is "string" */
                var strEnc = DMSEncoder.EncodeData(Encoding.UTF8.GetBytes(v));
                sb.Append(strEnc).Append(",");
            }

            var encodedLines = DMSEncoder.FormatEncodedData(sb.ToString());

            foreach(var line in encodedLines)
            {
                sw.WriteLine(line);
            }

            sw.WriteLine("//");
        }
    }
}
