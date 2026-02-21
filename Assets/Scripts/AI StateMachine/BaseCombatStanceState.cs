using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 战斗姿态状态的共享逻辑基类。
    /// 子类通过抽象/虚方法提供退出状态和 Companion 特有的行为差异。
    /// </summary>
    public abstract class BaseCombatStanceState : State
    {
        protected abstract BaseAttackState GetAttackState();
        protected abstract State GetPursueState();
        protected abstract State GetFallbackState();

        public ItemBasedAttackAction[] enemyAttacks;
        protected bool randomDestinationSet = false;
        protected float verticalMovementValue = 0;
        protected float horizontalMovementValue = 0;

        [Header("状态标志")]
        bool willPerformBlock = false;
        bool willPerformDodge = false;
        bool willPerformParry = false;

        bool hasPerformedDodge = false;
        bool hasRandomDodgeDirection = false;
        bool hasPerformedParry = false;
        Quaternion targetDodgeDirection;

        bool hasAmmoLoaded = false;

        /// <summary>
        /// 子类可重写此方法添加额外的前置退出检查（如 Companion 距离限制）。
        /// 返回 true 表示应该退出到 FallbackState。
        /// </summary>
        protected virtual bool ShouldExitToFallback(EnemyManager enemy) => false;

        /// <summary>
        /// 子类可重写此方法添加格挡的额外条件（如 Companion 体力检查）。
        /// </summary>
        protected virtual bool CanPerformBlock(EnemyManager enemy) => true;

        public override void OnExit(EnemyManager enemy)
        {
            hasPerformedDodge = false;
            hasRandomDodgeDirection = false;
            hasAmmoLoaded = false;
            hasPerformedParry = false;
            randomDestinationSet = false;
            willPerformBlock = false;
            willPerformDodge = false;
            willPerformParry = false;
        }

        public override State OnUpdate(EnemyManager enemy)
        {
            if (enemy.currentTarget == null)
            {
                State fallback = GetFallbackState();
                return fallback != null ? fallback : (State)this;
            }

            if (ShouldExitToFallback(enemy))
            {
                return GetFallbackState();
            }

            if (enemy.combatStyle == E_AICombatStyle.SwordAndShield)
                return ProcessSwordAndShieldCombat(enemy);
            else if (enemy.combatStyle == E_AICombatStyle.Archer)
                return ProcessArcherCombat(enemy);
            else
                return this;
        }

        #region 剑盾战斗方式
        private State ProcessSwordAndShieldCombat(EnemyManager enemy)
        {
            if (!enemy.isGrounded || enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
                return GetPursueState();

            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemy);
            }

            if (enemy.allowToPerformParry)
            {
                if (enemy.currentTarget != null && enemy.currentTarget.canBeRiposted)
                {
                    CheckForRiposte(enemy);
                }
            }

            if (enemy.allowToPerformBlock)
                RollForBlockChance(enemy);
            if (enemy.allowToPerformDodge)
                RollForDodgeChance(enemy);
            if (enemy.allowToPerformParry)
                RollForParryChance(enemy);

            if (willPerformBlock && CanPerformBlock(enemy))
                BlockUsingOffHand(enemy);

            if (willPerformDodge && enemy.currentTarget.isAttacking)
                Dodge(enemy);

            if (enemy.currentTarget != null && enemy.currentTarget.isAttacking)
            {
                if (willPerformParry && !hasPerformedParry)
                {
                    ParryCurrentTarget(enemy);
                    return this;
                }
            }

            HandleRotateTowardsTarget(enemy);

            BaseAttackState attackState = GetAttackState();
            if (enemy.currentRecoveryTime <= 0 && attackState.currentAttack != null)
            {
                return attackState;
            }
            else
            {
                GetNewAttack(enemy);
            }

            CheckIfTooCloseToTarget(enemy);
            return this;
        }
        #endregion

        #region 弓箭手战斗方式
        private State ProcessArcherCombat(EnemyManager enemy)
        {
            if (!enemy.isGrounded || enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                return GetPursueState();
            }

            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemy);
            }

            if (enemy.allowToPerformDodge)
                RollForDodgeChance(enemy);

            if (willPerformDodge && enemy.currentTarget.isAttacking)
                Dodge(enemy);

            HandleRotateTowardsTarget(enemy);

            if (!hasAmmoLoaded)
            {
                DrawArrow(enemy);
                AimAtTargetBeforeShooting(enemy);
                return this;
            }

            if (enemy.currentRecoveryTime <= 0 && hasAmmoLoaded)
            {
                return GetAttackState();
            }

            if (enemy.isStationaryArcher)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
            }
            else
            {
                CheckIfTooCloseToTarget(enemy);
            }

            return this;
        }
        #endregion

        #region 移动相关
        protected void HandleRotateTowardsTarget(EnemyManager enemy)
        {
            if (enemy.currentTarget == null) return;
            Vector3 direction = enemy.currentTarget.transform.position - enemy.transform.position;
            direction.y = 0;
            direction.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
        }

        protected void DecideCirclingAction(EnemyManager enemy)
        {
            WalkAroundTarget(enemy);
        }

        protected void WalkAroundTarget(EnemyManager enemy)
        {
            verticalMovementValue = 0.5f;
            horizontalMovementValue = Random.Range(-1f, 1f);
            if (horizontalMovementValue <= 1 && horizontalMovementValue >= 0)
            {
                horizontalMovementValue = 0.5f;
            }
            else if (horizontalMovementValue >= -1 && horizontalMovementValue < 0)
            {
                horizontalMovementValue = -0.5f;
            }
        }

        private void CheckIfTooCloseToTarget(EnemyManager enemy)
        {
            if (enemy.distanceFromTarget <= enemy.stoppingDistance)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);
            }
            else
            {
                enemy.animator.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);
            }
        }
        #endregion

        #region 攻击选择
        protected virtual void GetNewAttack(EnemyManager enemy)
        {
            if (enemyAttacks == null || enemyAttacks.Length == 0) return;

            BaseAttackState attackState = GetAttackState();
            int maxScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                ItemBasedAttackAction enemyAttackAction = enemyAttacks[i];
                if (enemy.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                   enemy.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (enemy.viewableAngle <= enemyAttackAction.maximumAttackAngle &&
                       enemy.viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        maxScore += enemyAttackAction.attckScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                ItemBasedAttackAction enemyAttackAction = enemyAttacks[i];
                if (enemy.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                   enemy.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (enemy.viewableAngle <= enemyAttackAction.maximumAttackAngle &&
                       enemy.viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        if (attackState.currentAttack != null)
                            return;
                        temporaryScore += enemyAttackAction.attckScore;
                        if (temporaryScore > randomValue)
                        {
                            attackState.currentAttack = enemyAttackAction;
                            attackState.hasPerformedAttack = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region 概率判定
        private void RollForBlockChance(EnemyManager enemy)
        {
            willPerformBlock = Random.Range(0, 100) <= enemy.blockLikelyHood;
        }

        private void RollForDodgeChance(EnemyManager enemy)
        {
            willPerformDodge = Random.Range(0, 100) <= enemy.dodgeLikelyHood;
        }

        private void RollForParryChance(EnemyManager enemy)
        {
            willPerformParry = Random.Range(0, 100) <= enemy.parryLikelyHood;
        }
        #endregion

        #region AI Actions
        private void BlockUsingOffHand(EnemyManager enemy)
        {
            if (enemy.isBlocking == false)
            {
                if (enemy.allowToPerformBlock)
                {
                    enemy.isBlocking = true;
                    enemy.characterInventoryManager.currentItemBeingUsed = enemy.characterInventoryManager.leftWeapon;
                    enemy.characterCombatManager.SetBlockingAbsorptions();
                }
            }
        }

        private void Dodge(EnemyManager enemy)
        {
            if (!hasPerformedDodge)
            {
                if (!hasRandomDodgeDirection)
                {
                    hasRandomDodgeDirection = true;
                    float randomDodgeDirection = Random.Range(0, 360);
                    targetDodgeDirection = Quaternion.Euler(enemy.transform.eulerAngles.x, randomDodgeDirection, enemy.transform.eulerAngles.z);
                }

                if (Quaternion.Angle(enemy.transform.rotation, targetDodgeDirection) > 1f)
                {
                    enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetDodgeDirection, 1f);

                    if (Quaternion.Angle(enemy.transform.rotation, targetDodgeDirection) <= 5f)
                    {
                        hasPerformedDodge = true;
                        enemy.transform.rotation = targetDodgeDirection;
                        if (Random.Range(0, 100) < 50)
                            enemy.enemyAnimatorManager.PlayTargetAnimation("Rolling", true);
                        else
                            enemy.enemyAnimatorManager.PlayTargetAnimation("Backstep", true);
                    }
                }
            }
        }

        private void DrawArrow(EnemyManager enemy)
        {
            if (!enemy.isTwoHandingWeapon)
            {
                enemy.isTwoHandingWeapon = true;
                enemy.characterWeaponSlotManager.LoadBothWeaponsOnSlots();
            }
            else
            {
                hasAmmoLoaded = true;
                enemy.characterInventoryManager.currentItemBeingUsed = enemy.characterInventoryManager.rightWeapon;
                enemy.characterInventoryManager.rightWeapon.th_hold_Mouse_L_Action.PerformAction(enemy);
            }
        }

        private void AimAtTargetBeforeShooting(EnemyManager enemy)
        {
            float aimTime = Random.Range(enemy.minimumTimeToAimAtTarget, enemy.maximumTimeToAimAtTarget);
            enemy.currentRecoveryTime = aimTime;
        }

        private void ParryCurrentTarget(EnemyManager enemy)
        {
            if (enemy.currentTarget.canBeParried)
            {
                if (enemy.distanceFromTarget <= 2)
                {
                    hasPerformedParry = true;
                    enemy.isParrying = true;
                    enemy.enemyAnimatorManager.PlayTargetAnimation("Parry", true);
                }
            }
        }

        private void CheckForRiposte(EnemyManager enemy)
        {
            if (enemy.isInteracting)
            {
                enemy.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                return;
            }
            if (enemy.distanceFromTarget >= 1)
            {
                HandleRotateTowardsTarget(enemy);
                enemy.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Vertical", 1, 0.2f, Time.deltaTime);
            }
            else
            {
                enemy.isBlocking = false;
                if (!enemy.isInteracting && !enemy.currentTarget.isBeingRiposted && !enemy.currentTarget.isBeingBackstabbed)
                {
                    enemy.enemyRigidbody.velocity = Vector3.zero;
                    enemy.animator.SetFloat("Vertical", 0);
                    enemy.characterCombatManager.AttemptBackStabOrRiposte();
                }
            }
        }
        #endregion
    }
}
