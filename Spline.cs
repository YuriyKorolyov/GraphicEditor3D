using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint3
{
    [Serializable]
    public class Spline
    {
        public bool clear = false;
        private Pen splinePen = new Pen(Color.Red, 2);
        public Pen pen = new Pen(Color.Black, 1);

        [JsonInclude]
        public List<Spot> points = new List<Spot>();
        [JsonInclude]
        public List<int> colors = new List<int>();

        public bool isMousePressed = false;
        public Point lastMousePosition;
        public int pIndex;
        private float CalculateSplineCoordinate(float p0, float p1, float p2, float p3, float t)
        {
            float a3 = (-p0 + 3 * (p1 - p2) + p3) / 6;
            float a2 = (p0 - 2 * p1 + p2) / 2;
            float a1 = (p2 - p0) / 2;
            float a0 = (p0 + 4 * p1 + p2) / 6;

            return ((a3 * t + a2) * t + a1) * t + a0;
        }
        public void Paint(Graphics g, MatrixLine matrixLine)
        {
            if (!clear)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Spot sp = matrixLine.Rotate(points[i]);
                    if (i == pIndex)
                        g.FillEllipse(Brushes.Red, sp.ToP.X - 3, sp.ToP.Y - 3, 6, 6);
                    else
                        g.FillEllipse(Brushes.Black, sp.ToP.X - 3, sp.ToP.Y - 3, 6, 6);
                }
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Spot sp = matrixLine.Rotate(points[i]);
                    Spot ep = matrixLine.Rotate(points[i + 1]);
                    g.DrawLine(pen, sp.ToP, ep.ToP);
                }
            }
            for (int i = 0; i < points.Count - 3; i++)
            {
                do
                {
                    Random random = new Random();

                    int red = random.Next(0, 256);
                    int green = random.Next(0, 256);
                    int blue = random.Next(0, 256);

                    Color randomColor = Color.FromArgb(red, green, blue);
                    int color=randomColor.ToArgb();
                    if (!colors.Contains(color))
                        colors.Add(color);
                } while (colors.Count < i);
                splinePen.Color = Color.FromArgb(colors[i]);

                for (float t = 0; t <= 1; t += 0.01f)
                {
                    int x = (int)Math.Round(CalculateSplineCoordinate(points[i].X, points[i + 1].X, points[i + 2].X, points[i + 3].X, t));
                    int y = (int)Math.Round(CalculateSplineCoordinate(points[i].Y, points[i + 1].Y, points[i + 2].Y, points[i + 3].Y, t));
                    double z = CalculateSplineCoordinate((float)points[i].Z, (float)points[i + 1].Z, (float)points[i + 2].Z, (float)points[i + 3].Z, t);
                    Spot sp = new Spot(x, y, z, 1);
                    sp = matrixLine.Rotate(sp);
                    g.DrawRectangle(splinePen, sp.X, sp.Y, 1, 1);
                }
            }
        }
    }
}
