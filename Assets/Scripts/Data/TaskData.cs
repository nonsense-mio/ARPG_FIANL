using System;
using System.Collections.Generic;

/// <summary>
/// 任务存档数据
/// </summary>
[Serializable]
public class TaskSaveData
{
    public string taskName;
    public bool isStarted;
    public bool isCompleted;
    public bool isTurnedIn;
    // 存储每个需求的当前进度
    public List<int> requireProgress = new List<int>();
}

/// <summary>
/// 任务系统存档数据容器
/// </summary>
[Serializable]
public class TaskData
{
    // 所有任务的存档数据
    public List<TaskSaveData> taskList = new List<TaskSaveData>();
    
    // 各 NPC 的任务链进度 (key: NPC ID, value: currentTaskIndex)
    public Dictionary<string, int> taskGiverProgress = new Dictionary<string, int>();
}
