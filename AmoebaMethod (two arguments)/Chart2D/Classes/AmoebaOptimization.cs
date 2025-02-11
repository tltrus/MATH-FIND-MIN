
namespace _Chart2D.Classes
{
    internal class AmoebaOptimization
    {
        public delegate void IterationSolutionHandler(Solution[] sln);
        public event IterationSolutionHandler? IterationSolutionNotify;
        public delegate void bestSolutionHandler(string str);
        public event bestSolutionHandler? BestSolutionNotify;

        public int amoebaSize;  // number of solutions, например 3
        public int dim;         // vector-solution size, also problem dimension, например 2

        /*
         * Массив решений содержит потенциальные объекты решений. 
         * Хотя это не ясно из объявления, решения с массивами должны быть отсортированы всегда, 
         * от наилучшего решения (поле наименьшего значения) до худшего решения
         */
        public Solution[] solutions;  // potential solutions (vector + value)

        /*
         * Поля minX и maxX ограничивают начальные значения в каждом объекте Solution. 
         * Эти значения будут варьироваться от проблемы к проблеме.
         */
        public double minX;
        public double maxX;

        public double alpha;  // reflection
        public double beta;   // contraction
        public double gamma;  // expansion

        public int maxLoop;   // количество итераций главного цикла

        int t = 0;

        public AmoebaOptimization(int amoebaSize, int dim, double minX, double maxX, int maxLoop)
        {
            this.amoebaSize = amoebaSize;
            this.dim = dim; // dim = 2
            this.minX = minX;
            this.maxX = maxX;
            alpha = 1.0;  // hard-coded values from theory
            beta = 0.5;
            gamma = 2.0;

            this.maxLoop = maxLoop;

            solutions = new Solution[amoebaSize]; // amoebaSize = 3
            // создаются случайные решения
            for (int i = 0; i < solutions.Length; ++i)
                solutions[i] = new Solution(dim, minX, maxX);  // the Solution ctor calls the objective function to compute value

            Array.Sort(solutions); // сортирует решения от наилучшего значения к худшему
        }

        // Я использую фиктивный входной параметр с именем dataSource, чтобы указать, 
        // что в большинстве ситуаций целевая функция зависит от некоторого внешнего источника данных, 
        // такого как текстовый файл или таблица SQL
        public static double ObjectiveFunction(double[] vector, object dataSource)
        {
            // Rosenbrock's function, the function to be minimized
            // no data source needed here but real optimization problems will often be based on data
            double x = vector[0];
            double y = vector[1];
            return 100.0 * Math.Pow(y - x * x, 2) + Math.Pow(1 - x, 2);
        }

        /*
         * Ключевым аспектом алгоритма оптимизации амебы является то, 
         * что текущее худшее решение заменяется - если оно приводит к лучшему набору решений - 
         * на так называемую отраженную точку, расширенную точку или сжатую точку.
         */

        /*
         * Вспомогательный метод класса Amoeba Centroid создает объект Solution, 
         * который в некотором смысле является промежуточным решением между всеми решениями в амебе, 
         * за исключением худшего решения (худшее решение - это решение с наибольшим значением решения, 
         * поскольку цель состоит в том, чтобы минимизировать целевую функцию, 
         * и он будет расположен по индексу amoebaSize-1)
         */
        public Solution Centroid()
        {
            // return the centroid of all solution vectors except for the worst (highest index) vector
            double[] c = new double[dim];
            for (int i = 0; i < amoebaSize - 1; ++i)
                for (int j = 0; j < dim; ++j)
                    c[j] += solutions[i].vector[j];  // accumulate sum of each vector component

            for (int j = 0; j < dim; ++j)
                c[j] = c[j] / (amoebaSize - 1);

            Solution s = new Solution(c);  // feed vector to ctor which calls objective function to compute value
            return s;
        }

        /*
         * Вспомогательный метод Reflected создает объект Solution, который находится в общем направлении лучших решений. 
         * Постоянная альфа, обычно равная 1,0, определяет, как далеко от центра тяжести нужно двигаться, 
         * чтобы получить отраженный раствор. Большие значения альфа генерируют отраженные точки, 
         * которые находятся дальше от центроида
         */
        public Solution Reflected(Solution centroid)
        {
            // the reflected solution extends from the worst (lowest index) solution through the centroid
            double[] r = new double[dim];
            double[] worst = solutions[amoebaSize - 1].vector;  // convenience only
            for (int j = 0; j < dim; ++j)
                r[j] = (1 + alpha) * centroid.vector[j] - alpha * worst[j];
            Solution s = new Solution(r);
            return s;
        }

        /*
         * Вспомогательный метод Expanded создает объект Solution, который находится еще дальше от центроида, 
         * чем отраженный раствор. Постоянная гамма, обычно равная 2,0, контролирует, 
         * как далеко отраженная точка находится от центроида
         */
        public Solution Expanded(Solution reflected, Solution centroid)
        {
            // expanded extends even more, from centroid, thru reflected
            double[] e = new double[dim];
            for (int j = 0; j < dim; ++j)
                e[j] = gamma * reflected.vector[j] + (1 - gamma) * centroid.vector[j];
            Solution s = new Solution(e);
            return s;
        }

