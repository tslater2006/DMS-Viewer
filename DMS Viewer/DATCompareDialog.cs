using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DMSLib;

namespace DMS_Viewer
{
    public partial class DATCompareDialog : Form
    {
        DMSFile leftFile = null;
        DMSFile rightFile = null;

        public DATCompareDialog(string initialPath)
        {
            InitializeComponent();
            if (initialPath?.Length > 0)
            {
                leftFile = DMSReader.Read(initialPath);
                leftFile.FileName = new FileInfo(initialPath).Name;
            }

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

                list.Items.Add(new ListViewItem() {Tag = table, Text = table.Name, BackColor = backgroundColor});
            }

            list.Items[0].Selected = true;
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

            worker.RunWorkerCompleted += (o, args) => { UpdateUI(false); };

            worker.RunWorkerAsync();
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

            worker.RunWorkerCompleted += (o, args) => { UpdateUI(true); };

            worker.RunWorkerAsync();
        }
    }

    class CompareWorker : BackgroundWorker
    {
        private DMSFile file;
        private DMSTable[] tables;

        public CompareWorker(DMSTable[] selected, DMSFile target)
        {
            tables = selected;
            file = target;

            WorkerReportsProgress = true;
            DoWork += OnDoWork;
        }


        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var table in tables)
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
                    foreach (var row in table.Rows)
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
                    }

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
        }

        void CompareRows(DMSRow left, DMSRow right, int[] keyFields)
        {
            /* check for "same" first */
            if (left.Values.SequenceEqual(right.Values))
            {
                /* rows are identical */
                left.CompareResult = DMSCompareResult.SAME;
                return;
            }

            /* do the rows have the same keys? if so its changed */
            if (keyFields.Select(left.GetStringValue).SequenceEqual(keyFields.Select(right.GetStringValue)))
            {
                left.CompareResult = DMSCompareResult.UPDATE;
            }
            else
            {
                left.CompareResult = DMSCompareResult.NONE;
            }
        }
    }
}