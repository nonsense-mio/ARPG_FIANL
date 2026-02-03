using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class PlayerEvents : MonoBehaviour
    {
        private PlayerManager player;

        public void Init(PlayerManager playerMgr)
        {
            player = playerMgr;
        }
        void OnEnable()
        {
            EventCenter.Instance.AddEventListener<CharacterManager>(E_EventType.E_Character_Death, ClearPlayerLockOn);
            EventCenter.Instance.AddEventListener<bool>(E_EventType.E_EnableInput, player.inputMgr.EnableOrDisableInput);
            EventCenter.Instance.AddEventListener<int>(E_EventType.E_Player_SpendSouls, player.playerStatsManager.SpendSouls);
            EventCenter.Instance.AddEventListener<TaskData_SO>(E_EventType.E_Task_TurnedIn, player.playerInventoryManager.RewardTask);
            EventCenter.Instance.AddEventListener<CharacterManager>(E_EventType.E_Character_Death, player.playerStatsManager.AddSouls);

        }

        void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<CharacterManager>(E_EventType.E_Character_Death, ClearPlayerLockOn);
            EventCenter.Instance.RemoveEventListener<bool>(E_EventType.E_EnableInput, player.inputMgr.EnableOrDisableInput);
            EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_Player_SpendSouls, player.playerStatsManager.SpendSouls);
            EventCenter.Instance.RemoveEventListener<TaskData_SO>(E_EventType.E_Task_TurnedIn, player.playerInventoryManager.RewardTask);
            EventCenter.Instance.RemoveEventListener<CharacterManager>(E_EventType.E_Character_Death, player.playerStatsManager.AddSouls);

        }
        //当玩家死亡时清空锁定目标 或 玩家锁定目标死亡时清空
        private void ClearPlayerLockOn(CharacterManager character)
        {   
            if(character != player.cameraMgr.currentLockOnTarget && character is EnemyManager)
                return;
            player.cameraMgr.ClearLockOnTargets();
            player.inputMgr.lockOnFlag = false;

        }
    }
}

