using System.Collections;
using System.Collections.Generic;
using HT;
using UnityEngine;
using XLua;

/// <summary>
/// 对话动作辅助器 - 桥接 UnityEvent 和游戏系统
/// 挂在对话NPC上，供 UnityEvent 调用
/// </summary>
[Hotfix]
public class DialogueActionHelper : MonoBehaviour
{
    [SerializeField]EnemyManager npc;
    private void Awake()
    {
        npc = GetComponent<EnemyManager>();
    }
    public void OpenLevelUpPanel()
    {
        UIMgr.Instance.HidePanel<DialoguePanel>();
        UIMgr.Instance.ShowPanel<LevelUpPanel>(callBack: (levelUpPanel) =>
        {
            //绑定facade 诸如数据
            PlayerManager pm = PlayerManager.localPlayer;
            if (pm != null)
            {
                IPlayerUIFacade facade = new PlayerUIFacade(pm.playerStatsManager, pm.playerInventoryManager);
                levelUpPanel.Bind(facade);
            }
        });
    }

    public void Task1EnemySpawn()
    {
        EnemySpawnMgr.Instance.SpawnEnemy();
    }

    public void Task2EnemySpawn()
    {

    }

    public void SpawnBoss()
    {
        EnemySpawnMgr.Instance.SpawnBoss();
    }

    public void NPCGOFollowPlayer()
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_NPC_FollowPlayer,true);
    }
    public void NPCGoHome()
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_NPC_FollowPlayer,false);
    }

    public void RebuildHome()
    {
        MusicMgr.Instance.PlayBKMusic("Home");
    }

}
