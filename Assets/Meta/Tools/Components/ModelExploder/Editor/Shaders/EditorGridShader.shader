// Very fast unlit shader.
// No lighting, lightmap support, etc.
// Compiles down to only performing the operations you're actually using.
// Uses material property drawers rather than a custom editor for ease of maintenance.

Shader "Meta/Tools/Editor Grid Shader"
{
	Properties
	{
		_LinesThickness("Thickness of lines", Range(0, 0.05)) = 0.01
		_NumOfLinesHorizontal("Horizontal number of lines", Float) = 16
		_NumOfLinesVertical("Vertical number of lines", Float) = 9

		_UpColor("Up Color", Color) = (0.262745, 0.290196, 0.321568)
		_DownColor("Down Color", Color) = (0.203921, 0.23137, 0.2549019)
		_ColorsOffsetValue("Colors Offset Value", Float) = 1

		_LinesColor("Color of Lines", Color) = (0.39215, 0.411764, 0.439215, 1)
		_BackgroundColor("Color of Background", Color) = (0, 0, 0, 1)

        [Header(Main Color)]
        [Toggle] _UseColor("Enabled?", Float) = 0
        _Color("Main Color", Color) = (0,0,0,1)
        [Space(20)]

        [Header(Base(RGB))]
        [Toggle] _UseMainTex("Enabled?", Float) = 1
        _MainTex("Base (RGB)", 2D) = "white" {}
        [Space(20)]

        [Header(Blend State)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
        [Space(20)]

        [Header(Other)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1.0 //"On"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("ColorWriteMask", Float) = 15 //"All"
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Blend[_SrcBlend][_DstBlend]
        ZTest[_ZTest]
        ZWrite[_ZWrite]
        Cull[_Cull]
        ColorMask[_ColorWriteMask]

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            // We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
            //#pragma target 5.0
            //#pragma only_renderers d3d11

            #pragma shader_feature _USECOLOR_ON
            #pragma shader_feature _USEMAINTEX_ON
            #pragma shader_feature _NEAR_PLANE_FADE_ON

            #include "EditorGridShader.cginc"

            ENDCG
        }
    }
}