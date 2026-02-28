using Framework;

namespace ARPG
{
    /// <summary>
    /// AOT/热更边界接口 — Bootstrap 唯一依赖的游戏入口契约。
    /// Main.cs 通过反射找到 HotUpdate 程序集中的实现，传入 IArchitecture 实例。
    /// HotUpdate 的 GameLauncher 通过 IArchitecture 与 AOT 层通信（发事件、访问 Model/System）。
    /// </summary>
    public interface IGameLauncher
    {
        void Launch(IArchitecture architecture);
    }
}
