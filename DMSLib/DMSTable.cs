using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSTable
    {
        public string Name;
        public string DBName;
        public string WhereClause;
        public DMSRecordMetadata Metadata;
        public List<DMSColumn> Columns = new List<DMSColumn>();
        public List<DMSRow> Rows = new List<DMSRow>();
        public override string ToString()
        {
            return Name;
        }

        public void AddColumn(DMSNewColumn newColumn, DMSColumn colBefore, string defaultValue)
        {
            
            /* Update DMSRecord metadata */
            Metadata.FieldCount++;

            /* Add the field metadata */
            var colIndex = Columns.IndexOf(colBefore) + 1;
            var newFieldMeta = new DMSRecordFieldMetadata(newColumn, this);
            Metadata.FieldMetadata.Insert(colIndex, newFieldMeta);

            /* Add the DMSColumn */
            var newDMSCol = new DMSColumn();
            newDMSCol.Name = newColumn.FieldName;
            if (newColumn.DecimalPositions > 0)
            {
                newDMSCol.Size = newColumn.FieldLength +"," + newColumn.DecimalPositions;
            }
            newDMSCol.Size = newColumn.FieldLength.ToString();

            newDMSCol.Type = "CHAR";

            Columns.Insert(colIndex, newDMSCol);

            foreach(var row in Rows)
            {
                row.InsertValueString(colIndex, defaultValue);
            }

        }

        public void WriteToStream(StreamWriter sw)
        {
            if (WhereClause.Length > 0)
            {
                sw.WriteLine($"EXPORT  {Name}.{DBName} WHERE ");
                WriteWhereClause(sw, WhereClause);
            }
            else
            {
                sw.WriteLine($"EXPORT  {Name}.{DBName} ");
            }
            sw.WriteLine("/");

            /* Write table metadata */
            Metadata.WriteToStream(sw);

            /* Write the column info */
            WriteColumns(sw, Columns);

            /* Write each row of data */
            foreach (DMSRow row in Rows)
            {
                row.WriteToStream(sw);
                /* WriteTableRow(stream, row);
                stream.WriteLine("//");*/
            }
            sw.WriteLine("/");
        }

        private void WriteWhereClause(StreamWriter stream, string where)
        {
            stream.WriteLine(where);
        }

        private void WriteColumns(StreamWriter stream, List<DMSColumn> columns)
        {
            var maxLength = 68;
            var curLineLength = 0;
            stream.WriteLine("/");
            foreach (DMSColumn col in columns)
            {
                var nextColumn = $"{col.Name}:{col.Type}({col.Size})~~~";
                if (curLineLength + nextColumn.Length < maxLength)
                {
                    stream.Write(nextColumn);
                    curLineLength += nextColumn.Length;
                }
                else
                {
                    stream.WriteLine();
                    stream.Write(nextColumn);
                    curLineLength = nextColumn.Length;
                }
            }
            stream.WriteLine();
            stream.WriteLine("/");
        }

        public void DropColumn(DMSColumn selectedColumn)
        {
            /* Update DMSRecord metadata */
            Metadata.FieldCount--;

            /* Add the field metadata */
            var colIndex = Columns.IndexOf(selectedColumn);

            /* Remove column from metadata */
            Metadata.FieldMetadata.Remove(Metadata.FieldMetadata.Where(p => p.FieldName == selectedColumn.Name).First());

            /* Remove the DMSColumn */
            Columns.Remove(selectedColumn);

            /* remove the value from the rows */
            foreach (var row in Rows)
            {
                row.DeleteValue(colIndex);
            }
        }
    }

    public class DMSNewColumn
    {
        public string FieldName;
        public int VersionNumber;
        public int DecimalPositions;

        public UseEditFlags UseEditMask;
        public FieldTypes FieldType;
        public FieldFormats FieldFormat;
        public int FieldLength;
        public GUIControls DefaultGUIControl;

        public DMSNewColumn(string fieldName, int version, int length, int decPositions, UseEditFlags useEdit, FieldTypes type, FieldFormats format, GUIControls gui = GUIControls.DEFAULT)
        {
            FieldName = fieldName;
            VersionNumber = version;
            FieldLength = length;
            DecimalPositions = decPositions;
            UseEditMask = useEdit;
            FieldType = type;
            FieldFormat = format;
            DefaultGUIControl = gui;
        }


    }
}
