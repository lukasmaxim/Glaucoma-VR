Shader "Debug/DrawCross"{
    
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float2 gaze;
    float offsetContextX;
    float offsetContextY;
    float4 _MainTex_TexelSize;

    float4 FragDefault(VaryingsDefault i) : SV_Target
    {
        float midLowX = 0.499f + offsetContextX;
        float midLowY = 0.499f + offsetContextY;
        float midUpX = 0.501f + offsetContextX;
        float midUpY = 0.501f + offsetContextY;

        if((i.texcoord.x > midLowX && i.texcoord.x < midUpX) || (i.texcoord.y > midLowY && i.texcoord.y < midUpY))
        {
            return float4(1,0,1,1);
        }
        else
        {
            return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        }
    }

    ENDHLSL

    SubShader{
        Cull Off ZWrite Off  ZTest Always

        Pass{

            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragDefault
            
            ENDHLSL
        }
    }
}