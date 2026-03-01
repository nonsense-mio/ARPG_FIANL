using Framework;
using ARPG;

namespace ARPG
{
    public class InitBeginSceneCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendCommand(new PlayBGMCommand("BeginScene1"));
            this.GetSystem<IUISystem>().ShowPanel<BeginPanel>();
        }
    }
}
