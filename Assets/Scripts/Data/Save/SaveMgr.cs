using System.IO;
using UnityEngine;
using QFramework;
using ARPG;

/// <summary>
/// 存档管理器：负责多槽位存档的读写
/// </summary>
public class SaveMgr : BaseManager<SaveMgr>
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

    private SaveMgr()
    {
        SlotInfo = JsonMgr.Instance.LoadData<SaveSlotInfo>(SLOT_INFO_FILE);
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

        // 初始化默认数据
        CurrentGameDataMgr.Instance.playerData = new PlayerData { playerName = playerName };
        CurrentGameDataMgr.Instance.playerInventoryData = new PlayerInventoryData();
        CurrentGameDataMgr.Instance.taskData = new TaskData();
        CurrentGameDataMgr.Instance.sceneStateData = new SceneStateData();

        // 同步到 QFramework Model 层
        SyncToModels();
        SaveCurrentGame();


    }

    /// <summary>
    /// 加载存档：从指定槽位读取数据到内存
    /// </summary>
    public bool LoadGame(int slotIndex)
    {
        if (SlotInfo.IsSlotEmpty(slotIndex))
        {
            Debug.LogWarning($"SaveMgr: 槽位 {slotIndex} 为空");
            return false;
        }

        CurrentSlotIndex = slotIndex;
        SlotInfo.lastUsedSlot = slotIndex;

        // 从文件加载到 GameDataMgr
        string suffix = $"_{slotIndex}";
        CurrentGameDataMgr.Instance.playerData = JsonMgr.Instance.LoadData<PlayerData>(PLAYER_DATA_FILE + suffix);
        CurrentGameDataMgr.Instance.playerInventoryData = JsonMgr.Instance.LoadData<PlayerInventoryData>(INVENTORY_DATA_FILE + suffix);
        CurrentGameDataMgr.Instance.taskData = JsonMgr.Instance.LoadData<TaskData>(TASK_DATA_FILE + suffix);
        CurrentGameDataMgr.Instance.sceneStateData = JsonMgr.Instance.LoadData<SceneStateData>(SCENE_STATE_FILE + suffix);

        // 恢复游戏时长
        CurrentPlayTime = SlotInfo.GetSlot(slotIndex)?.playTimeSeconds ?? 0f;

        // 同步到 QFramework Model 层
        SyncToModels();

        JsonMgr.Instance.SaveData(SlotInfo, SLOT_INFO_FILE);
        return true;
    }

    /// <summary>
    /// 保存当前游戏：将内存数据写入当前槽位
    /// </summary>
    public void SaveCurrentGame()
    {
        if (CurrentSlotIndex < 0)
        {
            Debug.LogError("SaveMgr: 未选择存档槽位");
            return;
        }

        // 仅对已迁移到 Model 层写入的模块执行导出
        // PlayerModel / InventoryModel 暂未迁移写入，不导出
        // TaskModel 由 TaskSaveHelper 单独处理
        var sceneStateModel = GameArchitecture.Interface.GetModel<ISceneStateModel>();
        sceneStateModel.ExportToSceneStateData(CurrentGameDataMgr.Instance.sceneStateData);

        // 先收集任务数据到 CurrentGameDataMgr
        TaskSaveHelper.SaveAllTaskData();

        // 更新槽位元数据（用于UI显示）
        SaveSlotData slot = SlotInfo.GetSlot(CurrentSlotIndex);
        PlayerData pd = CurrentGameDataMgr.Instance.playerData;

        slot.isEmpty = false;
        slot.playerName = pd.playerName;
        slot.playerLevel = pd.playerLevel;
        slot.currentSouls = pd.currentSoulCount;
        slot.maxHealth = pd.healthLevel * 10;
        slot.maxStamina = pd.staminaLevel * 10;
        slot.maxFocus = pd.focusLevel * 10;
        slot.playTimeSeconds = CurrentPlayTime;
        slot.UpdateSaveTime();
        // 保存最后使用的槽位索引
        SlotInfo.lastUsedSlot = CurrentSlotIndex;

        // 保存到文件
        string suffix = $"_{CurrentSlotIndex}";
        JsonMgr.Instance.SaveData(CurrentGameDataMgr.Instance.playerData, PLAYER_DATA_FILE + suffix);
        JsonMgr.Instance.SaveData(CurrentGameDataMgr.Instance.playerInventoryData, INVENTORY_DATA_FILE + suffix);
        JsonMgr.Instance.SaveData(CurrentGameDataMgr.Instance.taskData, TASK_DATA_FILE + suffix);
        JsonMgr.Instance.SaveData(CurrentGameDataMgr.Instance.sceneStateData, SCENE_STATE_FILE + suffix);
        JsonMgr.Instance.SaveData(SlotInfo, SLOT_INFO_FILE);
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

        JsonMgr.Instance.SaveData(SlotInfo, SLOT_INFO_FILE);
    }

    public void StartPlayTimer()
    {
        IsPlaying = true;
        MonoMgr.Instance.AddUpdateListener(UpdatePlayTime);
    }

    public void StopPlayTimer()
    {
        IsPlaying = false;
        MonoMgr.Instance.RemoveUpdateListener(UpdatePlayTime);
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

    #region QFramework Model 同步

    /// <summary>
    /// 将 CurrentGameDataMgr 数据同步到 QFramework Model 层
    /// </summary>
    private void SyncToModels()
    {
        var playerModel = GameArchitecture.Interface.GetModel<IPlayerModel>();
        playerModel.ImportFromPlayerData(CurrentGameDataMgr.Instance.playerData);

        var inventoryModel = GameArchitecture.Interface.GetModel<IInventoryModel>();
        inventoryModel.ImportFromInventoryData(CurrentGameDataMgr.Instance.playerInventoryData);

        var taskModel = GameArchitecture.Interface.GetModel<ITaskModel>();
        taskModel.ImportFromTaskData(CurrentGameDataMgr.Instance.taskData);

        var sceneStateModel = GameArchitecture.Interface.GetModel<ISceneStateModel>();
        sceneStateModel.ImportFromSceneStateData(CurrentGameDataMgr.Instance.sceneStateData);
    }

    /// <summary>
    /// 从 QFramework Model 层导出数据到 CurrentGameDataMgr
    /// </summary>
    private void SyncFromModels()
    {
        var playerModel = GameArchitecture.Interface.GetModel<IPlayerModel>();
        playerModel.ExportToPlayerData(CurrentGameDataMgr.Instance.playerData);

        var inventoryModel = GameArchitecture.Interface.GetModel<IInventoryModel>();
        inventoryModel.ExportToInventoryData(CurrentGameDataMgr.Instance.playerInventoryData);

        var taskModel = GameArchitecture.Interface.GetModel<ITaskModel>();
        taskModel.ExportToTaskData(CurrentGameDataMgr.Instance.taskData);

        var sceneStateModel = GameArchitecture.Interface.GetModel<ISceneStateModel>();
        sceneStateModel.ExportToSceneStateData(CurrentGameDataMgr.Instance.sceneStateData);
    }

    #endregion
}
