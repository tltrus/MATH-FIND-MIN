using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace _Chart2D
{
    internal class Golden_SectionMethod
    {
        private Point point;
        public Point Point
        {
            get
            {
                return this.point;
            }
        }

        double phi = (1 + Math.Sqrt(5)) / 2;    // phi - Фи, пропорция золотого сечения = 1,618..... 
        double a, b;          // a - start of section, b - end of section
        double x1 = 0.0; double x2 = 0.0;       // х1 - first point of section, х2 - second point of section
        double E = 0.05;                        // error

        public Golden_SectionMethod() { }

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
            if (Math.Abs(b - a) < E) return;

            x1 = b - (b - a) / phi;
            x2 = a + (b - a) / phi;

            double y1 = F(x1); double y2 = F(x2);

            if (y1 >= y2) a = x1;
            else b = x2;

            var Xmin = (a + b) / 2;
            var Ymin = F(Xmin);

            point = new Point(Xmin, Ymin);
        }
    }
}
