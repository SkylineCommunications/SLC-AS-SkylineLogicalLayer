namespace LogicalLayer_1.ElementAlarmMonitor
{
    using System;
    using static LogicalLayer_1.Script;

    public class ElementAlarmMonitorModel
    {
        public readonly string Command = "ElementAlarmMonitorModel";

        public string ElementAlarmMonitorName { get; set; }

        public string ElementName { get; set; }

        public int ElementDmaId { get; set; }

        public int ElementElementId { get; set; }

        public string ParameterDescription { get; set; }

        public int ParameterId { get; set; }

        public string Index { get; set; }

        public ElementParameter ElementParameter { get; set; }
    }
}
