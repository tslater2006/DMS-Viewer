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
