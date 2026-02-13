using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class EnemyInventoryManager : CharacterInventoryManager
    {
        EnemyManager enemy;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            enemy = characterMgr as EnemyManager;
        }
    }

}
