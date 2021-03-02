using Microsoft.Xna.Framework;
using SConsole = SadConsole.Consoles.Console;
using Whetstone.ECMS.Systems;

namespace Whetstone.ECMS.Components
{
    public class CRenderer : Component
    {
        public CRenderer(Entity entity) : base(entity) 
        { 
            CRendererSystem.Register(this);
        }

        public Color color { get; set; }
        public char character { get; set; }
        public int renderLayer { get; set; }

        public override void Update(GameTime gameTime, SConsole console, IMap map, FOV fov)
        {
            Render(console, map, fov);
        }

        private void Render(SConsole console, IMap map, FOV fov)
        {
            CTransform cTransform = base.entity.GetComponent<CTransform>();
            int x = cTransform.position.x;
            int y = cTransform.position.y;

            if (!map.GetCell(x, y).explored)
            {
                return;
            }

            if (fov.IsInFov(x, y))
            {
                console.CellData.SetCharacter(x, y, character, color);
            }
            else
            {
                Cell cell = map.GetCell(x, y);
                if (cell.collider)
                {
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
                else
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
            }
        }
    }
}