namespace ARPG
{
    /// <summary>
    /// 热更入口事件 — GameLauncher (HotUpdate) 发送，Main.cs (AOT) 监听。
    /// 桥接热更程序集和 AOT 命令层，避免 HotUpdate 直接依赖 AOT 中的 Command。
    /// </summary>
    public struct LaunchGameEvent { }
}
