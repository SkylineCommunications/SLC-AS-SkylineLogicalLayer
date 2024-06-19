using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalLayer_1.Condition
{
    public class ConditionEventArgs : EventArgs
    {
        public string ConditionName { get; set; }

        public string Condition { get; set; }

        public bool Visualize { get; set; }

        public bool AutomaticCorrection { get; set; }

        public string CorrectiveActionScript { get; set; }
    }
}
