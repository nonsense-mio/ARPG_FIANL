using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARPG
{
    public class LevelUpPanel : BasePanel, IPointerClickHandler
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

        private IPlayerModel playerModel;

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

            playerModel = this.GetModel<IPlayerModel>();
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
            Init();
            UpdateProjectedPlayerLevel();
            btnLevUp.onClick.AddListener(ConfirmPlayerLevelUpStats);
            btnLevUp.Select();
            btnLevUp.OnSelect(null);
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
            currentPlayerLevel = playerModel.PlayerLevel;
            txtCurrentPlayerLevel.text = currentPlayerLevel.ToString();
            projectedPlayerLevel = playerModel.PlayerLevel;
            txtProjectedPlayerLevel.text = projectedPlayerLevel.ToString();

            sliderHealth.value = playerModel.HealthLevel;
            sliderHealth.minValue = playerModel.HealthLevel;
            sliderHealth.maxValue = 99;
            sliderHealth.wholeNumbers = true;
            txtCurrentHealthLevel.text = playerModel.HealthLevel.ToString();
            txtProjectedHealthLevel.text = playerModel.HealthLevel.ToString();

            sliderStamina.value = playerModel.StaminaLevel;
            sliderStamina.minValue = playerModel.StaminaLevel;
            sliderStamina.maxValue = 99;
            sliderStamina.wholeNumbers = true;
            txtCurrentStaminaLevel.text = playerModel.StaminaLevel.ToString();
            txtProjectedStaminaLevel.text = playerModel.StaminaLevel.ToString();

            sliderFocus.value = playerModel.FocusLevel;
            sliderFocus.minValue = playerModel.FocusLevel;
            sliderFocus.maxValue = 99;
            sliderFocus.wholeNumbers = true;
            txtCurrentFocusLevel.text = playerModel.FocusLevel.ToString();
            txtProjectedFocusLevel.text = playerModel.FocusLevel.ToString();

            sliderPoise.value = playerModel.PoiseLevel;
            sliderPoise.minValue = playerModel.PoiseLevel;
            sliderPoise.maxValue = 99;
            sliderPoise.wholeNumbers = true;
            txtCurrentPoiseLevel.text = playerModel.PoiseLevel.ToString();
            txtProjectedPoiseLevel.text = playerModel.PoiseLevel.ToString();

            sliderStrength.value = playerModel.StrengthLevel;
            sliderStrength.minValue = playerModel.StrengthLevel;
            sliderStrength.maxValue = 99;
            sliderStrength.wholeNumbers = true;
            txtCurrentStrengthLevel.text = playerModel.StrengthLevel.ToString();
            txtProjectedStrengthLevel.text = playerModel.StrengthLevel.ToString();

            sliderDexterity.value = playerModel.DexterityLevel;
            sliderDexterity.minValue = playerModel.DexterityLevel;
            sliderDexterity.maxValue = 99;
            sliderDexterity.wholeNumbers = true;
            txtCurrentDexterityLevel.text = playerModel.DexterityLevel.ToString();
            txtProjectedDexterityLevel.text = playerModel.DexterityLevel.ToString();

            sliderIntelligence.value = playerModel.IntelligenceLevel;
            sliderIntelligence.minValue = playerModel.IntelligenceLevel;
            sliderIntelligence.maxValue = 99;
            sliderIntelligence.wholeNumbers = true;
            txtCurrentIntelligenceLevel.text = playerModel.IntelligenceLevel.ToString();
            txtProjectedIntelligenceLevel.text = playerModel.IntelligenceLevel.ToString();

            sliderFaith.value = playerModel.FaithLevel;
            sliderFaith.minValue = playerModel.FaithLevel;
            sliderFaith.maxValue = 99;
            sliderFaith.wholeNumbers = true;
            txtCurrentSouls.text = playerModel.CurrentSoulCount.ToString();
        }

        //确认升级 (业务逻辑委托给 ConfirmLevelUpCommand)
        public void ConfirmPlayerLevelUpStats()
        {
            this.SendCommand(new ConfirmLevelUpCommand(
                projectedPlayerLevel, soulsRequiredToLevelUp,
                Mathf.RoundToInt(sliderHealth.value),
                Mathf.RoundToInt(sliderStamina.value),
                Mathf.RoundToInt(sliderFocus.value),
                Mathf.RoundToInt(sliderPoise.value),
                Mathf.RoundToInt(sliderStrength.value),
                Mathf.RoundToInt(sliderDexterity.value),
                Mathf.RoundToInt(sliderIntelligence.value),
                Mathf.RoundToInt(sliderFaith.value)));

            this.GetSystem<IUISystem>().HidePanel<LevelUpPanel>();
            this.GetSystem<IUISystem>().ShowPanel<DialoguePanel>();
        }

        //更新目标等级 (计算逻辑委托给 GetLevelUpCostQuery)
        private void UpdateProjectedPlayerLevel()
        {
            var result = this.SendQuery(new GetLevelUpCostQuery(
                Mathf.RoundToInt(sliderHealth.value) - playerModel.HealthLevel,
                Mathf.RoundToInt(sliderStamina.value) - playerModel.StaminaLevel,
                Mathf.RoundToInt(sliderFocus.value) - playerModel.FocusLevel,
                Mathf.RoundToInt(sliderPoise.value) - playerModel.PoiseLevel,
                Mathf.RoundToInt(sliderStrength.value) - playerModel.StrengthLevel,
                Mathf.RoundToInt(sliderDexterity.value) - playerModel.DexterityLevel,
                Mathf.RoundToInt(sliderIntelligence.value) - playerModel.IntelligenceLevel,
                Mathf.RoundToInt(sliderFaith.value) - playerModel.FaithLevel));

            projectedPlayerLevel = result.ProjectedLevel;
            soulsRequiredToLevelUp = result.SoulCost;
            txtProjectedPlayerLevel.text = projectedPlayerLevel.ToString();
            txtSoulsRequired.text = soulsRequiredToLevelUp.ToString();
            btnLevUp.interactable = playerModel.CurrentSoulCount >= soulsRequiredToLevelUp;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("123");
            if (EventSystem.current.currentSelectedGameObject == null)
                btnLevUp.Select();
        }
    }
}
