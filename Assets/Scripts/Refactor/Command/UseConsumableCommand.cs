using Framework;

namespace ARPG
{
    public class UseConsumableCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var player = PlayerManager.localPlayer;
            if (player == null) return;

            var consumable = player.playerInventoryManager.currentConsumable;
            if (consumable != null)
            {
                consumable.AttemptToConsumeItem(player);
            }
        }
    }
}
