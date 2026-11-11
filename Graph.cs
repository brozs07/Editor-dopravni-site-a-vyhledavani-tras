using System.Collections.Generic;
using System.Drawing;

namespace TransportNetworkEditor
{
    public class Graph
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();
        private int nextNodeId = 1;

        public Node AddNode(string name, PointF position)
        {
            var node = new Node(nextNodeId++, name, position);
            Nodes.Add(node);
            return node;
        }

        public void RemoveNode(Node node)
        {
            Edges.RemoveAll(e => e.From == node || e.To == node);
            Nodes.Remove(node);
        }

        public void AddEdge(Node from, Node to, double distance, bool isBiDirectional = true)
        {
            Edges.Add(new Edge(from, to, distance, isBiDirectional));
        }
    }
}