using Framework;

namespace ARPG
{
    /// <summary>
    /// 返回主菜单命令 - 编排:
    /// 隐藏所有游戏面板 → 保存 → 清理 → 加载主菜单场景 → 初始化
    /// </summary>
    public class ReturnToMainMenuCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var ui = this.GetSystem<IUISystem>();
            ui.HidePanel<InteractionPanel>(true);
            ui.HidePanel<EquipPanel>(true);
            ui.HidePanel<BagPanel>(true);
            ui.HidePanel<GamePanel>(true);
            ui.HidePanel<TaskPanel>(true);
            ui.HidePanel<DialoguePanel>(true);
            ui.HidePanel<LevelUpPanel>(true);

            this.SendCommand(new SaveGameCommand());
            this.SendCommand(new ClearGameInfoCommand());

            this.GetSystem<ISceneSystem>().LoadSceneAsync("BeginScene", () =>
            {
                this.SendCommand(new InitBeginSceneCommand());
            });
        }
    }
}
