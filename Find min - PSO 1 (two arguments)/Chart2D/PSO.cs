using System.Windows;
using System.Windows.Media;

namespace _Chart2D
{
    // Particle swarm optimization
    internal class PSO
    {
        Random rnd = new Random();
        int m = 200; // количество частиц
        double alpha = 0.2;
        double beta = 0.2;
        double gamma = 0.6;

        public List<Vector2D> X;
        List<Vector2D> V;
        List<Vector2D> p; // минимальная точка частицы
        Vector2D g; // минимальная точка всего роя

        Func<double, double, double> F;

        public PSO(Func<double, double, double> f)
        {
            X = new List<Vector2D>();
            V = new List<Vector2D>();
            p = new List<Vector2D>(); // минимальная точка частицы
            Vector2D g = new Vector2D(); // минимальная точка всего роя

            F = f;
        }

        public void Init(int min, int max)
        {
            // Создание случайного роя из m частиц 
            // и инициализация скоростей частиц
            for (int i = 0; i < m; i++)
            {
                Vector2D x = new Vector2D();
                x.x = rnd.Next(min, max);
                x.y = rnd.Next(min, max);
                X.Add(x);
                p.Add(x);

                Vector2D v = new Vector2D();
                v.x = rnd.NextDouble() * rnd.Next(-1, 2);
                v.y = rnd.NextDouble() * rnd.Next(-1, 2);

                V.Add(v);
            }

            // Вычисление вектора g - минимум всего роя
            g = CalcMinAll(p);
        }
        private Vector2D CalcMinAll(List<Vector2D> P)
        {
            Vector2D res = P[0];

            foreach (var p in P)
                if (F(p.x, p.y) < F(res.x, res.y)) res = p;

            return res;
        }

        public void Clculation()
        {
            // цикл частиц
            for (int i = 0; i < m; i++)
            {
                // Формула расчета скорости отдельных частиц
                // vi = Gamma*vi + Alpha*(pi - xi) + Beta*(g - xi):
                V[i] = gamma * V[i] + alpha * (p[i] - X[i]) + beta * (g - X[i]);

                // Изменение положения частицы
                X[i] = X[i] + V[i];

                if (F(X[i].x, X[i].y) < F(p[i].x, p[i].y))
                    p[i] = X[i];

            }
            g = CalcMinAll(p);
        }

        public void Drawing(DrawingContext dc, Axis axis)
        {
            // цикл частиц
            for (int i = 0; i < m; i++)
            {
                var x = axis.Xto(X[i].x);
                var y = axis.Yto(X[i].y);
                dc.DrawEllipse(Brushes.Red, null, new Point(x, y), 2, 2);
            }
        }
    }
}
