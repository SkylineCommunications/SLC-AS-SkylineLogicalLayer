namespace LogicalLayer_1.ParameterMonitor
{
    using System;
    using Skyline.DataMiner.Automation;

    public class CellMonitorModel
    {
        public readonly string Command = "CellMonitorModel";

        public string CellMonitorName { get; set; }

        public string ElementName { get; set; }

        public int ElementDmaId { get; set; }

        public int ElementElementId { get; set; }

        public int TableId { get; set; }

        public string ColumnDescription { get; set; }

        public int ColumnId { get; set; }

        public string Index { get; set; }
    }
}
