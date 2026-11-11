using System.Drawing;

namespace TransportNetworkEditor
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PointF Position { get; set; }
        public float Radius { get; set; } = 16f;
        public bool IsSelected { get; set; }

        public Node(int id, string name, PointF position)
        {
            Id = id;
            Name = name;
            Position = position;
        }

        public bool Contains(PointF worldPoint)
        {
            float dx = worldPoint.X - Position.X;
            float dy = worldPoint.Y - Position.Y;
            return (dx * dx + dy * dy) <= (Radius * Radius);
        }
    }
}