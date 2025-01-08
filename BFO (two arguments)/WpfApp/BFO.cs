using System.Windows;

namespace WpfApp
{
    // Test Run - Bacterial Foraging Optimization
    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2012/april/test-run-bacterial-foraging-optimization
    internal class BFO
    {
        public delegate void StopHandler();
        public event StopHandler? TimerNotify;
        public delegate void CountersHandler(int[] value);
        public event CountersHandler? ContersNotify;

        Random random = null;
        public Colony colony;
        public double[] bestPosition;
        double bestCost;

        int dim = 2; // это число измерений в задаче, x и y для фунции Растригина
        double minValue = -5.12; // предел для x и y
        double maxValue = 5.12; // предел для x и y
        int S = 100;    // количество бактерий
        int Nc = 20;    // хемотаксические шаги, счетчик, представляющий продолжительность жизни каждой бактерии
        int Ns = 5;     // максимальное количество раз, когда бактерия будет плавать в одном и том же направлении
        int Nre = 8;    // количество шагов воспроизведения, т.е. количество поколений бактерий
        int Ned = 4;    // число шагов разгона
        double Ped = 0.25;  // вероятность рассеивания конкретной бактерии
        double Ci = 0.05;   // базовая длина плавания для каждой бактерии. При плавании бактерии будут двигаться не более, чем Ci за один шаг

        int l, k, j;

        public BFO()
        {
            Init();
        }

        private void Init()
        {
            l = k = j = 0;

            random = new Random(0);

            // Initialize bacteria colony
            colony = new Colony(S, dim, minValue, maxValue);
            
            // Computing the cost value for each bacterium
            for (int i = 0; i < S; ++i)
            {
                double cost = Cost(colony.bacteria[i].position);
                colony.bacteria[i].cost = cost;
                colony.bacteria[i].prevCost = cost;
            }

            /*
             * Find best initial cost and position
             * После инициализации определяется лучшая бактерия в колонии, имея в виду, что более низкие затраты лучше, 
             * чем более высокие затраты в случае минимизации функции Растригина
             */
            bestCost = colony.bacteria[0].cost;
            int indexOfBest = 0;
            for (int i = 0; i < S; ++i)
            {
                if (colony.bacteria[i].cost < bestCost)
                {
                    bestCost = colony.bacteria[i].cost;
                    indexOfBest = i;
                }
            }
            bestPosition = new double[dim];
            colony.bacteria[indexOfBest].position.CopyTo(bestPosition, 0);
        }



