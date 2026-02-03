using System.Collections;
using System.Collections.Generic;
using HT;
using UnityEngine;

public class DialogueController : Interactable
{
    public DialogueData_SO currentData;
    //bool canTalk = false;
    
    // 关联的任务发放者（如果有）
    private TaskGiver taskGiver;
    
    override protected void Awake()
    {
        base.Awake();
        interactionPrompt = "对话";
        taskGiver = GetComponent<TaskGiver>();
    }

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        
        // 交互前刷新任务对话状态
        if (taskGiver != null)
        {
            taskGiver.RefreshDialogue();
        }
        
        //打开对话UI
        UIMgr.Instance.ShowPanel<DialoguePanel>(E_UILayer.Top,(panel) =>
        {
            //初始化对话数据
            panel.InitDialogueData(currentData);
        });
        //隐藏游戏UI
        UIMgr.Instance.HidePanel<GamePanel>();

    }
    
}
