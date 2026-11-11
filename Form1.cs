using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TransportNetworkEditor
{
    public partial class Form1 : Form
    {
        private Graph graph = new Graph();

        private float zoom = 1.0f;
        private PointF panOffset = new PointF(0, 0);
        private bool isPanning = false;
        private Point lastMousePos;

        private Node selectedNode = null;
        private Node draggedNode = null;

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Text = "Editor dopravní sítě";
            this.WindowState = FormWindowState.Maximized;

            this.MouseWheel += Form1_MouseWheel;
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.KeyDown += Form1_KeyDown;
            this.Paint += Form1_Paint;
        }

        private PointF ScreenToWorld(Point screenPt)
        {
            return new PointF(
                (screenPt.X - panOffset.X) / zoom,
                (screenPt.Y - panOffset.Y) / zoom
            );
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float zoomFactor = e.Delta > 0 ? 1.15f : 0.85f;
            float newZoom = zoom * zoomFactor;

            if (newZoom < 0.2f || newZoom > 5.0f) return;

            panOffset.X = e.X - (e.X - panOffset.X) * zoomFactor;
            panOffset.Y = e.Y - (e.Y - panOffset.Y) * zoomFactor;
            zoom = newZoom;

            Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            PointF worldPt = ScreenToWorld(e.Location);

            if (e.Button == MouseButtons.Middle)
            {
                isPanning = true;
                lastMousePos = e.Location;
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                string nodeName = $"Zastávka {graph.Nodes.Count + 1}";
                graph.AddNode(nodeName, worldPt);
                Invalidate();
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                Node clickedNode = graph.Nodes.FindLast(n => n.Contains(worldPt));

                foreach (var n in graph.Nodes)
                {
                    n.IsSelected = false;
                }

                if (clickedNode != null)
                {
                    selectedNode = clickedNode;
                    selectedNode.IsSelected = true;
                    draggedNode = clickedNode;
                }
                else
                {
                    selectedNode = null;
                }
                Invalidate();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                panOffset.X += e.X - lastMousePos.X;
                panOffset.Y += e.Y - lastMousePos.Y;
                lastMousePos = e.Location;
                Invalidate();
                return;
            }

            if (draggedNode != null && e.Button == MouseButtons.Left)
            {
                draggedNode.Position = ScreenToWorld(e.Location);
                Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) isPanning = false;
            if (e.Button == MouseButtons.Left) draggedNode = null;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && selectedNode != null)
            {
                graph.RemoveNode(selectedNode);
                selectedNode = null;
                Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.TranslateTransform(panOffset.X, panOffset.Y);
            g.ScaleTransform(zoom, zoom);

            using (Pen edgePen = new Pen(Color.DimGray, 2f))
            {
                foreach (var edge in graph.Edges)
                {
                    g.DrawLine(edgePen, edge.From.Position, edge.To.Position);
                }
            }

            foreach (var node in graph.Nodes)
            {
                Brush fillBrush = node.IsSelected ? Brushes.Orange : Brushes.SteelBlue;
                Pen borderPen = node.IsSelected ? Pens.Red : Pens.Navy;

                float r = node.Radius;
                RectangleF rect = new RectangleF(node.Position.X - r, node.Position.Y - r, r * 2, r * 2);

                g.FillEllipse(fillBrush, rect);
                g.DrawEllipse(borderPen, rect);

                using (Font font = new Font("Segoe UI", 9f, FontStyle.Bold))
                {
                    g.DrawString(node.Name, font, Brushes.Black, node.Position.X + r + 4, node.Position.Y - r / 2);
                }
            }
        }
    }
}