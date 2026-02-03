using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    //玩家状态类型枚举
    public enum E_PlayerStatType
    {
        Health,
        Stamina,
        Focus,
        PoisonBuild,
        PoisonAmount,
    }

    public readonly struct StatChanged
    {
        public readonly E_PlayerStatType stat;
        public readonly int cur;
        public readonly int max;

        public StatChanged(E_PlayerStatType stat, int cur, int max)
        {
            this.stat = stat;
            this.cur = cur;
            this.max = max;
        }

        public override string ToString() => $"{stat}: {cur}/{max}";
    }
}
