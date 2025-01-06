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
    class MethodGradientDescentSplittingStep : MethodBase
    {
        //константа для метода градиентного спуска с дроблением шага
        double LAMBDA_METHOD2 = 1;

        //параметры для метода с дроблением шага
        double DELTA_FOR_METHOD2 = 0.5;
        double EPS_FOR_METHOD2 = 0.5;

        double[] gr; // градиент

        public event StopHandler? TimerNotify;
        public event InfoHandler? InfoNotify;

        // Градиентный метод с дроблением шага
        public MethodGradientDescentSplittingStep(double x1, double x2, Brush br)
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
            for (int j = 0; j < x.Length; j++) // x[] --> old[]
                old[j] = x[j];

            gr = Gradient(x);

            //вычисляем новое значение
            for (int j = 0; j < old.Length; j++)
                x[j] = x[j] - LAMBDA_METHOD2 * gr[j];

            //вычисляем квадрат нормы градиента
            s = 0;
            for (int j = 0; j < old.Length; j++)
                s += gr[j] * gr[j];

            while (F(x) > F(old) - EPS_FOR_METHOD2 * LAMBDA_METHOD2 * s)
            {
                LAMBDA_METHOD2 = LAMBDA_METHOD2 * DELTA_FOR_METHOD2;

                //пересчет значения Лямда
                for (int j = 0; j < x.Length; j++) // old[] --> x[]
                    x[j] = old[j];

                for (int j = 0; j < old.Length; j++)
                    x[j] = x[j] - LAMBDA_METHOD2 * gr[j];
            }

            path.Add(new double[] { x[0], x[1] });

            StopCondition();

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

        void StopCondition()
        {
            //условие останова
            s = 0;
            for (int j = 0; j < old.Length; j++)
                s += (old[j] - x[j]) * (old[j] - x[j]);

            s = Math.Sqrt(s);
            if (s < E)
            {
                TimerNotify?.Invoke();
                InfoNotify?.Invoke(x, iter);
                isFinished = true;

                iter = 1;
                return;
            }
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
