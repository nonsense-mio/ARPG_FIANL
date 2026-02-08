using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

namespace HT
{
    public class GameManager : BaseManager<GameManager>
    {
        public PlayerManager player;
        private IMusicSystem musicSystem;
        private IMusicSystem MusicSystem =>
            musicSystem ?? (musicSystem = GameArchitecture.Interface.GetSystem<IMusicSystem>());
        private ISaveSystem saveSystem;
        private ISaveSystem SaveSystem =>
            saveSystem ?? (saveSystem = GameArchitecture.Interface.GetSystem<ISaveSystem>());
        private IAssetSystem assetSystem;
        private IAssetSystem AssetSystem =>
            assetSystem ?? (assetSystem = GameArchitecture.Interface.GetUtility<IAssetSystem>());
        private ISceneSystem sceneSystem;
        private ISceneSystem SceneSystem =>
            sceneSystem ?? (sceneSystem = GameArchitecture.Interface.GetSystem<ISceneSystem>());
        private IUISystem uiSystem;
        private IUISystem UISystem =>
            uiSystem ?? (uiSystem = GameArchitecture.Interface.GetSystem<IUISystem>());

        private GameManager()
        {

        }

        /// <summary>
        /// 切换场景时初始化游戏信息
        /// 注意：在调用此方法前，应该已经通过 ISaveSystem 加载了存档数据
        /// </summary>
        public void InitGameScene()
        {
            LoadNPC();
            //更换背景音乐
            MusicSystem.PlayBGM("GameScene1");
            GameObject cameraObj = null;
            //同步加载摄像机 并实例化
            AssetSystem.LoadAssetAsync<GameObject>("character", "PlayerCamera", (obj) =>
            {
                cameraObj = GameObject.Instantiate(obj);
            }, true);
            CameraMgr cameraMgr = cameraObj.GetComponent<CameraMgr>();

            //同步加载玩家并实例化玩家
            AssetSystem.LoadAssetAsync<GameObject>("character", "Player", (obj) =>
            {
                GameObject playerObj = GameObject.Instantiate(obj);
                player = playerObj.GetComponent<PlayerManager>();
                //更新玩家数据到场景中
                player.playerSaveManager.SyncFromModel();

                // 恢复任务数据（需要在场景加载完成后调用，确保 TaskGiver 已存在）
                var allTasks = CollectAllTasksFromScene();
                var taskSystem = GameArchitecture.Interface.GetSystem<ITaskSystem>();
                taskSystem.RebuildFromModel(allTasks);

                // 通知所有监听者数据加载完成
                GameArchitecture.Interface.SendEvent(new GameDataLoadedEvent());
            }, true);

            //设置摄像机脚本参数
            cameraMgr.targetTransform = player.transform;
            cameraMgr.targetTransformWhileAiming = player.targetTransformWhileAiming;
            cameraMgr.inputMgr = player.inputMgr;
            cameraMgr.playerManager = player;

            //显示游戏面板
            UISystem.ShowPanel<GamePanel>();
            UISystem.ShowPanel<InteractionPanel>();



            // 开始游戏时长计时
            SaveSystem.StartPlayTimer();
        }
        private void LoadNPC()
        {
            AssetSystem.LoadAssetAsync<GameObject>("character", "CiriPos", (obj) =>
            {
                AssetSystem.LoadAssetAsync<GameObject>("character", "Ciri", (prefab) =>
                {
                    GameObject npcObj = GameObject.Instantiate(prefab, obj.transform.position, obj.transform.rotation);
                }, true);
            }, true);
            AssetSystem.LoadAssetAsync<GameObject>("character", "GeraltPos", (obj) =>
            {
                AssetSystem.LoadAssetAsync<GameObject>("character", "Geralt", (prefab) =>
                {
                    GameObject npcObj = GameObject.Instantiate(prefab, obj.transform.position, obj.transform.rotation);
                }, true);
            }, true);
        }
        /// <summary>
        /// 开始新游戏
        /// </summary>
        public void StartNewGame(int slotIndex, string playerName = "Player")
        {
            SaveSystem.CreateNewGame(slotIndex, playerName);
            SceneSystem.LoadSceneAsync("GameScene", InitGameScene);
        }

        /// <summary>
        /// 继续游戏（加载指定槽位）
        /// </summary>
        public bool ContinueGame(int slotIndex)
        {
            if (!SaveSystem.LoadGame(slotIndex))
                return false;

            SceneSystem.LoadSceneAsync("GameScene", InitGameScene);
            return true;
        }

        /// <summary>
        /// 保存当前游戏
        /// </summary>
        public void SaveCurrentGame()
        {
            if (player != null)
                player.playerSaveManager.SyncRuntimeToModel();

            SaveSystem.SaveCurrentGame();
        }

        //初始化开始场景
        public void InitBeginScene()
        {
            MusicSystem.PlayBGM("BeginScene");
            UISystem.ShowPanel<BeginPanel>();
        }

        /// <summary>
        /// 从场景中所有 TaskGiver 收集所有任务 SO
        /// </summary>
        private static List<TaskData_SO> CollectAllTasksFromScene()
        {
            List<TaskData_SO> allTasks = new List<TaskData_SO>();
            var allTaskGivers = Object.FindObjectsOfType<TaskGiver>();

            foreach (var giver in allTaskGivers)
            {
                foreach (var node in giver.taskChain)
                {
                    var task = node.GetTask();
                    if (task != null && !allTasks.Exists(t => t.taskName == task.taskName))
                    {
                        allTasks.Add(task);
                    }
                }
            }

            return allTasks;
        }

        //切换场景时清理信息
        public void ClearInfo()
        {
            // 停止游戏时长计时
            SaveSystem.StopPlayTimer();

            MusicSystem.ClearAllSounds();
            GameArchitecture.Interface.GetSystem<IPoolSystem>().ClearAllPools();
            GameArchitecture.Interface.GetSystem<ITaskSystem>().ClearTasks();
        }
    }
}

