using Framework;
using UnityEngine;

namespace ARPG
{
    public struct LevelUpCostResult
    {
        public int ProjectedLevel;
        public int SoulCost;
    }

    /// <summary>
    /// 根据属性变化量计算升级所需魂数和预计等级
    /// </summary>
    public class GetLevelUpCostQuery : AbstractQuery<LevelUpCostResult>
    {
        private int healthDelta, staminaDelta, focusDelta, poiseDelta;
        private int strengthDelta, dexterityDelta, intelligenceDelta, faithDelta;

        private const int BASE_LEVEL_UP_SOUL_COST = 5;

        public GetLevelUpCostQuery() { }

        public GetLevelUpCostQuery(int healthDelta, int staminaDelta, int focusDelta, int poiseDelta,
            int strengthDelta, int dexterityDelta, int intelligenceDelta, int faithDelta)
        {
            this.healthDelta = healthDelta;
            this.staminaDelta = staminaDelta;
            this.focusDelta = focusDelta;
            this.poiseDelta = poiseDelta;
            this.strengthDelta = strengthDelta;
            this.dexterityDelta = dexterityDelta;
            this.intelligenceDelta = intelligenceDelta;
            this.faithDelta = faithDelta;
        }

        protected override LevelUpCostResult OnDo()
        {
            var playerModel = this.GetModel<IPlayerModel>();
            int currentLevel = playerModel.PlayerLevel;

            int totalDelta = healthDelta + staminaDelta + focusDelta + poiseDelta
                           + strengthDelta + dexterityDelta + intelligenceDelta + faithDelta;
            int projectedLevel = currentLevel + totalDelta;

            int soulCost = 0;
            for (int i = 0; i < projectedLevel; i++)
            {
                soulCost += Mathf.RoundToInt(totalDelta * BASE_LEVEL_UP_SOUL_COST * 1.1f);
            }

            return new LevelUpCostResult
            {
                ProjectedLevel = projectedLevel,
                SoulCost = soulCost
            };
        }
    }
}
