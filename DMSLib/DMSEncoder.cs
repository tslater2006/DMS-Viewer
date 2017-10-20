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
            
                var startPos = LINE_LENGTH - 1;

                var endingChar = sb[startPos];

                /* Ending Char can either be a ',' for column data
                 * a '(' that is the start of a block 
                 * a ')' for the end of an encoded block
                 * a 'A' or 'B' for the start of an encoded block
                 * anything else for in the middle of a block 
                 */

                if (endingChar == ',' && sb[startPos-1] == ')')
                {
                    /* If we are on a comma, include it on the current line and then break */
                    fixedLine = sb.ToString(0, lineLength);
                    sb.Remove(0, lineLength);
                    lines.Add(fixedLine);
                }
                else if (endingChar == '(' && (sb[startPos-1] == 'A' || sb[startPos - 1] == 'B'))
                {
                    lineLength -= 2;
                    fixedLine = sb.ToString(0, lineLength);
                    sb.Remove(0, lineLength);
                    lines.Add(fixedLine);
                }
                else if (endingChar == ')' && sb[startPos - 1] != '\\')
                {
                    /* We are on a ) that isn't escaped, include it and then break */
                    fixedLine = sb.ToString(0, lineLength);
                    sb.Remove(0, lineLength);
                    lines.Add(fixedLine);
                }
                else if ((endingChar == 'A' || endingChar == 'B') && sb[startPos + 1] == '(')
                {
                    /* We are on an A or a B which is the start of a new block, we know this since ( is right after us, just cut the the line */
                    lineLength--;
                    fixedLine = sb.ToString(0, lineLength);
                    sb.Remove(0, lineLength);
                    lines.Add(fixedLine);
                } else
                {
                    /* We are inside a block, lets see if 1 more char ends the block or not */
                    if (sb[startPos + 1] == ')')
                    {
                        lineLength++;
                        fixedLine = sb.ToString(0, lineLength);
                        sb.Remove(0, lineLength);
                        lines.Add(fixedLine);
                    }
                    else
                    {

                        /* If not, we need to figure out if we are in a A() or a B() so we can determine how to split */
                        var lookingSpot = startPos;
                        while (sb[lookingSpot] != '(' && (sb[lookingSpot - 1] != 'A' || sb[lookingSpot - 1] != 'B'))
                        {
                            lookingSpot--;
                        }

                        var tagType = sb[lookingSpot - 1] == 'A' ? EncodeTags.ASCII : EncodeTags.BINARY;

                        if (tagType == EncodeTags.ASCII)
                        {
                            /* if its ASCII, cut, terminate, and reopen */
                            fixedLine = sb.ToString(0, lineLength) + ")";
                            sb.Remove(0, lineLength);
                            sb.Insert(0, "A(");
                            lines.Add(fixedLine);
                        }
                        else if (tagType == EncodeTags.BINARY)
                        {
                            /* its Binary, we need to cut but on an even # of letters */
                            var binLength = startPos - lookingSpot;
                            if (binLength % 2 == 1)
                            {
                                lineLength++;
                            }

                            fixedLine = sb.ToString(0, lineLength) + ")";
                            sb.Remove(0, lineLength);
                            sb.Insert(0, "B(");
                            lines.Add(fixedLine);
                        }
                    }
                }


            }
            if (sb.Length > 0)
            {
                lines.Add(sb.ToString());
            }

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
                    }
                    else if ((char)b == '\\')
                    {
                        sb.Append("\\\\");
                    }
                    else
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
