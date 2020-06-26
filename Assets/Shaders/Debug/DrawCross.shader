Shader "Debug/DrawCross"{
    
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float2 gaze;
    int screen;
    float2 offset;
    float aspect;
    float scaleFactor;
    float offsetContextX;
    float offsetContextY;
    float4 _MainTex_TexelSize;

    float4 FragDefault(VaryingsDefault i) : SV_Target
    {
        float midLowX = 0.499f + offset.x;// * 1/scaleFactor * aspect + offset.x;
        float midLowY = 0.499f + offset.y;// * 1/scaleFactor * aspect + offset.y;
        float midUpX = 0.501f + offset.x;// * 1/scaleFactor * aspect + offset.x;
        float midUpY = 0.501f + offset.y;// * 1/scaleFactor * aspect + offset.y;

        // if(screen == -1)
        // {
        //     return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);  
        // }

        if((i.texcoord.x * aspect * 1/scaleFactor > midLowX && i.texcoord.x * aspect * 1/scaleFactor < midUpX) || (i.texcoord.y * aspect * 1/scaleFactor > midLowY && i.texcoord.y * aspect * 1/scaleFactor < midUpY))
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