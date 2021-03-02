using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SConsole = SadConsole.Consoles.Console;
using Whetstone.ECMS.Components;

namespace Whetstone.ECMS.Systems
{
    public class BaseComponentSystem<T> where T : Component
    {
        protected static List<T> components = new List<T>();

        public static void Register(T component)
        {
            if (components.Contains(component))
            {
                return;
            }

            components.Add(component);
        }

        public static void Clear()
        {
            components = new List<T>();
        }

        public static void Update(GameTime gameTime)
        {
            foreach (T component in components)
            {
                component.Update(gameTime);
            }
        }

        public static void Update(GameTime gameTime, SConsole console, IMap map, FOV fov)
        {
            foreach (T component in components)
            {
                component.Update(gameTime, console, map, fov);
            }
        }
    }

    public class CStatsSystem : BaseComponentSystem<CStats> {}
    public class CTransformSystem : BaseComponentSystem<CTransform> {}
    public class CRendererSystem : BaseComponentSystem<CRenderer> {}
}
