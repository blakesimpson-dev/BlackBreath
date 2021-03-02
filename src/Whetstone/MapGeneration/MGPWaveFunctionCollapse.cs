
using System;
using System.Xml.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Whetstone.Random;
using Whetstone.MapGeneration.WFC;

namespace Whetstone.MapGeneration
{
    public class MGPWaveFunctionCollapse<TMap> : MGPWaveFunctionCollapse<TMap, Cell>, IMapGenerationProcess<TMap> where TMap : class, IMap<Cell>, new()
    {
        public MGPWaveFunctionCollapse(int width, int height, string palettePath, IRandom random) : base(width, height, palettePath, random) { }

        public MGPWaveFunctionCollapse(int width, int height, string palettePath) : base(width, height, palettePath) { }
    }

    public class MGPWaveFunctionCollapse<TMap, TCell> : IMapGenerationProcess<TMap, TCell> where TMap : IMap<TCell>, new() where TCell : ICell
    {
        private readonly IRandom _random;
        private readonly int _width;
        private readonly int _height;
        private readonly string _palettePath;
        private TMap _map;

        private readonly List<ICell> _indoorCells = new List<ICell>();
        private readonly List<ICell> _outdoorCells = new List<ICell>();
        private readonly List<ICell> _doorCells = new List<ICell>();

        public MGPWaveFunctionCollapse(int width, int height, string palettePath, IRandom random)
        {
            _width = width;
            _height = height;
            _palettePath = palettePath;
            _random = random;
            _map = new TMap();
        }

        public MGPWaveFunctionCollapse(int width, int height, string palettePath)
        {
            _width = width;
            _height = height;
            _palettePath = palettePath;
            _random = RSingleton.DefaultRandom;
            _map = new TMap();
        }

