using System.Collections.Generic;

namespace Victoria.Shared
{

	public class InvalidNodeReferenceException : System.Exception
	{
		public InvalidNodeReferenceException (string fromName, IEnumerable<string> toName) 
			: base(string.Format ("Error: el nodo: {0} referencia al nodo {1} que no se encuentra entre los nodos parseados",fromName,toName))
		{}
	}
}