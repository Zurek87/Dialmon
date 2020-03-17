using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Dialmon.Dialmon
{
    public class Connections : DialmonThread
    {
        private List<ConnectionInfo> _connections = new List<ConnectionInfo>();
        public event OnUpdateDelegate OnUpdate;
        public ConnectionInfo[] ConnectionList { get { return _connections.ToArray(); } }


        [DllImport("iphlpapi.dll", SetLastError = true)]
        static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TCP_TABLE_CLASS tblClass, int reserved);

        protected override void UpdateThread()
        {
            Thread.Sleep(_refreshTime/2);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (_enabled)
            {
                UpdateConnections();
                OnUpdate?.Invoke();

                if (_enabled && _refreshTime >= stopWatch.ElapsedMilliseconds)
                {
                    long sleep = _refreshTime - stopWatch.ElapsedMilliseconds - 1;
                    
                    if (sleep > 0) Thread.Sleep((int)sleep);
                }
                stopWatch.Restart();
            }
        }

        private void UpdateConnections()
        {
            _connections = GetConnectionList().ToList();
        }

        private ConnectionInfo[] GetConnectionList()
        {
            var rawConnections = GetAllTcpConnections();
            var connections = new List<ConnectionInfo>();
            foreach (var raw in rawConnections)
            {
                connections.Add(ConnectionInformation(raw));
            }

            return connections.ToArray();
        }

        private ConnectionInfo ConnectionInformation(MIB_TCPROW_OWNER_PID rawConection)
        {
            return new ConnectionInfo()
            {
                LocalPort = rawConection.LocalPort,
                LocalIP = rawConection.LocalIP,
                RemotePort = rawConection.RemotePort,
                RemoteIP = rawConection.RemoteIP,
                Pid = rawConection.Pid,
                Status = (ConnectionStatus)rawConection.state,
            };
        }

        private MIB_TCPROW_OWNER_PID[] GetAllTcpConnections()
        {
            //Based on http://stackoverflow.com/questions/577433/which-pid-listens-on-a-given-port-in-c-sharp
            MIB_TCPROW_OWNER_PID[] tTable;
            int AF_INET = 2;    // IP_v4
            int bufferSize = 0;

            uint ret = GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            if (ret != 0 && ret != 122)
            {
                throw new Exception("bad ret on check " + ret);
            }
            IntPtr buffTable = Marshal.AllocHGlobal(bufferSize);

            try
            {
                ret = GetExtendedTcpTable(buffTable, ref bufferSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                if (ret != 0)
                {
                    throw new Exception("bad ret " + ret);
                }

                // get the number of entries in the table
                MIB_TCPTABLE_OWNER_PID tab = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_TCPTABLE_OWNER_PID));

                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new MIB_TCPROW_OWNER_PID[tab.dwNumEntries];

                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    MIB_TCPROW_OWNER_PID tcpRow = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_TCPROW_OWNER_PID));
                    tTable[i] = tcpRow;
                    rowPtr += Marshal.SizeOf(tcpRow);
                }
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(buffTable);
            }
            return tTable;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    struct MIB_TCPROW_OWNER_PID
    {
        public uint state;
        public byte localAddr1, localAddr2, localAddr3, localAddr4;
        public byte localPort1, localPort2, localPort3, localPort4;
        public byte remoteAddr1, remoteAddr2, remoteAddr3, remoteAddr4;
        public byte remotePort1, remotePort2, remotePort3, remotePort4;
        public int Pid;

        public ushort LocalPort => BitConverter.ToUInt16(new byte[2] {localPort2, localPort1}, 0);

        public string LocalIP => String.Format("{0}.{1}.{2}.{3}", localAddr1, localAddr2, localAddr3, localAddr4);
        public ushort RemotePort => BitConverter.ToUInt16(new byte[2] {remotePort2, remotePort1}, 0);
        public string RemoteIP => String.Format("{0}.{1}.{2}.{3}", remoteAddr1, remoteAddr2, remoteAddr3, remoteAddr4);


    }

    [StructLayout(LayoutKind.Sequential)]
    struct MIB_TCPTABLE_OWNER_PID
    {
        public uint dwNumEntries;
        MIB_TCPROW_OWNER_PID table;
    }

    enum TCP_TABLE_CLASS : int
    {
        TCP_TABLE_BASIC_LISTENER,
        TCP_TABLE_BASIC_CONNECTIONS,
        TCP_TABLE_BASIC_ALL,
        TCP_TABLE_OWNER_PID_LISTENER,
        TCP_TABLE_OWNER_PID_CONNECTIONS,
        TCP_TABLE_OWNER_PID_ALL,
        TCP_TABLE_OWNER_MODULE_LISTENER,
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,
        TCP_TABLE_OWNER_MODULE_ALL
    }
}
