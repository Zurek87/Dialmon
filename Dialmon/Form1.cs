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
        ImageList _images = new ImageList();
        Dictionary<string, ListViewGroup> _listGroups = new Dictionary<string, ListViewGroup>();
        Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();

        public Form1(Adapters aEngine, Connections cEngine)
        {
            _aEngine = aEngine;
            _cEngine = cEngine;
            InitializeComponent();

            connectionList.SmallImageList = _images;
            connectionList.LargeImageList = _images;
            _images.ImageSize = new Size(20, 20);
            _images.ColorDepth = ColorDepth.Depth32Bit;

            AdaptersView av = new AdaptersView(_aEngine,this, adaptersList);
           _connectionView = new ConnectionsView(_cEngine, this, connectionList);
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

        public void OnUpdateConnections()
        {
            var connections = _connectionView.Connections;
            UpdateConnectionDetails(connections.ToArray());
            var items = GetItemsFiltered();
            connectionList.Items.Clear();
            connectionList.Items.AddRange(items);
        }


        private void UpdateConnectionDetails(Connection[] connections)
        {
            foreach (var conNew in connections)
            {
                Connection con = _connections.ContainsKey(conNew.Key) ? _connections[conNew.Key] : conNew;
                con.Archived = conNew.Archived;
                con.Status = conNew.Status;
                if (!_images.Images.ContainsKey(con.Pid.ToString()))
                {
                    var proc = _connectionView.Processes[con.Pid];
                    if (proc.Icon != null)
                    {
                        _images.Images.Add(con.Pid.ToString(), proc.Icon);
                    }
                    else
                    {
                        _images.Images.Add(con.Pid.ToString(), SystemIcons.Application);
                    }
                }
                if(con.Item.Group == null)
                {
                    con.Item.Group = GetOrCreateListViewGrop(con.ExePath);
                }
                _connections[con.Key] = con;
            }
        }

        private ListViewGroup GetOrCreateListViewGrop(string name)
        {
            if (_listGroups.Keys.Contains(name)) return _listGroups[name];
            var group = new ListViewGroup(name);
            _listGroups.Add(name, group);
            connectionList.Groups.Add(group);
            return group;
        }

        private ListViewItem[] GetItemsFiltered()
        {
            var items = _connections.Values.Where(c => c.Status == ConnectionStatus.listen);
                
                

            return _connections.Values.Select(c => c.Item).ToArray();
        }
    

        private void connectionList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
