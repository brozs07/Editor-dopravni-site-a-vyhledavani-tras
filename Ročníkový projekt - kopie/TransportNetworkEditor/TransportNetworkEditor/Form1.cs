using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TransportNetworkEditor
{
    public partial class Form1 : Form
    {
        private Graf graf = new Graf();

        // Nastavení pohledu (Zoom a Pan)
        private float priblizeni = 1.0f;
        private PointF posunPlatna = new PointF(0, 0);
        private bool posouvamPlatno = false;
        private Point posledniPoziceMysi;

        // Responzivní měřítko okna
        private Size referencniVelikost = Size.Empty;
        private float meritkoOkna = 1.0f;

        // Manipulace s uzly
        private Uzel vybranyUzel = null;
        private Uzel tazenyUzel = null;

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.KeyPreview = true;
            this.Text = "Editor dopravní sítě";
            this.WindowState = FormWindowState.Maximized;

            this.Load += Form1_Nacteni;
            this.Resize += Form1_ZmenaVelikosti;
            this.MouseWheel += Form1_KoleckoMysi;
            this.MouseDown += Form1_StiskMysi;
            this.MouseMove += Form1_PohybMysi;
            this.MouseUp += Form1_PusteniMysi;
            this.KeyDown += Form1_StiskKlavesy;
            this.Paint += Form1_Vykresleni;
        }

        private void Form1_Nacteni(object sender, EventArgs e)
        {
            referencniVelikost = this.ClientSize;
        }

        // Přepočet měřítka vůči původní velikosti okna
        private void Form1_ZmenaVelikosti(object sender, EventArgs e)
        {
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0) return;

            if (referencniVelikost.IsEmpty)
            {
                referencniVelikost = this.ClientSize;
                return;
            }

            float pomerX = (float)this.ClientSize.Width / referencniVelikost.Width;
            float pomerY = (float)this.ClientSize.Height / referencniVelikost.Height;
            meritkoOkna = Math.Min(pomerX, pomerY);

            Invalidate();
        }

        // Převod souřadnic z monitoru do souřadnic mapy (grafu)
        private PointF ObrazovkaNaSvet(Point bodNaObrazovce)
        {
            float celkovyZoom = priblizeni * meritkoOkna;
            return new PointF(
                (bodNaObrazovce.X - posunPlatna.X * meritkoOkna) / celkovyZoom,
                (bodNaObrazovce.Y - posunPlatna.Y * meritkoOkna) / celkovyZoom
            );
        }

        private void Form1_KoleckoMysi(object sender, MouseEventArgs e)
        {
            float faktorPriblizeni = e.Delta > 0 ? 1.15f : 0.85f;
            float novePriblizeni = priblizeni * faktorPriblizeni;

            if (novePriblizeni < 0.2f || novePriblizeni > 5.0f) return;

            PointF bodPredZoomem = ObrazovkaNaSvet(e.Location);
            priblizeni = novePriblizeni;

            posunPlatna.X = (e.X / meritkoOkna) - bodPredZoomem.X * priblizeni;
            posunPlatna.Y = (e.Y / meritkoOkna) - bodPredZoomem.Y * priblizeni;

            Invalidate();
        }

        private void Form1_StiskMysi(object sender, MouseEventArgs e)
        {
            PointF bodVPlose = ObrazovkaNaSvet(e.Location);

            // Prostřední tlačítko = posun plátna
            if (e.Button == MouseButtons.Middle)
            {
                posouvamPlatno = true;
                posledniPoziceMysi = e.Location;
                return;
            }

            // Pravé tlačítko = vytvoření nového uzlu
            if (e.Button == MouseButtons.Right)
            {
                string nazevUzlu = $"Zastávka {graf.Uzly.Count + 1}";
                graf.PridejUzel(nazevUzlu, bodVPlose);
                Invalidate();
                return;
            }

            // Levé tlačítko = výběr a uchopení uzlu
            if (e.Button == MouseButtons.Left)
            {
                Uzel kliknutyUzel = graf.Uzly.FindLast(u => u.ObsahujeBod(bodVPlose));

                foreach (var u in graf.Uzly)
                {
                    u.JeVybrany = false;
                }

                if (kliknutyUzel != null)
                {
                    vybranyUzel = kliknutyUzel;
                    vybranyUzel.JeVybrany = true;
                    tazenyUzel = kliknutyUzel;
                }
                else
                {
                    vybranyUzel = null;
                }
                Invalidate();
            }
        }

        private void Form1_PohybMysi(object sender, MouseEventArgs e)
        {
            if (posouvamPlatno)
            {
                posunPlatna.X += (e.X - posledniPoziceMysi.X) / meritkoOkna;
                posunPlatna.Y += (e.Y - posledniPoziceMysi.Y) / meritkoOkna;
                posledniPoziceMysi = e.Location;
                Invalidate();
                return;
            }

            if (tazenyUzel != null && e.Button == MouseButtons.Left)
            {
                tazenyUzel.Pozice = ObrazovkaNaSvet(e.Location);
                Invalidate();
            }
        }

        private void Form1_PusteniMysi(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) posouvamPlatno = false;
            if (e.Button == MouseButtons.Left) tazenyUzel = null;
        }

        private void Form1_StiskKlavesy(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && vybranyUzel != null)
            {
                graf.SmazUzel(vybranyUzel);
                vybranyUzel = null;
                Invalidate();
            }
        }

        private void Form1_Vykresleni(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float celkovyZoom = priblizeni * meritkoOkna;
            g.TranslateTransform(posunPlatna.X * meritkoOkna, posunPlatna.Y * meritkoOkna);
            g.ScaleTransform(celkovyZoom, celkovyZoom);

            // Vykreslení hran
            using (Pen peroHrany = new Pen(Color.DimGray, 2f))
            {
                foreach (var hrana in graf.Hrany)
                {
                    g.DrawLine(peroHrany, hrana.Od.Pozice, hrana.Do.Pozice);
                }
            }

            // Vykreslení uzlů
            foreach (var uzel in graf.Uzly)
            {
                Brush stetecVyplne = uzel.JeVybrany ? Brushes.Orange : Brushes.SteelBlue;
                Pen peroOkraje = uzel.JeVybrany ? Pens.Red : Pens.Navy;

                float r = uzel.Polomer;
                RectangleF obdelnik = new RectangleF(uzel.Pozice.X - r, uzel.Pozice.Y - r, r * 2, r * 2);

                g.FillEllipse(stetecVyplne, obdelnik);
                g.DrawEllipse(peroOkraje, obdelnik);

                using (Font pismo = new Font("Segoe UI", 9f, FontStyle.Bold))
                {
                    g.DrawString(uzel.Nazev, pismo, Brushes.Black, uzel.Pozice.X + r + 4, uzel.Pozice.Y - r / 2);
                }
            }
        }
    }
}