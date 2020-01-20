using UnityEngine;

[ExecuteInEditMode]
public class PostprocessingBox : MonoBehaviour
{
    // material that's applied when doing postprocessing
    [SerializeField]
    private Material postprocessMaterial;

	// method which is automatically called by unity after the camera is done rendering
	void OnRenderImage(RenderTexture source, RenderTexture destination){
		// draws the pixels from the source texture to the destination texture
		var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, destination, postprocessMaterial, 0);
	}
}
