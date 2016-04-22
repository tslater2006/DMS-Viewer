﻿using System;
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
                DMSParser parser = new DMSParser();
                currentDmsPath = openFileDialog1.FileName;
                dmsFile = parser.ParseFile(currentDmsPath);
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
            generateSQLToolStripMenuItem.Enabled = true;
            dataViewer.Enabled = true;
            foreach(var table in dmsFile.Tables)
            {
                tableList.Items.Add(table);
            }
            tableList.SelectedIndex = 0;
        }

        private void tableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var value = tableList.SelectedItem as DMSTable;
            whereClause.Text = value.WhereClause;
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
    }
}
