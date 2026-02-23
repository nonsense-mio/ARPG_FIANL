using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class EnemyWeaponSlotManager : CharacterWeaponSlotManager
    {
        EnemyManager enemy;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            enemy = characterMgr as EnemyManager;
        }
        public override void GrandWeaponAttackingPoiseBonus()
        {
            character.characterStatsManager.totalPoiseDefense += character.characterStatsManager.offensivePoiseBonus;
        }

        /// <summary>
        /// 销毁所有武器槽的武器模型（对象池回收时调用，防止脏武器残留）
        /// </summary>
        public void UnloadAllWeaponModels()
        {
            leftHandSlot?.UnloadWeaponAndDestory();
            rightHandSlot?.UnloadWeaponAndDestory();
            backSlot?.UnloadWeaponAndDestory();
        }

    }
}