        /*
         * Вспомогательный метод Contracted создает объект Solution, 
         * который находится примерно между худшим решением и центроидом. Константа бета, обычно равная 0,50, 
         * контролирует, насколько близко к худшему решению точка контракта
         */
        public Solution Contracted(Solution centroid)
        {
            // contracted extends from worst (lowest index) towards centoid, but not past centroid
            double[] v = new double[dim];  // didn't want to reuse 'c' from centoid routine
            double[] worst = solutions[amoebaSize - 1].vector;  // convenience only
            for (int j = 0; j < dim; ++j)
                v[j] = beta * worst[j] + (1 - beta) * centroid.vector[j];
            Solution s = new Solution(v);
            return s;
        }

        /*
         * Если ни отраженная, ни расширенная, ни сжатая точка не дают лучшего набора решений, 
         * алгоритм амебы сокращает текущий набор решений. 
         * Каждая точка решения, за исключением лучшей точки с индексом 0, 
         * перемещается на полпути от ее текущего местоположения к лучшей точке
         */
        public void Shrink()
        {
            // move all vectors, except for the best vector (at index 0), halfway to the best vector
            // compute new objective function values and sort result
            for (int i = 1; i < amoebaSize; ++i)  // note we don't start at [0]
            {
                for (int j = 0; j < dim; ++j)
                {
                    solutions[i].vector[j] = (solutions[i].vector[j] + solutions[0].vector[j]) / 2.0;
                    solutions[i].value = ObjectiveFunction(solutions[i].vector, null);
                }
            }
            Array.Sort(solutions);
        }

        /*
         * Вспомогательный метод ReplaceWorst заменяет текущее худшее решение, расположенное по индексу amoebaSize-1, 
         * на другое решение (отраженная, расширенная или сжатая точка)
         */
        public void ReplaceWorst(Solution newSolution)
        {
            // replace the worst solution (at index size-1) with contents of parameter newSolution's vector
            for (int j = 0; j < dim; ++j)
                solutions[amoebaSize - 1].vector[j] = newSolution.vector[j];
            solutions[amoebaSize - 1].value = newSolution.value;
            Array.Sort(solutions);
        }

        /*
         * Вспомогательный метод IsWorseThanAllButWorst делает метод Solve немного более аккуратным. 
         * Помощник проверяет объект Solution и возвращает true, 
         * только если объект Solution (всегда отраженное решение в алгоритме) хуже (имеет большее значение целевой функции), 
         * чем все другие решения в амебе, за исключением, возможно, худшего решения (расположенного в индекс amoebaSize-1)
         */
        public bool IsWorseThanAllButWorst(Solution reflected)
        {
            // Solve needs to know if the reflected vector is worse (greater value) than every vector in the amoeba, except for the worst vector (highest index)
            for (int i = 0; i < amoebaSize - 1; ++i)  // not the highest index (worst)
            {
                if (reflected.value <= solutions[i].value)  // reflected is better (smaller value) than at least one of the non-worst solution vectors
                    return false;
            }
            return true;
        }

        // Основной метод
        public Solution Solve()
        {
            if (t < maxLoop)
            {
                ++t;

                if (t % 10 == 0)
                {
                    BestSolutionNotify?.Invoke("\rAt t = " + t + " curr best solution = " + solutions[0]);
                }
                IterationSolutionNotify?.Invoke(solutions);


                Solution centroid = Centroid();  // compute centroid
                Solution reflected = Reflected(centroid);  // compute reflected

                if (reflected.value < solutions[0].value)  // reflected is better than the curr best
                {
                    Solution expanded = Expanded(reflected, centroid);  // can we do even better??
                    if (expanded.value < solutions[0].value)  // winner! expanded is better than curr best
                        ReplaceWorst(expanded);  // replace curr worst solution with expanded
                    else
                        ReplaceWorst(reflected);  // it was worth a try . . . 
                    return null;
                }

                if (IsWorseThanAllButWorst(reflected) == true)  // reflected is worse (larger value) than all solution vectors (except possibly the worst one)
                {
                    if (reflected.value <= solutions[amoebaSize - 1].value)  // reflected is better (smaller) than the curr worst (last index) vector
                        ReplaceWorst(reflected);

                    Solution contracted = Contracted(centroid);  // compute a point 'inside' the amoeba

                    if (contracted.value > solutions[amoebaSize - 1].value)  // contracted is worse (larger value) than curr worst (last index) solution vector
                        Shrink();
                    else
                        ReplaceWorst(contracted);

                    return null;
                }

                ReplaceWorst(reflected);

            }  // solve loop

            if (t >= maxLoop)
                return solutions[0];  // best solution is always at [0]

            return null;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < solutions.Length; ++i)
                s += "[" + i + "] " + solutions[i].ToString() + Environment.NewLine;
            return s;
        }

    }
}
