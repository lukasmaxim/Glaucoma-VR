Shader "Impairment/BoxBlurMask"{

    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    #define PI 3.14159265359

    int _KernelSize1;
    int _KernelSize2;
    int _KernelSize3;
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float2 _MainTex_TexelSize;
    TEXTURE2D_SAMPLER2D(_MaskTexLeftContext, sampler_MaskTexLeftContext);
    TEXTURE2D_SAMPLER2D(_MaskTexLeftFocus, sampler_MaskTexLeftFocus);
    TEXTURE2D_SAMPLER2D(_MaskTexRightContext, sampler_MaskTexRightContext);
    TEXTURE2D_SAMPLER2D(_MaskTexRightFocus, sampler_MaskTexRightFocus);
    float maskAlpha;

    int screen;
    int eye;

    float2 offset;
    float2 offsetBlur;

    float3 gaze;
    float4 gazeProjected;
    float2 gazeNormalized;

    float scaleFactor;
    float aspect;

    float2 samplePoint;

    // draw a circle where the gaze goes
    float4 FragDefault(VaryingsDefault i) : SV_Target
    {
        // gaze is in object coords; first turn into world coords, then use the view projection matrix (VP) to get clip coords;
        // normally we could do this with MVP, but MVP is no longer :(
        //
        // turn object coords into clip coords by multiplying unity_ObjectToWorld (for getting world coords) and unity_MatrixVP (for getting eye coords and then clip coords)
        // helpful image: http://blog.hvidtfeldts.net/media/opengl.png
        gazeProjected = mul(mul(unity_ObjectToWorld, unity_MatrixVP), float4(gaze, 1.0f)); // 4th dim has to be 1.0
        gazeNormalized = (gazeProjected.xy / gazeProjected.w) * float2(0.5f, -0.5f); // multiplication and addition is for transforming -1...1 to 0...1 in directx and metal

        float4 originalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

        // TODO maybe exhange with method call
        if(screen == -1)
        {
            if(eye == -1)
            {
                samplePoint = float2(i.texcoord.x * 1/scaleFactor + -gazeNormalized.x * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
                maskAlpha = SAMPLE_TEXTURE2D(_MaskTexLeftContext, sampler_MaskTexLeftContext, samplePoint).a;
            }
            else
            {
                samplePoint = float2(i.texcoord.x * 1/scaleFactor + -gazeNormalized.x * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
                maskAlpha = SAMPLE_TEXTURE2D(_MaskTexRightContext, sampler_MaskTexRightContext, samplePoint).a;
            }
        }
        else
        {
            if(eye == -1)
            {
                samplePoint = float2(i.texcoord.x * aspect * 1/scaleFactor + -gazeNormalized.x * aspect * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
                maskAlpha = SAMPLE_TEXTURE2D(_MaskTexLeftFocus, sampler_MaskTexLeftFocus, samplePoint).a;
            }
            else
            {
                samplePoint = float2(i.texcoord.x * aspect * 1/scaleFactor + -gazeNormalized.x * aspect * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
                maskAlpha = SAMPLE_TEXTURE2D(_MaskTexRightFocus, sampler_MaskTexRightFocus, samplePoint).a;
            }
        }

        // box blur with samples
        // _KernelSize1 = 5;
        // _KernelSize2 = 50;
        // _KernelSize3 = 100;

        half3 sum1 = half3(0.0, 0.0, 0.0);
        int upper1 = ((_KernelSize1 - 1) / 2);
        int lower1 = -upper1;

        int samples1 = _KernelSize1/5;
        int sampleStep1 = _KernelSize1 / samples1;

        for (int x = lower1; x <= upper1; x+= sampleStep1)
        {
            for (int y = lower1; y <= upper1; y+=sampleStep1)
            {
                float2 offsetBlur1 = float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
                sum1 += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + offsetBlur1) * sampleStep1 *sampleStep1;
            }
        }
        sum1 /= (_KernelSize1 * _KernelSize1);

        half3 sum2 = half3(0.0, 0.0, 0.0);
        int upper2 = ((_KernelSize2 - 1) / 2);
        int lower2 = -upper2;

        int samples2 = _KernelSize2/5;
        int sampleStep2 = _KernelSize2 / samples2;

        for (int x = lower2+1; x <= upper2; x += sampleStep2)
        {
            for (int y = lower2+1; y <= upper2; y += sampleStep2)
            {
                float2 offsetBlur2 = float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
                sum2 += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + offsetBlur2) * sampleStep2 * sampleStep2;
            }
        }

        
        sum2 /= (_KernelSize2 * _KernelSize2);

        half3 sum3 = half3(0.0, 0.0, 0.0);
        int upper3 = ((_KernelSize3 - 1) / 2);
        int lower3 = -upper3;

        int samples3 = _KernelSize3/5;
        int sampleStep3 = _KernelSize3 / samples3;

        for (int x = lower3+1; x <= upper3; x += sampleStep3)
        {
            for (int y = lower3+1; y <= upper3; y += sampleStep3)
            {
                float2 offsetBlur3 = float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
                sum3 += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + offsetBlur3) * sampleStep3 * sampleStep3;
            }
        }

        sum3 /= (_KernelSize3 * _KernelSize3);

        if (maskAlpha <= 0.33) {
            return lerp(originalColor, float4(sum1,1), maskAlpha / 0.33);
        }
        else if (maskAlpha <= 0.66) {
            return lerp(float4(sum1,1), float4(sum2,1), (maskAlpha - 0.33) / 0.33);
        }
        else {
            return lerp(float4(sum2,1), float4(sum3,1), (maskAlpha - 0.66) / 0.33);
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