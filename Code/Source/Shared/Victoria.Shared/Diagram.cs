using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Victoria.Shared
{
    public class Diagram
    {
        #region Fields

        public string name;
        public Node initializeNode;
        public Node mainNode;


        public ObservableCollection<Node> _nodes;

        #endregion

        #region Properties
        public ObservableCollection<Node> Nodes
        {
            get { return _nodes; }
            set
            {
                _nodes = value;
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }
        #endregion

        #region Constructor

        #endregion

        #region Methods

        public virtual Node Execute(IList<Variable> variables)
        {
            try
            {
                return this.Nodes.First().Execute(variables);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}
