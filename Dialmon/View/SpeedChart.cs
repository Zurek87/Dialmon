using Dialmon.Dialmon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialmon.View
{
    class SpeedChart
    {
        const int maxCount = 300;
        const int FrameBorder = 5;
        int offset = 0;

        private IRunForm _form;
        private PictureBox _box;
        private Size _boxSize;
        private Size _size;
        
        private Bitmap _chart;
        float rowHeight = 10;
        float columnWidth = 10;

        Adapters _aEngine;
        StatItem statistic = new StatItem();
        // ConcurrentQueue<>

        public SpeedChart(IRunForm form, Adapters aEngine, PictureBox box)
        {
            _form = form;
            _box = box;
            _aEngine = aEngine;
            _aEngine.OnUpdate += OnStatsUpdate;
            _box.SizeChanged += OnResize;
            initChartBitmap();
        }

        private void OnStatsUpdate()
        {
            var aList =  _aEngine.AdapterList;
            var s = aList.Select(ada => ada.Value.ActualV4Statistics).ToArray();
            statistic.AddNewReading(s);
            _form.RunInFormThread(Draw);
        } 

        private void OnResize(object sender, EventArgs e)
        {
            initChartBitmap();
            Draw();
        }

        private void initChartBitmap()
        {
            _size = new Size(_box.Size.Width - FrameBorder * 2, _box.Size.Height - FrameBorder * 2);
            _boxSize = _box.Size;
            rowHeight = (float)_size.Height / 10;
            columnWidth = (float)_size.Width / (maxCount / 10);
        }

        private void DrawChart(ref Bitmap bmp)
        {
            List<Point> inData = new List<Point>();
            List<Point> outData = new List<Point>();
            var rows = statistic.Rows.Reverse();
            var maxIn = rows.Max(x => x.BytesReceived);
            var maxOut = rows.Max(x => x.BytesSent);
            var max = (maxIn > maxOut) ? maxIn : maxOut;
            if (max < 1) max = 1;
            int i = 300;
            foreach (var row in rows)
            {
                float wsIn = (float)row.BytesReceived / max;
                float wsOut = (float)row.BytesSent / max;
                int x = (int)(i * ((float)_size.Width / maxCount));
                int yIn = _size.Height - (int)(wsIn * _size.Height);
                int yOut = _size.Height - (int)(wsOut * _size.Height);

                inData.Add(new Point(x, yIn));
                outData.Add(new Point(x, yOut));
                i--;
            }
            if (inData.Count < 2) return;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Pen pOut = new Pen(Color.FromArgb(224, 123, 123), 2);
                g.DrawCurve(pOut, outData.ToArray(), 0.1f);
                Pen pIn = new Pen(Color.FromArgb(125, 0, 255, 0), 2);
                g.DrawCurve(pIn, inData.ToArray(), 0.1f);
            }

        }
        private void DrawChartBackLines(ref Bitmap bmp)
        {
            var numCols = (maxCount / 10);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Pen p = new Pen(Color.FromArgb(224, 224, 224));

                for (int y = 1; y < 10; ++y)
                {
                    g.DrawLine(p, FrameBorder, y * rowHeight + FrameBorder, _size.Width + FrameBorder, y * rowHeight + FrameBorder);
                }/**/

                for (int x = 1; x <= numCols + 1; ++x)
                {
                    if (x <= numCols || offset == 10)
                    {
                        var xx = ((float)x - (float)offset / 10) * columnWidth + FrameBorder;
                        g.DrawLine(p, xx, FrameBorder, xx, _size.Height + FrameBorder );
                    }

                }
            }
            offset++;
            if (offset > 10) offset = 1;
        }

        private void Draw() // Run in form main thread!
        {
            if (_boxSize.Width == 0) return;
            var bmp = new Bitmap(_boxSize.Width, _boxSize.Height);
            DrawChartBackLines(ref bmp);
            DrawChart(ref bmp);
            _chart?.Dispose();
            _chart = bmp;
            _box.Image = _chart;
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
        public void AddNewReading(IPInterfaceStatistics[] ipStats)
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

            if (_rows.Count > 300) _rows.Dequeue();
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
