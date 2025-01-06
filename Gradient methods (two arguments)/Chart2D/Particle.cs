using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Chart2D
{
    /*
     * У каждой частицы есть текущая скорость, которая отражает ее абсолютную величину (magnitude) и направление к новому, 
     * предположительно лучшему, решению/позиции. У частицы также имеются мера добротности ее текущей позиции, 
     * лучшая известная позиция (т. е. предыдущая позиция с лучшей известной добротностью) и добротность лучшей известной позиции
     */
    public class Particle
    {
        public double[] position;   // положение эквивалентно решению задачи
        public double fitness;      // мера того, насколько хорошим является решение, представленное позицией
        public double[] velocity;   // представляет информацию, необходимую для обновления текущей позиции/решения частицы

        public double[] bestPosition; // лучшая позиция частицы
        public double bestFitness;  // добротность лучшей позиции

        // Конструктор просто копирует значение каждого параметра в соответствующее поле данных
        public Particle(double[] position, double fitness, double[] velocity, double[] bestPosition, double bestFitness)
        {
            this.position = new double[position.Length];
            position.CopyTo(this.position, 0);
            this.fitness = fitness;
            this.velocity = new double[velocity.Length];
            velocity.CopyTo(this.velocity, 0);
            this.bestPosition = new double[bestPosition.Length];
            bestPosition.CopyTo(this.bestPosition, 0);
            this.bestFitness = bestFitness;
        }

        public override string ToString()
        {
            string s = "";
            s += "==========================\n";
            s += "Position: ";
            for (int i = 0; i < this.position.Length; ++i)
                s += this.position[i].ToString("F2") + " ";
            s += "\n";
            s += "Fitness = " + this.fitness.ToString("F4") + "\n";
            s += "Velocity: ";
            for (int i = 0; i < this.velocity.Length; ++i)
                s += this.velocity[i].ToString("F2") + " ";
            s += "\n";
            s += "Best Position: ";
            for (int i = 0; i < this.bestPosition.Length; ++i)
                s += this.bestPosition[i].ToString("F2") + " ";
            s += "\n";
            s += "Best Fitness = " + this.bestFitness.ToString("F4") + "\n";
            s += "==========================\n";
            return s;
        }

    } // class Particle

}
