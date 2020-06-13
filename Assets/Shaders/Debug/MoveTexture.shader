Shader "Custom/MoveTexture"{

    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float4 _MainTex_TexelSize;

    float3 gaze;
    float4 gazeProjected;
    float2 gazeNormalized;

    float distance;
    float aspect;

    float circleRadius;
    float4 circleColor;

    // draw a circle where the gaze goes
    float4 FragDefault(VaryingsDefault i) : SV_Target
    {
        circleRadius = 0.01f;
        circleColor = float4(1.0f, 0.2f, 0.0f, 0.0f);

        // gaze is in object coords; first turn into world coords, then use the view projection matrix (VP) to get clip coords;
        // normally we could do this with MVP, but MVP is no longer :(
        //
        // turn object coords into clip coords by multiplying unity_ObjectToWorld (for getting world coords) and unity_MatrixVP (for getting eye coords and then clip coords)
        // helpful image: http://blog.hvidtfeldts.net/media/opengl.png
        gazeProjected = mul(mul(unity_ObjectToWorld, unity_MatrixVP), float4(gaze, 1.0f)); // 4th dim has to be 1.0
        gazeNormalized = (gazeProjected.xy / gazeProjected.w) * float2(0.5f, -0.5f) + float2(0.5f, 0.5f); // multiplication and addition is for transforming -1...1 to 0...1 in directx and metal

        // account for aspect ratio
        aspect = _MainTex_TexelSize.w / _MainTex_TexelSize.z;

        // calculate the distance between i and the gaze
        distance = sqrt(pow(gazeNormalized.x - i.texcoord.x, 2) + pow(gazeNormalized.y * aspect - i.texcoord.y * aspect, 2));

        // if i is in the radius, fill with color
        if(distance < circleRadius)
        {
            return circleColor;
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