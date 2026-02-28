using System;
using Framework;

namespace ARPG
{
    /// <summary>
    /// 计时器系统接口 - QFramework System层
    /// 职责: 管理多个并发计时器，支持暂停/恢复/重置，支持 TimeScale 和 RealTime 双轨
    /// </summary>
    public interface ITimerSystem : ISystem
    {
        /// <summary>
        /// 创建计时器，返回唯一 ID
        /// </summary>
        /// <param name="isRealTime">true = 不受 Time.timeScale 影响</param>
        /// <param name="duration">总时长（秒）</param>
        /// <param name="onComplete">计时结束回调</param>
        /// <param name="interval">间隔回调时间（秒），0 = 不触发间隔回调</param>
        /// <param name="onInterval">间隔回调</param>
        /// <returns>计时器唯一 ID，用于后续控制</returns>
        int CreateTimer(bool isRealTime, float duration, Action onComplete,
                        float interval = 0f, Action onInterval = null);

        /// <summary>
        /// 移除计时器（立即销毁，不触发回调）
        /// </summary>
        void RemoveTimer(int id);

        /// <summary>
        /// 暂停计时器
        /// </summary>
        void PauseTimer(int id);

        /// <summary>
        /// 恢复已暂停的计时器
        /// </summary>
        void ResumeTimer(int id);

        /// <summary>
        /// 重置计时器（恢复到初始时长，自动恢复运行）
        /// </summary>
        void ResetTimer(int id);

        /// <summary>
        /// 清空所有计时器（不触发任何回调），用于场景切换时防止过期回调访问已销毁对象
        /// </summary>
        void ClearAllTimers();
    }

    /// <summary>
    /// 计时器数据对象，通过 IPoolSystem 泛型池复用
    /// </summary>
    public class TimerData : IPoolObject
    {
        public int Id;
        public float Duration;       // 剩余时长（秒）
        public float MaxDuration;    // 初始时长（用于 Reset）
        public float Interval;       // 间隔回调时间（秒）
        public float MaxInterval;    // 初始间隔（用于 Reset）
        public float IntervalTimer;  // 当前间隔累计
        public Action OnComplete;
        public Action OnInterval;
        public bool IsRunning;
        public bool IsRealTime;

        public void Init(int id, float duration, Action onComplete,
                         float interval, Action onInterval, bool isRealTime)
        {
            Id = id;
            MaxDuration = Duration = duration;
            OnComplete = onComplete;
            MaxInterval = Interval = interval;
            IntervalTimer = 0f;
            OnInterval = onInterval;
            IsRunning = true;
            IsRealTime = isRealTime;
        }

        public void ResetTimer()
        {
            Duration = MaxDuration;
            IntervalTimer = 0f;
            IsRunning = true;
        }

        public void ResetInfo()
        {
            OnComplete = null;
            OnInterval = null;
        }
    }
}
