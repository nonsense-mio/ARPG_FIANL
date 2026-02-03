using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using HT;
/// <summary>
/// 音乐音效管理器
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    //背景音乐播放组件
    private AudioSource bkMusic = null;
    //背景音乐大小
    private float bkMusicValue = 0.8f;

    //管理正在播放的音效的容器
    private List<AudioSource> soundList = new List<AudioSource>();
    //音效音量大小
    private float soundValue = 0.8f;
    //音效是否在播放
    private bool soundIsPlay = true;
    // 添加标志位，防止在清理时Update执行
    private bool isClearing = false;

    private MusicMgr()
    {
        MonoMgr.Instance.AddFixedUpdateListener(Update);
        MusicData data = CurrentGameDataMgr.Instance.musicData;
        soundValue = data.soundValue;
        soundIsPlay = data.soundOpen;
        ChangeBKMusicValue(data.musicValue);
        ChangeSoundValue(data.soundValue);
    }
    private void Update()
    {
        if (!soundIsPlay || isClearing)
            return;

        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            try
            {
                AudioSource source = soundList[i];
                
               
                if (source == null)
                {
                    soundList.RemoveAt(i);
                    continue;
                }

                if (!source.isPlaying)
                {
                    source.clip = null;
                    PoolMgr.Instance.PushObj(source.gameObject);
                    soundList.RemoveAt(i);
                }
            }
            catch (MissingReferenceException)
            {
                // 对象已被销毁，直接移除引用
                soundList.RemoveAt(i);
            }
        }
    }

    //播放背景音乐
    public void PlayBKMusic(string name)
    {
        //动态创建播放背景音乐的组件 并且不会 过场景移除
        //保证背景音乐在过场景时也能播放
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }
        //根据传入的背景音乐名 来播放背景音乐 
        ABResMgr.Instance.LoadResAsync<AudioClip>("music", name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        });

    }
    //停止背景音乐
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }
    //暂停背景音乐
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }
    //继续播放或暂停背景音乐
    public void PlayOrPauseBKMusic(bool isplay)
    {
        if (bkMusic == null)
            return;
        if (isplay)
            bkMusic.Play();
        else bkMusic.Pause();
    }
    //设置背景音乐大小
    public void ChangeBKMusicValue(float v)
    {
        bkMusicValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkMusicValue;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名字</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否同步加载</param>
    /// <param name="callBack">加载结束后的回调</param>
    public void PlaySound(string name, bool isLoop = false, bool isSync = false, UnityAction<AudioSource> callBack = null)
    {
        if (!soundIsPlay)
            return;
        //加载音效资源 进行播放
        ABResMgr.Instance.LoadResAsync<AudioClip>("sound", name, (clip) =>
        {
            //从缓存池中取出音效对象 得到音效组件
            AudioSource source = PoolMgr.Instance.GetObj("Sound/soundObj").GetComponent<AudioSource>();
            //如果取出来的音效组件 是之前正在使用的 我们先停止它
            source.Stop();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.mute = !soundIsPlay;
            source.Play();
            //存储到容器中 方便之后判断是否停止
            //由于从缓存池中去除对象 有可能取出一个之前正在使用的(超上限时)
            //所以需要判断 容器中没有记录时再去记录 不要重复添加
            if (!soundList.Contains(source))
                soundList.Add(source);
            //传递给外部使用
            callBack?.Invoke(source);
        }, isSync);
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    /// <param name="source">音效组件对象</param>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            //停止播放
            source.Stop();
            //从容器中移除
            soundList.Remove(source);
            //将音效组件依附的对象放回缓存池中
            source.clip = null;
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }

    /// <summary>
    /// 改变音效大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeSoundValue(float v)
    {
        soundValue = v;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = soundValue;
        }
    }

    /// <summary>
    /// 继续播放或暂停所有音效
    /// </summary>
    /// <param name="isPlay">是否继续播放 true为播放  false为暂停</param>
    public void PlayOrPauseSound(bool isPlay)
    {
        if (isPlay)
        {
            soundIsPlay = true;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Play();
            }
        }
        else
        {
            soundIsPlay = false;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Pause();
            }
        }
    }
    /// <summary>
    /// 清空音效 相关记录  过场景时在清空缓存池之前去调用它
    /// </summary>
    public void ClearSound()
    {
        isClearing = true;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        //清空音效列表

        soundList.Clear();
        isClearing = false;

    }
}
