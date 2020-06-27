Shader "Debug/MoveTexture"{

    Properties
	{
		_MaskTexLeftContext ("Mask Texture Context", 2D) = "white" {}
		_MaskTexLeftFocus ("Mask Texture Focus", 2D) = "white" {}
		_MaskTexRightContext ("Mask Texture Context", 2D) = "white" {}
		_MaskTexRightFocus ("Mask Texture Focus", 2D) = "white" {}
	}

    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    #include "../Commons.hlsl"

    // move debug textures for alignment with gaze
    float4 Frag(VaryingsDefault i) : SV_Target
    {    
        originalColor = MainSamplePoint(i);
        maskColor = MaskGazeSamplePoint(i);

		return lerp(originalColor, maskColor, maskColor.a);
    }

    ENDHLSL

    SubShader{
        Cull Off ZWrite Off  ZTest Always

        Pass{

            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag
            
            ENDHLSL
        }
    }
}