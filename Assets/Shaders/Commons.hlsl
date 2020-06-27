#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
TEXTURE2D_SAMPLER2D(_MaskTexLeftContext, sampler_MaskTexLeftContext);
TEXTURE2D_SAMPLER2D(_MaskTexLeftFocus, sampler_MaskTexLeftFocus);
TEXTURE2D_SAMPLER2D(_MaskTexRightContext, sampler_MaskTexRightContext);
TEXTURE2D_SAMPLER2D(_MaskTexRightFocus, sampler_MaskTexRightFocus);

float3 gaze;
float4 gazeProjected;
float2 gazeNormalized;

float4 originalColor;
float4 maskColor;

int eye;
int screen;
float2 offset;
float scaleFactor;
float aspect;
float2 samplePoint;

// converts gaze to world coords
void GazeToWorldCoords()
{
	// gaze is in object coords; first turn into world coords, then use the view projection matrix (VP) to get clip coords;
	// normally we could do this with MVP, but MVP is no longer :(
	//
	// turn object coords into clip coords by multiplying unity_ObjectToWorld (for getting world coords) and unity_MatrixVP (for getting eye coords and then clip coords)
	// helpful image: http://blog.hvidtfeldts.net/media/opengl.png
    gazeProjected = mul(mul(unity_ObjectToWorld, unity_MatrixVP), float4(gaze, 1.0f)); // 4th dim has to be 1.0
    gazeNormalized = (gazeProjected.xy / gazeProjected.w) * float2(0.5f, -0.5f); // multiplication and addition is for transforming -1...1 to 0...1 in directx and metal
}

// sample mask tex gaze point
float4 MaskGazeSamplePoint(VaryingsDefault i)
{
    GazeToWorldCoords();

	if(screen == -1) // context eye
	{
		if(eye == -1) // left
		{
			samplePoint = float2(i.texcoord.x * 1/scaleFactor + -gazeNormalized.x * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
			maskColor = SAMPLE_TEXTURE2D(_MaskTexLeftContext, sampler_MaskTexLeftContext, samplePoint);
		}
		else // right
		{
			samplePoint = float2(i.texcoord.x * 1/scaleFactor + -gazeNormalized.x * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
			maskColor = SAMPLE_TEXTURE2D(_MaskTexRightContext, sampler_MaskTexRightContext, samplePoint);
		}
	}
	else // focus
	{
		if(eye == -1) // left
		{
			samplePoint = float2(i.texcoord.x * aspect * 1/scaleFactor + -gazeNormalized.x * aspect * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
			maskColor = SAMPLE_TEXTURE2D(_MaskTexLeftFocus, sampler_MaskTexLeftFocus, samplePoint);
		}
		else // right
		{
			samplePoint = float2(i.texcoord.x * aspect * 1/scaleFactor + -gazeNormalized.x * aspect * 1/scaleFactor + offset.x, i.texcoord.y * 1/scaleFactor  + -gazeNormalized.y * 1/scaleFactor + offset.y);
			maskColor = SAMPLE_TEXTURE2D(_MaskTexRightFocus, sampler_MaskTexRightFocus, samplePoint);
		}
	}
    return maskColor;
}

// sample main tex point
float4 MainSamplePoint(VaryingsDefault i)
{
	return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
}
