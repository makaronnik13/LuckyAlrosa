Shader "SelectionShaders/GlowNet" {
     Properties {
	  _Color("Color", Color) = (0.26,0.19,0.16,0.0)
	  _MainTex("Texture", 2D) = "white" {}
      _NetTex ("Net", 2D) = "white" {}
      _TransparencyMap ("TransparencyMap", 2D) = "white" {}
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _Offset ("Ofset", Vector) = (0, 0,0,0)
    }

    SubShader {

	  Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
	  ZWrite Off
	  ZTest Always
	  Lighting Off
      Blend SrcAlpha OneMinusSrcAlpha


      CGPROGRAM
      #include "UnityCG.cginc"
      #pragma surface surf NoLighting alpha:fade

	  fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	  {
		  fixed4 c;
		  c.rgb = s.Albedo;
		  c.a = s.Alpha;
		  return c;
	  }

      struct Input {
          float2 uv_NetTex;
		  float2 uv_MainTex;
          float2 uv_TransparencyMap;
          float3 viewDir;
          float2 uv_Illum;
      };

      sampler2D _NetTex;
	  sampler2D _MainTex;
      sampler2D _TransparencyMap;
      float4 _Offset;
      float4 _RimColor;
	  float4 _Color;

      void surf (Input IN, inout SurfaceOutput o) {
      	half rim = saturate(dot (normalize(IN.viewDir), o.Normal));

		float glowAlpha = tex2D(_TransparencyMap, IN.uv_TransparencyMap + _Offset*_Time.y).a;

		//+ tex2D(_NetTex, IN.uv_NetTex).a*_RimColor*(1 - glowAlpha)*(tex2D(_MainTex, IN.uv_MainTex + _Offset*_Time.y * 3).a)*rim;
		o.Albedo = tex2D(_NetTex, IN.uv_NetTex - _Offset*_Time.y).a*_RimColor *(1 - tex2D(_MainTex, IN.uv_MainTex + _Offset*_Time.y * 3).a) + tex2D(_MainTex, IN.uv_MainTex + _Offset*_Time.y * 3)*tex2D(_MainTex, IN.uv_MainTex + _Offset*_Time.y * 3).a*_Color;//_RimColor.rgb*(netAlpha *2* glowAlpha +  glowAlpha);
		o.Alpha = (1-glowAlpha)*(tex2D(_MainTex, IN.uv_MainTex + _Offset*_Time.y*3).a + (rim - (1-_RimColor.a))* tex2D(_NetTex, IN.uv_NetTex - _Offset*_Time.y).a);
      }
      ENDCG



    } 
   
   
    Fallback "Diffuse"
  }
