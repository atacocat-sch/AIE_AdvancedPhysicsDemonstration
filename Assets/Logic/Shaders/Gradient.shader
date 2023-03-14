Shader "Unlit/Gradient"
{
    Properties
    {
        _ColA("Color A", Color) = (1, 1, 1, 1)
        _ColB("Color B", Color) = (0, 0, 0, 1)
        _P1 ("Point 1", Vector) = (0, 0, 0, 0)
        _P2 ("Point 2", Vector) = (1, 0, 0, 0)
        [MaterialToggle] _Debug ("Draw Points", int) = 0
        
        [HideInInspector] _Color ("Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8.000000
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0.000000
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0.000000
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255.000000
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255.000000
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15.000000
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "CanUseSpriteAtlas"="true" "PreviewType"="Plane" }

        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            ZTest [unity_GUIZTestMode]
            ZWrite Off
            Cull Off
            Stencil {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
            }
            Blend One OneMinusSrcAlpha
            ColorMask [_ColorMask]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 col : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 col : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.col = v.col;
                return o;
            }

            float4 _ColA, _ColB;
            float2 _P1, _P2;
            int _Debug;

            float smootherstep(float x)
            {
                return x * x * x * (x * (x * 6.0f - 15.0f) + 10.0f);
            }

            fixed4 frag (const v2f i) : SV_Target
            {
                const float l = length(_P2 - _P1);
                const float2 n = (_P2 - _P1) / l;
                const float o = dot(_P1, n);
                const float d = dot(i.uv, n);
                float p = (d - o) / l;
                p = clamp(p, 0, 1);
                
                fixed4 col = lerp(_ColA, _ColB, smootherstep(p)) * i.col;

                if (_Debug)
                {
                    if (min(length(i.uv - _P1), length(i.uv - _P2)) < 0.01f)
                    {
                        col = float4(1 - col.rgb, 1);
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }
}
