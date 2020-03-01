using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Dialmon.Dialmon
{
    public class Adapters:DialmonThread
    {
        private Dictionary<String, AdapterInterface> _adapters = new Dictionary<string, AdapterInterface>();
        public event OnUpdateDelegate OnUpdate;

        public Dictionary<String, AdapterInterface> AdapterList { get { return _adapters; } }



        protected override void UpdateThread()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (_enabled)
            {
                UpdateAdapters();
                OnUpdate?.Invoke();
                                
                if(_enabled && _refreshTime >= stopWatch.ElapsedMilliseconds) {
                    long sleep = _refreshTime - stopWatch.ElapsedMilliseconds - 1;
                    if(sleep > 0) Thread.Sleep((int) sleep);
                }
                stopWatch.Restart();
            }
        }

        private void UpdateAdapters()
        {
            NetworkInterface[] adaptersArr = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in adaptersArr)
            {
                String aId = adapter.Id;
                if (_adapters.ContainsKey(aId))
                {
                    var netInter = _adapters[aId];
                    netInter.NetInterface = adapter;
                    netInter.ActualV4Statistics = adapter.GetIPv4Statistics();
                }
                else
                {
                    var netInter = new AdapterInterface
                    {
                        NetInterface = adapter,
                        ActualV4Statistics = adapter.GetIPv4Statistics()
                    };
                    _adapters.Add(aId, netInter);
                }
            }
        }
    }
}
