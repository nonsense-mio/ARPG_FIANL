using ARPG;
using Framework;
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
        UIMgr.Instance.ShowPanel<LevelUpPanel>();
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
        GameArchitecture.Interface.SendEvent(new NPCFollowPlayerEvent { IsFollowing = true });
    }
    public void NPCGoHome()
    {
        GameArchitecture.Interface.SendEvent(new NPCFollowPlayerEvent { IsFollowing = false });
    }

    public void RebuildHome()
    {
        GameArchitecture.Interface.GetSystem<IMusicSystem>().PlayBGM("Home");
    }

}
