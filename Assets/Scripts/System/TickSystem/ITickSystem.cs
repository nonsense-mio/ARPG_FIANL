using System;
using System.Collections;
using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// Tick 系统接口 - 提供 Unity 生命周期回调注册 + 协程托管
    /// </summary>
    public interface ITickSystem : ISystem
    {
        void RegisterUpdate(Action onUpdate);
        void UnregisterUpdate(Action onUpdate);

        void RegisterFixedUpdate(Action onFixedUpdate);
        void UnregisterFixedUpdate(Action onFixedUpdate);

        void RegisterLateUpdate(Action onLateUpdate);
        void UnregisterLateUpdate(Action onLateUpdate);

        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine coroutine);
    }
}
