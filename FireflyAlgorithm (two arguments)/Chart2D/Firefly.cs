using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Chart2D
{
    internal class Firefly : IComparable<Firefly>
    {
        public double[] position;
        public double error;
        public double intensity;

        public Firefly(int dim)
        {
            position = new double[dim];
            error = 0.0;
            intensity = 0.0;
        }

        public int CompareTo(Firefly other)
        {
            // allow auto sort low error to high
            if (error < other.error)
                return -1;
            else if (error > other.error)
                return +1;
            else
                return 0;
        }
    } // class Firefly
}
