using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalLayer_1.ViewAlarmMonitor
{
    public class ViewAlarmMonitorModel
    {
        public readonly string Command = "ViewAlarmMonitorModel";

        public int ViewId { get; set; }

        public string ViewName { get; set; }

        public string ViewAlarmMonitorName { get; set; }

        public string Parameter { get; set; }
    }
}
