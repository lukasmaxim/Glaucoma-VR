Shader "Debug/MaskCoverage"{
	
	HLSLINCLUDE

    #include "../Commons.hlsl"

	float4 _OverlayColor;
	float _AlphaCutoff;
	float maskAlpha;

	// create binary mask of given mask with alpha cutoff
	float4 MaskCoverage(VaryingsDefault i) : SV_Target
	{
		originalColor = MainSamplePoint(i);
        maskColor = MaskGazeSamplePoint(i);
		maskAlpha = maskColor.a;

		// alpha cutoff so everything above the threshold is clamped to 1, which creates a binary mask
		if(maskAlpha > _AlphaCutoff)
		{
			maskAlpha = 1;
		}
		else
		{
			maskAlpha = 0;
		}

		return lerp(originalColor, _OverlayColor, maskAlpha);
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