using Framework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// 从 PlayerModel 读取复活点坐标
    /// </summary>
    public class GetRespawnPositionQuery : AbstractQuery<Vector3>
    {
        public GetRespawnPositionQuery() { }

        protected override Vector3 OnDo()
        {
            var model = this.GetModel<IPlayerModel>();
            return new Vector3(
                model.RespawnX.Value,
                model.RespawnY.Value,
                model.RespawnZ.Value);
        }
    }
}
