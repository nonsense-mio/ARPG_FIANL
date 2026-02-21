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
   
    }
}

