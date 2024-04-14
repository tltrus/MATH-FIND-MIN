using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace _Chart2D
{
    internal class BisectionMethod
    {
        private Point point;
        public Point Point
        {
            get
            {
                return this.point;
            }
        }

        double G = 0.1; // step
        double E = 0.05; // error

        double a;   // start of section
        double b;    // end of section

        double x1;    // first point of section
        double x2;    // second point of section


        public BisectionMethod() { }

        public void SetPoint(double x, double y)
        {
            point.X = x;
            point.Y = y;
        }

        public void SetSection(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public void Calculation(Func<double, double> F)
        {
            if ((b - a) / 2 >= E)
            {
                x1 = (a + b - G) / 2;
                x2 = (a + b + G) / 2;

                double y1 = F(x1);
                double y2 = F(x2);

                if (y1 > y2) a = x1;
                if (y1 < y2) b = x2;

                var Xmin = (a + b) / 2;
                var Ymin = F(Xmin);

                point = new Point(Xmin, Ymin);
            }
        }
    }
}
