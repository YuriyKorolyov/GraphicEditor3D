using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using Rectangle = System.Drawing.Rectangle;

namespace Paint3
{
    [Serializable]
    public class ArrayLine
    {
        [JsonInclude]
        public List<Line> lines = new List<Line>();
        public List<Line> linesDraw = new List<Line>();
        [JsonInclude]
        public string Name
        {
            get; set;
        }
        public static bool MoveGroup = false;
        public List<int> indexCircleList = new List<int>();
        public int indexCircle = -1;
        public int indexColorLine = -1;
        public int indexColorMove = -1;
        public static int circleRadius = 3;
        public static bool isMove = false;
        public int dX;
        public int dY;
        public int Count
        {
            get { return lines.Count; }
        }
        public Line this[int index]
        {
            get { return lines[index]; }
            set { lines[index] = value; }
        }
        public void Add(Line line)
        {
            lines.Add(line);
        }
        public void RemoveAt(int index)
        {
            lines.RemoveAt(index);
        }
        public int Contains(Point cursor)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (linesDraw[i].Contains(cursor))
                    return i;
            }
            return -1;
        }
        public void Draw(MatrixLine matrixLine)
        {
            linesDraw.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                Spot sp = matrixLine.Rotate(lines[i].startPoint);
                Spot ep = matrixLine.Rotate(lines[i].endPoint);
                linesDraw.Add(new Line(sp, ep));
            }
        }
        public void Paint(Graphics g)
        {
            for (int i = 0; i < linesDraw.Count; i++)
            {
                Pen pen = new Pen(Color.FromArgb(lines[i].color), lines[i].width);//(indexCircleList.Contains(i * 2) || indexCircleList.Contains(i * 2 + 1)) && isMove
                Pen pNew = new Pen(Color.Red, 5);

                if (i == indexColorLine || i == indexColorMove || lines[i].IsSelected)
                {
                    g.DrawLine(pNew, linesDraw[i].startPoint.ToP, linesDraw[i].endPoint.ToP);
                    Circle.DrawCircle(g, linesDraw[i].startPoint.ToP, pNew);
                    Circle.DrawCircle(g, linesDraw[i].endPoint.ToP, pNew);
                }
                else
                {
                    g.DrawLine(pen, linesDraw[i].startPoint.ToP, linesDraw[i].endPoint.ToP);

                    if (indexCircleList.Contains(i * 2))
                    {
                        Circle.DrawCircle(g, linesDraw[i].startPoint.ToP, pNew);
                    }
                    else
                        Circle.DrawCircle(g, linesDraw[i].startPoint.ToP, pen);

                    if (indexCircleList.Contains(i * 2 + 1))
                    {
                        Circle.DrawCircle(g, linesDraw[i].endPoint.ToP, pNew);
                    }
                    else
                        Circle.DrawCircle(g, linesDraw[i].endPoint.ToP, pen);
                }
            }
        }
        public Point MidPoint()
        {
            int mX = 0;
            int mY = 0;
            int count = lines.Count;
            foreach (var line in lines)
            {
                mX += line.startPoint.X + line.endPoint.X;
                mY += line.startPoint.Y + line.endPoint.Y;
            }
            return new Point(mX / count / 2, mY / count / 2);
        }
        public void Rotate(double degrees)
        {
            Point p = MidPoint();
            double angle = Math.PI * degrees / 180.0;

            foreach (var line in lines)
            {

                Point sP = new Point(line.startPoint.X - p.X, line.startPoint.Y - p.Y);
                Point eP = new Point(line.endPoint.X - p.X, line.endPoint.Y - p.Y);

                line.startPoint = new Spot((int)Math.Round(sP.X * Math.Cos(angle) - Math.Sin(angle) * sP.Y), (int)Math.Round(sP.X * Math.Sin(angle) + Math.Cos(angle) * sP.Y), line.startPoint.Z, 1);
                line.endPoint = new Spot((int)Math.Round(eP.X * Math.Cos(angle) - Math.Sin(angle) * eP.Y), (int)Math.Round(eP.X * Math.Sin(angle) + Math.Cos(angle) * eP.Y), line.endPoint.Z, 1);

                line.startPoint.X += p.X;
                line.startPoint.Y += p.Y;
                line.endPoint.X += p.X;
                line.endPoint.Y += p.Y;
            }
        }
        public void Scale(double scale, MatrixLine matrixLine)
        {
            Point p = lines[0].startPoint.ToP;

            foreach (var line in lines)
            {
                line.startPoint = new Spot((int)Math.Round((line.startPoint.X - matrixLine.translationX) * scale + matrixLine.translationX), (int)Math.Round((line.startPoint.Y - matrixLine.translationY) * scale + matrixLine.translationY), line.startPoint.Z * scale, 1);
                line.endPoint = new Spot((int)Math.Round((line.endPoint.X - matrixLine.translationX) * scale + matrixLine.translationX), (int)Math.Round((line.endPoint.Y - matrixLine.translationY) * scale + matrixLine.translationY), line.endPoint.Z * scale, 1);
            }
        }
        public void MirrorV()
        {
            Point p = MidPoint();

            foreach (var line in lines)
            {

                Point sP = new Point(line.startPoint.X - p.X, line.startPoint.Y - p.Y);
                Point eP = new Point(line.endPoint.X - p.X, line.endPoint.Y - p.Y);

                line.startPoint = new Spot(line.startPoint.X, -sP.Y + p.Y, line.startPoint.Z, 1);
                line.endPoint = new Spot(line.endPoint.X, -eP.Y + p.Y, line.endPoint.Z, 1);
            }
        }
        public void MirrorG()
        {
            Point p = MidPoint();

            foreach (var line in lines)
            {
                Point sP = new Point(line.startPoint.X - p.X, line.startPoint.Y - p.Y);
                Point eP = new Point(line.endPoint.X - p.X, line.endPoint.Y - p.Y);

                line.startPoint = new Spot(-sP.X + p.X, line.startPoint.Y, line.startPoint.Z, 1);
                line.endPoint = new Spot(-eP.X + p.X, line.endPoint.Y, line.endPoint.Z, 1);
            }
        }
        public void DrawHouse(MatrixLine matrixLine)
        {
            Pen pen = new Pen(Color.Black, 2);

            double scale = 100.0;
            double translationX = matrixLine.translationX;
            double translationY = matrixLine.translationY;

            for (int i = 0; i < MatrixLine.links.Count; i++)
            {              
                int X = (int)Math.Round((MatrixLine.coords[i][0] - 1.5) * scale + translationX);
                int Y = (int)Math.Round(-(MatrixLine.coords[i][1] - 2.25) * scale + translationY);
                double Z = (int)Math.Round((MatrixLine.coords[i][2] - 1.5) * scale);

                Spot sp = new Spot(X, Y, Z, 1);

                for (int j = i; j < MatrixLine.links[i].Count; j++)
                {
                    if (MatrixLine.links[i][j] != 0)
                    {
                        X = (int)Math.Round((MatrixLine.coords[j][0] - 1.5) * scale + translationX);
                        Y = (int)Math.Round(-(MatrixLine.coords[j][1] - 2.25) * scale + translationY);
                        Z = (int)Math.Round((MatrixLine.coords[j][2] - 1.5) * scale);
                        Spot ep = new Spot(X, Y, Z, 1);

                        lines.Add(new Line(sp, ep, pen));
                    }
                }
            }
        }
    }
    public class Circle
    {
        public Rectangle Bounds { get; set; }

        public void Move(Point currentMousePoint)
        {
            Bounds = new Rectangle(currentMousePoint, Bounds.Size);
        }

        public static void DrawCircle(Graphics g, Point center, Pen pen)
        {
            int radius = ArrayLine.circleRadius;
            Brush brush = new SolidBrush(pen.Color);
            g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
            g.DrawEllipse(pen, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        public static bool Contains(Point p1, Point p2)
        {
            int x = p1.X - p2.X;
            int y = p1.Y - p2.Y;
            int distance = (int)Math.Round(Math.Sqrt(x * x + y * y));
            return distance <= ArrayLine.circleRadius;
        }
    }
}
