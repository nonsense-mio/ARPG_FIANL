namespace ARPG
{
    /// <summary>
    /// AOT↔热更唯一契约 — Bootstrap(AOT) 仅依赖此接口。
    /// Main.cs 通过反射在热更程序集中找到 GameLauncher 实现并调用 Launch()。
    /// 启动接线（访问架构、注册事件、进入首场景）全部在热更侧的 GameLauncher 内完成，
    /// 故此接口无参、且不依赖 Framework —— 保证 AOT 引导层与 QFramework 彻底解耦。
    /// </summary>
    public interface IGameLauncher
    {
        void Launch();
    }
}
