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
        ImageList _images = new ImageList();
        Dictionary<string, ListViewItem> _listItems = new Dictionary<string, ListViewItem>();
        Processes _processList = new Processes();

        public ConnectionsView(Connections cEngine, IRunForm form, ListView list)
        {
            _cEngine = cEngine;
            _list = list;
            _list.SmallImageList = _images;
            _list.LargeImageList = _images;
            _images.ImageSize = new Size(20, 20);
            _images.ColorDepth = ColorDepth.Depth32Bit;
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
            ListViewItem item = new ListViewItem();
            try
            {
                var proc = _processList[con.Pid];
                con.ExePath = proc.Process.MainModule.FileName;
                con.ExeName = proc.Process.ProcessName;
                if(!_images.Images.ContainsKey(proc.Pid.ToString()))
                {
                    if (proc.Icon != null)
                    {
                        _images.Images.Add(con.Pid.ToString(), proc.Icon);
                    } 
                    else
                    {
                        _images.Images.Add(con.Pid.ToString(), SystemIcons.Application);
                    }
                }

            }
            catch (Exception)
            {
                con.ExeName = "System process";
                con.ExePath = "_ System - no access";
                _images.Images.Add(con.Pid.ToString(), SystemIcons.Application);
            }
            item.ImageKey = con.Pid.ToString();
            item.SubItems.Add(FirstToUpper(con.ExeName));

            return item;
        }

        private void updateProcessInfo()
        {

        }
        private void AddItem(ListViewItem item, Connection con)
        {
            _listItems.Add(con.Key, item);
            _list.Items.Add(item);
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
