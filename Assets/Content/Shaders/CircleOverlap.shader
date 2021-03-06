// The original shader was written by Sander Homan and provided in his blogpost
// http://homans.nhlrebel.com/2011/12/07/remove-overlap-of-circles-with-shaders/
// Daniel Steegmüller (http://www.cxyda.github.io) extended the codebase and added support for constant circle widths
// regardless of the size of the mesh it is drawn to.

Shader "Custom/CircleOverlap" {
  Properties {
    _Color ("Tint Color", Color) = (1,1,1,1)
		_BorderThickness ("Falloff", Range(0.0, 2.0)) = 0.1
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
        float length_factor : COLOR0;
      };
  
      VOut vert (VIn vin) {
        VOut vout;
        vout.vertex = UnityObjectToClipPos(vin.vertex);
        vout.uv = vin.texcoord;
        const float quadLength = length(mul(unity_ObjectToWorld, float4(1, 0, 0, 0)));
        vout.length_factor = 1.0 / quadLength;
        return vout;
      }
      
      float4 frag(VOut vout) : COLOR
      {
        float circle = distance(float2(0.5,0.5),vout.uv)*2;
        float innerCircle = step(1-_BorderThickness * vout.length_factor,circle);
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
        float circle = distance(float2(0.5,0.5),vout.uv)*2;
        float outerCircle = step(1,circle);
        clip(-outerCircle);
        return _Color;
      }
      
      ENDCG
    }
  } 
  FallBack "Transparent/VertexLit"
}