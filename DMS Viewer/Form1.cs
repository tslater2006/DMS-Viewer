using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DMSLib;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;

namespace DMS_Viewer
{
    public partial class Form1 : Form
    {
        private string ConnectedDBName = "";
        string currentDmsPath;
        OracleConnection dbConn = null;
        DMSFile dmsFile = null;
        private bool IsRunningMono = false;
        private bool IsRunningOSX = false;
        ScriptRebuildOptions scriptOpts = null;
        SQLGeneratorOptions sqlOpts = null;

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

            /* set the checked states for our settings */
            ignoreVersionToolStripMenuItem.Checked = Properties.Settings.Default.IgnoreVersion;
            ignoreDatesTimesToolStripMenuItem.Checked = Properties.Settings.Default.IgnoreDates;
            hideEmptyTablesToolStripMenuItem.Checked = Properties.Settings.Default.HideEmptyTables;

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
                try
                {
                    dmsFile = DMSReader.Read(currentDmsPath);
                }
                catch(FormatException fe)
                {
                    MessageBox.Show(this, fe.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                toolStripStatusLabel1.Text = "File: " + currentDmsPath;
                /* Set the file name */
                dmsFile.FileName = new FileInfo(currentDmsPath).Name;

                sw.Stop();

                UpdateUI();
                tableList.Items[0].Selected = true;
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
            btnCompareToDB.Enabled = true;
            foreach (var table in dmsFile.Tables)
            {
                if (Properties.Settings.Default.HideEmptyTables == false ||
                    (Properties.Settings.Default.HideEmptyTables == true && table.Rows.Count > 0))
                {
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

                    tableList.Items.Add(
                        new ListViewItem() {Tag = table, Text = table.Name, BackColor = backgroundColor});
                }
            }

            tableList.SelectedItems.Clear();
            saveAsToolStripMenuItem.Enabled = true;
            compareToDATToolStripMenuItem.Enabled = true;
        }

        private void tableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tableList.SelectedItems.Count > 0)
            {
                dataViewer.Enabled = true;
                btnRecordMeta.Enabled = true;
                if (dbConn != null)
                {
                    btnCompareToDB.Enabled = true;
                }
                else
                {
                    btnCompareToDB.Enabled = false;
                }

                if (tableList.SelectedItems.Count > 1)
                {
                    dataViewer.Enabled = false;
                    btnRecordMeta.Enabled = false;
                    lblRowCount.Text = "0";
                    columnList.Items.Clear();
                }
                else
                {
                    DrawColumns();
                }
            }
            else
            {
                dataViewer.Enabled = false;
                btnRecordMeta.Enabled = false;
                btnCompareToDB.Enabled = false;
                columnList.Items.Clear();
            }
        }

        private void DrawColumns()
        {
            var value = tableList.SelectedItems[0].Tag as DMSTable;
            whereClause.Text = value.WhereClause;
            if (value == null)
            {
                return;
            }

            columnList.Items.Clear();

            foreach (DMSColumn col in value.Columns)
            {
                var isKey = value.Metadata.FieldMetadata.Where(m => m.FieldName == col.Name).First().UseEditMask
                    .HasFlag(UseEditFlags.KEY);
                ListViewItem item = new ListViewItem(isKey ? "✓" : " ");
                item.Tag = col;

                /* is this item a key? */

                item.SubItems.Add(col.Name);
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
            if (sqlOpts == null)
            {
                sqlOpts = new SQLGeneratorOptions(dmsFile, currentDmsPath);
            }
            else
            {
                /* always make sure it has reference to current dmsFile */
                sqlOpts.UpdateDMSInfo(dmsFile, currentDmsPath);
            }

            sqlOpts.ShowDialog(this);
        }

        private void rebuildScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scriptOpts == null)
            {
                scriptOpts = new ScriptRebuildOptions(currentDmsPath);
            }
            else
            {
                /* make sure DMS path is always the latest */
                scriptOpts.UpdateDMSPath(currentDmsPath);
            }

