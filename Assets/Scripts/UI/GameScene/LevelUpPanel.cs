using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
namespace HT
{
    public class LevelUpPanel : BasePanel
    {
        private Button btnLevUp;
        [Header("玩家等级")]
        [SerializeField]
        private int currentPlayerLevel;
        [SerializeField]
        private int projectedPlayerLevel;
        private Text txtCurrentPlayerLevel;
        private Text txtProjectedPlayerLevel;
        [Header("魂")]
        private Text txtCurrentSouls;
        private Text txtSoulsRequired;
        private int soulsRequiredToLevelUp;
        private int baseLevelUpSoulCost = 5;

        [Header("生命值")]
        private Slider sliderHealth;
        private Text txtCurrentHealthLevel;
        private Text txtProjectedHealthLevel;

        [Header("耐力值")]
        private Slider sliderStamina;
        private Text txtCurrentStaminaLevel;
        private Text txtProjectedStaminaLevel;

        [Header("专注值")]
        private Slider sliderFocus;
        private Text txtCurrentFocusLevel;
        private Text txtProjectedFocusLevel;
        [Header("姿态值")]
        private Slider sliderPoise;
        private Text txtCurrentPoiseLevel;
        private Text txtProjectedPoiseLevel;
        [Header("力量值")]
        private Slider sliderStrength;
        private Text txtCurrentStrengthLevel;
        private Text txtProjectedStrengthLevel;
        [Header("敏捷值")]
        private Slider sliderDexterity;
        private Text txtCurrentDexterityLevel;
        private Text txtProjectedDexterityLevel;
        [Header("智力值")]
        private Slider sliderIntelligence;
        private Text txtCurrentIntelligenceLevel;
        private Text txtProjectedIntelligenceLevel;
        [Header("信仰值")]
        private Slider sliderFaith;
        private Text txtCurrentFaithLevel;
        private Text txtProjectedFaithLevel;
        protected override void Awake()
        {
            base.Awake();
            btnLevUp = GetControl<Button>("btnLevUp");
            txtCurrentPlayerLevel = GetControl<Text>("txtCurrentPlayerLevel");
            txtProjectedPlayerLevel = GetControl<Text>("txtProjectedPlayerLevel");
            txtCurrentSouls = GetControl<Text>("txtCurrentSouls");
            txtSoulsRequired = GetControl<Text>("txtSoulsRequired");
            txtCurrentHealthLevel = GetControl<Text>("txtCurrentHealthLevel");
            txtProjectedHealthLevel = GetControl<Text>("txtProjectedHealthLevel");
            sliderHealth = GetControl<Slider>("sliderHealth");
            txtCurrentStaminaLevel = GetControl<Text>("txtCurrentStaminaLevel");
            txtProjectedStaminaLevel = GetControl<Text>("txtProjectedStaminaLevel");
            sliderStamina = GetControl<Slider>("sliderStamina");
            txtCurrentFocusLevel = GetControl<Text>("txtCurrentFocusLevel");
            txtProjectedFocusLevel = GetControl<Text>("txtProjectedFocusLevel");
            sliderFocus = GetControl<Slider>("sliderFocus");
            txtCurrentPoiseLevel = GetControl<Text>("txtCurrentPoiseLevel");
            txtProjectedPoiseLevel = GetControl<Text>("txtProjectedPoiseLevel");
            sliderPoise = GetControl<Slider>("sliderPoise");
            txtCurrentStrengthLevel = GetControl<Text>("txtCurrentStrengthLevel");
            txtProjectedStrengthLevel = GetControl<Text>("txtProjectedStrengthLevel");
            sliderStrength = GetControl<Slider>("sliderStrength");
            txtCurrentDexterityLevel = GetControl<Text>("txtCurrentDexterityLevel");
            txtProjectedDexterityLevel = GetControl<Text>("txtProjectedDexterityLevel");
            sliderDexterity = GetControl<Slider>("sliderDexterity");
            txtCurrentIntelligenceLevel = GetControl<Text>("txtCurrentIntelligenceLevel");
            txtProjectedIntelligenceLevel = GetControl<Text>("txtProjectedIntelligenceLevel");
            sliderIntelligence = GetControl<Slider>("sliderIntelligence");
            txtCurrentFaithLevel = GetControl<Text>("txtCurrentFaithLevel");
            txtProjectedFaithLevel = GetControl<Text>("txtProjectedFaithLevel");
            sliderFaith = GetControl<Slider>("sliderFaith");
        }

