using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _Chart2D
{
    internal class FireflyOptimization
    {
        public delegate void StopHandler();
        public event StopHandler? TimerNotify;
        public delegate void bestPositionHandler(double[] bestPosition);
        public event bestPositionHandler? BestPositionNotify;

        public int numFireflies = 40; // typically 15-40
        public int dim = 2;
        public int maxEpochs = 300;
        public int seed = 0;
        public int epoch;

        Random rnd;
        double minX, maxX, B0, g, a, bestError;
        int displayInterval;
        double[] bestPosition;
        public Firefly[] swarm;
        public double error, z;

        public FireflyOptimization() 
        {
            rnd = new Random(seed);
            minX = -2; // 0.0; // specific to Michalewicz function
            maxX = 3.2;

            B0 = 1.0;  // beta (attractiveness base)
                              //double betaMin = 0.20;
            g = 1.0;   // gamma (absorption for attraction)
            a = 0.20;    // alpha
                                //double a0 = 1.0;    // base alpha for decay
            displayInterval = maxEpochs / 10;

            bestError = double.MaxValue;
            bestPosition = new double[dim]; // best ever

            swarm = new Firefly[numFireflies]; // all null

            // initialize swarm at random positions
            for (int i = 0; i < numFireflies; ++i)
            {
                swarm[i] = new Firefly(dim); // position 0, error and intensity 0.0
                for (int k = 0; k < dim; ++k) // random position
                    swarm[i].position[k] = (maxX - minX) * rnd.NextDouble() + minX;
                swarm[i].error = Error(swarm[i].position); // associated error
                swarm[i].intensity = 1 / (swarm[i].error + 1); // +1 prevent div by 0
                if (swarm[i].error < bestError)
                {
                    bestError = swarm[i].error;
                    for (int k = 0; k < dim; ++k)
                        bestPosition[k] = swarm[i].position[k];
                }
            }

            epoch = 0;
        }

        public void Calculation()
        {
            // Starting firefly algorithm
            bestPosition = Solve(numFireflies, dim, seed, maxEpochs);

            // Error at best position = 
            error = Error(bestPosition);
        }
        
        double[] Solve(int numFireflies, int dim, int seed, int maxEpochs)
        {
            if (epoch < maxEpochs) // main processing
            {
                //if (bestError < errThresh) break; // are we good?
                if (epoch % displayInterval == 0 && epoch < maxEpochs) // show progress?
                {
                    string sEpoch = epoch.ToString().PadLeft(6);
                    Console.Write("epoch = " + sEpoch);
                    Console.WriteLine("   error = " + bestError.ToString("F14"));
                }

                for (int i = 0; i < numFireflies; ++i) // each firefly
                {
                    for (int j = 0; j < numFireflies; ++j) // each other firefly. weird!
                    {
                        if (swarm[i].intensity < swarm[j].intensity)
                        {
                            // curr firefly i is less intense (i is worse) so move i toward j
                            double r = Distance(swarm[i].position, swarm[j].position);
                            double beta = B0 * Math.Exp(-g * r * r); // original 
                                                                     //double beta = (B0 - betaMin) * Math.Exp(-g * r * r) + betaMin; // better
                                                                     //double a = a0 * Math.Pow(0.98, epoch); // better
                            for (int k = 0; k < dim; ++k)
                            {
                                swarm[i].position[k] += beta * (swarm[j].position[k] - swarm[i].position[k]);
                                swarm[i].position[k] += a * (rnd.NextDouble() - 0.5);
                                if (swarm[i].position[k] < minX) swarm[i].position[k] = (maxX - minX) * rnd.NextDouble() + minX;
                                if (swarm[i].position[k] > maxX) swarm[i].position[k] = (maxX - minX) * rnd.NextDouble() + minX;
                            }
                            swarm[i].error = Error(swarm[i].position);
                            swarm[i].intensity = 1 / (swarm[i].error + 1);
                        }
                    } // j
                } // i each firefly

                Array.Sort(swarm); // low error to high
                if (swarm[0].error < bestError) // new best?
                {
                    bestError = swarm[0].error;
                    for (int k = 0; k < dim; ++k)
                        bestPosition[k] = swarm[0].position[k];
                }
                ++epoch;
            }
            else
            {
                z = Michalewicz(bestPosition);
                error = Error(bestPosition);

                BestPositionNotify?.Invoke(bestPosition);
                TimerNotify?.Invoke();
            }

            return bestPosition;
        }

        double Distance(double[] posA, double[] posB)
        {
            double ssd = 0.0; // sum squared diffrences (Euclidean)
            for (int i = 0; i < posA.Length; ++i)
                ssd += (posA[i] - posB[i]) * (posA[i] - posB[i]);
            return Math.Sqrt(ssd);
        }

        public double Michalewicz(double[] xValues)
        {
            double result = 0.0;
            for (int i = 0; i < xValues.Length; ++i)
            {
                double a = Math.Sin(xValues[i]);
                double b = Math.Sin(((i + 1) * xValues[i] * xValues[i]) / Math.PI);
                double c = Math.Pow(b, 20);
                result += a * c;
            }
            return -1.0 * result;
        } // Michalewicz

        double Error(double[] xValues)
        {
            int dim = xValues.Length;
            double trueMin = 0.0;
            if (dim == 2)
                trueMin = -1.8013; // approx.
            else if (dim == 5)
                trueMin = -4.687658; // approx.
            else if (dim == 10)
                trueMin = -9.66015; // approx.
            double calculated = Michalewicz(xValues);
            return (trueMin - calculated) * (trueMin - calculated);
        }

        void ShowVector(double[] v, int dec, bool nl)
        {
            for (int i = 0; i < v.Length; ++i)
                Console.Write(v[i].ToString("F" + dec) + " ");
            if (nl == true)
                Console.WriteLine("");
        }

    }

}
