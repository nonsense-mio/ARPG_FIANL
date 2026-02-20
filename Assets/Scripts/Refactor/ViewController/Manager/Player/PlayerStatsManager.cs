using UnityEngine;
using Framework;

namespace ARPG
{
    //玩家状态类
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;
        private IPlayerModel playerModel;
        public float staminaRegenerationAmount = 1;
        public float staminaRegenerationAmountWhileBlocking = 0.1f;
        private float staminaRegenTimer = 0;
        private float sprintingTimer = 0;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            player = characterMgr as PlayerManager;
            playerModel = this.GetModel<IPlayerModel>();

            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            maxFocus = SetMaxFocusPointsFromFocusLevel();
            currentFocus = maxFocus;

            SyncAllStatsToModel();
        }

        void Start()
        {

        }

        public void FullPlayerStats()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            currentFocus = maxFocus;

            SyncAllStatsToModel();
        }
        //同步状态数据 到playerModel
        private void SyncAllStatsToModel()
        {
            if (playerModel == null) return;
            playerModel.MaxHP.Value = maxHealth;
            playerModel.CurrentHP.Value = currentHealth;
            playerModel.MaxStamina.Value = (int)maxStamina;
            playerModel.CurrentStamina.Value = (int)currentStamina;
            playerModel.MaxFocus.Value = (int)maxFocus;
            playerModel.CurrentFocus.Value = (int)currentFocus;
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

            if (playerModel != null) { playerModel.CurrentHP.Value = currentHealth; }

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

            if (playerModel != null) { playerModel.CurrentHP.Value = currentHealth; }
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

            if (playerModel != null) { playerModel.CurrentHP.Value = currentHealth; }
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
            if (playerModel != null) { playerModel.CurrentStamina.Value = (int)currentStamina; }
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
                    if (playerModel != null) { playerModel.CurrentStamina.Value = (int)currentStamina; }
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
                    }
                    else
                    {
                        currentStamina += staminaRegenerationAmount * Time.deltaTime;
                    }
                    if (playerModel != null) { playerModel.CurrentStamina.Value = (int)currentStamina; }
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
            if (playerModel != null) { playerModel.CurrentHP.Value = currentHealth; }
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
            if (playerModel != null) { playerModel.CurrentFocus.Value = (int)currentFocus; }
        }
        public void AddFocusPoints(int focusPoints)
        {
            currentFocus += focusPoints;
            if (currentFocus > maxFocus)
            {
                currentFocus = maxFocus;
            }
            if (playerModel != null) { playerModel.CurrentFocus.Value = (int)currentFocus; }
        }

        //加魂
        public void AddSouls(CharacterManager enemy)
        {
            if (teamIDNumber == enemy.characterStatsManager.teamIDNumber)
                return;
            currentSoulCount += enemy.characterStatsManager.soulsAwardedOnDeath;
            if (playerModel != null) { playerModel.CurrentSoulCount.Value = currentSoulCount; }
        }
        public void SpendSouls(int amount)
        {
            currentSoulCount -= amount;
            if (currentSoulCount < 0)
                currentSoulCount = 0;
            if (playerModel != null) { playerModel.CurrentSoulCount.Value = currentSoulCount; }
        }

        protected override void HandleDeath()
        {
            base.HandleDeath();
            playerModel.CurrentHP.Value = 0;
            this.GetSystem<ITimerSystem>().CreateTimer(false, 3f,
                () => this.SendCommand(new RespawnPlayerCommand()));
        }
    }
}


