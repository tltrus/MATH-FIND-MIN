using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _Chart2D
{
    internal class Quadr3D
    {
        Brush color;
        double xMin, yMin, xMax, yMax, step;
        List<Point3D> points;
        double minZ;
        double maxZ;
        Func<double, double, double> F;

        public Quadr3D(Func<double, double, double> func)
        {
            color = Brushes.Red;
            points = new List<Point3D>();
            F = func;
        }

        public void Init(double xMin, double xMax, double yMin, double yMax, double step)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.xMax = xMax;
            this.yMax = yMax;
            this.step = step;
        }

        public void CalMinMaxZ()
        {
            minZ = points.Min(v => v.Z);
            maxZ = points.Max(v => v.Z);
        }

        public void Calculation(Axis axis)
        {
            points.Clear();

            for (double y = yMin; y < yMax; y += step)
            {
                for (double x = xMin; x < xMax; x += step)
                {
                    double Z = F(x, y);

                    double X = axis.Xto(x); 
                    double Y = axis.Yto(y); 

                    points.Add(new Point3D(X, Y, Z));
                }
            }
        }

        public void Draw(DrawingContext dc)
        {
            foreach (var p in points)
            {
                var rgb = (byte)Tools.Map(p.Z, minZ, maxZ, 0, 255);
                Brush brush = new SolidColorBrush(Color.FromRgb(rgb, rgb, rgb));

                Point p2D = new Point(p.X, p.Y);
                dc.DrawEllipse(brush, null, p2D, 2, 2);
            }
        }
    }
}
