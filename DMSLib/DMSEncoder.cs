using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSEncoder
    {
        public static List<string> EncodeDataToLines(byte[] data)
        {
            return FormatEncodedData(EncodeData(data));
        }

        public static List<string> FormatEncodedData(string data)
        {
            StringBuilder sb = new StringBuilder(data);
            var LINE_LENGTH = 70;

            /* Split this into lines of LINE_LENGTH */
            string fixedLine = "";
            List<string> lines = new List<string>();
            while (sb.Length > LINE_LENGTH)
            {
                LINE_LENGTH = 70;

                var lineLength = LINE_LENGTH;
                /* Need to find if we are in an A tag or a B tag */
                var startPos = LINE_LENGTH - 1;

                var endingChar = sb[startPos];

                if (endingChar == ',')
                {
                    fixedLine = sb.ToString(0, LINE_LENGTH);
                    lines.Add(fixedLine);
                    sb.Remove(0, LINE_LENGTH);
                }
                else if (endingChar == ')')
                {
                    fixedLine = sb.ToString(0, LINE_LENGTH + 1);
                    lines.Add(fixedLine);
                    sb.Remove(0, LINE_LENGTH + 1);
                }
                else if (endingChar == '(')
                {
                    fixedLine = sb.ToString(0, LINE_LENGTH - 2);
                    lines.Add(fixedLine);
                    sb.Remove(0, LINE_LENGTH - 2);
                }
                else
                {
                    if ((endingChar == 'A' || endingChar == 'B') && (sb[startPos - 1] == ',' || sb[startPos-1] == ')'))
                    {
                        fixedLine = sb.ToString(0, LINE_LENGTH - 1);
                        lines.Add(fixedLine);
                        sb.Remove(0, LINE_LENGTH - 1);
                    }
                    else
                    {
                        while (sb[startPos] != '(')
                        {
                            startPos--;
                        }

                        var tagType = sb[startPos - 1] == 'A' ? EncodeTags.ASCII : EncodeTags.BINARY;
                        var lengthAdd = 0;
                        if (sb[LINE_LENGTH - 2] == '(')
                        {
                            /* we dont want to end with A() */
                            lengthAdd = (tagType == EncodeTags.ASCII ? 1 : 2);
                        }
                        if (sb[LINE_LENGTH] == ')')
                        {
                            /* Allow it to close the encoding */
                            lengthAdd++;
                            tagType = EncodeTags.NONE;
                        }
                        if (sb.Length > LINE_LENGTH + 1 && sb[LINE_LENGTH + 1] == ',')
                        {
                            /* Allow trailing commas on the line */
                            lengthAdd++;
                            tagType = EncodeTags.NONE;
                        }

                        LINE_LENGTH += lengthAdd;
                        fixedLine = sb.ToString(0, LINE_LENGTH - 1) + (tagType != EncodeTags.NONE ? ")" : "");
                        lines.Add(fixedLine);
                        sb.Remove(0, LINE_LENGTH - 1);
                        if (tagType != EncodeTags.NONE)
                        {
                            sb.Insert(0, tagType == EncodeTags.ASCII ? "A(" : "B(");
                        }
                    }
                }

            }
            lines.Add(sb.ToString());

            return lines;
        }


        public static string EncodeData(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            EncodeTags currentTag = EncodeTags.NONE;

            foreach (byte b in data)
            {
                if (b >= 32 && b <= 127 && (char)b != ']' && (char)b != '[')
                {
                    /* we'll treat this as an ascii character */
                    switch (currentTag)
                    {
                        case EncodeTags.NONE:
                            sb.Append("A(");
                            currentTag = EncodeTags.ASCII;
                            break;
                        case EncodeTags.BINARY:
                            sb.Append(")A(");
                            currentTag = EncodeTags.ASCII;
                            break;
                        case EncodeTags.ASCII:
                            break;
                    }
                    if ((char)b == ')' || (char)b == '(')
                    {
                        sb.Append($"\\{(char)b}");
                    }else
                    {
                        sb.Append((char)b);
                    }
                    
                }
                else
                {
                    /* we'll treat this as a binary */
                    switch (currentTag)
                    {
                        case EncodeTags.NONE:
                            sb.Append("B(");
                            currentTag = EncodeTags.BINARY;
                            break;
                        case EncodeTags.ASCII:
                            sb.Append(")B(");
                            currentTag = EncodeTags.BINARY;
                            break;
                        case EncodeTags.BINARY:
                            break;
                    }
                    sb.Append(EncodeByte(b));
                }

            }

            if (currentTag != EncodeTags.NONE)
            {
                sb.Append(")");
            }

            return sb.ToString();
        }

        private static string EncodeByte(byte b)
        {
            StringBuilder sb = new StringBuilder();
            /*if (b < 194)
            {*/
                var lowerHalf = (char)((b & 0xF) + 65);
                var upperHalf = (char)(((b & 0xF0) >> 4) + 65);
                sb.Append(upperHalf).Append(lowerHalf);
            /*}
            else
            {
                var firstNumber = 194 + (b / 64);
                var secondNumber = b - (64 * (firstNumber - 194));

                var firstLower = (char)((firstNumber & 0xF) + 65);
                var firstUpper = (char)(((firstNumber & 0xF0) >> 4) + 65);

                var secondLower = (char)((secondNumber & 0xF) + 65);
                var secondUpper = (char)(((secondNumber & 0xF0) >> 4) + 65);

                sb.Append(firstUpper).Append(firstLower).Append(secondUpper).Append(secondLower);
            }*/

            return sb.ToString();
        }

        enum EncodeTags
        {
            ASCII, BINARY, NONE
        }

    }
}
