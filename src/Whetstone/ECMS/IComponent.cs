using Microsoft.Xna.Framework;
using SConsole = SadConsole.Consoles.Console;

namespace Whetstone.ECMS
{
    public interface IComponent
    {
        Entity entity { get; set; }

        void Update(GameTime gameTime);
        void Update(GameTime gameTime, SConsole console, IMap map, FOV fov);
    }
}