using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SConsole = SadConsole.Consoles.Console;
using Whetstone.MapGeneration;

namespace Whetstone
{
    public class Map : Map<Cell>, IMap
    {
        public Map() { }

        public Map(int width, int height, string palettePath) : base(width, height, palettePath) { }

        public static TMap Generate<TMap>(IMapGenerationProcess<TMap> mapGenerationProcess) where TMap : IMap<Cell>
        {
            if (mapGenerationProcess == null)
            {
                throw new ArgumentNullException(nameof(mapGenerationProcess), "mapGenerationProcess cannot be null");
            }

            return mapGenerationProcess.GenerateMap();
        }

        public static TMap Generate<TMap, TCell>(IMapGenerationProcess<TMap, TCell> mapGenerationProcess) where TMap : IMap<TCell> where TCell : ICell
        {
            if (mapGenerationProcess == null)
            {
                throw new ArgumentNullException(nameof(mapGenerationProcess), "mapGenerationProcess cannot be null");
            }

            return mapGenerationProcess.GenerateMap();
        }
    }

    public class Map<TCell> : IMap<TCell> where TCell : ICell
    {
        private TCell[,] _cells;

        public Map() { }

        public Map(int width, int height, string palettePath)
        {
            Init(width, height, palettePath);
        }

        public TCell this[int x, int y]
        {
            get => _cells[x, y];
            set => _cells[x, y] = value;
        }

        public Colors mapColors { get; private set; }
        public string palettePath { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        public virtual void Initialize(int width, int height, string palettePath)
        {
            Init(width, height, palettePath);
        }

        private void Init(int width_, int height_, string palettePath_)
        {
            width = width_;
            height = height_;
            palettePath = palettePath_;
            mapColors = new Colors(palettePath);

            _cells = new TCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _cells[x, y] = Activator.CreateInstance<TCell>();
                    _cells[x, y].x = x;
                    _cells[x, y].y = y;
                }
            }
        }

        public bool IsOpaque(int x, int y)
        {
            return _cells[x, y].opaque;
        }

        public bool IsCollider(int x, int y)
        {
            return _cells[x, y].collider;
        }

        public bool IsExplored(int x, int y)
        {
            return _cells[x, y].explored;
        }

        public void SetCellProperties(int x, int y, bool opaque, bool collider, bool explored)
        {
            _cells[x, y].opaque = opaque;
            _cells[x, y].collider = collider;
            _cells[x, y].explored = explored;
        }

        public void SetCellRenderProperties(int x, int y, Char character, 
            Color color, Color bgColor, Color oosColor, Color oosBgColor)
        {
            _cells[x, y].character = character;
            _cells[x, y].color = color;
            _cells[x, y].bgColor = bgColor;
            _cells[x, y].oosColor = oosColor;
            _cells[x, y].oosBgColor = oosBgColor;

        }

        public void Clear()
        {
            Clear(true, false, false);
        }

        public void Clear(bool opaque, bool collider, bool explored)
        {
            foreach (TCell cell in GetAllCells())
            {
                SetCellProperties(cell.x, cell.y, opaque, collider, explored);
            }
        }

        public void Render(SConsole console, FOV fov)
        {
            foreach (TCell cell in GetAllCells())
            {
                if (!cell.explored)
                {
                    continue;
                }

                if (fov.IsInFov(cell.x, cell.y))
                {
                    console.CellData.SetCharacter(cell.x, cell.y, cell.character, cell.color, cell.bgColor);
                }
                else
                {
                    console.CellData.SetCharacter(cell.x, cell.y, cell.character, cell.oosColor, cell.oosBgColor);
                }
            }
        }        

        public virtual TMap Clone<TMap>() where TMap : IMap<TCell>, new()
        {
            var map = new TMap();
            map.Initialize(width, height, palettePath);
            map.Clear(true, false, false);

            foreach (TCell cell in GetAllCells())
            {
                map.SetCellProperties(cell.x, cell.y, cell.opaque, cell.collider, cell.explored);
            }

            return map;
        }

