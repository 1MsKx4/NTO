Shader "NTO/FinalPostProcess"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "FinalDistortion"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _NTOFinalStrength;
            float4 _NTOFinalTint;
            float4 _NTOFinalSettings;
            TEXTURE2D(_NTOFilter);
            SAMPLER(sampler_NTOFilter);
            float _NTOFilterStrength;
            int _NTOSaturationOnly;

            float LuminanceValue(float3 color)
            {
                return dot(color, float3(0.3, 0.59, 0.11));
            }

            float3 LayerColor(float3 layer, float light)
            {
                float3 result = layer + light - LuminanceValue(layer);
                float low = min(result.r, min(result.g, result.b));
                float high = max(result.r, max(result.g, result.b));
                if (low < 0)
                {
                    result = light + (result - light) * light / max(light - low, 0.0001);
                }
                if (high > 1)
                {
                    result = light + (result - light) * (1 - light) / max(high - light, 0.0001);
                }
                return saturate(result);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float strength = saturate(_NTOFinalStrength);
                float wave = _NTOFinalSettings.x * strength;
                float clock = _Time.y * _NTOFinalSettings.y;
                float2 uv = 0.5 + (input.texcoord - 0.5) / (1.0 + wave * 3.0);
                uv.x += sin(uv.y * 18.0 + clock) * wave;
                uv.y += sin(uv.x * 14.0 - clock * 0.8) * wave * 0.55;
                uv = saturate(uv);

                float shift = _NTOFinalSettings.z * strength;
                half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
                color.r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp,
                    saturate(uv + float2(shift, 0))).r;
                color.b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp,
                    saturate(uv - float2(shift, 0))).b;

                half4 layer = SAMPLE_TEXTURE2D(_NTOFilter, sampler_NTOFilter, uv);
                float light = LuminanceValue(color.rgb);
                float3 filtered = LayerColor(layer.rgb, light);
                if (_NTOSaturationOnly == 1)
                {
                    float baseSat = max(color.r, max(color.g, color.b)) - min(color.r, min(color.g, color.b));
                    float layerSat = max(layer.r, max(layer.g, layer.b)) - min(layer.r, min(layer.g, layer.b));
                    filtered = LayerColor(light + (color.rgb - light) * layerSat / max(baseSat, 0.0001), light);
                }
                color.rgb = lerp(color.rgb, filtered, layer.a * _NTOFilterStrength);
                color.rgb = lerp(color.rgb, color.rgb * _NTOFinalTint.rgb, strength);
                float2 edge = (input.texcoord - 0.5) * 2.0;
                float vignette = saturate(dot(edge, edge) * 0.5);
                color.rgb *= 1.0 - vignette * _NTOFinalSettings.w * strength;
                return color;
            }
            ENDHLSL
        }
    }
}
