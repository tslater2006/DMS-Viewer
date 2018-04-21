using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSRowDecoder
    {
        List<int> indexes = new List<int>();
        DecoderState currentState = DecoderState.NO_BLOCK;
        MemoryStream ms = new MemoryStream();

        public DMSRowDecoder()
        {
            indexes.Add(0);
        }

        public void Reset()
        {
            indexes.Clear();
            indexes.Add(0);
            ms = new MemoryStream();
        }

        public void DecodeLine(string str)
        {
            char binChar1 = '\0';
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
                        else if (str[x] == ',')
                        {
                            indexes.Add((int)ms.Length);
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
        }

        public void Finish(DMSRow row)
        {
            row.Values = ms.ToArray();
            row.Indexes = indexes.ToArray();
            indexes.Clear();
            ms.Dispose();
        }
    }
}
