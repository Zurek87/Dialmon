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
    class SpeedChart
    {
        private IRunForm _form;
        private PictureBox _box;
        Adapters _aEngine;
        // ConcurrentQueue<>

        public SpeedChart(IRunForm form, Adapters aEngine, PictureBox box)
        {
            _form = form;
            _box = box;
            _aEngine = aEngine;
        }

        private void OnStatsUpdate()
        {
            var a =  _aEngine.AdapterList;
        }

        private void CalculateStats() // Run in aEngine thread!
        {

        } 

        private void Draw() // Run in form main thread!
        {

        }
  
    }

    struct StatItem
    {
        private Queue<StatRow> _rows;
        private StatRow _lastRow;
        public StatRow[] Rows
        {
            get
            {
                if (_rows == null) _rows = new Queue<StatRow>();
                return _rows.ToArray();
            }
        }
        public void AddNewReading(IPv4InterfaceStatistics[] ipStats)
        {
            if (_rows == null) _rows = new Queue<StatRow>();
            long received = 0;
            long sent = 0;
            foreach(var stat in ipStats)
            {
                received += stat.BytesReceived;
                sent += stat.BytesSent;
            }
            var row = new StatRow()
            {
                TotalBytesReceived = received,
                TotalBytesSent = sent
            };

            if (_lastRow.TotalBytesReceived > 0)
            {
                long dRcv = (row.TotalBytesReceived - _lastRow.TotalBytesReceived);
                row.BytesReceived = dRcv > 0 ? dRcv : 0;

                long dSnd = (row.TotalBytesSent - _lastRow.TotalBytesSent);
                row.BytesSent = dSnd > 0 ? dSnd : 0;
            }
            _rows.Enqueue(row);
            _lastRow = row;
        }
    }
    struct StatRow
    {
        public long BytesReceived;
        public long BytesSent;
        public long TotalBytesReceived;
        public long TotalBytesSent;
    }
}
