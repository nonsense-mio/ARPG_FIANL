using System.Collections.Generic;
using Framework;

namespace ARPG
{
    /// <summary>
    /// 任务数据模型实现 - 管理任务持久化数据
    /// </summary>
    public class TaskModel : AbstractModel, ITaskModel
    {
        #region 任务数据
        public BindableList<TaskSaveData> ActiveTasks { get; } = new BindableList<TaskSaveData>();
        public Dictionary<string, int> TaskGiverProgress { get; } = new Dictionary<string, int>();
        #endregion

        protected override void OnInit()
        {
            // Model 初始化时不需要特别操作
            // 数据通过 LoadData 加载
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

            ActiveTasks.Add(taskData);
        }

        public void UpdateTaskProgress(string taskName, int requireIndex, int amount)
        {
            var task = GetTask(taskName);
            if (task == null || task.isTurnedIn) return;
            if (requireIndex < 0 || requireIndex >= task.requireProgress.Count) return;

            task.requireProgress[requireIndex] += amount;
        }

        public void CompleteTask(string taskName)
        {
            var task = GetTask(taskName);
            if (task == null || task.isCompleted) return;

            task.isCompleted = true;
        }

        public void TurnInTask(string taskName)
        {
            var task = GetTask(taskName);
            if (task == null || task.isTurnedIn) return;

            task.isTurnedIn = true;
        }

        public TaskSaveData GetTask(string taskName)
        {
            foreach (var task in ActiveTasks)
                if (task.taskName == taskName) return task;
            return null;
        }

        public bool HasTask(string taskName)
        {
            return GetTask(taskName) != null;
        }
        #endregion

        #region NPC 任务链进度
        public void SetGiverProgress(string giverId, int index)
        {
            TaskGiverProgress[giverId] = index;
        }

        public int GetGiverProgress(string giverId)
        {
            return TaskGiverProgress.TryGetValue(giverId, out int index) ? index : 0;
        }
        #endregion

        #region 数据导入导出
        public void LoadData(TaskData data)
        {
            if (data == null)
            {
                ActiveTasks.Clear();
                TaskGiverProgress.Clear();
                return;
            }

            // 深拷贝任务列表，使用 Reset 批量替换
            var copies = new List<TaskSaveData>();
            if (data.taskList != null)
            {
                foreach (var task in data.taskList)
                {
                    copies.Add(new TaskSaveData
                    {
                        taskName = task.taskName,
                        isStarted = task.isStarted,
                        isCompleted = task.isCompleted,
                        isTurnedIn = task.isTurnedIn,
                        requireProgress = new List<int>(task.requireProgress ?? new List<int>())
                    });
                }
            }
            ActiveTasks.Reset(copies);

            // 深拷贝 NPC 进度
            TaskGiverProgress.Clear();
            if (data.taskGiverProgress != null)
            {
                foreach (var kvp in data.taskGiverProgress)
                {
                    TaskGiverProgress[kvp.Key] = kvp.Value;
                }
            }
        }

        public void SaveData(TaskData data)
        {
            if (data == null) return;

            // 导出任务列表
            data.taskList = new List<TaskSaveData>();
            foreach (var task in ActiveTasks)
            {
                data.taskList.Add(new TaskSaveData
                {
                    taskName = task.taskName,
                    isStarted = task.isStarted,
                    isCompleted = task.isCompleted,
                    isTurnedIn = task.isTurnedIn,
                    requireProgress = new List<int>(task.requireProgress ?? new List<int>())
                });
            }

            // 导出 NPC 进度
            data.taskGiverProgress = new Dictionary<string, int>();
            foreach (var kvp in TaskGiverProgress)
            {
                data.taskGiverProgress[kvp.Key] = kvp.Value;
            }
        }
        #endregion
    }
}
