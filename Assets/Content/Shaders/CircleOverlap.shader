// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CircleOverlap" {
  Properties {
    _Color ("Tint Color", Color) = (1,1,1,1)
		_BorderThickness ("Falloff", Range(0.0, 1.0)) = 0.1
  }
  SubShader {
    Tags { "Queue"="Geometry+1" }
    LOD 200
    pass
    {
      Blend ZERO ONE
      Offset 1,-1000
      ZTest off
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

			float _BorderThickness;
      
      struct VIn {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
      };
      
      struct VOut {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
      };
  
      VOut vert (VIn vin) {
        VOut vout;
        vout.vertex = UnityObjectToClipPos(vin.vertex);
        vout.uv = vin.texcoord;
        return vout;
      }
      
      float4 frag(VOut vout) : COLOR
      {
        float circle = distance(float2(0.5,0.5),vout.uv)*2;
        float innerCircle = step(1-_BorderThickness,circle);
        clip(-innerCircle);
        return float4(0,0,0,1);
      }
      
      ENDCG
    }
    pass
    {
      //Blend SrcAlpha OneMinusSrcAlpha
      ZTest LEqual
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      fixed4 _Color;

      struct VIn {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
      };
      
      struct VOut {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
      };
  
      VOut vert (VIn vin) {
        VOut vout;
        vout.vertex = UnityObjectToClipPos(vin.vertex);
        vout.uv = vin.texcoord;
        return vout;
      }
      
      fixed4 frag(VOut vout) : COLOR
      {
        //return float4(1,1,1,1);
        float circle = distance(float2(0.5,0.5),vout.uv)*2;
        float outerCircle = step(1,circle);
        clip(-outerCircle);
        return _Color;
      }
      
      ENDCG
    }
  } 
  FallBack "Diffuse"
}