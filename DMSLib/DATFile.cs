using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DATFile
    {
        public string Version;
        public string Database;
        public string Started;
        public string FileName;

        List<string> DATHeaderText = new List<string>();
        List<string> DatHeaderEncoded = new List<string>();
        List<DATRecord> Records = new List<DATRecord>();

        public void LoadFromFile(string filePath)
        {
            FileName = new FileInfo(filePath).Name;

            var lines = File.ReadAllLines(filePath);
            List<List<string>> Chunks = new List<List<string>>();
            List<string> curList = new List<string>();
            Chunks.Add(curList);
            foreach (var l in lines)
            {
                if (l.Equals("/") == false)
                {
                    curList.Add(l);
                } else
                {
                    curList = new List<string>();
                    Chunks.Add(curList);
                }
            }
            DATRecord curRecord = null;
            for (var x = 0; x < Chunks.Count; x++)
            {
                if (x == 0)
                {
                    DATHeaderText = Chunks[x];
                } else if (x == 1)
                {
                    DatHeaderEncoded = Chunks[x];
                } else if (x < Chunks.Count - 1)
                {
                    /* record data, 4 chunks per record */
                    var normalizedX = (x - 2) % 4;
                    switch(normalizedX)
                    {
                        case 0:
                            curRecord = new DATRecord();
                            curRecord.ExportLines = Chunks[x];
                            break;
                        case 1:
                            curRecord.WorkspaceLines = Chunks[x];
                            break;
                        case 2:
                            curRecord.ColumnLines = Chunks[x];
                            break;
                        case 3:
                            curRecord.RowLines = Chunks[x];
                            Records.Add(curRecord);
                            curRecord = null;
                            break;
                    }
                }

            }

            /* Set Version */
            Version = DATHeaderText[0].Split(' ').Last();
            Database = DATHeaderText[4].Replace("REM Database: ", "");
            Started = DATHeaderText[5].Replace("REM Started: ", "");

            foreach (DATRecord rec in Records)
            {
                rec.Process();
                File.WriteAllBytes("C:\\users\\tslat\\desktop\\export_ws.bin", rec.WorkspaceBytes);
            }
        }
    }

    public class DATRecord
    {
        internal List<string> ExportLines = new List<string>();
        internal List<string> WorkspaceLines = new List<string>();
        internal List<string> ColumnLines = new List<string>();
        internal List<string> RowLines = new List<string>();

        internal byte[] WorkspaceBytes;

        public string Name;
        public string DBName;
        public string WhereClause;

        public string ExportStatement
        {
            get
            {
                return "EXPORT " + Name + " WHERE " + WhereClause;
            }
        }

        public void Process()
        {
            var nameParts = ExportLines[0].Replace("EXPORT ", "").Replace(" WHERE", "").Trim().Split('.');
            Name = nameParts[0];
            DBName = nameParts[1];
            /* Pull out Export Statement */
            for (var x = 1; x < ExportLines.Count; x++)
            {
                WhereClause += ExportLines[x];
            }

            WorkspaceBytes = DATUtils.DecodeBytes(WorkspaceLines);
        }
    }

    public class DATUtils
    {
        public static byte[] DecodeBytes(List<string> lines)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var l in lines)
            {
                sb.Append(l);
            }
            string dataString = sb.ToString();


            return DecodeBytes(dataString);
        }

        public static byte[] DecodeBytes(string data)
        {
            int leftOverValue = 0;
            MemoryStream ms = new MemoryStream();
            var curIndex = 0;

            while (curIndex < data.Length - 1)
            {
                if (data[curIndex] == 'A' && data[curIndex + 1] == '(')
                {
                    curIndex += 2;
                    /* ascii */
                    while (data[curIndex] != ')')
                    {
                        if (data[curIndex] == '\\')
                        {
                            /* escape char */
                            curIndex++;

                        }
                        ms.WriteByte((byte)data[curIndex++]);
                    }
                    /* skip closing paren */
                    curIndex++;
                    continue;
                }
                else if (data[curIndex] == 'B' && data[curIndex + 1] == '(')
                {
                    curIndex += 2;
                    /* binary */
                    StringBuilder byteString = new StringBuilder();
                    while (data[curIndex] != ')')
                    {
                        byteString.Append(data[curIndex++]);
                    }
                    curIndex++;
                    var chars = byteString.ToString().ToCharArray();

                    bool isUsingLeftover = leftOverValue != 0;

                    for (var x = 0; x < chars.Length - 1; x += 2)
                    {
                        var decodedValue = 0;
                        if (isUsingLeftover)
                        {
                            decodedValue = leftOverValue;
                        }
                        else
                        {
                            decodedValue = (chars[x] - 'A') << 4 | (chars[x + 1] - 'A');
                        }

                        if (decodedValue >= 194)
                        {
                            if (isUsingLeftover == false)
                            {
                                x += 2;
                            }
                            else
                            {
                                isUsingLeftover = false;
                                leftOverValue = 0;
                            }
                            if (x == chars.Length)
                            {
                                leftOverValue = decodedValue;
                                break;
                            }
                            var secondValue = (chars[x] - 'A') << 4 | (chars[x + 1] - 'A');

                            if (decodedValue > 194)
                            {
                                secondValue += (64 * (decodedValue - 194));
                            }
                            decodedValue = secondValue;
                        }
                        ms.WriteByte((byte)decodedValue);
                    }
                }
            }

            return ms.ToArray();
        }
    }
}
