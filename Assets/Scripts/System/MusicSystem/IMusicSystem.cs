using Framework;
using UnityEngine;
using UnityEngine.Events;

namespace ARPG
{
    /// <summary>
    /// 音乐系统接口 - 管理 BGM 播放、SFX 播放、音量控制
    /// 替代原 MusicMgr (BaseManager 单例)
    /// </summary>
    public interface IMusicSystem : ISystem
    {
        #region BGM
        void PlayBGM(string name);
        void StopBGM();
        void PauseBGM();
        void SetBGMPlaying(bool isPlaying);
        void SetBGMVolume(float volume);
        #endregion

        #region SFX
        void PlaySound(string name, bool isLoop = false, bool isSync = false,
                        UnityAction<AudioSource> callBack = null);
        void StopSound(AudioSource source);
        void SetSoundVolume(float volume);
        void SetSoundPlaying(bool isPlaying);
        void ClearAllSounds();
        #endregion
    }
}
