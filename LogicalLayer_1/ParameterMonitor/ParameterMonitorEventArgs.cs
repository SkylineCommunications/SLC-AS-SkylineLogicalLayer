namespace LogicalLayer_1.ParameterMonitor
{
    using System;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.CICD.Parsers.Protocol.Xml;
    using Skyline.DataMiner.Net.Messages;

    public class ParameterMonitorEventArgs : EventArgs
    {
        public string ParameterMonitorName { get; set; }

        public Element Element { get; set; }

        public ParameterInfo Parameter { get; set; }
    }
}
