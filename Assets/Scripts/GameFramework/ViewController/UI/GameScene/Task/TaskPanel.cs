using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace ARPG
{
    public class TaskPanel : BasePanel
    {
        public RectTransform taskListTransform;
        public RectTransform requireListTransform;
        public RectTransform rewardListTransform;
        private Text txtTaskTitle;
        private Text txtTaskContent;
        private IPoolSystem poolSystem;

        protected override void Awake()
        {
            base.Awake();
            txtTaskTitle = GetControl<Text>("txtTaskTitle");
            txtTaskContent = GetControl<Text>("txtTaskContent");
            poolSystem = this.GetSystem<IPoolSystem>();
        }

        //设置任务需求列表
        public void SetRequireList(TaskData_SO taskData)
        {
            // 倒序遍历
            for (int i = requireListTransform.childCount - 1; i >= 0; i--)
            {
                poolSystem.Recycle(requireListTransform.GetChild(i).gameObject);
            }
            //生成需求列表
            for (int i = 0; i < taskData.requireList.Count; i++)
            {
                GameObject go = poolSystem.Spawn("UI/TaskRequirement");
                go.transform.SetParent(requireListTransform, false);
                TaskRequirement taskRequirement = go.GetComponent<TaskRequirement>();
                taskRequirement.InitRequirement(taskData.requireList[i].name, taskData.requireList[i].requireAmount, taskData.requireList[i].currentAmount);
                if(taskData.isTurnedIn)
                    taskRequirement.InitRequirement(taskData.requireList[i].name, true);
            }
        }
        public void SetRewardList(TaskData_SO taskData)
        {
            // 倒序遍历，避免索引错乱
            for (int i = rewardListTransform.childCount - 1; i >= 0; i--)
            {
                poolSystem.Recycle(rewardListTransform.GetChild(i).gameObject);
            }
            //生成奖励列表
            for (int i = 0; i < taskData.rewardList.Count; i++)
            {
                GameObject obj = poolSystem.Spawn("UI/btnBagItem");
                obj.transform.SetParent(rewardListTransform, false);
                BagItem bagItem = obj.GetComponent<BagItem>();
                bagItem.InitInfo(taskData.rewardList[i]);
            }
        }
        public override void ShowMe()
        {
            // 防御性清理：确保不会累积旧按钮
            for (int i = taskListTransform.childCount - 1; i >= 0; i--)
            {
                poolSystem.Recycle(taskListTransform.GetChild(i).gameObject);
            }

            //生成任务列表
            var taskSystem = this.GetSystem<ITaskSystem>();
            var allTasks = taskSystem.GetAllTaskData();
            for (int i = 0; i < allTasks.Count; i++)
            {
                GameObject btnTaskName = poolSystem.Spawn("UI/btnTaskName");
                btnTaskName.transform.SetParent(taskListTransform, false);
                btnTaskName.transform.localScale = Vector3.one;
                TaskNameButton taskNameButton = btnTaskName.GetComponent<TaskNameButton>();
                taskNameButton.Init(allTasks[i]);
                //添加按钮监听事件 显示任务内容、需求和奖励
                taskNameButton.btnTask.onClick.AddListener(() =>
                {
                    txtTaskTitle.text = taskNameButton.currentData.taskName;
                    txtTaskContent.text = taskNameButton.currentData.description;
                    SetRequireList(taskNameButton.currentData);
                    SetRewardList(taskNameButton.currentData);
                });
            }
        }
        public override void HideMe()
        {
            txtTaskContent.text = "";
            txtTaskTitle.text = "任务信息";
            
            // 倒序遍历清理
            for (int i = taskListTransform.childCount - 1; i >= 0; i--)
            {
                poolSystem.Recycle(taskListTransform.GetChild(i).gameObject);
            }
            for (int i = requireListTransform.childCount - 1; i >= 0; i--)
            {
                poolSystem.Recycle(requireListTransform.GetChild(i).gameObject);
            }
            for (int i = rewardListTransform.childCount - 1; i >= 0; i--)
            {
                poolSystem.Recycle(rewardListTransform.GetChild(i).gameObject);
            }
        }


    }
}

