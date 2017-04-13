Shader "Custom/TransparentGlow" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_GlowColor("GlowColor", Color) = (0,0.7,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Frequency("_Frequency", Range(0,5)) = 0.7
	}
	SubShader {
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			ZWrite Off
			ZTest Always
			Lighting Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha

		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		fixed4 _Color;
		fixed4 _GlowColor;
		half _Frequency;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half rim = saturate(dot(normalize(IN.viewDir), o.Normal));
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = _GlowColor * abs(sin(_Time.y/ _Frequency))*(1-pow(rim,0.3));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
