using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Paint3
{
    [Serializable]
    public class Spot
    {
        [JsonInclude]
        public int X;
        [JsonInclude]
        public int Y;
        [JsonInclude]
        public double Z;
        [JsonInclude]
        public double W;
        public Spot(int x, int y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
        public Spot(Point p)
        {
            X = p.X;
            Y = p.Y;
            Z = 0;
            W = 1;
        }

        public Point ToP
        {
            get { return new Point(X, Y); }
        }
        public Spot()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.W = 1;
        }
    }
}
