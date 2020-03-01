using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialmon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var aEngine = new Dialmon.Adapters();
            var cEngine = new Dialmon.Connections();
            var form1 = new Form1(aEngine, cEngine);
            aEngine.StartMonitor();
            cEngine.StartMonitor();
            
            Application.ApplicationExit += (object sender, EventArgs e) =>
            {
                aEngine.StopMonitor();
                cEngine.StopMonitor();
            };
            Application.Run(form1);
        }
    }
}
