using System;
using System.Collections.Generic;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace Exporters
{
    /// <summary>
    /// Simple class that lists all of the existing SmartExporters and then on a timer
    /// will call ExportNew
    /// </summary>
    internal class SmartExportRunner
    {
        private List<SmartExporter> Exporters = new List<SmartExporter>();
        const int DEFAULT_PERIOD = 5_000; // every 5 seconds
        System.Timers.Timer PeriodicTimer;
        System.Threading.Lock Lock = new System.Threading.Lock();

        public SmartExportRunner()
        {
            PeriodicTimer = new System.Timers.Timer(DEFAULT_PERIOD);
            PeriodicTimer.Elapsed += PeriodicTimer_Elapsed;
        }

        private void PeriodicTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //lock (Lock)
            {
                foreach (var exporter in Exporters)
                {
                    exporter.ExportRowMostRecentData();
                }
            }
        }

        public void Add(SmartExporter exporter)
        {
            //lock (Lock)
            {
                Exporters.Add(exporter);
                if (!IsStarted)
                {
                    Start();
                }
            }
        }

        public void Start()
        {
            PeriodicTimer.Start();
        }

        public bool IsStarted { get { var retval = PeriodicTimer.Enabled; return retval; } }
    }
}
