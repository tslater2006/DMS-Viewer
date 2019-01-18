using DMSLib;
using Oracle.ManagedDataAccess.Client;
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
    public partial class FindAndReplace : Form
    {
        private DMSFile inFile;

        public FindAndReplace(DMSFile file)
        {
            InitializeComponent();
            inFile = file;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("This will replace *ALL* occurences (case sensitive) in all exported tables/columns. Continue?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var count = 0;
                foreach (var table in inFile.Tables)
                {
                    foreach (var row in table.Rows)
                    {
                        for (var x = 0; x < table.Columns.Count - 1; x++)
                        {
                            if (row.GetStringValue(x).Equals(txtSearch.Text))
                            {
                                count++;
                                /* found a match, replace*/
                                row.ChangeValue(x, txtReplace.Text);
                            }
                        }
                    }
                }
                MessageBox.Show("Replaced " + count + " occurences.");
            }
        }
    }
}
