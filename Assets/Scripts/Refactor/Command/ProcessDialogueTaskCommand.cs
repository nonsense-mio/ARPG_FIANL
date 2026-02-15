using Framework;

namespace ARPG
{
    /// <summary>
    /// 对话选项点击命令 - 编排:
    /// 任务逻辑(接取/提交) → 对话导航(跳转/关闭)
    /// </summary>
    public class ProcessDialogueTaskCommand : AbstractCommand
    {
        private TaskData_SO task;
        private bool takeQuest;
        private string nextPieceID;

        public ProcessDialogueTaskCommand() { }

        public ProcessDialogueTaskCommand(TaskData_SO task, bool takeQuest, string nextPieceID)
        {
            this.task = task;
            this.takeQuest = takeQuest;
            this.nextPieceID = nextPieceID;
        }

        protected override void OnExecute()
        {
            // 任务逻辑
            if (task != null && takeQuest)
            {
                var taskSystem = this.GetSystem<ITaskSystem>();

                if (taskSystem.HaveTask(task.taskName))
                {
                    var taskData = taskSystem.GetTaskData(task.taskName);
                    if (taskData != null && taskData.isCompleted)
                    {
                        this.SendEvent(new TaskTurnedInEvent(task));
                    }
                }
                else
                {
                    this.SendEvent(new TaskStartedEvent(task));
                }
            }

            // 对话导航
            var ui = this.GetSystem<IUISystem>();
            if (string.IsNullOrEmpty(nextPieceID))
            {
                ui.HidePanel<DialoguePanel>();
                ui.ShowPanel<GamePanel>();
            }
            else
            {
                ui.GetPanel<DialoguePanel>(panel => panel.JumpToPiece(nextPieceID));
            }
        }
    }
}
