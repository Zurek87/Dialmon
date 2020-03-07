
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Dialmon.Dialmon
{
    public delegate void OnUpdateDelegate();

    public enum ConnectionStatus
    {
        closed = 1,
        listen,
        syn_sent, // MIB_TCP_STATE_SYN_SENT
        syn_rcvd, // MIB_TCP_STATE_SYN_RCVD
        estab, // MIB_TCP_STATE_ESTAB
        fin_wait1, // MIB_TCP_STATE_FIN_WAIT1
        fin_wait2, // MIB_TCP_STATE_FIN_WAIT2
        close_wait, // MIB_TCP_STATE_CLOSE_WAIT
        closing, // MIB_TCP_STATE_CLOSING
        last_ack, // MIB_TCP_STATE_LAST_ACK
        time_wait, // MIB_TCP_STATE_TIME_WAIT
        delete_tcb,// MIB_TCP_STATE_DELETE_TCB
    }

    public struct ProcessInfo
    {
        private Process _process;
        public int Pid;
        public Icon Icon;
        public Process Process
        {
            get => _process;
            set
            {
                _process = value;
                try
                {
                    Icon = Icon.ExtractAssociatedIcon(_process.MainModule.FileName);
                }
                catch (Exception)
                {
                    Icon = SystemIcons.Application;
                }
                
            }
        }
    }

    public struct Connection
    {
        private string _key;
        public int Pid;
        public string RemoteIP;
        public ushort RemotePort;
        public ushort LocalPort;
        public string LocalIP;
        public ConnectionStatus Status;
        public bool Archived;
        public string ExeName; // exe file name or "System process" 
        public string ExePath; // full path or "_ System - no access"
        public ListViewItem Item;
        public string Key
        {
            get
            {
                if (_key == null)
                {
                    _key = String.Format("{0}.{1}.{2}.{3}.{4}", Pid, LocalIP, LocalPort, RemoteIP, RemotePort);
                }
                return _key;
            }
        }
    }
    public struct AdapterInterface
    {
        Queue<IPv4InterfaceStatistics> _ipV4Statistics;
        IPv4InterfaceStatistics _actualV4Stat;

        public NetworkInterface NetInterface;
        public IPv4InterfaceStatistics[] IpV4Statistics { get { return _ipV4Statistics.ToArray(); } }
        public IPv4InterfaceStatistics ActualV4Statistics
        {
            get { return _actualV4Stat; }
            set
            {
                if (_ipV4Statistics == null) _ipV4Statistics = new Queue<IPv4InterfaceStatistics>();
                _actualV4Stat = value;
                _ipV4Statistics.Enqueue(value);
                if (_ipV4Statistics.Count > 300) _ipV4Statistics.Dequeue();
            }
        }
    }
}