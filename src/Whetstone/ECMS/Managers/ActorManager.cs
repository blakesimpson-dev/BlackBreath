using System.Collections.Generic;
using SConsole = SadConsole.Consoles.Console;
using Whetstone.ECMS.Entities;

namespace Whetstone.ECMS.Managers
{
    public class ActorManager : BaseEntityManager<Actor>
    {
        public static Actor player;

        public static void RegisterPlayer(Actor actor)
        {
            if (player != null)
            {
                return;
            }

            player = actor;
            entities.Add(actor);
        }

        public static bool MoveActor(Actor actor, Map map, int x, int y)
        {
            Cell cell = map.GetCell(x, y);
            if (!cell.collider)
            {
                map.SetCellProperties(
                    actor.cTransform.position.x,
                    actor.cTransform.position.y,
                    cell.opaque,
                    false,
                    cell.explored
                );
                
                actor.cTransform.position.x = x;
                actor.cTransform.position.y = y;

                map.SetCellProperties(
                    actor.cTransform.position.x,
                    actor.cTransform.position.y,
                    cell.opaque,
                    true,
                    cell.explored
                );

                return true;
            }
            return false;
        }
    }
}