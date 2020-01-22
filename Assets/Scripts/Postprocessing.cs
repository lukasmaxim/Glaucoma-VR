﻿using UnityEngine;

[ExecuteInEditMode]
public class Postprocessing : MonoBehaviour
{
    // material that's applied when doing postprocessing
    [SerializeField]
    private Material postprocessMaterial;

	void Awake(){
		
	}

	// method which is automatically called by unity after the camera is done rendering
	void OnRenderImage(RenderTexture source, RenderTexture destination){
		// draws the pixels from the source texture to the destination texture
		var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, temporaryTexture, postprocessMaterial, 0);
		Graphics.Blit(temporaryTexture, destination, postprocessMaterial, 1);
		RenderTexture.ReleaseTemporary(temporaryTexture);
	}
}
