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
    public partial class LongDataViewer : Form
    {
        private bool IsRunningMono = false;
        private bool IsRunningOSX = false;

        private DMSRow tableRow;
        private int colIndex;
        private DataViewer viewerForm;
        public LongDataViewer(string content, DataViewer viewerForm, DMSRow row, int columnIndex): this(content)
        {
            tableRow = row;
            colIndex = columnIndex;
            this.viewerForm = viewerForm;
            textBox1.ReadOnly = false;
            this.Text = "Data Editor";
            button1.Text = "Save Changes";
        }

        public LongDataViewer(string content)
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            textBox1.Text = content;

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

        private void button1_Click(object sender, EventArgs e)
        {
            if (tableRow != null)
            {
                tableRow.Values[colIndex] = textBox1.Text;
                viewerForm.DrawDataTable();
                this.Close();
            }
            if (IsRunningOSX)
            {
                OSXClipboard.CopyToClipboard(textBox1.Text);
            }
            else
            {
                Clipboard.SetText(textBox1.Text);
            }
        }
    }
}
