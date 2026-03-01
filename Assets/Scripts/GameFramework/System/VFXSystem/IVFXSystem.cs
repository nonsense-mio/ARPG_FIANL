using Framework;

namespace ARPG
{
    /// <summary>
    /// VFX 特效系统接口 - 纯事件驱动，无需对外暴露方法
    /// 注册到架构中是为了让框架管理其生命周期和事件订阅
    /// </summary>
    public interface IVFXSystem : ISystem { }
}
