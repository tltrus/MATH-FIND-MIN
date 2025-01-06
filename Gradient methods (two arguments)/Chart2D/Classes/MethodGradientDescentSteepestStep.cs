using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace _Chart2D.Classes
{
    class MethodGradientDescentSteepestStep : MethodBase
    {
        double[] gr; // градиент

        public event StopHandler? TimerNotify;
        public event InfoHandler? InfoNotify;

        // Наискорейший спуск = Золотое сечение + Градиент
        public MethodGradientDescentSteepestStep(double x1, double x2, Brush br)
        {
            x = new double[] { x1, x2 };
            brush = br;

            old = new double[x.Length];
            gr = new double[2];

            path = new List<double[]>();
            path.Add(new double[] { x1, x2 });
        }

        public void Calculation()
        {
            x = SteepestDescent(x, -10, 10);

            path.Add(new double[] { x[0], x[1] });

            //условие остановa
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

        //градиент исследуемой функции
        double[] Gradient(double[] x)
        {
            return new double[] {
                    10 * x[0],
                    2 * x[1]
                };
        }

        /// <summary>
        /// Метод 3. Наискорейший спуск = Золотое сечение + Градиент
        /// </summary>
        /// <param name="x">Аргументы многомерной функции</param>
        /// <param name="a">Начало отрезка</param>
        /// <param name="b">Конец отрезка</param>
        /// <returns></returns>
        double[] SteepestDescent(double[] x, double a, double b)
        {
            double phi = (1 + Math.Sqrt(5)) / 2;    // phi - Фи, пропорция золотого сечения = 1,618..... 
            double[] GF = Gradient(x);
            double[] tmp = new double[2];
            double u1, u2;
            double fu1, fu2;
            int j;

            while (b - a >= E)
            {
                u1 = b - (b - a) / phi;
                u2 = a + (b - a) / phi;

                for (j = 0; j < x.Length; j++)
                    tmp[j] = x[j] + u1 * GF[j];
                fu1 = F(tmp);

                for (j = 0; j < x.Length; j++)
                    tmp[j] = x[j] + u2 * GF[j];
                fu2 = F(tmp);

                if (fu1 >= fu2) a = u1;
                else b = u2;
            }
            return tmp;
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
