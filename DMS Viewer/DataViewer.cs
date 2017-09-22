using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    public partial class DataViewer : Form
    {
        private bool IsRunningMono = false;
        public DataViewer(DMSTable table)
        {
            InitializeComponent();
            this.Text = "Data Viewer: " + table.DBName;
            dataGridView1.DataSource = BuildDataTable(table);

            DataGridViewButtonColumn copyCol = new DataGridViewButtonColumn();
            copyCol.Text = "Copy Row";
            copyCol.HeaderText = "";
            copyCol.UseColumnTextForButtonValue = true;
            copyCol.Name = "Copy Row";
            copyCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dataGridView1.Columns.Add(copyCol);

            int colIndex = 0;
            for (colIndex = 0; colIndex < table.Columns.Count ; colIndex++)
            {
                if (table.Columns[colIndex].Type.Equals("LONG"))
                {
                    dataGridView1.Columns[colIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
                else
                {
                    dataGridView1.Columns[colIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }

            IsRunningMono = Type.GetType("Mono.Runtime") != null;
        }

        private DataTable BuildDataTable(DMSTable tbl)
        {
            DataTable dt = new DataTable();

            foreach (var c in tbl.Columns)
            {
                var dc = new DataColumn(c.Name);
                dt.Columns.Add(dc);
            }

            foreach(var r in tbl.Rows)
            {
                var dr = dt.NewRow();
                dr.ItemArray = r.Values.ToArray();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsRunningMono)
            {
                if (e.RowIndex >= 0)
                {
                    var content = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    new LongDataViewer(content).ShowDialog(this);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (IsRunningMono)
            {
                if (e.RowIndex >= 0)
                {
                    var content = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    new LongDataViewer(content).ShowDialog(this);
                }
            }
            
        }
    }
}
