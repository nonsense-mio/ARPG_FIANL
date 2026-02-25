using Framework;

namespace ARPG
{
    /// <summary>
    /// 过渡到开始场景命令 - 加载 BeginScene 并初始化开始界面
    /// 不含任何游戏会话清理逻辑，可安全地在 Boot 流程和游戏结束后调用
    /// </summary>
    public class TransitionToBeginSceneCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<ISceneSystem>().LoadSceneAsync("BeginScene", () =>
            {
                this.SendCommand(new InitBeginSceneCommand());
            });
        }
    }
}
