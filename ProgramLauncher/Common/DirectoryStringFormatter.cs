using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.Common
{
    public static class DirectoryStringFormatter
    {

        public static string Format(string input)
        {
            string temp = input.Replace(@"/", @"\").ToLower();
            
            while (temp.Contains(@"\\"))
            {
                temp = temp.Replace(@"\\", @"\");
            }

            return temp;
        }

    }
}
