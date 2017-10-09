using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSWriter
    {
        public static void Write(string path, DMSFile file)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            using (StreamWriter sw = new StreamWriter(File.OpenWrite(path)))
            {
                file.WriteToStream(sw);
            }

        }
    }
}
