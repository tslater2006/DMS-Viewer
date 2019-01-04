using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    public partial class DBLogin : Form
    {
        public OracleConnection Connection;
        public DBLogin()
        {
            InitializeComponent();
        }

        private void btnDBConnect_Click(object sender, EventArgs e)
        {
            Connection = new OracleConnection($"Data Source={txtDBName.Text};User Id={txtDBUser.Text}; Password={txtDBPass.Text}");
            try
            {
                Connection.Open();
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }catch(Exception ex)
            {
                MessageBox.Show("Failed to connect, please check your information and ensure TNS_ADMIN environment variable is set properly.");
            }
        }
    }
}
