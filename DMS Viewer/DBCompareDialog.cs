using DMSLib;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    public partial class DBCompareDialog : Form
    {
        OracleConnection dbConn;
        DMSFile file;
        List<DMSTable> tables;
        BackgroundWorker worker;
        public DBCompareDialog(OracleConnection dbConn, DMSFile file, List<DMSTable> tables)
        {
            InitializeComponent();

            progressBar1.Maximum = 100;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            this.dbConn = dbConn;
            this.file = file;
            this.tables = tables;

        }

        private void RunCompare(object sender, DoWorkEventArgs e)
        {
            var totalRows = tables.Select(t => t.Rows.Count).Sum();
            var rowsProcessed = 0;

            /* check to see if the table exists ... */
            foreach (var table in tables)
            {
                using (OracleCommand existsCheck = new OracleCommand($"SELECT 'Y' FROM {table.DBName} WHERE ROWNUM = 1", dbConn))
                {
                    try
                    {
                        existsCheck.ExecuteReader();
                        List<DMSRecordFieldMetadata> keyFields = table.Metadata.FieldMetadata.Where(m => (int)m.UseEditMask % 2 == 1).ToList();
                        foreach (var curRow in table.Rows)
                        {
                            /* compare this row to the DB */
                            CompareRow(table, keyFields, curRow);
                            /* count this row */
                            rowsProcessed++;
                            worker.ReportProgress((int)((rowsProcessed / (double)totalRows) * 100));
                        }

                        if (table.Rows.Where(r => r.CompareResult == DMSCompareResult.UPDATE).Count() > 0)
                        {
                            table.CompareResult = DMSCompareResult.UPDATE;
                        }
                        else
                        {
                            if (table.Rows.Where(r => r.CompareResult == DMSCompareResult.NEW).Count() > 0)
                            {
                                table.CompareResult = DMSCompareResult.NEW;
                            }
                            else
                            {
                                table.CompareResult = DMSCompareResult.SAME;
                            }
                        }
                    }

                    catch (OracleException ex)
                    {
                        if (ex.Number == 942)
                        {
                            /* table doesn't exist */
                            MessageBox.Show($"The table {table.Name} doesn't exist in the target database. It will be created at import time.");
                            foreach (var tbl in file.Tables.Where(t => t.Name == table.Name))
                            {
                                foreach (var row in tbl.Rows)
                                {
                                    row.CompareResult = DMSCompareResult.NEW;
                                }
                                tbl.CompareResult = DMSCompareResult.NEW;
                            }
                            rowsProcessed += table.Rows.Count;
                            worker.ReportProgress((int)((rowsProcessed / (double)totalRows) * 100));
                        }
                    }
                }
            }

        }

        private void SetOracleParamValue(OracleParameter param, FieldTypes type, DMSRow curRow, int index)
        {
            switch (type)
            {
                case FieldTypes.CHAR:
                    param.OracleDbType = OracleDbType.Varchar2;
                    param.Value = curRow.GetValue(index);
                    break;
                case FieldTypes.LONG_CHAR:
                    param.OracleDbType = OracleDbType.Clob;
                    param.Value = curRow.GetValue(index);
                    if ((string)param.Value == "\0")
                    {
                        param.Value = null;
                    }
                    break;
                case FieldTypes.NUMBER:
                    param.OracleDbType = OracleDbType.Int64;
                    param.Value = curRow.GetValue(index);
                    break;
                case FieldTypes.SIGNED_NUMBER:
                    param.OracleDbType = OracleDbType.Int64;
                    param.Value = curRow.GetValue(index);
                    break;
                case FieldTypes.DATE:
                    param.OracleDbType = OracleDbType.Date;
                    param.Value = curRow.GetValue(index);
                    break;
                case FieldTypes.DATETIME:
                    param.OracleDbType = OracleDbType.TimeStamp;
                    param.Value = curRow.GetValue(index);
                    break;
                case FieldTypes.TIME:
                    param.OracleDbType = OracleDbType.TimeStamp;
                    param.Value = curRow.GetValue(index);
                    break;
                case FieldTypes.IMG_OR_ATTACH:
                    param.OracleDbType = OracleDbType.Blob;
                    param.Value = curRow.GetValue(index);
                    break;
                default:
                    Debugger.Break();
                    break;
            }
        }

        private static byte[] HexStringToBytes(string s)
        {
            const string HEX_CHARS = "0123456789ABCDEF";

            if (s.Length == 0)
                return new byte[0];

            if (s.Length % 2 != 0)
                throw new FormatException();

            byte[] bytes = new byte[s.Length / 2];

            int state = 0; // 0 = expect first digit, 1 = expect second digit, 2 = expect hyphen
            int currentByte = 0;
            int x;
            int value = 0;

            foreach (char c in s)
            {
                switch (state)
                {
                    case 0:
                        x = HEX_CHARS.IndexOf(Char.ToUpperInvariant(c));
                        if (x == -1)
                            throw new FormatException();
                        value = x << 4;
                        state = 1;
                        break;
                    case 1:
                        x = HEX_CHARS.IndexOf(Char.ToUpperInvariant(c));
                        if (x == -1)
                            throw new FormatException();
                        bytes[currentByte++] = (byte)(value + x);
                        state = 2;
                        break;
                }
            }

            return bytes;
        }

        private void CompareRow(DMSTable table, List<DMSRecordFieldMetadata> keys, DMSRow curRow)
        {
            int[] keyIndexes = new int[keys.Count];
            for (var x = 0; x < keys.Count; x++)
            {
                var fieldName = keys[x].FieldName;
                var fieldIndex = table.Columns.FindIndex(c => c.Name == fieldName);
                keyIndexes[x] = fieldIndex;
            }

            /* create SQL statement for this item */
            /* SELECT 'Y' FROM DBNAME WHERE KEY1 = :1 */

            var sqlBuilder = new StringBuilder($"SELECT 'Y' FROM {table.DBName} WHERE ");

            for (var x = 0; x < keys.Count; x++)
            {
                if (x > 0)
                {
                    sqlBuilder.Append(" AND ");
                }

                sqlBuilder.Append($"{keys[x].FieldName} = :{x + 1}");
            }

            /* DEBUG */
            //using (var dbg = new OracleCommand("SELECT IS_DX_INPUT_VALUE FROM PS_IS_DX_USERINPUT WHERE ROWNUM = 1 AND dbms_lob.compare(IS_DX_INPUT_VALUE, :1) != 0", dbConn))
            //using (var dbg = new OracleCommand("SELECT IS_DX_LAST_UPDT FROM PS_IS_DX_INSTANCE WHERE IS_DX_LAST_UPDT != :1 AND ROWNUM = 1", dbConn))
            /*using (var dbg = new OracleCommand("SELECT CONTDATA FROM PSCONTENT WHERE ROWNUM = 1", dbConn))
            {

                using (var dbgRead = dbg.ExecuteReader())
                {
                    dbgRead.Read();
                    var fieldType = dbgRead.GetFieldType(0);
                    var oracleBlob = dbgRead.GetOracleBlob(0);
                    byte[] blobData = new byte[oracleBlob.Length];
                    oracleBlob.Read(blobData, 0, blobData.Length);
                    Console.WriteLine(BitConverter.ToString(blobData));
                }
                
            }*/

            using (var cmd = new OracleCommand(sqlBuilder.ToString(), dbConn))
            {

                /* set bind parameters */
                for (var x = 0; x < keys.Count; x++)
                {
                    OracleParameter keyParam = new OracleParameter();
                    SetOracleParamValue(keyParam, keys[x].FieldType, curRow, keyIndexes[x]);
                    cmd.Parameters.Add(keyParam);
                }
                /* execute */
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        /* if a row came back, it exists... */

                        /* we need to check if the row that exists matches on each and every column */
                        StringBuilder diffCheck = new StringBuilder($"SELECT 'Y' FROM {table.DBName} WHERE ");
                        for (var x = 0; x < table.Metadata.FieldMetadata.Count; x++) 
                        {
                            var column = table.Metadata.FieldMetadata[x];
                            if (column.FieldType == FieldTypes.LONG_CHAR || column.FieldType == FieldTypes.IMG_OR_ATTACH)
                            {
                                /* dbms_lob.compare */
                                diffCheck.Append($"dbms_lob.compare(nvl({column.FieldName},'Null'),nvl(:{x + 1},'Null')) = 0 ");
                            } else
                            {
                                diffCheck.Append($"{column.FieldName} = :{x + 1} ");
                            }
                            if (x + 1 < table.Metadata.FieldMetadata.Count)
                            {
                                diffCheck.Append(" AND ");
                            }
                        }
                        using (var diffCheckCmd = new OracleCommand(diffCheck.ToString(), dbConn))
                        {
                            for (var x = 0; x < table.Metadata.FieldMetadata.Count; x++)
                            {
                                var column = table.Metadata.FieldMetadata[x];
                                OracleParameter fieldParam = new OracleParameter();
                                SetOracleParamValue(fieldParam, column.FieldType, curRow, x);
                                diffCheckCmd.Parameters.Add(fieldParam);
                            }
                            try
                            {
                                using (var diffCheckReader = diffCheckCmd.ExecuteReader())
                                {
                                    if (diffCheckReader.Read())
                                    {
                                        curRow.CompareResult = DMSCompareResult.SAME;
                                    }
                                    else
                                    {
                                        curRow.CompareResult = DMSCompareResult.UPDATE;
                                    }
                                }
                            } catch (Exception ex)
                            {
                                /* failed to do the diff, likely due to column changes */
                                /* mark as an update */
                                curRow.CompareResult = DMSCompareResult.UPDATE;
                            }
                        }
                    } else
                    {
                        curRow.CompareResult = DMSCompareResult.NEW;
                    }
                }
            }
        }
        private void DBCompareDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private async void DBCompareDialog_Load(object sender, EventArgs e)
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += RunCompare;
            worker.ProgressChanged += Bw_ProgressChanged;
            worker.RunWorkerCompleted += Bw_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Update();
        }
    }
}
