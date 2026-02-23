Shader "Unlit/BillboardShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
        //用于控制垂直广告牌和全向广告牌的变化
        _VerticalBillboard("VerticalBillboard",Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "DisableBatching" = "True" }

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
                float2 uv: TEXCOORD0;
                float4 vertex: SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _VerticalBillboard;

            v2f vert (appdata_base v)
            {
                v2f o;
                //新坐标系的中心点
                float3 center = float3(0,0,0);
                //计算z轴
                //将摄像机位置 从世界空间转换到模型空间
                float3 cameraInObjectPos = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1));
                //得到z轴对应向量
                float3 normalDir = cameraInObjectPos - center; 
                //_VerticalBillboard为0代表把z轴压到了xz平面，垂直广告牌效果 _VerticalBillboard为1就是视角方向
                normalDir.y *= _VerticalBillboard;
                //单位化z轴
                normalDir = normalize(normalDir);

                //为了避免z轴和010重合 normalDir.y > 0.999
                //模型空间下的Y轴正方向 oldUp
                float3 upDir = abs(normalDir.y) > 0.999 ? float3(0,0,1): float3(0,1,0);
                //利用叉乘计算x轴 right
                float3 rightDir = normalize(cross(upDir,normalDir));
                //计算新的Y轴方向
                upDir = normalize(cross(normalDir,rightDir));

                //得到顶点相对于新坐标系中心点的偏移位置
                float3 centerOffset = v.vertex.xyz - center;
                //利用3个轴向进行最终的顶点位置的计算
                float3 newVertexPos = center + rightDir * centerOffset.x + upDir * centerOffset.y + normalDir * centerOffset.z;
                //把新顶点转换到裁剪空间
                o.vertex = UnityObjectToClipPos(float4(newVertexPos,1));

                //uv坐标偏移缩放
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex,i.uv);
                color *= _Color;
                return color;
            }
            ENDCG
        }
    }
}
