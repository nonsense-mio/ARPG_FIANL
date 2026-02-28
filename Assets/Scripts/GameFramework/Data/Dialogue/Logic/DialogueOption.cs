using System;
using UnityEngine.Events;

[Serializable]
public class DialogueOption
{
    public string text;
    public string targetID;
    public bool takeQuest;
    
    public UnityEvent onSelectAction;
}
