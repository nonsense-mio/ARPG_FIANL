using Framework;

namespace ARPG
{
    /// <summary>
    /// 根据攻击方向角度返回受击动画名称
    /// </summary>
    public class GetHitDirectionQuery : AbstractQuery<string>
    {
        private float angle;

        public GetHitDirectionQuery() { }

        public GetHitDirectionQuery(float angle)
        {
            this.angle = angle;
        }

        protected override string OnDo()
        {
            if ((angle >= 135 && angle <= 180) || (angle <= -135 && angle >= -180))
                return "Hit Forward";
            if (angle >= -45 && angle <= 45)
                return "Hit Backward";
            if (angle > 45 && angle < 135)
                return "Hit Right";
            return "Hit Left";
        }
    }
}
