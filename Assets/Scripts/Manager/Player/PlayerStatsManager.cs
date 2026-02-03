using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    //玩家状态类
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;
        public float staminaRegenerationAmount = 1;
        public float staminaRegenerationAmountWhileBlocking = 0.1f;
        private float staminaRegenTimer = 0;
        private float sprintingTimer = 0;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            player = characterMgr as PlayerManager;
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            maxFocus = SetMaxFocusPointsFromFocusLevel();
            currentFocus = maxFocus;


            EventCenter.Instance.EventTrigger(E_EventType.E_Player_Init_UI);
        }

        void Start()
        {

        }

        public void FullPlayerStats()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            currentFocus = maxFocus;

            //发布玩家初始化UI事件
            EventCenter.Instance.EventTrigger(E_EventType.E_Player_Init_UI);
        }


        public override void HandlePoiseResetTimer()
        {
            if (poiseResetTimer > 0)
            {
                poiseResetTimer -= Time.deltaTime;
            }
            else if (poiseResetTimer <= 0 && !player.isInteracting)
            {
                totalPoiseDefense = armorPoiseBonus;
            }
        }


        /// <summary>
        /// 受伤
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int physicalDamage, int fireDamage, string damageAnimation, CharacterManager enemyCharacterDamagingMe)
        {
            base.TakeDamage(physicalDamage, fireDamage, damageAnimation, enemyCharacterDamagingMe);
            if (player.isInvulnerable)
                return;
            if (player.isDead)
                return;

            // 状态事件：给 UI/数值联动用
            EventCenter.Instance.EventTrigger<StatChanged>(
                E_EventType.E_Player_StatChanged,
                new StatChanged(E_PlayerStatType.Health, currentHealth, maxHealth)
            );


            player.playerAnimatorManager.PlayTargetAnimation(damageAnimation, true);
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }

        /// <summary>
        /// 受伤时不播放其他动画 主要用于特定动画的使用
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamageNoAnimation(int physicalDamage, int fireDamage, CharacterManager enemyCharacterDamagingMe)
        {
            base.TakeDamageNoAnimation(physicalDamage, fireDamage, enemyCharacterDamagingMe);

            // 状态事件：给 UI/数值联动用
            EventCenter.Instance.EventTrigger<StatChanged>(
                E_EventType.E_Player_StatChanged,
                new StatChanged(E_PlayerStatType.Health, currentHealth, maxHealth)
            );
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }

        public override void TakePoiseDamage(int damage)
        {
            base.TakePoiseDamage(damage);
            if (player.isDead)
                return;

            // 状态事件：给 UI/数值联动用
            EventCenter.Instance.EventTrigger<StatChanged>(
                E_EventType.E_Player_StatChanged,
                new StatChanged(E_PlayerStatType.Health, currentHealth, maxHealth)
            );
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }

        /// <summary>
        /// 体力消耗
        /// </summary>
        /// <param name="damage"></param>
        public void DeductStamina(float stamina)
        {
            currentStamina -= stamina;
            if (currentStamina < 0)
            {
                currentStamina = 0;
            }
            //发布玩家体力消耗事件
            // 状态事件
            EventCenter.Instance.EventTrigger<StatChanged>(
                E_EventType.E_Player_StatChanged,
                new StatChanged(E_PlayerStatType.Stamina, (int)currentStamina, (int)maxStamina)
            );
        }
        public void DeductSprintingStamina(float stamina)
        {
            if (player.isSprinting)
            {
                sprintingTimer += Time.deltaTime;
                if (sprintingTimer >= 0.1f)
                {
                    currentStamina -= stamina;
                    sprintingTimer = 0;
                    //发布玩家体力消耗事件
                    // 状态事件
                    EventCenter.Instance.EventTrigger<StatChanged>(
                        E_EventType.E_Player_StatChanged,
                        new StatChanged(E_PlayerStatType.Stamina, (int)currentStamina, (int)maxStamina)
                    );
                }
            }
            else
            {
                sprintingTimer = 0;
            }
        }
        /// <summary>
        /// 体力回复
        /// </summary>
        public void RegenerateStamina()
        {
            //当交互或冲刺时 重置计时器 不要回复体力
            if (player.isInteracting || player.isSprinting)
            {
                staminaRegenTimer = 0;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;

                if (currentStamina < maxStamina && staminaRegenTimer > 1f)
                {
                    if (player.isBlocking)
                    {
                        currentStamina += staminaRegenerationAmountWhileBlocking * Time.deltaTime;
                        //回复体力时也更新UI显示
                        // 状态事件
                        EventCenter.Instance.EventTrigger<StatChanged>(
                            E_EventType.E_Player_StatChanged,
                            new StatChanged(E_PlayerStatType.Stamina, (int)currentStamina, (int)maxStamina)
                        );
                    }
                    else
                    {
                        currentStamina += staminaRegenerationAmount * Time.deltaTime;
                        //回复体力时也更新UI显示
                        // 状态事件
                        EventCenter.Instance.EventTrigger<StatChanged>(
                            E_EventType.E_Player_StatChanged,
                            new StatChanged(E_PlayerStatType.Stamina, (int)currentStamina, (int)maxStamina)
                        );
                    }


                }
            }

        }

        /// <summary>
        /// 治疗玩家
        /// </summary>
        /// <param name="healAmount"></param>
        public override void HealCharacter(int healAmount)
        {
            base.HealCharacter(healAmount);
            //发布玩家更新生命值事件
            // 状态事件：给 UI/数值联动用
            EventCenter.Instance.EventTrigger<StatChanged>(
                E_EventType.E_Player_StatChanged,
                new StatChanged(E_PlayerStatType.Health, currentHealth, maxHealth)
            );
        }
        /// <summary>
        /// 减少focuspoints
        /// </summary>
        /// <param name="focusPoints"></param>
        public void DeductFocusPoints(int focusPoints)
        {
            currentFocus -= focusPoints;
            if (currentFocus < 0)
            {
                currentFocus = 0;
            }
            //发布玩家专注消耗事件
            // 状态事件
            EventCenter.Instance.EventTrigger<StatChanged>(
                E_EventType.E_Player_StatChanged,
                new StatChanged(E_PlayerStatType.Focus, (int)currentFocus, (int)maxFocus)
            );

        }
        public void AddFocusPoints(int focusPoints)
        {
            currentFocus += focusPoints;
            if (currentFocus > maxFocus)
            {
                currentFocus = maxFocus;
            }
            // 状态事件
            EventCenter.Instance.EventTrigger<StatChanged>(
                E_EventType.E_Player_StatChanged,
                new StatChanged(E_PlayerStatType.Focus, (int)currentFocus, (int)maxFocus)
            );

        }

        //加魂
        public void AddSouls(CharacterManager enemy)
        {
            if (teamIDNumber == enemy.characterStatsManager.teamIDNumber)
                return;
            currentSoulCount += enemy.characterStatsManager.soulsAwardedOnDeath;
            EventCenter.Instance.EventTrigger(E_EventType.E_Player_Update_SoulCount_UI);
        }
        public void SpendSouls(int amount)
        {
            currentSoulCount -= amount;
            if (currentSoulCount < 0)
                currentSoulCount = 0;
            EventCenter.Instance.EventTrigger(E_EventType.E_Player_Update_SoulCount_UI);
        }

        protected override void HandleDeath()
        {
            base.HandleDeath();
            StartCoroutine(RespawnAfterDelay(3f));
        }
        private IEnumerator RespawnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            RespawnAtLastBonfire();
        }

        public void RespawnAtLastBonfire()
        {
            var data = CurrentGameDataMgr.Instance.playerData;

            // 如果没有激活过任何篝火，使用默认出生点
            if (string.IsNullOrEmpty(data.lastRestedBonfireID))
            {
                Debug.LogWarning("[PlayerStatsManager] 没有激活的篝火，使用默认位置");
                // 可以设置一个默认出生点
                return;
            }

            // 传送到复活点
            Vector3 respawnPosition = new Vector3(data.respawnX, data.respawnY, data.respawnZ);
            player.SetPlayerPosition(respawnPosition);

            // 重置玩家状态
            player.isDead = false;
            FullPlayerStats();

            // 播放复活动画（可选）
            player.playerAnimatorManager.PlayTargetAnimation("Get up", true);

            Debug.Log($"[PlayerStatsManager] 在篝火 {data.lastRestedBonfireID} 复活");
        }
    }
}


