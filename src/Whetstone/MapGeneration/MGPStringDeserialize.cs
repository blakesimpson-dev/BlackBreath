namespace Whetstone.MapGeneration
{
    public class MGPStringDeserialize<TMap> : MGPStringDeserialize<TMap, Cell>, IMapGenerationProcess<TMap> where TMap : IMap<Cell>, new()
    {
        public MGPStringDeserialize(string mapRepresentation, string palettePath) : base(mapRepresentation, palettePath) { }
    }

    public class MGPStringDeserialize<TMap, TCell> : IMapGenerationProcess<TMap, TCell> where TMap : IMap<TCell>, new() where TCell : ICell
    {
        private readonly string _mapRepresentation;
        private readonly string _palettePath;
        private readonly TMap _map;

        public MGPStringDeserialize(string mapRepresentation, string palettePath)
        {
            _mapRepresentation = mapRepresentation;
            _palettePath = palettePath;
            _map = new TMap();
        }

        public TMap GenerateMap()
        {
            string[] lines = _mapRepresentation.Replace(" ", "").Replace("\r", "").Split('\n');

            int width = lines[0].Length;
            int height = lines.Length;
            
            _map.Initialize(width, height, _palettePath);

            for (int y = 0; y < height; y++)
            {
                string line = lines[y];
                for (int x = 0; x < width; x++)
                {
                    if (line[x] == '.')
                    {
                        _map.SetCellProperties(x, y, true, false, false);
                        _map.SetCellRenderProperties(
                            x,
                            y,
                            '.',
                            _map.mapColors.floor,
                            _map.mapColors.bgFloor,
                            _map.mapColors.oosFloor,
                            _map.mapColors.oosBgFloor
                        );
                    }
                    else if (line[x] == 's')
                    {
                        _map.SetCellProperties(x, y, false, false, false);
                        _map.SetCellRenderProperties(
                            x,
                            y,
                            '.',
                            _map.mapColors.floor,
                            _map.mapColors.bgFloor,
                            _map.mapColors.oosFloor,
                            _map.mapColors.oosBgFloor
                        );
                    }
                    else if (line[x] == 'o')
                    {
                        _map.SetCellProperties(x, y, true, true, false);
                        _map.SetCellRenderProperties(
                            x,
                            y,
                            '#',
                            _map.mapColors.wall,
                            _map.mapColors.bgWall,
                            _map.mapColors.oosWall,
                            _map.mapColors.oosBgWall
                        );
                    }
                    else if (line[x] == '#')
                    {
                        _map.SetCellProperties(x, y, false, true, false);
                        _map.SetCellRenderProperties(
                            x,
                            y,
                            '#',
                            _map.mapColors.wall,
                            _map.mapColors.bgWall,
                            _map.mapColors.oosWall,
                            _map.mapColors.oosBgWall
                        );
                    }
                }
            }

            return _map;
        }
    }
}