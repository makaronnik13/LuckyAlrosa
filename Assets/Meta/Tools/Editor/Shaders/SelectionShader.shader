Shader "Meta/Tools/SelectionShaders/TesselerationSelection" {
	Properties { 
		_Displacement_("Amplitude", Range(0.01, 0.5)) = -0.15
		[Header (Textures and Bumpmaps)]_DispMap("DispMap", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0,0,0,0)
		_Transparency("Transparency", Range(0,1)) = 0
		_FlickerFrequency("FlickerFrequency", Range(1,500)) = 10
		[HideInInspector]_Phase ("Phase", float) = 0
	}
	SubShader {
		LOD 300
		Tags {
			"Queue" = "Geometry"
			"RenderType" = "Transparent"
		}

		Cull Back
		//ColorMask   RGBA
		ZWrite Off
	 	ZTest Always
	 	Blend SrcAlpha OneMinusSrcAlpha
	 

		CGPROGRAM 
		#pragma surface surf Lambert alpha vertex:vert tessellate:tess
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#include "UnityCG.cginc"

		float _Cutoff_;
		sampler2D _DispMap;

		float _Displacement_;
		uniform float4 _DispMap_ST;
		float4 _OutlineColor;
		float2 _DispMap_TexelSize;
		float _Transparency;
		float _FlickerFrequency;

		struct appdata{
			float4 vertex    : POSITION;  // The vertex position in model space.
			float3 normal    : NORMAL;    // The vertex normal in model space.
			float4 texcoord  : TEXCOORD0; // The first UV coordinate.
			float4 texcoord1 : TEXCOORD1; // The second UV coordinate.
			float4 texcoord2 : TEXCOORD2; // The third UV coordinate.
			float4 tangent   : TANGENT;   // The tangent vector in Model Space (used for normal mapping).
			float4 color     : COLOR;     // Per-vertex color.
		};

		struct Input{
			float2 uv_DispMap;
			float3 viewDir;
			float3 worldPos;
			float3 worldRefl;
			float3 worldNormal;
			float4 screenPos;
			float4 color : COLOR;

			INTERNAL_DATA
		};

		float4 tess (appdata v0, appdata v1, appdata v2) {
			float minDist = 10.0;
			float maxDist = 25.0;
			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, 50);

		}

		void vert (inout appdata v){
			v.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy, _DispMap);

			float disp = (tex2Dlod(_DispMap, float4((v.texcoord.xy + _Time.y *_DispMap_TexelSize.x).x, (v.texcoord.xy + _Time.y * _DispMap_TexelSize.x).y, 1.0f, 0) * 15)).r * _Displacement_*abs(sin(_Time.x*_FlickerFrequency))/2;
			v.vertex.xyz += v.normal * disp;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			o.Emission = _OutlineColor;
			//o.Albedo = float4(0.6397059, 0.1975562, 0.1975562, 1).rgb;
			o.Alpha = _Transparency;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
 