using UnityEngine;
using UnityEngine.UI;


namespace HT
{
    /// <summary>
    /// 存档槽位UI：显示单个槽位的信息（空槽位显示"新游戏"，有存档显示详细信息）
    /// </summary>
    public class SaveSlotUI : MonoBehaviour
    {
        [Header("空槽位显示")]
        [SerializeField] private Text txtNewGame;

        [Header("有存档时显示")]
        [SerializeField] private GameObject saveInfoGroup;
        [SerializeField] private Text txtLevel;
        [SerializeField] private Text txtHP;
        [SerializeField] private Text txtStamina;
        [SerializeField] private Text txtFocus;
        [SerializeField] private Text txtSouls;
        [SerializeField] private Text txtPlayTime;
        [SerializeField] private Text txtSceneName;

        private SaveSlotData currentData;

        /// <summary>
        /// 更新显示内容
        /// </summary>
        public void UpdateDisplay(SaveSlotData data)
        {
            currentData = data;
            //没有存档数据时显示空槽位
            if (data == null || data.isEmpty)
            {
                ShowEmptySlot();
            }
            //有存档时显示存档信息
            else
            {
                ShowSaveInfo(data);
            }
        }

        /// <summary>
        /// 检查当前槽位是否为空
        /// </summary>
        public bool IsEmpty()
        {
            return currentData == null || currentData.isEmpty;
        }

        // 显示空槽位UI
        private void ShowEmptySlot()
        {
            if (txtNewGame != null)
                txtNewGame.gameObject.SetActive(true);

            if (saveInfoGroup != null)
                saveInfoGroup.SetActive(false);
        }
        
        // 显示存档信息UI
        private void ShowSaveInfo(SaveSlotData data)
        {
            if (txtNewGame != null)
                txtNewGame.gameObject.SetActive(false);

            if (saveInfoGroup != null)
                saveInfoGroup.SetActive(true);

            if (txtLevel != null)
                txtLevel.text = $"LV.{data.playerLevel}";

            if (txtHP != null)
                txtHP.text = $"HP: {data.maxHealth}";

            if (txtStamina != null)
                txtStamina.text = $"STAMINA: {data.maxStamina}";

            if (txtFocus != null)
                txtFocus.text = $"FOCUS: {data.maxFocus}";

            if (txtSouls != null)
                txtSouls.text = $"SOULS: {data.currentSouls}";

            if (txtPlayTime != null)
                txtPlayTime.text = data.GetFormattedPlayTime();

            if (txtSceneName != null)
                txtSceneName.text = "失落王国";
        }
    }
}
