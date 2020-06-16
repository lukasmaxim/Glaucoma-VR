Shader "Impairment/BlurMask"{

    Properties
    {
        [Slider]_Contrast ("Contrast", Range(0, 1)) = 0.5
        _KernelSize("Kernel Size (N)", Int) = 10
    }

    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    int _KernelSize;
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float2 _MainTex_TexelSize;
    TEXTURE2D_SAMPLER2D(_MaskTexLeftContext, sampler_MaskTexLeftContext);
    TEXTURE2D_SAMPLER2D(_MaskTexLeftFocus, sampler_MaskTexLeftFocus);
    TEXTURE2D_SAMPLER2D(_MaskTexRightContext, sampler_MaskTexRightContext);
    TEXTURE2D_SAMPLER2D(_MaskTexRightFocus, sampler_MaskTexRightFocus);
    float _Contrast;
    float maskAlpha;

    int screen;
    int eye;

    float2 cutoff;

    float2 offset;
    float2 offset_asdf;

    float3 gaze;
    float4 gazeProjected;
    float2 gazeNormalized;

    float scaleFactor;
    float aspect;

    float distance;

    float circleRadius;
    float4 circleColor;

    float2 samplePoint;

    // draw a circle where the gaze goes
    float4 FragDefault(VaryingsDefault i) : SV_Target
    {
        half3 sum = half3(0.0, 0.0, 0.0);

        int upper = ((_KernelSize - 1) / 2);
        int lower = -upper;

        for (int x = lower; x <= upper; ++x)
        {
            for (int y = lower; y <= upper; ++y)
            {
                float2 offset_asdf = float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + offset_asdf);
            }
        }

        sum /= (_KernelSize * _KernelSize);

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
            if(eye == 1)
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

        return lerp(originalColor, float4(sum, 1.0), maskAlpha);
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