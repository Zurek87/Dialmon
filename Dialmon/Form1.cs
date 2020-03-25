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
        ConnectionsView _connectionView;
        SpeedChart _speedChart;

        public Form1(Adapters aEngine, Connections cEngine)
        {
            _aEngine = aEngine;
            _cEngine = cEngine;
            InitializeComponent();
            AdaptersView av = new AdaptersView(_aEngine,this, adaptersList);
           _connectionView = new ConnectionsView(_cEngine, this, connectionList);
            _speedChart = new SpeedChart(this, _aEngine, pictureBox1);

            _connectionView.OnUpdateGroup += GropsUpdated;
        }

        public void RunInFormThread(Action action)
        {
            RunInFormThreadCallback callback = new RunInFormThreadCallback(action);
            try
            {
                Invoke(callback);
            }
            catch (Exception)
            {
                // throw
            }
        }

        private void GropsUpdated()
        {
            RunInFormThread(() => {
                var groups = _connectionView.Groups;
                cbGroup.Items.Clear();
                cbGroup.Items.AddRange(groups);
            });
            
        }
    
        private void connectionList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void adaptersList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            } 
            else
            {
                Hide();
                WindowState = FormWindowState.Minimized;
            }
        }

        private void cbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            _connectionView.Filters.GroupName = cbGroup.Text;
        }

        private void txtBoxIP_TextChanged(object sender, EventArgs e)
        {
            _connectionView.Filters.IpFilter = txtBoxIP.Text;
        }

        private void txtBoxPort_TextChanged(object sender, EventArgs e)
        {
            _connectionView.Filters.PortFilter = txtBoxPort.Text;
        }
    }
}
