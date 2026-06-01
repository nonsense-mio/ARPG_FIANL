using Framework;

namespace ARPG
{
    /// <summary>
    /// 音乐设置 Model 实现 - 全局设置，非存档绑定
    /// OnInit 从 IStorage 加载，SaveMusicSettings 保存
    /// </summary>
    public class MusicModel : AbstractModel, IMusicModel
    {
        private const string MUSIC_DATA_KEY = "MusicData";

        public BindableProperty<bool> MusicOpen { get; } = new BindableProperty<bool>(true);
        public BindableProperty<bool> SoundOpen { get; } = new BindableProperty<bool>(true);
        public BindableProperty<float> MusicValue { get; } = new BindableProperty<float>(0.2f);
        public BindableProperty<float> SoundValue { get; } = new BindableProperty<float>(0.2f);

        protected override void OnInit()
        {
            var data = this.GetUtility<IStorage>().LoadData<MusicData>(MUSIC_DATA_KEY);
            MusicOpen.Value = data.musicOpen;
            SoundOpen.Value = data.soundOpen;
            MusicValue.Value = data.musicValue;
            SoundValue.Value = data.soundValue;
        }

        public void SaveMusicSettings()
        {
            var data = new MusicData
            {
                musicOpen = MusicOpen.Value,
                soundOpen = SoundOpen.Value,
                musicValue = MusicValue.Value,
                soundValue = SoundValue.Value
            };
            this.GetUtility<IStorage>().SaveData(data, MUSIC_DATA_KEY);
        }
    }
}
