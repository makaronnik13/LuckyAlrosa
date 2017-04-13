// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:922,x:33198,y:32904,varname:node_922,prsc:2|emission-4392-OUT;n:type:ShaderForge.SFN_Color,id:8305,x:30611,y:31708,ptovrint:False,ptlb:End Color 2,ptin:_EndColor2,varname:node_8305,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.2680001,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:511,x:30611,y:31525,ptovrint:False,ptlb:End Color 1,ptin:_EndColor1,varname:node_511,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_TexCoord,id:698,x:29651,y:32681,varname:node_698,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:6466,x:30512,y:33234,ptovrint:False,ptlb:Fade 2,ptin:_Fade2,varname:node_6466,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:2989,x:31357,y:33127,varname:node_2989,prsc:2|A-8672-OUT,B-5919-OUT,T-6466-OUT;n:type:ShaderForge.SFN_Multiply,id:8092,x:30888,y:31638,varname:node_8092,prsc:2|A-511-RGB,B-698-U;n:type:ShaderForge.SFN_OneMinus,id:440,x:30179,y:32443,varname:node_440,prsc:2|IN-698-U;n:type:ShaderForge.SFN_Multiply,id:5446,x:30888,y:31776,varname:node_5446,prsc:2|A-8305-RGB,B-440-OUT;n:type:ShaderForge.SFN_Add,id:8179,x:31169,y:31722,varname:node_8179,prsc:2|A-8092-OUT,B-5446-OUT,C-6989-OUT;n:type:ShaderForge.SFN_Lerp,id:328,x:31357,y:33003,varname:node_328,prsc:2|A-8672-OUT,B-3318-OUT,T-7490-OUT;n:type:ShaderForge.SFN_Slider,id:7490,x:30512,y:33145,ptovrint:False,ptlb:Fade 1,ptin:_Fade1,varname:node_7490,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Blend,id:381,x:31864,y:31962,varname:node_381,prsc:2,blmd:12,clmp:True|SRC-5831-OUT,DST-8179-OUT;n:type:ShaderForge.SFN_Color,id:9472,x:30206,y:31959,ptovrint:False,ptlb:_Color,ptin:_Color,varname:node_9472,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:0.07352948,c4:1;n:type:ShaderForge.SFN_OneMinus,id:7104,x:30179,y:32588,varname:node_7104,prsc:2|IN-698-V;n:type:ShaderForge.SFN_Min,id:6672,x:31520,y:32728,varname:node_6672,prsc:2|A-698-V,B-7104-OUT;n:type:ShaderForge.SFN_Blend,id:2411,x:32176,y:32146,varname:node_2411,prsc:2,blmd:10,clmp:True|SRC-9472-RGB,DST-381-OUT;n:type:ShaderForge.SFN_Power,id:3318,x:30944,y:32793,varname:node_3318,prsc:2|VAL-440-OUT,EXP-8672-OUT;n:type:ShaderForge.SFN_Vector1,id:8672,x:30633,y:32922,varname:node_8672,prsc:2,v1:1;n:type:ShaderForge.SFN_Power,id:5919,x:30934,y:33051,varname:node_5919,prsc:2|VAL-698-U,EXP-8672-OUT;n:type:ShaderForge.SFN_Multiply,id:5831,x:32222,y:32738,varname:node_5831,prsc:2|A-9825-OUT,B-328-OUT,C-2989-OUT,D-8592-OUT;n:type:ShaderForge.SFN_Multiply,id:9825,x:31776,y:32587,varname:node_9825,prsc:2|A-332-OUT,B-6672-OUT;n:type:ShaderForge.SFN_ArcCos,id:3960,x:32041,y:32483,varname:node_3960,prsc:2|IN-9825-OUT;n:type:ShaderForge.SFN_OneMinus,id:8592,x:32255,y:32483,varname:node_8592,prsc:2|IN-3960-OUT;n:type:ShaderForge.SFN_Multiply,id:6989,x:31060,y:31994,varname:node_6989,prsc:2|A-5611-OUT,B-328-OUT,C-2989-OUT;n:type:ShaderForge.SFN_Min,id:4259,x:30376,y:32140,varname:node_4259,prsc:2|A-3318-OUT,B-5919-OUT;n:type:ShaderForge.SFN_Multiply,id:2875,x:30602,y:32180,varname:node_2875,prsc:2|A-4259-OUT,B-6274-OUT;n:type:ShaderForge.SFN_Vector1,id:6274,x:30376,y:32300,varname:node_6274,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:5611,x:30685,y:32021,varname:node_5611,prsc:2|A-9472-RGB,B-2875-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:4346,x:31238,y:32174,varname:node_4346,prsc:2,a:1.95,b:2|IN-6161-OUT;n:type:ShaderForge.SFN_Time,id:4067,x:30862,y:32174,varname:node_4067,prsc:2;n:type:ShaderForge.SFN_Sin,id:6161,x:31061,y:32174,varname:node_6161,prsc:2|IN-4067-T;n:type:ShaderForge.SFN_Add,id:1857,x:32449,y:33484,varname:node_1857,prsc:2|A-403-RGB,B-8674-RGB,C-5553-RGB;n:type:ShaderForge.SFN_Tex2d,id:403,x:32110,y:33334,ptovrint:False,ptlb:Glow Effect 1,ptin:_GlowEffect1,varname:node_3994,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7ee9874ec4f3b2e428ee51c66dd67fb5,ntxv:0,isnm:False|UVIN-5540-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:8674,x:32110,y:33521,ptovrint:False,ptlb:Glow Effect 2,ptin:_GlowEffect2,varname:node_7981,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:44fe02b3d81d4fe4f9bc562c452a9390,ntxv:0,isnm:False|UVIN-1239-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5553,x:32110,y:33704,ptovrint:False,ptlb:Glow Effect 3,ptin:_GlowEffect3,varname:node_4466,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ae481c7c4f24dc64f8857a5429b50afa,ntxv:0,isnm:False|UVIN-7398-UVOUT;n:type:ShaderForge.SFN_Panner,id:7398,x:31463,y:33726,varname:node_7398,prsc:2,spu:0.04,spv:-0.04|UVIN-698-UVOUT;n:type:ShaderForge.SFN_Panner,id:1239,x:31463,y:33533,varname:node_1239,prsc:2,spu:0.065,spv:0.065|UVIN-698-UVOUT;n:type:ShaderForge.SFN_Panner,id:5540,x:31463,y:33339,varname:node_5540,prsc:2,spu:-0.1,spv:0.1|UVIN-698-UVOUT;n:type:ShaderForge.SFN_Multiply,id:6650,x:32750,y:32723,varname:node_6650,prsc:2|A-2411-OUT,B-6333-OUT;n:type:ShaderForge.SFN_ArcCos,id:7310,x:32008,y:32949,varname:node_7310,prsc:2|IN-6672-OUT;n:type:ShaderForge.SFN_OneMinus,id:3470,x:32222,y:32949,varname:node_3470,prsc:2|IN-7310-OUT;n:type:ShaderForge.SFN_Add,id:6333,x:32490,y:32843,varname:node_6333,prsc:2|A-5831-OUT,B-3470-OUT,C-8592-OUT;n:type:ShaderForge.SFN_Blend,id:8005,x:32777,y:32977,varname:node_8005,prsc:2,blmd:10,clmp:True|SRC-3999-OUT,DST-6650-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:3999,x:32755,y:33248,ptovrint:False,ptlb:Glow FX I/O,ptin:_GlowFXIO,varname:node_3999,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-8173-OUT,B-1857-OUT;n:type:ShaderForge.SFN_Vector1,id:8173,x:32222,y:33162,varname:node_8173,prsc:2,v1:0.25;n:type:ShaderForge.SFN_SwitchProperty,id:332,x:31417,y:32375,ptovrint:False,ptlb:Pulse FX I/O,ptin:_PulseFXIO,varname:node_332,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5750-OUT,B-4346-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:5750,x:31164,y:32352,varname:node_5750,prsc:2,a:1.78,b:1.81|IN-6161-OUT;n:type:ShaderForge.SFN_Slider,id:9645,x:33030,y:32719,ptovrint:False,ptlb:Master Level,ptin:_MasterLevel,varname:node_9645,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Multiply,id:4392,x:33020,y:32951,varname:node_4392,prsc:2|A-9645-OUT,B-8005-OUT;proporder:511-8305-9472-6466-7490-403-8674-5553-3999-332-9645;pass:END;sub:END;*/

