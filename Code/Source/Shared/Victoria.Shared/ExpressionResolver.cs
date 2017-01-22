using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;

namespace Victoria.Shared
{
    public static class ExpressionResolver
    {
        public static void Resolve(Variable variable, string expression)
        {
            Jint.Engine engine = new Engine();
            variable.ActualValue = engine.Execute(expression).GetCompletionValue().AsNumber();
        }

        public static bool ResolveBoolen(string expression)
        {
            Jint.Engine engine = new Engine();
            return engine.Execute(expression).GetCompletionValue().AsBoolean();
        }
    }
}
