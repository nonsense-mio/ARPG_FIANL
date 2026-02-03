using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    //对话片段列表
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();


    /// <summary>
    /// 根据ID获取对话片段在列表中的索引
    /// </summary>
    public int GetIndexByID(string id)
    {
        for (int i = 0; i < dialoguePieces.Count; i++)
        {
            if (dialoguePieces[i].ID == id)
                return i;
        }
        return -1;
    }

    public TaskData_SO GetTask()
    {
        TaskData_SO currentTask = null;
        foreach (var piece in dialoguePieces)
        {
            if (piece.task != null)
            {
                currentTask = piece.task;

            }
        }
        return currentTask;

    }

}




