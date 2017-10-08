using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSDecoder
    {
        public static byte[] DecodeString(string str)
        {
            /* We need to get a list of Encoding blocks from the string */

            /* Regex: [A]\(([^\(\)\\]|\\\(|\\\))*\)|[B]\([A-Z]*\) */
            Regex encodingGroup = new Regex("[A]\\(([^\\(\\)\\\\]|\\\\\\(|\\\\\\))*\\)|[B]\\([A-Z]*\\)");
            var matchList = encodingGroup.Matches(str);

            List<DMSEncodedBlock> encodedBlocks = new List<DMSEncodedBlock>();

            foreach( Match match in matchList)
            {
                DMSEncodedBlock encodingBlock = new DMSEncodedBlock();

                if (match.Value.StartsWith("A"))
                {
                    encodingBlock.Type = EncodedBlockType.ASCII;                    
                } else
                {
                    encodingBlock.Type = EncodedBlockType.BINARY;
                }

                encodingBlock.Contents = match.Value.Substring(2, match.Value.Length - 3);

                /* If previous block type == this one, append contents to previous */

                if (encodedBlocks.Count > 0 && encodedBlocks.Last().Type == encodingBlock.Type)
                {
                    encodedBlocks.Last().Contents += encodingBlock.Contents;
                } else
                {
                    encodedBlocks.Add(encodingBlock);
                }
            }

            MemoryStream ms = new MemoryStream();

            foreach (DMSEncodedBlock block in encodedBlocks)
            {
                byte[] blockBytes = block.GetBytes();
                ms.Write(blockBytes, 0, blockBytes.Length);
            }

            return ms.ToArray();
        }
    }


    class DMSEncodedBlock {
        public string Contents;
        public EncodedBlockType Type;

        public byte[] GetBytes()
        {
            if (Type == EncodedBlockType.ASCII)
            {
                return ASCIIEncoding.ASCII.GetBytes(Contents);
            }

            else
            {
                MemoryStream ms = new MemoryStream();
                for (var x = 0; x < Contents.Length - 1; x += 2)
                {
                    var decodedValue = 0;
                    decodedValue = (Contents[x] - 'A') << 4 | (Contents[x + 1] - 'A');

                    if (decodedValue >= 194)
                    {
                        var secondValue = (Contents[x] - 'A') << 4 | (Contents[x + 1] - 'A');

                        if (decodedValue > 194)
                        {
                            secondValue += (64 * (decodedValue - 194));
                        }
                        decodedValue = secondValue;
                    }
                    ms.WriteByte((byte)decodedValue);
                }
                return ms.ToArray();
            }
        }
    }

    enum EncodedBlockType
    {
        ASCII, BINARY
    }
}
