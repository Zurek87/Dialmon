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

        Connections _cEngine;
        ListView _listView;
        IRunForm _form;

        List<string> _itemsToShow = new List<string>();
        ImageList _images = new ImageList();
        Dictionary<string, ListViewGroup> _listGroups = new Dictionary<string, ListViewGroup>();
        ConcurrentQueue<string> _newItems = new ConcurrentQueue<string>();
        ConcurrentQueue<string> _newItemsInView = new ConcurrentQueue<string>();

        Processes _processList = new Processes();
        private Dictionary<String, Connection> _connections = new Dictionary<string, Connection>();
        private bool _isFullAddNeed = true;
        private bool _filters = false; // change only in form thread!

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
            _form.RunInFormThread(UpdateListView);
            //
        }
        private void UpdateListView()
        {
            UpdateGroupInNew();
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

        private void ResetViewConnections()
        {
            _listView.Items.Clear();
            ListViewItem[] items;
            if(_filters)
            {
                items = _connections.Where(con => _itemsToShow.Contains(con.Key)).Select(con => con.Value.Item).ToArray();
            } else
            {
                items = _connections.Select(con => con.Value.Item).ToArray();
            }
            _listView.Items.AddRange(items.ToArray());
        }

        private void UpdateViewConnections()
        {
            //
            while (_newItemsInView.Count > 0)
            {
                string key;
                if (_newItemsInView.TryDequeue(out key))
                {
                    var con = _connections[key];
                    _listView.Items.Add(con.Item);
                }
            }
            var list = _connections.Where(con => con.Value.Archived).Select(con => con.Value).ToList();
            list.ForEach(con => con.Item.ForeColor = Color.Brown);
        }

        private void UpdateGroupInNew()
        {
            while(_newItems.Count > 0)
            {
                string key;
                if(_newItems.TryDequeue(out key))
                {
                    var con = _connections[key];
                    con.Item.Group = GetOrCreateGroup(con.ExePath);
                    var proc = _processList[con.Pid];
                    if (!_images.Images.ContainsKey(con.Pid.ToString()))
                    {
                        _images.Images.Add(con.Pid.ToString(), proc.Icon);
                    }
                }
            }
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
            return group;
        }

        private void UpdateConnectionsList()
        {
            var connectionInfos = _cEngine.ConnectionList;
            var keys = _connections.Keys.ToArray();
            foreach (var key in keys)
            {
                var con = _connections[key];
                con.Archived = true;
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
                    _newItems.Enqueue(con.Key);
                    if(!_isFullAddNeed) _newItemsInView.Enqueue(con.Key); // filter !!
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
