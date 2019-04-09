using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DMSLib;

namespace DMS_Viewer
{
    public partial class SQLGeneratorOptions : Form
    {
        private DMSFile dmsFile;
        private string filePath;
        private List<DMSTable> selectedTables = null;

        public SQLGeneratorOptions()
        {
            InitializeComponent();
        }

        public SQLGeneratorOptions(DMSFile dmsFile, string filePath, List<DMSTable> tables) : this(dmsFile, filePath)
        {
            selectedTables = tables;
        }

        public SQLGeneratorOptions(DMSFile dmsFile, string filePath) : this()
        {
            /*bool padColumns, bool extractLongs, bool ignoreEmptyTables */
            UpdateDMSInfo(dmsFile, filePath);
        }

        public void UpdateDMSInfo(DMSFile file, string path)
        {
            dmsFile = file;
            filePath = path;

            var fi = new FileInfo(filePath);
            textBox4.Text = fi.DirectoryName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == null || Directory.Exists(textBox4.Text) == false)
            {
                MessageBox.Show("Please specify and existing directory for the output directory.");
                return;
            }

            SQLGenerator.GenerateSQLFile(dmsFile, selectedTables, textBox4.Text, checkBox1.Checked, checkBox2.Checked,
                checkBox3.Checked, textBox1.Text);
            MessageBox.Show("SQL Generated Successfully!");
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox4.Text;
            var result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBox4.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}