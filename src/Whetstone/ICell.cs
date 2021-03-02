using System;
using Microsoft.Xna.Framework;

namespace Whetstone
{
    public interface ICell : IEquatable<ICell>
    {
        int x { get; set; }
        int y { get; set; }
        bool opaque { get; set; }
        bool collider { get; set; }
        bool explored { get; set; }
        
        Char character { get; set; }
        Color color { get; set; }
        Color bgColor { get; set; }
        Color oosColor { get; set; }
        Color oosBgColor { get; set; }
    }
}