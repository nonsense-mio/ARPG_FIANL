using Framework;
using UnityEngine;

namespace ARPG
{
    public class GameArchitecture : Architecture<GameArchitecture>
    {
        protected override void Init()
        {
            // 注册 Utility
            RegisterUtility<IStorage>(new JsonStorage());
            RegisterUtility<IAssetSystem>(new AssetSystem());

            // 注册 Model
            RegisterModel<IPlayerModel>(new PlayerModel());
            RegisterModel<IInventoryModel>(new InventoryModel());
            RegisterModel<ITaskModel>(new TaskModel());
            RegisterModel<ISceneStateModel>(new SceneStateModel());

            // 注册 System
            RegisterSystem<ITaskSystem>(new TaskSystem());
            RegisterSystem<IPoolSystem>(new PoolSystem());
            RegisterSystem<ITickSystem>(new TickSystem());
            RegisterSystem<ITimerSystem>(new TimerSystem());
            RegisterSystem<IVFXSystem>(new VFXSystem());
            RegisterSystem<IMusicSystem>(new MusicSystem());
            RegisterSystem<ISoundSystem>(new SoundSystem());
            RegisterSystem<ISaveSystem>(new SaveSystem());
        }
    }
}