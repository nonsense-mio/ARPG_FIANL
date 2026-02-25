using Framework;
using ARPG;

namespace ARPG
{
    public class InitBeginSceneCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<IMusicSystem>().PlayBGM("BeginScene1");
            this.GetSystem<IUISystem>().ShowPanel<BeginPanel>();
        }
    }
}