        public TMap GenerateMap()
        {
            System.Random random = new System.Random();
            XDocument xdoc = XDocument.Load("assets/WFC/Input/WFCInput.xml");

            List<Bitmap> bitmaps = new List<Bitmap>();

            int counter = 1;
            foreach (XElement xelem in xdoc.Root.Elements("overlapping"))
            {
                Model model;
                string xName = xelem.Get<string>("name");
                if (xelem.Name == "overlapping")
                {
                    model = new OverlappingModel(xName, xelem.Get("N", 2), xelem.Get("width", 48), xelem.Get("height", 48),
                        xelem.Get("periodicInput", true), xelem.Get("periodic", false), xelem.Get("symmetry", 8), xelem.Get("ground", 0));
                }
                else continue;
                for (int i = 0; i < xelem.Get("screenshots", 2); i++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        int seed = random.Next();
                        bool finished = model.Run(seed, xelem.Get("limit", 0));
                        if (finished)
                        {
                            Bitmap xBitmap = model.Graphics();
                            bitmaps.Add(xBitmap);
                            model.Graphics().Save(@"assets\WFC\Output\" + $"{counter}{xName}-{i}.png");
                            break;
                        }
                        else System.Console.WriteLine("CONTRADICTION");
                    }
                }
                counter++;
            }

            _map.Initialize(_width, _height, _palettePath);

            foreach (TCell cell in _map.GetAllCells())
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

            // string name = "2Input-010";
            // var bitmap = new Bitmap(@"assets\WFC\Output\" + $"{name}.png");
            var bitmap = bitmaps[0];
            int x, y;

            for (x = 0; x < bitmap.Width; x++)
            {
                for (y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);

                    // Make red and white into Floor
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


/*
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Whetstone.MapGeneration
{
    public class MGPWaveFunctionCollapse
    {
        private readonly int _width;
        private readonly int _height;

        private List<ICell> _indoorCells = new List<ICell>();
        private List<ICell> _outdoorCells = new List<ICell>();
        private List<ICell> _doorCells = new List<ICell>();

        private readonly MDungeon _map;

        public MGPWaveFunctionCollapse(int width, int height, string palettePath)
        {
            _width = width;
            _height = height;
            _map = new MDungeon();
        }

        public MDungeon Generate()
        {
            _map.Initialize(_width, _height);
            foreach (Cell cell in _map.GetAllCells())
            {
                _map.SetCellProperties(cell.X, cell.Y, false, false, false);
            }

            // Read first bitmap file
            string name = "2Input-010";
            var bitmap = new Bitmap(@"assets\WFC\Output\" + $"{name}.png");
            int x, y;

            for (x = 0; x < bitmap.Width; x++)
            {
                for (y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);

                    // Make red and white into Floor
                    if (pixelColor.ToArgb() != Color.Black.ToArgb())
                    {
                        _map.SetCellProperties(x, y, true, true, false);
                    }
                }
            }

            // Create walls for boundary row cells marked red
            foreach (Cell cell in _map.GetCellsInRows(
                3,
                _height - 4))
            {
                Color pixelColor = bitmap.GetPixel(cell.X, cell.Y);
                if (pixelColor.ToArgb() != Color.Black.ToArgb()
                    && pixelColor.ToArgb() != Color.White.ToArgb())
                {
                    _map.SetCellProperties(cell.X, cell.Y, false, false, false);
                    bitmap.SetPixel(cell.X, cell.Y, Color.Black);
                }
            }

            // Create walls for boundary column cells marked red
            foreach (Cell cell in _map.GetCellsInColumns(
                3,
                _width - 4))
            {
                Color pixelColor = bitmap.GetPixel(cell.X, cell.Y);
                if (pixelColor.ToArgb() != Color.Black.ToArgb()
                    && pixelColor.ToArgb() != Color.White.ToArgb())
                {
                    _map.SetCellProperties(cell.X, cell.Y, false, false, false);
                    bitmap.SetPixel(cell.X, cell.Y, Color.Black);
                }
            }

            // Set the first 3 and last 3 rows in the map to not be transparent or walkable
            foreach (Cell cell in _map.GetCellsInRows(
                0,
                1,
                2,
                _height - 1,
                _height - 2,
                _height - 3))
            {
                _map.SetCellProperties(cell.X, cell.Y, true, true, false);
                bitmap.SetPixel(cell.X, cell.Y, Color.White);
            }

            // Set the first 3 and last 3 columns in the map to not be transparent or walkable
            foreach (Cell cell in _map.GetCellsInColumns(
                0,
                1,
                2,
                _width - 1,
                _width - 2,
                _width - 3))
            {
                _map.SetCellProperties(cell.X, cell.Y, true, true, false);
                bitmap.SetPixel(cell.X, cell.Y, Color.White);
            }

            // Set _outdoorCells
            foreach(ICell cell in _map.GetAllCells())
            {
                Color pixelColor = bitmap.GetPixel(cell.X, cell.Y);
                if (pixelColor.ToArgb() == Color.White.ToArgb())
                {
                    _outdoorCells.Add(cell);
                }
            }

            // Find starting point for postProcessing
            x = (int)(Math.Floor((double)(_width / 2)));
            y = (int)(Math.Floor((double)(_height / 2)));
            Console.WriteLine(string.Format("Starting origin search at [{0}, {1}]", x, y));

            // Check starting point first
            ICell origin = null;
            if (_map.GetCell(x, y).IsWalkable
                && bitmap.GetPixel(x, y).ToArgb() == Color.White.ToArgb())
            {
                origin = _map.GetCell(x, y);
            }

            // Now loop through neighbours until origin found
            while (origin == null)
            {
                List<ICell> neighbourCells = new List<ICell>();
                neighbourCells.Add(_map.GetCell(x - 1, y - 1));
                neighbourCells.Add(_map.GetCell(x - 1, y));
                neighbourCells.Add(_map.GetCell(x - 1, y + 1));
                neighbourCells.Add(_map.GetCell(x, y + 1));
                neighbourCells.Add(_map.GetCell(x + 1, y + 1));
                neighbourCells.Add(_map.GetCell(x + 1, y));
                neighbourCells.Add(_map.GetCell(x + 1, y - 1));
                neighbourCells.Add(_map.GetCell(x, y - 1));

                foreach (ICell cell in neighbourCells)
                {
                    if (cell.IsWalkable
                        && bitmap.GetPixel(cell.X, cell.Y).ToArgb() == Color.White.ToArgb())
                    {
                        origin = cell;
                        break;
                    }
                }
                x++;
                y++;
            }

            // Mark origin with the following:
            x = origin.X;
            y = origin.Y;
            Console.WriteLine(string.Format("Origin for build found: [{0}, {1}]", x, y));
            // bitmap.SetPixel(x, y, Color.Green);

            // 1. Iterate over all cells to find a walkable cell that is red
            //  - replace this with make a list of all red cells
            //  - run logic while unreachableCell count is != 0
            //  - randomly select one
            //  - after we find a door for a room, remove the floodCells from redCells list
            //
            List<ICell> unreachableCells = new List<ICell>();
            foreach (ICell cell in _map.GetAllCells())
            {
                if (cell.IsWalkable
                    && bitmap.GetPixel(cell.X, cell.Y).ToArgb() != Color.White.ToArgb())
                {
                    unreachableCells.Add(cell);
                    _indoorCells.Add(cell);
                }
            }

            int opCounter = 0;
            while (unreachableCells.Count != 0)
            {
                Console.WriteLine(string.Format(
                    "Beginning operation {0}. Current UnreachableCount: {1}", opCounter++, unreachableCells.Count));

                Random r = new Random();
                int rInt = r.Next(0, unreachableCells.Count);
                ICell randomCell = unreachableCells[rInt];

                Console.WriteLine(string.Format("Pathfinding first target: [{0}, {1}]", randomCell.X, randomCell.Y));
                // bitmap.SetPixel(target.X, target.Y, Color.Green);    

                // 2. Floodfill get all redcells in this 'room'
                List<ICell> floodCells = new List<ICell>();
                FloodFill(floodCells, randomCell, bitmap);

                // 3. Once we have this list of cells, randomly select one
                rInt = r.Next(0, floodCells.Count);
                randomCell = floodCells[rInt];

                // Declare doorcell to start search
                ICell doorCell = null;
                while (doorCell == null)
                {
                    // 4. now randomly select one of 4 directions
                    int rDir = r.Next(0, 4);
                    int xMod = 0;
                    int yMod = 0;
                    if (rDir == 0)
                        xMod = 1;
                    else if (rDir == 1)
                        xMod = -1;
                    else if (rDir == 2)
                        yMod = 1;
                    else if (rDir == 3)
                        yMod = -1;

                    Console.WriteLine(string.Format("Direction to step: [{0}, {1}]", xMod, yMod));

                    // 5. Step along this direction until we find a wall
                    ICell stepTowardWallCell = randomCell;
                    while (stepTowardWallCell.IsWalkable)
                    {
                        int newX = stepTowardWallCell.X + xMod;
                        int newY = stepTowardWallCell.Y + yMod;
                        stepTowardWallCell = _map.GetCell(newX, newY);
                    }

                    // 6. If this wall has no white cell one step further in the current direction, go back to step 2.
                    ICell wallCell = stepTowardWallCell;
                    if (_map.GetCell(wallCell.X + xMod, wallCell.Y + yMod).IsWalkable)
                    {
                        doorCell = wallCell;
                    }
                }

                // 7. If it does, this is a door candidate - store it in list of doors (set _map tile as walkable)
                _doorCells.Add(doorCell);
                _map.SetCellProperties(doorCell.X, doorCell.Y, true, true, false);

                // 8. Set color of door tile to white and all floodfilled cells in this 'room' to white
                bitmap.SetPixel(doorCell.X, doorCell.Y, Color.White);

                // Remove floodCells from unreachableCells
                foreach(ICell cell in floodCells)
                {
                    unreachableCells.Remove(cell);
                }
            }

            // 9. Find a new red cell, go back to step 1.
            // 10. If no red cells remain, draw doors in bitmap
            foreach (ICell cell in _doorCells)
            {
                bitmap.SetPixel(cell.X, cell.Y, Color.Yellow);
            }

            // Draw indoor cells in bitmap
            foreach (ICell cell in _indoorCells)
            {
                bitmap.SetPixel(cell.X, cell.Y, Color.DimGray);
            }

            // Draw outdoor cells in bitmap using second bitmap as alternator
            foreach (ICell cell in _outdoorCells)
            {
                // Read second bitmap file
                string name2 = "3Input-020";
                var bitmap2 = new Bitmap(@"assets\WFC\Output\" + $"{name2}.png");

                Color pixelColor = bitmap2.GetPixel(cell.X, cell.Y);
                // If bitmap2 value is white, color map bitmap light green
                if (pixelColor.ToArgb() != Color.Black.ToArgb())
                {
                    bitmap.SetPixel(cell.X, cell.Y, Color.LightGreen);
                }
                else
                {
                    bitmap.SetPixel(cell.X, cell.Y, Color.SlateBlue);
                }
            }

            // LARGE AREAS WITH MULTIPLE DOORS?
            // SPARSE AREAS ON BOUNDARY - MAYBE SOME RANDOM SCATTERED WALLS ?
            // TEST WITH OTHER INPUTS ??

            // !TODO! *** CAN HAVE VERY SMALL AREAS THAT HAVE DOORS PLACED ***
            // When floodfilling - check volume (floodCell count)
            // if its below a certain threshhold, fill the room (set unwalkable and color black)

            // !TODO! *** CAN HAVE DOORS THAT DONT HAVE ADJACENT WALLS ***
            // Replace current door placement with a method - check left/right and up/down to determine door 'direction'
            // Then make sure there is a wall on both sides of the door in the other direction

            // !TODO! *** THERE MAY BE ISSUES IF YOU HAVE ROOMS WITHIN ROOMS ***
            // If you have this issue, instead of iterating to select a red origin for build, select it randomly
            // Try generating new maps over and over until you see a room within a room...

            // !TODO! *** CLEAN UP THIS METHOD, REMOVE WHILE LOOPS AND USE RECURSIVE FUNCTIONS ***
            // Turn some of it into re-uable helpers, simplify and optimize where possible
            // Use roguesharp 'getline' etc. and anything else you can think off

            name += "-Built";
            bitmap.Save(@"assets\WFC\Output\" + $"{name}.png");

            // Reveal the whole map for testing purposes
            foreach (Cell cell in _map.GetAllCells())
            {
                _map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }

            return _map;
        }

        public void FloodFill(List<ICell> floodCells, ICell target, Bitmap bitmap)
        {
            if (floodCells.Contains(target))
            {
                return;
            }

            if (bitmap.GetPixel(target.X, target.Y).ToArgb() == Color.White.ToArgb()
                || bitmap.GetPixel(target.X, target.Y).ToArgb() == Color.Black.ToArgb())
            {
                return;
            }

            bitmap.SetPixel(target.X, target.Y, Color.White);
            floodCells.Add(target);

            FloodFill(floodCells, _map.GetCell(target.X + 1, target.Y), bitmap);
            FloodFill(floodCells, _map.GetCell(target.X - 1, target.Y), bitmap);
            FloodFill(floodCells, _map.GetCell(target.X, target.Y + 1), bitmap);
            FloodFill(floodCells, _map.GetCell(target.X, target.Y - 1), bitmap);
        }
    }
}
*/