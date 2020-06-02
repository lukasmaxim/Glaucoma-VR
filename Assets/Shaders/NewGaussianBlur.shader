Shader "Custom/NewGaussianBlur"{
	//show values to edit in inspector
	Properties{
		// 	_MaskLeft ("Blur Mask Left Eye", 2D) = "white" {}
		// 	_MaskRight ("Blur Mask Right Eye", 2D) = "white" {}
		// 	_MaskColorLeft ("Mask Color Left Eye", Color) = (1,1,0,1)
		// 	_MaskColorRight ("Mask Color Right Eye", Color) = (0,1,1,1)
	}

	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

	// just a test so I don't go insanse
	float4 FragDefault(VaryingsDefault i) : SV_Target
	{
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        return color;
	};

	//normpdf function gives us a Guassian distribution for each blur iteration; 
	//this is equivalent of multiplying by hard #s 0.16,0.15,0.12,0.09, etc. in code above
	float normpdf(float x, float sigma)
	{
		return 0.39894*exp(-0.5*x*x / (sigma*sigma)) / sigma;
	};

	//this is the blur function... pass in standard col derived from tex2d(_MainTex,i.uv)
	half4 GaussianBlur(VaryingsDefault i) : SV_Target
	{
		float blurAmount = 100;
		//get our base color...
		half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.texcoord);
		//total width/height of our blur "grid":
		const int mSize = 10;
		//this gives the number of times we'll iterate our blur on each side 
		//(up,down,left,right) of our uv coordinate;
		//NOTE that this needs to be a const or you'll get errors about unrolling for loops
		const int iter = (mSize - 1) / 2;
		//run loops to do the equivalent of what's written out line by line above
		//(number of blur iterations can be easily sized up and down this way)
		for (int j = -iter; j <= iter; ++j) {
			for (int k = -iter; k <= iter; ++k) {
				col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.texcoord.x + j * blurAmount, i.texcoord.y + k * blurAmount)) * normpdf(float(j), 7);
			}
		}
		//return blurred color
		return col/mSize;
	};

	ENDHLSL

	SubShader{
		Cull Off ZWrite Off  ZTest Always

		Pass{

			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment GaussianBlur
			
			ENDHLSL
		}
	}
}