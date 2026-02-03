using UnityEngine;

/// <summary>
/// 游戏数据容器：保存当前游戏的内存数据
/// 存档读写由 SaveMgr 负责
/// </summary>
public class CurrentGameDataMgr : BaseManager<CurrentGameDataMgr>
{
    public PlayerData playerData;
    public PlayerInventoryData playerInventoryData;
    public TaskData taskData;
    public MusicData musicData;

    private CurrentGameDataMgr()
    {
        playerData = new PlayerData();
        playerInventoryData = new PlayerInventoryData();
        taskData = new TaskData();
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");
    }
    
    /// <summary>
    /// 保存音乐设置（全局设置，不随存档变化）
    /// </summary>
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicData, "MusicData");
    }
}
