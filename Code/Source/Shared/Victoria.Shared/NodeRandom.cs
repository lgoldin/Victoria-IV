using System;
using System.Collections.Generic;
using System.Linq;

namespace Victoria.Shared
{
    public class NodeRandom : Node
    {
        #region Fields
        private string code;
        #endregion

        #region Properties
        public string Code
        {
            get { return code; }
            set
            {
                code = value;
            }
        }
        #endregion

        #region Constructor

        #endregion

        #region Methods
        public override Node Execute(IList<Variable> variables)
        {
            return base.Execute(variables);
        }
        #endregion
    }
}
