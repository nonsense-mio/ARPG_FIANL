using Framework;

namespace ARPG
{
    /// <summary>
    /// 根据攻击类型计算体力消耗
    /// 公式: baseStamina * (轻攻击倍率 或 重攻击倍率)
    /// </summary>
    public class GetAttackStaminaCostQuery : AbstractQuery<float>
    {
        private float baseStamina;
        private float lightMultiplier;
        private float heavyMultiplier;
        private E_AttackType attackType;

        public GetAttackStaminaCostQuery() { }

        public GetAttackStaminaCostQuery(float baseStamina, float lightMultiplier,
            float heavyMultiplier, E_AttackType attackType)
        {
            this.baseStamina = baseStamina;
            this.lightMultiplier = lightMultiplier;
            this.heavyMultiplier = heavyMultiplier;
            this.attackType = attackType;
        }

        protected override float OnDo()
        {
            if (attackType == E_AttackType.LightAttack)
                return baseStamina * lightMultiplier;
            if (attackType == E_AttackType.HeavyAttack)
                return baseStamina * heavyMultiplier;
            return 0f;
        }
    }
}
