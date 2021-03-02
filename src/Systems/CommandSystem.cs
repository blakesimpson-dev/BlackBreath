using BlackBreath.Helpers;
using Whetstone.ECMS.Components;
using Whetstone.ECMS.Managers;

namespace BlackBreath.Systems
{
    public class CommandSystem
    {
        public bool MovePlayer(Direction direction)
        {
            CTransform playerTransform = ActorManager.player.cTransform;
            int x = playerTransform.position.x;
            int y = playerTransform.position.y;

            switch (direction)
            {
                case Direction.Up:
                    {
                        y = playerTransform.position.y - 1;
                        break;
                    }
                case Direction.Down:
                    {
                        y = playerTransform.position.y + 1;
                        break;
                    }
                case Direction.Left:
                    {
                        x = playerTransform.position.x - 1;
                        break;
                    }
                case Direction.Right:
                    {
                        x = playerTransform.position.x + 1;
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            if (ActorManager.MoveActor(ActorManager.player, Game.currentMap, x, y))
            {
                return true;
            }

            return false;
        }
    }
}