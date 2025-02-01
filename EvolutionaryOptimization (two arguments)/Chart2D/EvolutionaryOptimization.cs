using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _Chart2D
{
    internal class EvolutionaryOptimization
    {
        public delegate void bestSolutionHandler(double[] value);
        public event bestSolutionHandler? BestSolutionNotify;
        public delegate void generationHandler(int value);
        public event generationHandler? GenerationNotify;

        public Evolver ev { get; private set; }

        int popSize = 100;
        int numGenes = 2;
        double minGene = -500.0;
        double maxGene = 500.0;
        double mutateRate;
        double precision = 0.0001;             // controls mutation magnitude
        double tau = 0.40;                     // tournament selection factor
        public int maxGeneration = 2000;

        public EvolutionaryOptimization()
        {
            mutateRate = 1.0 / numGenes;

            // Evolutionary Optimization demo
            // Goal is to find the (x,y) that minimizes Schwefel's function
            // f(x,y) = (-x * sin(sqrt(abs(x)))) + (-y * sin(sqrt(abs(y))))
            // Known solution is x = y = 420.9687 when f = -837.9658

            ev = new Evolver(popSize, numGenes, minGene, maxGene, mutateRate, precision, tau, maxGeneration); // assumes existence of a Problem.Fitness method

            Calculate();
        }

        public void Calculate()
        {
            try
            {
                double[] best = ev.Evolve();

                BestSolutionNotify?.Invoke(best);
                GenerationNotify?.Invoke(ev.gen);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal: " + ex.Message);
                Console.ReadLine();
            }
        }

        // Содержит большую часть логики алгоритма
        public class Evolver
        {
            public int popSize; // количество людей в популяции. Большие значения popSize увеличивают точность алгоритма за счет скорости
            public Individual[] population;

            public int numGenes;
            public double minGene;
            public double maxGene;
            public double mutateRate;  // used by Mutate
            public double precision;   // used by Mutate

            public double tau;         // используются методом Select
            private int[] indexes;      // используется методом Select, который выбирает двух родителей

            public int maxGeneration;
            private static Random rnd = null;
            public int gen = 1;

            /*
             * Конструктор выделяет память для массива популяции, а затем использует конструктор Individual, 
             * чтобы заполнить массив индивидуумами, которые имеют случайные значения генов.
             */
            public Evolver(int popSize, int numGenes, double minGene, double maxGene, double mutateRate, double precision, double tau, int maxGeneration)
            {
                this.popSize = popSize;
                this.population = new Individual[popSize];
                for (int i = 0; i < population.Length; ++i)
                    population[i] = new Individual(numGenes, minGene, maxGene, mutateRate, precision);

                this.numGenes = numGenes;
                this.minGene = minGene;
                this.maxGene = maxGene;
                this.mutateRate = mutateRate;
                this.precision = precision;
                this.tau = tau;

                this.indexes = new int[popSize];
                for (int i = 0; i < indexes.Length; ++i)
                    this.indexes[i] = i;
                this.maxGeneration = maxGeneration;
                rnd = new Random(0);
            }

            // Метод Evolve возвращает лучшее решение, найденное в виде массива типа double.
            public double[]? Evolve()
            {
                /*
                 * Метод Evolve начинается с инициализации лучшей пригодности и лучших хромосом первыми в популяции. 
                 * Метод итерирует в точности времена maxGenerations, используя gen (generation) в качестве счетчика цикла
                 */
                double bestFitness = this.population[0].fitness;
                double[] bestChomosome = new double[numGenes];
                population[0].chromosome.CopyTo(bestChomosome, 0);

                //while (gen < maxGeneration)
                if (gen < maxGeneration)
                {
                    /*
                     * Метод Select возвращает двух хороших, но не обязательно лучших людей из населения. 
                     * Эти два родителя передаются в Reproduce, которая создает и возвращает двух детей. 
                     */
                    Individual[] parents = Select(2);
                    Individual[] children = Reproduce(parents[0], parents[1]); // crossover & mutation
                                                                               // Метод Accept помещает двух детей в популяцию, заменяя двух существующих людей.
                    Accept(children[0], children[1]);
                    // Метод Иммиграции генерирует нового случайного человека и помещает его в популяцию
                    Immigrate();

                    // Новое население затем сканируется, чтобы увидеть, является ли какой-либо из трех новых людей в популяции новым лучшим решением
                    for (int i = popSize - 3; i < popSize; ++i)
                    {
                        if (population[i].fitness < bestFitness)
                        {
                            bestFitness = population[i].fitness;
                            population[i].chromosome.CopyTo(bestChomosome, 0);
                        }
                    }
                    ++gen;
                }
                else
                {
                    gen = 1;
                    return null;
                }

                return bestChomosome;
            }

            /*
             * Метод принимает число хороших людей для выбора и возвращает их в массиве типа Individual
             * Метод Select использует технику, называемую ВЫБОРОМ ТУРНИРА. Генерируется подмножество случайных кандидатов, 
             * и возвращаются лучшие n из них. Количество кандидатов вычисляется в переменную tournSize, 
             * которая является некоторой долей, tau, от общей численности населения. 
             * Большие значения tau увеличивают вероятность выбора двух лучших людей
             */
            private Individual[] Select(int n) // select n 'good' Individuals
            {
                //if (n > popSize)
                //  throw new Exception("xxxx");

                int tournSize = (int)(tau * popSize); // tau = 0.40; popSize = 100 =>> tournSize = 40
                if (tournSize < n) tournSize = n;
                Individual[] candidates = new Individual[tournSize]; // candidates - массив из 40 элементов

                // Вспомогательный метод ShuffleIndexes перемешивает массив indexes в случайном порядке
                ShuffleIndexes();
                for (int i = 0; i < tournSize; ++i)
                    candidates[i] = population[indexes[i]]; // формируется массив кандидатов из перемешанной популяции
                Array.Sort(candidates); // сортировка кандидатов от минимальной (наилучшей) пригодности до наибольшей

                Individual[] results = new Individual[n];
                for (int i = 0; i < n; ++i)
                    results[i] = candidates[i]; // выбор лучших n кандидатов возвращаются

                return results;
            }

            // Вспомогательный метод для Select
            // Переупорядочивает значения в индексах массива в случайном порядке алгоритмом Фишера-Йейтса
            private void ShuffleIndexes()
            {
                for (int i = 0; i < this.indexes.Length; ++i)
                {
                    int r = rnd.Next(i, indexes.Length);
                    int tmp = indexes[r]; indexes[r] = indexes[i]; indexes[i] = tmp;
                }
            }

            //public override string ToString()
            //{
            //  string s = "";
            //  for (int i = 0; i < this.population.Length; ++i)
            //    s += i + ": " + this.population[i].ToString() + Environment.NewLine;
            //  return s;
            //}


            // Метод репродукции
            private Individual[] Reproduce(Individual parent1, Individual parent2) // crossover and mutation
            {
                // Метод начинается с генерации случайной точки пересечения cross
                int cross = rnd.Next(0, numGenes - 1); // crossover point. 0 means 'between 0 and 1'. numGenes = 2

                Individual child1 = new Individual(numGenes, minGene, maxGene, mutateRate, precision); // random chromosome
                Individual child2 = new Individual(numGenes, minGene, maxGene, mutateRate, precision);

                /*
                 * Child1 создается из левой части parent1 и правой части parent2. 
                 * Child2 создается из левой части parent2 и правой части parent1
                 */
                for (int i = 0; i <= cross; ++i)
                    child1.chromosome[i] = parent1.chromosome[i];
                for (int i = cross + 1; i < numGenes; ++i)
                    child2.chromosome[i] = parent1.chromosome[i];
                for (int i = 0; i <= cross; ++i)
                    child2.chromosome[i] = parent2.chromosome[i];
                for (int i = cross + 1; i < numGenes; ++i)
                    child1.chromosome[i] = parent2.chromosome[i];

                child1.Mutate();
                child2.Mutate();

                child1.fitness = Problem.Fitness(child1.chromosome);
                child2.fitness = Problem.Fitness(child2.chromosome);

                Individual[] result = new Individual[2];
                result[0] = child1;
                result[1] = child2;

                return result;
            } // Reproduce


            /* 
             * Метод Accept помещает двух дочерних индивидуумов, созданных Reproduce, в популяцию
             * Массив популяции отсортирован по пригодности, которая помещает двух худших людей в последние две ячейки массива, 
             * где они затем заменяются детьми.
            */
            private void Accept(Individual child1, Individual child2)
            {
                // place child1 and chil2 into the population, replacing two worst individuals
                Array.Sort(this.population);
                population[popSize - 1] = child1;
                population[popSize - 2] = child2;
                return;
            }

            /*
             * Метод Immigrate генерирует нового случайного индивида и помещает его в популяцию чуть выше местоположения 
             * двух только что сгенерированных детей (иммиграция помогает предотвратить застревание эволюционных алгоритмов 
             * в локальных минимальных решениях)
             */
            private void Immigrate()
            {
                Individual immigrant = new Individual(numGenes, minGene, maxGene, mutateRate, precision);
                population[popSize - 3] = immigrant; // replace third worst individual
            }

        } // class Evolver

        // Возможное решение проблемы минимизации
        public class Individual : IComparable<Individual>
        {
            /*
             * Обратите внимание, что хромосома - это массив двойных, а не массив с некоторой формой представления битов, 
             * обычно используемой генетическими алгоритмами. 
             * Алгоритмы эволюционной оптимизации иногда называют действительными генетическими алгоритмами.
             */
            public double[] chromosome; // возможное решение целевой проблемы
            public double fitness; // Пригодность. Чем меньше - тем лучше

            private int numGenes; // для функции Швефеля = 2. Число значений в возможном решении

            /*
             * При многих проблемах числовой оптимизации можно указать минимальное и максимальное значения для каждого гена, 
             * и эти значения сохраняются в minGene и maxGene. Если эти значения не известны, 
             * minGene и maxGene могут быть установлены в double.MinValue и double.MaxValue
             */
            private double minGene;
            private double maxGene;

            private double mutateRate;
            private double precision;

            static Random rnd = new Random(0);

            /*
             * Конструктор выделяет память для массива хромосом и назначает случайные значения в диапазоне (minGene, maxGene) для каждой генной клетки. 
             * Обратите внимание, что значение поля пригодности устанавливается путем вызова внешнего метода Fitness. 
             * В качестве альтернативы вы можете передать в конструктор ссылку на метод Fitness через делегат
             */
            public Individual(int numGenes, double minGene, double maxGene, double mutateRate, double precision)
            {
                this.numGenes = numGenes;
                this.minGene = minGene;
                this.maxGene = maxGene;
                this.mutateRate = mutateRate;
                this.precision = precision;
                this.chromosome = new double[numGenes]; // numGenes = 2
                for (int i = 0; i < this.chromosome.Length; ++i)
                    this.chromosome[i] = (maxGene - minGene) * rnd.NextDouble() + minGene; // Создаем две хромосомы
                this.fitness = Problem.Fitness(chromosome); // Вычисляем пригодность
            }

            /*
             * В примере на рисунке 2точность установлена на 0,0001, а maxGene установлена на 500. 
             * Максимально возможное значение для мутации гена составляет 0,0001 * 500 = 0,05, что означает, 
             * что если ген мутирован, его новым значением будет старое значение плюс или минус случайное значение от -0.05 до +0.05. 
             * Обратите внимание, что значение точности соответствует числу десятичных знаков в решении; 
             * это разумная эвристика для использования в качестве значения точности. Значение мутации контролирует, сколько генов в хромосоме будет изменено. 
            */
            public void Mutate()
            {
                double hi = precision * maxGene;
                double lo = -hi;
                for (int i = 0; i < chromosome.Length; ++i)
                {
                    if (rnd.NextDouble() < mutateRate) // mutateRate = 1.0 / numGenes;
                        chromosome[i] += (hi - lo) * rnd.NextDouble() + lo;
                }
            }

            //public override string ToString()
            //{
            //  string s = "";
            //  for (int i = 0; i < chromosome.Length; ++i)
            //    s += chromosome[i].ToString("F2") + " ";
            //  if (this.fitness == double.MaxValue)
            //    s += "| fitness = maxValue";
            //  else
            //    s += "| fitness = " + this.fitness.ToString("F4");
            //  return s;
            //}

            /*
             * Метод CompareTo определяет порядок сортировки по умолчанию для отдельных объектов, 
             * в данном случае от наименьшего соответствия (лучшего) до наибольшего соответствия.
             */
            public int CompareTo(Individual other) // from smallest fitness (better) to largest
            {
                if (this.fitness < other.fitness) return -1;
                else if (this.fitness > other.fitness) return 1;
                else return 0;
            }

        } // class Individual

        // Функция для минимизации
        public class Problem
        {
            public static double Fitness(double[] chromosome) // the 'cost' function we are trying to minimize
            {
                // Schwefel's function.
                // for n=2, solution is x = y = 420.9687 when f(x,y) = -837.9658
                double result = 0.0;
                for (int i = 0; i < chromosome.Length; ++i)
                    result += (-1.0 * chromosome[i]) * Math.Sin(Math.Sqrt(Math.Abs(chromosome[i])));
                return result;
            }
        }
    }
}
