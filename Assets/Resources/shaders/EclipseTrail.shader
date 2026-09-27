Shader "Hidden/Eclipse/Trail"
{
    // Fighter trail ribbons (WeaponTrail), hard-edged. Each pixel of one ribbon mesh is drawn
    // once: the first pass marks stencil bit 128 where it draws and skips pixels
    // already marked, so a swing that folds over itself does not double its alpha.
    // The second pass clears the bit again for everything drawn after.
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source blend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination blend", Float) = 10
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" }
        Cull Off
        ZWrite Off
        Lighting Off

        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            Stencil
            {
                Ref 128
                ReadMask 128
                WriteMask 128
                Comp NotEqual
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata { float4 vertex : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * i.color;
                // Invisible pixels must not claim the stencil from older, visible ones.
                clip(c.a - 0.004);
                return c;
            }
            ENDCG
        }

        Pass
        {
            ColorMask 0
            Stencil
            {
                Ref 0
                WriteMask 128
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 vert(float4 vertex : POSITION) : SV_POSITION { return UnityObjectToClipPos(vertex); }
            fixed4 frag() : SV_Target { return 0; }
            ENDCG
        }
    }

    Fallback Off
}
