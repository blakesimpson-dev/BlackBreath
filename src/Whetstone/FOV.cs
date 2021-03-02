using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Whetstone
{
    public class FOV : FOV<Cell>
    {
        public FOV(IMap<Cell> map) : base(map) { }
    }

    public class FOV<TCell> where TCell : ICell
    {
        private readonly IMap<TCell> _map;
        private readonly HashSet<int> _inFov;

        public FOV(IMap<TCell> map)
        {
            _map = map;
            _inFov = new HashSet<int>();
        }

        internal FOV(IMap<TCell> map, HashSet<int> inFov)
        {
            _map = map;
            _inFov = inFov;
        }

        public FOV<TCell> Clone()
        {
            var inFovCopy = new HashSet<int>();
            foreach (int i in _inFov)
            {
                inFovCopy.Add(i);
            }
            return new FOV<TCell>(_map, inFovCopy);
        }

        public bool IsInFov(int x, int y)
        {
            return _inFov.Contains(_map.IndexFor(x, y));
        }

        public ReadOnlyCollection<TCell> ComputeFov(int xOrigin, int yOrigin, int radius, bool lightWalls)
        {
            ClearFov();
            return AppendFov(xOrigin, yOrigin, radius, lightWalls);
        }

        public ReadOnlyCollection<TCell> AppendFov(int xOrigin, int yOrigin, int radius, bool lightWalls)
        {
            foreach (TCell borderCell in _map.GetBorderCellsInSquare(xOrigin, yOrigin, radius))
            {
                foreach (TCell cell in _map.GetCellsAlongLine(xOrigin, yOrigin, borderCell.x, borderCell.y))
                {
                    if ((Math.Abs(cell.x - xOrigin) + Math.Abs(cell.y - yOrigin)) > radius)
                    {
                        break;
                    }

                    if (cell.opaque)
                    {
                        _inFov.Add(_map.IndexFor(cell));
                        cell.explored = true;
                    }
                    else
                    {
                        if (lightWalls)
                        {
                            _inFov.Add(_map.IndexFor(cell));
                            cell.explored = true;
                        }
                        break;
                    }
                }
            }

            if (lightWalls)
            {
                foreach (TCell cell in _map.GetCellsInSquare(xOrigin, yOrigin, radius))
                {
                    if (cell.x > xOrigin)
                    {
                        if (cell.y > yOrigin)
                        {
                            PostProcessFovQuadrant(cell.x, cell.y, Quadrant.SE);
                        }
                        else if (cell.y < yOrigin)
                        {
                            PostProcessFovQuadrant(cell.x, cell.y, Quadrant.NE);
                        }
                    }
                    else if (cell.x < xOrigin)
                    {
                        if (cell.y > yOrigin)
                        {
                            PostProcessFovQuadrant(cell.x, cell.y, Quadrant.SW);
                        }
                        else if (cell.y < yOrigin)
                        {
                            PostProcessFovQuadrant(cell.x, cell.y, Quadrant.NW);
                        }
                    }
                }
            }

            return CellsInFov();
        }

        private ReadOnlyCollection<TCell> CellsInFov()
        {
            var cells = new List<TCell>();
            foreach (int index in _inFov)
            {
                cells.Add(_map.CellFor(index));
            }
            return new ReadOnlyCollection<TCell>(cells);
        }

        private void ClearFov()
        {
            _inFov.Clear();
        }

        private void PostProcessFovQuadrant(int x, int y, Quadrant quadrant)
        {
            int x1 = x;
            int y1 = y;
            int x2 = x;
            int y2 = y;
            switch (quadrant)
            {
                case Quadrant.NE:
                    {
                        y1 = y + 1;
                        x2 = x - 1;
                        break;
                    }
                case Quadrant.SE:
                    {
                        y1 = y - 1;
                        x2 = x - 1;
                        break;
                    }
                case Quadrant.SW:
                    {
                        y1 = y - 1;
                        x2 = x + 1;
                        break;
                    }
                case Quadrant.NW:
                    {
                        y1 = y + 1;
                        x2 = x + 1;
                        break;
                    }
            }
            if (!IsInFov(x, y) && !_map.IsOpaque(x, y))
            {
                if ((_map.IsOpaque(x1, y1) && IsInFov(x1, y1)) || (_map.IsOpaque(x2, y2) && IsInFov(x2, y2))
                    || (_map.IsOpaque(x2, y1) && IsInFov(x2, y1)))
                {
                    _inFov.Add(_map.IndexFor(x, y));
                }
            }
        }

        private enum Quadrant
        {
            NE = 1,
            SE = 2,
            SW = 3,
            NW = 4
        }
    }
}