            scriptOpts.ShowDialog(this);
        }

        private void dataViewer_Click(object sender, EventArgs e)
        {
            var viewer = new DataViewer(tableList.SelectedItems[0].Tag as DMSTable, ConnectedDBName);
            viewer.ShowDialog(this);
            DrawColumns();
        }

        private void copyTables_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ListViewItem i in tableList.Items)
            {
                sb.AppendLine(((DMSTable) i.Tag).Name);
            }

            if (IsRunningOSX)
            {
                OSXClipboard.CopyToClipboard(sb.ToString());
            }
            else
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
            MessageBox.Show(this,
                "Saving of DAT files is currently in testing.\r\n\r\nPlease do not rely on this feature yet as there may be bugs or DAT format issues that haven't been accounted for. \r\n\r\nIf you feel like being adventureous please go ahead and give this feature a try!",
                "Modifed DAT is in BETA", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
            DMSTable curTable = tableList.SelectedItems[0].Tag as DMSTable;
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
            var menuItem = (MenuItem) sender;
            var hitTest = (ListViewHitTestInfo) menuItem.Tag;

            var column = (DMSColumn) hitTest.Item.Tag;
            DMSTable curTable = tableList.SelectedItems[0].Tag as DMSTable;
            var columnMetadata = curTable.Metadata.FieldMetadata.Where(p => p.FieldName == column.Name).First();

            FieldMetadataViewer viewer = new FieldMetadataViewer(column, columnMetadata);
            viewer.ShowDialog(this);

            DrawColumns();
        }

        private void btnCompareToDB_Click(object sender, EventArgs e)
        {
            if (dbConn != null)
            {
                /* create the compare dialog which runs the compare */
                new DBCompareDialog(dbConn, dmsFile,
                        tableList.SelectedItems.Cast<ListViewItem>().Select(i => (DMSTable) i.Tag).ToList())
                    .ShowDialog(this);
                /* save off the saved index for the table */

                var savedIndexes = tableList.SelectedIndices;
                UpdateUI();
                foreach (var x in savedIndexes.Cast<int>())
                {
                    tableList.Items[x].Selected = true;
                }
            }
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FindAndReplace(dmsFile).ShowDialog(this);
        }

        private void SaveDATDiff(DMSFile file)
        {
            saveFileDialog1.Filter = @"Data Mover Data Files|*.dat;*.DAT";
            var result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                DMSWriter.Write(saveFileDialog1.FileName, dmsFile, true);
            }
        }

        private void hideEmptyTablesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
                dbConn = null;
            }

            var dbConnForm = new DBLogin();
            if (dbConnForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Connected to the database!");
                dbConn = dbConnForm.Connection;
                ConnectedDBName = dbConnForm.DBName;
                toolStripStatusLabel2.Text = "Database: " + ConnectedDBName;
            }

            disconnectToolStripMenuItem.Visible = true;
            if (dmsFile != null && tableList.SelectedItems.Count > 0)
            {
                /* We have a DMS File loaded, and at least 1 table is selected */
                btnCompareToDB.Enabled = true;
            }
        }

        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void TableList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var selectedTables =
                    tableList.SelectedItems.Cast<ListViewItem>().Select(i => (DMSTable) i.Tag).ToList();
                if (selectedTables.Count > 0)
                {
                    ContextMenu m = new ContextMenu();

                    MenuItem generateSQL = new MenuItem("Generate SQL...");
                    generateSQL.Tag = selectedTables;
                    generateSQL.Click += (o, args) =>
                    {
                        var sqlGen = new SQLGeneratorOptions(dmsFile, currentDmsPath, selectedTables);
                        sqlGen.ShowDialog(this);
                    };

                    m.MenuItems.Add(generateSQL);

                    MenuItem exportToExcel = new MenuItem("Export to Excel...");
                    exportToExcel.Tag = selectedTables;
                    exportToExcel.Click += ExportToExcel_Click;
                    m.MenuItems.Add(exportToExcel);

                    if (dmsFile.Tables.Any(t =>
                        t.CompareResult == DMSCompareResult.NEW || t.CompareResult == DMSCompareResult.UPDATE))
                    {
                        MenuItem saveDiffs = new MenuItem("Save DAT diff...");
                        saveDiffs.Tag = selectedTables;
                        saveDiffs.Click += (o, args) => { SaveDATDiff(dmsFile); };
                        m.MenuItems.Add(saveDiffs);
                    }

                    m.Show(tableList, new Point(e.X, e.Y));
                }
            }
        }

        private void ExportToExcel_Click(object sender, EventArgs e)
        {
            List<DMSTable> tables = (sender as MenuItem).Tag as List<DMSTable>;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Workbook *.xlsx|*.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string outFile = sfd.FileName;
                WriteTablesToExcel(tables, outFile);
            }
        }

        private void WriteTablesToExcel(List<DMSTable> tables, string path)
        {
            using (var package = new ExcelPackage())
            {
                foreach (var table in tables)
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets.Add(table.Name);

                    /* add column names */
                    for (var x = 0; x < table.Columns.Count; x++)
                    {
                        sheet.Cells[1, x + 1].Value = table.Columns[x].Name;
                    }

                    /* Auto fit columns w/ headers in place */
                    sheet.Cells.AutoFitColumns(0);

                    /* add each row */
                    for (var x = 0; x < table.Rows.Count; x++)
                    {
                        for (var y = 0; y < table.Columns.Count; y++)
                        {
                            sheet.Cells[x + 2, y + 1].Value = table.Rows[x].GetStringValue(y);
                        }
                    }
                }

                package.SaveAs(new FileInfo(path));
            }

            MessageBox.Show(this, "Table(s) exported to Excel!");
        }

        private void CompareToDATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DATCompareDialog datComp = new DATCompareDialog(currentDmsPath);
            datComp.ShowDialog(this);
        }

        private void HideEmptyTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.HideEmptyTables = hideEmptyTablesToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void IgnoreVersionToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IgnoreVersion = ignoreVersionToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void IgnoreDatesTimesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IgnoreDates = ignoreDatesTimesToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ColumnHeader header = new ColumnHeader();
            header.Text = "";
            header.Name = "col1";
            header.Width = tableList.Width - 25;
            tableList.Columns.Add(header);
        }
    }
}