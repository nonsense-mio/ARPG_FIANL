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
            RegisterUtility<IAssetLoader>(new AssetBundleLoader());
            RegisterUtility<IResourceLoader>(new ResourceLoader());

            // 注册 Model
            RegisterModel<IPlayerModel>(new PlayerModel());
            RegisterModel<IInventoryModel>(new InventoryModel());
            RegisterModel<ITaskModel>(new TaskModel());
            RegisterModel<ISceneStateModel>(new SceneStateModel());
            RegisterModel<IMusicModel>(new MusicModel());

            // 注册 System
            RegisterSystem<ITaskSystem>(new TaskSystem());
            RegisterSystem<IPoolSystem>(new PoolSystem());
            RegisterSystem<ITickSystem>(new TickSystem());
            RegisterSystem<ISceneSystem>(new SceneSystem());
            RegisterSystem<ITimerSystem>(new TimerSystem());
            RegisterSystem<IVFXSystem>(new VFXSystem());
            RegisterSystem<ISpellSystem>(new SpellSystem());
            RegisterSystem<IMusicSystem>(new MusicSystem());
            RegisterSystem<ISoundSystem>(new SoundSystem());
            RegisterSystem<ISaveSystem>(new SaveSystem());
            RegisterSystem<IUISystem>(new UISystem());
        }
    }
}