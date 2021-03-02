using System;
using Microsoft.Xna.Framework;

namespace Whetstone
{
    public class Cell : ICell
    {
        public Cell() { }

        public Cell(int x_, int y_, bool opaque_, bool collider_, bool explored_)
        {
            x = x_;
            y = y_;
            opaque = opaque_;
            collider = collider_;
            explored = explored_;
        }

        public int x { get; set; }
        public int y { get; set; }
        public bool opaque { get; set; }
        public bool collider { get; set; }
        public bool explored { get; set; }

        public Char character { get; set; }
        public Color color { get; set; }
        public Color bgColor { get; set; }
        public Color oosColor { get; set; }
        public Color oosBgColor { get; set; }

        public override string ToString()
        {
            if (collider)
            {
                if (opaque)
                {
                    return ".";
                }

                return "s";
            }

            if (opaque)
            {
                return "o";
            }

            return "#";
        }

        public bool Equals(ICell other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return x == other.x
                && y == other.y
                && opaque == other.opaque
                && collider == other.collider
                && explored == other.explored
                && character == other.character
                && color == other.color
                && bgColor == other.bgColor
                && oosColor == oosColor
                && oosBgColor == oosBgColor;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Cell)obj);
        }

        public static bool operator ==(Cell l, Cell r)
        {
            return Equals(l, r);
        }

        public static bool operator !=(Cell l, Cell r)
        {
            return !Equals(l, r);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = x;
                hash = (hash * 397) ^ y;
                hash = (hash * 397) ^ opaque.GetHashCode();
                hash = (hash * 397) ^ collider.GetHashCode();
                hash = (hash * 397) ^ explored.GetHashCode();
                hash = (hash * 397) ^ character.GetHashCode();
                hash = (hash * 397) ^ color.GetHashCode();
                hash = (hash * 397) ^ bgColor.GetHashCode();
                hash = (hash * 397) ^ oosColor.GetHashCode();
                hash = (hash * 397) ^ oosBgColor.GetHashCode();
                return hash;
            }
        }
    }
}