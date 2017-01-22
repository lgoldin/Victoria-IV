using System.Collections.Generic;

namespace Victoria.Shared
{
    public class PreParsedNodeCondition : PreParsedNode
    {
        public override void posprocesar(Dictionary<string, PreParsedNode> nodosPreProcesados, List<Node> nodos)
        {
           ((NodeCondition)this.node).ChilNodeTrue = getRefNode(nodosPreProcesados, this.next[0]).node;
           ((NodeCondition)this.node).ChildNodeFalse = getRefNode(nodosPreProcesados, this.next[1]).node;
          nodos.Add(this.node);
        }

    }

}
