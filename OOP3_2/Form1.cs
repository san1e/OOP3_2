using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OOP3_2
{
    public partial class Form1 : Form
    {
        public Form1 form1;
        public Form2 form2;
        List<PointColor> listOfPoints = new List<PointColor>();
        List<Point> coordinates = new List<Point>();
        public int colorRGB = 0;
        public int count = 1;
        public int countPoints;
        Random random = new Random();
        private bool isPaintHandlerAdded = false;
        private bool isDrawPointHandlerAdded = false;
        public Stopwatch realTimetopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
            Opacity = 0;
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Ви бажаєте використати автоматичне заповнення?", "Вибір заповнення", MessageBoxButtons.YesNo);
            switch (result)
            {
                case DialogResult.Yes:
                    
                    form2 = new Form2();
                    form2.Show();
                    form2.FormClosed += Form2_FormClosed;
                    button1.Visible = false;
                    break;

                case DialogResult.No:
                    Opacity = 100;
                    MouseDown += OnMouseDown;
                    break;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point clickLocation = new Point(e.X, e.Y); 
                for (int i = listOfPoints.Count - 1; i >= 0; i--)
                {
                    Point pointLocation = listOfPoints[i].Location;

                    if (Math.Abs(clickLocation.X - pointLocation.X) <= 20 && Math.Abs(clickLocation.Y - pointLocation.Y) <= 20)
                    {
                        Brush backgroundColor = new SolidBrush(BackColor);
                        Graphics g = this.CreateGraphics();
                        g.FillEllipse(backgroundColor, pointLocation.X-4, pointLocation.Y-4, 8, 8);
                        listOfPoints.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                coordinates.Add(e.Location);
                Graphics g = CreateGraphics();
                g.FillEllipse(Brushes.Black, e.X-4, e.Y-4, 8, 8);
                colorRGB = random.Next(5, 255);
                listOfPoints.Add(new PointColor(e.X, e.Y, colorRGB * count));
                count++;
                countPoints++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isPaintHandlerAdded)
            {
                Paint += PixelDrawingForm1_Paint;
                isPaintHandlerAdded = true;
            }

            if (!isDrawPointHandlerAdded)
            {
                Paint += DrawPoint1;
                isDrawPointHandlerAdded = true;
            }

            this.Invalidate();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            countPoints = ((Form2)sender).numPoints;
            Opacity = 100;
            colorRGB = 255 / countPoints;
            RandomPoints(countPoints);
        }

        public void RandomPoints(int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.Paint += new PaintEventHandler(DrawPoint);
            }
            Paint += PixelDrawingForm1_Paint;
            Paint += DrawPoint1;
            this.Invalidate();
        }
        public void DrawPoint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int x = random.Next(0, this.ClientSize.Width-10);
            int y = random.Next(0, this.ClientSize.Height-10);
            g.FillEllipse(Brushes.Black, x-4, y-4, 8, 8);
            listOfPoints.Add(new PointColor(x, y, colorRGB * count));
            count++;
        }
        private void PixelDrawingForm1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            int cores = Environment.ProcessorCount;
            int linesPerCore = height / cores;
            realTimetopwatch.Start();
            Task[] tasks = new Task[cores];
            for (int core = 0; core < cores; core++)
            {
                int startY = core * linesPerCore;
                int endY = (core == cores - 1) ? height : (core + 1) * linesPerCore;

                tasks[core] = Task.Run(() =>
                {
                    for (int y = startY; y < endY; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            double distance = Distance(listOfPoints[0], x, y);
                            int num = 0;
                            for (int i = 1; i < listOfPoints.Count; i++)
                            {
                                double newDistance = Distance(listOfPoints[i], x, y);
                                if (distance >= newDistance)
                                {
                                    distance = newDistance;
                                    num = i;
                                }
                            }
                            Color color = listOfPoints[num].Color;
                            lock (g)
                            {
                                g.FillRectangle(new SolidBrush(color), x, y, 1, 1);
                            }
                        }
                    }
                });
            }
            Task.WaitAll(tasks);
            realTimetopwatch.Stop();
            DialogResult dialogResult = MessageBox.Show(realTimetopwatch.Elapsed.TotalSeconds.ToString());
        }
        public double Distance(PointColor point, int xPikcel, int yPikcel)
        {
            return Math.Sqrt(Math.Pow(point.Location.X - xPikcel, 2) + Math.Pow(point.Location.Y - yPikcel, 2));
        }

        private void DrawPoint1(object sender, PaintEventArgs e)
        {
            if (listOfPoints.Count == 0)
            {
                return; 
            }
            Graphics g = e.Graphics;
            foreach (PointColor point in listOfPoints)
            {
                g.FillEllipse(Brushes.Black, point.Location.X-4, point.Location.Y-4, 8, 8);
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
