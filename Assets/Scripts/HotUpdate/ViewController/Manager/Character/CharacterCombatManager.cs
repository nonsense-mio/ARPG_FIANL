using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class CharacterCombatManager : ARPGController
    {
        CharacterManager character;
        Rigidbody characterRigidbody;
        [Header("Combat Transform")]
        public Transform backStabReceiverTransform;
        public Transform riposteReceiverTransform;
        public LayerMask characterLayer;
        public float criticalAttackRange = 0.7f;
        [Header("当前攻击类型")]
        public E_AttackType currentAttackType;


        [Header("攻击动画")]
        public string oh_light_attack_01 = "OH_Light_Attack_01";
        public string oh_light_attack_02 = "OH_Light_Attack_02";
        public string oh_light_attack_03 = "OH_Light_Attack_03";
        public string oh_heavy_attack_01 = "OH_Heavy_Attack_01";
        public string oh_heavy_attack_02 = "OH_Heavy_Attack_02";
        public string oh_running_attack_01 = "OH_Running_Attack_01";
        public string oh_jumping_attack_01 = "OH_Jumping_Attack_01";
        public string oh_charge_attack_01 = "OH_Charging_Attack_Charge_01";
        public string oh_charge_attack_02 = "OH_Charging_Attack_Charge_02";

        public string th_light_attack_01 = "TH_Light_Attack_01";
        public string th_light_attack_02 = "TH_Light_Attack_02";
        public string th_heavy_attack_01 = "TH_Heavy_Attack_01";
        public string th_heavy_attack_02 = "TH_Heavy_Attack_02";
        public string th_running_attack_01 = "TH_Running_Attack_01";
        public string th_jumping_attack_01 = "TH_Jumping_Attack_01";
        public string th_charge_attack_01 = "TH_Charging_Attack_Charge_01";
        public string th_charge_attack_02 = "TH_Charging_Attack_Charge_02";

        public string weapon_art = "Weapon Art";
        public string lastAttack;
        public float pendingCriticalDamage;
        public void Init(CharacterManager charMgr)
        {
            character = charMgr;
            characterRigidbody = charMgr.GetComponent<Rigidbody>();
        }

        public virtual void DrainStaminaBasedOnAttackType()
        {

        }

        //设置格挡吸收属性
        public virtual void SetBlockingAbsorptions()
        {
            if (character.isUsingRightHand)
            {
                character.characterStatsManager.blockingPhysicalDamageAbsorption = character.characterInventoryManager.rightWeapon.physicalBlockingDamageAbsorption;
                character.characterStatsManager.blockingFireDamageAbsorption = character.characterInventoryManager.rightWeapon.fireBlockingDamageAbsorption;
                character.characterStatsManager.blockingStabilityRating = character.characterInventoryManager.rightWeapon.stability;
            }
            else if (character.isUsingLeftHand)
            {
                character.characterStatsManager.blockingPhysicalDamageAbsorption = character.characterInventoryManager.leftWeapon.physicalBlockingDamageAbsorption;
                character.characterStatsManager.blockingFireDamageAbsorption = character.characterInventoryManager.leftWeapon.fireBlockingDamageAbsorption;
                character.characterStatsManager.blockingStabilityRating = character.characterInventoryManager.leftWeapon.stability;
            }
        }




        private void SuccessfullyCastSpell()
        {
            character.characterInventoryManager.currentSpell.SuccessfullyCastSpell(character);
            character.animator.SetBool("isFiringSpell", true);
        }

        //强制在被背刺时移动到背刺位置
        IEnumerator ForceMoveWhenBeingBackstabbed(CharacterManager characterPerformingBackstab)
        {
            for (float timer = 0.05f; timer < 0.5f; timer += 0.05f)
            {

                Quaternion backStabRotation = Quaternion.LookRotation(characterPerformingBackstab.transform.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, backStabRotation, 1);
                transform.parent = characterPerformingBackstab.characterCombatManager.backStabReceiverTransform;
                transform.localPosition = characterPerformingBackstab.characterCombatManager.backStabReceiverTransform.localPosition;
                transform.parent = null;
                yield return new WaitForSeconds(0.05f);
            }
        }
        //被背刺
        public void GetBackStabbed(CharacterManager characterPerformingBackstab)
        {
            //设置被背刺状态为true
            character.isBeingBackstabbed = true;
            // 冻结物理，防止箭矢等外力将目标打飞
            if (characterRigidbody != null) characterRigidbody.isKinematic = true;
            //锁定位置
            StartCoroutine(ForceMoveWhenBeingBackstabbed(characterPerformingBackstab));
            //播放被背刺动画
            character.characterAnimatorManager.PlayTargetAnimation("Back Stabbed", true);

        }

        //强制在被招架时移动到招架位置
        IEnumerator ForceMoveWhenBeingRiposted(CharacterManager characterPerformingRiposte)
        {
            for (float timer = 0.05f; timer < 0.5f; timer += 0.05f)
            {

                Quaternion riposteRotation = Quaternion.LookRotation(-characterPerformingRiposte.transform.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, riposteRotation, 1);
                transform.parent = characterPerformingRiposte.characterCombatManager.riposteReceiverTransform;
                transform.localPosition = characterPerformingRiposte.characterCombatManager.riposteReceiverTransform.localPosition;
                transform.parent = null;
                yield return new WaitForSeconds(0.05f);
            }
        }

        //被招架
        public void GetRiposted(CharacterManager characterPerformingRiposte)
        {
            //设置被招架状态为true
            character.isBeingRiposted = true;
            // 冻结物理，防止箭矢等外力将目标打飞
            if (characterRigidbody != null) characterRigidbody.isKinematic = true;
            //锁定位置
            StartCoroutine(ForceMoveWhenBeingRiposted(characterPerformingRiposte));
            //播放被招架动画
            character.characterAnimatorManager.PlayTargetAnimation("Riposted", true);

        }


        /// <summary>
        /// 尝试进行背刺或招架
        /// </summary>
        public void AttemptBackStabOrRiposte()
        {
            if (character.isInteracting)
                return;
            if (character.characterStatsManager.currentStamina <= 0)
                return;
            RaycastHit hit;
            if (Physics.Raycast(character.criticalAttackRayCastStartPoint.position, character.transform.TransformDirection(Vector3.forward), out hit, criticalAttackRange, characterLayer))
            {
                CharacterManager enemyCharacter = hit.transform.GetComponent<CharacterManager>();
                Vector3 directionFromAttackerToEnemy = transform.position - enemyCharacter.transform.position;
                directionFromAttackerToEnemy.Normalize();
                //点乘 计算敌人面向与攻击者方向的夹角
                float dotValue = Vector3.Dot(directionFromAttackerToEnemy, enemyCharacter.transform.forward);
                Debug.Log("点乘值：" + dotValue);
                if (enemyCharacter.canBeRiposted)
                {
                    if (dotValue <= 1.2f && dotValue >= 0.6f)
                    {
                        HandleRiposte(hit);
                        return;
                    }
                }
                if (dotValue >= -1f && dotValue <= -0.6f)
                {
                    HandleBackStab(hit);
                }
            }
        }


        private void HandleBackStab(RaycastHit hit)
        {
            CharacterManager enemyCharacter = hit.transform.GetComponent<CharacterManager>();
            if (enemyCharacter != null)
            {
                if (!enemyCharacter.isBeingBackstabbed || !enemyCharacter.isBeingRiposted)
                {

                    EnableIsInvulnerable();
                    character.isPerformingBackstab = true;
                    character.characterAnimatorManager.EraseHandIKForWeapon();

                    character.characterAnimatorManager.PlayTargetAnimation("Back Stab", true);

                    var weapon = character.characterInventoryManager.rightWeapon;
                    int criticalDamage = this.SendQuery(new GetCriticalDamageQuery(
                        weapon.criticalDamageMultiplier, weapon.physicalDamage, weapon.fireDamage));
                    enemyCharacter.characterCombatManager.pendingCriticalDamage = criticalDamage;
                    enemyCharacter.characterCombatManager.GetBackStabbed(character);
                }
            }
        }

        private void HandleRiposte(RaycastHit hit)
        {
            CharacterManager enemyCharacter = hit.transform.GetComponent<CharacterManager>();
            if (enemyCharacter != null)
            {
                if (!enemyCharacter.isBeingBackstabbed || !enemyCharacter.isBeingRiposted)
                {

                    EnableIsInvulnerable();
                    character.isPerformingRiposte = true;
                    character.characterAnimatorManager.EraseHandIKForWeapon();

                    character.characterAnimatorManager.PlayTargetAnimation("Riposte", true);

                    var weapon = character.characterInventoryManager.rightWeapon;
                    int criticalDamage = this.SendQuery(new GetCriticalDamageQuery(
                        weapon.criticalDamageMultiplier, weapon.physicalDamage, weapon.fireDamage));
                    enemyCharacter.characterCombatManager.pendingCriticalDamage = criticalDamage;
                    enemyCharacter.characterCombatManager.GetRiposted(character);
                }
            }
        }

        private void EnableIsInvulnerable()
        {
            character.animator.SetBool("isInvulnerable", true);
        }

        //应用待处理的暴击伤害
        public void ApplyPendingDamage()
        {
            character.characterStatsManager.TakeDamageNoAnimation(Mathf.RoundToInt(pendingCriticalDamage), 0, character);
            // 触发命中音效和血液特效（与普通攻击走相同路径）
            Vector3 hitPoint = character.lockOnTransform != null
                ? character.lockOnTransform.position
                : character.transform.position + Vector3.up;
            this.SendEvent(new CharacterDamageEvent { HitPoint = hitPoint });
        }


        public void EnableCanBeParried()
        {
            character.canBeParried = true;
        }
        public void DisableCanBeParried()
        {
            character.canBeParried = false;
        }
    }

}
