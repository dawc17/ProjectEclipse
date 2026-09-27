Shader "Eclipse/RimFeather"
{
    Properties { _Color ("Color", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "Queue"="Geometry+1" "RenderType"="Transparent" }
        // The solid rim and fighter have already written depth. Only the outer
        // fringe blends into the scenery; it never changes the solid rim tint.
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct v2f { float4 pos : SV_POSITION; float coverage : TEXCOORD0; };
            float4 _Color;
            v2f vert(appdata_full v)
            {
                v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.coverage = v.color.a; return o;
            }
            float4 frag(v2f i) : SV_Target
            {
                return float4(_Color.rgb, _Color.a * smoothstep(0, 1, i.coverage));
            }
            ENDCG
        }
    }
}
