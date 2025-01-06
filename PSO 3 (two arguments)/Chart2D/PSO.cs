using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _Chart2D
{
    // Particle swarm optimization
    internal class PSO
    {
        Random ran = new Random();

        public int numberParticles = 20; // количество частиц
        int Dim = 2; // размерность (количество переменных функции)
        public double minX, maxX;

        Particle[] swarm;
        double[] bestGlobalPosition;
        double bestGlobalFitness;
        double minV;
        double maxV;

        double w = 0.729; // весовая доля инерции
        double c1 = 1.49445; // когнитивная (локальная) весовая доля
        double c2 = 1.49445; // социальная (глобальная) весовая доля
        double r1, r2; // cognitive and social рандомизация

        Func<double[], double> ObjectiveFunction;

        public PSO(Func<double[], double> f)
        {
            ObjectiveFunction = f;
        }

        public void Init(double minX, double maxX)
        {
            this.minX = minX;
            this.maxX = maxX;
            
            swarm = new Particle[numberParticles];
            bestGlobalPosition = new double[Dim]; // best solution found by any particle in the swarm. implicit initialization to all 0.0
            bestGlobalFitness = double.MaxValue; // smaller values better

            minV = -1.0 * maxX;
            maxV = maxX;

            // частицы в рое инициализируются случайной позицией. 
            // Позиция частицы представляет возможное решение выполняемой задачи оптимизации
            for (int i = 0; i < swarm.Length; ++i) // Цикл по каждой частице
            {
                // Рандомно вычисляем позицию
                double[] randomPosition = new double[Dim];
                for (int j = 0; j < randomPosition.Length; ++j)
                {
                    double lo = minX;
                    double hi = maxX;
                    randomPosition[j] = (hi - lo) * ran.NextDouble() + lo; // 
                }
                double fitness = ObjectiveFunction(randomPosition);

                // Рандомно вычисляем скорость
                double[] randomVelocity = new double[Dim];

                for (int j = 0; j < randomVelocity.Length; ++j)
                {
                    double lo = -1.0 * Math.Abs(maxX - minX);
                    double hi = Math.Abs(maxX - minX);
                    randomVelocity[j] = (hi - lo) * ran.NextDouble() + lo;
                }

                swarm[i] = new Particle(randomPosition, fitness, randomVelocity, randomPosition, fitness);

                /*
                 * является ли добротность текущего Particle лучшей на данный момент (минимальной в случае задачи нахождения минимума). 
                 * Если да, я обновляю массив bestGlobalPosition и соответствующую переменную bestGlobalFitness.
                 */
                if (swarm[i].fitness < bestGlobalFitness)
                {
                    bestGlobalFitness = swarm[i].fitness;
                    swarm[i].position.CopyTo(bestGlobalPosition, 0);
                }
            } // Инициализация

        }

        public void Clculation()
        {
            double[] newVelocity = new double[Dim];
            double[] newPosition = new double[Dim];
            double newFitness;

            for (int i = 0; i < swarm.Length; ++i) // Цикл по каждой частице
            {
                Particle currP = swarm[i];

                for (int j = 0; j < currP.velocity.Length; ++j) // each x value of the velocity (X0 and X1)
                {
                    r1 = ran.NextDouble();
                    r2 = ran.NextDouble();

                    newVelocity[j] = (w * currP.velocity[j]) +
                        (c1 * r1 * (currP.bestPosition[j] - currP.position[j])) +
                        (c2 * r2 * (bestGlobalPosition[j] - currP.position[j]));

                    /*
                    * Если этот элемент выходит за диапазон, я возвращаю его в диапазон. 
                    * Здесь идея в том, что мне не нужны экстремальные значения для элемента скорости, 
                    * так как они могли бы вызвать выход новой позиции за границы.
                    */
                    if (newVelocity[j] < minV)
                        newVelocity[j] = minV;
                    else if (newVelocity[j] > maxV)
                        newVelocity[j] = maxV;
                }

                newVelocity.CopyTo(currP.velocity, 0); // новая скорость сохраняется в скорости частицы

                // Вычисление новой позиции, используя новую скорость
                for (int j = 0; j < currP.position.Length; ++j)
                {
                    newPosition[j] = currP.position[j] + newVelocity[j];

                    /*
                    * И вновь выполняется проверка на попадание в диапазон — на этот раз с каждым элементом новой позиции текущей частицы. 
                    * В каком-то смысле это избыточная проверка, так как я уже ограничил значение каждого элемента скорости, 
                    * но на мой взгляд она служит дополнительной гарантией.
                    */
                    if (newPosition[j] < minX)
                        newPosition[j] = minX;
                    else if (newPosition[j] > maxX)
                        newPosition[j] = maxX;
                }
                newPosition.CopyTo(currP.position, 0);

                // Вычисление добротности
                newFitness = ObjectiveFunction(newPosition);
                currP.fitness = newFitness;

                // Соответствует ли новая позиция лучшей известной для этой частицы?
                if (newFitness < currP.bestFitness)
                {
                    newPosition.CopyTo(currP.bestPosition, 0);
                    currP.bestFitness = newFitness;
                }

                // Является ли новая позиция лучшей глобальной позицией роя?
                if (newFitness < bestGlobalFitness)
                {
                    newPosition.CopyTo(bestGlobalPosition, 0);
                    bestGlobalFitness = newFitness;
                }

            } // each Particle

        }

        public void Drawing(DrawingContext dc, double width, double height, double Xmin, double Xmax, double Ymin, double Ymax)
        {
            for (int i = 0; i < numberParticles; i++)
            {
                var x = swarm[i].position[0];
                var y = swarm[i].position[1];
                Point norm = Tools.Normalize(new Point(x, y), width, height, Xmin, Xmax, Ymin, Ymax);
                dc.DrawEllipse(Brushes.WhiteSmoke, null, norm, 3, 3);
            }
        }
    }
}
