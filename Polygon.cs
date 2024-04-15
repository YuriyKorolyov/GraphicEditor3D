using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint3
{
    public class Polygon
    {
        public List<Spot> points;
        public Brush br;
        public Polygon(List<Spot> points, Brush br)
        {
            this.points = points;
            this.br = br;
        }
        public List<Point> getArrP()
        {
            List<Point> arr = new List<Point>();
            foreach (Spot sp in points)
            {
                arr.Add(sp.ToP);
            }
            return arr;
        }
    }
}
