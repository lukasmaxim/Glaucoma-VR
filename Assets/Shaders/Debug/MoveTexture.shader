Shader "Debug/MoveTexture"{

    Properties
	{
		_MaskTexContext ("Mask Texture Context", 2D) = "white" {}
		_MaskTexFocus ("Mask Texture Focus", 2D) = "white" {}
        _MaskColor ("Mask Color", Color) = (0,1,1,1)
	}

    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_MaskTexContext, sampler_MaskTexContext);
    TEXTURE2D_SAMPLER2D(_MaskTexFocus, sampler_MaskTexFocus);
    float4 _MainTex_TexelSize;
    float4 _MaskTexContext_TexelSize;
    float4 _MaskTexFocus_TexelSize;
	float4 _MaskColor;
    float4 maskColor;

    int screen;

    float2 cutoff;

    float2 offset;

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
        // gaze is in object coords; first turn into world coords, then use the view projection matrix (VP) to get clip coords;
        // normally we could do this with MVP, but MVP is no longer :(
        //
        // turn object coords into clip coords by multiplying unity_ObjectToWorld (for getting world coords) and unity_MatrixVP (for getting eye coords and then clip coords)
        // helpful image: http://blog.hvidtfeldts.net/media/opengl.png
        gazeProjected = mul(mul(unity_ObjectToWorld, unity_MatrixVP), float4(gaze, 1.0f)); // 4th dim has to be 1.0
        gazeNormalized = (gazeProjected.xy / gazeProjected.w) * float2(0.5f, -0.5f); // multiplication and addition is for transforming -1...1 to 0...1 in directx and metal

		float4 originalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

        if(screen == -1)
        {
            samplePoint = float2(i.texcoord.x * 1/scaleFactor + -gazeNormalized.x * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
            maskColor = SAMPLE_TEXTURE2D(_MaskTexContext, sampler_MaskTexContext, samplePoint);
        }
        else
        {
            samplePoint = float2(i.texcoord.x * aspect * 1/scaleFactor + -gazeNormalized.x * aspect * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
            maskColor = SAMPLE_TEXTURE2D(_MaskTexFocus, sampler_MaskTexFocus, samplePoint);
        }

		return lerp(originalColor, maskColor, maskColor.a);
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