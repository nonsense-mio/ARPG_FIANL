using System;
using System.Collections;
using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// Tick 系统实现 - 通过内部 MonoBehaviour 获取 Unity 生命周期回调 + 协程托管
    /// 替代 MonoMgr 的 Update 事件分发 + StartCoroutine 职责
    /// </summary>
    public class TickSystem : AbstractSystem, ITickSystem
    {
        private event Action updateEvent;
        private event Action fixedUpdateEvent;
        private event Action lateUpdateEvent;

        private TickBehaviour behaviour;

        protected override void OnInit()
        {
            var go = new GameObject("[TickSystem]");
            GameObject.DontDestroyOnLoad(go);
            behaviour = go.AddComponent<TickBehaviour>();
            behaviour.owner = this;
        }

        public void RegisterUpdate(Action onUpdate) => updateEvent += onUpdate;
        public void UnregisterUpdate(Action onUpdate) => updateEvent -= onUpdate;

        public void RegisterFixedUpdate(Action onFixedUpdate) => fixedUpdateEvent += onFixedUpdate;
        public void UnregisterFixedUpdate(Action onFixedUpdate) => fixedUpdateEvent -= onFixedUpdate;

        public void RegisterLateUpdate(Action onLateUpdate) => lateUpdateEvent += onLateUpdate;
        public void UnregisterLateUpdate(Action onLateUpdate) => lateUpdateEvent -= onLateUpdate;

        public Coroutine StartCoroutine(IEnumerator routine) => behaviour.StartCoroutine(routine);
        public void StopCoroutine(Coroutine coroutine) => behaviour.StopCoroutine(coroutine);

        private class TickBehaviour : MonoBehaviour
        {
            internal TickSystem owner;

            private void Update() => owner.updateEvent?.Invoke();
            private void FixedUpdate() => owner.fixedUpdateEvent?.Invoke();
            private void LateUpdate() => owner.lateUpdateEvent?.Invoke();
        }
    }
}
