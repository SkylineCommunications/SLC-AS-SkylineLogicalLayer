namespace LogicalLayer_1.ElementAlarmMonitor
{
    using System;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net.Messages;

    public class ElementCellAlarmMonitorEventArgs : EventArgs
    {
        public string ElementAlarmMonitorName { get; set; }

        public Element Element { get; set; }

        public string ElementParameter { get; set; }

        public ParameterInfo Table { get; set; }

        public ParameterInfo Column { get; set; }

        public string Index { get; set; }
    }
}
