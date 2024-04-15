using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Paint3
{
    [Serializable]
    public class MatrixLine
    {
        [JsonInclude]
        public bool mirrorV = false;
        [JsonInclude]
        public bool mirrorG = false;
        [JsonInclude]
        public bool mirrorGl = false;
        [JsonInclude]
        public double rotationX = 30.0;
        [JsonInclude]
        public double rotationY = 45.0;
        [JsonInclude]
        public double translationX = 900.0;
        [JsonInclude]
        public double translationY = 500.0;
        [JsonInclude]
        public double scale = 1.0;
        [JsonInclude]
        public double zc = 700.0;
        public static bool S = false;
        public static bool R = false;
        public static int Count
        { get => links.Count; }

        public static List<List<int>> links = new List<List<int>>()
        {
            new List<int> {0, 1, 0,1,1,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//1
            new List<int> {1, 0, 1,0,0,1, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//2
            new List<int> {0, 1, 0,1,0,0, 1, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//3
            new List<int> {1, 0, 1,0,0,0, 0, 1,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//4
            new List<int> {1, 0, 0,0,0,1, 0, 1,1,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//5
            new List<int> {0, 1, 0,0,1,0, 1, 0,0,1,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//6
            new List<int> {0, 0, 1,0,0,1, 0, 1,0,0,1, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//7
            new List<int> {0, 0, 0,1,1,0, 1, 0,0,0,0, 1, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//8
            new List<int> {0, 0, 0,0,1,0, 0, 0,0,1,0, 0, 1,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//9
            new List<int> {0, 0, 0,0,0,1, 0, 0,1,0,0, 0, 0,1,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//10
            new List<int> {0, 0, 0,0,0,0, 1, 0,0,0,0, 1, 0,1,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//11
            new List<int> {0, 0, 0,0,0,0, 0, 1,0,0,1, 0, 1,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//12
            new List<int> {0, 0, 0,0,0,0, 0, 0,1,0,0, 1, 0,1,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//13
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,1,1, 0, 1,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0},//14
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,1, 0, 0,0,1,0, 0, 0,0,0,0, 0, 0,0,0},//15
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,1,0, 0, 0,1,0,0, 0, 0,0,0,0, 0, 0,0,0},//16
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 1,1,0,0, 0, 0,0,0,0, 0, 0,0,0},//17
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 1, 0,0,1,0, 0, 0,0,0,0, 0, 0,0,0},//18
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,1,0, 0, 0,0,0,0, 0, 0,0,0},//19
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,1,0,0, 0, 0,0,0,0, 0, 0,0,0},//20
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 1, 0,1,0,0, 0, 0,0,0},//21
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,1, 0, 1,0,0,0, 0, 0,0,0},//22
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 1, 0,1,0,0, 0, 0,0,0},//23
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,1, 0, 1,0,0,0, 0, 0,0,0},//24
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,1, 0, 0,0,0},//25
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,1,0, 0, 0,0,0},//26
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 1,0,1},//27
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 1, 0,1,0},//28
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 1,0,1},//29
            new List<int> {0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 0, 0,0,0,0, 1, 0,1,0}//30
        };
        public static List<List<double>> coords = new List<List<double>>()
        {
            new List<double>(){ 0, 0,3,1 },
            new List<double>(){ 0,0,0,1},
            new List<double>(){ 3,0,0,1},
            new List<double>(){ 3,0,3,1},
            new List<double>(){ 0,3,3,1},
            new List<double>(){ 0,3,0,1},
            new List<double>(){ 3,3,0,1},
            new List<double>(){ 3,3,3,1},
            new List<double>(){ -0.5,3,3,1},
            new List<double>(){ -0.5,3,0,1 },
            new List<double>(){ 3.5, 3,0,1},
            new List<double>(){ 3.5,3,3,1},
            new List<double>(){ 1.5,4.5,3,1},
            new List<double>(){ 1.5,4.5,0,1},
            new List<double>(){ 1,1,3,1},
            new List<double>(){ 1,2.5,3,1},
            new List<double>(){ 2,2.5,3,1},
            new List<double>(){ 2,1,3,1},
            new List<double>(){ 1.5,2.5,3,1},
            new List<double>(){ 1.5,1,3,1},
            new List<double>(){ 3,0,2.5,1},
            new List<double>(){ 3,1.5,2.5,1},
            new List<double>(){ 3,1.5,1.5,1},
            new List<double>(){ 3,0,1.5,1},
            new List<double>(){ 3,1.5,2,1},
            new List<double>(){ 3,0,2,1},
            new List<double>(){ 3,2,1,1},
            new List<double>(){ 3,2,0.5,1},
            new List<double>(){ 3,1,0.5,1},
            new List<double>(){ 3,1,1,1}
        };
        
        public Spot Rotate(Spot cor)
        {
            double a = Math.PI * rotationX / 180.0;
            double b = -Math.PI * rotationY / 180.0;

            int cx = (int)Math.Round((cor.X - translationX) * scale);
            int cy = (int)Math.Round((cor.Y - translationY) * scale);
            double cz = cor.Z * scale;

            if (mirrorV)
                cx = -cx;
            if (mirrorG)
                cy = -cy;
            if (mirrorGl)
                cz = -cz;

            double ok = cx * Math.Sin(a) * Math.Cos(b) / zc - cy * Math.Sin(b) / zc - cz * Math.Cos(a) * Math.Cos(b) / zc + 1;
            cy = (int)Math.Round((cx * Math.Sin(a) * Math.Sin(b) + cy * Math.Cos(b) - cz * Math.Cos(a) * Math.Sin(b)) / ok);
            cx = (int)Math.Round((cx * Math.Cos(a) + cz * Math.Sin(a)) / ok);

            cx += (int)Math.Round(translationX);
            cy += (int)Math.Round(translationY);

            return new Spot(cx, cy, 0, 1);
        }  
    }
}
