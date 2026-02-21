using ARPG;

namespace ARPG
{
    /// <summary>
    /// 任务开始事件
    /// </summary>
    public struct TaskStartedEvent
    {
        public TaskData_SO Task;
        public string TaskName;

        public TaskStartedEvent(TaskData_SO task)
        {
            Task = task;
            TaskName = task != null ? task.taskName : "";
        }

        public TaskStartedEvent(string taskName)
        {
            Task = null;
            TaskName = taskName;
        }
    }

    /// <summary>
    /// 任务完成事件（所有需求达成）
    /// </summary>
    public struct TaskCompletedEvent
    {
        public string TaskName;

        public TaskCompletedEvent(string taskName)
        {
            TaskName = taskName;
        }
    }

    /// <summary>
    /// 任务上交事件
    /// </summary>
    public struct TaskTurnedInEvent
    {
        public TaskData_SO Task;
        public string TaskName;

        public TaskTurnedInEvent(TaskData_SO task)
        {
            Task = task;
            TaskName = task != null ? task.taskName : "";
        }

        public TaskTurnedInEvent(string taskName)
        {
            Task = null;
            TaskName = taskName;
        }
    }

    /// <summary>
    /// 任务进度更新事件
    /// </summary>
    public struct TaskProgressUpdatedEvent
    {
        public string TaskName;
        public int RequireIndex;
        public int NewAmount;

        public TaskProgressUpdatedEvent(string taskName, int requireIndex, int newAmount)
        {
            TaskName = taskName;
            RequireIndex = requireIndex;
            NewAmount = newAmount;
        }
    }
}
