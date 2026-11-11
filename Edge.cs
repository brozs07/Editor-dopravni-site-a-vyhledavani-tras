namespace TransportNetworkEditor
{
    public class Edge
    {
        public Node From { get; set; }
        public Node To { get; set; }
        public double Distance { get; set; }
        public bool IsBiDirectional { get; set; } = true;
        public bool IsClosed { get; set; } = false;

        public Edge(Node from, Node to, double distance, bool isBiDirectional = true)
        {
            From = from;
            To = to;
            Distance = distance;
            IsBiDirectional = isBiDirectional;
        }
    }
}