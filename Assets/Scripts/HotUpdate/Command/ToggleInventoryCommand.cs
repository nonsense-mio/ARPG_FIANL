using Framework;

namespace ARPG
{
    public class ToggleInventoryCommand : AbstractCommand
    {
        private readonly bool isOpen;
        public ToggleInventoryCommand(){}

        public ToggleInventoryCommand(bool isOpen)
        {
            this.isOpen = isOpen;
        }

        protected override void OnExecute()
        {
            var uiSystem = this.GetSystem<IUISystem>();
            this.SendEvent(new SelectWindowEvent { IsOpen = isOpen });

            if (isOpen)
            {
                uiSystem.HidePanel<GamePanel>();
            }
            else
            {
                uiSystem.HidePanel<EquipPanel>();
                uiSystem.HidePanel<BagPanel>();
                uiSystem.HidePanel<LevelUpPanel>();
                uiSystem.HidePanel<TaskPanel>();
                uiSystem.ShowPanel<GamePanel>();
            }
        }
    }
}
