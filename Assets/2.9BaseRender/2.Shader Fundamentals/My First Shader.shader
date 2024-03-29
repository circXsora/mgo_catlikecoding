﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/My First Shader"
{
    Properties
    {
        _Tint ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            struct VertexData {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            Interpolators MyVertexProgram (VertexData v) {
                Interpolators i;
                i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                i.position = UnityObjectToClipPos(v.position);
                return i;
            }


            float4 MyFragmentProgram(Interpolators i) : SV_TARGET {
                return tex2D(_MainTex, i.uv) * _Tint;
            }

            ENDCG
        }
    }
}
