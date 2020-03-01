using Dialmon.Dialmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialmon.View
{
    class ConnectionsView
    {
        Connections _cEngine;
        ListView _list;
        IRunForm _form;
        Dictionary<string, ListViewItem> _listItems = new Dictionary<string, ListViewItem>();

        public ConnectionsView(Connections cEngine, IRunForm form, ListView list)
        {
            _cEngine = cEngine;
            _list = list;
            _form = form;
            _cEngine.OnUpdate += onUpdateAdapters;
        }

        private void onUpdateAdapters()
        {
            _form.RunInFormThread(UpdateConnectionsList);
        }

        private void UpdateConnectionsList()
        {
            foreach(var con in _cEngine.ConnectionList.Values)
            {
                if (_listItems.ContainsKey(con.Key))
                {
                    UpdateItem(con);
                } 
                else
                {
                    var item = CreateItem(con);
                    AddItem(item, con);
                }
            }
        }

        private void UpdateItem(Connection con)
        {

        }
        private ListViewItem CreateItem(Connection con)
        {
            ListViewItem item = new ListViewItem(con.Pid.ToString());


            return item;
        }
        private void AddItem(ListViewItem item, Connection con)
        {
            _listItems.Add(con.Key, item);
            _list.Items.Add(item);
        }


    }
}
