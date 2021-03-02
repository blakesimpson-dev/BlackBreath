// using System;
// using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XnaGame = Microsoft.Xna.Framework.Game;
using XnaPoint = Microsoft.Xna.Framework.Point;
using Microsoft.Xna.Framework.Input;
using SConsole = SadConsole.Consoles.Console;
using Whetstone;
using Whetstone.MapGeneration;
using Whetstone.ECMS.Entities;
// using Whetstone.ECMS.Components;
using Whetstone.ECMS.Managers;
using Whetstone.ECMS.Systems;
using BlackBreath.Helpers;
using BlackBreath.Systems;

namespace BlackBreath
{
    public class Game : XnaGame
    {
        private readonly GraphicsDeviceManager _graphics;

        private static readonly int
            _screenWidth = 100,
            _screenHeight = 70,
            _mapWidth = 80,
            _mapHeight = 48,
            _messageWidth = 80,
            _messageHeight = 11,
            _statWidth = 20,
            _statHeight = 70,
            _inventoryWidth = 80,
            _inventoryHeight = 11;

        private static SConsole
            _mapConsole,
            _messageConsole,
            _statConsole,
            _inventoryConsole;

        private static bool _render = true;
        private static InputState _inputState;

        private string gamePalettePath = "assets/Palettes/Default.xml";
        private string mapPalettePath = "assets/Palettes/Default.xml";
        public static Colors gameColors { get; private set; }

        public static MessagingSystem messagingSystem { get; private set; }
        public static CommandSystem commandSystem { get; private set; }

        public static Map currentMap { get; private set; }
        public static FOV fov { get; private set; }

        public Game()
        {
            _inputState = new InputState();

            gameColors = new Colors(gamePalettePath);

            messagingSystem = new MessagingSystem();
            messagingSystem.Add("Welcome to Black Breath.");
            commandSystem = new CommandSystem();

            ActorManager.RegisterPlayer(new Actor("Rogue", 15, gameColors.player, '@', 5, 5));

            // IMapGenerationProcess<Map> mapGenerationProcess = new MGPBorderOnly<Map>(_mapWidth, _mapHeight, mapPalettePath);            
            // IMapGenerationProcess<Map> mapGenerationProcess = new MGPBitmapRead<Map>("assets/Maps/Bitmap/TestInput.png", mapPalettePath);
            // IMapGenerationProcess<Map> mapGenerationProcess = new MGPWaveFunctionCollapse<Map>(_mapWidth, _mapHeight, mapPalettePath);

            IMapGenerationProcess<Map> mapGenerationProcess = new MGPRandomRooms<Map>(
                _mapWidth,
                _mapHeight,
                mapPalettePath,
                20,
                10,
                8
            );

            currentMap = Map.Generate(mapGenerationProcess);
            fov = new FOV(currentMap);

            foreach(Cell cell in currentMap.GetAllCells())
                cell.explored = true;

            _graphics = new GraphicsDeviceManager(this);
            this.Window.Title = "Black Breath";
            Content.RootDirectory = "Content";

            var sadConsoleComponent = new SadConsole.EngineGameComponent(this, () =>
            {
                using (var stream = System.IO.File.OpenRead("assets/Fonts/Cheepicus12.font"))
                    SadConsole.Engine.DefaultFont = SadConsole.Serializer.Deserialize<SadConsole.Font>(stream);

                SadConsole.Engine.DefaultFont.ResizeGraphicsDeviceManager(_graphics, _screenWidth, _screenHeight, 0, 0);
                SadConsole.Engine.UseMouse = true;
                SadConsole.Engine.UseKeyboard = true;

                _mapConsole = new SConsole(_mapWidth, _mapHeight);
                _messageConsole = new SConsole(_messageWidth, _messageHeight);
                _statConsole = new SConsole(_statWidth, _statHeight);
                _inventoryConsole = new SConsole(_inventoryWidth, _inventoryHeight);

                _mapConsole.Position = new XnaPoint(0, _inventoryHeight);
                _messageConsole.Position = new XnaPoint(0, _screenHeight - _messageHeight);
                _statConsole.Position = new XnaPoint(_mapWidth, 0);
                _inventoryConsole.Position = new XnaPoint(0, 0);

                SadConsole.Engine.ConsoleRenderStack.Add(_mapConsole);
                SadConsole.Engine.ConsoleRenderStack.Add(_messageConsole);
                SadConsole.Engine.ConsoleRenderStack.Add(_statConsole);
                SadConsole.Engine.ConsoleRenderStack.Add(_inventoryConsole);

                SadConsole.Engine.ActiveConsole = _mapConsole;
            });

            Components.Add(sadConsoleComponent);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            bool performedAction = false;
            _inputState.Update(gameTime);

            if (_inputState.IsKeyPressed(Keys.Up))
            {
                performedAction = commandSystem.MovePlayer(Direction.Up);
            }
            else if (_inputState.IsKeyPressed(Keys.Down))
            {
                performedAction = commandSystem.MovePlayer(Direction.Down);
            }
            else if (_inputState.IsKeyPressed(Keys.Left))
            {
                performedAction = commandSystem.MovePlayer(Direction.Left);
            }
            else if (_inputState.IsKeyPressed(Keys.Right))
            {
                performedAction = commandSystem.MovePlayer(Direction.Right);
            }
            else if (_inputState.IsKeyPressed(Keys.Escape))
            {
                this.Exit();
            }

            if (performedAction)
            {
                _render = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_render)
            {
                Render(gameTime);
                base.Draw(gameTime);
                _render = false;
            }
        }

        protected void Render(GameTime gameTime)
        {
            GraphicsDevice.Clear(gameColors.screen);

            _mapConsole.CellData.Clear();
            _messageConsole.CellData.Clear();
            _statConsole.CellData.Clear();
            _inventoryConsole.CellData.Clear();

            fov.ComputeFov(
                ActorManager.player.cTransform.position.x,
                ActorManager.player.cTransform.position.y,
                ActorManager.player.cStats.awareness,
                true
            );

            currentMap.Render(_mapConsole, fov);
            messagingSystem.Render(_messageConsole);

            CRendererSystem.Update(gameTime, _mapConsole, currentMap, fov);
        }
    }
}
