using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
            yield return 0;
        }
        callBack?.Invoke();
    }
}
