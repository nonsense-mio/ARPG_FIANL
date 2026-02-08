using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.Events;

namespace ARPG
{
    /// <summary>
    /// 音乐系统实现 - 管理 BGM 和 SFX 的播放/回收
    /// 替代原 MusicMgr (BaseManager 单例)
    /// </summary>
    public class MusicSystem : AbstractSystem, IMusicSystem
    {
        private AudioSource bgmSource;
        private float bgmVolume;

        private readonly List<AudioSource> activeSounds = new List<AudioSource>();
        private float soundVolume;
        private bool soundEnabled;
        private bool isClearing;

        private IPoolSystem poolSystem;
        private IAssetSystem assetSystem;

        private const string SoundObjPoolPath = "Sound/soundObj";
        private const string MusicBundleName = "music";
        private const string SoundBundleName = "sound";

        protected override void OnInit()
        {
            poolSystem = this.GetSystem<IPoolSystem>();
            assetSystem = this.GetUtility<IAssetSystem>();

            // 创建 BGM AudioSource (DontDestroyOnLoad)
            var bgmObj = new GameObject("[MusicSystem] BGM");
            Object.DontDestroyOnLoad(bgmObj);
            bgmSource = bgmObj.AddComponent<AudioSource>();
            bgmSource.loop = true;

            // 从 IMusicModel 读取初始设置
            var musicModel = this.GetModel<IMusicModel>();
            bgmVolume = musicModel.MusicValue.Value;
            soundVolume = musicModel.SoundValue.Value;
            soundEnabled = musicModel.SoundOpen.Value;
            bgmSource.volume = bgmVolume;

            // 注册 Update 检测 SFX 播放完毕
            this.GetSystem<ITickSystem>().RegisterUpdate(OnUpdate);
        }

        private void OnUpdate()
        {
            if (!soundEnabled || isClearing)
                return;

            for (int i = activeSounds.Count - 1; i >= 0; i--)
            {
                try
                {
                    AudioSource source = activeSounds[i];

                    if (source == null)
                    {
                        activeSounds.RemoveAt(i);
                        continue;
                    }

                    if (!source.isPlaying)
                    {
                        source.clip = null;
                        poolSystem.Recycle(source.gameObject);
                        activeSounds.RemoveAt(i);
                    }
                }
                catch (MissingReferenceException)
                {
                    activeSounds.RemoveAt(i);
                }
            }
        }

        #region BGM
        public void PlayBGM(string name)
        {
            assetSystem.LoadAssetAsync<AudioClip>(MusicBundleName, name, (clip) =>
            {
                bgmSource.clip = clip;
                bgmSource.volume = bgmVolume;
                bgmSource.Play();
            });
        }

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void PauseBGM()
        {
            bgmSource.Pause();
        }

        public void SetBGMPlaying(bool isPlaying)
        {
            if (isPlaying)
                bgmSource.Play();
            else
                bgmSource.Pause();
        }

        public void SetBGMVolume(float volume)
        {
            bgmVolume = volume;
            bgmSource.volume = bgmVolume;
        }
        #endregion

        #region SFX
        public void PlaySound(string name, bool isLoop = false, bool isSync = false,
                              UnityAction<AudioSource> callBack = null)
        {
            if (!soundEnabled)
                return;

            assetSystem.LoadAssetAsync<AudioClip>(SoundBundleName, name, (clip) =>
            {
                AudioSource source = poolSystem.Spawn(SoundObjPoolPath).GetComponent<AudioSource>();
                source.Stop();
                source.clip = clip;
                source.loop = isLoop;
                source.volume = soundVolume;
                source.mute = !soundEnabled;
                source.Play();

                if (!activeSounds.Contains(source))
                    activeSounds.Add(source);

                callBack?.Invoke(source);
            }, isSync);
        }

        public void StopSound(AudioSource source)
        {
            if (activeSounds.Contains(source))
            {
                source.Stop();
                activeSounds.Remove(source);
                source.clip = null;
                poolSystem.Recycle(source.gameObject);
            }
        }

        public void SetSoundVolume(float volume)
        {
            soundVolume = volume;
            for (int i = 0; i < activeSounds.Count; i++)
                activeSounds[i].volume = soundVolume;
        }

        public void SetSoundPlaying(bool isPlaying)
        {
            soundEnabled = isPlaying;
            if (isPlaying)
            {
                for (int i = 0; i < activeSounds.Count; i++)
                    activeSounds[i].Play();
            }
            else
            {
                for (int i = 0; i < activeSounds.Count; i++)
                    activeSounds[i].Pause();
            }
        }

        public void ClearAllSounds()
        {
            isClearing = true;
            for (int i = 0; i < activeSounds.Count; i++)
            {
                activeSounds[i].Stop();
                activeSounds[i].clip = null;
                poolSystem.Recycle(activeSounds[i].gameObject);
            }
            activeSounds.Clear();
            isClearing = false;
        }
        #endregion
    }
}
