using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSRecordMetadata
    {
        public string RecordLanguage;
        public string OwnerID;
        public string AnalyticDeleteRecord;
        public string ParentRecord;
        public string RecordName;
        public string RelatedLanguageRecord;
        public string RecordDBName;
        /* Unknown 76 bytes, seems to be all 0's */
        public byte[] Unknown1;

        public string OptimizationTriggers;

        /* 10 bytes unknown 00 00 01 00 00 00 00 00 00 00 */
        public byte[] Unknown2;

        public int VersionNumber;
        public int FieldCount;
        public int BuildSequence;
        public int IndexCount;

        /* 4 bytes unknown */
        public int Unknown3;

        public int VersionNumber2;

        /* 22 bytes unknown*/
        public byte[] Unknown4;

        /* List of Field Info structures */
        public List<DMSRecordFieldMetadata> FieldMetadata = new List<DMSRecordFieldMetadata>();

        /* This contains the "indexes" for the record */
        public byte[] LeftoverData;

        private string FromUnicodeBytes(byte[] data)
        {
            var str = Encoding.Unicode.GetString(data);
            var nullIndex = str.IndexOf('\0');
            str = str.Substring(0, nullIndex);

            return str;
        }

        public DMSRecordMetadata(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));
            RecordLanguage = FromUnicodeBytes(br.ReadBytes(8));
            OwnerID = FromUnicodeBytes(br.ReadBytes(10));
            AnalyticDeleteRecord = FromUnicodeBytes(br.ReadBytes(32));
            ParentRecord = FromUnicodeBytes(br.ReadBytes(32));
            RecordName = FromUnicodeBytes(br.ReadBytes(32));
            RelatedLanguageRecord = FromUnicodeBytes(br.ReadBytes(32));
            RecordDBName = FromUnicodeBytes(br.ReadBytes(38));

            /* Unknwon 76 bytes */
            Unknown1 = br.ReadBytes(76);
            OptimizationTriggers = FromUnicodeBytes(br.ReadBytes(4));

            /* Unknown 10 bytes */
            Unknown2 = br.ReadBytes(10);

            VersionNumber = BitConverter.ToInt32(br.ReadBytes(4), 0);
            FieldCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
            BuildSequence = BitConverter.ToInt32(br.ReadBytes(4), 0);
            IndexCount = BitConverter.ToInt32(br.ReadBytes(4), 0);

            /* Unknown 4 bytes */
            Unknown3 = BitConverter.ToInt32(br.ReadBytes(4), 0);

            VersionNumber2 = BitConverter.ToInt32(br.ReadBytes(4), 0);

            /* Unknown 22 bytes */
            Unknown4 = br.ReadBytes(22);

            byte[] fieldMetadata; 

            for (var x = 0; x < FieldCount; x++)
            {
                fieldMetadata = br.ReadBytes(106);
                FieldMetadata.Add(new DMSRecordFieldMetadata(fieldMetadata));
            }

            var leftOverCount = data.Length - br.BaseStream.Position;
            LeftoverData = new byte[leftOverCount];
            br.Read(LeftoverData, 0, LeftoverData.Length);

        }
    }

    public class DMSRecordFieldMetadata
    {
        public string FieldName;
        public string RecordName;
        public int Unknown1;
        public int Unknown2;
        public int DecimalPositions;
        public int Unknown3;
        public int Unknown4;
        public short FieldFormat;
        public int FieldLength;
        public int DefaultGUIControl;
        public int Unknown5;
        public short Unknown6;

        public DMSRecordFieldMetadata(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));
            FieldName = FromUnicodeBytes(br.ReadBytes(38));
            RecordName = FromUnicodeBytes(br.ReadBytes(32));
            Unknown1 = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Unknown2 = BitConverter.ToInt32(br.ReadBytes(4), 0);
            DecimalPositions = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Unknown3 = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Unknown4 = BitConverter.ToInt32(br.ReadBytes(4), 0);
            FieldFormat = BitConverter.ToInt16(br.ReadBytes(2), 0);
            FieldLength = BitConverter.ToInt32(br.ReadBytes(4), 0);
            DefaultGUIControl = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Unknown5 = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Unknown6 = BitConverter.ToInt16(br.ReadBytes(2), 0);
        }


        private string FromUnicodeBytes(byte[] data)
        {
            var str = Encoding.Unicode.GetString(data);
            var nullIndex = str.IndexOf('\0');
            str = str.Substring(0, nullIndex);
            return str;
        }
    }
}
