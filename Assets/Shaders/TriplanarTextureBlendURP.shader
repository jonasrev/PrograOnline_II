Shader "Custom/TriplanarURP_AdvancedNormals"
{
    Properties
    {
        _SnowTex ("Snow Texture", 2D) = "white" {}
        _SnowNormal ("Snow Normal", 2D) = "bump" {}
        _RockTex ("Rock Texture", 2D) = "gray" {}
        _RockNormal ("Rock Normal", 2D) = "bump" {}

        _BlendSharpness ("Triplanar Blend Sharpness", Range(1, 10)) = 4
        _SnowColor ("Snow Tint", Color) = (1,1,1,1)
        _RockColor ("Rock Tint", Color) = (1,1,1,1)

        _SnowSmoothness ("Snow Smoothness", Range(0,1)) = 0.4
        _RockSmoothness ("Rock Smoothness", Range(0,1)) = 0.2
        _SnowMetallic ("Snow Metallic", Range(0,1)) = 0.0
        _RockMetallic ("Rock Metallic", Range(0,1)) = 0.0

        _Tiling ("Tiling", Float) = 1.0
        _SnowStartAngle ("Snow Start Angle (Degrees)", Range(0,90)) = 45
        _SnowBlendRange ("Snow Blend Range", Range(0.01,1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            // --- Texturas y samplers ---
            TEXTURE2D(_SnowTex); SAMPLER(sampler_SnowTex);
            TEXTURE2D(_SnowNormal); SAMPLER(sampler_SnowNormal);
            TEXTURE2D(_RockTex); SAMPLER(sampler_RockTex);
            TEXTURE2D(_RockNormal); SAMPLER(sampler_RockNormal);

            // --- Propiedades ---
            float _BlendSharpness;
            float _Tiling;
            float4 _SnowColor;
            float4 _RockColor;
            float _SnowSmoothness;
            float _RockSmoothness;
            float _SnowMetallic;
            float _RockMetallic;
            float _SnowStartAngle;
            float _SnowBlendRange;

            // --- Vertex Shader ---
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionCS = TransformWorldToHClip(OUT.worldPos);
                return OUT;
            }

            // --- Función auxiliar para normal triplanar ---
            float3 TriplanarNormal(TEXTURE2D_PARAM(tex, samplerTex), float3 worldPos, float3 worldNormal, float blendSharpness, float tiling)
            {
                float3 n = abs(worldNormal);
                n = pow(n, blendSharpness);
                n /= (n.x + n.y + n.z + 1e-6);

                float2 xProj = worldPos.zy * tiling;
                float2 yProj = worldPos.xz * tiling;
                float2 zProj = worldPos.xy * tiling;

                float3 tX = UnpackNormal(SAMPLE_TEXTURE2D(tex, samplerTex, xProj));
                float3 tY = UnpackNormal(SAMPLE_TEXTURE2D(tex, samplerTex, yProj));
                float3 tZ = UnpackNormal(SAMPLE_TEXTURE2D(tex, samplerTex, zProj));

                float3 blended = normalize(tX * n.x + tY * n.y + tZ * n.z);
                return blended;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 n = normalize(IN.worldNormal);

                // Pesos triplanares
                float3 blendWeights = pow(abs(n), _BlendSharpness);
                blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z + 1e-6);

                float3 worldPos = IN.worldPos * _Tiling;
                float2 xProj = worldPos.zy;
                float2 yProj = worldPos.xz;
                float2 zProj = worldPos.xy;

                // --- Nieve ---
                float4 snowX = SAMPLE_TEXTURE2D(_SnowTex, sampler_SnowTex, xProj);
                float4 snowY = SAMPLE_TEXTURE2D(_SnowTex, sampler_SnowTex, yProj);
                float4 snowZ = SAMPLE_TEXTURE2D(_SnowTex, sampler_SnowTex, zProj);
                float4 snowTex = snowX * blendWeights.x + snowY * blendWeights.y + snowZ * blendWeights.z;

                // --- Roca ---
                float4 rockX = SAMPLE_TEXTURE2D(_RockTex, sampler_RockTex, xProj);
                float4 rockY = SAMPLE_TEXTURE2D(_RockTex, sampler_RockTex, yProj);
                float4 rockZ = SAMPLE_TEXTURE2D(_RockTex, sampler_RockTex, zProj);
                float4 rockTex = rockX * blendWeights.x + rockY * blendWeights.y + rockZ * blendWeights.z;

                // --- Ángulo para transición nieve/roca ---
                float cosThreshold = cos(radians(_SnowStartAngle));
                float snowFactor = saturate((n.y - cosThreshold) / _SnowBlendRange);

                // --- Color final ---
                float4 baseColor = lerp(rockTex * _RockColor, snowTex * _SnowColor, snowFactor);

                // --- Normales triplanares ---
                float3 snowNormal = TriplanarNormal(TEXTURE2D_ARGS(_SnowNormal, sampler_SnowNormal), IN.worldPos, n, _BlendSharpness, _Tiling);
                float3 rockNormal = TriplanarNormal(TEXTURE2D_ARGS(_RockNormal, sampler_RockNormal), IN.worldPos, n, _BlendSharpness, _Tiling);
                float3 blendedNormal = normalize(lerp(rockNormal, snowNormal, snowFactor));

                // --- Suavidad y metálico según el tipo de superficie ---
                float smoothness = lerp(_RockSmoothness, _SnowSmoothness, snowFactor);
                float metallic = lerp(_RockMetallic, _SnowMetallic, snowFactor);

                // --- Iluminación PBR URP ---
                InputData inputData = (InputData)0;
                inputData.positionWS = IN.worldPos;
                inputData.normalWS = normalize(mul(blendedNormal, (float3x3)unity_ObjectToWorld)); // normal en espacio mundo
                inputData.viewDirectionWS = normalize(GetWorldSpaceViewDir(IN.worldPos));
                inputData.shadowCoord = TransformWorldToShadowCoord(IN.worldPos);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = baseColor.rgb;
                surfaceData.metallic = metallic;
                surfaceData.smoothness = smoothness;
                surfaceData.normalTS = float3(0,0,1);
                surfaceData.occlusion = 1;
                surfaceData.emission = 0;
                surfaceData.alpha = 1;

                return UniversalFragmentPBR(inputData, surfaceData);
            }

            ENDHLSL
        }
    }
}
