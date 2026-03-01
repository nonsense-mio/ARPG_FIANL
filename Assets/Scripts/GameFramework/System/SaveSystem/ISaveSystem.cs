using Framework;

namespace ARPG
{
    /// <summary>
    /// 存档系统接口 - 管理多槽位存档的读写和游戏时长计时
    /// 替代原 SaveMgr (BaseManager 单例)
    /// </summary>
    public interface ISaveSystem : ISystem
    {
        int CurrentSlotIndex { get; }
        SaveSlotInfo SlotInfo { get; }
        float CurrentPlayTime { get; }
        bool IsPlaying { get; }

        bool HasAnySave();
        SaveSlotData GetSlotData(int index);

        void CreateNewGame(int slotIndex, string playerName = "Player");
        bool LoadGame(int slotIndex);
        void SaveCurrentGame();
        void DeleteSave(int slotIndex);

        void SaveTaskData();
        void SaveInventoryData();

        void StartPlayTimer();
        void StopPlayTimer();
    }
}
