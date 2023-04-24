Shader "Unlit/Glass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uvLM : TEXCOORD1;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : VAR_NORMAL;
                float3 pos : VAR_POSITION;
                float2 uvLM : VAR_UVLIGHTMAPS;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.pos = TransformObjectToWorld(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.uvLM = v.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                BRDFData brdf;
                float alpha = 1.0;
                InitializeBRDFData(0.0, 0.0, 0.0, 1.0, alpha, brdf);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.pos);
                half3 bakedGI = SampleSH(normal);
                float3 env = GlobalIllumination(brdf, bakedGI, 1.0, normal, viewDir);

                float4 col = float4(env, min(max(env.r, max(env.g, env.b)), 1.0));
                return col;
            }
            ENDHLSL
        }
    }
}
