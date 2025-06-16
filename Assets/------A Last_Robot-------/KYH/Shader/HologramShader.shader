Shader "Custom/HologramShader"
{
    Properties
    {
        _MainTex ("Video Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (0.2, 1, 1, 1)
        _Alpha ("Alpha", Range(0,1)) = 0.8
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _EmissionColor;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _EmissionColor.rgb;
                col.a *= _Alpha;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}