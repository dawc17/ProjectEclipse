Shader "Eclipse/UI/Title Ink"
{
    Properties { [PerRendererData] _MainTex ("Source", 2D) = "white" {} _WhiteInk ("White ink", Float) = 0 }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            sampler2D _MainTex;
            float _WhiteInk;
            v2f vert(appdata v) { v2f o; o.vertex = UnityObjectToClipPos(v.vertex); o.uv = v.uv; o.color = v.color; return o; }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 c = tex2D(_MainTex, i.uv).rgb;
                // Recover only black and vermilion ink from the original loading plate.
                // The old baked paper remains in its source asset and never covers the new paper.
                float blackInk = 1 - smoothstep(.16, .43, max(c.r, max(c.g, c.b)));
                float redInk = smoothstep(.18, .42, c.r - max(c.g, c.b));
                fixed3 ink = lerp(fixed3(.045,.035,.025), fixed3(.56,.09,.06), redInk);
                return fixed4(lerp(ink, fixed3(.97,.91,.79), _WhiteInk), max(blackInk,redInk) * i.color.a);
            }
            ENDCG
        }
    }
}
