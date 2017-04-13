Shader "Meta/Tools/SelectionShaders/TranspatentGlow" {
        Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _TransparencyMap ("TransparencyMap", 2D) = "white" {}
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0,1)) = 1.0
      [HideInInspector]_Phase("Phase", Float) = 0
      _Offset ("Ofset", Vector) = (0, 0,0,0)
      _Transparency ("Transparency", Range(0, 1)) = 0.7
      _NormalPower ("NormalPower", Range(-1,5)) = 1
    }





    SubShader {

     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     ZWrite Off
	 ZTest GEqual

      CGPROGRAM
      #include "UnityCG.cginc"
      #pragma surface surf Lambert alpha

      struct Input {
          float2 uv_MainTex;
          float2 uv_TransparencyMap;
          float3 viewDir;
          float2 uv_Illum;
      };

      float _Phase;
      sampler2D _MainTex;
      sampler2D _TransparencyMap;
      float4 _Offset;
      float4 _RimColor;
      float _Transparency;
      float _RimPower;

      void surf (Input IN, inout SurfaceOutput o) {
      	half rim = saturate(dot (normalize(IN.viewDir), o.Normal));
        o.Alpha = _Transparency*(tex2D(_MainTex, IN.uv_MainTex).rgb-(0.5-tex2D(_TransparencyMap, IN.uv_TransparencyMap+ _Offset*_Phase).rgb)*rim*_RimPower);
        o.Emission = _RimColor.rgb;
      }
      ENDCG



    } 
   
   
    Fallback "Diffuse"
  }
