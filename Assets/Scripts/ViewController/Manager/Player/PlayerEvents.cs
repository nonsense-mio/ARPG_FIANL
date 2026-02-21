using ARPG;
using Framework;
using UnityEngine;

namespace ARPG
{
    public class PlayerEvents : ARPGController
    {
        private PlayerManager player;

        public void Init(PlayerManager playerMgr)
        {
            player = playerMgr;
        }
        void OnEnable()
        {
            this.RegisterEvent<CharacterDeathEvent>(e => ClearPlayerLockOn(e.Character))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<EnableInputEvent>(e => player.inputMgr.EnableOrDisableInput(e.Enabled))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<TaskTurnedInEvent>(e =>
            {
                foreach (var reward in e.Task.rewardList)
                    this.SendCommand(new AddItemToInventoryCommand(reward));
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<CharacterDeathEvent>(e => player.playerStatsManager.AddSouls(e.Character))
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

