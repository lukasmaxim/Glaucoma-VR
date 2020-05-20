using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class PostprocessingContrastReduction : MonoBehaviour
{
    // material that's applied when doing postprocessing
    [SerializeField]
    Material postprocessMaterial;
	[SerializeField]
	string leftEyeFileName, rightEyeFileName;
    [SerializeField]
    Texture2D placeHolderTextureLeftEye, placeHolderTextureRightEye;
    Texture2D textureLeftEye, textureRightEye;
    [SerializeField]
    bool saveTexture;

	// // set left and right eye texture on awakening
    // void Awake()
    // {
    //     if(leftEyeFileName != null && rightEyeFileName != null){
    //         Debug.Log("Filenames set, generating masks.");
    //         MaskGenerator maskGenerator = new MaskGenerator();
    //         Texture2D textureLeftEye = maskGenerator.Generate(leftEyeFileName, saveTexture);
    //         Texture2D textureRightEye = maskGenerator.Generate(rightEyeFileName, saveTexture);
    //         if(textureLeftEye != null && textureRightEye != null){
    //             Debug.Log("Mask generation successful, using masks.");
    //             postprocessMaterial.SetTexture("_MaskLeft", textureLeftEye);
    //             postprocessMaterial.SetTexture("_MaskRight", textureRightEye);
    //         } else {
    //             Debug.Log("Mask generation failed, using placeholders.");
    //             postprocessMaterial.SetTexture("_MaskLeft", placeHolderTextureLeftEye);
    //             postprocessMaterial.SetTexture("_MaskRight", placeHolderTextureRightEye);
    //         }
    //     } else {
    //         Debug.Log("Filenames not set, using placeholders.");
    //         postprocessMaterial.SetTexture("_MaskLeft", placeHolderTextureLeftEye);
    //         postprocessMaterial.SetTexture("_MaskRight", placeHolderTextureRightEye);
    //     }
    // }

    // // method which is automatically called by unity after the camera is done rendering
    // void OnRenderImage(RenderTexture source, RenderTexture destination)
    // {
    //     // draws the pixels from the source texture to the destination texture
    //     Graphics.Blit(source, destination, postprocessMaterial, 0);
    // }
}
