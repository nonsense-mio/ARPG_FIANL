using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 根据攻击类型(轻/重) × 使用手(左/右) × 武器修正系数 计算最终物理伤害
    /// </summary>
    public class CalculateAttackTypeDamageQuery : AbstractQuery<int>
    {
        private int basePhysicalDamage;
        private E_AttackType attackType;
        private bool isUsingRightHand;
        private float rightLightModifier;
        private float rightHeavyModifier;
        private float leftLightModifier;
        private float leftHeavyModifier;

        public CalculateAttackTypeDamageQuery() { }

        public CalculateAttackTypeDamageQuery(int basePhysicalDamage, E_AttackType attackType,
            bool isUsingRightHand,
            float rightLightModifier, float rightHeavyModifier,
            float leftLightModifier, float leftHeavyModifier)
        {
            this.basePhysicalDamage = basePhysicalDamage;
            this.attackType = attackType;
            this.isUsingRightHand = isUsingRightHand;
            this.rightLightModifier = rightLightModifier;
            this.rightHeavyModifier = rightHeavyModifier;
            this.leftLightModifier = leftLightModifier;
            this.leftHeavyModifier = leftHeavyModifier;
        }

        protected override int OnDo()
        {
            float finalDamage = basePhysicalDamage;

            if (isUsingRightHand)
            {
                if (attackType == E_AttackType.LightAttack)
                    finalDamage *= rightLightModifier;
                else if (attackType == E_AttackType.HeavyAttack)
                    finalDamage *= rightHeavyModifier;
            }
            else
            {
                if (attackType == E_AttackType.LightAttack)
                    finalDamage *= leftLightModifier;
                else if (attackType == E_AttackType.HeavyAttack)
                    finalDamage *= leftHeavyModifier;
            }

            return Mathf.RoundToInt(finalDamage);
        }
    }
}
