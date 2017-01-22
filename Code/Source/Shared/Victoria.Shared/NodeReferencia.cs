using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victoria.Shared
{
    public class NodeReferencia : Node
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
            return this.NextNode;
        }
        #endregion
    }
}
