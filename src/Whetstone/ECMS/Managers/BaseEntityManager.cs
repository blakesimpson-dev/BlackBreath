using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SConsole = SadConsole.Consoles.Console;
using Whetstone.ECMS.Components;

namespace Whetstone.ECMS.Managers
{
    public class BaseEntityManager<T> where T : Entity
    {
        protected static List<T> entities = new List<T>();

        public static void Register(T entity)
        {
            if (entities.Contains(entity))
            {
                return;
            }

            entities.Add(entity);
        }

        public static void Clear()
        {
            entities = new List<T>();
        }

        public static void Update(GameTime gameTime)
        {
            foreach (T entity in entities)
            {
                entity.Update(gameTime);
            }
        }
    }
}
