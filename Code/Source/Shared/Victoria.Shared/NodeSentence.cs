using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Victoria.Shared
{
    public class NodeSentence : Node
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
            var cultureInfo = new CultureInfo("en-US");
            var indexEqual = this.Code.IndexOf("=");
            var variableStr = this.Code.Substring(0, indexEqual).Trim();
            var sentenceStr = this.code.Substring(indexEqual + 1).Trim();
            Variable variable = null;
            foreach (var var in variables.OrderByDescending(x => x.Name.Length))
            {
                sentenceStr = sentenceStr.Replace(var.Name, var.ActualValue.ToString(cultureInfo));
            }

            foreach (VariableArray varArray in variables.Where(v => v is VariableArray).OrderByDescending(x => x.Name.Length))
            {
                foreach (Variable var in varArray.Variables)
                {
                    sentenceStr = sentenceStr.Replace(var.Name, var.ActualValue.ToString(cultureInfo));
                }
            }

            if (sentenceStr.Contains("R"))
            {
                Random r = new Random(DateTime.Now.Millisecond);
                sentenceStr = sentenceStr.Replace("R", r.Next(0, 10).ToString(cultureInfo));
            }

            if (variableStr.Contains("(") && variableStr.Contains(")"))
            {
                //es vector
                var vectorIndex = Regex.Match(this.Code, @"(?<=\().+?(?=\))").Value;
                var variableIndex = variables.FirstOrDefault(v => v.Name == vectorIndex);
                if (variableIndex != null)
                {
                    variableStr = variableStr.Replace(variableIndex.Name, variableIndex.ActualValue.ToString(cultureInfo));
                }

                foreach (VariableArray varArray in variables.Where(v => v is VariableArray).OrderByDescending(x => x.Name.Length))
                {
                    variable = varArray.Variables.FirstOrDefault(x => x.Name == variableStr);
                    if (variable != null) break;
                }

            }
            else
            {
                variable = variables.First(x => x.Name == variableStr);

            }


            ExpressionResolver.Resolve(variable, sentenceStr);
            return base.Execute(variables);
        }
        #endregion
    }
}

