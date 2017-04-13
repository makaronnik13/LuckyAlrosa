#include "UnityCG.cginc"

#if _USEMAINTEX_ON
    UNITY_DECLARE_TEX2D(_MainTex);
    float4 _MainTex_ST;
#endif

#if _USECOLOR_ON
    float4 _Color;
#endif

struct appdata_t
{
    float4 vertex : POSITION;
    #if _USEMAINTEX_ON
        float2 texcoord : TEXCOORD0;
    #endif
};

struct v2f
{
    float4 vertex : SV_POSITION;
    #if _USEMAINTEX_ON
        float2 texcoord : TEXCOORD0;
    #endif
    UNITY_FOG_COORDS(1)
    #if _NEAR_PLANE_FADE_ON
        float fade : TEXCOORD2;
    #endif

	float4 scrPos : TEXCOORD3;
};

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
	o.scrPos = ComputeScreenPos(o.vertex);

    #if _USEMAINTEX_ON
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
    #endif
    
    #if _NEAR_PLANE_FADE_ON
        o.fade = ComputeNearPlaneFadeLinear(v.vertex);
    #endif

    UNITY_TRANSFER_FOG(o, o.vertex);
    return o;
}

float4 _LinesColor;
float _LinesThickness;
float4 _BackgroundColor;
float _NumOfLinesVertical;
float _NumOfLinesHorizontal;
float4 _UpColor;
float4 _DownColor;
float _ColorsOffsetValue;

float4 frag(v2f i) : SV_Target
{
    float4 c;

    #if _USEMAINTEX_ON
        //c = UNITY_SAMPLE_TEX2D(_MainTex, i.texcoord);

		float lineSegment = 1.0f / _NumOfLinesHorizontal;
		if ((i.texcoord.x + lineSegment /2.0f + _LinesThickness/2.0f) % lineSegment <= _LinesThickness)
		{
			c = _LinesColor;
		}
		else
		{
			lineSegment = 1.0f / _NumOfLinesVertical;
			if ((i.texcoord.y + lineSegment / 2.0f + _LinesThickness / 2.0f) % lineSegment <= _LinesThickness)
			{
				c = _LinesColor;
			}
			else
			{
				float4 offset = (_UpColor - _DownColor) * _ColorsOffsetValue;
				c = lerp(_DownColor - offset, _UpColor + offset, i.texcoord.y);
			}
		}


		/*i.scrPos.xy /= i.scrPos.w;

		float lineSegment = 1.0f / _NumOfLinesHorizontal;
		if ((i.scrPos.x + lineSegment / 2.0f + _LinesThickness / 2.0f) % lineSegment <= _LinesThickness)
		{
			c = _LinesColor;
		}
		else
		{
			lineSegment = 1.0f / _NumOfLinesVertical;
			if ((i.scrPos.y + lineSegment / 2.0f + _LinesThickness / 2.0f) % lineSegment <= _LinesThickness)
			{
				c = _LinesColor;
			}
			else
			{
				c = _BackgroundColor;
			}
		}*/
    #else
        c = 1;
    #endif

    #if _USECOLOR_ON
        c *= _Color;
    #endif
        
    UNITY_APPLY_FOG(i.fogCoord, c);

    #if _NEAR_PLANE_FADE_ON
        c.rgb *= i.fade;
    #endif

    return c;
}