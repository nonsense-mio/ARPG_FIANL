using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    [CreateAssetMenu(menuName = "A.I/Enemy Action/Attack Action")]
    public class EnemyAttackAction : EnemyAction
    {
        public bool canCombo;
        public EnemyAttackAction comboAction;

        public int attckScore = 3;
        public float recoveryTime = 2;

        public float maximumAttackAngle = 35;
        public float minimumAttackAngle = -35;

        public float minimumDistanceNeededToAttack = 0;
        public float maximumDistanceNeededToAttack = 3;
    }
}

