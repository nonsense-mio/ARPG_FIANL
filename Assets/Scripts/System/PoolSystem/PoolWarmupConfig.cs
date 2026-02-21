using UnityEngine;

namespace ARPG
{
    [CreateAssetMenu(fileName = "PoolWarmupConfig", menuName = "ARPG/Pool Warmup Config")]
    public class PoolWarmupConfig : ScriptableObject
    {
        [System.Serializable]
        public struct PoolEntry
        {
            public string prefabPath;
            public int warmupCount;
            public int maxCapacity;
        }

        public PoolEntry[] entries;
    }
}
