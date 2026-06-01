
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务链节点，包含一个任务的所有对话状态
/// </summary>
[Serializable]
public class TaskChainNode
{
    [Tooltip("节点名称")]
    public string nodeName;
    
    [Tooltip("任务开始时的对话（包含接任务选项）")]
    public DialogueData_SO startDialogue;
    
    [Tooltip("任务进行中的对话")]
    public DialogueData_SO progressDialogue;
    
    [Tooltip("任务完成可提交时的对话")]
    public DialogueData_SO completeDialogue;

    /// <summary>
    /// 获取当前节点关联的任务数据
    /// </summary>
    public TaskData_SO GetTask()
    {
        return startDialogue?.GetTask();
    }

    /// <summary>
    /// 加载对话选项中的事件
    /// </summary>
    /// <param name="dialogueData"></param> <summary>
    public void LoadDialogueAction(DialogueData_SO dialogueData)
    {
        List<DialoguePiece> pieces = dialogueData.dialoguePieces;
        foreach (var piece in pieces)
        {
            foreach (var option in piece.options)
            {
                option.onSelectAction?.Invoke();
            }
        }
    }
}
