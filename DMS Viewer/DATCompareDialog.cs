using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DMSLib;

namespace DMS_Viewer
{
    public partial class DATCompareDialog : Form
    {
        DMSFile leftFile = null;
        private string leftPath = "";
        DMSFile rightFile = null;
        private string rightPath = "";

        public DATCompareDialog(string initialPath)
        {
            InitializeComponent();
            if (initialPath?.Length > 0)
            {
                try
                {
                    leftFile = DMSReader.Read(initialPath);
                }catch (FormatException fe)
                {
                    MessageBox.Show(this, fe.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                leftFile.FileName = new FileInfo(initialPath).Name;
                leftPath = initialPath;
            }

            UpdateUI(true);
        }

        void UpdateUI(bool leftSide)
        {

            if (leftSide && leftFile == null)
            {
                return;
            }

            if (!leftSide && rightFile == null)
            {
                return;
            }

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

            var savedIndexes = list.SelectedIndices;

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

                list.Items.Add(new ListViewItem() {Tag = table, Text = table.Name, BackColor = backgroundColor});
            }

            if (savedIndexes.Count > 0)
            {
                foreach (var x in savedIndexes.Cast<int>())
                {
                    list.Items[x].Selected = true;
                }
            }
            else
            {
                list.Items[0].Selected = true;
            }

            btn.Enabled = true;

            /* enable the compare buttons? */
            if (leftFile != null && rightFile != null)
            {
                btnCompareRight.Enabled = true;
                btnCompareToLeft.Enabled = true;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DMSTable[] selectedTables =
                lstRight.SelectedItems.Cast<ListViewItem>().Select(i => (DMSTable) i.Tag).ToArray();

            var worker = new CompareWorker(selectedTables, leftFile);
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = selectedTables.Sum(t => t.Rows.Count);

            worker.ProgressChanged += (o, args) =>
            {
                progressBar1.Value += args.ProgressPercentage;
                progressBar1.Update();
            };

            worker.RunWorkerCompleted += (o, args) =>
            {
                UpdateUI(false);
                MessageBox.Show(@"Compare has completed!");
            };

            worker.RunWorkerAsync();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            GetDATFile(true);
            UpdateUI(true);
        }

        private void GetDATFile(bool isLeft)
        {
            openFileDialog1.Filter = @"Data Mover Data Files|*.dat;*.DAT";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                DMSFile dmsFile = null;

                try
                {
                    dmsFile = DMSReader.Read(openFileDialog1.FileName);
                }catch(FormatException fe)
                {
                    MessageBox.Show(this, fe.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                /* Set the file name */
                dmsFile.FileName = new FileInfo(openFileDialog1.FileName).Name;

                if (isLeft)
                {
                    leftFile = dmsFile;
                    leftPath = openFileDialog1.FileName;
                }
                else
                {
                    rightFile = dmsFile;
                    rightPath = openFileDialog1.FileName;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GetDATFile(false);
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

        private void CompareFiles(DMSTable[] selectedTables, DMSFile target)
        {
            /* this compares the rows for tables in Source to tables in Target */
            foreach (var table in selectedTables)
            {
                table.CompareResult = DMSCompareResult.NEW;
            }
        }

        private void btnCompareRight_Click(object sender, EventArgs e)
        {
            DMSTable[] selectedTables =
                lstLeft.SelectedItems.Cast<ListViewItem>().Select(i => (DMSTable) i.Tag).ToArray();

            var worker = new CompareWorker(selectedTables, rightFile);
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = selectedTables.Sum(t => t.Rows.Count);

            worker.ProgressChanged += (o, args) =>
            {
                progressBar1.Value += args.ProgressPercentage;
                progressBar1.Update();
            };

            worker.RunWorkerCompleted += (o, args) =>
            {
                UpdateUI(true);
                MessageBox.Show(@"Compare has completed!");
            };

            worker.RunWorkerAsync();
        }

        private void lstLeft_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var selectedTables =
                    lstLeft.SelectedItems.Cast<ListViewItem>().Select(i => (DMSTable) i.Tag).ToList();
                if (selectedTables.Count > 0)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem generateSQL = new MenuItem("Generate SQL...");
                    generateSQL.Tag = selectedTables;
                    generateSQL.Click += (o, args) =>
                    {
                        var sqlGen = new SQLGeneratorOptions(leftFile, leftPath, selectedTables);
                        sqlGen.ShowDialog(this);
                    };

                    m.MenuItems.Add(generateSQL);

                    if (leftFile.Tables.Any(t =>
                        t.CompareResult == DMSCompareResult.NEW || t.CompareResult == DMSCompareResult.UPDATE))
                    {
                        MenuItem saveDiffs = new MenuItem("Save DAT diff...");
                        saveDiffs.Tag = selectedTables;
                        saveDiffs.Click += (o, args) => { SaveDATDiff(leftFile); };
                        m.MenuItems.Add(saveDiffs);
                    }

                    m.Show(lstLeft, new Point(e.X, e.Y));
                }
            }
        }

        private void SaveDATDiff(DMSFile file)
        {
            saveFileDialog1.Filter = @"Data Mover Data Files|*.dat;*.DAT";
            var result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                DMSWriter.Write(saveFileDialog1.FileName, file, true);
            }
        }

        private void lstRight_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var selectedTables =
                    lstRight.SelectedItems.Cast<ListViewItem>().Select(i => (DMSTable) i.Tag).ToList();
                if (selectedTables.Count > 0)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem generateSQL = new MenuItem("Generate SQL...");
                    generateSQL.Tag = selectedTables;
                    generateSQL.Click += (o, args) =>
                    {
                        var sqlGen = new SQLGeneratorOptions(rightFile, rightPath, selectedTables);
                        sqlGen.ShowDialog(this);
                    };
                    m.MenuItems.Add(generateSQL);

                    if (rightFile.Tables.Any(t =>
                        t.CompareResult == DMSCompareResult.NEW || t.CompareResult == DMSCompareResult.UPDATE))
                    {
                        MenuItem saveDiffs = new MenuItem("Save DAT diff...");
                        saveDiffs.Tag = selectedTables;
                        saveDiffs.Click += (o, args) => { SaveDATDiff(rightFile); };
                        m.MenuItems.Add(saveDiffs);
                    }

                    m.Show(lstRight, new Point(e.X, e.Y));
                }
            }
        }
    }

    class CompareWorker : BackgroundWorker
    {
        private DMSFile file;
        private DMSTable[] tables;

        bool ignoreVersion = Properties.Settings.Default.IgnoreVersion;
        bool ignoreDates = Properties.Settings.Default.IgnoreDates;

        public CompareWorker(DMSTable[] selected, DMSFile target)
        {
            tables = selected;
            file = target;

            WorkerReportsProgress = true;
            DoWork += OnDoWork;
        }


        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            Parallel.ForEach(tables, table =>
                {
                    /* determine if this table exists in target file */
                    var targetTables = file.Tables.Where(t => t.Name == table.Name).ToList();
                    if (targetTables.Count == 0)
                    {
                        foreach (var row in table.Rows)
                        {
                            row.CompareResult = DMSCompareResult.NEW;
                        }

                        ReportProgress(table.Rows.Count);
                    }
                    else
                    {
                        var keyFieldIndexes = table.Metadata.FieldMetadata
                            .Where(m => m.UseEditMask.HasFlag(UseEditFlags.KEY))
                            .Select(t => table.Columns.IndexOf(table.Columns
                                .First(c => c.Name == t.FieldName))).ToArray();

                        /* for each row in source table, compare against target tables */
                        Parallel.ForEach(table.Rows, row =>
                            {
                                row.CompareResult = DMSCompareResult.NONE;

                                foreach (var targetRow in targetTables.SelectMany(t => t.Rows))
                                {
                                    CompareRows(row, targetRow, keyFieldIndexes);
                                    if (row.CompareResult != DMSCompareResult.NONE)
                                    {
                                        break;
                                    }
                                }

                                if (row.CompareResult == DMSCompareResult.NONE)
                                {
                                    row.CompareResult = DMSCompareResult.NEW;
                                }

                                ReportProgress(1);
                            }
                        );

                        if (table.Rows.Any(r => r.CompareResult == DMSCompareResult.UPDATE))
                        {
                            table.CompareResult = DMSCompareResult.UPDATE;
                        }
                        else
                        {
                            table.CompareResult = table.Rows.Any(r => r.CompareResult == DMSCompareResult.NEW)
                                ? DMSCompareResult.NEW
                                : DMSCompareResult.SAME;
                        }
                    }
                }
            );
        }

        void CompareRows(DMSRow left, DMSRow right, int[] keyFields)
        {
            bool isSame = false;
            if (left.ValueHash == right.ValueHash)
            {
                isSame = true;
                if (!ignoreDates)
                {
                    isSame = left.DateHash == right.DateHash;
                }

                if (!ignoreVersion && isSame)
                {
                    isSame = left.VersionHash == right.VersionHash;
                }
            }

            if (isSame)
            {
                left.CompareResult = DMSCompareResult.SAME;
            } else
            {
                if (left.KeyHash == right.KeyHash)
                {
                    left.CompareResult = DMSCompareResult.UPDATE;
                } else
                {
                    left.CompareResult = DMSCompareResult.NEW;
                }
            }
        }
    }
}