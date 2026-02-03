using System;
using System.Collections;
using System.Collections.Generic;
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

    // 当前任务链节点
    private TaskChainNode CurrentNode => currentTaskIndex < taskChain.Count ? taskChain[currentTaskIndex] : null;

    // 当前任务数据
    private TaskData_SO CurrentTask => CurrentNode?.GetTask();

    #region 任务状态属性
    private bool IsStarted => GetTaskState(t => t.IsStarted);
    private bool IsCompleted => GetTaskState(t => t.IsCompleted);
    private bool IsTurnedIn => GetTaskState(t => t.IsTurnedIn);

    private bool GetTaskState(Func<TaskManager.TaskInfo, bool> getter)
    {
        var task = CurrentTask;
        if (task != null)
        {
            var taskInfo = TaskManager.Instance.GetTask(task);
            if (taskInfo != null) return getter(taskInfo);
        }
        return false;
    }
    #endregion

    void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
    }

    void OnEnable()
    {
        // 监听任务提交事件
        EventCenter.Instance.AddEventListener<TaskData_SO>(E_EventType.E_Task_TurnedIn, OnTaskTurnedIn);
        // 监听存档加载完成事件
        EventCenter.Instance.AddEventListener(E_EventType.E_Game_DataLoaded, OnGameDataLoaded);
    }

    void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<TaskData_SO>(E_EventType.E_Task_TurnedIn, OnTaskTurnedIn);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Game_DataLoaded, OnGameDataLoaded);
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
    /// 保存当前任务进度索引到 CurrentGameDataMgr
    /// </summary>
    public void SaveProgress()
    {
        var taskData = CurrentGameDataMgr.Instance.taskData;
        string id = GetGiverId();

        if (taskData.taskGiverProgress.ContainsKey(id))
        {
            taskData.taskGiverProgress[id] = currentTaskIndex;
        }
        else
        {
            taskData.taskGiverProgress.Add(id, currentTaskIndex);
        }
    }

    /// <summary>
    /// 从 CurrentGameDataMgr 恢复任务进度索引
    /// </summary>
    public void LoadProgress()
    {
        var taskData = CurrentGameDataMgr.Instance.taskData;
        string id = GetGiverId();

        if (taskData.taskGiverProgress.TryGetValue(id, out int savedIndex))
        {
            currentTaskIndex = savedIndex;
        }
        else
        {
            currentTaskIndex = 0;
        }

        // 校正：如果当前任务在 TaskManager 中已被标记为已提交，则跳过
        SyncIndexWithTaskManager();

        UpdateDialogue();
    }

    /// <summary>
    /// 根据 TaskManager 中的任务提交状态校正 currentTaskIndex
    /// 解决保存时 TaskManager 和 TaskGiver 状态不同步的问题
    /// </summary>
    private void SyncIndexWithTaskManager()
    {
        while (currentTaskIndex < taskChain.Count)
        {
            var task = taskChain[currentTaskIndex].GetTask();
            if (task == null) break;

            var taskInfo = TaskManager.Instance.GetTask(task);
            // 如果任务已提交，跳到下一个
            if (taskInfo != null && taskInfo.IsTurnedIn)
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

