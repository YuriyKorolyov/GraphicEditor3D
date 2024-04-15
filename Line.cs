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
    public class Line
    {
        [JsonInclude]
        public Spot startPoint;
        [JsonInclude]
        public Spot endPoint;
        [JsonInclude]
        public int color;
        [JsonInclude]
        public float width;

        public Point lastSP;
        public Point lastEP;
        [JsonInclude]
        public bool IsSelected { get; set; }

        public int coefA
        {
            get
            {
                return endPoint.Y - startPoint.Y;
            }
        }
        public int coefB
        {
            get
            {
                return startPoint.X - endPoint.X;
            }
        }
        public int coefC
        {
            get => endPoint.X * startPoint.Y - startPoint.X * endPoint.Y;
        }

        public Line()
        {
            startPoint = new Spot();
            endPoint = new Spot();
            color = Color.Black.ToArgb();
            width = 2;
        }
        public Line(Spot startPoint, Spot endPoint, Pen pen)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.color = pen.Color.ToArgb();
            this.width = pen.Width;
            this.IsSelected = false;
        }

        public Line(Spot startPoint, Spot endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            color = Color.Black.ToArgb();
            width = 2;
        }

        public bool Contains(Point cursor)
        {
            int a = endPoint.Y - startPoint.Y;
            int b = startPoint.X - endPoint.X;
            int c = endPoint.X * startPoint.Y - startPoint.X * endPoint.Y;
            int distance = (int)Math.Round(Math.Abs(a * cursor.X + b * cursor.Y + c) / Math.Sqrt(a * a + b * b));

            return distance < 10;
        }
    }
}
