using Whetstone.ECMS.Systems;

namespace Whetstone.ECMS.Components
{
    public class CStats : Component
    {
        public CStats(Entity entity) : base(entity) 
        {
            CStatsSystem.Register(this);
        }

        public int awareness { get; set; }
    }
}