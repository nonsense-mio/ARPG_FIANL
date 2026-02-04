using System.Collections.Generic;
using QFramework;

namespace ARPG
{
    /// <summary>
    /// 任务数据模型实现 - 管理任务持久化数据
    /// </summary>
    public class TaskModel : AbstractModel, ITaskModel
    {
        #region 任务数据
        public BindableProperty<List<TaskSaveData>> ActiveTasks { get; } = new BindableProperty<List<TaskSaveData>>(new List<TaskSaveData>());
        public BindableProperty<Dictionary<string, int>> TaskGiverProgress { get; } = new BindableProperty<Dictionary<string, int>>(new Dictionary<string, int>());
        #endregion

        protected override void OnInit()
        {
            // Model 初始化时不需要特别操作
            // 数据通过 ImportFromTaskData 加载
        }

        #region 任务操作
        public void AddTask(string taskName, int requireCount)
        {
            if (HasTask(taskName)) return;

            var taskData = new TaskSaveData
            {
                taskName = taskName,
                isStarted = true,
                isCompleted = false,
                isTurnedIn = false,
                requireProgress = new List<int>()
            };

            // 初始化需求进度为 0
            for (int i = 0; i < requireCount; i++)
            {
                taskData.requireProgress.Add(0);
            }

            ActiveTasks.Value.Add(taskData);

            // 发送任务开始事件
            this.SendEvent(new TaskStartedEvent(taskName));
        }

        public void UpdateTaskProgress(string taskName, int requireIndex, int amount)
        {
            var task = GetTask(taskName);
            if (task == null || task.isTurnedIn) return;
            if (requireIndex < 0 || requireIndex >= task.requireProgress.Count) return;

            task.requireProgress[requireIndex] += amount;

            // 发送进度更新事件
            this.SendEvent(new TaskProgressUpdatedEvent(taskName, requireIndex, task.requireProgress[requireIndex]));
        }

        public void CompleteTask(string taskName)
        {
            var task = GetTask(taskName);
            if (task == null || task.isCompleted) return;

            task.isCompleted = true;

            // 发送任务完成事件
            this.SendEvent(new TaskCompletedEvent(taskName));
        }

        public void TurnInTask(string taskName)
        {
            var task = GetTask(taskName);
            if (task == null || task.isTurnedIn) return;

            task.isTurnedIn = true;

            // 发送任务上交事件
            this.SendEvent(new TaskTurnedInEvent(taskName));
        }

        public TaskSaveData GetTask(string taskName)
        {
            return ActiveTasks.Value.Find(t => t.taskName == taskName);
        }

        public bool HasTask(string taskName)
        {
            return ActiveTasks.Value.Exists(t => t.taskName == taskName);
        }
        #endregion

        #region NPC 任务链进度
        public void SetGiverProgress(string giverId, int index)
        {
            TaskGiverProgress.Value[giverId] = index;
        }

        public int GetGiverProgress(string giverId)
        {
            return TaskGiverProgress.Value.TryGetValue(giverId, out int index) ? index : 0;
        }
        #endregion

        #region 数据导入导出
        public void ImportFromTaskData(TaskData data)
        {
            if (data == null)
            {
                ActiveTasks.Value = new List<TaskSaveData>();
                TaskGiverProgress.Value = new Dictionary<string, int>();
                return;
            }

            // 深拷贝任务列表
            ActiveTasks.Value = new List<TaskSaveData>();
            if (data.taskList != null)
            {
                foreach (var task in data.taskList)
                {
                    var copy = new TaskSaveData
                    {
                        taskName = task.taskName,
                        isStarted = task.isStarted,
                        isCompleted = task.isCompleted,
                        isTurnedIn = task.isTurnedIn,
                        requireProgress = new List<int>(task.requireProgress ?? new List<int>())
                    };
                    ActiveTasks.Value.Add(copy);
                }
            }

            // 深拷贝 NPC 进度
            TaskGiverProgress.Value = new Dictionary<string, int>();
            if (data.taskGiverProgress != null)
            {
                foreach (var kvp in data.taskGiverProgress)
                {
                    TaskGiverProgress.Value[kvp.Key] = kvp.Value;
                }
            }
        }

        public void ExportToTaskData(TaskData data)
        {
            if (data == null) return;

            // 导出任务列表
            data.taskList = new List<TaskSaveData>();
            foreach (var task in ActiveTasks.Value)
            {
                var copy = new TaskSaveData
                {
                    taskName = task.taskName,
                    isStarted = task.isStarted,
                    isCompleted = task.isCompleted,
                    isTurnedIn = task.isTurnedIn,
                    requireProgress = new List<int>(task.requireProgress ?? new List<int>())
                };
                data.taskList.Add(copy);
            }

            // 导出 NPC 进度
            data.taskGiverProgress = new Dictionary<string, int>();
            foreach (var kvp in TaskGiverProgress.Value)
            {
                data.taskGiverProgress[kvp.Key] = kvp.Value;
            }
        }
        #endregion
    }
}
