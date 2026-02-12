using Framework;
using HT;

namespace ARPG
{
    public class InitBeginSceneCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<IMusicSystem>().PlayBGM("BeginScene");
            this.GetSystem<IUISystem>().ShowPanel<BeginPanel>();
        }
    }
}
