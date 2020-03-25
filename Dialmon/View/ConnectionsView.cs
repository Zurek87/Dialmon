using Dialmon.Dialmon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialmon.View
{
    class ConnectionsView
    {
        readonly Action<ListViewItem.ListViewSubItem, string> ifDiff = (x, y) => { if (x.Text != y) { x.Text = y; } };
        public event OnUpdateDelegate OnUpdateGroup;

        Connections _cEngine;
        ListView _listView;
        IRunForm _form;

        List<string> _itemsToShow = new List<string>();
        ImageList _images = new ImageList();
        HashSet<string> _listGroups = new HashSet<string>();

        Processes _processList = new Processes();
        private Dictionary<String, Connection> _connections = new Dictionary<string, Connection>();
        private bool _isFullAddNeed = true;
        private bool _groupsUpdated = true;

        public string[] Groups => _listGroups.ToArray();

        public ConnectionFilters Filters;

        public ConnectionsView(Connections cEngine, IRunForm form, ListView listView)
        {
            _cEngine = cEngine;
            _listView = listView;
            _form = form;
            _cEngine.OnUpdate += OnUpdateConnections;

            _listView.SmallImageList = _images;
            _listView.LargeImageList = _images;
            _images.ImageSize = new Size(20, 20);
            _images.ColorDepth = ColorDepth.Depth32Bit;
        }

        private void OnUpdateConnections()
        {
            UpdateConnectionsList();
            if(_groupsUpdated)
            {
                OnUpdateGroup?.Invoke();
                _groupsUpdated = false;
            }
            FilterItems();
            _form.RunInFormThread(UpdateListView);
            //
        }
        private void UpdateListView()
        {
            if (_isFullAddNeed)
            {
                _isFullAddNeed = false;
                ResetViewConnections();
            }
            else
            {
                UpdateViewConnections();
            }
        }

        private void FilterItems()
        {
            var conn = _connections.Select(con => con.Value);
            if (!String.IsNullOrWhiteSpace(Filters.GroupName))
            {
                var name = Filters.GroupName.ToLower();
                conn = conn.Where(con => con.ExeName.ToLower().Contains(name));
            }
            if (!String.IsNullOrWhiteSpace(Filters.PortFilter))
            {
                conn = conn.Where(con =>
                        con.LocalPort.ToString().Contains(Filters.PortFilter)
                        || (con.Status != ConnectionStatus.listen && con.RemotePort.ToString().Contains(Filters.PortFilter))
                       
                    );
            }
            if (!String.IsNullOrWhiteSpace(Filters.IpFilter))
            {
                conn = conn.Where(con =>
                        con.LocalIP.ToString().Contains(Filters.IpFilter)
                        || (con.Status != ConnectionStatus.listen && con.RemoteIP.ToString().Contains(Filters.IpFilter))
                    );
            }
            var keys = conn.Select(con => con.Key).ToList();
            foreach (var key in keys)
            {
                var con = _connections[key];
                con.Visible = true;
                _connections[key] = con;
            }
        }

        private void ResetViewConnections()
        {
            _listView.Items.Clear();
            _connections.Select(con => con.Value).ToList().ForEach(con => BeforeItemAdd(con));
            _listView.Items.AddRange(_connections.Where(con => con.Value.Visible).Select(con => con.Value.Item).ToArray());
        }

        private void UpdateViewConnections()
        {
            //

            var list = _connections.Where(con => con.Value.Archived).Select(con => con.Value).ToList();
            list.ForEach(con => con.Item.ForeColor = Color.Brown);
            var time = DateTime.Now.AddSeconds(-30);
            var listToForget = _connections.Where(con => (con.Value.Archived && con.Value.LastUpdate < time)).Select(con => con.Value).ToList();
            foreach (var con in listToForget)
            {
                con.Item.Remove();
                _connections.Remove(con.Key);
            }

            var listToHide = _connections.Where(con => (!con.Value.Visible)).Select(con => con.Value).ToList();
            foreach (var con in listToHide)
            {
                con.Item.ListView?.Items.Remove(con.Item);
            }
            var listToShow = _connections.Where(con => (con.Value.Visible)).Select(con => con.Value).ToList();
            foreach (var con in listToShow)
            {
                BeforeItemAdd(con);
                if (con.Item.ListView == null)
                {
                    _listView.Items.Add(con.Item);
                    
                }
            }
        }

        private void BeforeItemAdd(Connection con) //run in form thread
        {
            var proc = _processList[con.Pid];
            if (!_images.Images.ContainsKey(con.Pid.ToString()))
            {
                _images.Images.Add(con.Pid.ToString(), proc.Icon);
            }
            _listGroups.Add(FirstToUpper(con.ExeName));
            con.Item.Group = GetOrCreateGroup(con.ExePath);
        }

        private ListViewGroup GetOrCreateGroup(string name)
        {
            // try find
            for(var i = 0; i<_listView.Groups.Count; i++)
            {
                var grp = _listView.Groups[i];
                if (grp.Header == name) return grp;
            }
            var group = new ListViewGroup(name);
            _listView.Groups.Add(group);
            _groupsUpdated = true;
            return group;
        }

        private void UpdateConnectionsList()
        {
            var connectionInfos = _cEngine.ConnectionList;
            var keys = _connections.Keys.ToArray();
            foreach (var key in keys)
            {
                var con = _connections[key];
                if (con.Archived)
                    continue;

                con.Archived = true;
                con.Visible = false;
                con.LastUpdate = DateTime.Now;
                _connections[key] = con;
            }
            foreach(var conInfo in connectionInfos)
            {
                if(_connections.ContainsKey(conInfo.Key))
                {
                    var con = _connections[conInfo.Key];
                    con.Archived = false;
                    con.Status = conInfo.Status;
                    _connections[conInfo.Key] = con;
                } 
                else
                {
                    var con = new Connection(conInfo);
                    CreateItem(con);
                    //con.Item.Group = GetOrCreateGroup(con.ExePath);
                    _connections.Add(con.Key, con);
                }
            }
        }

        private void CreateItem(Connection con)
        {
            ListViewItem item = new ListViewItem();
            try
            {
                var proc = _processList[con.Pid];
                if (proc.Pid >= 0)
                {
                    con.ExePath = proc.Process.MainModule.FileName;
                    con.ExeName = proc.Process.ProcessName;
                } else
                {
                    con.ExePath = "Process not found (probably close after short connection)";
                    con.ExeName = "Not Found - Pid:" + con.Pid.ToString();
                }
            }
            catch (Exception)
            {
                con.ExeName = "System process";
                con.ExePath = " System process no access";
            }
            item.ImageKey = con.Pid.ToString();
            item.SubItems.Add(FirstToUpper(con.ExeName));
            item.SubItems.Add(con.Pid.ToString());
            item.SubItems.Add(con.Status.ToString());
            item.SubItems.Add(con.LocalIP);
            item.SubItems.Add(con.LocalPort.ToString());
            if(con.Status != ConnectionStatus.listen)
            {
                item.SubItems.Add(con.RemoteIP);
                item.SubItems.Add(con.RemotePort.ToString());
            }
            else
            {
                item.SubItems.Add("");
                item.SubItems.Add("");
            }
           // 

            con.Item = item;
        }

        private string FirstToUpper(string input)
        {
            switch (input)
            {
                case null:
                case "": return "";
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}
