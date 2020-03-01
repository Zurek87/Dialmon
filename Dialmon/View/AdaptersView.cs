using Dialmon.Dialmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialmon.View
{
    class AdaptersView
    {
        private static readonly string[] speedUnits = { "b/s", "kb/s", "Mb/s", "Gb/s", "Tb/s" };
        private static readonly string[] sizeUnits = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

        Adapters _aEngine;
        ListView _list;
        IRunForm _form;
        Dictionary<string, ListViewItem> _listItems = new Dictionary<string, ListViewItem>();
        public AdaptersView(Adapters aEngine, IRunForm form, ListView list)
        {
            _aEngine = aEngine;
            _list = list;
            _form = form;
            _aEngine.OnUpdate += onUpdateAdapters;
        }

        private void onUpdateAdapters()
        {
            _form.RunInFormThread(UpdateAdaptersList);
        }

        private void UpdateAdaptersList()
        {

            foreach (AdapterInterface adapter in _aEngine.AdapterList.Values)
            {
                if (_listItems.ContainsKey(adapter.NetInterface.Id))
                {
                    var item = _listItems[adapter.NetInterface.Id];
                    UpdateItem(item, adapter.NetInterface);
                }
                else
                {
                    var item = CreateItem(adapter.NetInterface);
                    _listItems[adapter.NetInterface.Id] = item;
                    _list.Items.Add(item);
                }

            }

        }

        private void UpdateItem(ListViewItem item, NetworkInterface adapter)
        {
            Action<ListViewItem.ListViewSubItem, string> ifDiff = (x, y) => { if (x.Text != y) { x.Text = y; } };
            IPInterfaceStatistics ipStats = adapter.GetIPStatistics();
            ifDiff(item.SubItems[0], adapter.Description);
            ifDiff(item.SubItems[1], GetAdapterIP(adapter));
            ifDiff(item.SubItems[2], GetAdapterMAC(adapter));
            ifDiff(item.SubItems[3], GetAdapterType(adapter));
            ifDiff(item.SubItems[4], FriendlyBits(adapter.Speed));
            ifDiff(item.SubItems[5], GetAdapterStatus(adapter));
            ifDiff(item.SubItems[6], FriendlyBits(ipStats.BytesReceived, false));
            ifDiff(item.SubItems[7], FriendlyBits(ipStats.BytesSent, false));
            ifDiff(item.SubItems[8], GetIpStatsErrorsCount(ipStats));
        }

        private ListViewItem CreateItem(NetworkInterface adapter)
        {
            ListViewItem item = new ListViewItem(adapter.Description);
            IPInterfaceStatistics ipStats = adapter.GetIPStatistics();
            item.SubItems.Add(GetAdapterIP(adapter));
            item.SubItems.Add(GetAdapterMAC(adapter));
            item.SubItems.Add(GetAdapterType(adapter));
            item.SubItems.Add(FriendlyBits(adapter.Speed));
            item.SubItems.Add(GetAdapterStatus(adapter));
            item.SubItems.Add(FriendlyBits(ipStats.BytesReceived, false));
            item.SubItems.Add(FriendlyBits(ipStats.BytesSent, false));
            item.SubItems.Add(GetIpStatsErrorsCount(ipStats));
            item.SubItems[2].Text = "Lol";
            return item;
        }

        public static string FriendlyBits(Int64 val, bool perS = true)
        {

            if (val <= 0) return "0.00 " + (perS ? speedUnits[0] : sizeUnits[0]);
            double double_mag = Math.Log(val, 1024);
            int mag = (int)(double_mag + 0.000001); //fix precision problem
            decimal ret = (decimal)val / (1L << (mag * 10));

            return String.Format("{0:n2} {1}", ret, (perS ? speedUnits[mag] : sizeUnits[mag]));
        }

        public static string GetAdapterMAC(NetworkInterface adapter)
        {
            byte[] mac_bytes = adapter.GetPhysicalAddress().GetAddressBytes();
            if (mac_bytes.Length > 6) return "";
            return BitConverter.ToString(mac_bytes);
            //return adapter.GetPhysicalAddress().ToString();
        }

        public static String GetAdapterIP(NetworkInterface adapter)
        {
            foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.Address.ToString();
                }
            }
            return "";
        }

        public static String GetAdapterType(NetworkInterface adapter)
        {
            return adapter.NetworkInterfaceType.ToString();
        }

        public static String GetAdapterStatus(NetworkInterface adapter)
        {
            return adapter.OperationalStatus.ToString();
        }

        public static String GetIpStatsErrorsCount(IPInterfaceStatistics ipStats)
        {
            return String.Format("{0} / {1}", ipStats.IncomingPacketsWithErrors, ipStats.OutgoingPacketsWithErrors);
        }

    }
}
