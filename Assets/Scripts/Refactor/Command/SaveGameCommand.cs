using Framework;
using HT;

namespace ARPG
{
    public class SaveGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var player = PlayerManager.localPlayer;
            if (player != null)
                player.playerSaveManager.SyncRuntimeToModel();

            this.GetSystem<ISaveSystem>().SaveCurrentGame();
        }
    }
}
