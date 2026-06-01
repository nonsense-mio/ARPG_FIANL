using Framework;

namespace ARPG
{
    /// <summary>
    /// 返回主菜单命令 - 编排:
    /// 清理所有面板 → 保存 → 清理游戏信息 → 过渡到开始场景
    /// </summary>
    public class ReturnToMainMenuCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendCommand(new SaveGameCommand());
            this.SendCommand(new ClearGameInfoCommand());
            this.GetSystem<ITimerSystem>().ClearAllTimers();
            this.SendCommand(new TransitionToBeginSceneCommand());
        }
    }
}
