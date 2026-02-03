using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型枚举
/// </summary>
public enum E_EventType
{
    //交互事件 —— 参数：Interactable
    E_Interact,
    E_Perform_Interaction,
    //角色受伤事件 —— 参数：
    E_Character_Damage,
    //玩家喝药事件 —— 参数：
    E_Player_DrinkPotion,
    //玩家治疗法术事件 —— 参数：
    E_Player_HealSpell,
    //玩家施法前摇事件 —— 参数：
    E_Player_WarmUpSpell,
    //玩家施法事件 —— 参数：
    E_Player_CastSpell,
    //玩家花费灵魂事件 —— 参数：
    E_Player_SpendSouls,

    // 玩家数值变化（状态事件）—— 参数：StatChanged
    E_Player_StatChanged,

    //初始化玩家UI事件 —— 参数：
    E_Player_Init_UI,
    //更新玩家灵魂数量UI事件 —— 参数：
    E_Player_Update_SoulCount_UI,
    // //更新快捷栏UI事件 —— 参数：
    // E_Update_Quick_Slots,
    //打开或关闭选择窗口事件 —— 参数：bool
    E_OpenOrCloseSelectWindow,
    //瞄准事件相关 —— 参数：
    E_AimAction,
    //挥砍事件 —— 参数：
    E_Slash,
    //火球命中事件 —— 参数：
    E_FireBallHit,
    //炸弹命中事件 —— 参数：
    E_BombHit,
    E_ChangeRightWeapon,
    E_ChangeLeftWeapon,
    E_ChangeConsumable,
    E_ChangeSpell,

    E_Character_Death,
    E_BossHudChanged,
    /// <summary>
    /// Boss 进入二阶段事件 —— 参数
    /// </summary>
    E_BossPhaseShift,
    //E_ActiveBoss,
    //E_BossDamage,

    /// <summary>
    /// 场景切换时进度变化获取——参数float
    /// </summary>
    E_SceneLoadChange,

    E_EnableInput,
    /// 任务开始事件 —— 参数：TaskData_SO
    E_Task_Started,
    /// 任务提交事件 —— 参数：TaskData_SO
    E_Task_TurnedIn,

    /// 游戏数据加载完成事件 —— 无参数
    E_Game_DataLoaded,

    E_NPC_FollowPlayer,

    E_ABUpdate,
}
