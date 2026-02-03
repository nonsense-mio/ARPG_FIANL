using UnityEngine;

namespace HT
{
    /// <summary>
    /// VFX特效管理器
    /// 作为独立系统，订阅EventCenter已有事件来播放对应特效
    /// </summary>
    public class VFXMgr : SingletonAutoMono<VFXMgr>
    {
        [Header("特效路径配置")]
        [SerializeField] string healFXPath = "FX/Recover";
        [SerializeField] string bloodFXPath = "FX/Blood";
        [SerializeField] string slashFXPath = "FX/Slash";
        [SerializeField] string phaseShiftFXPath = "FX/PhaseShift";

        private void OnEnable()
        {
            EventCenter.Instance.AddEventListener<Transform>(E_EventType.E_BombHit, BombHitFX);
            EventCenter.Instance.AddEventListener<CharacterManager>(E_EventType.E_Slash, OnSlash);
            EventCenter.Instance.AddEventListener<Vector3>(E_EventType.E_FireBallHit, FireBallHitFX);
            EventCenter.Instance.AddEventListener<Vector3>(E_EventType.E_Character_Damage, OnCharacterDamage);
            EventCenter.Instance.AddEventListener<EnemyManager>(E_EventType.E_BossPhaseShift, OnBossPhaseShift);
            EventCenter.Instance.AddEventListener<PlayerManager>(E_EventType.E_Player_DrinkPotion, OnPlayerHeal);
            EventCenter.Instance.AddEventListener<(CharacterManager, SpellItem)>(E_EventType.E_Player_CastSpell, CastSpellFX);
            EventCenter.Instance.AddEventListener<(CharacterManager, SpellItem)>(E_EventType.E_Player_WarmUpSpell, WarmUpSpellFX);
        }

        private void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<Transform>(E_EventType.E_BombHit, BombHitFX);
            EventCenter.Instance.RemoveEventListener<CharacterManager>(E_EventType.E_Slash, OnSlash);
            EventCenter.Instance.RemoveEventListener<Vector3>(E_EventType.E_FireBallHit, FireBallHitFX);
            EventCenter.Instance.RemoveEventListener<Vector3>(E_EventType.E_Character_Damage, OnCharacterDamage);
            EventCenter.Instance.RemoveEventListener<EnemyManager>(E_EventType.E_BossPhaseShift, OnBossPhaseShift);
            EventCenter.Instance.RemoveEventListener<PlayerManager>(E_EventType.E_Player_DrinkPotion, OnPlayerHeal);
            EventCenter.Instance.RemoveEventListener<(CharacterManager, SpellItem)>(E_EventType.E_Player_CastSpell, CastSpellFX);
            EventCenter.Instance.RemoveEventListener<(CharacterManager, SpellItem)>(E_EventType.E_Player_WarmUpSpell, WarmUpSpellFX);

        }

        public void Init()
        {

        }


