using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class EnemyAnimatorManager : CharacterAnimatorManager
    {
        EnemyManager enemy;

        public override void Init(CharacterManager characterMgr)
        {
            base.Init(characterMgr);
            enemy = characterMgr as EnemyManager;
        }


        public void InstantiateBossParticalFX()
        {
            BossFXTransform bossFXTransform = GetComponentInChildren<BossFXTransform>();
            GameObject phaseFX = Instantiate(enemy.enemyBossManager.particalFX, bossFXTransform.transform);
        }

        protected override void OnAnimatorMove()
        {
            if (enemy.isDead)
                return;
            Vector3 velocity = enemy.animator.deltaPosition;
            enemy.characterController.Move(velocity);

            if (enemy.isRotatingWithRootMotion)
            {
                enemy.transform.rotation *= enemy.animator.deltaRotation;
            }

        }

    }
}

