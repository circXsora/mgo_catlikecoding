// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/My First Lighting Shader"
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

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityPBSLighting.cginc"

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD2;
            };

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _SpecularTint;
            fixed _Smoothness;
            float _Metallic;


            Interpolators MyVertexProgram (VertexData v) {
                Interpolators i;
                i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                i.normal = UnityObjectToWorldNormal(v.normal);
                i.position = UnityObjectToClipPos(v.position);
                i.worldPos = mul(unity_ObjectToWorld, v.position);
                return i;
            }


            float4 MyFragmentProgram(Interpolators i) : SV_TARGET {
                i.normal = normalize(i.normal);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 reflectionDir = reflect(-lightDir, i.normal);
                float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;

                float3 specularTint;
                float oneMinusReflectivity;

                albedo = DiffuseAndSpecularFromMetallic(
                albedo, _Metallic, specularTint, oneMinusReflectivity
                );
                UnityLight light;
                light.color = lightColor;
                light.dir = lightDir;
                light.ndotl = DotClamped(i.normal, lightDir);

                UnityIndirect indirectLight;
                indirectLight.diffuse = 0;
                indirectLight.specular = 0;

                return UNITY_BRDF_PBS(albedo, specularTint,
                oneMinusReflectivity, _Smoothness,
                i.normal, viewDir,
                light, indirectLight
                );
            }
            ENDCG
        }
    }
}