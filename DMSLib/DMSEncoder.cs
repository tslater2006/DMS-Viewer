using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            List<string> lines = new List<string>();

            /* Tokenize the string into "blocks" */
            BlockStack stack = new BlockStack(data);
            List<DataBlock> BlocksForLine = new List<DataBlock>();
            EncodeTags lastTypeOnLine = EncodeTags.NONE;

            while (stack.Count > 0)
            {
                BlocksForLine.Clear();
                int lineLength = 0;
                DataBlock currentBlock;

                if (lines.Count > 0)
                {
                    if (lastTypeOnLine != EncodeTags.COMMA && stack.Peek().Type != EncodeTags.COMMA && stack.Peek().Type != lastTypeOnLine)
                    {
                        /* different types, inject an empty of the previous type */
                        DataBlock dummy = new DataBlock() { Type = lastTypeOnLine, Contents = "()" };
                        BlocksForLine.Add(dummy);
                        lineLength += dummy.Length;
                    }
                }
                
                while (stack.Count > 0)
                {
                    currentBlock = stack.Pop();
                    if (lineLength + currentBlock.Length <= 70)
                    {
                        BlocksForLine.Add(currentBlock);
                        lineLength += currentBlock.Length;
                    } else
                    {
                        stack.Push(currentBlock);
                        break;
                    }
                }

                /* We've filled the line with as many full blocks as we can */

                /* If next block is a length of 4 for ASCII or 5 for BINARY, add it */
                if (lineLength < 70 && stack.Count > 0) {
                    var needsSplit = true;
                    switch (stack.Peek().Type)
                    {
                        case EncodeTags.ASCII:
                            if (stack.Peek().Length == 4)
                            {
                                BlocksForLine.Add(stack.Pop());
                                needsSplit = false;
                            }
                            break;
                        case EncodeTags.BINARY:
                            if (stack.Peek().Length == 5)
                            {
                                BlocksForLine.Add(stack.Pop());
                                needsSplit = false;
                            }
                            break;
                        case EncodeTags.COMMA:
                            BlocksForLine.Add(stack.Pop());
                            needsSplit = false;
                            break;
                    }


                    if (needsSplit)
                    {
                        /* determine if we can split it */

                        var blockToSplit = stack.Pop();
                        Tuple<DataBlock, DataBlock> splitParts = blockToSplit.Split(70 - lineLength);
                        if (splitParts.Item2 != null)
                        {
                            stack.Push(splitParts.Item2);
                        }
                        BlocksForLine.Add(splitParts.Item1);
                    }
                } else
                {
                    if (BlocksForLine.Sum(b => b.Length) == 70)
                    {
                        if (stack.Count > 0 && BlocksForLine.Last().Type == EncodeTags.COMMA)
                        {
                            var blockToSplit = stack.Pop();
                            Tuple<DataBlock, DataBlock> splitParts = blockToSplit.Split(70 - lineLength);
                            if (splitParts.Item2 != null)
                            {
                                stack.Push(splitParts.Item2);
                            }
                            BlocksForLine.Add(splitParts.Item1);
                        }
                    }
                }
                lastTypeOnLine = BlocksForLine.Last().Type;

                /* if the next token happens to be a comma, just add it?*/
                if (stack.Count > 0 && stack.Peek().Type == EncodeTags.COMMA)
                {
                    BlocksForLine.Add(stack.Pop());
                    lastTypeOnLine = EncodeTags.COMMA;
                }

                /* Process all blocks to string */
                StringBuilder line = new StringBuilder();
                foreach(DataBlock block in BlocksForLine)
                {
                    switch(block.Type)
                    {
                        case EncodeTags.ASCII:
                            line.Append("A");
                            break;
                        case EncodeTags.BINARY:
                            line.Append("B");
                            break;
                    }
                    line.Append(block.Contents);
                }
                var lineText = line.ToString();
                lines.Add(lineText);
                line.Clear();
                line = null;
            }



            return lines;
        }

        
        public static string EncodeData(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            EncodeTags currentTag = EncodeTags.NONE;

            foreach (byte b in data)
            {
                if (b >= 32 && b < 127 && (char)b != ']' && (char)b != '[')
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
            var lowerHalf = (char)((b & 0xF) + 65);
            var upperHalf = (char)(((b & 0xF0) >> 4) + 65);
            sb.Append(upperHalf).Append(lowerHalf);
            return sb.ToString();
        }

        

    }
    enum EncodeTags
    {
        ASCII, BINARY, NONE, COMMA
    }

    class DataBlock
    {
        public EncodeTags Type;
        public string Contents;

        public override string ToString()
        {

            switch (Type)
            {
                case EncodeTags.ASCII:
                    return "A" + Contents;
                case EncodeTags.BINARY:
                    return "B" + Contents;
                case EncodeTags.COMMA:
                    return Contents;
                default:
                    return "";
            }
        }

        public int Length
        {
            get
            {
                if (Type != EncodeTags.COMMA)
                {
                    return (Contents?.Length).GetValueOrDefault() + 1;
                } else
                {
                    return 1;
                }
            }
        }

        internal Tuple<DataBlock, DataBlock> Split(int leftSize)
        {
            /* ensure we split on even block */
            if (Type == EncodeTags.BINARY)
            {
                if (leftSize < 5)
                {
                    leftSize = 5;
                }
                else if ((leftSize - 1) % 2 == 1)
                {
                    leftSize++;
                }
            }

            if (Type == EncodeTags.ASCII)
            {
                if (leftSize < 4)
                {
                    leftSize = 4;
                }
            }
            DataBlock left = new DataBlock() { Type = Type };
            DataBlock right = new DataBlock() { Type = Type };
            left.Contents = Contents.Substring(0, leftSize - 2) + ")";

            /* We may have split on an escape character */
            /* left.Contents[left.Contents.Length - 2] == '\\' && left.Contents[left.Contents.Length - 3] != '\\'*/
            if (Type == EncodeTags.ASCII && HasIncompleteEscape(left.Contents.Substring(1,left.Contents.Length - 2)))
            {
                leftSize++;
                left.Contents = Contents.Substring(0, leftSize - 2) + ")";
            }
            right.Contents = "(" + Contents.Substring(leftSize - 2);
            if (right.Contents == "()")
            {
                right = null;
            }
            return Tuple.Create(left, right);
        }

        private bool HasIncompleteEscape(string content)
        {
            var escapeFound = false;
            foreach(var c in content)
            {
                if (c == '\\' && escapeFound)
                {
                    escapeFound = false;
                } else if (c == '\\' && escapeFound == false)
                {
                    escapeFound = true;
                } else if (escapeFound)
                {
                    escapeFound = false;
                }
            }
            return escapeFound;
        }
    }

    class BlockStack : Stack<DataBlock>
    {


        public BlockStack(string data)
        {
            List<DataBlock> pieces = new List<DataBlock>();
            for(var x = 0; x < data.Length;x++)
            {
                var curChar = data[x];
                DataBlock dataBlock = null;
                EncodeTags curBlockType;
                switch (curChar)
                {
                    case 'A':
                        /* We're at an ascii block */
            curBlockType = EncodeTags.ASCII;
                        break;
                    case 'B':
                        curBlockType = EncodeTags.BINARY;
                        break;
                    case ',':
                        curBlockType = EncodeTags.COMMA;
                        break;
                    default:
                        curBlockType = EncodeTags.NONE;
                        break;
                }

                dataBlock = new DataBlock() { Type = curBlockType };

                if (dataBlock.Type == EncodeTags.ASCII || dataBlock.Type == EncodeTags.BINARY)
                {
                    StringBuilder sb = new StringBuilder();
                    /* read the contents of this block */
                    var isOpen = true;
                    var foundEscape = false;
                    while (isOpen)
                    {
                        x++;
                        curChar = data[x];
                        sb.Append(curChar);
                        if (curChar == ')' && foundEscape == false)
                        {
                            isOpen = false;
                        }
                        if (foundEscape)
                        {
                            foundEscape = false;
                            continue;
                        }
                        if (curChar == '\\')
                        {
                            foundEscape = true;
                        }
                    }

                    dataBlock.Contents = sb.ToString();
                    sb.Clear();
                    sb = null;
                }
                else if (dataBlock.Type == EncodeTags.COMMA)
                {
                    dataBlock.Contents = ",";
                }

                pieces.Add(dataBlock);

                
            }
            pieces.Reverse();
            foreach (var block in pieces)
            {
                Push(block);
            }
        }
    }
}
