using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionUI : ARPGController, IPoolable
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
        onSelectAction?.Invoke();
        this.SendCommand(new ProcessDialogueTaskCommand(
            currentPiece.task, takeQuest, nextPieceID));
    }
}
