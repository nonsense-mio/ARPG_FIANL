namespace ARPG
{
    /// <summary>
    /// AOT/热更边界接口 — Bootstrap 唯一依赖的游戏入口契约。
    /// 当前由 AOT 的 GameLauncher 实现；将 GameLauncher 迁移到 HotUpdate 后
    /// Main.cs 无需任何修改，反射会自动找到热更版本。
    /// </summary>
    public interface IGameLauncher
    {
        void Launch();
    }
}
