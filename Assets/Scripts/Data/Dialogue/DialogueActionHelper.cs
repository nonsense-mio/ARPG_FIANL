
using ARPG;
using Framework;
using UnityEngine;

/// <summary>
/// 对话动作辅助器 - 桥接 UnityEvent 和游戏系统
/// 挂在对话NPC上，供 UnityEvent 调用
/// </summary>
public class DialogueActionHelper : ARPGController
{
    [SerializeField]EnemyManager npc;
    private void Awake()
    {
        npc = GetComponent<EnemyManager>();
    }
    private IUISystem uiSystem;
    private IUISystem UISystem =>
        uiSystem ?? (uiSystem = this.GetSystem<IUISystem>());

    public void OpenLevelUpPanel()
    {
        UISystem.HidePanel<DialoguePanel>();
        UISystem.ShowPanel<LevelUpPanel>();
    }

    public void Task1EnemySpawn()
    {
        this.SendEvent(new SpawnEnemyEvent());
    }

    public void Task2EnemySpawn()
    {

    }

    public void SpawnBoss()
    {
        this.SendEvent(new SpawnBossEvent());
    }

    public void NPCGOFollowPlayer()
    {
        this.SendEvent(new NPCFollowPlayerEvent { IsFollowing = true });
    }
    public void NPCGoHome()
    {
        this.SendEvent(new NPCFollowPlayerEvent { IsFollowing = false });
    }

    public void RebuildHome()
    {
        this.GetSystem<IMusicSystem>().PlayBGM("Home");
    }

}