        public void Calculate()
        {
            try
            {
                int t = 0;  // является счетчиком времени для отслеживания прогресса BFO

                // Цикл обрабатывает шаги разгона
                //for (int l = 0; l < Ned; ++l) // Eliminate-disperse loop
                if (l < Ned) //4
                {
                   
                    // Цикл обрабатывает этапы воспроизведения
                    //for (k = 0; k < Nre; ++k) // Reproduce-eliminate loop
                    if (k < Nre) //8
                    {
                        // Цикл обрабатывает хемотаксические этапы, т.е. продолжительность жизни каждой бактерии
                        //for (j = 0; j < Nc; ++j) // Chemotactic loop
                        if (j < Nc) //20
                        {
                            // Reset the health of each bacterium to 0.0
                            for (int i = 0; i < S; ++i)
                            {
                                /*
                                 * Внутри хемотаксической петли только что было создано новое поколение бактерий, 
                                 * поэтому здоровье каждой бактерии сбрасывается на 0
                                 */
                                colony.bacteria[i].health = 0.0;
                            }

                            for (int i = 0; i < S; ++i) // Each bacterium
                            {
                                /*
                                 * После сброса значений здоровья бактерий внутри хемотаксической петли каждая бактерия падает, 
                                 * чтобы определить новое направление, а затем движется в новом направлении. направление, вот так
                                 */
                                double[] tumble = new double[dim];
                                for (int p = 0; p < dim; ++p)
                                {
                                    // Сначала для каждого компонента текущей позиции бактерии генерируется случайное значение от -1,0 до +1,0
                                    tumble[p] = 2.0 * random.NextDouble() - 1.0;
                                }
                                double rootProduct = 0.0;
                                for (int p = 0; p < dim; ++p)
                                {
                                    // Затем вычисляется корневое произведение результирующего вектора
                                    rootProduct += (tumble[p] * tumble[p]);
                                }
                                for (int p = 0; p < dim; ++p)
                                {
                                    // И затем новое положение бактерии рассчитывается путем взятия старого положения и перемещения некоторой доли значения переменной Ci.
                                    colony.bacteria[i].position[p] += (Ci * tumble[p]) / rootProduct;
                                }

                                // update costs of new position
                                // После кувырка текущая бактерия обновляется, а затем бактерия проверяется, чтобы найти новое лучшее глобальное решение
                                colony.bacteria[i].prevCost = colony.bacteria[i].cost;
                                colony.bacteria[i].cost = Cost(colony.bacteria[i].position);
                                colony.bacteria[i].health += colony.bacteria[i].cost;
                                if (colony.bacteria[i].cost < bestCost)
                                {
                                    // New best solution found by bacteria i at time t
                                    bestCost = colony.bacteria[i].cost;
                                    colony.bacteria[i].position.CopyTo(bestPosition, 0);
                                }

                                // Затем бактерия входит в цикл плавания, где она будет плавать в том же направлении, 
                                // пока она улучшается, находя лучшее положение
                                int m = 0; // счетчик плавания, ограничивающий максимальное количество последовательных плаваний в одном и том же направлении значением в переменной Ns
                                while (m < Ns && colony.bacteria[i].cost < colony.bacteria[i].prevCost) // Ns - макс.количество плавания бактерией, = 5.
                                {
                                    ++m;
                                    for (int p = 0; p < dim; ++p)
                                    {
                                        colony.bacteria[i].position[p] += (Ci * tumble[p]) / rootProduct;
                                    }
                                    colony.bacteria[i].prevCost = colony.bacteria[i].cost;
                                    colony.bacteria[i].cost = Cost(colony.bacteria[i].position);
                                    if (colony.bacteria[i].cost < bestCost)
                                    {
                                        // New best solution found by bacteria i at time t
                                        bestCost = colony.bacteria[i].cost;
                                        colony.bacteria[i].position.CopyTo(bestPosition, 0);
                                    }
                                } // while improving
                            } // i, each bacterium

                            // После плавания счетчик времени увеличивается, и хемотаксическая петля завершается.
                            ++t;    // increment the time counter

                            j++;
                        }   // j, chemotactic loop
                        else
                        {
                            // Reproduce healthiest bacteria, eliminate other half
                            // В этот момент все бактерии прожили свою продолжительность жизни, данную Nc, и самая здоровая половина колонии выживет, 
                            // а наименее здоровая половина умрет:
                            Array.Sort(colony.bacteria);
                            for (int left = 0; left < S / 2; ++left)
                            {
                                /*
                                 * метод Array.Sort автоматически сортирует от наименьшего здоровья (чем меньше, тем лучше) к наибольшему здоровью, 
                                 * поэтому лучшие бактерии находятся в левых ячейках S / 2 массива колоний. 
                                 * Более слабая половина бактерий в правых клетках колонии эффективно погибает, 
                                 * копируя данные лучшей половины массива бактерий в более слабую правую половину. 
                                 * Обратите внимание, что это означает, что общее количество бактерий S должно быть числом, равномерно делимым на 2
                                 */
                                int right = left + S / 2;
                                colony.bacteria[left].position.CopyTo(colony.bacteria[right].position, 0);
                                colony.bacteria[right].cost = colony.bacteria[left].cost;
                                colony.bacteria[right].prevCost = colony.bacteria[left].prevCost;
                                colony.bacteria[right].health = colony.bacteria[left].health;
                            }

                            j = 0;
                            k++;
                        }

                    } // k, reproduction loop
                    else
                    {
                        k = 0;
                        l++;


                        // eliminate-disperse
                        // На этом этапе петли хемотаксиса и репродукции завершены, поэтому алгоритм BFO входит в фазу рассеивания:
                        for (int i = 0; i < S; ++i)
                        {
                            double prob = random.NextDouble();

                            /*
                             * Каждая бактерия исследована. Произвольное значение генерируется и сравнивается с переменной Ped, 
                             * чтобы определить, будет ли текущая бактерия перемещена в случайное место. Если бактерия на самом деле рассеяна, 
                             * она проверяется, не обнаружила ли она новую лучшую позицию в мире по чистой случайности.
                             */
                            if (prob < Ped)
                            {
                                for (int p = 0; p < dim; ++p)
                                {
                                    double x = (maxValue - minValue) *
                                      random.NextDouble() + minValue;
                                    colony.bacteria[i].position[p] = x;
                                }

                                double cost = Cost(colony.bacteria[i].position);
                                colony.bacteria[i].cost = cost;
                                colony.bacteria[i].prevCost = cost;
                                colony.bacteria[i].health = 0.0;

                                if (colony.bacteria[i].cost < bestCost)
                                {
                                    // New best solution found by bacteria i at time t
                                    bestCost = colony.bacteria[i].cost;
                                    colony.bacteria[i].position.CopyTo(bestPosition, 0);
                                }
                            } // if (prob < Ped)
                        } // for
                    }

                    //l++;
                } // l, elimination-dispersal loop
                else
                {
                    TimerNotify?.Invoke();
                }

                /*
                 * На этом этапе все циклы были выполнены, и алгоритм BFO отображает лучшее решение, 
                 * найденное с использованием определяемого программой вспомогательного метода с именем ShowVector:
                 */

                // Send to event l, k, j counters
                int[] counters = { l, k, j };
                ContersNotify?.Invoke(counters);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal: " + ex.Message);
            }
        }

