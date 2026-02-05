using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class GameManager : BaseManager<GameManager>
    {
        public PlayerManager player;
        private GameManager()
        {

        }

        /// <summary>
        /// 切换场景时初始化游戏信息
        /// 注意：在调用此方法前，应该已经通过 SaveMgr 加载了存档数据
        /// </summary>
        public void InitGameScene()
        {
            LoadNPC();
            //更换背景音乐
            MusicMgr.Instance.PlayBKMusic("GameScene1");
            GameObject cameraObj = null;
            //同步加载摄像机 并实例化
            ABResMgr.Instance.LoadResAsync<GameObject>("character", "PlayerCamera", (obj) =>
            {
                cameraObj = GameObject.Instantiate(obj);
            }, true);
            CameraMgr cameraMgr = cameraObj.GetComponent<CameraMgr>();

            PlayerData playerData = CurrentGameDataMgr.Instance.playerData;

            //同步加载玩家并实例化玩家
            ABResMgr.Instance.LoadResAsync<GameObject>("character", "Player", (obj) =>
            {
                GameObject playerObj = GameObject.Instantiate(obj);
                player = playerObj.GetComponent<PlayerManager>();
                //更新玩家数据到场景中
                player.playerSaveManager.LoadDataFromGameDataMgr();

                // 恢复任务数据（需要在场景加载完成后调用，确保 TaskGiver 已存在）
                TaskSaveHelper.LoadAllTaskData();

                // 通知所有监听者数据加载完成
                EventCenter.Instance.EventTrigger(E_EventType.E_Game_DataLoaded);
            }, true);

            //设置摄像机脚本参数
            cameraMgr.targetTransform = player.transform;
            cameraMgr.targetTransformWhileAiming = player.targetTransformWhileAiming;
            cameraMgr.inputMgr = player.inputMgr;
            cameraMgr.playerManager = player;

            // 初始化VFX管理器
            VFXMgr.Instance.Init();

            //显示游戏面板
            UIMgr.Instance.ShowPanel<GamePanel>();
            UIMgr.Instance.ShowPanel<InteractionPanel>();



            // 开始游戏时长计时
            SaveMgr.Instance.StartPlayTimer();
        }
        private void LoadNPC()
        {
            ABResMgr.Instance.LoadResAsync<GameObject>("character", "CiriPos", (obj) =>
            {
                ABResMgr.Instance.LoadResAsync<GameObject>("character", "Ciri", (prefab) =>
                {
                    GameObject npcObj = GameObject.Instantiate(prefab, obj.transform.position, obj.transform.rotation);
                }, true);
            }, true);
            ABResMgr.Instance.LoadResAsync<GameObject>("character", "GeraltPos", (obj) =>
            {
                ABResMgr.Instance.LoadResAsync<GameObject>("character", "Geralt", (prefab) =>
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
            SaveMgr.Instance.CreateNewGame(slotIndex, playerName);
            SceneMgr.Instance.LoadSceneAsync("GameScene", InitGameScene);
        }

        /// <summary>
        /// 继续游戏（加载指定槽位）
        /// </summary>
        public bool ContinueGame(int slotIndex)
        {
            if (!SaveMgr.Instance.LoadGame(slotIndex))
                return false;

            SceneMgr.Instance.LoadSceneAsync("GameScene", InitGameScene);
            return true;
        }

        /// <summary>
        /// 保存当前游戏
        /// </summary>
        public void SaveCurrentGame()
        {
            if (player != null)
                player.playerSaveManager.SaveDataToGameDataMgr();

            SaveMgr.Instance.SaveCurrentGame();
        }

        //初始化开始场景
        public void InitBeginScene()
        {
            MusicMgr.Instance.PlayBKMusic("BeginScene");
            UIMgr.Instance.ShowPanel<BeginPanel>();
        }

        //切换场景时清理信息
        public void ClearInfo()
        {
            // 停止游戏时长计时
            SaveMgr.Instance.StopPlayTimer();

            MusicMgr.Instance.ClearSound();
            PoolMgr.Instance.ClearPool();
            TaskManager.Instance.ClearTasks();
        }
    }
}

