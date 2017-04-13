Shader "MK/MKGlow/Toon/BasicOutline" 
{
	Properties{
		_MainTex("Base", 2D) = "white" {}
		_MainColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		[Enum(On,0, Off,1)] _Zwrite("Zwrite", Int) = 1
		[Enum(Less,0, Greater,1, LEqual, 2, GEqual, 3, Equal, 4, NotEqual, 5, Always, 6)] _Ztest("Ztest", Int) = 1
	}

		CGINCLUDE

#include "UnityCG.cginc"

		sampler2D _MainTex;
	fixed4 _MainColor;

	half4 _MainTex_ST;

	struct v2f {
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		fixed4 vertexColor : COLOR;
	};

	v2f vert(appdata_full v) {
		v2f o;

		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.vertexColor = v.color * _MainColor;

		return o;
	}

	fixed4 frag(v2f i) : COLOR{
		return tex2D(_MainTex, i.uv.xy) * i.vertexColor;
	}

		ENDCG

		SubShader {
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+100" }
			Cull Back
			Lighting Off
			ZWrite [_Zwrite]
			ZTest [_Ztest]
			Fog{ Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

			Pass{

			CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

			ENDCG

		}

	}
	FallBack Off
}