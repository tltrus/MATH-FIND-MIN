using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _Chart2D.Classes
{
    internal class MethodBase
    {
        public double[] x = { 9, -9 };  // исходная точка (x, y)
        public double E = 0.0001;      // ошибка

        public Func<double[], double> F;

        public double[] old;
        public double s;

        public delegate void StopHandler();
        public delegate void InfoHandler(double[] x, int iter);

        public int iter = 1;

        public List<double[]> path;

        public Brush brush;

        public bool isFinished;

        // функция
        public void SetFunc(Func<double[], double> f) => F = f;
    }
}
