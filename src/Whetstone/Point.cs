using System;

namespace Whetstone
{
    public struct Point : IEquatable<Point>
    {
        public int x { get; set; }
        public int y { get; set; }

        public Point(int x_, int y_) : this()
        {
            x = x_;
            y = y_;
        }

        public Point(int value) : this()
        {
            x = value;
            y = value;
        }

        public static Point Zero { get; } = new Point();

        public static Point operator +(Point value1, Point value2)
        {
            return new Point(value1.x + value2.x, value1.y + value2.y);
        }

        public static Point operator -(Point value1, Point value2)
        {
            return new Point(value1.x - value2.x, value1.y - value2.y);
        }

        public static Point operator *(Point value1, Point value2)
        {
            return new Point(value1.x * value2.x, value1.y * value2.y);
        }

        public static Point operator /(Point source, Point divisor)
        {
            return new Point(source.x / divisor.x, source.y / divisor.y);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !a.Equals(b);
        }

        public static Point Add(Point value1, Point value2)
        {
            return value1 + value2;
        }

        public static Point Subtract(Point value1, Point value2)
        {
            return value1 - value2;
        }

        public static Point Multiply(Point value1, Point value2)
        {
            return value1 * value2;
        }

        public static Point Divide(Point source, Point divisor)
        {
            return new Point(source.x / divisor.x, source.y / divisor.y);
        }

        public override bool Equals(object obj)
        {
            return obj is Point && Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            return (x == other.x) && (y == other.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }

        public override string ToString()
        {
            return "{x:" + x + " y:" + y + "}";
        }

        public static float Distance(Point value1, Point value2)
        {
            float v1 = value1.x - value2.x, v2 = value1.y - value2.y;
            return (float)Math.Sqrt((v1 * v1) + (v2 * v2));
        }

        public static Point Negate(Point value)
        {
            value.x = -value.x;
            value.y = -value.y;
            return value;
        }
    }
}
