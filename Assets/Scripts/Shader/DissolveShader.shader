Shader "Custom/DissolveShader"
{
    Properties
    {
        _MainColor("NainColor",Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap("BumpMap",2D) = ""{}
        _BumpScale("BumpScale",Range(0,1)) = 1 //凹凸程度
        _SpecularColor("SpecularColor",Color) = (1,1,1,1)
        _SpecularNum("SpecularNum",Range(0,100)) = 20

        [HDR] _EmissionColor("EmissionColor",Color) = (0,0,0,0) //自发光颜色（支持HDR）
        _EmissionMap("EmissionMap",2D) = "black" {} //自发光纹理

        _Noise("Noise",2D) = ""{} //噪声纹理
        _Gradient("Gradient",2D) = ""{}//渐变纹理
        _Dissolve("Dissolve",Range(0,1)) = 0 //消融进度
        _EdgeRange("EdgeRange",Range(0,0.2)) = 0.1 //消融范围
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Cull Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"


            struct v2f
            {
                float4 pos:SV_POSITION;
                float4 uv:TEXCOORD0; //xy记录颜色纹理的uv zw记录法线纹理的uv
                float2 uvNoise:TEXCOORD1; //噪声纹理的uv 用于之后计算偏移缩放
                float3 lightDir:TEXCOORD2; //切线空间下的光照方向
                float3 viewDir:TEXCOORD3; //切线空间下的视角方向
                float3 worldPos:TEXCOORD4; //世界空间下的顶点位置
                SHADOW_COORDS(5)
            };

            float4 _MainColor; //漫反射颜色
            sampler2D _MainTex;//颜色纹理
            float4 _MainTex_ST;//颜色纹理的缩放和平移
            sampler2D _BumpMap;//法线纹理
            float4 _BumpMap_ST;//法线纹理的缩放和平移
            float _BumpScale;
            float4 _SpecularColor;
            float _SpecularNum;
            sampler2D _EmissionMap; //自发光纹理（与MainTex共享UV）
            float4 _EmissionColor;  //自发光颜色（支持HDR，分量可超过1）

            sampler2D _Noise;//噪声纹理
            float4 _Noise_ST;//噪声纹理的缩放和平移
            sampler2D _Gradient;//渐变纹理
            float _Dissolve;
            float _EdgeRange;

            v2f vert (appdata_full v)
            {
                v2f o;
                //把模型空间下的顶点转换到裁剪空间下
                o.pos = UnityObjectToClipPos(v.vertex);
                //计算纹理的缩放偏移
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
                o.uvNoise.xy = v.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
                //在顶点着色器中 得到 模型空间到切线空间的变换矩阵
                //切线 法线 副切线
                
                float3 tangent = normalize(v.tangent);
                float3 normal = normalize(v.normal);
                float3 binormal = normalize(cross(normal,tangent) * v.tangent.w);
                //变换矩阵
                float3x3 rotation = float3x3(tangent,
                binormal,
                normal);
                //模型空间下的光照方向
                o.lightDir = ObjSpaceLightDir(v.vertex);
                //切线空间下的光照方向
                o.lightDir = normalize(mul(rotation,o.lightDir));
                //模型空间下的视角方向
                o.viewDir = ObjSpaceViewDir(v.vertex);
                //切线空间下的视角方向
                o.viewDir = normalize(mul(rotation,o.viewDir));
                //世界空间下的顶点位置
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //剔除 消融
                fixed3 noiseColor = tex2D(_Noise,i.uvNoise.xy).rgb;
                clip(_Dissolve == 1 ? -1 : noiseColor.r - _Dissolve);

                //通过纹理采样函数 取出法线纹理贴图当中的数据
                float4 packedNormal = tex2D(_BumpMap,i.uv.zw);
                //将取出来的法线数据 进行逆运算以及可能的解压 最终得到切线空间下的法线数据
                float3 tangentNormal = UnpackNormal(packedNormal);
                //乘以凹凸程度系数 控制凹凸程度
                tangentNormal.xy *= _BumpScale;
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy,tangentNormal.xy)));
                //得到单张纹理颜色和漫反射颜色的叠加颜色
                fixed3 albedo = tex2D(_MainTex,i.uv.xy) * _MainColor.rgb;
                ///用切线空间下的 光方向、视角方向、法线方向 进行 Blinn Phong光照模型
                //兰伯特光照颜色
                fixed3 lambertColor = _LightColor0.rgb * albedo * max(0,dot(tangentNormal,i.lightDir));
                //高光反射颜色
                fixed3 halfA = normalize(i.viewDir + i.lightDir);
                fixed3 specularColor = _LightColor0 * _SpecularColor.rgb * pow(max(0,dot(tangentNormal,halfA)),_SpecularNum);

                UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);

                //自发光：采样EmissionMap后乘以HDR颜色系数，不受光照/阴影影响，直接叠加
                fixed3 emission = tex2D(_EmissionMap,i.uv.xy).rgb * _EmissionColor.rgb;
                fixed3 color = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo + lambertColor * atten + specularColor + emission;

                //渐变颜色的采样
                fixed value = 1 - smoothstep(0,_EdgeRange,noiseColor.r - _Dissolve);
                fixed3 gradientColor = tex2D(_Gradient,float2(value,0)).rgb;
                //最终颜色
                //当消融进度为0时 要完全显示原始颜色
                fixed3 finalColor = lerp(color,gradientColor,value * step(0.000001,_Dissolve));

                return fixed4(finalColor,1);
            }
            ENDCG
        }

        //该Pass 主要用来计算阴影映射纹理
        Pass
        {
            Tags{"LightMode" = "ShadowCaster"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag   
            #pragma multi_compile_shadowCaster

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uvNoise:TEXCOORD0; //噪声纹理的uv 用于之后计算偏移缩放
                //    顶点到片元着色器阴影投射结构体数据宏
                //    这个宏定义了一些标准的成员变量
                //    这些变量用于在阴影投射路径中传递顶点数据到片元着色器
                //    我们主要在结构体中使用
                V2F_SHADOW_CASTER;
            };
            sampler2D _Noise; //噪声纹理
            float4 _Noise_ST; //噪声纹理的缩放和平移
            float _Dissolve; //消融进度

            v2f vert(appdata_base v)
            {
                v2f o;
                o.uvNoise.xy = v.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
                //    转移阴影投射器法线偏移宏
                //    用于在顶点着色器中计算和传递阴影投射所需的变量
                //    主要做了
                //    2-2-1.将对象空间的顶点位置转换为裁剪空间的位置
                //    2-2-2.考虑法线偏移，以减轻阴影失真问题，尤其是在处理自阴影时
                //    2-2-3.传递顶点的投影空间位置，用于后续的阴影计算
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
                return o;
            }

            float4 frag(v2f i):SV_Target
            {
                //剔除——消融
                fixed3 noiseColor = tex2D(_Noise,i.uvNoise.xy).rgb;
                clip(_Dissolve == 1 ? -1 :noiseColor.r - _Dissolve);
                //    阴影投射片元宏
                //    将深度值写入到阴影映射纹理中
                //    我们主要在片元着色器中使用
                SHADOW_CASTER_FRAGMENT(i);
            }

            ENDCG
        }

    }
    FallBack "Diffuse"
}
