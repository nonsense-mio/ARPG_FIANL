using Framework;

namespace ARPG
{
    public class PlayBGMCommand : AbstractCommand
    {
        private readonly string bgmName;
        public PlayBGMCommand(){}

        public PlayBGMCommand(string bgmName)
        {
            this.bgmName = bgmName;
        }

        protected override void OnExecute()
        {
            this.GetSystem<IMusicSystem>().PlayBGM(bgmName);
        }
    }
}
