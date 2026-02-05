using System.Collections.Generic;
using Framework;

namespace ARPG
{
    /// <summary>
    /// 场景状态模型实现 - 管理场景元素的持久化状态
    /// </summary>
    public class SceneStateModel : AbstractModel, ISceneStateModel
    {
        #region 内部数据
        private Dictionary<string, bool> chestStates = new Dictionary<string, bool>();
        private Dictionary<string, bool> bonfireStates = new Dictionary<string, bool>();
        private Dictionary<string, bool> bossDefeatedStates = new Dictionary<string, bool>();
        #endregion

        protected override void OnInit()
        {
            // Model 初始化时不需要特别操作
            // 数据通过 ImportFromSceneStateData 加载
        }

        #region 宝箱状态
        public void SetChestOpened(string chestId, bool opened)
        {
            chestStates[chestId] = opened;
        }

        public bool IsChestOpened(string chestId)
        {
            return chestStates.TryGetValue(chestId, out bool opened) && opened;
        }
        #endregion

        #region 篝火状态
        public void SetBonfireActivated(string bonfireId, bool activated)
        {
            bonfireStates[bonfireId] = activated;
        }

        public bool IsBonfireActivated(string bonfireId)
        {
            return bonfireStates.TryGetValue(bonfireId, out bool activated) && activated;
        }
        #endregion

        #region Boss状态
        public void SetBossDefeated(string bossId, bool defeated)
        {
            bossDefeatedStates[bossId] = defeated;
        }

        public bool IsBossDefeated(string bossId)
        {
            return bossDefeatedStates.TryGetValue(bossId, out bool defeated) && defeated;
        }
        #endregion

        #region 数据导入导出
        public void ImportFromSceneStateData(SceneStateData data)
        {
            // 清空现有数据
            chestStates.Clear();
            bonfireStates.Clear();
            bossDefeatedStates.Clear();

            if (data == null) return;

            // 导入宝箱状态
            if (data.chestStates != null)
            {
                foreach (var kvp in data.chestStates)
                    chestStates[kvp.Key] = kvp.Value;
            }

            // 导入篝火状态
            if (data.bonfireStates != null)
            {
                foreach (var kvp in data.bonfireStates)
                    bonfireStates[kvp.Key] = kvp.Value;
            }

            // 导入 Boss 状态
            if (data.bossDefeatedStates != null)
            {
                foreach (var kvp in data.bossDefeatedStates)
                    bossDefeatedStates[kvp.Key] = kvp.Value;
            }
        }

        public void ExportToSceneStateData(SceneStateData data)
        {
            if (data == null) return;

            // 导出宝箱状态
            data.chestStates.Clear();
            foreach (var kvp in chestStates)
                data.chestStates[kvp.Key] = kvp.Value;

            // 导出篝火状态
            data.bonfireStates.Clear();
            foreach (var kvp in bonfireStates)
                data.bonfireStates[kvp.Key] = kvp.Value;

            // 导出 Boss 状态
            data.bossDefeatedStates.Clear();
            foreach (var kvp in bossDefeatedStates)
                data.bossDefeatedStates[kvp.Key] = kvp.Value;
        }
        #endregion
    }
}
