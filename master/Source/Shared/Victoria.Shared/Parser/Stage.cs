using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Victoria.Shared.Parser;

namespace Victoria.Shared
{
	public class Stage
	{
		public string Name {
			get;
			set;
		}

		public List<Variable> Variables {
			get;
			set;
		}
		public List<Chart> Charts{
			get;
			set;
		}
	}
    
}
