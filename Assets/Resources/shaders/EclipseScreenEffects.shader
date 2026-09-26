Shader "Hidden/Eclipse/ScreenEffects"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}
        _BloomTex ("Bloom", 2D) = "black" {}
        _HalationTex ("Halation", 2D) = "black" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _BloomTex;
    sampler2D _HalationTex;
    float4 _MainTex_TexelSize;
    float _Threshold;
    float _Knee;
    float _BloomIntensity;
    float _Impact;
    float _Saturation;
    float _Contrast;
    float _Brightness;
    float4 _Tint;
    float _TintStrength;
    float _Vignette;
    float4 _VignetteCenter;
    float _Grain;
    float _GrainTime;
    float _Halation;
    float4 _HalationColor;
    float _AccentStrength;
    float _AccentWidth;
    float _AccentHue;

    half HueOf(half3 c)
    {
        half4 k = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        half4 p = lerp(half4(c.bg, k.wz), half4(c.gb, k.xy), step(c.b, c.g));
        half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));
        half d = q.x - min(q.w, q.y);
        return abs(q.z + (q.w - q.y) / (6.0 * d + 1e-4));
    }

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
                // Halation: a warm glow bleeding from highlights, as on film.
                color.rgb += tex2D(_HalationTex, bloomUV).rgb * _HalationColor.rgb * _Halation;

                // sf2.fx.screen grading (identity when no grade is active).
                half luma = dot(color.rgb, half3(0.299, 0.587, 0.114));
                half saturation = _Saturation;
                if (_AccentStrength > 0.001)
                {
                    // Pixels near the accent hue keep their colour through desaturation.
                    half chroma = max(color.r, max(color.g, color.b)) - min(color.r, min(color.g, color.b));
                    half distance = abs(HueOf(color.rgb) - _AccentHue);
                    distance = min(distance, 1.0 - distance);
                    half keep = (1.0 - smoothstep(0.0, _AccentWidth, distance)) * saturate(chroma * 4.0) * _AccentStrength;
                    saturation = lerp(saturation, max(saturation, 1.0), keep);
                }
                color.rgb = lerp(luma.xxx, color.rgb, saturation);
                color.rgb = (color.rgb - 0.5) * _Contrast + 0.5 + _Brightness;
                color.rgb = lerp(color.rgb, color.rgb * _Tint.rgb, _TintStrength);
                // The vignette centre moves with _VignetteCenter (-1..1 of the half
                // screen), so the dark falls mostly on the side away from the light.
                float2 fromVignette = fromCenter - _VignetteCenter.xy * 0.5;
                float edge = saturate((length(fromVignette) * 1.41421356 - 0.35) / 0.65);
                color.rgb *= 1 - _Vignette * edge * edge;
                if (_Grain > 0.001)
                {
                    float n = frac(sin(dot(uv * 431.7 + _GrainTime * 17.3, float2(12.9898, 78.233))) * 43758.5453);
                    color.rgb += (n - 0.5) * _Grain * 0.16;
                }
                color.rgb = saturate(color.rgb);
                return color;
            }
            ENDCG
        }
    }

    Fallback Off
}
