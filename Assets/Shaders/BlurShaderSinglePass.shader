Shader "Unlit/BlurShaderSinglePass"{
	//show values to edit in inspector
	Properties{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_Mask ("Blur Mask", 2D) = "white" {}
		_BlurSize("Blur Size", Range(0,0.5)) = 0
		[KeywordEnum(Low, Medium, High)] _Samples ("Sample amount", Float) = 0
		[Toggle(GAUSS)] _Gauss ("Gaussian Blur", float) = 0
		[PowerSlider(3)]_StandardDeviation("Standard Deviation (Gauss only)", Range(0.00, 0.3)) = 0.02
	}

	SubShader{
		// markers that specify that we don't need culling 
		// or reading/writing to the depth buffer
		Cull Off
		ZWrite Off 
		ZTest Always

		// Blur
		Pass{
			CGPROGRAM
			// include useful shader functions
			#include "UnityCG.cginc"

			// define vertex and fragment shader
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH
			#pragma shader_feature GAUSS

			// texture and transforms of the texture
			sampler2D _MainTex;
			sampler2D _Mask;
			float _BlurSize;
			float _StandardDeviation;

			#define PI 3.14159265359
			#define E 2.71828182846

			#if _SAMPLES_LOW
				#define SAMPLES 10
			#elif _SAMPLES_MEDIUM
				#define SAMPLES 30
			#else
				#define SAMPLES 100
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

				// abort if pixel is totally black in mask
				if(tex2D(_Mask, i.uv).a == 0)
					return tex2D(_MainTex, i.uv);

				#if GAUSS
					// failsafe so we can use turn off the blur by setting the deviation to 0
					if(_StandardDeviation == 0)
					return tex2D(_MainTex, i.uv);
				#endif
				// calculate aspect ratio		
				float invAspect = _ScreenParams.y / _ScreenParams.x;
				// init color variable
				float4 colVert = 0;
				float4 colHor = 0;
				float4 col = 0;
				#if GAUSS
					float sumVert = 0;
					float sumHor = 0;
				#else
					float sumVert = SAMPLES;
					float sumHor = SAMPLES;
				#endif
				// iterate over blur samples
				for(float index = 0; index < SAMPLES; index++){
					// get the offset of the sample
					float offsetVertical = (index/(SAMPLES-1) - 0.5) * _BlurSize;
					float offsetHorizontal = (index/(SAMPLES-1) - 0.5) * _BlurSize * invAspect;
					// get uv coordinate of sample
					float2 uvVertical = i.uv + float2(0, offsetVertical);
					float2 uvHorizontal = i.uv + float2(offsetHorizontal, 0);
					#if !GAUSS
						// simply add the color if we don't have a gaussian blur (box)
						colVert += tex2D(_MainTex, uvVertical);
						colHor += tex2D(_MainTex, uvHorizontal);
					#else
						// calculate the result of the gaussian function
						float stDevSquared = _StandardDeviation*_StandardDeviation;
						float gaussVert = (1 / sqrt(2*PI*stDevSquared)) * pow(E, -((offsetVertical*offsetVertical)/(2*stDevSquared)));
						float gaussHor = (1 / sqrt(2*PI*stDevSquared)) * pow(E, -((offsetHorizontal*offsetHorizontal)/(2*stDevSquared)));
						// add result to sum
						sumVert += gaussVert;
						sumHor += gaussHor;
						// multiply color with influence from gaussian function and add it to sum color
						colVert += tex2D(_MainTex, uvVertical) * gaussVert;
						colHor += tex2D(_MainTex, uvHorizontal) * gaussHor;
					#endif
				}

				// get color pixel in rendered image and mask texture
				half4 originalColor = tex2D(_MainTex, i.uv);
				half4 maskColor = tex2D(_Mask, i.uv);

				// divide the sum of values by the amount of samples
				colVert = colVert / sumVert;
				colHor = colHor / sumHor;
				col = (colVert + colHor) / 2;

				// interpolate between original pixel color and blur color by factor of the mask pixel's alpha
				return lerp(originalColor, col, maskColor.a);
			}

			ENDCG
		}
	}
}