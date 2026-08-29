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
            float _HistoryWeight;

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 currentFrame = tex2D(_MainTex, i.uv);
                fixed4 historyFrame = tex2D(_HistoryTex, i.uv);
                return lerp(currentFrame, historyFrame, saturate(_HistoryWeight));
            }
            ENDCG
        }
    }

    Fallback Off
}