        // Целевая функция
        private double Cost(double[] position)
        {
            double result = 0.0;
            for (int i = 0; i < position.Length; ++i)
            {
                double xi = position[i];
                result += (xi * xi) - (10 * Math.Cos(2 * Math.PI * xi)) + 10;
            }
            return result;
        }

        public class Colony // Collection of Bacterium
        {
            public Bacterium[] bacteria;

            // Конструктор Colony создает коллекцию Bacterium, 
            // в которой каждому Bacterium назначается случайная позиция путем вызова конструктора Bacterium
            public Colony(int S, int dim, double minValue, double maxValue)
            {
                this.bacteria = new Bacterium[S];
                for (int i = 0; i < S; ++i)
                    bacteria[i] = new Bacterium(dim, minValue, maxValue);
            }

            public override string ToString()
            {
                return "";
            }

            // ************************************************************************************

            public class Bacterium : IComparable<Bacterium>
            {
                public double[] position;
                public double cost;     // это стоимость, связанная с позицией
                public double prevCost; // это стоимость, связанная с предыдущим положением бактерии
                public double health;   // сумма накопленных затрат бактерии за время жизни бактерии. Поскольку цель состоит в том, чтобы минимизировать затраты, маленькие значения здоровья лучше, чем большие.
                static Random random = new Random(0);

                // Конструктор Bacterium инициализирует объект Bacterium в случайном положении
                public Bacterium(int dim, double minValue, double maxValue)
                {
                    this.position = new double[dim];
                    for (int p = 0; p < dim; ++p)
                    {
                        double x = (maxValue - minValue) * random.NextDouble() + minValue;
                        this.position[p] = x;
                    }
                    this.health = 0.0;
                }

                public override string ToString()
                {
                    return "";
                }

                // Метод CompareTo упорядочивает объекты Bacterium от наименьшего здоровья до самого большого здоровья
                public int CompareTo(Bacterium other)
                {
                    if (this.health < other.health)
                        return -1;
                    else if (this.health > other.health)
                        return 1;
                    else
                        return 0;
                }
            }
        }

    }
}
