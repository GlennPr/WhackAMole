Shader "UI/Custom/Wormhole"
{
    Properties
    {
        _BGColor("Background Color", Color) = (0,0,0,1)
        _SwirlCount("Swirl Count", Float) = 50.0
        _Thickness("Swirl Thickness", Float) = 0
        _Edge("Swirl Edge Width", Float) = 0.02
        _Speed("Swirl Speed", Float) = 1.0

        _MinNoiseFiltering("_MinNoiseFiltering", Float) = 0.99
        _MaxNoiseFiltering("_MaxNoiseFiltering", Float) = 1.0
        [Header(Default)]
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]

            Pass
            {
                Name "Default"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"
                #include "Includes/IncludeGeneral.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                fixed4 _Color;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;

                fixed4 _BGColor;

                float _SwirlCount;
                float _Thickness;
                float _Edge;
                float _Speed;


                float _MinNoiseFiltering;
                float _MaxNoiseFiltering;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                    OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                    OUT.color = v.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    float2 polarCoordinate = PolarCoordinate_LOG(IN.texcoord);
                    polarCoordinate.r = (polarCoordinate.x + 0.5);

                    half4 color = (tex2D(_MainTex, polarCoordinate) + _TextureSampleAdd) * IN.color;

                    #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                    #endif

                    // log length
                    float fromCenterLength = polarCoordinate.g * _SwirlCount;

                    // prevent artifacts in the center
                    _Edge *= 1 + fwidth(fromCenterLength) * 3.0;
                    _Thickness *= 1 + fwidth(_Thickness) * 3.0;

                    float lowerBand = -_Thickness * 0.5;
                    float higherBand = _Thickness * 0.5;
    
                    float spiralLine = sin(fromCenterLength + polarCoordinate.x * Custom_TWO_PI - _Time.y * _Speed);
                    spiralLine = smoothstep(lowerBand - _Edge, lowerBand, spiralLine) * smoothstep(higherBand + _Edge, higherBand, spiralLine);

                    color.rgb = lerp(_BGColor, _Color, spiralLine);

                    // clean length
                    fromCenterLength = length(IN.texcoord - 0.5) * 2;

                    // fade out center
                    color.rgb = lerp(color.rgb, _BGColor, smoothstep(_MinNoiseFiltering, _MaxNoiseFiltering, fromCenterLength));

                    color.a = 1.0 - smoothstep(0.94, 1.0, fromCenterLength);
                    color.a *= IN.color.a;

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.001);
                    #endif

                    return color;
                }
            ENDCG
            }
        }
}