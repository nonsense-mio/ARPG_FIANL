using System.Collections.Generic;
using Framework;
using HT;

namespace ARPG
{
    /// <summary>
    /// 任务系统接口 - QFramework System层
    /// 职责: 管理运行时任务对象 (TaskData_SO 克隆)，处理任务事件和进度计算
    /// </summary>
    public interface ITaskSystem : ISystem
    {
        #region 查询
        /// <summary>
        /// 是否拥有该任务
        /// </summary>
        bool HaveTask(string taskName);

        /// <summary>
        /// 获取运行时任务数据 (TaskData_SO 克隆)
        /// </summary>
        TaskData_SO GetTaskData(string taskName);

        /// <summary>
        /// 获取所有运行时任务数据
        /// </summary>
        List<TaskData_SO> GetAllTaskData();
        #endregion

        #region 存档集成
        /// <summary>
        /// 从 ITaskModel 数据 + 原始 SO 重建运行时克隆 (存档加载时调用)
        /// </summary>
        /// <param name="allOriginalTasks">场景中收集的所有原始 TaskData_SO</param>
        void RebuildFromModel(List<TaskData_SO> allOriginalTasks);

        /// <summary>
        /// 将运行时状态同步到 ITaskModel (存档保存前调用)
        /// </summary>
        void SyncToModel();

        /// <summary>
        /// 清空所有任务 (切场景/新游戏时调用)
        /// </summary>
        void ClearTasks();
        #endregion
    }
}
