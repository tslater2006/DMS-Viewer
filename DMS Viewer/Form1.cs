using DMSLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    public partial class Form1 : Form
    {
        DMSFile dmsFile = null;
        string currentDmsPath;
        private bool IsRunningMono = false;
        private bool IsRunningOSX = false;

        public Form1()
        {
            InitializeComponent();

            IsRunningMono = Type.GetType("Mono.Runtime") != null;
            if (IsRunningMono)
            {
                /* detect if running OSX for special "Copy" functionality */
                if (Directory.Exists("/Applications")
                    && Directory.Exists("/Users"))
                {
                    IsRunningOSX = true;
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Data Mover Data Files|*.dat;*.DAT";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                currentDmsPath = openFileDialog1.FileName;

                Stopwatch sw = new Stopwatch();
                sw.Start();
                dmsFile = DMSReader.Read(currentDmsPath);
                sw.Stop();

                UpdateUI();

                DMSLib.DMSReader.Read(currentDmsPath);
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
            generateSQLToolStripMenuItem.Enabled = true;
            dataViewer.Enabled = true;
            btnRecordMeta.Enabled = true;
            foreach(var table in dmsFile.Tables)
            {
                tableList.Items.Add(table);
            }
            tableList.SelectedIndex = 0;
            saveAsToolStripMenuItem.Enabled = true;
        }

        private void tableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawColumns();
        }

        private void DrawColumns()
        {
            var value = tableList.SelectedItem as DMSTable;
            whereClause.Text = value.WhereClause;
            if (value == null)
            {
                return;
            }
            columnList.Items.Clear();

            foreach (DMSColumn col in value.Columns)
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

        }

        private void generateSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* DMSFile dms, string outputFolder, bool padColumns, bool extractLongs, bool ignoreEmptyTables*/

            var opts = new SQLGeneratorOptions(dmsFile);
            opts.ShowDialog(this);
            opts = null;

            // SQLGenerator.GenerateSQLFile(dmsFile, @"out.sql",false);
        }

        private void rebuildScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var opts = new ScriptRebuildOptions(currentDmsPath);
            opts.ShowDialog(this);
            opts = null;
        }

        private void dataViewer_Click(object sender, EventArgs e)
        {
            var viewer = new DataViewer(tableList.SelectedItem as DMSTable);
            viewer.ShowDialog(this);
            DrawColumns();
        }

        private void copyTables_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var i in tableList.Items)
            {
                sb.AppendLine(i.ToString());
            }
            if (IsRunningOSX)
            {
                OSXClipboard.CopyToClipboard(sb.ToString());
            } else
            {
                Clipboard.SetText(sb.ToString());
            }
        }

        private void copyWhereClause_Click(object sender, EventArgs e)
        {
            if (IsRunningOSX)
            {
                OSXClipboard.CopyToClipboard(whereClause.Text);
            }
            else
            {
                Clipboard.SetText(whereClause.Text);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Saving of DAT files is currently in testing.\r\n\r\nPlease do not rely on this feature yet as there may be bugs or DAT format issues that haven't been accounted for. \r\n\r\nIf you feel like being adventureous please go ahead and give this feature a try!", "Modifed DAT is in BETA", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            saveFileDialog1.FileName = currentDmsPath;
            saveFileDialog1.Filter = "Data Mover Data Files|*.dat;*.DAT";

            var dlgResult = saveFileDialog1.ShowDialog();

            if (dlgResult == DialogResult.OK)
            {
                DMSWriter.Write(saveFileDialog1.FileName, dmsFile);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DMSTable curTable = tableList.SelectedItem as DMSTable;
            RecordMetadataViewer viewer = new RecordMetadataViewer(curTable.Metadata);
            viewer.ShowDialog(this);
        }

        private void columnList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = columnList.HitTest(e.X, e.Y);

                ContextMenu m = new ContextMenu();
                MenuItem editField = new MenuItem("Edit Field...");
                editField.Tag = hitTest;
                editField.Click += EditField_Click;
                m.MenuItems.Add(editField);
                m.Show(columnList, new Point(e.X, e.Y));

            }
        }

        private void EditField_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var hitTest = (ListViewHitTestInfo)menuItem.Tag;

            var column = (DMSColumn)hitTest.Item.Tag;
            DMSTable curTable = tableList.SelectedItem as DMSTable;
            var columnMetadata = curTable.Metadata.FieldMetadata.Where(p => p.FieldName == column.Name).First();

            FieldMetadataViewer viewer = new FieldMetadataViewer(column, columnMetadata);
            viewer.ShowDialog(this);

            DrawColumns();

        }
    }
}
