using System.Collections.Generic;

namespace Victoria.Shared
{
    public class Node
    {
        #region Fields

        private string name;
        private Node nextNode;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public Node NextNode
        {
            get { return nextNode; }
            set
            {
                nextNode = value;
            }
        }

        #endregion

        #region Constructor


        #endregion

        #region Methods
        public virtual Node Execute(IList<Variable> variables)
        {
            if (this.nextNode != null)
                return this.nextNode.Execute(variables);

            return null;
        }

        #endregion


    }
}
