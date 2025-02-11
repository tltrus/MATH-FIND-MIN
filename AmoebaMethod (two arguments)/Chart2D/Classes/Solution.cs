using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Chart2D.Classes
{
    internal class Solution : IComparable<Solution>
    {
        // a potential solution (array of double) and associated value (so can be sorted against several potential solutions
        public double[] vector; // массив вектора с двойным именем, который содержит числовые значения решения
        public double value;    // значение целевой функции

        static Random random = new Random(1);  // to allow creation of random solutions

        // КОНСТРУКТОР 1. Создаем случайное решение
        public Solution(int dim, double minX, double maxX)
        {
            // a random Solution
            vector = new double[dim]; // для функции Розенброка dim = 2 (x и y)
            for (int i = 0; i < dim; ++i)
                vector[i] = (maxX - minX) * random.NextDouble() + minX; // создаем случайные переменные x и y
            value = AmoebaOptimization.ObjectiveFunction(vector, null);    // вычисляем значение (добротность)
        }

        // КОНСТРУКТОР 2. Создаем решение из указанного массива double
        public Solution(double[] vector)
        {
            // a specifiede solution
            this.vector = new double[vector.Length];
            Array.Copy(vector, this.vector, vector.Length);
            value = AmoebaOptimization.ObjectiveFunction(this.vector, null);
        }

        /*
         * Поскольку класс Solution является производным от интерфейса IComparable, 
         * класс должен реализовывать метод CompareTo. CompareTo определено так, ч
         * то объекты Solution будут автоматически отсортированы от лучших (меньших) к худшим (больших) значений целевой функции
         */
        public int CompareTo(Solution other) // based on vector/solution value
        {
            if (value < other.value)
                return -1;
            else if (value > other.value)
                return 1;
            else
                return 0;
        }

        // простой метод ToString, использующий конкатенацию строк
        public override string ToString()
        {
            string s = "[ ";
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] >= 0.0) s += " ";
                s += vector[i].ToString("F2") + " ";
            }
            s += "]  val = " + value.ToString("F4");
            return s;
        }
    }
}
