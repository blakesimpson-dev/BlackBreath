using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SConsole = SadConsole.Consoles.Console;

namespace Whetstone
{
    public interface IMap : IMap<Cell> { }

    public interface IMap<TCell> where TCell : ICell
    {
        TCell this[int x, int y] { get; set; }
        
        Colors mapColors { get; }
        string palettePath { get; }   
        int width { get; }
        int height { get; }

        void Initialize(int width, int height, string palettePath);
        bool IsOpaque(int x, int y);
        bool IsCollider(int x, int y);
        bool IsExplored(int x, int y);
        void SetCellProperties(int x, int y, bool opaque, bool collider, bool explored);
        void SetCellRenderProperties(int x, int y, Char character, Color color, Color bgColor, Color oosColor, Color oosBgColor);
        void Clear();
        void Clear(bool opaque, bool collider, bool explored);
        void Render(SConsole console, FOV fov);
        TMap Clone<TMap>() where TMap : IMap<TCell>, new();
        void Copy(IMap<TCell> source);
        void Copy(IMap<TCell> sourceMap, int left, int top);
        IEnumerable<TCell> GetAllCells();
        IEnumerable<TCell> GetCellsAlongLine(int xOrigin, int yOrigin, int xDestination, int yDestination);
        IEnumerable<TCell> GetCellsInCircle(int xCenter, int yCenter, int radius);
        IEnumerable<TCell> GetBorderCellsInCircle(int xCenter, int yCenter, int radius);
        IEnumerable<TCell> GetCellsInDiamond(int xCenter, int yCenter, int distance);
        IEnumerable<TCell> GetBorderCellsInDiamond(int xCenter, int yCenter, int distance);
        IEnumerable<TCell> GetCellsInSquare(int xCenter, int yCenter, int distance);
        IEnumerable<TCell> GetBorderCellsInSquare(int xCenter, int yCenter, int distance);
        IEnumerable<TCell> GetCellsInRectangle(int top, int left, int width, int height);
        // IEnumerable<TCell> GetBorderCellsInRectangle(int top, int left, int width, int height);
        IEnumerable<TCell> GetCellsInRows(params int[] rows);
        IEnumerable<TCell> GetCellsInColumns(params int[] columns);
        IEnumerable<TCell> GetAdjacentCells(int xCenter, int yCenter);
        IEnumerable<TCell> GetAdjacentCells(int xCenter, int yCenter, bool includeDiagonals);
        TCell GetCell(int x, int y);
        string ToString();
        MapState Save();
        void Restore(MapState state);
        TCell CellFor(int index);
        int IndexFor(int x, int y);
        int IndexFor(TCell cell);
    }

    public class MapState
    {
        [Flags]
        public enum CellProperties
        {
            None = 0,
            Opaque = 1,
            Collider = 2,
            Explored = 3
        }

        public int width { get; set; }
        public int height { get; set; }
        public string palettePath { get; set; }
        public CellProperties[] Cells { get; set; }
    }
}