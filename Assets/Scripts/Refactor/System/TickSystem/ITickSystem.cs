using System;
using Framework;

namespace ARPG
{
    /// <summary>
    /// Tick 系统接口 - 提供 Unity Update/FixedUpdate/LateUpdate 生命周期回调注册
    /// 替代 MonoMgr 的 AddUpdateListener 等 API (仅限已迁移的 System 层)
    /// </summary>
    public interface ITickSystem : ISystem
    {
        void RegisterUpdate(Action onUpdate);
        void UnregisterUpdate(Action onUpdate);

        void RegisterFixedUpdate(Action onFixedUpdate);
        void UnregisterFixedUpdate(Action onFixedUpdate);

        void RegisterLateUpdate(Action onLateUpdate);
        void UnregisterLateUpdate(Action onLateUpdate);
    }
}
