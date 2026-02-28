namespace ARPG
{
    /// <summary>
    /// AOT 薄包装实现。
    /// 当需要热更此逻辑时，在 HotUpdate 程序集中创建同名同命名空间的类，
    /// Main.cs 的反射查找会自动优先使用热更版本，此文件可直接删除。
    /// </summary>
    public class GameLauncher : IGameLauncher
    {
        public void Launch()
        {
            GameArchitecture.Interface.SendCommand<TransitionToBeginSceneCommand>();
        }
    }
}
