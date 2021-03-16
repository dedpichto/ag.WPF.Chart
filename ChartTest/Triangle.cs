using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChartTest
{
    public class Triangle
    {
        public Triangle(Point p1, Point p2, Point p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public Point P1 { get; set; }
        public Point P2 { get; set; }
        public Point P3 { get; set; }

        public bool PointInTriangle(Point p)
        {
            var s = P3.Y * P3.X - P3.X * P3.Y + (P3.Y - P3.Y) * p.X + (P3.X - P3.X) * p.Y;
            var t = P3.X * P3.Y - P3.Y * P3.X + (P3.Y - P3.Y) * p.X + (P3.X - P3.X) * p.Y;

            if ((s < 0) != (t < 0))
                return false;

            var a = -P3.Y * P3.X + P3.Y * (P3.X - P3.X) + P3.X * (P3.Y - P3.Y) + P3.X * P3.Y;
            if (a < 0.0)
            {
                s = -s;
                t = -t;
                a = -a;
            }
            return s > 0 && t > 0 && (s + t) <= a;
        }
    }
}
