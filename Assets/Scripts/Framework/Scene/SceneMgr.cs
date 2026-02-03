using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HT;
/// <summary>
/// 场景切换管理器
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr() { }

    //同步切换场景的方法
    public void LoadScene(string sceneName,UnityAction callBack = null)
    {
        //切换场景
        SceneManager.LoadScene(sceneName);
        //调用回调
        callBack?.Invoke();
    }
    //异步切换场景的方法
    public void LoadSceneAsync(string sceneName, UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName, callBack));
    }
    IEnumerator ReallyLoadSceneAsync(string sceneName,UnityAction callBack)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);

        while (!ao.isDone)
        {
            //可以在这里利用事件中心模块 每一帧将进度发送给想要得到的地方
            EventCenter.Instance.EventTrigger(E_EventType.E_SceneLoadChange,ao.progress);
            yield return 0;
        }
        //避免最后一帧直接结束了 没有同步1出去
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange, 1);
        callBack?.Invoke();
    }
}
