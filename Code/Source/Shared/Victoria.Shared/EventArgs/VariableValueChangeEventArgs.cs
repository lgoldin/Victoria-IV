using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victoria.Shared.EventArgs
{
    public class VariableValueChangeEventArgs : System.EventArgs
    {

        public VariableValueChangeEventArgs(Variable variable, double oldValue, double newValue)
        {
            this.Variable = variable;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        /// <summary>
        /// Variable with changes
        /// </summary>
        public Variable Variable { get; set; }

        /// <summary>
        /// Old Value
        /// </summary>
        public double OldValue { get; set; }

        /// <summary>
        /// New Value
        /// </summary>
        public double NewValue { get; set; }
    }
}
