using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 计算暴击伤害 (背刺/招架处决)
    /// 公式: criticalMultiplier * (physicalDamage + fireDamage)
    /// </summary>
    public class GetCriticalDamageQuery : AbstractQuery<int>
    {
        private float criticalMultiplier;
        private int physicalDamage;
        private int fireDamage;

        public GetCriticalDamageQuery() { }

        public GetCriticalDamageQuery(float criticalMultiplier, int physicalDamage, int fireDamage)
        {
            this.criticalMultiplier = criticalMultiplier;
            this.physicalDamage = physicalDamage;
            this.fireDamage = fireDamage;
        }

        protected override int OnDo()
        {
            return Mathf.RoundToInt(criticalMultiplier * (physicalDamage + fireDamage));
        }
    }
}
