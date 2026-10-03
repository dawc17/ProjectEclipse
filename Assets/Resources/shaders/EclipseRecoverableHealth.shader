Shader "Eclipse/UI/Recoverable Health"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Segment ("Recoverable segment", Vector) = (0,0,0,0)
        _SpriteUV ("Sprite atlas bounds", Vector) = (0,0,1,1)
        _CenterOpacity ("Center opacity", Range(0,1)) = 0.32
        _OutlineWidth ("Outline width / bar height", Range(0.01,0.5)) = 0.12
        _EdgeFade ("End fade / full bar width", Range(0,0.1)) = 0.012
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct Input { float4 vertex:POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Output { float4 vertex:SV_POSITION; fixed4 color:COLOR; float2 uv:TEXCOORD0; float4 local:TEXCOORD1; float2 barUV:TEXCOORD2; UNITY_VERTEX_OUTPUT_STEREO };
            sampler2D _MainTex;
            fixed4 _Color, _TextureSampleAdd;
            float4 _ClipRect;
            float4 _Segment, _SpriteUV;
            float _CenterOpacity, _OutlineWidth, _EdgeFade;
            Output vert(Input input)
            {
                Output output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.local=input.vertex;
                output.vertex=UnityObjectToClipPos(input.vertex);
                output.uv=input.uv;
                output.color=input.color*_Color;
                // UGUI batches positions into canvas space. Sprite UVs retain
                // bar-local progress at every HUD position and through skew.
                output.barUV=(input.uv-_SpriteUV.xy)/max(float2(.000001,.000001),_SpriteUV.zw-_SpriteUV.xy);
                output.barUV.x=lerp(output.barUV.x,1-output.barUV.x,_Segment.z);
                return output;
            }
            fixed4 frag(Output input):SV_Target
            {
                clip(input.barUV.x-_Segment.x);
                clip(_Segment.y-input.barUV.x);
                fixed4 sample=tex2D(_MainTex,input.uv)+_TextureSampleAdd;
                // Remove the recovered orange hue without dimming its strongest channel.
                fixed brightness=max(sample.r,max(sample.g,sample.b));
                fixed4 result=fixed4(brightness,brightness,brightness,sample.a)*input.color;
                // Keep the native top/bottom silhouette clear while letting the
                // backdrop show through the middle. UV space also follows skew.
                float rimDistance=min(input.barUV.y,1-input.barUV.y);
                float rim=1-smoothstep(_OutlineWidth*.35,_OutlineWidth,rimDistance);
                result.a*=lerp(_CenterOpacity,1,rim);
                // Fade both ends, limiting the fade for small recovery amounts
                // so a narrow segment still retains a visible center/outline.
                float fadeWidth=max(.000001,min(_EdgeFade,(_Segment.y-_Segment.x)*.2));
                float endDistance=min(input.barUV.x-_Segment.x,_Segment.y-input.barUV.x);
                result.a*=smoothstep(0,fadeWidth,endDistance);
                #ifdef UNITY_UI_CLIP_RECT
                result.a*=UnityGet2DClipping(input.local.xy,_ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(result.a-.001);
                #endif
                return result;
            }
            ENDCG
        }
    }
}
