using ARPG;
using Framework;
using UnityEngine.UI;

namespace HT
{
    public class SettingPanel : BasePanel
    {
        private IMusicSystem musicSystem;
        private IMusicModel musicModel;

        private void Start()
        {
            AddUISelectSound(GetControl<Button>("btnClose"));
            AddUISelectSound(GetControl<Toggle>("togMusic"));
            AddUISelectSound(GetControl<Toggle>("togSound"));
            AddUIConfirmSound(GetControl<Button>("btnClose"));
            AddUIConfirmSound(GetControl<Toggle>("togMusic"));
            AddUIConfirmSound(GetControl<Toggle>("togSound"));
        }
        public override void ShowMe()
        {
            musicSystem = this.GetSystem<IMusicSystem>();
            musicModel = this.GetModel<IMusicModel>();
            //根据存储的音乐相关数据来初始化面板显示内容
            GetControl<Toggle>("togMusic").isOn = musicModel.MusicOpen.Value;
            GetControl<Toggle>("togSound").isOn = musicModel.SoundOpen.Value;
            GetControl<Slider>("sliderMusic").value = musicModel.MusicValue.Value;
            GetControl<Slider>("sliderSound").value = musicModel.SoundValue.Value;
        }

        public override void HideMe()
        {
            musicModel.SaveMusicSettings();
        }
        protected override void ClickBtn(string btnName)
        {
            //关闭设置面板
            switch (btnName)
            {
                case "btnClose":
                    this.GetSystem<IUISystem>().HidePanel<SettingPanel>();
                    break;
            }
        }
        protected override void SliderValueChange(string sliderName, float value)
        {
            //设置音乐音量大小
            //记录音量的数据
            switch (sliderName)
            {
                case "sliderMusic":
                    musicSystem.SetBGMVolume(value);
                    musicModel.MusicValue.Value = value;
                    break;
                case "sliderSound":
                    musicSystem.SetSoundVolume(value);
                    musicModel.SoundValue.Value = value;
                    break;
            }
        }
        protected override void ToggleValueChange(string togName, bool value)
        {
            //设置音乐的开关
            //记录开关的数据
            switch (togName)
            {
                case "togMusic":
                    musicSystem.SetBGMPlaying(value);
                    musicModel.MusicOpen.Value = value;
                    break;
                case "togSound":
                    musicSystem.SetSoundPlaying(value);
                    musicModel.SoundOpen.Value = value;
                    break;
            }
        }
    }
}
