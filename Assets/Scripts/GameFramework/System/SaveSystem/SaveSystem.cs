using System.IO;
using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 存档系统 - 管理多槽位存档的读写和游戏时长计时
    /// 替代原 SaveMgr (BaseManager 单例)
    /// </summary>
    public class SaveSystem : AbstractSystem, ISaveSystem
    {
        private const string SLOT_INFO_FILE = "SaveSlotInfo";
        private const string PLAYER_DATA_FILE = "PlayerData";
        private const string INVENTORY_DATA_FILE = "PlayerInventoryData";
        private const string TASK_DATA_FILE = "TaskData";
        private const string SCENE_STATE_FILE = "SceneStateData";

        public int CurrentSlotIndex { get; private set; } = -1;
        public SaveSlotInfo SlotInfo { get; private set; }
        public float CurrentPlayTime { get; private set; } = 0f;
        public bool IsPlaying { get; private set; } = false;

        private IStorage storage;
        private ITickSystem tickSystem;

        protected override void OnInit()
        {
            storage = this.GetUtility<IStorage>();
            tickSystem = this.GetSystem<ITickSystem>();
            SlotInfo = storage.LoadData<SaveSlotInfo>(SLOT_INFO_FILE);

            // 任务关键节点自动保存
            this.RegisterEvent<TaskStartedEvent>(_ => SaveTaskData());
            this.RegisterEvent<TaskCompletedEvent>(_ => SaveTaskData());
            this.RegisterEvent<TaskTurnedInEvent>(_ => SaveTaskData());
        }

        public bool HasAnySave() => SlotInfo.HasAnySave();
        public SaveSlotData GetSlotData(int index) => SlotInfo.GetSlot(index);

        /// <summary>
        /// 创建新游戏：初始化默认数据并保存到指定槽位
        /// </summary>
        public void CreateNewGame(int slotIndex, string playerName = "Player")
        {
            CurrentSlotIndex = slotIndex;
            SlotInfo.lastUsedSlot = slotIndex;
            CurrentPlayTime = 0f;

            // 创建默认数据 → 直接加载到 Model 层
            this.GetModel<IPlayerModel>().LoadData(new PlayerData { playerName = playerName });
            this.GetModel<IInventoryModel>().LoadData(new PlayerInventoryData());
            this.GetModel<ITaskModel>().LoadData(new TaskData());
            this.GetModel<ISceneStateModel>().LoadData(new SceneStateData());

            SaveCurrentGame();
        }

        /// <summary>
        /// 加载存档：从指定槽位读取数据到 Model 层
        /// </summary>
        public bool LoadGame(int slotIndex)
        {
            if (SlotInfo.IsSlotEmpty(slotIndex))
            {
                Debug.LogWarning($"SaveSystem: 槽位 {slotIndex} 为空");
                return false;
            }

            CurrentSlotIndex = slotIndex;
            SlotInfo.lastUsedSlot = slotIndex;

            // 从文件加载 transient data → 直接加载到 Model 层
            string suffix = $"_{slotIndex}";
            this.GetModel<IPlayerModel>().LoadData(
                storage.LoadData<PlayerData>(PLAYER_DATA_FILE + suffix));
            this.GetModel<IInventoryModel>().LoadData(
                storage.LoadData<PlayerInventoryData>(INVENTORY_DATA_FILE + suffix));
            this.GetModel<ITaskModel>().LoadData(
                storage.LoadData<TaskData>(TASK_DATA_FILE + suffix));
            this.GetModel<ISceneStateModel>().LoadData(
                storage.LoadData<SceneStateData>(SCENE_STATE_FILE + suffix));

            // 恢复游戏时长
            CurrentPlayTime = SlotInfo.GetSlot(slotIndex)?.playTimeSeconds ?? 0f;

            storage.SaveData(SlotInfo, SLOT_INFO_FILE);
            return true;
        }

        /// <summary>
        /// 保存当前游戏：从 Model 层导出数据写入当前槽位
        /// </summary>
        public void SaveCurrentGame()
        {
            if (CurrentSlotIndex < 0)
            {
                Debug.LogError("SaveSystem: 未选择存档槽位");
                return;
            }

            // 导出所有 Model → transient data objects
            var playerData = new PlayerData();
            this.GetModel<IPlayerModel>().SaveData(playerData);

            var inventoryData = new PlayerInventoryData();
            this.GetModel<IInventoryModel>().SaveData(inventoryData);

            // 同步 TaskSystem 运行时状态到 TaskModel
            var taskSystem = this.GetSystem<ITaskSystem>();
            taskSystem.SyncToModel();

            // 收集 TaskGiver 进度到 TaskModel
            var taskModel = this.GetModel<ITaskModel>();
            foreach (var giver in Object.FindObjectsOfType<TaskGiver>())
                taskModel.SetGiverProgress(giver.GetGiverId(), giver.CurrentTaskIndex);

            // 导出 TaskModel
            var taskData = new TaskData();
            taskModel.SaveData(taskData);

            var sceneStateData = new SceneStateData();
            this.GetModel<ISceneStateModel>().SaveData(sceneStateData);

            // 更新槽位元数据（用于UI显示）
            SaveSlotData slot = SlotInfo.GetSlot(CurrentSlotIndex);

            slot.isEmpty = false;
            slot.playerName = playerData.playerName;
            slot.playerLevel = playerData.playerLevel;
            slot.currentSouls = playerData.currentSoulCount;
            slot.maxHealth = playerData.healthLevel * 10;
            slot.maxStamina = playerData.staminaLevel * 10;
            slot.maxFocus = playerData.focusLevel * 10;
            slot.playTimeSeconds = CurrentPlayTime;
            slot.UpdateSaveTime();
            // 保存最后使用的槽位索引
            SlotInfo.lastUsedSlot = CurrentSlotIndex;

            // 保存到文件
            string suffix = $"_{CurrentSlotIndex}";
            storage.SaveData(playerData, PLAYER_DATA_FILE + suffix);
            storage.SaveData(inventoryData, INVENTORY_DATA_FILE + suffix);
            storage.SaveData(taskData, TASK_DATA_FILE + suffix);
            storage.SaveData(sceneStateData, SCENE_STATE_FILE + suffix);
            storage.SaveData(SlotInfo, SLOT_INFO_FILE);
        }

        /// <summary>
        /// 局部保存：仅写入任务数据到当前槽位
        /// </summary>
        public void SaveTaskData()
        {
            if (CurrentSlotIndex < 0) return;

            // 同步 TaskSystem 运行时状态到 TaskModel
            var taskSystem = this.GetSystem<ITaskSystem>();
            taskSystem.SyncToModel();

            // 收集 TaskGiver 进度
            var taskModel = this.GetModel<ITaskModel>();
            foreach (var giver in Object.FindObjectsOfType<TaskGiver>())
                taskModel.SetGiverProgress(giver.GetGiverId(), giver.CurrentTaskIndex);

            // 导出并写入文件
            var taskData = new TaskData();
            taskModel.SaveData(taskData);
            storage.SaveData(taskData, TASK_DATA_FILE + $"_{CurrentSlotIndex}");
        }

        /// <summary>
        /// 局部保存：仅写入背包数据到当前槽位
        /// </summary>
        public void SaveInventoryData()
        {
            if (CurrentSlotIndex < 0) return;

            var inventoryData = new PlayerInventoryData();
            this.GetModel<IInventoryModel>().SaveData(inventoryData);
            storage.SaveData(inventoryData, INVENTORY_DATA_FILE + $"_{CurrentSlotIndex}");
        }

        /// <summary>
        /// 删除指定槽位的存档
        /// </summary>
        public void DeleteSave(int slotIndex)
        {
            SlotInfo.GetSlot(slotIndex)?.Reset();

            // 删除文件
            string suffix = $"_{slotIndex}";
            DeleteFile(PLAYER_DATA_FILE + suffix);
            DeleteFile(INVENTORY_DATA_FILE + suffix);
            DeleteFile(TASK_DATA_FILE + suffix);
            DeleteFile(SCENE_STATE_FILE + suffix);

            if (SlotInfo.lastUsedSlot == slotIndex)
                SlotInfo.lastUsedSlot = -1;

            storage.SaveData(SlotInfo, SLOT_INFO_FILE);
        }

        public void StartPlayTimer()
        {
            IsPlaying = true;
            tickSystem.RegisterUpdate(UpdatePlayTime);
        }

        public void StopPlayTimer()
        {
            IsPlaying = false;
            tickSystem.UnregisterUpdate(UpdatePlayTime);
        }

        private void UpdatePlayTime()
        {
            if (IsPlaying) CurrentPlayTime += Time.deltaTime;
        }

        private void DeleteFile(string fileName)
        {
            string path = $"{Application.persistentDataPath}/{fileName}.json";
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
