using System.Collections.Generic;
using System.Xml.Linq;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Victoria.Shared
{
    public class PreParsedDiagram
	{
		public string name;
		public Diagram diagram;
		public Dictionary<string,PreParsedNode> nodos;
	}
	
}
