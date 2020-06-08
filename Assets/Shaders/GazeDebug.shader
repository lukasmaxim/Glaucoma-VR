Shader "Custom/GazeDebug"
{
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_ColorMap, sampler_ColorTex);

    // just a test so I don't go insanse
    float4 Pink(VaryingsDefault i) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        return float4(1,1,0,1);
    };

    struct VertexShaderStruct
    {
        float4 Position : POSITION0;
        float2 Tex0 : TEXCOORD0;
    };

    VertexShaderStruct VertexShaderFunction(VertexShaderStruct input)
    {
        VertexShaderStruct output;

        float4 worldPosition = mul(input.Position, World);
        float4 viewPosition = mul(worldPosition, View);
        output.Position = mul(viewPosition, Projection);
        output.Tex0 = input.Tex0;

        return output;
    }

    float4 PixelShaderFunction(VertexShaderStruct input) : COLOR0
    {
        float dx = 2 * input.Tex0.x - 1;
        float dy = 2 * input.Tex0.y - 1;
        float hyp = (dx * dx + dy * dy);

        return (hyp == 1)? circleColor : otherColor;
    }


    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off  ZTest Always

        Pass{

            HLSLPROGRAM

            #pragma vertex VertexShaderFunction
            #pragma fragment PS_GaussianBlur
            
            ENDHLSL
        }
    }
}