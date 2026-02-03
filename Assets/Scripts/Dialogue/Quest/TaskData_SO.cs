using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HT;
using UnityEngine;
[CreateAssetMenu(fileName = "New Task Data", menuName = "Task/Task Data")]
public class TaskData_SO : ScriptableObject
{
    [Serializable]
    public class TaskRequire
    {
        public string name;//需求名称
        public int requireAmount;
        public int currentAmount;
    }

    
    public string taskName;//任务名
    [TextArea]
    public string description;//任务描述

    public bool isStarted;//任务是否已被接受
    public bool isCompleted;//任务是否已完成
    public bool isTurnedIn;//任务是否已上交


    public List<TaskRequire> requireList = new List<TaskRequire>();

    public List<Item_SO> rewardList = new List<Item_SO>();

    ///检查任务进度
    public void CheckTaskProgress()
    {
        var finishedRequires = requireList.Where(r => r.currentAmount >= r.requireAmount);
        //如果所有需求都完成，则任务完成
        isCompleted = finishedRequires.Count() == requireList.Count;
        if (isCompleted)
        {
            Debug.Log($"任务[{taskName}]已完成");
        }
    }

    //当前任务所需目标名称列表
    public List<string> RequireTargetNames()
    {
        List<string> names = new List<string>();
        foreach (var require in requireList)
        {
            names.Add(require.name);
        } 
        return names;   
    }


}
