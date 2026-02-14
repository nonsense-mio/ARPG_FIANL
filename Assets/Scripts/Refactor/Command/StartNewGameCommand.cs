using Framework;

namespace ARPG
{
    public class StartNewGameCommand : AbstractCommand
    {
        private int slotIndex;
        private string playerName;
        public StartNewGameCommand(){}

        public StartNewGameCommand(int slotIndex, string playerName = "Player")
        {
            this.slotIndex = slotIndex;
            this.playerName = playerName;
        }

        protected override void OnExecute()
        {
            this.GetSystem<ISaveSystem>().CreateNewGame(slotIndex, playerName);
            this.GetSystem<ISceneSystem>().LoadSceneAsync("GameScene", () =>
            {
                this.SendCommand(new InitGameSceneCommand());
            });
        }
    }
}
