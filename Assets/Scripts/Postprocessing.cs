using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class Postprocessing : MonoBehaviour
{
    // material that's applied when doing postprocessing
    [SerializeField]
    private Material postprocessMaterial;

    void Awake()
    {
        MaskGenerator maskGenerator = new MaskGenerator();
        maskGenerator.Generate();
		Texture2D texture = LoadPNG(Application.dataPath + "/Textures/mask.png");		
        postprocessMaterial.SetTexture("_MaskLeft", texture);
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

    Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
