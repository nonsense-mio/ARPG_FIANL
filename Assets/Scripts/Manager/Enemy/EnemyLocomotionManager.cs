using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARPG
{
    public class EnemyLocomotionManager : CharacterLocomotionManager
    {
        EnemyManager enemy;
        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            enemy = characterMgr as EnemyManager;
        }

    }
}

