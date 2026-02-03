using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class CompanionStateCombatStance : State
    {
        CompanionStateAttack attackState;
        CompanionStatePursueTarget pursueTargetState;
        CompanionStateFollowHost followHostState;
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
        private void Awake()
        {
            attackState = GetComponent<CompanionStateAttack>();
            pursueTargetState = GetComponent<CompanionStatePursueTarget>();
            followHostState = GetComponent<CompanionStateFollowHost>();
        }

        public override State Tick(EnemyManager enemy)
        {
            if (enemy.currentTarget == null)
            {
                return followHostState;
            }
            //如果与友军距离过远 则返回跟随状态
            if (enemy.distanceFromCompanion > enemy.maxDistanceFromCompanion)
            {
                return followHostState;
            }

            if (enemy.combatStyle == E_AICombatStyle.SwordAndShield)
            {
                return ProcessSwordAndShieldCombat(enemy);
            }
            else if (enemy.combatStyle == E_AICombatStyle.Archer)
            {
                return ProcessArcherCombat(enemy);
            }
            else
            {
                return this;
            }

        }


        #region 剑盾战斗方式
        //剑盾战斗姿态
        private State ProcessSwordAndShieldCombat(EnemyManager enemy)
        {

            //如果敌人不在地面上 或者 正在交互中 则不做任何操作
            if (!enemy.isGrounded || enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }

            //当目标距离大于攻击距离 返回追击状态
            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                return pursueTargetState;
            }
            //AI向目标随机移动
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemy);
            }

            if (enemy.allowToPerformParry)
            {
                //敌人当前目标可以被反击
                if (enemy.currentTarget != null && enemy.currentTarget.canBeRiposted)
                {
                    //执行反击
                    CheckForRiposte(enemy);
                }
            }


            if (enemy.allowToPerformBlock)
            {
                RollForBlockChance(enemy);
            }

            if (enemy.allowToPerformDodge)
            {
                RollForDodgeChance(enemy);
            }

            if (enemy.allowToPerformParry)
            {
                RollForParryChance(enemy);
            }
            //执行格挡
            if (willPerformBlock)
            {
                if(enemy.enemyStatsManager.currentStamina > 0)
                    BlockUsingOffHand(enemy);
            }
            //执行闪避
            if (willPerformDodge && enemy.currentTarget.isAttacking)
            {
                Dodge(enemy);
            }



            if (enemy.currentTarget.isAttacking)
            {
                //执行招架
                if (willPerformParry && !hasPerformedParry)
                {
                    ParryCurrentTarget(enemy);
                    return this;
                }
            }


            //检查攻击距离
            //进入战斗姿态
            HandleRotateTowardsTarget(enemy);
            //当攻击冷却时间结束 且 当前攻击不为null时 进入攻击状态
            if (enemy.currentRecoveryTime <= 0 && attackState.currentAttack != null)
            {
                //重置状态标志
                ResetStateFlags();
                return attackState;
            }
            //返回当前状态 战斗姿态
            else
            {
                GetNewAttack(enemy);
            }
            CheckIfTooCloseToTarget(enemy);
            return this;
        }
        #endregion
        #region 弓箭手战斗方式
        //弓箭手战斗姿态
        private State ProcessArcherCombat(EnemyManager enemy)
        {
            //如果敌人不在地面上 或者 正在交互中 则不做任何操作
            if (!enemy.isGrounded || enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }

            //当目标距离大于攻击距离 返回追击状态
            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                ResetStateFlags();
                return pursueTargetState;
            }
            //AI向目标随机移动
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemy);
            }


            if (enemy.allowToPerformDodge)
            {
                RollForDodgeChance(enemy);
            }

            //执行闪避
            if (willPerformDodge && enemy.currentTarget.isAttacking)
            {
                Dodge(enemy);
            }

            HandleRotateTowardsTarget(enemy);
            if (!hasAmmoLoaded)
            {
                DrawArrow(enemy);
                AimAtTargetBeforeShooting(enemy);
                return this;
            }
            //当攻击冷却时间结束 且 弹药已装填时 进入攻击状态
            if (enemy.currentRecoveryTime <= 0 && hasAmmoLoaded)
            {
                //重置状态标志
                ResetStateFlags();
                return attackState;
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
        #region  移动相关
        protected void HandleRotateTowardsTarget(EnemyManager enemy)
        {

            Vector3 direction = enemy.currentTarget.transform.position - enemy.transform.position;
            direction.y = 0;
            direction.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed / Time.deltaTime);

        }
        //决定绕目标走动的方式
        protected void DecideCirclingAction(EnemyManager enemy)
        {

            WalkAroundTarget(enemy);
        }
        //绕目标走动
        protected void WalkAroundTarget(EnemyManager enemy)
        {
            verticalMovementValue = 0.5f;
            horizontalMovementValue = Random.Range(-1, 1);
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

        /// <summary>
        /// 得到攻击的方法
        /// </summary>
        protected virtual void GetNewAttack(EnemyManager enemy)
        {
            int maxScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                ItemBasedAttackAction enemyAttackAction = enemyAttacks[i];

                //到达攻击范围内
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

                //到达攻击范围内 得到攻击
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
                            attackState.hasperformedAttack = false;
                        }
                    }
                }
            }
        }

        //决定格挡/闪避/招架的几率
        private void RollForBlockChance(EnemyManager enemy)
        {
            int blockChance = Random.Range(0, 100);
            if (blockChance <= enemy.blockLikelyHood)
            {
                willPerformBlock = true;
            }
            else
            {
                willPerformBlock = false;
            }
        }
        private void RollForDodgeChance(EnemyManager enemy)
        {
            int dodgeChance = Random.Range(0, 100);
            if (dodgeChance <= enemy.dodgeLikelyHood)
            {
                willPerformDodge = true;
            }
            else
            {
                willPerformDodge = false;
            }
        }
        private void RollForParryChance(EnemyManager enemy)
        {
            int parryChance = Random.Range(0, 100);
            if (parryChance <= enemy.parryLikelyHood)
            {
                willPerformParry = true;
            }
            else
            {
                willPerformParry = false;
            }
        }


        //重置状态标志 当退出当前状态时调用
        private void ResetStateFlags()
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

        //AI Actions
        //执行使用副手武器格挡
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
        #region 闪避相关
        //执行闪避
        private void Dodge(EnemyManager enemy)
        {
            if (!hasPerformedDodge)
            {
                if (!hasRandomDodgeDirection)
                {
                    float randomDodgeDirction;

                    hasRandomDodgeDirection = true;
                    randomDodgeDirction = Random.Range(0, 360);
                    targetDodgeDirection = Quaternion.Euler(enemy.transform.eulerAngles.x, randomDodgeDirction, enemy.transform.eulerAngles.z);
                }

                if (enemy.transform.rotation != targetDodgeDirection)
                {
                    enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetDodgeDirection, 1f);

                    float targetYRotation = targetDodgeDirection.eulerAngles.y;
                    float currentYRotation = enemy.transform.eulerAngles.y;
                    float rotationDifference = Mathf.Abs(targetYRotation - currentYRotation);

                    if (rotationDifference <= 5f)
                    {
                        hasPerformedDodge = true;
                        enemy.transform.rotation = targetDodgeDirection;
                        if (Random.Range(0, 100) < 50)
                            enemy.enemyAnimatorManager.PlayTargetAnimation("Rolling", true);
                        else
                            enemy.enemyAnimatorManager.PlayTargetAnimation("Backstep", true);
                    }
                }
                else
                {

                }

            }
        }
        #endregion
        #region 射箭相关
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
            //瞄准时间
            float aimTime = Random.Range(enemy.minimumTimeToAimAtTarget, enemy.maximumTimeToAimAtTarget);
            enemy.currentRecoveryTime = aimTime;
        }

        #endregion

        #region Parry
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
                    //执行反击
                    enemy.characterCombatManager.AttemptBackStabOrRiposte();
                }
            }
        }
        #endregion
    }
}


