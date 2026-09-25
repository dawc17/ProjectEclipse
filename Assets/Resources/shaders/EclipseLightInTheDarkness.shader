Shader "Eclipse/Fight/Light In The Darkness"
{
    Properties
    {
        _Center ("Light center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Light radius", Range(0, 1)) = 0.2
        _Shape ("Round shape", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "CanUseSpriteAtlas" = "True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Center;
            float _Radius;
            float _Shape;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float2 offset = input.uv - _Center.xy;
                float distanceToLight = lerp(max(abs(offset.x), abs(offset.y)), length(offset), saturate(_Shape));
                float edge = max(fwidth(distanceToLight), 0.01);
                float darkness = smoothstep(_Radius - edge, _Radius + edge, distanceToLight);
                return fixed4(0, 0, 0, darkness);
            }
            ENDCG
        }
    }
}
