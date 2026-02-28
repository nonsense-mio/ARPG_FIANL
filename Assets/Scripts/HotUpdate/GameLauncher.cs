using Framework;

namespace ARPG
{
    /// <summary>
    /// 热更版游戏启动器 — 在 HotUpdate 程序集中，可通过热更修改启动逻辑。
    /// Main.cs 通过反射找到此类，调用 Launch(architecture) 启动游戏。
    /// 通过 LaunchGameEvent 桥接到 AOT 层的 Command（TransitionToBeginSceneCommand）。
    /// </summary>
    public class GameLauncher : IGameLauncher
    {
        public void Launch(IArchitecture architecture)
        {
            architecture.SendEvent(new LaunchGameEvent());
        }
    }
}
