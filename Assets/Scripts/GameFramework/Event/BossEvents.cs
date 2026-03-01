using ARPG;

namespace ARPG
{
    public struct BossHudChangedEvent
    {
        public BossHudData? Data;
    }

    public struct BossPhaseShiftEvent
    {
        public EnemyManager Boss;
    }

    /// <summary>
    /// AOT → HotUpdate 桥接事件：请求激活 Boss 战。
    /// BossFightManager (AOT) 发送，HotUpdate 中的处理器执行具体逻辑。
    /// </summary>
    public struct ActivateBossFightRequestEvent
    {
        public BossFightManager BossFight;
    }

    /// <summary>
    /// AOT → HotUpdate 桥接事件：请求处理 Boss 击败。
    /// BossFightManager (AOT) 发送，HotUpdate 中的处理器执行具体逻辑。
    /// </summary>
    public struct BossDefeatedRequestEvent
    {
        public BossFightManager BossFight;
    }
}
