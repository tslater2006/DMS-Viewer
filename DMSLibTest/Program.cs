using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMSLib;
namespace DMSLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DATFile dat = new DATFile();
            dat.LoadFromFile(@"C:\Users\tslat\Desktop\export.DAT");
            //dat.LoadFromFile(@"C:\Users\tslat\Downloads\ITG_131044.DAT");
        }
    }
}
