Shader "Hidden/SF2/FrameBlendMotionBlur"
{
    Properties
    {
        _MainTex ("Current Frame", 2D) = "white" {}
        _HistoryTex ("History", 2D) = "black" {}
        _HistoryWeight ("History Weight", Range(0, 1)) = 0
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _HistoryTex;
            float4 _MainTex_TexelSize;
            float4 _HistoryTex_TexelSize;
            float _HistoryWeight;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 historyUV = i.uv;

                // Render textures can use opposite vertical UV orientations on
                // platforms such as Direct3D. Match the history texture to the
                // orientation Unity supplied for the current image-effect source.
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y * _HistoryTex_TexelSize.y < 0.0)
                {
                    historyUV.y = 1.0 - historyUV.y;
                }
                #endif

                fixed4 currentFrame = tex2D(_MainTex, i.uv);
                fixed4 historyFrame = tex2D(_HistoryTex, historyUV);
                return lerp(currentFrame, historyFrame, saturate(_HistoryWeight));
            }
            ENDCG
        }
    }

    Fallback Off
}
