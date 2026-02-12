using System.Collections.Generic;
using Framework;
using HT;
using UnityEngine;

namespace ARPG
{
    public class InitGameSceneCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var assetSystem = this.GetUtility<IAssetSystem>();
            var musicSystem = this.GetSystem<IMusicSystem>();
            var uiSystem = this.GetSystem<IUISystem>();
            var saveSystem = this.GetSystem<ISaveSystem>();

            LoadNPC(assetSystem);

            musicSystem.PlayBGM("GameScene1");

            GameObject cameraObj = null;
            assetSystem.LoadAssetAsync<GameObject>("character", "PlayerCamera", (obj) =>
            {
                cameraObj = GameObject.Instantiate(obj);
            }, true);
            CameraMgr cameraMgr = cameraObj.GetComponent<CameraMgr>();

            PlayerManager player = null;
            assetSystem.LoadAssetAsync<GameObject>("character", "Player", (obj) =>
            {
                GameObject playerObj = GameObject.Instantiate(obj);
                player = playerObj.GetComponent<PlayerManager>();
                player.playerSaveManager.SyncFromModel();

                var allTasks = CollectAllTasksFromScene();
                var taskSystem = this.GetSystem<ITaskSystem>();
                taskSystem.RebuildFromModel(allTasks);

                GameArchitecture.Interface.SendEvent(new GameDataLoadedEvent());
            }, true);

            cameraMgr.targetTransform = player.transform;
            cameraMgr.targetTransformWhileAiming = player.targetTransformWhileAiming;
            cameraMgr.inputMgr = player.inputMgr;
            cameraMgr.playerManager = player;

            uiSystem.ShowPanel<GamePanel>();
            uiSystem.ShowPanel<InteractionPanel>();

            saveSystem.StartPlayTimer();
        }

        private static void LoadNPC(IAssetSystem assetSystem)
        {
            assetSystem.LoadAssetAsync<GameObject>("character", "CiriPos", (obj) =>
            {
                assetSystem.LoadAssetAsync<GameObject>("character", "Ciri", (prefab) =>
                {
                    GameObject.Instantiate(prefab, obj.transform.position, obj.transform.rotation);
                }, true);
            }, true);
            assetSystem.LoadAssetAsync<GameObject>("character", "GeraltPos", (obj) =>
            {
                assetSystem.LoadAssetAsync<GameObject>("character", "Geralt", (prefab) =>
                {
                    GameObject.Instantiate(prefab, obj.transform.position, obj.transform.rotation);
                }, true);
            }, true);
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
