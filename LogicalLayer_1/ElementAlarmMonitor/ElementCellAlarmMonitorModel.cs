namespace LogicalLayer_1.ElementAlarmMonitor
{
    using System;
    using static LogicalLayer_1.Script;

    public class ElementCellAlarmMonitorModel
    {
        public readonly string Command = "ElementCellAlarmMonitorModel";

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