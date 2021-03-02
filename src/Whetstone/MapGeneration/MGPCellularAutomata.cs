using System;
using System.Collections.Generic;
using Whetstone.Algorithms;
using Whetstone.Random;

namespace Whetstone.MapGeneration
{
    public class MGPCellularAutomata<TMap> : MGPCellularAutomata<TMap, Cell>, IMapGenerationProcess<TMap> where TMap : class, IMap<Cell>, new()
    {

        public MGPCellularAutomata(int width, int height, string palettePath, int fillProbability, int totalIterations, int cutoffOfBigAreaFill, IRandom random)
           : base(width, height, palettePath, fillProbability, totalIterations, cutoffOfBigAreaFill, random) { }

        public MGPCellularAutomata(int width, int height, string palettePath, int fillProbability, int totalIterations, int cutoffOfBigAreaFill)
           : base(width, height, palettePath, fillProbability, totalIterations, cutoffOfBigAreaFill) { }
    }

    public class MGPCellularAutomata<TMap, TCell> : IMapGenerationProcess<TMap, TCell> where TMap : class, IMap<TCell>, new() where TCell : ICell
    {
        private readonly int _width;
        private readonly int _height;
        private readonly string _palettePath;
        private readonly int _fillProbability;
        private readonly int _totalIterations;
        private readonly int _cutoffOfBigAreaFill;
        private readonly IRandom _random;
        private TMap _map;

        public MGPCellularAutomata(int width, int height, string palettePath, int fillProbability, int totalIterations, int cutoffOfBigAreaFill, IRandom random)
        {
            _width = width;
            _height = height;
            _palettePath = palettePath;
            _fillProbability = fillProbability;
            _totalIterations = totalIterations;
            _cutoffOfBigAreaFill = cutoffOfBigAreaFill;
            _random = random;
            _map = new TMap();
        }


        public MGPCellularAutomata(int width, int height, string palettePath, int fillProbability, int totalIterations, int cutoffOfBigAreaFill)
        {
            _width = width;
            _height = height;
            _palettePath = palettePath;
            _fillProbability = fillProbability;
            _totalIterations = totalIterations;
            _cutoffOfBigAreaFill = cutoffOfBigAreaFill;
            _random = RSingleton.DefaultRandom;
            _map = new TMap();
        }

        public TMap GenerateMap()
        {
            _map.Initialize(_width, _height, _palettePath);

            RandomlyFillCells();

            for (int i = 0; i < _totalIterations; i++)
            {
                if (i < _cutoffOfBigAreaFill)
                {
                    CellularAutomataBigAreaAlgorithm();
                }
                else if (i >= _cutoffOfBigAreaFill)
                {
                    CellularAutomataNearestNeighborsAlgorithm();
                }
            }

            ConnectCaves();

            foreach(TCell cell in _map.GetAllCells())
            {
                if (cell.collider)
                {
                    _map.SetCellRenderProperties(
                        cell.x,
                        cell.y,
                        '#',
                        _map.mapColors.wall,
                        _map.mapColors.bgWall,
                        _map.mapColors.oosWall,
                        _map.mapColors.oosBgWall
                    );
                }
                else
                {
                    _map.SetCellRenderProperties(
                        cell.x,
                        cell.y,
                        '.',
                        _map.mapColors.floor,
                        _map.mapColors.bgFloor,
                        _map.mapColors.oosFloor,
                        _map.mapColors.oosBgFloor
                    );
                }
            }

            return _map;
        }

        private void RandomlyFillCells()
        {
            foreach (TCell cell in _map.GetAllCells())
            {
                if (IsBorderCell(cell))
                {
                    _map.SetCellProperties(cell.x, cell.y, false, true, cell.explored);
                }
                else if (_random.Next(1, 100) < _fillProbability)
                {
                    _map.SetCellProperties(cell.x, cell.y, true, false, cell.explored);
                }
                else
                {
                    _map.SetCellProperties(cell.x, cell.y, false, true, cell.explored);
                }
            }
        }

        private void CellularAutomataBigAreaAlgorithm()
        {
            TMap updatedMap = _map.Clone<TMap>();

            foreach (TCell cell in _map.GetAllCells())
            {
                if (IsBorderCell(cell))
                {
                    continue;
                }
                if ((CountWallsNear(cell, 1) >= 5) || (CountWallsNear(cell, 2) <= 2))
                {
                    updatedMap.SetCellProperties(cell.x, cell.y, false, true, cell.explored);
                }
                else
                {
                    updatedMap.SetCellProperties(cell.x, cell.y, true, false, cell.explored);
                }
            }

            _map = updatedMap;
        }

        private void CellularAutomataNearestNeighborsAlgorithm()
        {
            TMap updatedMap = _map.Clone<TMap>();

            foreach (TCell cell in _map.GetAllCells())
            {
                if (IsBorderCell(cell))
                {
                    continue;
                }
                if (CountWallsNear(cell, 1) >= 5)
                {
                    updatedMap.SetCellProperties(cell.x, cell.y, false, true, cell.explored);
                }
                else
                {
                    updatedMap.SetCellProperties(cell.x, cell.y, true, false, cell.explored);
                }
            }

            _map = updatedMap;
        }

        private bool IsBorderCell(TCell cell)
        {
            return cell.x == 0 || cell.x == _map.width - 1
                   || cell.y == 0 || cell.y == _map.height - 1;
        }

        private int CountWallsNear(TCell cell, int distance)
        {
            int count = 0;
            foreach (TCell nearbyCell in _map.GetCellsInSquare(cell.x, cell.y, distance))
            {
                if (nearbyCell.x == cell.x && nearbyCell.y == cell.y)
                {
                    continue;
                }
                if (nearbyCell.collider)
                {
                    count++;
                }
            }
            return count;
        }

