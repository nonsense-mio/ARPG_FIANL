Shader "Custom/StandardDissolve"
{
    Properties
    {
        // ================================================================
        // PBR 属性（与Unity Standard材质完全一致）
        // ================================================================
        _Color              ("Color",               Color)          = (1,1,1,1)
        _MainTex            ("Albedo",              2D)             = "white" {}

        _Glossiness         ("Smoothness",          Range(0,1))     = 0.5
        _Metallic           ("Metallic",            Range(0,1))     = 0.0
        _MetallicGlossMap   ("Metallic (R) Gloss (A)", 2D)         = "white" {}

        [Normal]
        _BumpMap            ("Normal Map",          2D)             = "bump"  {}
        _BumpScale          ("Normal Scale",        Float)          = 1.0

        [HDR]
        _EmissionColor      ("Emission Color",      Color)          = (0,0,0,0)
        _EmissionMap        ("Emission",            2D)             = "black" {}

        // ================================================================
        // 消融属性
        // ================================================================
        _NoiseTex           ("Dissolve Noise",      2D)             = "white" {}
        _NoiseWorldScale    ("Noise World Scale",   Float)          = 0.5
        _Dissolve           ("Dissolve",            Range(0,1))     = 0
        _EdgeRange          ("Edge Range",          Range(0,0.2))   = 0.05
        _GradientTex        ("Edge Gradient",       2D)             = "white" {}
        [HDR]
        _EdgeColor          ("Edge Color Tint",     Color)          = (1,1,1,1) //渐变纹理整体色调
    }

    SubShader
    {
        // TransparentCutout + AlphaTest：消融镂空写入深度缓冲，可接收/投射阴影
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        Cull Off // 双面渲染，消融时内面可见

        CGPROGRAM

        // Standard    = Unity PBS 光照模型（等同于标准材质）
        // fullforwardshadows = 支持所有类型阴影（平行光/点光/聚光）
        // addshadow   = 自动生成 ShadowCaster Pass，并继承 surf() 中的 clip()
        //               ← 这一个关键词替代了你手写的整个 ShadowCaster Pass
        #pragma surface surf Standard fullforwardshadows addshadow
        #pragma target 3.0

        // ----------------------------------------------------------------
        // PBR 变量
        // ----------------------------------------------------------------
        sampler2D   _MainTex;
        sampler2D   _MetallicGlossMap;
        sampler2D   _BumpMap;
        sampler2D   _EmissionMap;
        fixed4      _Color;
        half        _Glossiness;
        half        _Metallic;
        float       _BumpScale;
        fixed4      _EmissionColor;

        // ----------------------------------------------------------------
        // 消融变量
        // ----------------------------------------------------------------
        sampler2D   _NoiseTex;
        float       _NoiseWorldScale;
        sampler2D   _GradientTex;
        float       _Dissolve;
        float       _EdgeRange;
        fixed4      _EdgeColor;

        // ----------------------------------------------------------------
        // Input 结构体
        // uv_XXX 命名：Unity 自动应用该纹理的 Tiling/Offset（_XXX_ST）
        // 等同于顶点着色器里手动写的 texcoord * _ST.xy + _ST.zw
        // ----------------------------------------------------------------
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos; // Unity 自动填充世界坐标，用于采样噪声（绕过UV分布问题）
            // EmissionMap、MetallicGlossMap 与 MainTex 共享 UV，无需重复声明
        };

        // ----------------------------------------------------------------
        // surf 函数：只负责填充材质数据，光照计算由 Unity 自动完成
        // ----------------------------------------------------------------
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // ① 消融裁剪（必须最先执行，被裁片元后续计算全部跳过）
            // 用世界坐标XZ平面采样噪声，与模型UV质量无关
            fixed noise = tex2D(_NoiseTex, IN.worldPos.xy * _NoiseWorldScale).r;
            clip(_Dissolve >= 1.0 ? -1.0 : noise - _Dissolve);

            // ② 基础 PBR 数据填充
            fixed4 c        = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo        = c.rgb;
            o.Alpha         = c.a;

            fixed4 mg       = tex2D(_MetallicGlossMap, IN.uv_MainTex);
            o.Metallic      = mg.r * _Metallic;     // R通道 = 金属度
            o.Smoothness    = mg.a * _Glossiness;   // A通道 = 光滑度

            o.Normal        = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);

            // ③ 消融边缘渐变
            //   noise - _Dissolve 越小 → 离消融边界越近 → edgeVal 越大（接近1）
            fixed  edgeVal   = 1.0 - smoothstep(0.0, _EdgeRange, noise - _Dissolve);
            //   _Dissolve == 0 时 edgeMask 归零，完全不显示边缘色
            fixed  edgeMask  = edgeVal * step(0.000001, _Dissolve);
            fixed3 edgeColor = tex2D(_GradientTex, float2(edgeVal, 0.0)).rgb * _EdgeColor.rgb;

            // ④ Emission = 标准自发光 + 消融边缘发光
            //   Emission 不受光照/阴影影响，天然适合"燃烧边缘"效果
            //   配合 Post Processing Bloom 可产生光晕溢出
            fixed3 stdEmission  = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor.rgb;
            o.Emission          = stdEmission + edgeColor * edgeMask;
        }

        ENDCG
    }

    // Standard Shader 的 FallBack 同样支持阴影
    FallBack "Standard"
}
