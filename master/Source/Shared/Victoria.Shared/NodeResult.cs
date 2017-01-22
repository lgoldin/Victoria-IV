using System.Collections.Generic;

namespace Victoria.Shared
{
    public class NodeResult : Node
    {
        #region Fields
        #endregion

        #region Properties

        public IEnumerable<string> Variables
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        #endregion

        #region Methods
        public override Node Execute(IList<Variable> variables)
        {
            return null;
        }
        #endregion
    }
}