        private void ConnectCaves()
        {
            var floodFillAnalyzer = new FloodFillAnalyzer(_map);
            List<MapSection> mapSections = floodFillAnalyzer.GetMapSections();
            var unionFind = new UnionFind(mapSections.Count);
            while (unionFind.Count > 1)
            {
                for (int i = 0; i < mapSections.Count; i++)
                {
                    int closestMapSectionIndex = FindNearestMapSection(mapSections, i, unionFind);
                    MapSection closestMapSection = mapSections[closestMapSectionIndex];
                    IEnumerable<TCell> tunnelCells = _map.GetCellsAlongLine(mapSections[i].Bounds.center.x, mapSections[i].Bounds.center.y,
                       closestMapSection.Bounds.center.x, closestMapSection.Bounds.center.y);
                    ICell previousCell = null;
                    foreach (TCell cell in tunnelCells)
                    {
                        _map.SetCellProperties(cell.x, cell.y, true, false, cell.explored);
                        if (previousCell != null)
                        {
                            if (cell.x != previousCell.x || cell.y != previousCell.y)
                            {
                                _map.SetCellProperties(cell.x + 1, cell.y, true, false, cell.explored);
                            }
                        }
                        previousCell = cell;
                    }
                    unionFind.Union(i, closestMapSectionIndex);
                }
            }
        }

        private static int FindNearestMapSection(IList<MapSection> mapSections, int mapSectionIndex, UnionFind unionFind)
        {
            MapSection start = mapSections[mapSectionIndex];
            int closestIndex = mapSectionIndex;
            int distance = int.MaxValue;
            for (int i = 0; i < mapSections.Count; i++)
            {
                if (i == mapSectionIndex)
                {
                    continue;
                }
                if (unionFind.Connected(i, mapSectionIndex))
                {
                    continue;
                }
                int distanceBetween = DistanceBetween(start, mapSections[i]);
                if (distanceBetween < distance)
                {
                    distance = distanceBetween;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        private static int DistanceBetween(MapSection startMapSection, MapSection destinationMapSection)
        {
            return Math.Abs(startMapSection.Bounds.center.x - destinationMapSection.Bounds.center.x) 
                + Math.Abs(startMapSection.Bounds.center.y - destinationMapSection.Bounds.center.y);
        }

        private class FloodFillAnalyzer
        {
            private readonly TMap _map;
            private readonly List<MapSection> _mapSections;

            private readonly int[][] _offsets =
            {
            new[] { 0, -1 }, new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, 1 }
         };

            private readonly bool[][] _visited;

            public FloodFillAnalyzer(TMap map)
            {
                _map = map;
                _mapSections = new List<MapSection>();
                _visited = new bool[_map.height][];
                for (int i = 0; i < _visited.Length; i++)
                {
                    _visited[i] = new bool[_map.width];
                }
            }

            public List<MapSection> GetMapSections()
            {
                IEnumerable<TCell> cells = _map.GetAllCells();
                foreach (TCell cell in cells)
                {
                    MapSection section = Visit(cell);
                    if (section.Cells.Count > 0)
                    {
                        _mapSections.Add(section);
                    }
                }

                return _mapSections;
            }

            private MapSection Visit(TCell cell)
            {
                Stack<TCell> stack = new Stack<TCell>(new List<TCell>());
                MapSection mapSection = new MapSection();
                stack.Push(cell);
                while (stack.Count != 0)
                {
                    cell = stack.Pop();
                    if (_visited[cell.y][cell.x] || cell.collider)
                    {
                        continue;
                    }
                    mapSection.AddCell(cell);
                    _visited[cell.y][cell.x] = true;
                    foreach (TCell neighbor in GetNeighbors(cell))
                    {
                        if (!cell.collider == !neighbor.collider && !_visited[neighbor.y][neighbor.x])
                        {
                            stack.Push(neighbor);
                        }
                    }
                }
                return mapSection;
            }

            private TCell GetCell(int x, int y)
            {
                if (x < 0 || y < 0)
                {
                    return default(TCell);
                }
                if (x >= _map.width || y >= _map.height)
                {
                    return default(TCell);
                }
                return _map.GetCell(x, y);
            }

            private IEnumerable<TCell> GetNeighbors(TCell cell)
            {
                List<TCell> neighbors = new List<TCell>(8);
                foreach (int[] offset in _offsets)
                {
                    TCell neighbor = GetCell(cell.x + offset[0], cell.y + offset[1]);
                    if (neighbor == null)
                    {
                        continue;
                    }
                    neighbors.Add(neighbor);
                }

                return neighbors;
            }
        }

        private class MapSection
        {
            private int _top;
            private int _bottom;
            private int _right;
            private int _left;

            public Rectangle Bounds => new Rectangle(_left, _top, _right - _left + 1, _bottom - _top + 1);

            public HashSet<TCell> Cells { get; private set; }

            public MapSection()
            {
                Cells = new HashSet<TCell>();
                _top = int.MaxValue;
                _left = int.MaxValue;
            }

            public void AddCell(TCell cell)
            {
                Cells.Add(cell);
                UpdateBounds(cell);
            }

            private void UpdateBounds(TCell cell)
            {
                if (cell.x > _right)
                {
                    _right = cell.x;
                }
                if (cell.x < _left)
                {
                    _left = cell.x;
                }
                if (cell.y > _bottom)
                {
                    _bottom = cell.y;
                }
                if (cell.y < _top)
                {
                    _top = cell.y;
                }
            }

            public override string ToString()
            {
                return $"Bounds: {Bounds}";
            }
        }
    }
}