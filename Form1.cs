using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;

namespace Paint3
{
    public partial class Form1 : Form
    {
        public Point lastMousePosition;//позиция мыши
        public Point lastObjectPosition;//позиция дома
        public bool isMousePressed;//нажата мышь?
        public double lastPosX;
        public double lastPosY;
        public double lastPosZ;

        private List<GroupLines> groupsLine;//лист групп
        private ArrayLine lines;//главная группа
        private MatrixLine matrixLine;

        private Spline splines;//сплайн

        private int dX;//сдвиг вершины
        private int dY;

        private bool edit;
        private bool group;
        private bool spline;
        private bool dimension;
        private bool XZ;

        private bool newLine;

        private Bitmap bm;
        private Graphics g;

        private Pen mainPen;

        private ColorDialog cd;

        public Form1()
        {
            InitializeComponent();

            groupsLine = new List<GroupLines>();
            lines = new ArrayLine();
            lines.Name = "Main";
            matrixLine=new MatrixLine();

            comboBox1.Items.Add(lines.Name);
            comboBox1.SelectedIndex = 0;

            splines = new Spline();

            ArrayLine.isMove = false;
            ArrayLine.MoveGroup = false;
            dX = 0;
            dY = 0;
            mainPen = new Pen(Color.Black, 2);


            edit = false;
            group = false;
            spline = false;
            newLine = false;

            this.Width = 950;
            this.Height = 700;
            bm = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;

            cd = new ColorDialog();

            radioButton1.Checked = true;
        }
        private void pic_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && ModifierKeys == Keys.Control)
            {
                float scaleFactor = 1.1f;
                if (MatrixLine.S)
                {
                    matrixLine.scale *= scaleFactor;
                    //for (int i = 0; i < lines.Count; i++)
                    //{
                    //    MatrixLine.Scale(lines[i].startPoint);
                    //    MatrixLine.Scale(lines[i].endPoint);
                    //}
                }
                else if (matrixLine.zc * 0.9 > 1) 
                    matrixLine.zc *= 0.9;
            }
            else if (e.Delta < 0 && ModifierKeys == Keys.Control)
            {
                float scaleFactor = 0.9f;
                if (MatrixLine.S)
                {
                    matrixLine.scale *= scaleFactor;
                    //for (int i = 0; i < lines.Count; i++)
                    //{
                    //    MatrixLine.Scale(lines[i].startPoint);
                    //    MatrixLine.Scale(lines[i].endPoint);
                    //}
                }
                else
                    matrixLine.zc *= 1.1;
            }
            lines.Draw(matrixLine);
            pic.Refresh();
        }
        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePosition = e.Location;
            isMousePressed = true;
            //lines.indexCircleList.Clear();

            if (spline)
            {
                for (int i = 0; i < splines.points.Count; i++)
                {
                    Spot sp = matrixLine.Rotate(splines.points[i]);
                    if (Circle.Contains(sp.ToP, e.Location))
                    {
                        splines.isMousePressed = true;
                        splines.pIndex = i;
                    }
                }
                if (!splines.isMousePressed)
                {
                    Random rnd = new Random();
                    double z = rnd.Next(1000) - 500;
                    splines.points.Add(new Spot(e.Location.X, e.Location.Y, z, 1));

                    pic.Refresh();
                }
            }             
            else
            {
                lines.indexColorMove = -1;
                lines.indexColorLine = -1;
                label4.Text = "";
                label5.Text = "";
                label6.Text = "";

                if (edit)//выделить 1 прямую
                {
                    for (int i = 0; i < lines.lines.Count; i++)
                    {
                        if (lines.linesDraw[i].Contains(e.Location))
                        {
                            lines.indexColorLine = i;
                        }
                        if (Circle.Contains(e.Location, lines.linesDraw[i].startPoint.ToP))
                        {
                            lines.indexColorLine = i;
                        }
                        if (Circle.Contains(e.Location, lines.linesDraw[i].endPoint.ToP))
                        {
                            lines.indexColorLine = i;
                        }
                    }
                    if (lines.indexColorLine != -1)//коэффициенты уравнения прямой
                    {
                        if (Math.Abs(lines[lines.indexColorLine].coefA) <= 1)
                        {
                            label4.Text = "0.00";
                            label5.Text = (-Math.Round((decimal)lines[lines.indexColorLine].coefB, 2)).ToString();
                            label6.Text = (-Math.Round((decimal)lines[lines.indexColorLine].coefC)).ToString();
                        }
                        else
                        {
                            label4.Text = (lines[lines.indexColorLine].coefA / lines[lines.indexColorLine].coefA).ToString();
                            label5.Text = (-Math.Round((decimal)lines[lines.indexColorLine].coefB / lines[lines.indexColorLine].coefA, 2)).ToString();
                            label6.Text = (-Math.Round((decimal)lines[lines.indexColorLine].coefC / lines[lines.indexColorLine].coefA, 2)).ToString();
                        }
                    }                    
                }
                else if (group)//выделить группу прямых
                {
                    int selected = -1;

                    for (int i = 0; i < lines.linesDraw.Count; i++)
                    {
                        if (Circle.Contains(e.Location, lines.linesDraw[i].startPoint.ToP))
                        {
                            selected = i;
                        }
                        if (Circle.Contains(e.Location, lines.linesDraw[i].endPoint.ToP))
                        {
                            selected = i;
                        }
                        if (lines.linesDraw[i].Contains(e.Location))
                        {
                            selected = i;
                        }
                    }

                    if (selected != -1)
                        lines[selected].IsSelected = true;
                }
                else if (ArrayLine.MoveGroup)//передвинуть группу
                {                    
                    foreach (var arrayLine in groupsLine)
                        if (arrayLine.Name == comboBox1.SelectedItem.ToString())
                        {
                            foreach (var item in arrayLine.group)
                            {
                                if (XZ)
                                {
                                    lines[item].lastSP = new Point(e.X - lines[item].startPoint.X, (int)(e.Y - lines[item].startPoint.Z));
                                    lines[item].lastEP = new Point(e.X - lines[item].endPoint.X, (int)(e.Y - lines[item].endPoint.Z));
                                }
                                else
                                {
                                    lines[item].lastSP = new Point(e.X - lines[item].startPoint.X, e.Y - lines[item].startPoint.Y);
                                    lines[item].lastEP = new Point(e.X - lines[item].endPoint.X, e.Y - lines[item].endPoint.Y);
                                }
                            }
                        }
                }
                else//сдвиг вершины или новая прямая
                {
                    for (int i = 0; i < lines.linesDraw.Count; i++)
                    {
                        if (Circle.Contains(e.Location, lines.linesDraw[i].startPoint.ToP))
                        {
                            dX = e.X - lines.linesDraw[i].startPoint.X;
                            dY = e.Y - lines.linesDraw[i].startPoint.Y;
                            ArrayLine.isMove = true;
                            lines.indexCircle = i * 2;
                            lines.indexCircleList.Add(i * 2);
                        }
                        if (Circle.Contains(e.Location, lines.linesDraw[i].endPoint.ToP))
                        {
                            dX = e.X - lines.linesDraw[i].endPoint.X;
                            dY = e.Y - lines.linesDraw[i].endPoint.Y;
                            ArrayLine.isMove = true;
                            lines.indexCircle = i * 2 + 1;
                            lines.indexCircleList.Add(i * 2 + 1);
                        }
                    }
                    if (!ArrayLine.isMove && newLine) 
                    {                        
                        Line line = new Line(new Spot(e.Location), new Spot(e.Location), mainPen);
                        lines.Add(line);
                        lines.linesDraw.Add(line);

                        ArrayLine.isMove = true;
                        lines.indexCircle = lines.Count * 2 - 1;
                        lines.indexCircleList.Add(lines.Count * 2 - 1);
                    }
                    if (comboBox1.SelectedItem.ToString() == "Main" && XZ)
                    {
                        foreach (int index in lines.indexCircleList)
                            if (index % 2 == 0)
                            {
                                lines[index / 2].lastSP = new Point(e.X - lines[index / 2].startPoint.X, (int)(e.Y - lines[index / 2].startPoint.Z));
                            }
                            else
                            {
                                lines[index / 2].lastEP = new Point(e.X - lines[index / 2].endPoint.X, (int)(e.Y - lines[index / 2].endPoint.Z));
                            }
                    }
                }
            }
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (splines.isMousePressed)
            {
                if (XZ)
                {
                    splines.points[splines.pIndex].X = (int)Math.Round((double)e.Location.X);
                    splines.points[splines.pIndex].Z = e.Location.Y / matrixLine.scale - matrixLine.translationY / matrixLine.scale;
                }
                else
                {
                    splines.points[splines.pIndex].X = e.Location.X;
                    splines.points[splines.pIndex].Y = e.Location.Y;
                }
                pic.Refresh();
            }
            if (isMousePressed)
            {
                int deltaX = e.X - lastMousePosition.X;
                int deltaY = e.Y - lastMousePosition.Y;
               
                if (MatrixLine.R)
                {
                    matrixLine.rotationX += deltaX * 0.5;
                    matrixLine.rotationY += deltaY * 0.5;
                    
                    lastMousePosition = e.Location;
                }
            }
            if (ArrayLine.isMove)// && !XZ) 
            {
                if (comboBox1.SelectedItem.ToString() == "Main" && XZ)
                {
                    foreach (int i in lines.indexCircleList)
                    {
                        if (i % 2 == 0)
                        {
                            lines[i / 2].startPoint = new Spot(e.X - lines[i / 2].lastSP.X, lines[i / 2].startPoint.Y, e.Y - lines[i / 2].lastSP.Y, 1);
                        }
                        else
                        {
                            lines[i / 2].endPoint = new Spot(e.X - lines[i / 2].lastEP.X, lines[i / 2].endPoint.Y, e.Y - lines[i / 2].lastEP.Y, 1);
                        }
                    }
                }
                else
                    foreach (int index in lines.indexCircleList)
                        if (index % 2 == 0)
                        {
                            lines[index / 2].startPoint = new Spot(e.X - dX, e.Y - dY, lines[index / 2].startPoint.Z, 1);
                        }
                        else
                        {
                            lines[index / 2].endPoint = new Spot(e.X - dX, e.Y - dY, lines[index / 2].endPoint.Z, 1);
                        }
            }
            if (ArrayLine.MoveGroup && isMousePressed) 
            {                
                    ////for (int i = 0; i < lines.Count; i++)
                    ////{
                    ////    if (i == lines.indexColorLine || i == lines.indexColorMove)
                    ////    {
                    ////        lines[i].startPoint = new Spot(e.X - lines[i].lastSP.X, lines[i].startPoint.Y, e.Y - lines[i].lastSP.Y, 1);
                    ////        lines[i].endPoint = new Spot(e.X - lines[i].lastEP.X, lines[i].endPoint.Y, e.Y - lines[i].lastEP.Y, 1);
                    ////    }
                    ////    else
                    ////    {
                    ////        if (lines.indexCircleList.Contains(i * 2))
                    ////        {
                    ////            transBT.BackColor = Color.White;
                    ////            lines[i].startPoint = new Spot(e.X - lines[i].lastSP.X, lines[i].startPoint.Y, e.Y - lines[i].lastSP.Y, 1);
                    ////        }

                    ////        if (lines.indexCircleList.Contains(i * 2 + 1))
                    ////        {
                    ////            transBT.BackColor = Color.White;
                    ////            lines[i].endPoint = new Spot(e.X - lines[i].lastEP.X, lines[i].endPoint.Y, e.Y - lines[i].lastEP.Y, 1);
                    ////        }
                    //////    }
                    //}                
                foreach (var arrayLine in groupsLine)                    
                    if (arrayLine.Name == comboBox1.SelectedItem.ToString())
                    {
                        foreach (var item in arrayLine.group)
                        {                            
                            if (XZ)
                            {
                                lines[item].startPoint = new Spot(e.X - lines[item].lastSP.X, lines[item].startPoint.Y, e.Y - lines[item].lastSP.Y, 1);
                                lines[item].endPoint = new Spot(e.X - lines[item].lastEP.X, lines[item].endPoint.Y, e.Y - lines[item].lastEP.Y, 1);
                            }
                            else
                            {
                                lines[item].startPoint = new Spot(e.X - lines[item].lastSP.X, e.Y - lines[item].lastSP.Y, lines[item].startPoint.Z, 1);
                                lines[item].endPoint = new Spot(e.X - lines[item].lastEP.X, e.Y - lines[item].lastEP.Y, lines[item].endPoint.Z, 1);
                            }
                        }
                    }
            }

            lines.Draw(matrixLine);
            pic.Refresh();
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            lines.indexCircleList.Clear();
            splines.isMousePressed = false;
            splines.pIndex = -1;
            isMousePressed = false;
            ArrayLine.isMove = false;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            lines.Paint(g);
            splines.Paint(g, matrixLine);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            mainPen.Width = 1;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            mainPen.Width = 3;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            mainPen.Width = 5;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            mainPen.Width = 8;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            mainPen.Width = 10;
        }

        private void button2_Click(object sender, EventArgs e)
        {           
            if (!edit)
            {
                edit = true;
                editBT.BackColor = Color.FromArgb(224, 224, 224);
                editBT.ForeColor = Color.Black;
            }
            else
            {
                edit = false;
                editBT.BackColor = Color.FromArgb(64, 64, 64);
                editBT.ForeColor = Color.White;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lines.indexColorLine != -1)
            {
                lines.RemoveAt(lines.indexColorLine);
                foreach (var arr in groupsLine)
                {
                    if (arr.group.Contains(lines.indexColorLine))
                    {
                        arr.group.Remove(lines.indexColorLine);
                        for (int i=0;i<arr.group.Count;i++)
                            if (arr.group[i] > lines.indexColorLine)
                                --arr.group[i];
                    }
                }
                lines.indexColorLine = -1;
                label4.Text = "";
                label5.Text = "";
                label6.Text = "";
                lines.Draw(matrixLine);
                pic.Refresh();
            }
        }

        private void pic_color_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            Color color = Color.FromArgb(cd.Color.ToArgb());
            mainPen.Color = color;
            pic_color.BackColor = cd.Color;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!group)
            {
                group = true;
                newGroup.BackColor = Color.FromArgb(224, 224, 224);
                newGroup.ForeColor = Color.Black;
            }
            else
            {
                group = false;
                newGroup.BackColor = Color.FromArgb(64, 64, 64);
                newGroup.ForeColor = Color.White;

                GroupLines arrayLine = new GroupLines();
                arrayLine.Name = textBox1.Text;
                comboBox1.Items.Add(arrayLine.Name);
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].IsSelected)
                    {
                        lines[i].IsSelected = false;
                        arrayLine.group.Add(i);
                    }
                }
                groupsLine.Add(arrayLine);

                textBox1.Text = "";

                pic.Refresh();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayLine.MoveGroup = false;
            transBT.BackColor = Color.FromArgb(64, 64, 64);
            transBT.ForeColor = Color.White;

            foreach (var line in lines.lines)
            {
                line.IsSelected = false;
            }

            foreach (var arrayLine in groupsLine)
                if (arrayLine.Name == comboBox1.SelectedItem.ToString())
                {
                    foreach (var item in arrayLine.group)
                        lines[item].IsSelected = true;
                    pic.Refresh();
                }
            pic.Refresh();

        }

        private void button3_Click(object sender, EventArgs e)//сдвиг группы
        {
            if (!ArrayLine.MoveGroup)
            {
                ArrayLine.MoveGroup = true;
                transBT.BackColor = Color.FromArgb(224, 224, 224);
                transBT.ForeColor = Color.Black;
            }
            else
            {
                ArrayLine.MoveGroup = false;
                transBT.BackColor = Color.FromArgb(64, 64, 64);
                transBT.ForeColor = Color.White;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            foreach (var arrayLine in groupsLine)
                if (arrayLine.Name == comboBox1.SelectedItem.ToString())
                {
                    ArrayLine arrl = new ArrayLine();
                    foreach (var n in arrayLine.group)
                        arrl.Add(lines[n]);
                    arrl.MirrorV();
                    lines.Draw(matrixLine);
                    pic.Refresh();
                }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            foreach (var arrayLine in groupsLine)
                if (arrayLine.Name == comboBox1.SelectedItem.ToString())
                {
                    double scale = (double)numericUpDown2.Value;
                    ArrayLine arrl = new ArrayLine();
                    foreach (var n in arrayLine.group)
                        arrl.Add(lines[n]);
                    arrl.Scale(scale, matrixLine);
                    numericUpDown2.Value = 1;
                    lines.Draw(matrixLine);
                    pic.Refresh();
                }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (var arrayLine in groupsLine)
                if (arrayLine.Name == comboBox1.SelectedItem.ToString())
                {
                    ArrayLine arrl = new ArrayLine();
                    foreach (var n in arrayLine.group)
                        arrl.Add(lines[n]);
                    arrl.MirrorG();
                    lines.Draw(matrixLine);
                    pic.Refresh();
                }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            foreach (var arrayLine in groupsLine)
                if (arrayLine.Name == comboBox1.SelectedItem.ToString())
                {
                    double degrees = (double)numericUpDown1.Value;
                    ArrayLine arrl = new ArrayLine();
                    foreach (var n in arrayLine.group)
                        arrl.Add(lines[n]);
                    arrl.Rotate(degrees);
                    numericUpDown1.Value = 0;
                    lines.Draw(matrixLine);
                    pic.Refresh();
                }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            matrixLine.mirrorV = !matrixLine.mirrorV;
            lines.Draw(matrixLine);
            pic.Refresh();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            matrixLine.mirrorG = !matrixLine.mirrorG;
            lines.Draw(matrixLine);
            pic.Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!MatrixLine.S)
            {
                MatrixLine.S = true;
                button7.BackColor = Color.FromArgb(224, 224, 224);
                button7.ForeColor = Color.Black;
            }
            else
            {
                MatrixLine.S = false;
                button7.BackColor = Color.FromArgb(64, 64, 64);
                button7.ForeColor = Color.White;
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                string jsonGL = JsonSerializer.Serialize(groupsLine);
                string jsonAL = JsonSerializer.Serialize(lines);
                string jsonSL = JsonSerializer.Serialize(splines);
                string jsonML = JsonSerializer.Serialize(matrixLine);

                WriteJsonToFileAsync(jsonGL, $"{selectedPath}/GroupsLine.json");
                WriteJsonToFileAsync(jsonAL, $"{selectedPath}/ArrayLine.json");
                WriteJsonToFileAsync(jsonSL, $"{selectedPath}/Spline.json");
                WriteJsonToFileAsync(jsonML, $"{selectedPath}/MatrixLine.json");
            }
        }
        public async void WriteJsonToFileAsync(string jsonString, string filePath)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                await streamWriter.WriteAsync(jsonString);
            }
        }

        private async void button10_ClickAsync(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                string fileName1 = $"{selectedPath}/GroupsLine.json";
                string fileName2 = $"{selectedPath}/ArrayLine.json";
                string fileName3 = $"{selectedPath}/Spline.json";
                string fileName4 = $"{selectedPath}/MatrixLine.json";
                
                using (FileStream openStream = File.OpenRead(fileName1))
                {
                    groupsLine =
                        await JsonSerializer.DeserializeAsync<List<GroupLines>>(openStream);
                }
                using (FileStream openStream = File.OpenRead(fileName2))
                {
                    lines =
                        await JsonSerializer.DeserializeAsync<ArrayLine>(openStream);
                }
                using (FileStream openStream = File.OpenRead(fileName3))
                {
                    splines =
                        await JsonSerializer.DeserializeAsync<Spline>(openStream);
                }
                using (FileStream openStream = File.OpenRead(fileName4))
                {
                    matrixLine =
                        await JsonSerializer.DeserializeAsync<MatrixLine>(openStream);
                }
                comboBox1.Items.Clear();
                comboBox1.Items.Add("Main");
                foreach (var group in groupsLine)
                    comboBox1.Items.Add(group.Name);
                comboBox1.SelectedItem = comboBox1.Items[0];
                comboBox1.Refresh();
                lines.Draw(matrixLine);
                pic.Refresh();
            }            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!spline)
            {
                spline = true;
                button11.BackColor = Color.FromArgb(224, 224, 224);
                button11.ForeColor = Color.Black;
            }
            else
            {
                spline = false;
                button11.BackColor = Color.FromArgb(64, 64, 64);
                button11.ForeColor = Color.White;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!splines.clear)
            {
                splines.clear = true;
                pic.Refresh();
                button12.BackColor = Color.FromArgb(224, 224, 224);
                button12.ForeColor = Color.Black;
            }
            else
            {
                splines.clear = false;
                pic.Refresh();
                button12.BackColor = Color.FromArgb(64, 64, 64);
                button12.ForeColor = Color.White;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            splines = new Spline();
            pic.Refresh();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            if (!newLine)
            {
                newLine = true;
                button16.BackColor = Color.FromArgb(224, 224, 224);
                button16.ForeColor = Color.Black;
            }
            else
            {
                newLine = false;
                button16.BackColor = Color.FromArgb(64, 64, 64);
                button16.ForeColor = Color.White;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {            
            if (!MatrixLine.R)
            {
                matrixLine.zc = 700;
                //MatrixLine.rotationX = 30;
                //MatrixLine.rotationY = 45;
                MatrixLine.R = true;
                button15.BackColor = Color.FromArgb(224, 224, 224);
                button15.ForeColor = Color.Black;

                dimension = false;
                button9.BackColor = Color.FromArgb(64, 64, 64);
                button9.ForeColor = Color.White;

                XZ = false;
                button19.BackColor = Color.FromArgb(64, 64, 64);
                button19.ForeColor = Color.White;

                newLine = false;
                button16.BackColor = Color.FromArgb(64, 64, 64);
                button16.ForeColor = Color.White;
                lines.Draw(matrixLine);
                pic.Refresh();
            }
            else
            {
                MatrixLine.R = false;
                button15.BackColor = Color.FromArgb(64, 64, 64);
                button15.ForeColor = Color.White;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            int countLines = lines.Count;
            lines.DrawHouse(matrixLine);

            GroupLines arrayLine = new GroupLines();
            int n = 1;
            do
            {
                arrayLine.Name = $"House{n}";
                n++;
            } while (comboBox1.Items.Contains(arrayLine.Name));
            comboBox1.Items.Add(arrayLine.Name);

            for (int i = countLines; i < lines.Count; i++)
            {
                arrayLine.group.Add(i);
            }
            groupsLine.Add(arrayLine);

            lines.Draw(matrixLine);
            pic.Refresh();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            int delete = -1;
            for (int n = 0; n < groupsLine.Count; n++)
                if (groupsLine[n].Name == comboBox1.SelectedItem.ToString())
                {
                    for (int i = 0; i < groupsLine[n].group.Count; i++) 
                    {
                        lines.RemoveAt(groupsLine[n].group[i]);
                        for (int k = 0; k < groupsLine.Count; k++)
                            for (int j = 0; j < groupsLine[k].group.Count; j++)
                                if (groupsLine[k].group[j] > groupsLine[n].group[i])
                                    --groupsLine[k].group[j];
                    }
                    delete = n;
                }
            if (delete != -1)
            {
                groupsLine.RemoveAt(delete);
                comboBox1.Items.RemoveAt(comboBox1.SelectedIndex);
            }
            lines.Draw(matrixLine);
            pic.Refresh();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!dimension)
            {
                dimension = true;
                button9.BackColor = Color.FromArgb(224, 224, 224);
                button9.ForeColor = Color.Black;
                matrixLine.scale = 1;
                matrixLine.zc = 1000000;
                matrixLine.rotationX = 0;
                matrixLine.rotationY = 0;
                lines.Draw(matrixLine);
                pic.Refresh();
                MatrixLine.R = false;
                button15.BackColor = Color.FromArgb(64, 64, 64);
                button15.ForeColor = Color.White;
                XZ = false;
                button19.BackColor = Color.FromArgb(64, 64, 64);
                button19.ForeColor = Color.White;
            }
            else
            {
                dimension = false;
                button9.BackColor = Color.FromArgb(64, 64, 64);
                button9.ForeColor = Color.White;
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (!XZ)
            {
                matrixLine.scale = 1;
                matrixLine.rotationX = 0;
                matrixLine.rotationY = 90;
                matrixLine.zc = 1000000;
                XZ = true;
                button19.BackColor = Color.FromArgb(224, 224, 224);
                button19.ForeColor = Color.Black;
                lines.Draw(matrixLine);
                pic.Refresh();
                MatrixLine.R = false;
                button15.BackColor = Color.FromArgb(64, 64, 64);
                button15.ForeColor = Color.White;
                dimension = false;
                button9.BackColor = Color.FromArgb(64, 64, 64);
                button9.ForeColor = Color.White;
            }
            else
            {
                XZ = false;
                button19.BackColor = Color.FromArgb(64, 64, 64);
                button19.ForeColor = Color.White;
            }
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            matrixLine.mirrorGl = !matrixLine.mirrorGl;
            lines.Draw(matrixLine);
            pic.Refresh();
        }
    }
}