using Dialmon.Dialmon;
using System;
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
        ListView _list;
        IRunForm _form;
        Dictionary<string, ListViewItem> _listItems = new Dictionary<string, ListViewItem>();
        
       
        Processes _processList = new Processes();
        private Dictionary<String, Connection> _connections = new Dictionary<string, Connection>();

        public ConnectionsView(Connections cEngine, IRunForm form, ListView list)
        {
            _cEngine = cEngine;
            _list = list;
            _form = form;
            _cEngine.OnUpdate += OnUpdateAdapters;
        }

        public Processes Processes => _processList;

        public List<Connection> Connections => _connections.Values.ToList();

        private void OnUpdateAdapters()
        {
            UpdateConnectionsList();
            _form.RunInFormThread(_form.OnUpdateConnections);
        }

        private void UpdateConnectionsList()
        {
           
            var list = _cEngine.ConnectionList;
            for (var i = 0; i < list.Count(); i++)
            {
                var con = _cEngine.ConnectionList[i];
                if (_connections.ContainsKey(con.Key))
                {
                    
                    var knowCon = _connections[con.Key];
                    knowCon.Archived = con.Archived;
                    knowCon.Status = con.Status;
                    _connections[knowCon.Key] = knowCon;
                    UpdateItem(knowCon);
                    //con.Item = _listItems[con.Key];
                    //knowCon.Item.Group = GetOrCreateListViewGrop(knowCon.ExePath);
                } 
                else
                {
                    CreateItem(ref con);
                    AddItem(con);
                    _connections.Add(con.Key, con);
                }
                
            }
        }

        private void UpdateItem(Connection con)
        {

        }


        private void CreateItem(ref Connection con)
        {
            ListViewItem item = new ListViewItem();
            try
            {
                var proc = _processList[con.Pid];
                con.ExePath = proc.Process.MainModule.FileName;
                con.ExeName = proc.Process.ProcessName;

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
           // item.Group = GetOrCreateListViewGrop(con.ExePath);

            con.Item = item;
        }

        private void updateProcessInfo()
        {

        }
        private void AddItem(Connection con)
        {
            _listItems.Add(con.Key, con.Item);
            //_list.Items.Add(con.Item);
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
