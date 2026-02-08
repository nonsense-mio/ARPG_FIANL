using System;
using Framework;

namespace ARPG
{
    /// <summary>
    /// 场景系统接口 - 替代 SceneMgr (BaseManager 单例)
    /// </summary>
    public interface ISceneSystem : ISystem
    {
        void LoadScene(string sceneName, Action callback = null);
        void LoadSceneAsync(string sceneName, Action callback);
    }
}
