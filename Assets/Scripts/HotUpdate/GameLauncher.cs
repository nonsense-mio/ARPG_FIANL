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
            UnityEngine.Debug.Log("[HotUpdate] GameLauncher.Launch() 执行");
            // 注册 Boss 战桥接事件处理器（AOT 发送事件 → HotUpdate 执行 Command）
            architecture.RegisterEvent<ActivateBossFightRequestEvent>(e =>
            {
                architecture.SendCommand(new ActivateBossFightCommand(e.BossFight));
            });

            architecture.RegisterEvent<BossDefeatedRequestEvent>(e =>
            {
                architecture.SendCommand(new BossDefeatedCommand(e.BossFight));
            });

            // 通知 AOT 侧启动游戏（触发 TransitionToBeginSceneCommand）
            architecture.SendEvent(new LaunchGameEvent());
        }
    }
}
