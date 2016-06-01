using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.Common
{
    public interface IStoppable
    {
        void Stop();

        bool Stopped { get; }
    }
}