        public void Copy(IMap<TCell> source)
        {
            Copy(source, 0, 0);
        }

        public void Copy(IMap<TCell> source, int left, int top)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Source map cannot be null");
            }

            if (source.width + left > width)
            {
                throw new ArgumentException("Source map 'width' + 'left' cannot be larger than the destination map width");
            }

            if (source.height + top > height)
            {
                throw new ArgumentException("Source map 'height' + 'top' cannot be larger than the destination map height");
            }

            foreach (TCell cell in source.GetAllCells())
            {
                SetCellProperties(cell.x + left, cell.y + top, cell.opaque, cell.collider, cell.explored);
            }
        }

        public IEnumerable<TCell> GetAllCells()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return GetCell(x, y);
                }
            }
        }

        public IEnumerable<TCell> GetCellsAlongLine(int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            xOrigin = ClampX(xOrigin);
            yOrigin = ClampY(yOrigin);
            xDestination = ClampX(xDestination);
            yDestination = ClampY(yDestination);

            int dx = Math.Abs(xDestination - xOrigin);
            int dy = Math.Abs(yDestination - yOrigin);

            int sx = xOrigin < xDestination ? 1 : -1;
            int sy = yOrigin < yDestination ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                yield return GetCell(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination)
                {
                    break;
                }
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    xOrigin = xOrigin + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    yOrigin = yOrigin + sy;
                }
            }
        }

        private int ClampX(int x)
        {
            return (x < 0) ? 0 : (x > width - 1) ? width - 1 : x;
        }

        private int ClampY(int y)
        {
            return (y < 0) ? 0 : (y > height - 1) ? height - 1 : y;
        }

        public IEnumerable<TCell> GetCellsInCircle(int xCenter, int yCenter, int radius)
        {
            var discovered = new HashSet<int>();

            int d = (5 - (radius * 4)) / 4;
            int x = 0;
            int y = radius;

            do
            {
                foreach (TCell cell in GetCellsAlongLine(xCenter + x, yCenter + y, xCenter - x, yCenter + y))
                {
                    if (AddToHashSet(discovered, cell))
                    {
                        yield return cell;
                    }
                }

                foreach (TCell cell in GetCellsAlongLine(xCenter - x, yCenter - y, xCenter + x, yCenter - y))
                {
                    if (AddToHashSet(discovered, cell))
                    {
                        yield return cell;
                    }
                }

                foreach (TCell cell in GetCellsAlongLine(xCenter + y, yCenter + x, xCenter - y, yCenter + x))
                {
                    if (AddToHashSet(discovered, cell))
                    {
                        yield return cell;
                    }
                }

                foreach (TCell cell in GetCellsAlongLine(xCenter + y, yCenter - x, xCenter - y, yCenter - x))
                {
                    if (AddToHashSet(discovered, cell))
                    {
                        yield return cell;
                    }
                }

                if (d < 0)
                {
                    d += (2 * x) + 1;
                }
                else
                {
                    d += (2 * (x - y)) + 1;
                    y--;
                }

                x++;

            } while (x <= y);
        }

        public IEnumerable<TCell> GetBorderCellsInCircle(int xCenter, int yCenter, int radius)
        {
            var discovered = new HashSet<int>();

            int d = (5 - (radius * 4)) / 4;
            int x = 0;
            int y = radius;

            TCell centerCell = GetCell(xCenter, yCenter);

            do
            {
                if (AddToHashSet(discovered, ClampX(xCenter + x), ClampY(yCenter + y), centerCell, out TCell cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, ClampX(xCenter + x), ClampY(yCenter - y), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, ClampX(xCenter - x), ClampY(yCenter + y), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, ClampX(xCenter - x), ClampY(yCenter - y), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, ClampX(xCenter + y), ClampY(yCenter + x), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, ClampX(xCenter + y), ClampY(yCenter - x), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, ClampX(xCenter - y), ClampY(yCenter + x), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, ClampX(xCenter - y), ClampY(yCenter - x), centerCell, out cell))
                {
                    yield return cell;
                }

                if (d < 0)
                {
                    d += (2 * x) + 1;
                }
                else
                {
                    d += (2 * (x - y)) + 1;
                    y--;
                }

                x++;
            } while (x <= y);
        }

        public IEnumerable<TCell> GetCellsInDiamond(int xCenter, int yCenter, int distance)
        {
            var discovered = new HashSet<int>();

            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(width - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(height - 1, yCenter + distance);

            for (int i = 0; i <= distance; i++)
            {
                for (int j = distance; j >= 0 + i; j--)
                {
                    if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Min(yMax, yCenter + distance - j), out TCell cell))
                    {
                        yield return cell;
                    }

                    if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Max(yMin, yCenter - distance + j), out cell))
                    {
                        yield return cell;
                    }

                    if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Min(yMax, yCenter + distance - j), out cell))
                    {
                        yield return cell;
                    }

                    if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Max(yMin, yCenter - distance + j), out cell))
                    {
                        yield return cell;
                    }
                }
            }
        }

        public IEnumerable<TCell> GetBorderCellsInDiamond(int xCenter, int yCenter, int distance)
        {
            var discovered = new HashSet<int>();

            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(width - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(height - 1, yCenter + distance);

            TCell centerCell = GetCell(xCenter, yCenter);
            if (AddToHashSet(discovered, xCenter, yMin, centerCell, out TCell cell))
            {
                yield return cell;
            }

            if (AddToHashSet(discovered, xCenter, yMax, centerCell, out cell))
            {
                yield return cell;
            }

            for (int i = 1; i <= distance; i++)
            {
                if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Min(yMax, yCenter + distance - i), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Max(yMin, yCenter - distance + i), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Min(yMax, yCenter + distance - i), centerCell, out cell))
                {
                    yield return cell;
                }

                if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Max(yMin, yCenter - distance + i), centerCell, out cell))
                {
                    yield return cell;
                }
            }
        }

        public IEnumerable<TCell> GetCellsInSquare(int xCenter, int yCenter, int distance)
        {
            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(width - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(height - 1, yCenter + distance);

            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    yield return GetCell(x, y);
                }
            }
        }

        public IEnumerable<TCell> GetBorderCellsInSquare(int xCenter, int yCenter, int distance)
        {
            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(width - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(height - 1, yCenter + distance);
            List<TCell> borderCells = new List<TCell>();

            for (int x = xMin; x <= xMax; x++)
            {
                borderCells.Add(GetCell(x, yMin));
                borderCells.Add(GetCell(x, yMax));
            }

            for (int y = yMin + 1; y <= yMax - 1; y++)
            {
                borderCells.Add(GetCell(xMin, y));
                borderCells.Add(GetCell(xMax, y));
            }

            TCell centerCell = GetCell(xCenter, yCenter);
            borderCells.Remove(centerCell);

            return borderCells;
        }

        public IEnumerable<TCell> GetCellsInRectangle(int top, int left, int width, int height)
        {
            int xMin = Math.Max(0, left);
            int xMax = Math.Min(width, left + width);
            int yMin = Math.Max(0, top);
            int yMax = Math.Min(height, top + height);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    yield return GetCell(x, y);
                }
            }
        }

        // public IEnumerable<TCell> GetBorderCellsInRectangle(int top, int left, int width, int height) {}

        public IEnumerable<TCell> GetCellsInRows(params int[] rows)
        {
            foreach (int y in rows)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return GetCell(x, y);
                }
            }
        }

        public IEnumerable<TCell> GetCellsInColumns(params int[] columns)
        {
            foreach (int x in columns)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return GetCell(x, y);
                }
            }
        }

        public IEnumerable<TCell> GetAdjacentCells(int xCenter, int yCenter)
        {
            return GetAdjacentCells(xCenter, yCenter, false);
        }

        public IEnumerable<TCell> GetAdjacentCells(int xCenter, int yCenter, bool includeDiagonals)
        {
            int topY = yCenter - 1;
            int bottomY = yCenter + 1;
            int leftX = xCenter - 1;
            int rightX = xCenter + 1;

            if (topY >= 0)
            {
                yield return GetCell(xCenter, topY);
            }

            if (leftX >= 0)
            {
                yield return GetCell(leftX, yCenter);
            }

            if (bottomY < height)
            {
                yield return GetCell(xCenter, bottomY);
            }

            if (rightX < width)
            {
                yield return GetCell(rightX, yCenter);
            }

            if (includeDiagonals)
            {
                if (rightX < width && topY >= 0)
                {
                    yield return GetCell(rightX, topY);
                }

                if (rightX < width && bottomY < height)
                {
                    yield return GetCell(rightX, bottomY);
                }

                if (leftX >= 0 && topY >= 0)
                {
                    yield return GetCell(leftX, topY);
                }

                if (leftX >= 0 && bottomY < height)
                {
                    yield return GetCell(leftX, bottomY);
                }
            }
        }

        public TCell GetCell(int x, int y)
        {
            return _cells[x, y];
        }

        public override string ToString()
        {
            var mapRepresentation = new StringBuilder();
            int lastY = 0;
            foreach (ICell iCell in GetAllCells())
            {
                Cell cell = (Cell)iCell;
                if (cell.x != lastY)
                {
                    lastY = cell.y;
                    mapRepresentation.Append(Environment.NewLine);
                }
                mapRepresentation.Append(cell.ToString());
            }
            return mapRepresentation.ToString().TrimEnd('\r', '\n');
        }

        public MapState Save()
        {
            var mapState = new MapState
            {
                width = width,
                height = height,
                palettePath = palettePath,
                Cells = new MapState.CellProperties[width * height]
            };

            foreach (TCell cell in GetAllCells())
            {
                MapState.CellProperties cellProperties = MapState.CellProperties.None;

                if (cell.opaque)
                {
                    cellProperties |= MapState.CellProperties.Opaque;
                }

                if (cell.collider)
                {
                    cellProperties |= MapState.CellProperties.Collider;
                }

                if (cell.explored)
                {
                    cellProperties |= MapState.CellProperties.Explored;
                }

                mapState.Cells[(cell.y * width) + cell.x] = cellProperties;
            }
            return mapState;
        }

        public void Restore(MapState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state), "Map state cannot be null");
            }

            Initialize(state.width, state.height, state.palettePath);
            foreach (TCell cell in GetAllCells())
            {
                MapState.CellProperties cellProperties = state.Cells[(cell.y * width) + cell.x];

                _cells[cell.x, cell.y].opaque = cellProperties.HasFlag(MapState.CellProperties.Opaque);
                _cells[cell.x, cell.y].collider = cellProperties.HasFlag(MapState.CellProperties.Collider);
                _cells[cell.x, cell.y].explored = cellProperties.HasFlag(MapState.CellProperties.Explored);
            }
        }

        public TCell CellFor(int index)
        {
            int x = index % width;
            int y = index / width;

            return GetCell(x, y);
        }

        public int IndexFor(int x, int y)
        {
            return (y * width) + x;
        }

        public int IndexFor(TCell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell), "Cell cannot be null");
            }

            return (cell.y * width) + cell.x;
        }

        private bool AddToHashSet(HashSet<int> hashSet, int x, int y, out TCell cell)
        {
            cell = GetCell(x, y);
            return hashSet.Add(IndexFor(cell));
        }

        private bool AddToHashSet(HashSet<int> hashSet, int x, int y, TCell centerCell, out TCell cell)
        {
            cell = GetCell(x, y);
            if (cell.Equals(centerCell))
            {
                return false;
            }

            return hashSet.Add(IndexFor(cell));
        }

        private bool AddToHashSet(HashSet<int> hashSet, TCell cell)
        {
            return hashSet.Add(IndexFor(cell));
        }
    }
}