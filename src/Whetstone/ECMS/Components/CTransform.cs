using Whetstone.ECMS.Systems;

namespace Whetstone.ECMS.Components
{
    public class CTransform : Component
    {
        public CTransform(Entity entity) : base(entity) {
            CTransformSystem.Register(this);
            position.x = 0;
            position.y = 0;
        }

        public Point position;
    }
}