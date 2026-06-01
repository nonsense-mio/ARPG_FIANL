using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 格挡计算结果
    /// </summary>
    public struct BlockResult
    {
        /// <summary>格挡是否成功 (方向+状态判定)</summary>
        public bool IsBlocked;
        /// <summary>穿透物理伤害 (格挡后残余)</summary>
        public int PhysicalDamageAfterBlock;
        /// <summary>穿透火焰伤害 (格挡后残余)</summary>
        public int FireDamageAfterBlock;
        /// <summary>格挡消耗的体力</summary>
        public float StaminaDamage;
        /// <summary>是否破防 (体力不足)</summary>
        public bool IsGuardBroken;
    }

    /// <summary>
    /// 格挡计算输入数据
    /// </summary>
    public struct BlockInput
    {
        // 方向判定
        public Vector3 AttackerPosition;
        public Vector3 DefenderPosition;
        public Vector3 DefenderForward;
        public bool IsDefenderBlocking;

        // 攻击数据
        public int PhysicalDamage;
        public int FireDamage;
        public float GuardBreakModifier;

        // 防御方格挡属性
        public float BlockingPhysicalAbsorption;
        public float BlockingFireAbsorption;
        public float BlockingStabilityRating;
        public float DefenderCurrentStamina;
    }

    /// <summary>
    /// 计算格挡结果 (纯计算，不修改任何状态)
    /// 合并 DamageCollider.CheckForBlock + CharacterCombatManager.AttemptBlock 的计算逻辑
    /// </summary>
    public class CalculateBlockResultQuery : AbstractQuery<BlockResult>
    {
        private BlockInput input;

        public CalculateBlockResultQuery() { }

        public CalculateBlockResultQuery(BlockInput input)
        {
            this.input = input;
        }

        protected override BlockResult OnDo()
        {
            var result = new BlockResult();

            // 方向判定: 攻击者是否在防御者正前方 ±72° 扇形内
            Vector3 directionToAttacker = (input.AttackerPosition - input.DefenderPosition).normalized;
            float facing = Vector3.Dot(directionToAttacker, input.DefenderForward);

            if (!input.IsDefenderBlocking || facing <= 0.3f)
            {
                result.IsBlocked = false;
                return result;
            }

            result.IsBlocked = true;

            // 穿透伤害计算
            result.PhysicalDamageAfterBlock = Mathf.RoundToInt(
                input.PhysicalDamage - (input.PhysicalDamage * input.BlockingPhysicalAbsorption) / 100f);
            result.FireDamageAfterBlock = Mathf.RoundToInt(
                input.FireDamage - (input.FireDamage * input.BlockingFireAbsorption) / 100f);

            // 体力伤害计算
            float totalDamage = (input.PhysicalDamage + input.FireDamage) * input.GuardBreakModifier;
            float staminaAbsorption = totalDamage * (input.BlockingStabilityRating / 100f);
            result.StaminaDamage = totalDamage - staminaAbsorption;

            // 破防判定
            result.IsGuardBroken = (input.DefenderCurrentStamina - result.StaminaDamage) <= 0;

            return result;
        }
    }
}