        private void EnsureFacade()
        {
            if (playerFacade == null)
                Debug.LogError($"{nameof(LevelUpPanel)} 未绑定 playerFacade，请在 ShowPanel/GetPanel 回调里调用 panel.Bind(facade)");
        }
        public override void HideMe()
        {
            btnLevUp.onClick.RemoveListener(ConfirmPlayerLevelUpStats);
            sliderHealth.onValueChanged.RemoveListener(UpdateHealthLevelSlider);
            sliderStamina.onValueChanged.RemoveListener(UpdateStaminaLevelSlider);
            sliderFocus.onValueChanged.RemoveListener(UpdateFocusLevelSlider);
            sliderPoise.onValueChanged.RemoveListener(UpdatePoiseLevelSlider);
            sliderStrength.onValueChanged.RemoveListener(UpdateStrengthLevelSlider);
            sliderDexterity.onValueChanged.RemoveListener(UpdateDexterityLevelSlider);
            sliderIntelligence.onValueChanged.RemoveListener(UpdateIntelligenceLevelSlider);
            sliderFaith.onValueChanged.RemoveListener(UpdateFaithLevelSlider);
        }

        public override void ShowMe()
        {
            EnsureFacade();
            Init();
            UpdateProjectedPlayerLevel();
            btnLevUp.onClick.AddListener(ConfirmPlayerLevelUpStats);
            sliderHealth.Select();
            sliderHealth.OnSelect(null);
            sliderHealth.onValueChanged.AddListener(UpdateHealthLevelSlider);
            sliderStamina.onValueChanged.AddListener(UpdateStaminaLevelSlider);
            sliderFocus.onValueChanged.AddListener(UpdateFocusLevelSlider);
            sliderPoise.onValueChanged.AddListener(UpdatePoiseLevelSlider);
            sliderStrength.onValueChanged.AddListener(UpdateStrengthLevelSlider);
            sliderDexterity.onValueChanged.AddListener(UpdateDexterityLevelSlider);
            sliderIntelligence.onValueChanged.AddListener(UpdateIntelligenceLevelSlider);
            sliderFaith.onValueChanged.AddListener(UpdateFaithLevelSlider);
        }
        private void Init()
        {
            if (playerFacade == null) return;

            currentPlayerLevel = playerFacade.PlayerLevel;
            txtCurrentPlayerLevel.text = currentPlayerLevel.ToString();
            projectedPlayerLevel = playerFacade.PlayerLevel;
            txtProjectedPlayerLevel.text = projectedPlayerLevel.ToString();

            sliderHealth.value = playerFacade.HealthLevel;
            sliderHealth.minValue = playerFacade.HealthLevel;
            sliderHealth.maxValue = 99;
            sliderHealth.wholeNumbers = true;
            txtCurrentHealthLevel.text = playerFacade.HealthLevel.ToString();
            txtProjectedHealthLevel.text = playerFacade.HealthLevel.ToString();

            sliderStamina.value = playerFacade.StaminaLevel;
            sliderStamina.minValue = playerFacade.StaminaLevel;
            sliderStamina.maxValue = 99;
            sliderStamina.wholeNumbers = true;
            txtCurrentStaminaLevel.text = playerFacade.StaminaLevel.ToString();
            txtProjectedStaminaLevel.text = playerFacade.StaminaLevel.ToString();

            sliderFocus.value = playerFacade.FocusLevel;
            sliderFocus.minValue = playerFacade.FocusLevel;
            sliderFocus.maxValue = 99;
            sliderFocus.wholeNumbers = true;
            txtCurrentFocusLevel.text = playerFacade.FocusLevel.ToString();
            txtProjectedFocusLevel.text = playerFacade.FocusLevel.ToString();

            sliderPoise.value = playerFacade.PoiseLevel;
            sliderPoise.minValue = playerFacade.PoiseLevel;
            sliderPoise.maxValue = 99;
            sliderPoise.wholeNumbers = true;
            txtCurrentPoiseLevel.text = playerFacade.PoiseLevel.ToString();
            txtProjectedPoiseLevel.text = playerFacade.PoiseLevel.ToString();

            sliderStrength.value = playerFacade.StrengthLevel;
            sliderStrength.minValue = playerFacade.StrengthLevel;
            sliderStrength.maxValue = 99;
            sliderStrength.wholeNumbers = true;
            txtCurrentStrengthLevel.text = playerFacade.StrengthLevel.ToString();
            txtProjectedStrengthLevel.text = playerFacade.StrengthLevel.ToString();

            sliderDexterity.value = playerFacade.DexterityLevel;
            sliderDexterity.minValue = playerFacade.DexterityLevel;
            sliderDexterity.maxValue = 99;
            sliderDexterity.wholeNumbers = true;
            txtCurrentDexterityLevel.text = playerFacade.DexterityLevel.ToString();
            txtProjectedDexterityLevel.text = playerFacade.DexterityLevel.ToString();

            sliderIntelligence.value = playerFacade.IntelligenceLevel;
            sliderIntelligence.minValue = playerFacade.IntelligenceLevel;
            sliderIntelligence.maxValue = 99;
            sliderIntelligence.wholeNumbers = true;
            txtCurrentIntelligenceLevel.text = playerFacade.IntelligenceLevel.ToString();
            txtProjectedIntelligenceLevel.text = playerFacade.IntelligenceLevel.ToString();

            sliderFaith.value = playerFacade.FaithLevel;
            sliderFaith.minValue = playerFacade.FaithLevel;
            sliderFaith.maxValue = 99;
            sliderFaith.wholeNumbers = true;
            txtCurrentSouls.text = playerFacade.CurrentSoulCount.ToString();
        }
        //确认升级
        public void ConfirmPlayerLevelUpStats()
        {
            if (playerFacade == null) return;
            playerFacade.PlayerLevel = projectedPlayerLevel;
            playerFacade.HealthLevel = Mathf.RoundToInt(sliderHealth.value);
            playerFacade.StaminaLevel = Mathf.RoundToInt(sliderStamina.value);
            playerFacade.FocusLevel = Mathf.RoundToInt(sliderFocus.value);
            playerFacade.PoiseLevel = Mathf.RoundToInt(sliderPoise.value);
            playerFacade.StrengthLevel = Mathf.RoundToInt(sliderStrength.value);
            playerFacade.DexterityLevel = Mathf.RoundToInt(sliderDexterity.value);
            playerFacade.IntelligenceLevel = Mathf.RoundToInt(sliderIntelligence.value);
            playerFacade.FaithLevel = Mathf.RoundToInt(sliderFaith.value);

            playerFacade.SetMaxHealthFromHealthLevel();
            playerFacade.SetMaxStaminaFromStaminaLevel();
            playerFacade.SetMaxFocusPointsFromFocusLevel();

            playerFacade.CurrentSoulCount -= soulsRequiredToLevelUp;
            // EventCenter.Instance.EventTrigger(E_EventType.E_Player_Init_UI);
            // EventCenter.Instance.EventTrigger(E_EventType.E_Player_SpendSouls, soulsRequiredToLevelUp);
            UIMgr.Instance.GetPanel<GamePanel>((gamePanel) =>
            {
                gamePanel.Bind(playerFacade);
                gamePanel.SetSoulCountText(playerFacade.CurrentSoulCount);
                gamePanel.InitializePlayerUI();
            });


            UIMgr.Instance.HidePanel<LevelUpPanel>();
            UIMgr.Instance.ShowPanel<DialoguePanel>();


        }
        //计算升级所需魂数
        private void CalculateSoulCostToLevelUp()
        {
            for (int i = 0; i < projectedPlayerLevel; i++)
            {
                soulsRequiredToLevelUp += Mathf.RoundToInt((projectedPlayerLevel - currentPlayerLevel) * baseLevelUpSoulCost * 1.1f);
            }
        }
        //更新目标等级
        private void UpdateProjectedPlayerLevel()
        {
            if (playerFacade == null) return;
            soulsRequiredToLevelUp = 0;

            projectedPlayerLevel = currentPlayerLevel;
            projectedPlayerLevel = currentPlayerLevel;
            projectedPlayerLevel += Mathf.RoundToInt(sliderHealth.value - playerFacade.HealthLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderStamina.value - playerFacade.StaminaLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderFocus.value - playerFacade.FocusLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderPoise.value - playerFacade.PoiseLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderStrength.value - playerFacade.StrengthLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderDexterity.value - playerFacade.DexterityLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderIntelligence.value - playerFacade.IntelligenceLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderFaith.value - playerFacade.FaithLevel);

            txtProjectedPlayerLevel.text = projectedPlayerLevel.ToString();
            CalculateSoulCostToLevelUp();
            txtSoulsRequired.text = soulsRequiredToLevelUp.ToString();
            //检查是否有足够的魂进行升级
            btnLevUp.interactable = playerFacade.CurrentSoulCount >= soulsRequiredToLevelUp;
        }


