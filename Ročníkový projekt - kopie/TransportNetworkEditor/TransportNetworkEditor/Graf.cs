using System.Collections.Generic;
using System.Drawing;

namespace TransportNetworkEditor
{
    public class Graf
    {
        public List<Uzel> Uzly { get; set; } = new List<Uzel>();
        public List<Hrana> Hrany { get; set; } = new List<Hrana>();
        private int dalsiIdUzlu = 1;

        public Uzel PridejUzel(string nazev, PointF pozice)
        {
            var uzel = new Uzel(dalsiIdUzlu++, nazev, pozice);
            Uzly.Add(uzel);
            return uzel;
        }

        public void SmazUzel(Uzel uzel)
        {
            // Nejdřív smaže všechny hrany připojené k tomuto uzlu, pak samotný uzel
            Hrany.RemoveAll(h => h.Od == uzel || h.Do == uzel);
            Uzly.Remove(uzel);
        }

        public void PridejHranu(Uzel od, Uzel kam, double vzdalenost, bool jeObousmerna = true)
        {
            Hrany.Add(new Hrana(od, kam, vzdalenost, jeObousmerna));
        }
    }
}