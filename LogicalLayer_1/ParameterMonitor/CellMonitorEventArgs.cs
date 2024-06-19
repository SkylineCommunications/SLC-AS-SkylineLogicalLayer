namespace LogicalLayer_1.ParameterMonitor
{
    using System;
    using System.Collections.Specialized;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.CICD.Parsers.Protocol.Xml;
    using Skyline.DataMiner.Net.Messages;

    public class CellMonitorEventArgs : EventArgs
    {
        public string CellMonitorName { get; set; }

        public Element Element { get; set; }

        public ParameterInfo Table { get; set; }

        public ParameterInfo Column { get; set; }

        public string Index { get; set; }

        public string DisplayKey { get; set; }

        public bool IsDiscreet { get; set; }
    }
}
