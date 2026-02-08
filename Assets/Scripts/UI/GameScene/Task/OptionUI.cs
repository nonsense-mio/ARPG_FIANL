using ARPG;
using Framework;
using HT;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour, IPoolable
{
    public Text optionText;

    private Button optionButton;

    private DialoguePiece currentPiece;

    private bool takeQuest;

    private string nextPieceID;
    
    private UnityEvent onSelectAction;

    private void Awake()
    {
        optionButton = GetComponent<Button>();
        optionButton.onClick.AddListener(OnOptionClick);
    }

    public void InitOption(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetID;
        takeQuest = option.takeQuest;
        onSelectAction = option.onSelectAction;
    }

    public void OnSpawn() { }

    public void OnRecycle()
    {
        currentPiece = null;
        nextPieceID = null;
        optionText.text = "";
        onSelectAction = null;

        // 重置Transform状态，让LayoutGroup能正确排列
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        // 重置RectTransform的锚点位置
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    //点击选项事件
    public void OnOptionClick()
    {
        // 触发自定义事件
        onSelectAction?.Invoke();
        
        //当前对话片段有任务
        if (currentPiece.task != null && takeQuest)
        {
            var taskSystem = GameArchitecture.Interface.GetSystem<ITaskSystem>();

            //判断是否已接取任务
            if (taskSystem.HaveTask(currentPiece.task.taskName))
            {
                //判断是否完成任务
                var taskData = taskSystem.GetTaskData(currentPiece.task.taskName);
                if (taskData != null && taskData.isCompleted)
                {
                    //触发任务提交事件，通知任务系统和UI更新 发布奖励
                    GameArchitecture.Interface.SendEvent(new TaskTurnedInEvent(currentPiece.task));
                }
            }
            //如果没有任务，添加任务
            else
            {
                GameArchitecture.Interface.SendEvent(new TaskStartedEvent(currentPiece.task));
            }
        }
        //根据选项的目标片段ID跳转到对应的对话片段
        if (string.IsNullOrEmpty(nextPieceID))
        {
            GameArchitecture.Interface.GetSystem<IUISystem>().HidePanel<DialoguePanel>();
            GameArchitecture.Interface.GetSystem<IUISystem>().ShowPanel<GamePanel>();
        }
        else
        {
            //获取当前面板并跳转到目标对话片段
            GameArchitecture.Interface.GetSystem<IUISystem>().GetPanel<DialoguePanel>((panel) =>
            {
                panel.JumpToPiece(nextPieceID);
            });
        }
    }
}
