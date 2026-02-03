using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT
{
    public class SettingPanel : BasePanel
    {

        private void Start()
        {
            UIMgr.AddUISelectSound(GetControl<Button>("btnClose"));
            UIMgr.AddUISelectSound(GetControl<Toggle>("togMusic"));
            UIMgr.AddUISelectSound(GetControl<Toggle>("togSound"));
            UIMgr.AddUIConfirmSound(GetControl<Button>("btnClose"));
            UIMgr.AddUIConfirmSound(GetControl<Toggle>("togMusic"));
            UIMgr.AddUIConfirmSound(GetControl<Toggle>("togSound"));
        }
        public override void ShowMe()
        {
            //根据存储的音乐相关数据来初始化面板显示内容
            MusicData data = CurrentGameDataMgr.Instance.musicData;
            GetControl<Toggle>("togMusic").isOn = data.musicOpen;
            GetControl<Toggle>("togSound").isOn = data.soundOpen;
            GetControl<Slider>("sliderMusic").value = data.musicValue;
            GetControl<Slider>("sliderSound").value = data.soundValue;
        }

        public override void HideMe()
        {
            CurrentGameDataMgr.Instance.SaveMusicData();
        }
        protected override void ClickBtn(string btnName)
        {
            //关闭设置面板
            switch (btnName)
            {
                case "btnClose":
                    UIMgr.Instance.HidePanel<SettingPanel>();
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
                    MusicMgr.Instance.ChangeBKMusicValue(value);
                    CurrentGameDataMgr.Instance.musicData.musicValue = value;
                    break;
                case "sliderSound":
                    MusicMgr.Instance.ChangeSoundValue(value);
                    CurrentGameDataMgr.Instance.musicData.soundValue = value;
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
                    MusicMgr.Instance.PlayOrPauseBKMusic(value);
                    CurrentGameDataMgr.Instance.musicData.musicOpen = value;
                    break;
                case "togSound":
                    MusicMgr.Instance.PlayOrPauseSound(value);
                    CurrentGameDataMgr.Instance.musicData.soundOpen = value;
                    break;
            }
        }


    }
}

