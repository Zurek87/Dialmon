using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialmon.Dialmon
{
    public class Processes
    {
        private static Dictionary<int, ProcessInfo> _processList = new Dictionary<int, ProcessInfo>();

        static Processes()
        {
            initList();
        }

        public ProcessInfo this[int pid]
        {
            get
            {
                if (_processList.ContainsKey(pid)) return _processList[pid];
                var process = Process.GetProcessById(pid);
                var pinfo = new ProcessInfo()
                {
                    Process = process
                };
                _processList.Add(process.Id, pinfo);
                return _processList[pid];
            }
        }

        private static void initList()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                var pinfo = new ProcessInfo()
                {
                    Process = process
                };
                _processList.Add(process.Id, pinfo);
            }
        }
    }
}
