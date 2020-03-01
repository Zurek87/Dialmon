using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dialmon.Dialmon
{
    public abstract class DialmonThread
    {
        protected int _refreshTime = 1000;
        private Thread _thread;
        protected bool _enabled = false;

        public int RefreshTime
        {
            get { return _refreshTime; }
            set
            {
                if (value < 100 || value > 600000)
                    throw new ArgumentOutOfRangeException("RefreshTime", "RefreshTime must be between 100ms - 600000ms");

                _refreshTime = value;
            }
        }

        public void StopMonitor()
        {
            _enabled = false;
        }
        public void StartMonitor()
        {
            if (_thread == null)
            {
                _thread = new Thread(UpdateThread);

            }
            _enabled = true;
            _thread.Start();
        }

        protected abstract void UpdateThread();
    }
}
