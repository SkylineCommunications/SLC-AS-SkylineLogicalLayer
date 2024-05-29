namespace LogicalLayer_1.ParameterMonitor
{
    using System;
    using Skyline.DataMiner.Automation;

    public class ParameterMonitorModel
    {
        public readonly string Command = "ParameterMonitorModel";

        public string ParameterMonitorName { get; set; }

        public string ElementName { get; set; }

        public int ElementDmaId { get; set; }

        public int ElementElementId { get; set; }

        public string ParameterDescription { get; set; }

        public int ParameterId { get; set; }

        public bool ParameterIsDiscreet { get; set; }
    }
}
