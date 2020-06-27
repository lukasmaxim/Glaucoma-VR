Shader "Debug/FollowGaze"{

    HLSLINCLUDE

    #include "../Commons.hlsl"

    float distance;

    float circleRadius;
    float4 circleColor;

    // draw a circle where the gaze goes
    float4 FragDefault(VaryingsDefault i) : SV_Target
    {
        circleRadius = 0.01f * scaleFactor;
        circleColor = float4(1.0f, 0.2f, 0.0f, 0.0f);

        GazeToWorldCoords();
        gazeNormalized += 0.5f;

        // calculate the distance between i and the gaze
        distance = sqrt(pow(gazeNormalized.x * aspect - i.texcoord.x * aspect, 2) + pow(gazeNormalized.y - i.texcoord.y, 2));

        // if i is in the radius, fill with color
        if(distance < circleRadius)
        {
            return circleColor;
        }
        else
        {
            maskColor = MainSamplePoint(i);
            return maskColor;
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