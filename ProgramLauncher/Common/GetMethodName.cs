using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.Common
{
    public static class GetMethodName
    {
        public static string GetCallingName([CallerMemberName] string callerName = "")
        {
            return callerName;
        }
    }
}
