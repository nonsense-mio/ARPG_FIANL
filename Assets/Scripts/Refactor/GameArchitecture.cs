using QFramework;
using UnityEngine;

namespace ARPG
{
    public class GameArchitecture : Architecture<GameArchitecture>
    {
        protected override void Init()
        {
            // 注册 存储工具
            RegisterUtility<IStorage>(new JsonStorage());
        }
    }
}