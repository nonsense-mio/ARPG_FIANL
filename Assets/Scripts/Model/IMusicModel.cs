using Framework;

namespace ARPG
{
    /// <summary>
    /// 音乐设置 Model - 全局设置，非存档绑定
    /// 替代 CurrentGameDataMgr.musicData + SaveMusicData()
    /// </summary>
    public interface IMusicModel : IModel
    {
        BindableProperty<bool> MusicOpen { get; }
        BindableProperty<bool> SoundOpen { get; }
        BindableProperty<float> MusicValue { get; }
        BindableProperty<float> SoundValue { get; }

        /// <summary>
        /// 保存音乐设置到持久化存储（全局设置，不随存档变化）
        /// </summary>
        void SaveMusicSettings();
    }
}
