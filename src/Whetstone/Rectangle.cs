using System;

namespace Whetstone
{
    public struct Rectangle : IEquatable<Rectangle>
    {

        public Rectangle(int x_, int y_, int width_, int height_) : this()
        {
            x = x_;
            y = y_;
            width = width_;
            height = height_;
        }

        public Rectangle(Point location, Point size) : this()
        {
            x = location.x;
            y = location.y;
            width = size.x;
            height = size.y;
        }

        public int width { get; set; }
        public int height { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public static Rectangle Empty { get; } = new Rectangle();

        public int left => x;
        public int right => x + width;
        public int top => y;
        public int bottom => y + height;

        public Point location
        {
            get => new Point(x, y);
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        public Point center => new Point(x + (width / 2), y + (height / 2));
        public bool isEmpty => width == 0 && height == 0 && x == 0 && y == 0;

        public bool Equals(Rectangle other)
        {
            return this == other;
        }

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return (a.x == b.x) && (a.y == b.y) && (a.width == b.width) && (a.height == b.height);
        }

        public bool Contains(int x_, int y_)
        {
            return (x <= x_) && (x_ < (x + width)) && (y <= y_) && (y_ < (y + height));
        }

        public bool Contains(Point value)
        {
            return (x <= value.x) && (value.x < (x + width)) && (y <= value.y) && (value.y < (y + height));
        }

        public bool Contains(Rectangle value)
        {
            return (x <= value.x) && ((value.x + value.width) <= (x + width)) && (y <= value.y)
                && ((value.y + value.height) <= (y + height));
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !(a == b);
        }

        public void Offset(Point offsetPoint)
        {
            x += offsetPoint.x;
            y += offsetPoint.y;
        }

        public void Offset(int offsetX, int offsetY)
        {
            x += offsetX;
            y += offsetY;
        }

        public void Inflate(int horizontalValue, int verticalValue)
        {
            x -= horizontalValue;
            y -= verticalValue;
            width += horizontalValue * 2;
            height += verticalValue * 2;
        }

        public override bool Equals(object obj)
        {
            return (obj is Rectangle) && this == ((Rectangle)obj);
        }

        public override string ToString()
        {
            return $"{{X:{x} Y:{y} Width:{width} Height:{height}}}";
        }

        public override int GetHashCode()
        {
            return x ^ y ^ width ^ height;
        }

        public bool Intersects(Rectangle value)
        {
            return value.left < right && left < value.right && value.top < bottom && top < value.bottom;
        }

        public void Intersects(ref Rectangle value, out bool result)
        {
            result = value.left < right && left < value.right && value.top < bottom && top < value.bottom;
        }

        public static Rectangle Intersect(Rectangle value1, Rectangle value2)
        {
            Intersect(ref value1, ref value2, out Rectangle rectangle);
            return rectangle;
        }

        public static void Intersect(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
        {
            if (value1.Intersects(value2))
            {
                int rightSide = Math.Min(value1.x + value1.width, value2.x + value2.width);
                int leftSide = Math.Max(value1.x, value2.x);
                int topSide = Math.Max(value1.y, value2.y);
                int bottomSide = Math.Min(value1.y + value1.height, value2.y + value2.height);
                result = new Rectangle(leftSide, topSide, rightSide - leftSide, bottomSide - topSide);
            }
            else
            {
                result = new Rectangle(0, 0, 0, 0);
            }
        }

        public static Rectangle Union(Rectangle value1, Rectangle value2)
        {
            int x = Math.Min(value1.x, value2.x);
            int y = Math.Min(value1.y, value2.y);
            return new Rectangle(x, y, Math.Max(value1.right, value2.right) - x, Math.Max(value1.bottom, value2.bottom) - y);
        }

        public static void Union(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
        {
            result = Union(value1, value2);
        }
    }
}