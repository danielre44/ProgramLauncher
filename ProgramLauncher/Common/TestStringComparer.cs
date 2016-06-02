using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.Common
{
    class TestStringComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return Compare((string) x, (string) y);
        }
        
    }
}
