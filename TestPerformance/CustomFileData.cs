using ProgramLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPerformance
{
    public class CustomFileData : FileData
    {


        public CustomFileData(string x, string y, string z) :
            base(x,y,z)
        {
            this.val = 0;
        }
        
        public int val { get; set; }
    }
}
