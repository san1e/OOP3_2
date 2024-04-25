using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP3_2
{
    public class PointColor
    {
        public Point Location { get; set; }
        public Color Color { get; set; }
        Random random = new Random();


        public PointColor(int x, int y, int color)
        {
            Location = new Point(x, y);
            int colorRandom = random.Next(0, 255);
            Color = Color.FromArgb((255 + color) % 255, (color * colorRandom) % 255, Math.Abs(255 - color) % 255);
        }
    }
}
