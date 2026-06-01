using System;
using System.Collections.Generic;

/// <summary>
/// 场景状态存档数据
/// 管理宝箱、篝火、Boss等场景元素的持久化状态
/// </summary>
[Serializable]
public class SceneStateData
{
    /// <summary>
    /// 宝箱开启状态 (key: chestId, value: isOpened)
    /// </summary>
    public Dictionary<string, bool> chestStates = new Dictionary<string, bool>();

    /// <summary>
    /// 篝火激活状态 (key: bonfireId, value: isActivated)
    /// </summary>
    public Dictionary<string, bool> bonfireStates = new Dictionary<string, bool>();

    /// <summary>
    /// Boss 击败状态 (key: bossId, value: isDefeated)
    /// </summary>
    public Dictionary<string, bool> bossDefeatedStates = new Dictionary<string, bool>();
}
