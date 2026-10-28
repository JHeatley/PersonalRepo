// The shader every card, the playmat and the slots are drawn with (v54.1). It lives in a Resources folder so Unity always
// puts it in a build. Before, the game asked for Unity's built-in "Unlit/Texture" by name, which a standalone build leaves
// out when no material in the project uses it; the game then fell back to "Sprites/Default", which doesn't write depth,
// so the playmat was painted over every card further from the camera than its middle (the opponent's whole side).
// Opaque, writes depth, plain texture times a tint. Works in the Built-in pipeline and in URP (as an unlit pass).
Shader "PokemonTCG/Card Unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        LOD 100
        Pass
        {
            ZWrite On
            ZTest LEqual
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                c.a = 1;
                return c;
            }
            ENDCG
        }
    }
    Fallback Off
}
