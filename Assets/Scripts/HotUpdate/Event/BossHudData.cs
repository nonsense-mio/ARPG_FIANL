using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// Boss HUD 需要的最小快照数据。UI 只依赖它，不依赖 EnemyManager。
    /// </summary>
    public readonly struct BossHudData
    {
        public readonly string name;
        public readonly int curHp;
        public readonly int maxHp;

        public BossHudData(string name, int curHp, int maxHp)
        {
            this.name = name;
            this.curHp = curHp;
            this.maxHp = maxHp;
        }
    }
}
