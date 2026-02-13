using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
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
            GameArchitecture.Interface.RegisterEvent<CharacterDeathEvent>(e => ClearPlayerLockOn(e.Character))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<EnableInputEvent>(e => player.inputMgr.EnableOrDisableInput(e.Enabled))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<TaskTurnedInEvent>(e =>
            {
                foreach (var reward in e.Task.rewardList)
                    GameArchitecture.Interface.SendCommand(new AddItemToInventoryCommand(reward));
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            GameArchitecture.Interface.RegisterEvent<CharacterDeathEvent>(e => player.playerStatsManager.AddSouls(e.Character))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
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

