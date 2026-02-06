using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;
using HT;

/// <summary>
/// 任务发放者 - 支持任务链的事件驱动模式
/// </summary>
[RequireComponent(typeof(DialogueController))]
public class TaskGiver : MonoBehaviour
{
    DialogueController dialogueController;

    [Header("任务链配置")]
    [Tooltip("按顺序配置多个任务节点，完成一个自动切换下一个")]
    public List<TaskChainNode> taskChain = new List<TaskChainNode>();

    [Tooltip("所有任务都完成后显示的对话")]
    public DialogueData_SO allCompletedDialogue;

    [Header("调试信息")]
    [SerializeField] private int currentTaskIndex = 0;
    [SerializeField] private string currentTaskName = "无";

    /// <summary>
    /// 供 SaveMgr 在保存时读取当前任务索引
    /// </summary>
    public int CurrentTaskIndex => currentTaskIndex;

    // 当前任务链节点
    private TaskChainNode CurrentNode => currentTaskIndex < taskChain.Count ? taskChain[currentTaskIndex] : null;

    // 当前任务数据
    private TaskData_SO CurrentTask => CurrentNode?.GetTask();

    private ITaskSystem taskSystem;
    private ITaskModel taskModel;

    #region 任务状态属性
    private bool IsStarted
    {
        get
        {
            var t = CurrentTask;
            if (t == null) return false;
            var data = taskSystem.GetTaskData(t.taskName);
            return data != null && data.isStarted;
        }
    }

    private bool IsCompleted
    {
        get
        {
            var t = CurrentTask;
            if (t == null) return false;
            var data = taskSystem.GetTaskData(t.taskName);
            return data != null && data.isCompleted;
        }
    }

    private bool IsTurnedIn
    {
        get
        {
            var t = CurrentTask;
            if (t == null) return false;
            var data = taskSystem.GetTaskData(t.taskName);
            return data != null && data.isTurnedIn;
        }
    }
    #endregion

    void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
        taskSystem = GameArchitecture.Interface.GetSystem<ITaskSystem>();
        taskModel = GameArchitecture.Interface.GetModel<ITaskModel>();
    }

    void OnEnable()
    {
        GameArchitecture.Interface.RegisterEvent<TaskTurnedInEvent>(e => OnTaskTurnedIn(e.Task))
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        GameArchitecture.Interface.RegisterEvent<GameDataLoadedEvent>(e => OnGameDataLoaded())
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    /// <summary>
    /// 游戏数据加载完成后刷新对话
    /// </summary>
    private void OnGameDataLoaded()
    {
        LoadProgress();


        if (IsStarted && !IsCompleted)
        {
            CurrentNode?.LoadDialogueAction(CurrentNode.startDialogue);
        }
        else if (IsCompleted && !IsTurnedIn)
        {
            CurrentNode?.LoadDialogueAction(CurrentNode.progressDialogue);
        }
    }

    /// <summary>
    /// 任务提交事件处理
    /// </summary>
    private void OnTaskTurnedIn(TaskData_SO turnedInTask)
    {
        if (turnedInTask == null || CurrentTask == null) return;

        // 检查是否是当前NPC的当前任务
        if (CurrentTask.taskName == turnedInTask.taskName)
        {
            // 推进到下一个任务
            currentTaskIndex++;

            // 更新对话
            UpdateDialogue();
        }
    }

    /// <summary>
    /// 根据当前任务状态更新对话数据
    /// </summary>
    private void UpdateDialogue()
    {
        // 所有任务都完成了
        if (currentTaskIndex >= taskChain.Count)
        {
            dialogueController.currentData = allCompletedDialogue;
            currentTaskName = "全部完成";
            return;
        }

        var node = CurrentNode;
        if (node == null) return;

        currentTaskName = node.nodeName;

        // 根据任务状态切换对话
        if (!IsStarted)
        {
            // 任务未开始，显示开始对话
            dialogueController.currentData = node.startDialogue;
        }
        else if (IsCompleted)
        {
            // 任务已完成，可以提交
            dialogueController.currentData = node.completeDialogue;
        }
        else
        {
            // 任务进行中
            dialogueController.currentData = node.progressDialogue;
        }
    }

    /// <summary>
    /// 外部调用刷新对话状态（比如任务进度更新后）
    /// </summary>
    public void RefreshDialogue()
    {
        UpdateDialogue();
    }

    #region 存档/读档
    /// <summary>
    /// 获取此 TaskGiver 的唯一标识符（用于存档）
    /// </summary>
    public string GetGiverId()
    {
        // 使用 NPC 名称 + 场景中的位置作为唯一标识
        return $"{gameObject.name}_{transform.position.x:F1}_{transform.position.z:F1}";
    }

    /// <summary>
    /// 从 ITaskModel 恢复任务进度索引
    /// </summary>
    public void LoadProgress()
    {
        string id = GetGiverId();
        currentTaskIndex = taskModel.GetGiverProgress(id);

        // 校正：如果当前任务在 TaskSystem 中已被标记为已提交，则跳过
        SyncIndexWithTaskSystem();

        UpdateDialogue();
    }

    /// <summary>
    /// 根据 TaskSystem 中的任务提交状态校正 currentTaskIndex
    /// </summary>
    private void SyncIndexWithTaskSystem()
    {
        while (currentTaskIndex < taskChain.Count)
        {
            var task = taskChain[currentTaskIndex].GetTask();
            if (task == null) break;

            var taskData = taskSystem.GetTaskData(task.taskName);
            // 如果任务已提交，跳到下一个
            if (taskData != null && taskData.isTurnedIn)
            {
                currentTaskIndex++;
            }
            else
            {
                break;
            }
        }
    }
    #endregion

}
