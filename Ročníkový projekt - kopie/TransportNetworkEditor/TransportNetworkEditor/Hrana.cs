namespace TransportNetworkEditor
{
    public class Hrana
    {
        public Uzel Od { get; set; }
        public Uzel Do { get; set; }
        public double Vzdalenost { get; set; }
        public bool JeObousmerna { get; set; } = true;
        public bool JeUzavrena { get; set; } = false;

        public Hrana(Uzel od, Uzel kam, double vzdalenost, bool jeObousmerna = true)
        {
            Od = od;
            Do = kam;
            Vzdalenost = vzdalenost;
            JeObousmerna = jeObousmerna;
        }
    }
}