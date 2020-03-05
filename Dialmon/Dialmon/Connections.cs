﻿using System;
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
        private Dictionary<String, Connection> _connections = new Dictionary<string, Connection>();
        public event OnUpdateDelegate OnUpdate;
        public Dictionary<String, Connection> ConnectionList { get { return _connections; } }


        [DllImport("iphlpapi.dll", SetLastError = true)]
        static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TCP_TABLE_CLASS tblClass, int reserved);

        protected override void UpdateThread()
        {
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
            var conList = GetConnectionList();
            foreach (var key in _connections.Keys)
            {
                var cc = _connections[key];
                cc.Archived = true;
            }
            foreach(var con in conList)
            {
                _connections[con.Key] = con;
            }
        }

        private Connection[] GetConnectionList()
        {
            var rawConnections = GetAllTcpConnections();
            var connections = new List<Connection>();
            foreach (var raw in rawConnections)
            {
                connections.Add(ConnectionInfo(raw));
            }

            return connections.ToArray();
        }

        private Connection ConnectionInfo(MIB_TCPROW_OWNER_PID rawConection)
        {
            return new Connection()
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
        public uint state, localAddr;
        public byte localPort1, localPort2, localPort3, localPort4;
        public uint remoteAddr;
        public byte remotePort1, remotePort2, remotePort3, remotePort4;
        public int Pid;

        public ushort LocalPort => BitConverter.ToUInt16(new byte[2] {localPort2, localPort1}, 0);

        public string LocalIP => "not implemented";
        public ushort RemotePort => BitConverter.ToUInt16(new byte[2] {remotePort2, remotePort1}, 0);
        public string RemoteIP => "not implemented";


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