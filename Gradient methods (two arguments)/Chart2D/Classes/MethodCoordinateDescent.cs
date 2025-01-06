using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _Chart2D.Classes
{
    internal class MethodCoordinateDescent : MethodBase
    {
        double[] old;
        double s;
        public event StopHandler? TimerNotify;
        public event InfoHandler? InfoNotify;

        // Метод покоординатного спуска
        public MethodCoordinateDescent(double x1, double x2, Brush br)
        {
            x = new double[] { x1, x2 };
            brush = br;

            old = new double[x.Length];

            path = new List<double[]>();
            path.Add(new double[] { x1, x2 });
        }

        // функция
        public void SetFunc(Func<double[], double> f) => F = f;

        public void Calculation()
        {
            for (int j = 0; j < x.Length; j++) // x[] --> old[]
                old[j] = x[j];

            for (int p = 0; p < x.Length; p++)
            {
                //ищем минимум вдоль p-й координаты
                x = GoldenSection(x, p, -10, 10);
                path.Add(new double[] { x[0], x[1] });
            }

            //условие останова
            s = Math.Abs(F(x) - F(old));
            if (s < E)
            {
                TimerNotify?.Invoke();
                InfoNotify?.Invoke(x, iter);
                isFinished = true;

                iter = 1;
                return;
            }

            iter++;
        }


        //метод золотого сечения одномерной оптимизации функции f
        //массив переменных x, оптимизация по переменной номер p на отрезке [a,b]
        double[] GoldenSection(double[] x, int p, double a, double b)
        {
            double phi = (1 + Math.Sqrt(5)) / 2;    // phi - Фи, пропорция золотого сечения = 1,618..... 
            double x1 = 0.0; double x2 = 0.0;       // х1 - первая точка, х2 - вторая точка


            while (b - a >= E)
            {
                x1 = x[p] = b - (b - a) / phi;
                double y1 = F(x);

                x2 = x[p] = a + (b - a) / phi;
                double y2 = F(x);

                if (y1 >= y2) a = x1;
                else b = x2;
            }

            x[p] = (a + b) / 2;

            return x;
        }

        public void Drawing(DrawingContext dc, double width, double height, double Xmin, double Xmax, double Ymin, double Ymax)
        {
            var X = x[0];
            var Y = x[1];
            Point norm = Tools.Normalize(new Point(X, Y), width, height, Xmin, Xmax, Ymin, Ymax);
            dc.DrawEllipse(brush, null, norm, 3, 3);

            for (int i = 0; i < path.Count - 1; ++i)
            {

                Point p0 = new Point(path[i][0], path[i][1]);           // current 
                Point p1 = new Point(path[i + 1][0], path[i + 1][1]);   // next

                Point norm0 = Tools.Normalize(p0, width, height, Xmin, Xmax, Ymin, Ymax);
                Point norm1 = Tools.Normalize(p1, width, height, Xmin, Xmax, Ymin, Ymax);

                Pen pen = new Pen(brush, 1);
                dc.DrawLine(pen, norm0, norm1);
            }
        }
    }
}
