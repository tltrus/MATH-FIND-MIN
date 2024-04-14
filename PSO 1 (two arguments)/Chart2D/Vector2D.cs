
namespace _Chart2D
{
    internal class Vector2D
    {
        public double x;
        public double y;

        public Vector2D(double x = 0, double y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2) => new Vector2D(v1.x + v2.x, v1.y + v2.y);
        public static Vector2D operator -(Vector2D v1, Vector2D v2) => new Vector2D(v1.x - v2.x, v1.y - v2.y);
        public static Vector2D operator *(double m, Vector2D v) => new Vector2D(v.x * m, v.y * m);
        public override string ToString() => $"[ {Math.Round(x, 2)} , {Math.Round(y, 2)} ]";
    }
}
