namespace LogicalLayer_1.ViewAlarmMonitor
{
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

    public class ViewAlarmMonitorEventArgs
    {
        public string ViewAlarmMonitorName { get; set; }

        public IDmsView View { get; set; }

        public string ViewParameter { get; set; }
    }
}
