using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalLayer_1.StaticVariable
{
    public class StaticVariableEventArgs : EventArgs
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
