using Framework;

namespace ARPG
{
    public class ContinueGameCommand : AbstractCommand
    {
        private int slotIndex;
        public ContinueGameCommand(){}
        public ContinueGameCommand(int slotIndex)
        {
            this.slotIndex = slotIndex;
        }

        protected override void OnExecute()
        {
            if (!this.GetSystem<ISaveSystem>().LoadGame(slotIndex))
                return;

            this.GetSystem<ISceneSystem>().LoadSceneAsync("GameScene", () =>
            {
                GameArchitecture.Interface.SendCommand(new InitGameSceneCommand());
            });
        }
    }
}