Shader "Unlit/MetaFrameShader" {
    Properties {
        _EndColor1 ("End Color 1", Color) = (1,0,0,1)
        _EndColor2 ("End Color 2", Color) = (0,0.2680001,1,1)
        _Color ("Color", Color) = (0,1,0.07352948,1)
        _Fade2 ("Fade 2", Range(0, 1)) = 0
        _Fade1 ("Fade 1", Range(0, 1)) = 0
        _GlowEffect1 ("Glow Effect 1", 2D) = "white" {}
        _GlowEffect2 ("Glow Effect 2", 2D) = "white" {}
        _GlowEffect3 ("Glow Effect 3", 2D) = "white" {}
        [MaterialToggle] _GlowFXIO ("Glow FX I/O", Float ) = 0
        [MaterialToggle] _PulseFXIO ("Pulse FX I/O", Float ) = 1.78
        _MasterLevel ("Master Level", Range(0, 2)) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _EndColor2;
            uniform float4 _EndColor1;
            uniform float _Fade2;
            uniform float _Fade1;
            uniform float4 _Color;
            uniform sampler2D _GlowEffect1; uniform float4 _GlowEffect1_ST;
            uniform sampler2D _GlowEffect2; uniform float4 _GlowEffect2_ST;
            uniform sampler2D _GlowEffect3; uniform float4 _GlowEffect3_ST;
            uniform fixed _GlowFXIO;
            uniform fixed _PulseFXIO;
            uniform float _MasterLevel;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_2829 = _Time + _TimeEditor;
                float2 node_5540 = (i.uv0+node_2829.g*float2(-0.1,0.1));
                float4 _GlowEffect1_var = tex2D(_GlowEffect1,TRANSFORM_TEX(node_5540, _GlowEffect1));
                float2 node_1239 = (i.uv0+node_2829.g*float2(0.065,0.065));
                float4 _GlowEffect2_var = tex2D(_GlowEffect2,TRANSFORM_TEX(node_1239, _GlowEffect2));
                float2 node_7398 = (i.uv0+node_2829.g*float2(0.04,-0.04));
                float4 _GlowEffect3_var = tex2D(_GlowEffect3,TRANSFORM_TEX(node_7398, _GlowEffect3));
                float4 node_4067 = _Time + _TimeEditor;
                float node_6161 = sin(node_4067.g);
                float node_6672 = min(i.uv0.g,(1.0 - i.uv0.g));
                float node_9825 = (lerp( lerp(1.78,1.81,node_6161), lerp(1.95,2,node_6161), _PulseFXIO )*node_6672);
                float node_8672 = 1.0;
                float node_440 = (1.0 - i.uv0.r);
                float node_3318 = pow(node_440,node_8672);
                float node_328 = lerp(node_8672,node_3318,_Fade1);
                float node_5919 = pow(i.uv0.r,node_8672);
                float node_2989 = lerp(node_8672,node_5919,_Fade2);
                float node_8592 = (1.0 - acos(node_9825));
                float node_5831 = (node_9825*node_328*node_2989*node_8592);
                float3 emissive = (_MasterLevel*saturate(( (saturate(( saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) ) > 0.5 ? (1.0-(1.0-2.0*(saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) )-0.5))*(1.0-_Color.rgb)) : (2.0*saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) )*_Color.rgb) ))*(node_5831+(1.0 - acos(node_6672))+node_8592)) > 0.5 ? (1.0-(1.0-2.0*((saturate(( saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) ) > 0.5 ? (1.0-(1.0-2.0*(saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) )-0.5))*(1.0-_Color.rgb)) : (2.0*saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) )*_Color.rgb) ))*(node_5831+(1.0 - acos(node_6672))+node_8592))-0.5))*(1.0-lerp( 0.25, (_GlowEffect1_var.rgb+_GlowEffect2_var.rgb+_GlowEffect3_var.rgb), _GlowFXIO ))) : (2.0*(saturate(( saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) ) > 0.5 ? (1.0-(1.0-2.0*(saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) )-0.5))*(1.0-_Color.rgb)) : (2.0*saturate((node_5831 > 0.5 ?  (1.0-(1.0-2.0*(node_5831-0.5))*(1.0-((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) : (2.0*node_5831*((_EndColor1.rgb*i.uv0.r)+(_EndColor2.rgb*node_440)+((_Color.rgb*(min(node_3318,node_5919)*2.0))*node_328*node_2989)))) )*_Color.rgb) ))*(node_5831+(1.0 - acos(node_6672))+node_8592))*lerp( 0.25, (_GlowEffect1_var.rgb+_GlowEffect2_var.rgb+_GlowEffect3_var.rgb), _GlowFXIO )) )));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor, _Color.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
