using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultNamespace
{
    public class Node
    {
        public int Id { get; }
        public List<Node> AdjacentNodes { get; } = new List<Node>();

        public Node(int id)
        {
            Id = id;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Node {Id} - Adjacent Nodes: ");
            sb.Append(AdjacentNodes.Count > 0 ? string.Join(", ", AdjacentNodes.Select(node => node.Id)) : "None");
            return sb.ToString();
        }
    }
}