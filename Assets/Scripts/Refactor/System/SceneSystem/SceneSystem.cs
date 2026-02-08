using System;
using System.Collections;
using Framework;
using UnityEngine.SceneManagement;

namespace ARPG
{
    /// <summary>
    /// 场景系统实现 - 替代 SceneMgr (BaseManager 单例)
    /// 通过 ITickSystem 协程托管实现异步场景加载
    /// </summary>
    public class SceneSystem : AbstractSystem, ISceneSystem
    {
        private ITickSystem tickSystem;

        protected override void OnInit()
        {
            tickSystem = this.GetSystem<ITickSystem>();
        }

        public void LoadScene(string sceneName, Action callback = null)
        {
            SceneManager.LoadScene(sceneName);
            callback?.Invoke();
        }

        public void LoadSceneAsync(string sceneName, Action callback)
        {
            tickSystem.StartCoroutine(LoadSceneCoroutine(sceneName, callback));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, Action callback)
        {
            var ao = SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
                yield return null;
            callback?.Invoke();
        }
    }
}
