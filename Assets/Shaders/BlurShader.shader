﻿Shader "Unlit/BlurShader"{
	//show values to edit in inspector
	Properties{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_MaskLeft ("Blur Mask Left Eye", 2D) = "white" {}
		_MaskRight ("Blur Mask Right Eye", 2D) = "white" {}
		_BlurSize("Blur Size", Range(0,0.5)) = 0
		[KeywordEnum(Low, Medium, High, Very High)] _Samples ("Sample amount", Float) = 0
		[Toggle(GAUSS)] _Gauss ("Gaussian Blur", float) = 0
		[PowerSlider(3)]_StandardDeviation("Standard Deviation (Gauss only)", Range(0.00, 0.3)) = 0.02
	}

	SubShader{
		// markers that specify that we don't need culling 
		// or reading/writing to the depth buffer
		Cull Off
		ZWrite Off 
		ZTest Always

		// Vertical Blur
		Pass{
			CGPROGRAM
			// include useful shader functions
			#include "UnityCG.cginc"

			// define vertex and fragment shader
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH _SAMPLES_VERY_HIGH
			#pragma shader_feature GAUSS

			// texture and transforms of the texture
			sampler2D _MainTex;
			sampler2D _MaskLeft;
			sampler2D _MaskRight;
			float _BlurSize;
			float _StandardDeviation;

			#define PI 3.14159265359
			#define E 2.71828182846

			#if _SAMPLES_LOW
				#define SAMPLES 10
			#elif _SAMPLES_MEDIUM
				#define SAMPLES 30
			#elif _SAMPLES_HIGH
				#define SAMPLES 100
			#else
				#define SAMPLES 200
			#endif

			// the object data that's put into the vertex shader
			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			// the data that's used to generate fragments and can be read by the fragment shader
			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// the vertex shader
			v2f vert(appdata v){
				v2f o;
				// convert the vertex positions from object space to clip space so they can be rendered
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			// the fragment shader
			fixed4 frag(v2f i) : SV_TARGET{

				// POSSIBLE OPTIMIZATION: abort if pixel in mask is black or close to
				// abort if pixel is totally black in mask
				// if(tex2D(_Mask, i.uv).a == 0)
				//	return tex2D(_MainTex, i.uv);

				#if GAUSS
					// failsafe so we can use turn off the blur by setting the deviation to 0
					if(_StandardDeviation == 0)
					return tex2D(_MainTex, i.uv);
				#endif
				// init color variable
				float4 col = 0;
				#if GAUSS
					float sum = 0;
				#else
					float sum = SAMPLES;
				#endif
				// iterate over blur samples
				for(float index = 0; index < SAMPLES; index++){
					// get the offset of the sample
					float offset = (index/(SAMPLES-1) - 0.5) * _BlurSize;
					// get uv coordinate of sample
					float2 uv = i.uv + float2(0, offset);
					#if !GAUSS
						// simply add the color if we don't have a gaussian blur (box)
						col += tex2D(_MainTex, uv);
					#else
						// calculate the result of the gaussian function
						float stDevSquared = _StandardDeviation*_StandardDeviation;
						float gauss = (1 / sqrt(2*PI*stDevSquared)) * pow(E, -((offset*offset)/(2*stDevSquared)));
						// add result to sum
						sum += gauss;
						// multiply color with influence from gaussian function and add it to sum color
						col += tex2D(_MainTex, uv) * gauss;
					#endif
				}

				// get color pixel in rendered image and mask texture
				half4 originalColor = tex2D(_MainTex, i.uv);
				half4 maskColorLeft = tex2D(_MaskLeft, i.uv);
				half4 maskColorRight = tex2D(_MaskRight, i.uv);

				// divide the sum of values by the amount of samples
				col = col / sum;

				// interpolate between original pixel color and blur color by factor of the mask pixel's alpha and apply to the respective eye
				if(unity_StereoEyeIndex == 0) {
					return lerp(originalColor, col, maskColorLeft.a);
				} else {
					return lerp(originalColor, col, maskColorRight.a);
				}
			}

			ENDCG
		}

		// Horizontal Blur
		Pass{
			CGPROGRAM
			// include useful shader functions
			#include "UnityCG.cginc"

			#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH _SAMPLES_VERY_HIGH
			#pragma shader_feature GAUSS

			// define vertex and fragment shader
			#pragma vertex vert
			#pragma fragment frag

			// texture and transforms of the texture
			sampler2D _MainTex;
			sampler2D _MaskLeft;
			sampler2D _MaskRight;
			float _BlurSize;
			float _StandardDeviation;

			#define PI 3.14159265359
			#define E 2.71828182846

			#if _SAMPLES_LOW
				#define SAMPLES 10
			#elif _SAMPLES_MEDIUM
				#define SAMPLES 30
			#elif _SAMPLES_HIGH
				#define SAMPLES 100
			#else
				#define SAMPLES 200
			#endif

			// the object data that's put into the vertex shader
			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			// the data that's used to generate fragments and can be read by the fragment shader
			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// the vertex shader
			v2f vert(appdata v){
				v2f o;
				// convert the vertex positions from object space to clip space so they can be rendered
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			// the fragment shader
			fixed4 frag(v2f i) : SV_TARGET{

				// POSSIBLE OPTIMIZATION: abort if pixel in mask is black or close to
				// abort if pixel is totally black in mask
				// if(tex2D(_Mask, i.uv).a == 0)
				//	return tex2D(_MainTex, i.uv);

				#if GAUSS
					// failsafe so we can use turn off the blur by setting the deviation to 0
					if(_StandardDeviation == 0)
					return tex2D(_MainTex, i.uv);
				#endif
				// calculate aspect ratio
				float invAspect = _ScreenParams.y / _ScreenParams.x;
				// init color variable
				float4 col = 0;
				#if GAUSS
					float sum = 0;
				#else
					float sum = SAMPLES;
				#endif
				// iterate over blur samples
				for(float index = 0; index < SAMPLES; index++){
					// get the offset of the sample
					float offset = (index/(SAMPLES-1) - 0.5) * _BlurSize * invAspect;
					// get uv coordinate of sample
					float2 uv = i.uv + float2(offset, 0);
					#if !GAUSS
						// simply add the color if we don't have a gaussian blur (box)
						col += tex2D(_MainTex, uv);
					#else
						// calculate the result of the gaussian function
						float stDevSquared = _StandardDeviation*_StandardDeviation;
						float gauss = (1 / sqrt(2*PI*stDevSquared)) * pow(E, -((offset*offset)/(2*stDevSquared)));
						// add result to sum
						sum += gauss;
						// multiply color with influence from gaussian function and add it to sum color
						col += tex2D(_MainTex, uv) * gauss;
					#endif
				}

				// get color pixel in rendered image and mask texture
				half4 originalColor = tex2D(_MainTex, i.uv);
				half4 maskColorLeft = tex2D(_MaskLeft, i.uv);
				half4 maskColorRight = tex2D(_MaskRight, i.uv);

				// divide the sum of values by the amount of samples
				col = col / sum;

				// interpolate between original pixel color and blur color by factor of the mask pixel's alpha and apply to the respective eye
				if(unity_StereoEyeIndex == 0) {
					return lerp(originalColor, col, maskColorLeft.a);
				} else {
					return lerp(originalColor, col, maskColorRight.a);
				}
			}

			ENDCG
		}
	}
}