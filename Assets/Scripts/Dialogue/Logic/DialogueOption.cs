using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueOption
{
    public string text;
    public string targetID;
    public bool takeQuest;
    
    public UnityEvent onSelectAction;
}
