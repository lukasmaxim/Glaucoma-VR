using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class Postprocessing : MonoBehaviour
{
    // material that's applied when doing postprocessing
    [SerializeField]
    Material postprocessMaterial;
	[SerializeField]
	string leftEyeFileName, rightEyeFileName;

	// set left and right eye texture on awakening
    void Awake()
    {
        MaskGenerator maskGenerator = new MaskGenerator();
        Texture2D textureLeftEye = maskGenerator.Generate(leftEyeFileName);
        Texture2D textureRightEye = maskGenerator.Generate(rightEyeFileName);
        postprocessMaterial.SetTexture("_MaskLeft", textureLeftEye);
        postprocessMaterial.SetTexture("_MaskRight", textureRightEye);
    }

    // method which is automatically called by unity after the camera is done rendering
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // draws the pixels from the source texture to the destination texture
        var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, temporaryTexture, postprocessMaterial, 0);
        Graphics.Blit(temporaryTexture, destination, postprocessMaterial, 1);
        RenderTexture.ReleaseTemporary(temporaryTexture);
    }
}
