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
    public partial class SQLGeneratorOptions : Form
    {
        private DMSFile dmsFile;

        public SQLGeneratorOptions()
        {
            InitializeComponent();
        }

        public SQLGeneratorOptions(DMSFile dmsFile) :this()
        {
            /*bool padColumns, bool extractLongs, bool ignoreEmptyTables */
            this.dmsFile = dmsFile;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == null || Directory.Exists(textBox4.Text) == false)
            {
                MessageBox.Show("Please specify and existing directory for the output directory.");
                return;
            }
            SQLGenerator.GenerateSQLFile(dmsFile, textBox4.Text, checkBox1.Checked, checkBox2.Checked, checkBox3.Checked);
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
