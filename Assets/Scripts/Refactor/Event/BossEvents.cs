using HT;

namespace ARPG
{
    public struct BossHudChangedEvent
    {
        public BossHudData? Data;
    }

    public struct BossPhaseShiftEvent
    {
        public EnemyManager Boss;
    }
}