        /// <summary>
        /// 响应治疗事件 - 播放治疗特效
        /// </summary>
        private void OnPlayerHeal(PlayerManager player)
        {
            GameObject fx = PoolMgr.Instance.GetObj(healFXPath);
            fx.transform.SetParent(player.transform);
            fx.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 响应受伤事件
        /// </summary>
        private void OnCharacterDamage(Vector3 hitPoint)
        {

            GameObject blood = PoolMgr.Instance.GetObj(bloodFXPath);
            blood.transform.position = hitPoint;
            blood.transform.rotation = Quaternion.identity;
            blood.transform.localScale = Vector3.one * 0.5f;

        }

        /// <summary>
        /// 响应挥砍事件 - 播放武器拖尾/剑气特效
        /// </summary>
        private void OnSlash(CharacterManager character)
        {
            string currentSlashPath = slashFXPath;
            if (character is EnemyManager)
            {
                if ((character as EnemyManager).enemyStatsManager.hasPublishedPhaseShift)
                {
                    currentSlashPath = "FX/Slash_Boss";
                }
            }
            else
            {
                currentSlashPath = "FX/Slash";
            }
            GameObject slash = PoolMgr.Instance.GetObj(currentSlashPath);
            slash.transform.position = character.transform.position + character.transform.up * 1.0f;
            slash.transform.rotation = character.transform.rotation;
            slash.transform.localScale = Vector3.one * 0.5f;
        }

        //Boss 进入二阶段特效
        private void OnBossPhaseShift(EnemyManager boss)
        {
            GameObject phaseShiftFX = PoolMgr.Instance.GetObj(phaseShiftFXPath);
            phaseShiftFX.transform.SetParent(boss.transform);
            phaseShiftFX.transform.localPosition = Vector3.zero + Vector3.up * 0.2f;
            phaseShiftFX.transform.localRotation = Quaternion.identity;
        }


        // 播放施法特效
        private void WarmUpSpellFX((CharacterManager, SpellItem) args)
        {
            //根据法术类型播放不同特效
            if (args.Item2 is HealingSpell)
            {
                GameObject fx = PoolMgr.Instance.GetObj(args.Item2.spellWarmUpFXName);
                fx.transform.SetParent(args.Item1.transform);
                fx.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            }
            else if (args.Item2 is ProjectileSpell)
            {
                GameObject fx = PoolMgr.Instance.GetObj(args.Item2.spellWarmUpFXName);
                if (args.Item1.isUsingRightHand)
                    fx.transform.SetParent(args.Item1.characterWeaponSlotManager.rightHandSlot.transform);
                else
                    fx.transform.SetParent(args.Item1.characterWeaponSlotManager.leftHandSlot.transform);

                fx.transform.localPosition = Vector3.zero;
                fx.transform.localScale = Vector3.one * 100;
            }
        }
        //施法特效
        private void CastSpellFX((CharacterManager, SpellItem) args)
        {
            //治疗法术
            if (args.Item2 is HealingSpell)
            {
                GameObject fx = PoolMgr.Instance.GetObj(args.Item2.spellCastFXName);
                fx.transform.SetParent(args.Item1.transform);
                fx.transform.localPosition = Vector3.zero;
                fx.transform.localScale = Vector3.one * 0.5f;
            }
            //如果是投射物法术
            else if (args.Item2 is ProjectileSpell)
            {
                ProjectileSpell ps = args.Item2 as ProjectileSpell;
                GameObject fx = PoolMgr.Instance.GetObj(args.Item2.spellCastFXName);
                PlayerManager player = args.Item1 as PlayerManager;
                if (args.Item1.isUsingRightHand)
                    fx.transform.position = args.Item1.characterWeaponSlotManager.rightHandSlot.transform.position;
                else
                    fx.transform.position = args.Item1.characterWeaponSlotManager.leftHandSlot.transform.position;

                fx.transform.rotation = player.cameraMgr.cameraPivotTransform.rotation;
                fx.transform.rotation = Quaternion.Euler(player.cameraMgr.cameraPivotTransform.eulerAngles.x, player.cameraMgr.cameraPivotTransform.eulerAngles.y, 0);


                SpellDamageCollider damageCollider = fx.GetComponent<SpellDamageCollider>();
                damageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;
                damageCollider.characterManager = player;

                // 获取刚体 (局部变量)
                Rigidbody rb = fx.GetComponent<Rigidbody>();

                SpellDamageCollider spellDamageCollider = fx.GetComponent<SpellDamageCollider>();
                spellDamageCollider.fireDamage = (int)ps.baseDamage;
                spellDamageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;

                // 施加力
                rb.AddForce(fx.transform.forward * ps.projectileForwardVelocity);
                rb.AddForce(fx.transform.up * ps.projectileUpWardVelocity);
            }
        }


        private void FireBallHitFX(Vector3 hitPoint)
        {
            GameObject fx = PoolMgr.Instance.GetObj("FX/FireBallImpact");
            fx.transform.position = hitPoint;
            fx.transform.rotation = Quaternion.LookRotation(-Vector3.up);
        }

        private void BombHitFX(Transform bombTransform)
        {
            GameObject fx = PoolMgr.Instance.GetObj("FX/BombFX");
            fx.transform.position = bombTransform.position;
            fx.transform.rotation = Quaternion.identity;
        }
    }
}
