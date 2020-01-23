Shader "Unlit/ContrastReductionShader"{
	//show values to edit in inspector
	Properties{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		[HideInInspector]_MaskLeft ("Blur Mask Left Eye", 2D) = "white" {}
		[HideInInspector]_MaskRight ("Blur Mask Right Eye", 2D) = "white" {}
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
			sampler2D _MaskLeft;
			sampler2D _MaskRight;

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

				// init color variable
				float4 col = 0;
				half contrast = 0.1;

				// get color pixel in rendered image and mask texture
				half4 originalColor = tex2D(_MainTex, i.uv);
				half4 maskColorLeft = tex2D(_MaskLeft, i.uv);
				half4 maskColorRight = tex2D(_MaskRight, i.uv);

				return lerp(originalColor,lerp(half4(0.5,0.5,0.5,0), originalColor, contrast),maskColorLeft.a);

				// interpolate between original pixel color and blur color by factor of the mask pixel's alpha and apply to the respective eye
				if(unity_StereoEyeIndex == 0) {
					return lerp(originalColor,lerp(half4(0.5,0.5,0.5,0), originalColor, contrast),maskColorLeft.a);
				} else {
					return lerp(originalColor,lerp(half4(0.5,0.5,0.5,0), originalColor, contrast),maskColorRight.a);
				}
			}

			ENDCG
		}
	}
}