using Framework;

namespace ARPG
{
    public class ToggleTwoHandCommand : AbstractCommand
    {
        private readonly bool enableTwoHand;
        public ToggleTwoHandCommand(){}

        public ToggleTwoHandCommand(bool enableTwoHand)
        {
            this.enableTwoHand = enableTwoHand;
        }

        protected override void OnExecute()
        {
            var player = PlayerManager.localPlayer;
            if (player == null) return;

            player.isTwoHandingWeapon = enableTwoHand;

            if (enableTwoHand)
            {
                player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                player.playerWeaponSlotManager.LoadTwoHandIKTargets(true);
            }
            else
            {
                player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.leftWeapon, true);
                player.playerWeaponSlotManager.LoadTwoHandIKTargets(false);
            }
        }
    }
}
