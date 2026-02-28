using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 计时器系统实现 - Update 驱动，帧精度计时
    /// 替代原 TimerMgr (BaseManager, 100ms 协程轮询)
    /// </summary>
    public class TimerSystem : AbstractSystem, ITimerSystem
    {
        private int nextId = 0;
        private readonly Dictionary<int, TimerData> scaledTimers = new Dictionary<int, TimerData>();
        private readonly Dictionary<int, TimerData> realTimers = new Dictionary<int, TimerData>();
        private readonly List<TimerData> pendingRemove = new List<TimerData>();
        private readonly List<TimerData> iteratingSnapshot = new List<TimerData>();
        private IPoolSystem poolSystem;

        protected override void OnInit()
        {
            poolSystem = this.GetSystem<IPoolSystem>();
            this.GetSystem<ITickSystem>().RegisterUpdate(OnUpdate);
        }

        private void OnUpdate()
        {
            UpdateTimers(scaledTimers, Time.deltaTime);
            UpdateTimers(realTimers, Time.unscaledDeltaTime);
        }

        private void UpdateTimers(Dictionary<int, TimerData> dict, float dt)
        {
            // 快照当前值，避免回调中 CreateTimer/RemoveTimer 修改 Dictionary 导致迭代异常
            iteratingSnapshot.AddRange(dict.Values);

            for (int i = 0; i < iteratingSnapshot.Count; i++)
            {
                var timer = iteratingSnapshot[i];
                if (!timer.IsRunning) continue;

                // 间隔回调
                if (timer.Interval > 0f && timer.OnInterval != null)
                {
                    timer.IntervalTimer += dt;
                    if (timer.IntervalTimer >= timer.Interval)
                    {
                        timer.IntervalTimer -= timer.Interval;
                        timer.OnInterval.Invoke();
                    }
                }

                // 总时长递减
                timer.Duration -= dt;
                if (timer.Duration <= 0f)
                {
                    timer.OnComplete?.Invoke();
                    pendingRemove.Add(timer);
                }
            }
            iteratingSnapshot.Clear();

            // 移除已完成的计时器
            for (int i = 0; i < pendingRemove.Count; i++)
            {
                var t = pendingRemove[i];
                // 回调中可能已调用 RemoveTimer 将其移除，需检查
                if (dict.Remove(t.Id))
                    poolSystem.Recycle(t);
            }
            pendingRemove.Clear();
        }

        public int CreateTimer(bool isRealTime, float duration, Action onComplete,
                               float interval = 0f, Action onInterval = null)
        {
            int id = ++nextId;
            var data = poolSystem.Spawn<TimerData>();
            data.Init(id, duration, onComplete, interval, onInterval, isRealTime);

            if (isRealTime)
                realTimers.Add(id, data);
            else
                scaledTimers.Add(id, data);

            return id;
        }

        public void RemoveTimer(int id)
        {
            if (scaledTimers.TryGetValue(id, out var t))
            {
                poolSystem.Recycle(t);
                scaledTimers.Remove(id);
            }
            else if (realTimers.TryGetValue(id, out t))
            {
                poolSystem.Recycle(t);
                realTimers.Remove(id);
            }
        }

        public void PauseTimer(int id)
        {
            if (scaledTimers.TryGetValue(id, out var t)) t.IsRunning = false;
            else if (realTimers.TryGetValue(id, out t)) t.IsRunning = false;
        }

        public void ResumeTimer(int id)
        {
            if (scaledTimers.TryGetValue(id, out var t)) t.IsRunning = true;
            else if (realTimers.TryGetValue(id, out t)) t.IsRunning = true;
        }

        public void ResetTimer(int id)
        {
            if (scaledTimers.TryGetValue(id, out var t)) t.ResetTimer();
            else if (realTimers.TryGetValue(id, out t)) t.ResetTimer();
        }

        public void ClearAllTimers()
        {
            foreach (var t in scaledTimers.Values)
                poolSystem.Recycle(t);
            scaledTimers.Clear();

            foreach (var t in realTimers.Values)
                poolSystem.Recycle(t);
            realTimers.Clear();

            pendingRemove.Clear();
            iteratingSnapshot.Clear();
        }
    }
}
