Shader "Custom RP/Unlit"
{
    Properties
    {
        _BaseColor("Color",color) = (1.0,1.0,1.0,1.0)
        _BaseMap("Texture",2D)="White"{}
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend ("Src Blend",Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend ("Dst Blend",Float) = 0
        [Enum(off,0,on,1)] _ZWrite("Z Write",Float)=1
        _CutOff("Alpha CutOff",Range(0.0,1.0))=0.5
        [Toggle(_CLIPPING)] _Clipping("Alpha Clipping" , Float) = 0
        //在AlphaTest渲染队列。在所有完全不透明渲染之后，这样不会影响GPU 做early z，在Opaque遮盖住后面的东西，就不需要再做处理
    }

    SubShader
    {
        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite[_ZWrite]
            HLSLPROGRAM
            #include "UnlitPass.hlsl"
            #pragma Shader_feature _CLIPPING
            #pragma multi_compile_instancing
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            ENDHLSL

        }
    }
    
    CustomEditor "CustomShaderGUI"
}