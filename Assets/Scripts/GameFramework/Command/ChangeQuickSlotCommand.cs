using Framework;

namespace ARPG
{
    public enum E_QuickSlotType
    {
        RightWeapon,
        LeftWeapon,
        Consumable,
        Spell
    }

    public class ChangeQuickSlotCommand : AbstractCommand
    {
        private readonly E_QuickSlotType slotType;
        public ChangeQuickSlotCommand(){}

        public ChangeQuickSlotCommand(E_QuickSlotType slotType)
        {
            this.slotType = slotType;
        }

        protected override void OnExecute()
        {
            var player = PlayerManager.localPlayer;
            if (player == null) return;

            var inventory = player.playerInventoryManager;
            var model = this.GetModel<IInventoryModel>();

            switch (slotType)
            {
                case E_QuickSlotType.RightWeapon:
                    inventory.ChangeRightWeapon();
                    player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
                    model.CurrentRightWeaponIndex.Value = inventory.currentRightWeaponIndex;
                    break;
                case E_QuickSlotType.LeftWeapon:
                    inventory.ChangeLeftWeapon();
                    player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
                    model.CurrentLeftWeaponIndex.Value = inventory.currentLeftWeaponIndex;
                    break;
                case E_QuickSlotType.Consumable:
                    inventory.ChangeConsumable();
                    model.CurrentConsumableIndex.Value = inventory.currentConsumableIndex;
                    break;
                case E_QuickSlotType.Spell:
                    inventory.ChangeSpell();
                    model.CurrentSpellIndex.Value = inventory.currentSpellIndex;
                    break;
            }
        }
    }
}
