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
        public List<byte[]> Values = new List<byte[]>();
        public string GetStringValue(int index)
        {
            return Encoding.UTF8.GetString(Values[index]);
        }

        public string[] GetValuesAsString()
        {
            List<string> returnValues = new List<string>();
            foreach(var data in Values)
            {
                var utf8Enc = Encoding.UTF8.GetString(data);
                if (Encoding.UTF8.GetBytes(utf8Enc).Length == data.Length)
                {
                    /* UTF8 encoding seems to match original data */
                    returnValues.Add(utf8Enc);
                } else
                {
                    /* UTF8 doesn't seem to work, present as hex encoded binary */
                    returnValues.Add("{" + BitConverter.ToString(data).Replace("-", "") + "}");
                }
            }
            return returnValues.ToArray();
        }

        internal void WriteToStream(StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in Values)
            {
                /* For now, assume everything is "string" */
                var strEnc = DMSEncoder.EncodeData(v);
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
