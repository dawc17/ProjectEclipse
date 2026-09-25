Shader "Hidden/Eclipse/ScreenEffects"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}
        _BloomTex ("Bloom", 2D) = "black" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _BloomTex;
    float4 _MainTex_TexelSize;
    float _Threshold;
    float _Knee;
    float _BloomIntensity;
    float _Impact;

    half3 Prefilter(half3 c)
    {
        half brightness = max(c.r, max(c.g, c.b));
        half soft = clamp(brightness - _Threshold + _Knee, 0, 2 * _Knee);
        soft = soft * soft / (4 * _Knee + 1e-4);
        half contribution = max(soft, brightness - _Threshold) / max(brightness, 1e-4);
        return c * contribution;
    }

    half3 Box4(float2 uv, float offset)
    {
        float4 o = _MainTex_TexelSize.xyxy * float2(-offset, offset).xxyy;
        half3 s = tex2D(_MainTex, uv + o.xy).rgb + tex2D(_MainTex, uv + o.zy).rgb +
                  tex2D(_MainTex, uv + o.xw).rgb + tex2D(_MainTex, uv + o.zw).rgb;
        return s * 0.25;
    }
    ENDCG

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        // 0: threshold + first downsample
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            half4 frag(v2f_img i) : SV_Target
            {
                return half4(Prefilter(Box4(i.uv, 1)), 1);
            }
            ENDCG
        }

        // 1: downsample
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            half4 frag(v2f_img i) : SV_Target
            {
                return half4(Box4(i.uv, 1), 1);
            }
            ENDCG
        }

        // 2: upsample, added onto the larger level
        Pass
        {
            Blend One One
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            half4 frag(v2f_img i) : SV_Target
            {
                return half4(Box4(i.uv, 0.5), 1);
            }
            ENDCG
        }

        // 3: composite bloom and the impact radial blur / colour split
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            half4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float2 fromCenter = uv - 0.5;
                half4 color;
                if (_Impact > 0.001)
                {
                    // Radial blur toward the centre plus a slight RGB split.
                    half3 sum = 0;
                    const int taps = 6;
                    for (int t = 0; t < taps; t++)
                    {
                        float scale = 1.0 - _Impact * 0.035 * t;
                        float2 tapUV = 0.5 + fromCenter * scale;
                        float2 split = fromCenter * _Impact * 0.012;
                        sum.r += tex2D(_MainTex, tapUV + split).r;
                        sum.g += tex2D(_MainTex, tapUV).g;
                        sum.b += tex2D(_MainTex, tapUV - split).b;
                    }
                    color = half4(sum / taps, 1);
                }
                else
                {
                    color = tex2D(_MainTex, uv);
                }

                float2 bloomUV = uv;
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0) bloomUV.y = 1 - bloomUV.y;
                #endif
                color.rgb += tex2D(_BloomTex, bloomUV).rgb * _BloomIntensity;
                return color;
            }
            ENDCG
        }
    }

    Fallback Off
}
