using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务系统存档辅助类
/// 统一管理所有 TaskGiver 和 TaskManager 的存档/读档
/// </summary>
public static class TaskSaveHelper
{
    /// <summary>
    /// 保存所有任务数据到 CurrentGameDataMgr
    /// 应在保存游戏时调用
    /// </summary>
    public static void SaveAllTaskData()
    {
        // 保存 TaskManager 中的任务列表
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.SaveToGameData();
        }

        // 保存所有 TaskGiver 的进度
        var allTaskGivers = Object.FindObjectsOfType<TaskGiver>();
        foreach (var giver in allTaskGivers)
        {
            giver.SaveProgress();
        }

        Debug.Log($"[TaskSaveHelper] 已保存 {TaskManager.Instance?.taskList.Count ?? 0} 个任务, {allTaskGivers.Length} 个任务发放者");
    }

    /// <summary>
    /// 从 CurrentGameDataMgr 恢复所有任务数据
    /// 会自动从场景中的 TaskGiver 收集所有可用任务
    /// 应在加载游戏、进入场景后调用
    /// 注意：TaskGiver 的进度会通过 E_Game_DataLoaded 事件自动恢复
    /// </summary>
    public static void LoadAllTaskData()
    {
        // 从场景中所有 TaskGiver 收集所有可用的 TaskData_SO
        List<TaskData_SO> allTasks = CollectAllTasksFromScene();

        // 恢复 TaskManager 中的任务列表
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.LoadFromGameData(allTasks);
        }

        // TaskGiver 的进度会通过监听 E_Game_DataLoaded 事件自动恢复

        Debug.Log($"[TaskSaveHelper] 已恢复 {TaskManager.Instance?.taskList.Count ?? 0} 个任务");
    }

    /// <summary>
    /// 从场景中所有 TaskGiver 收集所有任务 SO
    /// </summary>
    private static List<TaskData_SO> CollectAllTasksFromScene()
    {
        List<TaskData_SO> allTasks = new List<TaskData_SO>();
        var allTaskGivers = Object.FindObjectsOfType<TaskGiver>();

        foreach (var giver in allTaskGivers)
        {
            foreach (var node in giver.taskChain)
            {
                var task = node.GetTask();
                if (task != null && !allTasks.Exists(t => t.taskName == task.taskName))
                {
                    allTasks.Add(task);
                }
            }
        }

        return allTasks;
    }


}
