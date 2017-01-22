using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victoria.Shared
{
    public class VariableArray : Variable
    {
        #region Fields

        private List<Variable> variables;

        #endregion

        #region Properties
        public List<Variable> Variables
        {
            get { return variables; }
            set { variables = value; }
        }
        #endregion


    }
}
