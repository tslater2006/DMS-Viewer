using DMSLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    public partial class DATCompareDialog : Form
    {
        DMSFile leftFile = null;
        DMSFile rightFile = null;
        public DATCompareDialog(DMSFile left)
        {
            InitializeComponent();

            leftFile = left;
            UpdateUI(true);
        }

        void UpdateUI(bool leftSide)
        {
            ListView list = null;
            DMSFile file = null;
            Button btn = null;
            Label lbl = null;
            if (leftSide)
            {
                list = lstLeft;
                file = leftFile;
                btn = btnViewDataLeft;
                lbl = lblLeft;
            }
            else
            {
                list = lstRight;
                file = rightFile;
                btn = btnViewDataRight;
                lbl = lblRight;
            }

            if (file == null)
            {
                lbl.Text = @"Select a file...";
                return;
            }
            else
            {
                lbl.Text = file.FileName;
            }
            
            list.Items.Clear();
            foreach (var table in file.Tables)
            {
                /* ignore empty tables */
                if (table.Rows.Count == 0)
                {
                    continue;
                }
                var backgroundColor = Color.White;
                switch (table.CompareResult)
                {
                    case DMSCompareResult.NEW:
                        backgroundColor = Color.LawnGreen;
                        break;
                    case DMSCompareResult.UPDATE:
                        backgroundColor = Color.Yellow;
                        break;
                }
                list.Items.Add(new ListViewItem() { Tag = table, Text = table.Name, BackColor = backgroundColor });
            }

            list.Items[0].Selected = true;
            btn.Enabled = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            leftFile = GetDATFile();
            UpdateUI(true);
        }

        private DMSFile GetDATFile()
        {
            openFileDialog1.Filter = @"Data Mover Data Files|*.dat;*.DAT";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                var dmsFile = DMSReader.Read(openFileDialog1.FileName);
                /* Set the file name */
                dmsFile.FileName = new FileInfo(openFileDialog1.FileName).Name;
                return dmsFile;
            }
            
            return null;
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            rightFile = GetDATFile();
            UpdateUI(false);
            
        }

        private void btnViewDataLeft_Click(object sender, EventArgs e)
        {
            var viewer = new DataViewer(lstLeft.SelectedItems[0].Tag as DMSTable, "");
            viewer.ShowDialog(this);
        }

        private void lstLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLeft.SelectedItems.Count == 1)
            {
                btnViewDataLeft.Enabled = true;
                lblLeftRows.Text = $@"Rows: {(lstLeft.SelectedItems[0].Tag as DMSTable)?.Rows.Count}";
            }
        }

        private void lstRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRight.SelectedItems.Count == 1)
            {
                btnViewDataRight.Enabled = true;
                lblRightRows.Text = $@"Rows: {(lstRight.SelectedItems[0].Tag as DMSTable)?.Rows.Count}";
            }
        }

        private void btnViewDataRight_Click(object sender, EventArgs e)
        {
            var viewer = new DataViewer(lstRight.SelectedItems[0].Tag as DMSTable, "");
            viewer.ShowDialog(this);
        }
    }
}
