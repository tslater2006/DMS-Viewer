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

        internal void WriteToStream(StreamWriter sw)
        {

            var lines = DMSEncoder.EncodeDataToLines(GetBytes());
            foreach(var line in lines)
            {
                sw.WriteLine(line);
            }
            MemoryStream ms = new MemoryStream();
            foreach (DMSRecordFieldMetadata fieldmeta in FieldMetadata)
            {
                byte[] data = fieldmeta.GetBytes();
                ms.Write(data, 0, data.Length);
            }

            lines = DMSEncoder.EncodeDataToLines(ms.ToArray());
            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }

            ms = new MemoryStream();
            foreach (DMSRecordIndexMetadata idx in Indexes)
            {
                byte[] headerBytes = idx.GetHeaderBytes();
                ms.Write(headerBytes, 0, headerBytes.Length);
            }

            lines = DMSEncoder.EncodeDataToLines(ms.ToArray());
            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }

            foreach (DMSRecordIndexMetadata idx in Indexes)
            {
                byte[] fieldsBytes = idx.GetFieldsBytes();

                lines = DMSEncoder.EncodeDataToLines(fieldsBytes);
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }

            

            ms = new MemoryStream();

            byte[] tableSpaceName = new byte[64];
            byte[] dbName = new byte[18];

            Encoding.Unicode.GetBytes(TableSpaceName, 0, TableSpaceName.Length, tableSpaceName, 0);
            Encoding.Unicode.GetBytes(DBName, 0, DBName.Length, dbName, 0);

            ms.Write(tableSpaceName, 0, tableSpaceName.Length);
            ms.Write(dbName, 0, dbName.Length);

            lines = DMSEncoder.EncodeDataToLines(ms.ToArray());
            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }
        }

        /* 4 bytes unknown */
        public int Unknown3;

        public int VersionNumber2;

        /* 22 bytes unknown*/
        public byte[] Unknown4;

        /* List of Field Info structures */
        public List<DMSRecordFieldMetadata> FieldMetadata = new List<DMSRecordFieldMetadata>();

        public List<DMSRecordIndexMetadata> Indexes = new List<DMSRecordIndexMetadata>();

        public string TableSpaceName;
        public string DBName;

        /* This contains the "indexes" for the record (if any), as well as tablespace info for the record */
        public byte[] LeftoverData;

        private byte[] GetBytes()
        {
            MemoryStream ms = new MemoryStream();

            byte[] recordLang = new byte[8];
            Encoding.Unicode.GetBytes(RecordLanguage, 0, RecordLanguage.Length, recordLang, 0);

            byte[] ownerId = new byte[10];
            Encoding.Unicode.GetBytes(OwnerID, 0, OwnerID.Length, ownerId, 0);

            byte[] analyticDelete = new byte[32];
            Encoding.Unicode.GetBytes(AnalyticDeleteRecord, 0, AnalyticDeleteRecord.Length, analyticDelete, 0);

            byte[] parentRecord = new byte[32];
            Encoding.Unicode.GetBytes(ParentRecord, 0, ParentRecord.Length, parentRecord, 0);

            byte[] recName = new byte[32];
            Encoding.Unicode.GetBytes(RecordName, 0, RecordName.Length, recName, 0);

            byte[] relLang = new byte[32];
            Encoding.Unicode.GetBytes(RelatedLanguageRecord, 0, RelatedLanguageRecord.Length, relLang, 0);

            byte[] recDbName = new byte[38];
            Encoding.Unicode.GetBytes(RecordDBName, 0, RecordDBName.Length, recDbName, 0);

            byte[] optTriggers = new byte[4];
            Encoding.Unicode.GetBytes(OptimizationTriggers, 0, OptimizationTriggers.Length, optTriggers, 0);

            ms.Write(recordLang, 0, recordLang.Length);
            ms.Write(ownerId, 0, ownerId.Length);
            ms.Write(analyticDelete, 0, analyticDelete.Length);
            ms.Write(parentRecord, 0, parentRecord.Length);
            ms.Write(recName, 0, recName.Length);
            ms.Write(relLang, 0, relLang.Length);
            ms.Write(recDbName, 0, recDbName.Length);

            ms.Write(Unknown1, 0, Unknown1.Length);
            ms.Write(optTriggers, 0, optTriggers.Length);
            ms.Write(Unknown2, 0, Unknown2.Length);

            ms.Write(BitConverter.GetBytes(VersionNumber), 0, 4);
            ms.Write(BitConverter.GetBytes(FieldCount), 0, 4);
            ms.Write(BitConverter.GetBytes(BuildSequence), 0, 4);
            ms.Write(BitConverter.GetBytes(IndexCount), 0, 4);
            ms.Write(BitConverter.GetBytes(Unknown3), 0, 4);
            ms.Write(BitConverter.GetBytes(VersionNumber2), 0, 4);
            ms.Write(Unknown4, 0, Unknown4.Length);


            return ms.ToArray();
        }

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

            /* Read in the Index headers (if any) */
            for (var x = 0; x < IndexCount; x++)
            {
                var indexHeaderData = br.ReadBytes(40);
                var index = new DMSRecordIndexMetadata(indexHeaderData);
                Indexes.Add(index);
            }

            foreach(var index in Indexes)
            {
                for (var x = 0; x < index.FieldCount; x++)
                {
                    var fieldInfo = new DMSRecordIndexField(br.ReadBytes(48));
                    index.Fields.Add(fieldInfo);
                }
            }

            TableSpaceName = FromUnicodeBytes(br.ReadBytes(64));
            DBName = FromUnicodeBytes(br.ReadBytes(18));

            br.Close();
        }
    }

    public class DMSRecordFieldMetadata
    {
        public string FieldName;
        public string RecordName;
        public int Unknown1;
        public int VersionNumber;
        public int DecimalPositions;

        /* Something to do with Family Name and Display Name used with "Custom" format type*/
        public short Unknown2;
        public UseEditFlags UseEditMask;

        public FieldTypes FieldType;
        public FieldFormats FieldFormat;
        public int FieldLength;
        public GUIControls DefaultGUIControl;

        public int Unknown5;
        public short Unknown6;

        public DMSRecordFieldMetadata(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));
            FieldName = FromUnicodeBytes(br.ReadBytes(38));
            RecordName = FromUnicodeBytes(br.ReadBytes(32));
            Unknown1 = BitConverter.ToInt32(br.ReadBytes(4), 0);
            VersionNumber = BitConverter.ToInt32(br.ReadBytes(4), 0);
            DecimalPositions = BitConverter.ToInt32(br.ReadBytes(4), 0);

            UseEditMask = (UseEditFlags)BitConverter.ToInt32(br.ReadBytes(4), 0);

            Unknown2 = BitConverter.ToInt16(br.ReadBytes(2), 0);
            
            FieldType = (FieldTypes)BitConverter.ToInt16(br.ReadBytes(2), 0);
            FieldFormat = (FieldFormats)BitConverter.ToInt16(br.ReadBytes(2), 0);
            FieldLength = BitConverter.ToInt32(br.ReadBytes(4), 0);
            DefaultGUIControl = (GUIControls)BitConverter.ToInt32(br.ReadBytes(4), 0);

            Unknown5 = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Unknown6 = BitConverter.ToInt16(br.ReadBytes(2), 0);

            br.Close();
        }

        public DMSRecordFieldMetadata(DMSNewColumn newColumn, DMSTable table)
        {
            FieldName = newColumn.FieldName;
            RecordName = table.Name;
            Unknown1 = 0;
            VersionNumber = newColumn.VersionNumber;
            DecimalPositions = newColumn.DecimalPositions;

            UseEditMask = newColumn.UseEditMask;

            Unknown2 = 0;

            FieldType = newColumn.FieldType;
            FieldFormat = newColumn.FieldFormat;
            FieldLength = newColumn.FieldLength;
            DefaultGUIControl = newColumn.DefaultGUIControl;

            Unknown5 = 0;
            Unknown6 = 0;
        }

        internal byte[] GetBytes()
        {
            MemoryStream ms = new MemoryStream();

            byte[] fieldName = new byte[38];
            byte[] recordName = new byte[32];

            Encoding.Unicode.GetBytes(FieldName, 0, FieldName.Length, fieldName, 0);
            Encoding.Unicode.GetBytes(RecordName, 0, RecordName.Length, recordName, 0);

            ms.Write(fieldName, 0, fieldName.Length);
            ms.Write(recordName, 0, recordName.Length);

            ms.Write(BitConverter.GetBytes(Unknown1), 0, 4);
            ms.Write(BitConverter.GetBytes(VersionNumber), 0, 4);
            ms.Write(BitConverter.GetBytes(DecimalPositions), 0, 4);
            ms.Write(BitConverter.GetBytes((int)UseEditMask), 0, 4);
            ms.Write(BitConverter.GetBytes(Unknown2), 0, 2);
            ms.Write(BitConverter.GetBytes((short)FieldType), 0, 2);
            ms.Write(BitConverter.GetBytes((short)FieldFormat), 0, 2);
            ms.Write(BitConverter.GetBytes(FieldLength), 0, 4);
            ms.Write(BitConverter.GetBytes((int)DefaultGUIControl), 0, 4);
            ms.Write(BitConverter.GetBytes(Unknown5), 0, 4);
            ms.Write(BitConverter.GetBytes(Unknown6), 0, 2);

            return ms.ToArray();
        }

        private string FromUnicodeBytes(byte[] data)
        {
            var str = Encoding.Unicode.GetString(data);
            var nullIndex = str.IndexOf('\0');
            str = str.Substring(0, nullIndex);
            return str;
        }
    }

    public class DMSRecordIndexMetadata
    {
        public string IndexID; /* 2 bytes */
        public short FieldCount; /* 2 bytes */
        public int Unknown1; /* 4 bytes */
        public short Unknown2; /* 2 bytes */
        public RecordIndexTypes IndexType; /* 2 bytes */
        public short Unique; /* 2 bytes */
        public short Cluster; /* 2 bytes */
        public short Active; /* 2 bytes */
        public short PlatformSBS; /* 2 bytes */
        public short PlatformDB2; /* 2 bytes */
        public short PlatformORA; /* 2 bytes */
        public short PlatformINF; /* 2 bytes */
        public short PlatformDBX; /* 2 bytes */
        public short PlatformALB; /* 2 bytes */
        public short PlatformSYB; /* 2 bytes */
        public short PlatformMSS; /* 2 bytes */
        public short PlatformDB4; /* 2 bytes */
        public int Unknown3; /* 4 bytes */

        public List<DMSRecordIndexField> Fields = new List<DMSRecordIndexField>();

        public DMSRecordIndexMetadata(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));

            IndexID = Encoding.Unicode.GetString(br.ReadBytes(2));
            FieldCount = BitConverter.ToInt16(br.ReadBytes(2), 0);
            Unknown1 = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Unknown2 = BitConverter.ToInt16(br.ReadBytes(2), 0);
            IndexType = (RecordIndexTypes)BitConverter.ToInt16(br.ReadBytes(2), 0);
            Unique = BitConverter.ToInt16(br.ReadBytes(2), 0);
            Cluster = BitConverter.ToInt16(br.ReadBytes(2), 0);
            Active = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformSBS = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformDB2 = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformORA = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformINF = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformDBX = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformALB = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformSYB = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformMSS = BitConverter.ToInt16(br.ReadBytes(2), 0);
            PlatformDB4 = BitConverter.ToInt16(br.ReadBytes(2), 0);
            Unknown3 = BitConverter.ToInt32(br.ReadBytes(4), 0);

            br.Close();
        }

        internal byte[] GetHeaderBytes()
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(Encoding.Unicode.GetBytes(IndexID), 0, 2);
            ms.Write(BitConverter.GetBytes(FieldCount), 0, 2);
            ms.Write(BitConverter.GetBytes(Unknown1), 0, 4);
            ms.Write(BitConverter.GetBytes(Unknown2), 0, 2);
            ms.Write(BitConverter.GetBytes((short)IndexType), 0, 2);
            ms.Write(BitConverter.GetBytes(Unique), 0, 2);
            ms.Write(BitConverter.GetBytes(Cluster), 0, 2);
            ms.Write(BitConverter.GetBytes(Active), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformSBS), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformDB2), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformORA), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformINF), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformDBX), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformALB), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformSYB), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformMSS), 0, 2);
            ms.Write(BitConverter.GetBytes(PlatformDB4), 0, 2);
            ms.Write(BitConverter.GetBytes(Unknown3), 0, 4);

            return ms.ToArray();
        }

        internal byte[] GetFieldsBytes()
        {
            MemoryStream ms = new MemoryStream();


            foreach(DMSRecordIndexField field in Fields)
            {
                byte[] fieldBytes = field.GetBytes();
                ms.Write(fieldBytes, 0, fieldBytes.Length);
            }

            return ms.ToArray();
        }
    }

    public class DMSRecordIndexField
    {
        public string FieldName;
        public int KeyPosition;
        public int Ascending;
        public short Unknown3;

        public DMSRecordIndexField(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));

            FieldName = FromUnicodeBytes(br.ReadBytes(38));
            KeyPosition = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Ascending = BitConverter.ToInt32(br.ReadBytes(4), 0);
            Unknown3 = BitConverter.ToInt16(br.ReadBytes(2), 0);

            br.Close();
        }

        internal byte[] GetBytes()
        {
            MemoryStream ms = new MemoryStream();

            byte[] fieldName = new byte[38];
            Encoding.Unicode.GetBytes(FieldName, 0, FieldName.Length, fieldName, 0);

            ms.Write(fieldName, 0, fieldName.Length);
            ms.Write(BitConverter.GetBytes(KeyPosition), 0, 4);
            ms.Write(BitConverter.GetBytes(Ascending), 0, 4);
            ms.Write(BitConverter.GetBytes(Unknown3), 0, 2);

            return ms.ToArray();
        }

        private string FromUnicodeBytes(byte[] data)
        {
            var str = Encoding.Unicode.GetString(data);
            var nullIndex = str.IndexOf('\0');
            str = str.Substring(0, nullIndex);
            return str;
        }
    }

    public enum FieldFormats : short
    {
        UPPER_OR_DEFAULT = 0,
        NAME = 1,
        PHONE_NORTH_AMERICA = 2,
        ZIP_NORTH_AMERICA = 3,
        SSN = 4,
        MIXEDCASE = 6,
        RAW_BINARY = 7,
        NUMBERS_ONLY = 8,
        SIN = 9,
        PHONE_INTL = 10,
        ZIP_INTL = 11,
        TIME_SECONDS = 12,
        TIME_MILLI = 13,
        CUSTOM = 14

    }

    public enum FieldTypes : short
    {
        CHAR = 0,
        LONG_CHAR = 1,
        NUMBER = 2,
        SIGNED_NUMBER = 3,
        DATE = 4,
        TIME = 5,
        DATETIME = 6,
        IMG_OR_ATTACH = 8,
        IMAGE_REF = 9,
    }

    public enum GUIControls : int
    {
        EDIT_BOX = 4,
        DROPDOWN = 5,
        CHECKBOX = 7,
        RADIO_BTN = 8,
        DEFAULT = 99,
    }

    [Flags]
    public enum UseEditFlags : uint
    {
        KEY = 1,
        DUP_ORDER_KEY = 1 << 1,
        SYS_MAINT = 1 << 2,
        AUD_FLD_ADD = 1 << 3,
        ALT_SRCH_KEY = 1 <<4,
        LIST_BOX_ITEM = 1 << 5,
        DESCEND_KEY = 1 << 6,
        AUD_FLD_CHG = 1 << 7,
        REQUIRED = 1 << 8,
        XLAT_EDIT = 1 << 9,
        AUD_FLD_DEL = 1 << 10,
        SRCH_KEY = 1 << 11,
        REASONABLE_DATE_EDIT = 1 << 12,
        YES_NO_EDIT = 1 << 13,
        PROMPT_EDIT = 1 << 14,
        AUTO_UPDATE = 1 << 15,
        UNKNOWN1 = 1 << 16,
        UNKNOWN2 = 1 << 17,
        FROM_SRCH_FLD = 1 << 18,
        THRU_SRCH_FLD = 1 << 19,
        ONE_ZERO_EDIT = 1 << 20,
        DISABLE_ADV_SRCH_OPT = 1 << 21,
        UNKNOWN3 = 1 << 22,
        UNKNOWN4 = 1 << 23,
        DFLT_SRCH_FIELD = 1 << 24,
        UNKNOWN5 = 1 << 25,
        UNKNOWN6 = 1 << 26,
        SRCH_EVENT_FOR_PROMPT = 1 << 27,
        SRCH_EDIT = 1 << 29,
        ENABLE_AUTO_CMPLT_SRCH_RECORD = 1 << 30,
        PERSIST_IN_MENU = (uint)1 << 31,
    }

    public enum RecordIndexTypes : short
    {
        KEY = 1,
        ALT = 3,
        USER = 4
    }

}
