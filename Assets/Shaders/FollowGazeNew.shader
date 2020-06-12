Shader "Custom/FollowGazeNew"{
    //show values to edit in inspector
    Properties
    {
        _MaskTexture ("Mask Texture", 2D) = "white" {}
        _MaskColor ("Mask Color", Color) = (0,1,1,1)
    }

    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_MaskTexture, sampler_MaskTexture);
    float4 _MaskColor;
    float4 _MainTex_TexelSize;

    // LETS DO THIS IN C#
    // float4 gazeDirection;
    // float4 normalizedGazeDireciton = normalize(gazeDirection);
    // float4 worldGazeDirection = mul(unity_ObjectToWorld, normalizedGazeDireciton);
    
    float2 gaze;

    // debug so I don't go insane
    float4 MaskCoverage(VaryingsDefault i) : SV_Target
    {
        half4 originalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        half4 maskValue = SAMPLE_TEXTURE2D(_MaskTexture, sampler_MaskTexture, i.texcoord);

        return lerp(originalColor, _MaskColor, maskValue);
    }

    // draw a circle where the gaze goes
    float4 FragDefault(VaryingsDefault i) : SV_Target
    {
        float aspect = _MainTex_TexelSize.w / _MainTex_TexelSize.z;

        // distance to gaze
        float d = sqrt(pow(gaze.x - i.texcoord.x, 2) + pow(gaze.y * aspect - i.texcoord.y * aspect, 2));

        if(d < 0.01f)
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