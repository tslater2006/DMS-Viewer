using DMSLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    public partial class DataViewer : Form
    {
        int sortColumn = -1;
        bool sortAscending = true;

        private bool IsRunningMono = false;
        private DMSTable viewerTable;
        public DataViewer(DMSTable table, string ConnectedDBName)
        {
            InitializeComponent();

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this.dataGridView1, new object[] { true });

            viewerTable = table;
            this.Text = "Data Viewer: " + table.DBName;
            if (table.CompareResult!= DMSCompareResult.SAME && ConnectedDBName.Length > 0)
            {
                this.Text += " - " + ConnectedDBName;
            }
            InitDataTable();
            //FillDataTable();
            dataGridView1.RowCount = viewerTable.Rows.Count;

            IsRunningMono = Type.GetType("Mono.Runtime") != null;
        }

        public void RedrawTable()
        {
            InitDataTable();
            //FillDataTable();
        }

        public void InitDataTable()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            foreach (var col in viewerTable.Columns)
            {
                dataGridView1.Columns.Add(col.Name, col.Name);
            }

            dataGridView1.RowCount = viewerTable.Rows.Count;
        }


        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var content = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    new LongDataViewer(content).ShowDialog(this);
                }            
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                var hitTest = dataGridView1.HitTest(e.X, e.Y);
                int currentRow = hitTest.RowIndex;
                int currentColumn = hitTest.ColumnIndex;
                if (currentRow >= 0 && currentColumn >= 0)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[currentRow].Cells[currentColumn];
                    ContextMenu m = new ContextMenu();
                    MenuItem editValue = new MenuItem("Edit Value");
                    editValue.Tag = hitTest;
                    editValue.Click += EditValue_Click;
                    m.MenuItems.Add(editValue);
                    m.Show(dataGridView1, new Point(e.X, e.Y));

                }

                if (currentRow >= 0 && currentColumn == -1)
                {
                    /* Right clicked on row header */
                    dataGridView1.Rows[currentRow].Selected = true;
                    ContextMenu m = new ContextMenu();
                    MenuItem deleteRow = new MenuItem("Delete Row");
                    deleteRow.Tag = hitTest;
                    deleteRow.Click += DeleteRow_Click;
                    m.MenuItems.Add(deleteRow);
                    m.Show(dataGridView1, new Point(e.X, e.Y));
                }
                
                if (currentRow == -1 && currentColumn >= 0)
                {
                    /* Right clicked a column header */
                    dataGridView1.Columns[currentColumn].Selected = true;
                    ContextMenu m = new ContextMenu();
                    
                    MenuItem deleteColumn = new MenuItem("Delete Column...");
                    deleteColumn.Tag = hitTest;
                    deleteColumn.Click += DeleteColumn_Click;
                    m.MenuItems.Add(deleteColumn);

                    MenuItem addColMenu = new MenuItem("Add Column After...");
                    addColMenu.Tag = hitTest;
                    addColMenu.Click += AddColMenu_Click;
                    m.MenuItems.Add(addColMenu);
                    m.Show(dataGridView1, new Point(e.X, e.Y));
                }

            }
        }

        private void DeleteColumn_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var hitTest = (DataGridView.HitTestInfo)menuItem.Tag;
            var selectedColumn = viewerTable.Columns[hitTest.ColumnIndex];
            viewerTable.DropColumn(selectedColumn);
            RedrawTable();
        }

        private void AddColMenu_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var hitTest = (DataGridView.HitTestInfo)menuItem.Tag;

            AddColumnOptions opts = new AddColumnOptions();
            opts.ShowDialog(this);

            DMSNewColumn newCol = opts.newColumn;
            if (newCol != null)
            {
                var defVal = opts.defaultValue;

                viewerTable.AddColumn(newCol, viewerTable.Columns[hitTest.ColumnIndex], defVal);
                RedrawTable();
            }

        }

        private void DeleteRow_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var hitTest = (DataGridView.HitTestInfo)menuItem.Tag;

            var result = MessageBox.Show(this, "Are you sure you want to remove this row?", "Confirm Row Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DMSRow curRow = viewerTable.Rows[hitTest.RowIndex];
                viewerTable.Rows.Remove(curRow);
                RedrawTable();
            } 
        }

        private void EditValue_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var hitTest = (DataGridView.HitTestInfo)menuItem.Tag;

            var content = dataGridView1.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex].Value.ToString();

            DMSRow curRow = viewerTable.Rows[hitTest.RowIndex];

            new LongDataViewer(content, this, curRow, hitTest.ColumnIndex).ShowDialog(this);
            
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            Color backColor = Color.White;
            //switch (viewerTable.Rows[e.RowIndex].CompareResult)
            if (dataGridView1.Rows[e.RowIndex].Tag == null) { return; }
            switch (((DMSRow)(dataGridView1.Rows[e.RowIndex].Tag)).CompareResult)
            {
                case DMSCompareResult.NEW:
                    backColor = Color.LawnGreen;
                    break;
                case DMSCompareResult.UPDATE:
                    backColor = Color.Yellow;
                    break;
            }

            dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = backColor;
        }

        private void dgGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            /*var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);*/

        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                e.Value = viewerTable.Rows[e.RowIndex].GetStringValue(e.ColumnIndex);
            }
            catch (Exception ex) { }
            //Debugger.Break();
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Debugger.Break();
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == sortColumn)
            {
                sortAscending = !sortAscending;
            } else
            {
                sortColumn = e.ColumnIndex;
                sortAscending = true;
            }

            /* sort the rows of the viewer table by the selected colum index */
            if (sortAscending) {
                viewerTable.Rows = viewerTable.Rows.OrderBy(r => r.GetStringValue(sortColumn)).ToList();
            } else
            {
                viewerTable.Rows = viewerTable.Rows.OrderByDescending(r => r.GetStringValue(sortColumn)).ToList();
            }

            RedrawTable();

        }
    }
}
