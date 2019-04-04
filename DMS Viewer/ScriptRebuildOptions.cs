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
    public partial class ScriptRebuildOptions : Form
    {
        private string currentDmsPath;

        public ScriptRebuildOptions()
        {
            InitializeComponent();
        }

        public ScriptRebuildOptions(string currentDmsPath) :this()
        {
            if (currentDmsPath == null)
            {
                currentDmsPath = "";
            }
            this.currentDmsPath = currentDmsPath;
            textBox1.Text = currentDmsPath;
            if (currentDmsPath.Equals(""))
            {
                return;
            }
            var fi = new FileInfo(currentDmsPath);
            textBox3.Text = fi.Name.Replace(fi.Extension,"");
            textBox4.Text = fi.DirectoryName;
        }

        public void UpdateDMSPath(string path)
        {
            currentDmsPath = path;
            textBox1.Text = currentDmsPath;
            if(currentDmsPath.Equals(""))
            {
                return;
            }
            var fi = new FileInfo(currentDmsPath);
            textBox3.Text = fi.Name.Replace(fi.Extension, "");
            textBox4.Text = fi.DirectoryName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "DMS DAT Files|*.dat;*.DAT";
            openFileDialog1.FileName = "";
            var result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBox2.Text = "";
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox2.Text;
            var result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBox1.Text = "";
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
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

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                /* single file mode */
                DMSRebuilder.RebuildSingleFile(textBox1.Text, textBox4.Text, textBox3.Text);
                this.Close();
            } 

            if (textBox2.Text.Length > 0)
            {
                /* multi file mode */
                DMSRebuilder.RebuildDirectory(textBox2.Text, textBox4.Text, textBox3.Text);
                this.Close();
            }
        }
    }
}
