using Dialmon.Dialmon;
using Dialmon.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialmon
{
    public partial class Form1 : Form, IRunForm
    {
        delegate void RunInFormThreadCallback();
        Adapters _aEngine;
        Connections _cEngine;
        public Form1(Adapters aEngine, Connections cEngine)
        {
            _aEngine = aEngine;
            _cEngine = cEngine;
            InitializeComponent();
            AdaptersView av = new AdaptersView(_aEngine,this, adaptersList);
            ConnectionsView cv = new ConnectionsView(_cEngine, this, connectionList);
        }

        public void RunInFormThread(Action action)
        {
            RunInFormThreadCallback callback = new RunInFormThreadCallback(action);
            try
            {
                this.Invoke(callback);
            }
            catch (Exception)
            {
                // throw
            }
        }
    }
}
