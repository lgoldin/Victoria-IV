using System.Collections.Generic;
using System.Linq;

namespace Victoria.Shared
{

    public class PreParsedNode
    {
        public string name;
        public Node node;
        public IList<string> next;


        public virtual void posprocesar(Dictionary<string, PreParsedNode> nodosPreProcesados, List<Node> nodos)
        {
            this.node.NextNode = this.next != null && this.next.Any() ? getRefNode(nodosPreProcesados, this.next[0]).node : null;
            nodos.Add(this.node);
        }

        public PreParsedNode getRefNode(Dictionary<string, PreParsedNode> nodos, string reference)
        {
            if (!nodos.ContainsKey(reference))
            {
                throw new InvalidNodeReferenceException(this.name, this.next);
            }
            return nodos[reference];
        }
    }

}