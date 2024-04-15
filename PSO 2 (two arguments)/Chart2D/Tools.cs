
namespace _Chart2D
{
    internal class Tools
    {
        public static double Constrain(double n, double low, double high) => Math.Max(Math.Min(n, high), low);

        public static double Map(double n, double start1, double stop1, double start2, double stop2, bool withinBounds = false)
        {
            var newval = (n - start1) / (stop1 - start1) * (stop2 - start2) + start2;
            if (!withinBounds)
            {
                return newval;
            }
            if (start2 < stop2)
            {
                return Constrain(newval, start2, stop2);
            }
            else
            {
                return Constrain(newval, stop2, start2);
            }
        }
    }
}
