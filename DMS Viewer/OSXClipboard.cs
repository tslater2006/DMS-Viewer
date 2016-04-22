using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS_Viewer
{
    public class OSXClipboard
    {
        public static void CopyToClipboard(string text)
        {
            using (var p = new Process())
            {

                p.StartInfo = new ProcessStartInfo("pbcopy", "-pboard general -Prefer txt");
                p.StartInfo.RedirectStandardOutput = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.StandardInput.Write(text);
                p.StandardInput.Close();
                p.WaitForExit();
            }
        }
    }
}
