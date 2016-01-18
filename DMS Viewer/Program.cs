using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMS_Viewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DMSParser parser = new DMSParser();
            var result = parser.ParseFile(@"C:\Users\tslat\Desktop\IS_TP_CHG_01_01_00_02_TREE.DAT");

            
            Application.Run(new Form1());
        }
    }
}
