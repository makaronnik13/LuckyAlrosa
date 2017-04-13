#if _USEMAINTEX_ON
    UNITY_DECLARE_TEX2D(_MainTex);
#endif

#if _USECOLOR_ON
    float4 _Color;
#endif

#if _USEBUMPMAP_ON
    UNITY_DECLARE_TEX2D(_BumpMap);
#endif

#if _USEEMISSIONTEX_ON
    UNITY_DECLARE_TEX2D(_EmissionTex);
#endif

struct Input
{
    // Will get compiled out if not touched
    float2 uv_MainTex;

    #if _NEAR_PLANE_FADE_ON
        float fade;
    #endif
};

void vert(inout appdata_full v, out Input o)
{
    UNITY_INITIALIZE_OUTPUT(Input, o);
    
    #if _NEAR_PLANE_FADE_ON
        o.fade = ComputeNearPlaneFadeLinear(v.vertex);
    #endif
}


float _SideX;
float _SideY;
float _AngleCoeff;
float _RatioX;
float _Thin;
float _PercentOfAppearing;

void surf(Input IN, inout SurfaceOutput o)
{
    float4 c = 1;

    #if _USEMAINTEX_ON
        //c = UNITY_SAMPLE_TEX2D(_MainTex, IN.uv_MainTex);
    #else
        c = 1;
    #endif

    #if _USECOLOR_ON
		c = float4(0,0,0,0);

		float2 coordsUV = float2(IN.uv_MainTex.r - 0.5f, IN.uv_MainTex.g - 0.5f);

		float xFactor = coordsUV.x*_SideX;
		if (xFactor >= 0.0f)
		{
			if (xFactor <= _PercentOfAppearing / 2.0f)
			{
				if (coordsUV.y*_SideY >= 0.0f)
				{
					float targetY;
					if (abs(coordsUV.x / 0.5f) >= _RatioX)
					{
						targetY = (_RatioX / 2.0f)*_AngleCoeff;
					}
					else
					{
						targetY = coordsUV.x*_AngleCoeff;
					}

					if ((coordsUV.y >= targetY - _Thin) && (coordsUV.y <= targetY + _Thin))
					{
						c = _Color;
					}
				}
			}
		}
    #endif

    o.Albedo = c.rgb;
    
    #if _NEAR_PLANE_FADE_ON
        o.Albedo.rgb *= IN.fade;
    #endif
    
    o.Alpha = c.a;

    #if _USEBUMPMAP_ON
        o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2D(_BumpMap, IN.uv_MainTex));
    #endif

    #if _USEEMISSIONTEX_ON
        o.Emission = UNITY_SAMPLE_TEX2D(_EmissionTex, IN.uv_MainTex);
    #endif

		o.Emission = c;
		o.Albedo = 0;

}