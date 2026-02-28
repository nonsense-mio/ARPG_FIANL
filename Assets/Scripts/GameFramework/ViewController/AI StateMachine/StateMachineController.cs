namespace ARPG
{
    /// <summary>
    /// 通用状态机控制器，管理 State 的 OnEnter/OnUpdate/OnExit 生命周期。
    /// 纯 C# 类，由 EnemyManager 持有并在 Update 中驱动。
    /// </summary>
    public class StateMachineController
    {
        public State CurrentState { get; private set; }

        /// <summary>
        /// 初始化状态机，设置初始状态并调用 OnEnter
        /// </summary>
        public void Initialize(State initialState, EnemyManager enemy)
        {
            CurrentState = initialState;
            CurrentState?.OnEnter(enemy);
        }

        /// <summary>
        /// 每帧调用，驱动当前状态的 OnUpdate 并处理转换
        /// </summary>
        public void Update(EnemyManager enemy)
        {
            if (CurrentState == null) return;

            State nextState = CurrentState.OnUpdate(enemy);

            if (nextState != null && nextState != CurrentState)
            {
                CurrentState.OnExit(enemy);
                CurrentState = nextState;
                CurrentState.OnEnter(enemy);
            }
        }

        /// <summary>
        /// 强制转换到指定状态（Boss 阶段切换等），会正确调用 OnExit/OnEnter
        /// </summary>
        public void ForceTransitionTo(State newState, EnemyManager enemy)
        {
            CurrentState?.OnExit(enemy);
            CurrentState = newState;
            CurrentState?.OnEnter(enemy);
        }

        /// <summary>
        /// 停止状态机（死亡时调用），调用当前状态的 OnExit 后置空
        /// </summary>
        public void Stop(EnemyManager enemy)
        {
            CurrentState?.OnExit(enemy);
            CurrentState = null;
        }
    }
}
