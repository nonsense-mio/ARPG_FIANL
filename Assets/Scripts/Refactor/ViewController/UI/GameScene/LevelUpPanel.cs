using ARPG;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace ARPG
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

        //确认升级
        public void ConfirmPlayerLevelUpStats()
        {
            int newHealthLevel = Mathf.RoundToInt(sliderHealth.value);
            int newStaminaLevel = Mathf.RoundToInt(sliderStamina.value);
            int newFocusLevel = Mathf.RoundToInt(sliderFocus.value);
            int newPoiseLevel = Mathf.RoundToInt(sliderPoise.value);
            int newStrengthLevel = Mathf.RoundToInt(sliderStrength.value);
            int newDexterityLevel = Mathf.RoundToInt(sliderDexterity.value);
            int newIntelligenceLevel = Mathf.RoundToInt(sliderIntelligence.value);
            int newFaithLevel = Mathf.RoundToInt(sliderFaith.value);
            int newSoulCount = playerModel.CurrentSoulCount - soulsRequiredToLevelUp;

            // 1) 写入 PlayerModel (驱动 GamePanel 响应式更新)
            playerModel.PlayerLevel.Value = projectedPlayerLevel;
            playerModel.HealthLevel.Value = newHealthLevel;
            playerModel.StaminaLevel.Value = newStaminaLevel;
            playerModel.FocusLevel.Value = newFocusLevel;
            playerModel.PoiseLevel.Value = newPoiseLevel;
            playerModel.StrengthLevel.Value = newStrengthLevel;
            playerModel.DexterityLevel.Value = newDexterityLevel;
            playerModel.IntelligenceLevel.Value = newIntelligenceLevel;
            playerModel.FaithLevel.Value = newFaithLevel;
            playerModel.CurrentSoulCount.Value = newSoulCount;

            // 2) 写入 PlayerStatsManager (运行时逻辑)
            var pm = PlayerManager.localPlayer;
            if (pm != null)
            {
                var stats = pm.playerStatsManager;
                stats.playerLevel = projectedPlayerLevel;
                stats.healthLevel = newHealthLevel;
                stats.staminaLevel = newStaminaLevel;
                stats.focusLevel = newFocusLevel;
                stats.poiseLevel = newPoiseLevel;
                stats.strengthLevel = newStrengthLevel;
                stats.dexterityLevel = newDexterityLevel;
                stats.intelligenceLevel = newIntelligenceLevel;
                stats.faithLevel = newFaithLevel;
                stats.currentSoulCount = newSoulCount;

                // 重新计算 max 值
                stats.SetMaxHealthFromHealthLevel();
                stats.SetMaxStaminaFromStaminaLevel();
                stats.SetMaxFocusPointsFromFocusLevel();

                // 同步 max 值到 Model (GamePanel 自动更新)
                playerModel.MaxHP.Value = stats.maxHealth;
                playerModel.MaxStamina.Value = (int)stats.maxStamina;
                playerModel.MaxFocus.Value = (int)stats.maxFocus;
            }

            this.GetSystem<IUISystem>().HidePanel<LevelUpPanel>();
            this.GetSystem<IUISystem>().ShowPanel<DialoguePanel>();
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
            soulsRequiredToLevelUp = 0;

            projectedPlayerLevel = currentPlayerLevel;
            projectedPlayerLevel += Mathf.RoundToInt(sliderHealth.value - playerModel.HealthLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderStamina.value - playerModel.StaminaLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderFocus.value - playerModel.FocusLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderPoise.value - playerModel.PoiseLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderStrength.value - playerModel.StrengthLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderDexterity.value - playerModel.DexterityLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderIntelligence.value - playerModel.IntelligenceLevel);
            projectedPlayerLevel += Mathf.RoundToInt(sliderFaith.value - playerModel.FaithLevel);

            txtProjectedPlayerLevel.text = projectedPlayerLevel.ToString();
            CalculateSoulCostToLevelUp();
            txtSoulsRequired.text = soulsRequiredToLevelUp.ToString();
            //检查是否有足够的魂进行升级
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
    }
}
