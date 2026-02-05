using System;
using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;
using System.Linq;
using HT;

public class TaskManager : SingletonAutoMono<TaskManager>
{
    [Serializable]
    public class TaskInfo
    {
        public TaskData_SO taskData;
        public bool IsStarted
        {
            get { return taskData.isStarted; }
            set { taskData.isStarted = value; }
        }
        public bool IsCompleted
        {
            get { return taskData.isCompleted; }
            set { taskData.isCompleted = value; }
        }
        public bool IsTurnedIn
        {
            get { return taskData.isTurnedIn; }
            set { taskData.isTurnedIn = value; }
        }
    }
    //任务列表
    public List<TaskInfo> taskList = new List<TaskInfo>();

    void OnEnable()
    {
        GameArchitecture.Interface.RegisterEvent<TaskStartedEvent>(e => AddTask(e.Task))
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        GameArchitecture.Interface.RegisterEvent<TaskTurnedInEvent>(e => TurnedInTask(e.Task))
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        GameArchitecture.Interface.RegisterEvent<CharacterDeathEvent>(e => EnemyDeathHandler(e.Character))
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }



    //敌人死亡 更新任务进度
    public void UpdateQuestProgress(string requireName, int amount)
    {
        foreach (var task in taskList)
        {
            if (task.taskData.isTurnedIn)
                continue;
            var matchRequire = task.taskData.requireList.Find(r => r.name == requireName);
            if (matchRequire != null)
            {
                matchRequire.currentAmount += amount;
            }
            task.taskData.CheckTaskProgress();
        }
    }

    public bool HaveTask(TaskData_SO taskData)
    {
        //判断任务列表中是否已有该任务
        if (taskData != null)
            return taskList.Any(task => task.taskData.taskName == taskData.taskName);
        return false;
    }
    /// <summary>
    /// 获取任务
    /// </summary>
    /// <param name="taskData"></param>
    /// <returns></returns>
    public TaskInfo GetTask(TaskData_SO taskData)
    {
        return taskList.Find(task => task.taskData.taskName == taskData.taskName);
    }

    /// <summary>
    /// 添加任务到任务列表
    /// </summary>
    public void AddTask(TaskData_SO task)
    {
        if (task == null || HaveTask(task)) return;
        //深拷贝
        TaskData_SO clonedTask = Instantiate(task);
        //clonedTask.name = task.name;
        taskList.Add(new TaskInfo { taskData = clonedTask });
        var taskInfo = GetTask(task);
        if (taskInfo != null)
        {
            taskInfo.IsStarted = true;
        }
    }

    /// <summary>
    /// 提交任务
    /// </summary>
    /// <param name="taskData"></param>
    private void TurnedInTask(TaskData_SO taskData)
    {
        var taskInfo = GetTask(taskData);
        if (taskInfo != null)
        {
            taskInfo.IsTurnedIn = true;
        }
    }



    
    private void EnemyDeathHandler(CharacterManager character)
    {
        if (character is EnemyManager)
        {
            UpdateQuestProgress(character.characterStatsManager.characterName, 1);
        }

    }

    #region 存档/读档
    /// <summary>
    /// 将当前任务状态保存到 CurrentGameDataMgr.taskData
    /// </summary>
    public void SaveToGameData()
    {
        var taskData = CurrentGameDataMgr.Instance.taskData;
        taskData.taskList.Clear();

        foreach (var taskInfo in taskList)
        {
            var saveData = new TaskSaveData
            {
                taskName = taskInfo.taskData.taskName,
                isStarted = taskInfo.IsStarted,
                isCompleted = taskInfo.IsCompleted,
                isTurnedIn = taskInfo.IsTurnedIn,
                requireProgress = new List<int>()
            };

            // 保存每个require的当前进度
            foreach (var require in taskInfo.taskData.requireList)
            {
                saveData.requireProgress.Add(require.currentAmount);
            }

            taskData.taskList.Add(saveData);
        }
    }

    /// <summary>
    /// 从 CurrentGameDataMgr.taskData 恢复任务状态
    /// </summary>
    /// <param name="allTasks">所有可用的原始 TaskData_SO 列表（用于克隆）</param>
    public void LoadFromGameData(List<TaskData_SO> allTasks)
    {
        var taskData = CurrentGameDataMgr.Instance.taskData;
        taskList.Clear();

        foreach (var saveData in taskData.taskList)
        {
            // 查找对应的原始 TaskData_SO
            var originalTask = allTasks.Find(t => t.taskName == saveData.taskName);
            if (originalTask == null)
            {
                Debug.LogWarning($"[TaskManager] 无法找到任务: {saveData.taskName}");
                continue;
            }

            // 克隆任务数据
            TaskData_SO clonedTask = Instantiate(originalTask);
            clonedTask.isStarted = saveData.isStarted;
            clonedTask.isCompleted = saveData.isCompleted;
            clonedTask.isTurnedIn = saveData.isTurnedIn;

            // 恢复每个require的进度
            for (int i = 0; i < clonedTask.requireList.Count && i < saveData.requireProgress.Count; i++)
            {
                clonedTask.requireList[i].currentAmount = saveData.requireProgress[i];
            }

            taskList.Add(new TaskInfo { taskData = clonedTask });
        }
    }

    /// <summary>
    /// 清空任务列表（用于新游戏或切换存档）
    /// </summary>
    public void ClearTasks()
    {
        taskList.Clear();
    }
    #endregion

}
