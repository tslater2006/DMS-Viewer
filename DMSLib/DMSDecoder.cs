using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMSLib
{
    enum DecoderState
    {
        NO_BLOCK,ASCII_BLOCK,BINARY_BLOCK,ESCAPE_CHAR, FIRST_BIN_CHAR
    }
    public class DMSDecoder
    {
        public static Stopwatch timer = new Stopwatch();
        static DecoderState currentState = DecoderState.NO_BLOCK;
        public static byte[] DecodeString(string str)
        {
            timer.Start();

            char binChar1 = '\0';
            MemoryStream ms = new MemoryStream();
            for (var x = 0; x < str.Length; x++)
            {
                switch (currentState)
                {
                    case DecoderState.NO_BLOCK:
                        if (str[x] == 'A')
                        {
                            currentState = DecoderState.ASCII_BLOCK;
                        }
                        else if (str[x] == 'B')
                        {
                            currentState = DecoderState.BINARY_BLOCK;
                        }
                        else
                        {
                            throw new Exception("Unexpected char: " + str[x] + " at start of block");
                        }
                        break;
                    case DecoderState.ASCII_BLOCK:
                        if (str[x] == '(')
                        {
                            x++;
                        }
                        if (str[x] == ')')
                        {
                            currentState = DecoderState.NO_BLOCK;
                            continue;
                        }
                        if (str[x] == '\\')
                        {
                            currentState = DecoderState.ESCAPE_CHAR;
                        }
                        else
                        {
                            ms.WriteByte((byte)str[x]);
                        }

                        break;
                    case DecoderState.BINARY_BLOCK:
                        if (str[x] == '(')
                        {
                            x++;
                        }
                        if (str[x] == ')')
                        {
                            currentState = DecoderState.NO_BLOCK;
                            continue;
                        }
                        binChar1 = str[x];
                        currentState = DecoderState.FIRST_BIN_CHAR;
                        break;
                    case DecoderState.FIRST_BIN_CHAR:
                        var binChar2 = str[x];
                        ms.WriteByte((byte)((binChar1 - 'A') << 4 | (binChar2 - 'A')));
                        currentState = DecoderState.BINARY_BLOCK;
                        break;
                    case DecoderState.ESCAPE_CHAR:
                        ms.WriteByte((byte)str[x]);
                        currentState = DecoderState.ASCII_BLOCK;
                        break;
                }
            }



            /* We need to get a list of Encoding blocks from the string */


            /*Regex encodingGroup = new Regex("[A]\\(([^\\(\\)\\\\]|\\\\\\(|\\\\\\)|\\\\\\\\)*\\)|[B]\\([A-Z]*\\)");
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
            }*/



            timer.Stop();
            var bytes = ms.ToArray();
            return bytes;
        }
    }


    class DMSEncodedBlock {
        public string Contents;
        public EncodedBlockType Type;

        public byte[] GetBytes()
        {
            if (Type == EncodedBlockType.ASCII)
            {
                var unescaped = Contents.Replace("\\(", "(");
                unescaped = unescaped.Replace("\\)", ")");
                unescaped = unescaped.Replace("\\\\", "\\");

                return ASCIIEncoding.ASCII.GetBytes(unescaped);
            }

            else
            {
                MemoryStream ms = new MemoryStream();
                for (var x = 0; x < Contents.Length - 1; x += 2)
                {
                    var decodedValue = 0;
                    decodedValue = (Contents[x] - 'A') << 4 | (Contents[x + 1] - 'A');

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
