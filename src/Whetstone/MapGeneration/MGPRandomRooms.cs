using System;
using System.Collections.Generic;
using Whetstone.Random;

namespace Whetstone.MapGeneration
{
    public class MGPRandomRooms<TMap> : MGPRandomRooms<TMap, Cell>, IMapGenerationProcess<TMap> where TMap : IMap<Cell>, new()
    {
        public MGPRandomRooms(int width, int height, string palettePath, int maxRooms, int roomMaxSize, int roomMinSize, IRandom random)
         : base(width, height, palettePath, maxRooms, roomMaxSize, roomMinSize, random) { }

        public MGPRandomRooms(int width, int height, string palettePath, int maxRooms, int roomMaxSize, int roomMinSize)
         : base(width, height, palettePath, maxRooms, roomMaxSize, roomMinSize) { }
    }

    public class MGPRandomRooms<TMap, TCell> : IMapGenerationProcess<TMap, TCell> where TMap : IMap<TCell>, new() where TCell : ICell
    {
        private readonly IRandom _random;
        private readonly int _width;
        private readonly int _height;
        private readonly string _palettePath;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;
        private TMap _map;

        public MGPRandomRooms(int width, int height, string palettePath, int maxRooms, int roomMaxSize, int roomMinSize, IRandom random)
        {
            _width = width;
            _height = height;
            _palettePath = palettePath;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _random = random;
            _map = new TMap();
        }

        public MGPRandomRooms(int width, int height, string palettePath, int maxRooms, int roomMaxSize, int roomMinSize)
        {
            _width = width;
            _height = height;
            _palettePath = palettePath;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _random = RSingleton.DefaultRandom;
            _map = new TMap();
        }

        public TMap GenerateMap()
        {
            var rooms = new List<Rectangle>();
            
            _map.Initialize(_width, _height, _palettePath);

            foreach(TCell cell in _map.GetAllCells())
            {
                _map.SetCellProperties(cell.x, cell.y, false, true, false);
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

            for (int r = 0; r < _maxRooms; r++)
            {
                int roomWidth = _random.Next(_roomMinSize, _roomMaxSize);
                int roomHeight = _random.Next(_roomMinSize, _roomMaxSize);
                int roomXPosition = _random.Next(0, _width - roomWidth - 1);
                int roomYPosition = _random.Next(0, _height - roomHeight - 1);

                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);
                bool newRoomIntersects = false;
                foreach (Rectangle room in rooms)
                {
                    if (newRoom.Intersects(room))
                    {
                        newRoomIntersects = true;
                        break;
                    }
                }
                if (!newRoomIntersects)
                {
                    rooms.Add(newRoom);
                }
            }

            foreach (Rectangle room in rooms)
            {
                MakeRoom(_map, room);
            }

            for (int r = 0; r < rooms.Count; r++)
            {
                if (r == 0)
                {
                    continue;
                }

                int previousRoomCenterX = rooms[r - 1].center.x;
                int previousRoomCenterY = rooms[r - 1].center.y;
                int currentRoomCenterX = rooms[r].center.x;
                int currentRoomCenterY = rooms[r].center.y;

                if (_random.Next(0, 2) == 0)
                {
                    MakeHorizontalTunnel(_map, previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    MakeVerticalTunnel(_map, previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    MakeVerticalTunnel(_map, previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    MakeHorizontalTunnel(_map, previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            return _map;
        }

        private static void MakeRoom(TMap _map, Rectangle room)
        {
            for (int x = room.left + 1; x < room.right; x++)
            {
                for (int y = room.top + 1; y < room.bottom; y++)
                {
                    TCell cell = _map.GetCell(x, y);
                    _map.SetCellProperties(x, y, true, false, cell.explored);
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
        }

        private static void MakeHorizontalTunnel(TMap _map, int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                TCell cell = _map.GetCell(x, yPosition);
                _map.SetCellProperties(x, yPosition, true, false, cell.explored);
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

        private static void MakeVerticalTunnel(TMap _map, int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                TCell cell = _map.GetCell(xPosition, y);
                _map.SetCellProperties(xPosition, y, true, false, cell.explored);
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
    }
}