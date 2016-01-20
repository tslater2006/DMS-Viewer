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
    public partial class Form1 : Form
    {
        DMSFile dmsFile = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Data Mover Data Files|*.dat";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                DMSParser parser = new DMSParser();
                dmsFile = parser.ParseFile(openFileDialog1.FileName);
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (dmsFile == null)
            {
                return;
            }
            txtVersion.Text = dmsFile.Version;
            txtDatabase.Text = dmsFile.Database;
            txtStarted.Text = dmsFile.Started;
            tableList.Items.Clear();
            foreach(var table in dmsFile.Tables)
            {
                tableList.Items.Add(table);
            }
        }

        private void tableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var value = tableList.SelectedItem as DMSTable;
            if (value == null)
            {
                return;
            }
            columnList.Items.Clear();
            
            foreach (DMSTableColumn col in value.Columns)
            {
                ListViewItem item = new ListViewItem(col.Name);
                item.Tag = col;
                item.SubItems.Add(col.Type);
                item.SubItems.Add(col.Size);
                columnList.Items.Add(item);
            }

            lblRowCount.Text = value.Rows.Count.ToString();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SQLGenerator.GenerateSQLFile(dmsFile, @"C:\users\tslat\Desktop\out.sql",false);
        }
    }
}
