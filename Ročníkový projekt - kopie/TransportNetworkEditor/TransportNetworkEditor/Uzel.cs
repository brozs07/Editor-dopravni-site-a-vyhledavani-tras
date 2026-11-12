using System.Drawing;

namespace TransportNetworkEditor
{
    public class Uzel
    {
        public int Id { get; set; }
        public string Nazev { get; set; }
        public PointF Pozice { get; set; }
        public float Polomer { get; set; } = 16f;
        public bool JeVybrany { get; set; }

        public Uzel(int id, string nazev, PointF pozice)
        {
            Id = id;
            Nazev = nazev;
            Pozice = pozice;
        }

        // Zjistí jestli kliknutí myší zasáhlo tento uzel
        public bool ObsahujeBod(PointF bodVPlose)
        {
            float dx = bodVPlose.X - Pozice.X;
            float dy = bodVPlose.Y - Pozice.Y;
            return (dx * dx + dy * dy) <= (Polomer * Polomer);
        }
    }
}