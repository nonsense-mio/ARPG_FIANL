using System.Collections.Generic;
using Framework;

namespace ARPG
{
    /// <summary>
    /// 任务数据模型接口 - QFramework Model层
    /// 职责: 管理任务持久化数据，提供响应式数据绑定
    /// </summary>
    public interface ITaskModel : IModel
    {
        #region 任务数据（响应式）
        /// <summary>
        /// 活跃任务存档数据列表
        /// </summary>
        BindableList<TaskSaveData> ActiveTasks { get; }

        /// <summary>
        /// NPC 任务链进度 (key: GiverId, value: 当前任务索引)
        /// </summary>
        Dictionary<string, int> TaskGiverProgress { get; }
        #endregion

        #region 任务操作
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="requireCount">需求数量</param>
        void AddTask(string taskName, int requireCount);

        /// <summary>
        /// 更新任务需求进度
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="requireIndex">需求索引</param>
        /// <param name="amount">增加的数量</param>
        void UpdateTaskProgress(string taskName, int requireIndex, int amount);

        /// <summary>
        /// 标记任务完成（需求达成）
        /// </summary>
        void CompleteTask(string taskName);

        /// <summary>
        /// 标记任务已上交
        /// </summary>
        void TurnInTask(string taskName);

        /// <summary>
        /// 获取任务存档数据
        /// </summary>
        TaskSaveData GetTask(string taskName);

        /// <summary>
        /// 检查是否拥有该任务
        /// </summary>
        bool HasTask(string taskName);
        #endregion

        #region NPC 任务链进度
        /// <summary>
        /// 设置 NPC 任务链进度
        /// </summary>
        /// <param name="giverId">NPC 唯一标识</param>
        /// <param name="index">当前任务索引</param>
        void SetGiverProgress(string giverId, int index);

        /// <summary>
        /// 获取 NPC 任务链进度
        /// </summary>
        /// <param name="giverId">NPC 唯一标识</param>
        /// <returns>当前任务索引，不存在返回 0</returns>
        int GetGiverProgress(string giverId);
        #endregion

        #region 数据导入导出 (与存档系统集成)
        /// <summary>
        /// 从 TaskData 加载数据 (存档加载时调用)
        /// </summary>
        void LoadData(TaskData data);

        /// <summary>
        /// 保存数据到 TaskData (存档保存时调用)
        /// </summary>
        void SaveData(TaskData data);
        #endregion
    }
}
