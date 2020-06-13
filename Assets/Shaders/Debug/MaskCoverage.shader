Shader "Debug/MaskCoverage"{
	
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

	float4 MaskCoverage(VaryingsDefault i) : SV_Target
	{
		half4 originalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		half4 maskValue = SAMPLE_TEXTURE2D(_MaskTexture, sampler_MaskTexture, i.texcoord);

		return lerp(originalColor, _MaskColor, maskValue);
	}

	ENDHLSL

	SubShader{
		Cull Off ZWrite Off  ZTest Always

		Pass{

			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment MaskCoverage
			
			ENDHLSL
		}
	}
}