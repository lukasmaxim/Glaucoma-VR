Shader "Unlit/MaskCoverageShader"{
	//show values to edit in inspector
	Properties{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_MaskLeft ("Blur Mask Left Eye", 2D) = "white" {}
		_MaskRight ("Blur Mask Right Eye", 2D) = "white" {}
		_MaskColorLeft ("Mask Color Left Eye", Color) = (1,1,0,1)
		_MaskColorRight ("Mask Color Right Eye", Color) = (0,1,1,1)
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

			#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH
			#pragma shader_feature GAUSS

			// define vertex and fragment shader
			#pragma vertex vert
			#pragma fragment frag

			// texture and transforms of the texture
			sampler2D _MainTex;
			sampler2D _MaskLeft;
			sampler2D _MaskRight;
			fixed4 _MaskColorLeft;
			fixed4 _MaskColorRight;

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

				half4 originalColor = tex2D(_MainTex, i.uv);

				// check for left / right eye rendering and lerp between original rendered texture and mask color with an alpha of the respective left / right texture
				if(unity_StereoEyeIndex == 0) {
					half4 maskValue = tex2D(_MaskLeft, i.uv);
					return lerp(originalColor, _MaskColorLeft, maskValue.a);
				} else {
					half4 maskValue = tex2D(_MaskRight, i.uv);
					return lerp(originalColor, _MaskColorRight, maskValue.a);
				}
			}

			ENDCG
		}
	}
}