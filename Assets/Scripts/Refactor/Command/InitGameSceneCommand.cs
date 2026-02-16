using System.Collections.Generic;
using Framework;
using ARPG;
using UnityEngine;

namespace ARPG
{
    public class InitGameSceneCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var assetLoader = this.GetUtility<IAssetLoader>();
            var musicSystem = this.GetSystem<IMusicSystem>();
            var uiSystem = this.GetSystem<IUISystem>();
            var saveSystem = this.GetSystem<ISaveSystem>();

            LoadNPC(assetLoader);

            musicSystem.PlayBGM("GameScene1");

            GameObject cameraObj = GameObject.Instantiate(assetLoader.LoadSync<GameObject>("character/PlayerCamera"));
            CameraMgr cameraMgr = cameraObj.GetComponent<CameraMgr>();

            GameObject playerObj = GameObject.Instantiate(assetLoader.LoadSync<GameObject>("character/Player"));
            PlayerManager player = playerObj.GetComponent<PlayerManager>();
            player.playerSaveManager.SyncFromModel();

            var allTasks = CollectAllTasksFromScene();
            var taskSystem = this.GetSystem<ITaskSystem>();
            taskSystem.RebuildFromModel(allTasks);

            this.SendEvent(new GameDataLoadedEvent());

            cameraMgr.targetTransform = player.transform;
            cameraMgr.targetTransformWhileAiming = player.targetTransformWhileAiming;
            cameraMgr.inputMgr = player.inputMgr;
            cameraMgr.playerManager = player;

            uiSystem.ShowPanel<GamePanel>();
            uiSystem.ShowPanel<InteractionPanel>();

            saveSystem.StartPlayTimer();
        }

        private static void LoadNPC(IAssetLoader assetLoader)
        {
            var ciriPos = assetLoader.LoadSync<GameObject>("character/CiriPos");
            var ciri = assetLoader.LoadSync<GameObject>("character/Ciri");
            GameObject.Instantiate(ciri, ciriPos.transform.position, ciriPos.transform.rotation);

            var geraltPos = assetLoader.LoadSync<GameObject>("character/GeraltPos");
            var geralt = assetLoader.LoadSync<GameObject>("character/Geralt");
            GameObject.Instantiate(geralt, geraltPos.transform.position, geraltPos.transform.rotation);
        }

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
    }
}
