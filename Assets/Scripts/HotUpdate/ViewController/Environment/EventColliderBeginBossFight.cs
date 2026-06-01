using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class EventColliderBeginBossFight : MonoBehaviour
    {

        [SerializeField]
        private BossFightManager bossFight;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Character"))
            {
                bossFight?.ActivateBossFight();
                print("触发boss战斗事件");
            }
        }
    }

}
