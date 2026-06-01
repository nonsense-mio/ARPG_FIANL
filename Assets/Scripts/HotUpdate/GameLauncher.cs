using Framework;

namespace ARPG
{
    /// <summary>
    /// 热更版游戏启动器 — 位于热更程序集，可随热更修改启动逻辑。
    /// Main.cs(AOT) 通过反射找到此类并调用无参 Launch()。
    /// 全部启动接线都在此完成，因此 AOT 引导层无需了解任何 QFramework 类型。
    /// </summary>
    public class GameLauncher : IGameLauncher
    {
        public void Launch()
        {
            UnityEngine.Debug.Log("[HotUpdate] GameLauncher.Launch() 执行");
            // 进入开始场景。游戏逻辑已全在热更程序集内，触发器可直接 SendCommand，
            // 无需任何 AOT↔热更事件桥（原 Boss 桥接已移除）。
            GameArchitecture.Interface.SendCommand<TransitionToBeginSceneCommand>();
        }
    }
}
