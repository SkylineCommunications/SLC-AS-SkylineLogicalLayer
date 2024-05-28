namespace LogicalLayer_1.ElementAlarmMonitor
{
    using System;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net.Messages;

    public class ElementAlarmMonitorEventArgs : EventArgs
    {
        public string ElementAlarmMonitorName { get; set; }

        public Element Element { get; set; }

        public string ElementParameter { get; set; }

        public ParameterInfo Parameter { get; set; }

        public string Index { get; set; }
    }
}
