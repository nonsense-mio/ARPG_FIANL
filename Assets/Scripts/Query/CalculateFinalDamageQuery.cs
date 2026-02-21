using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 伤害计算输入数据
    /// </summary>
    public struct DamageInput
    {
        // 攻击方原始伤害
        public int RawPhysicalDamage;
        public int RawFireDamage;
        // 攻击方伤害百分比修正 (默认100)
        public float AttackerPhysicalModifier;
        public float AttackerFireModifier;
        public bool IsFullyChargedAttack;

        // 防御方护甲吸收 (4个部位)
        public float PhysicalAbsorptionHead;
        public float PhysicalAbsorptionBody;
        public float PhysicalAbsorptionLegs;
        public float PhysicalAbsorptionHands;
        public float FireAbsorptionHead;
        public float FireAbsorptionBody;
        public float FireAbsorptionLegs;
        public float FireAbsorptionHands;

        // 防御方额外吸收修正 (默认0)
        public float PhysicalAbsorptionModifier;
        public float FireAbsorptionModifier;
    }

    /// <summary>
    /// 计算最终伤害值 (纯计算，不修改任何状态)
    /// 公式: 原始伤害 × 攻击者修正 → 乘法护甲吸收 → 吸收修正 → 蓄力加成
    /// </summary>
    public class CalculateFinalDamageQuery : AbstractQuery<int>
    {
        private DamageInput input;

        public CalculateFinalDamageQuery() { }

        public CalculateFinalDamageQuery(DamageInput input)
        {
            this.input = input;
        }

        protected override int OnDo()
        {
            // 攻击者伤害修正
            float physicalDamage = input.RawPhysicalDamage * (input.AttackerPhysicalModifier / 100f);
            float fireDamage = input.RawFireDamage * (input.AttackerFireModifier / 100f);

            // 乘法护甲吸收 (物理)
            float totalPhysicalAbsorption = 1f
                - (1 - input.PhysicalAbsorptionHead / 100f)
                * (1 - input.PhysicalAbsorptionBody / 100f)
                * (1 - input.PhysicalAbsorptionLegs / 100f)
                * (1 - input.PhysicalAbsorptionHands / 100f);

            // 乘法护甲吸收 (火焰)
            float totalFireAbsorption = 1f
                - (1 - input.FireAbsorptionHead / 100f)
                * (1 - input.FireAbsorptionBody / 100f)
                * (1 - input.FireAbsorptionLegs / 100f)
                * (1 - input.FireAbsorptionHands / 100f);

            physicalDamage -= physicalDamage * totalPhysicalAbsorption;
            fireDamage -= fireDamage * totalFireAbsorption;

            // 额外吸收修正
            physicalDamage *= (1 - input.PhysicalAbsorptionModifier / 100f);
            fireDamage *= (1 - input.FireAbsorptionModifier / 100f);

            float finalDamage = physicalDamage + fireDamage;

            // 蓄力攻击加成
            if (input.IsFullyChargedAttack)
            {
                finalDamage *= 2f;
            }

            return Mathf.RoundToInt(finalDamage);
        }
    }
}