        private void UpdateHealthLevelSlider(float f)
        {
            txtProjectedHealthLevel.text = sliderHealth.value.ToString();
            UpdateProjectedPlayerLevel();
        }

        private void UpdateStaminaLevelSlider(float f)
        {
            txtProjectedStaminaLevel.text = sliderStamina.value.ToString();
            UpdateProjectedPlayerLevel();
        }

        private void UpdateFocusLevelSlider(float f)
        {
            txtProjectedFocusLevel.text = sliderFocus.value.ToString();
            UpdateProjectedPlayerLevel();
        }

        private void UpdatePoiseLevelSlider(float f)
        {
            txtProjectedPoiseLevel.text = sliderPoise.value.ToString();
            UpdateProjectedPlayerLevel();
        }
        private void UpdateStrengthLevelSlider(float f)
        {
            txtProjectedStrengthLevel.text = sliderStrength.value.ToString();
            UpdateProjectedPlayerLevel();
        }
        private void UpdateDexterityLevelSlider(float f)
        {
            txtProjectedDexterityLevel.text = sliderDexterity.value.ToString();
            UpdateProjectedPlayerLevel();
        }
        private void UpdateIntelligenceLevelSlider(float f)
        {
            txtProjectedIntelligenceLevel.text = sliderIntelligence.value.ToString();
            UpdateProjectedPlayerLevel();
        }
        private void UpdateFaithLevelSlider(float f)
        {
            txtProjectedFaithLevel.text = sliderFaith.value.ToString();
            UpdateProjectedPlayerLevel();

        }

    }
}

