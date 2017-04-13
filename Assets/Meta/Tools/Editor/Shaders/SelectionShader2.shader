Shader "Meta/Tools/SelectionShaders/CutoffGlow" {
        Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      [HideInInspector]_Phase("Phase", Float) = 0
      _Offset ("Ofset", Vector) = (0, 0,0,0)
      _XRay ("XRay", Range(0, 1)) = 0.7
      _Transparency ("Transparency", Range(0, 1)) = 0.7
    }





    SubShader {


      Pass {
    Name "OUTLINE"
			//Tags { "LightMode" = "Always" }
			//Tags {"Queue"="Cutout" "RenderType"="Cutout"}
			//Cull Front
			ZWrite Off
			ZTest GEqual
			ColorMask RGB
 
			// you can choose what kind of blending mode you want for the outline
			Blend SrcAlpha OneMinusSrcAlpha // Normal

    CGPROGRAM

    #pragma vertex vert             
    #pragma fragment frag alpha
    #include "UnityCG.cginc"

    float4 _RimColor;
    float _Phase;
    sampler2D _MainTex;
    float4 _Offset;
    sampler2D _BumpMap;
    float _XRay;
    float4 _BumpMap_ST;

    struct vertInput {
        float4 pos : POSITION;
        float2 uv : TEXCOORD0;
        float2 tuv: TEXCOORD1;
    };  

    struct vertOutput {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float2 tuv: TEXCOORD1;
    };


    vertOutput vert(vertInput input) {
        vertOutput o;
        o.pos = mul(UNITY_MATRIX_MVP, input.pos);
        o.uv =  TRANSFORM_TEX(input.uv, _BumpMap);
        o.tuv = input.tuv;
        return o;
    }

    half4 frag(vertOutput output) : COLOR {

    	fixed4 col = _RimColor * (tex2D(_BumpMap ,output.uv+_Offset*_Phase));
    	col.a = _XRay;
        return col; 
    }
    ENDCG


	}




     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      CGPROGRAM
      #include "UnityCG.cginc"
      #pragma surface surf Lambert alpha

      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 viewDir;
          float2 uv_Illum;
      };
      float _Phase;
      sampler2D _MainTex;
      float4 _Offset;
      sampler2D _BumpMap;
      float4 _RimColor;
      float _RimPower;
      float _Transparency;

      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = (0,0,0);
          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap+ _Offset*_Phase));
          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          o.Alpha = _Transparency * rim;
          o.Emission = _RimColor.rgb * pow (rim, _RimPower);
      }
      ENDCG



    } 
   
   
    Fallback "Diffuse"
  }