using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SalesProject
{
    partial class WatcherService : ServiceBase
    {
        private Watcher watcher = new Watcher();

        public WatcherService()
        {
            InitializeComponent();
            watcher.SetCatalog("D:\\Test");
        }

        protected override void OnStart(string[] args)
        {
            watcher.Watch();
            AddLog("start");
        }

        protected override void OnStop()
        {
            watcher.Dispose();
            AddLog("stop");
        }

        private void AddLog(string message)
        {
            try
            {
                if (!EventLog.SourceExists("WatcherService"))
                {
                    EventLog.CreateEventSource("WatcherService", "WatcherService");
                }
                eventLog1.Source = "WatcherService";
                eventLog1.WriteEntry(message);
            }
            catch { }
        }
    }
}
