using System.Collections.Generic;
using Framework;
using ARPG;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 任务系统实现 - 管理运行时任务对象和事件处理
    /// </summary>
    public class TaskSystem : AbstractSystem, ITaskSystem
    {
        /// <summary>
        /// 运行时任务数据 (TaskData_SO 深拷贝), key = taskName
        /// </summary>
        private Dictionary<string, TaskData_SO> taskDict = new Dictionary<string, TaskData_SO>();

        protected override void OnInit()
        {
            this.RegisterEvent<TaskStartedEvent>(OnTaskStarted);
            this.RegisterEvent<TaskTurnedInEvent>(OnTaskTurnedIn);
            this.RegisterEvent<CharacterDeathEvent>(OnCharacterDeath);
        }

        #region 事件处理

        private void OnTaskStarted(TaskStartedEvent e)
        {
            if (e.Task == null) return;
            if (HaveTask(e.Task.taskName)) return;

            // 深拷贝 SO
            TaskData_SO cloned = Object.Instantiate(e.Task);
            cloned.isStarted = true;
            taskDict[cloned.taskName] = cloned;

            // 同步到 Model (持久化层)
            var model = this.GetModel<ITaskModel>();
            model.AddTask(cloned.taskName, cloned.requireList.Count);
        }

        private void OnTaskTurnedIn(TaskTurnedInEvent e)
        {
            if (e.Task == null) return;
            string taskName = e.Task.taskName;

            if (taskDict.TryGetValue(taskName, out var task))
            {
                task.isTurnedIn = true;
            }

            // 同步到 Model
            var model = this.GetModel<ITaskModel>();
            model.TurnInTask(taskName);
        }

        private void OnCharacterDeath(CharacterDeathEvent e)
        {
            if (e.Character is EnemyManager)
            {
                string enemyName = e.Character.characterStatsManager.characterName;
                UpdateQuestProgress(enemyName, 1);
            }
        }

        #endregion

        #region 进度更新

        private void UpdateQuestProgress(string requireName, int amount)
        {
            var model = this.GetModel<ITaskModel>();

            foreach (var kvp in taskDict)
            {
                var task = kvp.Value;
                if (task.isTurnedIn) continue;

                for (int i = 0; i < task.requireList.Count; i++)
                {
                    if (task.requireList[i].name == requireName)
                    {
                        task.requireList[i].currentAmount += amount;
                        // 同步到 Model
                        model.UpdateTaskProgress(task.taskName, i, amount);
                    }
                }

                task.CheckTaskProgress();

                // 如果刚完成，同步完成状态到 Model 并发送事件
                if (task.isCompleted)
                {
                    model.CompleteTask(task.taskName);
                    this.SendEvent(new TaskCompletedEvent(task.taskName));
                }
            }
        }

        #endregion

        #region 查询

        public bool HaveTask(string taskName)
        {
            return !string.IsNullOrEmpty(taskName) && taskDict.ContainsKey(taskName);
        }

        public TaskData_SO GetTaskData(string taskName)
        {
            if (string.IsNullOrEmpty(taskName)) return null;
            taskDict.TryGetValue(taskName, out var task);
            return task;
        }

        public List<TaskData_SO> GetAllTaskData()
        {
            return new List<TaskData_SO>(taskDict.Values);
        }

        #endregion

        #region 存档集成

        public void RebuildFromModel(List<TaskData_SO> allOriginalTasks)
        {
            taskDict.Clear();
            var model = this.GetModel<ITaskModel>();

            foreach (var saveData in model.ActiveTasks)
            {
                // 查找对应的原始 TaskData_SO
                var original = allOriginalTasks.Find(t => t.taskName == saveData.taskName);
                if (original == null)
                {
                    Debug.LogWarning($"[TaskSystem] 无法找到任务: {saveData.taskName}");
                    continue;
                }

                // 克隆并恢复状态
                TaskData_SO cloned = Object.Instantiate(original);
                cloned.isStarted = saveData.isStarted;
                cloned.isCompleted = saveData.isCompleted;
                cloned.isTurnedIn = saveData.isTurnedIn;

                // 恢复需求进度
                for (int i = 0; i < cloned.requireList.Count && i < saveData.requireProgress.Count; i++)
                {
                    cloned.requireList[i].currentAmount = saveData.requireProgress[i];
                }

                taskDict[cloned.taskName] = cloned;
            }

            Debug.Log($"[TaskSystem] 已重建 {taskDict.Count} 个任务");
        }

        public void SyncToModel()
        {
            var model = this.GetModel<ITaskModel>();

            // 将运行时状态同步到 Model 的 ActiveTasks
            var saveList = new List<TaskSaveData>();
            foreach (var kvp in taskDict)
            {
                var task = kvp.Value;
                var saveData = new TaskSaveData
                {
                    taskName = task.taskName,
                    isStarted = task.isStarted,
                    isCompleted = task.isCompleted,
                    isTurnedIn = task.isTurnedIn,
                    requireProgress = new List<int>()
                };
                foreach (var require in task.requireList)
                {
                    saveData.requireProgress.Add(require.currentAmount);
                }
                saveList.Add(saveData);
            }
            model.ActiveTasks.Reset(saveList);
        }

        public void ClearTasks()
        {
            taskDict.Clear();
            var model = this.GetModel<ITaskModel>();
            model.ActiveTasks.Clear();
        }

        #endregion
    }
}
