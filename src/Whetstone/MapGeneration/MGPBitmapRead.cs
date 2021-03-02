using System.Drawing;

namespace Whetstone.MapGeneration
{
    public class MGPBitmapRead<TMap> : MGPBitmapRead<TMap, Cell>, IMapGenerationProcess<TMap> where TMap : IMap<Cell>, new()
    {
        public MGPBitmapRead(string bitmapPath, string palettePath) : base(bitmapPath, palettePath) { }
    }

    public class MGPBitmapRead<TMap, TCell> : IMapGenerationProcess<TMap, TCell> where TMap : IMap<TCell>, new() where TCell : ICell
    {
        private readonly string _bitmapPath;
        private readonly string _palettePath;
        private readonly TMap _map;

        public MGPBitmapRead(string bitmapPath, string palettePath)
        {
            _bitmapPath = bitmapPath;
            _palettePath = palettePath;
            _map = new TMap();
        }

        public TMap GenerateMap()
        {
            var bitmap = new Bitmap(_bitmapPath);
            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;
            
            _map.Initialize(width, height, _palettePath);

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
        
            int x, y;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);

                    if (pixelColor.ToArgb() != Color.Black.ToArgb())
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
                }
            }

            return _map;
        }
    }
}