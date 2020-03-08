using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialmon.View
{
    interface IRunForm
    {
        void RunInFormThread(Action action);
    }
}
