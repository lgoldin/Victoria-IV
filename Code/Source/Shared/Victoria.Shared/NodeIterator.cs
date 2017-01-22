using System.Collections.Generic;
using System.Linq;

namespace Victoria.Shared
{
    public class NodeIterator : Node
    {
        #region Fields

        private int valorInicial;
        private int incremento;
        private int valorFinal;
        private Variable variableIteradora;
        private string variableName;
        private Node iterationNode;
        #endregion

        #region Properties
        public int ValorInicial
        {
            get { return valorInicial; }
            set { valorInicial = value; }
        }

        public int Incremento
        {
            get { return incremento; }
            set { incremento = value; }
        }

        public int ValorFinal
        {
            get { return valorFinal; }
            set { valorFinal = value; }
        }

        public string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }

        public Node IterationNode
        {
            get { return iterationNode; }
            set { iterationNode = value; }
        }

        #endregion

        #region Constructor

        #endregion

        #region Methods
        public override Node Execute(IList<Variable> variables)
        {
            if (variableIteradora == null) InicializarVariableIteradora(variables);
            if (this.variableIteradora.ActualValue < this.valorFinal)
            {
                this.variableIteradora.ActualValue += this.incremento;
                return this.IterationNode.Execute(variables);
            }
            else
            {
                this.variableIteradora.ActualValue = this.valorInicial;
                return base.Execute(variables);
            }

        }

        private void InicializarVariableIteradora(IList<Variable> variables)
        {
            var variable = variables.FirstOrDefault(v => v.Name == variableName);
            if (!string.IsNullOrEmpty(this.VariableName) && variable != null)
            {
                this.variableIteradora = variable;
            }
            else
            {
                this.variableIteradora = new Variable();

            }

            this.variableIteradora.InitialValue = this.ValorInicial;
            this.variableIteradora.ActualValue = this.ValorInicial;
        }
        #endregion
    }
}
