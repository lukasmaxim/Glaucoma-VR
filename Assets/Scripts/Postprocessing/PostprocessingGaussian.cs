using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostprocessingGaussian : MonoBehaviour
{
    // material that's applied when doing postprocessing
    [SerializeField]
    Material postprocessMaterial;
    [SerializeField]
    Texture2D placeHolderTextureLeftEye, placeHolderTextureRightEye;
    [SerializeField]
    bool saveTexture, useGeneratedMasks;
    bool created = false;

    // set eye textures to placeholder
    void Awake()
    {
        postprocessMaterial.SetTexture("_MaskLeft", placeHolderTextureLeftEye);
        postprocessMaterial.SetTexture("_MaskRight", placeHolderTextureRightEye);
    }

    // set eye textures to generated masks
    public void SetTextures(List<Texture2D> masks)
    {
        if (useGeneratedMasks)
        {
            postprocessMaterial.SetTexture("_MaskLeft", masks[0]);
            postprocessMaterial.SetTexture("_MaskRight", masks[1]);
        }
    }

    // method which is automatically called by unity after the camera is done rendering
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // draws the pixels from the source texture to the destination texture
        var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, temporaryTexture, postprocessMaterial, 0);
        Graphics.Blit(temporaryTexture, destination, postprocessMaterial, 1);

        if(!this.created){
            SaveTexture(source, "Source");
            SaveTexture(temporaryTexture, "Temp");
        }
        RenderTexture.ReleaseTemporary(temporaryTexture);


    }

    public void SaveTexture(RenderTexture rt, string name)
    {
        this.created = true;
        byte[] bytes = toTexture2D(rt).EncodeToPNG();
        System.IO.File.WriteAllBytes("C:/Users/Lukas Masopust/Documents/SavedScreen" + name + ".png", bytes);
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
