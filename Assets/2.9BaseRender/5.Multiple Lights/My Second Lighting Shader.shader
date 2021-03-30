// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/My Second Lighting Shader"
{
    Properties
    {
        _Tint ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        [Gamma]_Metallic ("Metallic", Range(0, 1)) = 0

        _SpecularTint ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Tags {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM

            #pragma target 3.0

			#pragma multi_compile _ VERTEXLIGHT_ON

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #define FORWARD_BASE_PASS

            #include "Assets/2.9BaseRender/Common/My Lighting.cginc"
            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "ForwardAdd"
            }
            Blend One One
            ZWrite Off
            CGPROGRAM

            #pragma target 3.0

            #pragma multi_compile_fwdadd

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "Assets/2.9BaseRender/Common/My Lighting.cginc"

            ENDCG
        }
    }
}