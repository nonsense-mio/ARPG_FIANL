using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialoguePiece
{
    public string ID;
    public Sprite image; 
    [TextArea]
    public string content;
    public TaskData_SO task;
    //对话选项列表
    public List<DialogueOption> options = new List<DialogueOption>();
}
