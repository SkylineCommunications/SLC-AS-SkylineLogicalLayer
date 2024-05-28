namespace LogicalLayer_1.Condition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ConditionModel
    {
        public readonly string Command = "ConditionModel";

        public string ConditionName { get; set; }

        public string Condition { get; set; }

        public bool Visualize { get; set; }
    }
}
