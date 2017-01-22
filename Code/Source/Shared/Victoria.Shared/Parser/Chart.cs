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

	public class Chart
	{
		public string Name {
			get;
			set;
		}
		public List<String> DependentVariables {
			get;
			set;
		}
	}
    
}
