Shader "Custom/SquareOutline"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _BorderThickness ("Falloff", Range(0.0, 2.0)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Geometry-1"
        }
        LOD 200
        Pass
        {
            Blend ZERO ONE
            Offset 1,-1000
            ZTest off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _BorderThickness;

            struct VIn
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct VOut
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            VOut vert(VIn vin)
            {
                VOut vout;
                vout.vertex = UnityObjectToClipPos(vin.vertex);
                vout.uv = vin.texcoord;

                return vout;
            }

            float4 frag(VOut vout) : COLOR
            {
                const float border = step(1 - _BorderThickness, vout.uv.y) + step(1 - _BorderThickness, vout.uv.x) +
                    step(1 - _BorderThickness, 1 - vout.uv.y) + step(1 - _BorderThickness, 1 - vout.uv.x);
                clip(-border);
                return float4(0, 0, 0, 1);
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}