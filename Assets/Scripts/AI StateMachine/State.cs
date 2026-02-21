using UnityEngine;

namespace ARPG
{
    public abstract class State : MonoBehaviour
    {
        /// <summary>
        /// 进入状态时调用一次
        /// </summary>
        public virtual void OnEnter(EnemyManager enemy) { }

        /// <summary>
        /// 每帧调用，返回 this 表示停留，返回其他 State 触发转换
        /// </summary>
        public abstract State OnUpdate(EnemyManager enemy);

        /// <summary>
        /// 离开状态时调用一次
        /// </summary>
        public virtual void OnExit(EnemyManager enemy) { }
    }
}
