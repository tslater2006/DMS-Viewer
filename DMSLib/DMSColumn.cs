using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLib
{
    public class DMSColumn
    {
        public string Name;
        public string Type;
        public string Size;

        public override string ToString()
        {
            return Name + ":" + Type + "(" + Size + ")";
        }
    }
}
