using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victoria.Shared.Parser
{
    class ParsingException : Exception
    {
        public ParsingException(string p, Exception e) : base(p, e) { }
    }
}
