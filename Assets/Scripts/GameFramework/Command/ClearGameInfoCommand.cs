using Framework;

namespace ARPG
{
    public class ClearGameInfoCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<ISaveSystem>().StopPlayTimer();
            this.GetSystem<IMusicSystem>().ClearAllSounds();
            this.GetSystem<IPoolSystem>().ClearAllPools();
            this.GetSystem<ITaskSystem>().ClearTasks();
            this.GetUtility<IAssetLoader>().UnloadAll();
        }
    }
}
