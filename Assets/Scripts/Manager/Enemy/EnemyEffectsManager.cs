using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class EnemyEffectsManager : CharacterEffectsManager
    {
        EnemyManager enemy;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            enemy = characterMgr as EnemyManager;
        }
    
    }
}

