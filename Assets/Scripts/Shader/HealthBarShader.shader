Shader "Custom/HealthBarShader"
{
    Properties
    {
        _FillAmount       ("Fill Amount",       Range(0,1)) = 1.0
        _YellowAmount     ("Yellow Amount",     Range(0,1)) = 1.0
        _FillColor        ("Fill Color",        Color) = (0.8,0.1,0.1,1)
        _YellowColor      ("Yellow Color",      Color) = (1,0.8,0,1)
        _BgColor          ("Background Color",  Color) = (0.15,0.15,0.15,1)
        // 0=全向广告牌(面向相机) 1=垂直轴广告牌(只绕Y轴旋转)
        _VerticalBillboard("Vertical Billboard",Range(0,1)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "DisableBatching"="True"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float  _VerticalBillboard;
            float  _FillAmount;
            float  _YellowAmount;
            fixed4 _FillColor;
            fixed4 _YellowColor;
            fixed4 _BgColor;

            v2f vert(appdata_base v)
            {
                v2f o;
                // 广告牌顶点变换
                float3 center = float3(0, 0, 0);

                // 将相机位置从世界空间变换到模型空间
                float3 cameraInObjectPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1)).xyz;

                // 计算 Z 轴方向（模型朝向相机的方向）
                float3 normalDir = cameraInObjectPos - center;
                // _VerticalBillboard=0：normalDir.y被压为0，投影到XZ平面，即全向广告牌
                // _VerticalBillboard=1：normalDir.y保留，即垂直轴广告牌
                normalDir.y *= _VerticalBillboard;
                normalDir = normalize(normalDir);

                // 防止 normalDir 与世界Y轴共线时叉积为零向量（相机正上方/正下方时）
                float3 upDir    = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
                float3 rightDir = normalize(cross(upDir, normalDir));
                upDir           = normalize(cross(normalDir, rightDir));

                // 将顶点投影到新的广告牌坐标系
                float3 offset  = v.vertex.xyz - center;
                float3 newPos  = center + rightDir * offset.x + upDir * offset.y + normalDir * offset.z;
                o.vertex = UnityObjectToClipPos(float4(newPos, 1));

                // Quad 的 UV.x 范围 0(左)~1(右)，用于表示血量区间
                o.uv = v.texcoord.xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // UV.x < 当前血量比例 → 红色（当前血量段）
                // UV.x < 黄条高水位   → 黄色（伤害溅射段）
                // 其余                → 深色背景
                if (i.uv.x < _FillAmount)
                    return _FillColor;
                else if (i.uv.x < _YellowAmount)
                    return _YellowColor;
                else
                    return _BgColor;
            }
            ENDCG
        }
    }
}
