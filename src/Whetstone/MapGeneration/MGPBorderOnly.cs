namespace Whetstone.MapGeneration
{
    public class MGPBorderOnly<TMap> : MGPBorderOnly<TMap, Cell>, IMapGenerationProcess<TMap> where TMap : IMap<Cell>, new()
    {
        public MGPBorderOnly(int width, int height, string palettePath) : base(width, height, palettePath) { }
    }

    public class MGPBorderOnly<TMap, TCell> : IMapGenerationProcess<TMap, TCell> where TMap : IMap<TCell>, new() where TCell : ICell
    {
        private readonly int _height;
        private readonly int _width;
        private readonly string _palettePath;

        public MGPBorderOnly(int width, int height, string palettePath)
        {
            _width = width;
            _height = height;
            _palettePath = palettePath;
        }

        public TMap GenerateMap()
        {
            var map = new TMap();
            map.Initialize(_width, _height, _palettePath);
            map.Clear(true, false, false);

            foreach (TCell cell in map.GetAllCells())
            {
                map.SetCellRenderProperties(
                    cell.x,
                    cell.y,
                    '.',
                    map.mapColors.floor,
                    map.mapColors.bgFloor,
                    map.mapColors.oosFloor,
                    map.mapColors.oosBgFloor
                );
            }

            foreach (TCell cell in map.GetCellsInRows(0, _height - 1))
            {
                map.SetCellProperties(cell.x, cell.y, false, true, false);
                map.SetCellRenderProperties(
                    cell.x,
                    cell.y,
                    '#',
                    map.mapColors.wall,
                    map.mapColors.bgWall,
                    map.mapColors.oosWall,
                    map.mapColors.oosBgWall
                );
            }

            foreach (TCell cell in map.GetCellsInColumns(0, _width - 1))
            {
                map.SetCellProperties(cell.x, cell.y, false, true, false);
                map.SetCellRenderProperties(
                    cell.x,
                    cell.y,
                    '#',
                    map.mapColors.wall,
                    map.mapColors.bgWall,
                    map.mapColors.oosWall,
                    map.mapColors.oosBgWall
                );
            }

            return map;
        }
    }
}