using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Victoria.Shared
{
    public class NodeCondition : Node
    {
        #region Fields

        string code;
        private Node childNodeFalse;
        private Node chilNodeTrue;
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
        public Node ChildNodeFalse
        {
            get { return childNodeFalse; }
            set
            {
                childNodeFalse = value;
            }
        }

        public Node ChilNodeTrue
        {
            get
            {
                return chilNodeTrue;
            }
            set
            {
                chilNodeTrue = value;
            }
        }
        #endregion

        #region Constructor

        #endregion

        #region Methods
        public override Node Execute(IList<Variable> variables)
        {
            var cultureInfo = new CultureInfo("en-US");
            var sentence = this.Code;
            foreach (var var in variables.OrderByDescending(x => x.Name.Length))
            {
                sentence = sentence.Replace(var.Name, var.ActualValue.ToString(cultureInfo));
            }

            foreach (VariableArray varArray in variables.Where(v => v is VariableArray).OrderByDescending(x => x.Name.Length))
            {
                foreach (Variable var in varArray.Variables)
                {
                    sentence = sentence.Replace(var.Name, var.ActualValue.ToString(cultureInfo));
                }
            }

            if (sentence.Contains("R"))
            {
                Random r = new Random(DateTime.Now.Millisecond);
                sentence = sentence.Replace("R", r.Next(0, 10).ToString(cultureInfo));
            }

            var result = ExpressionResolver.ResolveBoolen(sentence);
            if (result)
                return this.ChilNodeTrue.Execute(variables);
            else
                return this.childNodeFalse.Execute(variables);
        }
        #endregion

    }
}

