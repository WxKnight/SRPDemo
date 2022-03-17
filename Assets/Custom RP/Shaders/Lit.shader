Shader "Custom RP/lit"
{
    Properties
    {
        _BaseColor("Color",color) = (0.5,0.5,0.5,1.0)
        _BaseMap("Texture",2D)="White"{}
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend ("Src Blend",Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend ("Dst Blend",Float) = 0
        [Enum(off,0,on,1)] _ZWrite("Z Write",Float)=1
        _CutOff("Alpha CutOff",Range(0.0,1.0))=0.5
        [Toggle(_CLIPPING)] _Clipping("Alpha Clipping" , Float) = 0
        [Toggle(_PREMULTIPLY_ALPHA)] _PremulAlpha ("Premultiply Alpha", Float) = 0
        _Metallic("Metallic",Range(0.0,1.0)) = 0.5
        _Smoothness("Smoothness",Range(0.0,1.0)) = 0.5
    }

    SubShader
    {
        Pass
        {
            Tags
            {
                "LightMode" = "CustomLit"
            }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite[_ZWrite]
            HLSLPROGRAM
            #pragma target 3.5
            #include "LitPass.hlsl"
            #pragma Shader_feature _CLIPPING
            #pragma shader_feature _PREMULTIPLY_ALPHA
            #pragma multi_compile_instancing
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            ENDHLSL

        }

		Pass {
			Tags {
				"LightMode" = "ShadowCaster"
			}

			ColorMask 0

			HLSLPROGRAM
			#pragma target 3.5
			#pragma shader_feature _CLIPPING
			#pragma multi_compile_instancing
			#pragma vertex ShadowCasterPassVertex
			#pragma fragment ShadowCasterPassFragment
			#include "ShadowCasterPass.hlsl"
			ENDHLSL
		}
    }

    CustomEditor "CustomShaderGUI"
}