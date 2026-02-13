using ARPG;
using UnityEngine;

/// <summary>
/// 对话动作辅助器 - 桥接 UnityEvent 和游戏系统
/// 挂在对话NPC上，供 UnityEvent 调用
/// </summary>
public class DialogueActionHelper : MonoBehaviour
{
    [SerializeField]EnemyManager npc;
    private void Awake()
    {
        npc = GetComponent<EnemyManager>();
    }
    private IUISystem uiSystem;
    private IUISystem UISystem =>
        uiSystem ?? (uiSystem = GameArchitecture.Interface.GetSystem<IUISystem>());

    public void OpenLevelUpPanel()
    {
        UISystem.HidePanel<DialoguePanel>();
        UISystem.ShowPanel<LevelUpPanel>();
    }

    public void Task1EnemySpawn()
    {
        GameArchitecture.Interface.SendEvent(new SpawnEnemyEvent());
    }

    public void Task2EnemySpawn()
    {

    }

    public void SpawnBoss()
    {
        GameArchitecture.Interface.SendEvent(new SpawnBossEvent());
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
