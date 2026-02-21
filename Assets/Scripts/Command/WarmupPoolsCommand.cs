using Framework;

namespace ARPG
{
    public class WarmupPoolsCommand : AbstractCommand
    {
        private readonly PoolWarmupConfig _config;

        public WarmupPoolsCommand(){}
        public WarmupPoolsCommand(PoolWarmupConfig config)
        {
            _config = config;
        }

        protected override void OnExecute()
        {
            var pool = this.GetSystem<IPoolSystem>();
            foreach (var entry in _config.entries)
            {
                pool.WarmupPool(entry.prefabPath, entry.warmupCount, entry.maxCapacity);
            }
        }
    }
}
