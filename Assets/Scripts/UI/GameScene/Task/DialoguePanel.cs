using ARPG;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Framework;

namespace HT
{
    public class DialoguePanel : BasePanel
    {
        private Text txtContent;
        public GameObject optionGroup;
        //当前对话数据
        public DialogueData_SO currentDialogueData;
        //当前对话片段
        public DialoguePiece currentDialoguePiece;
        int currentIndex = 0;
        private PoolSystem poolSystem;
        protected override void Awake()
        {
            base.Awake();
            txtContent = GetControl<Text>("txtContent");
            poolSystem = this.GetSystem<PoolSystem>();
        }
        public void InitDialogueData(DialogueData_SO dialogueData)
        {
            currentDialogueData = dialogueData;
            currentIndex = 0;
            // 清理之前残留的选项
            ClearOptions();
        }

        /// <summary>
        /// 清理所有对话选项
        /// </summary>
        private void ClearOptions()
        {
            for (int i = optionGroup.transform.childCount - 1; i >= 0; i--)
            {
                var child = optionGroup.transform.GetChild(i).gameObject;
                child.transform.SetParent(null); // 先解除父子关系，确保立即生效
                poolSystem.Recycle(child);
            }
        }

        /// <summary>
        /// 跳转到指定ID的对话片段
        /// </summary>
        public void JumpToPiece(string pieceID)
        {
            int index = currentDialogueData.GetIndexByID(pieceID);
            if (index >= 0)
            {
                currentIndex = index;
                ShowMe();
            }
            else
            {
                Debug.LogWarning($"找不到对话片段ID: {pieceID}");
            }
        }
        protected override void ClickBtn(string btnName)
        {
            switch (btnName)
            {
                case "btnNext":
                    //如果没有对话选项，直接进入下一句对话
                    if (currentDialoguePiece.options.Count == 0)
                    {
                        currentIndex++;
                        //检查对话是否结束
                        if (currentIndex >= currentDialogueData.dialoguePieces.Count)
                        {
                            //对话结束
                            UIMgr.Instance.HidePanel<DialoguePanel>();
                            UIMgr.Instance.ShowPanel<GamePanel>();
                        }
                        //还有对话，刷新显示
                        else
                        {
                            //刷新UI显示
                            ShowMe();
                        }
                    }


                    break;
            }
        }
        public override void ShowMe()
        {
            currentDialoguePiece = currentDialogueData.dialoguePieces[currentIndex];
            txtContent.text = "";
            //显示当前对话内容            
            txtContent.DOText(currentDialoguePiece.content, 1f);
            //销毁之前的选项
            ClearOptions();
            //创建新的选项
            for (int i = 0; i < currentDialoguePiece.options.Count; i++)
            {
                GameObject optionObj = poolSystem.Spawn("UI/Option");
                optionObj.transform.SetParent(optionGroup.transform, false);
                OptionUI optionUI = optionObj.GetComponent<OptionUI>();
                optionUI.InitOption(currentDialoguePiece, currentDialoguePiece.options[i]);
            }
        }

        public override void HideMe()
        {

        }


    }
}

