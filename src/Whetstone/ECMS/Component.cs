using Microsoft.Xna.Framework;
using SConsole = SadConsole.Consoles.Console;

namespace Whetstone.ECMS
{
    public class Component : IComponent
    {
        private Entity _entity;
        public Entity entity { 
            get { return _entity; } 
            set { _entity = value; } 
        }

        public Component(Entity entity) {
            _entity = entity;
        }

        public virtual void Update(GameTime gameTime) {}
        public virtual void Update(GameTime gameTime, SConsole console, IMap map, FOV fov) {}
    }
}