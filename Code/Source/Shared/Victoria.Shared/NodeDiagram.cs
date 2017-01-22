using System.Collections.Generic;

namespace Victoria.Shared
{
    public class NodeDiagram : Node
    {
        #region Fields

        #endregion

        #region Properties

        public string DiagramName
        {
            get;
            set;
        }

        public Diagram Diagram
        {
            get;
            set;
        }

        public bool IsInitializer { get; set; }
        #endregion

        #region Constructor

        #endregion

        #region Methods
        public override Node Execute(IList<Variable> variables)
        {
            if (!IsInitializer) Diagram.Execute(variables);
            return this.NextNode.Execute(variables);
        }
        #endregion
    }
}
