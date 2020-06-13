using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo;

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
    bool saved = false;
    VarjoViewCamera.CAMERA_ID varjoCameraId;

    void Start()
    {
        this.varjoCameraId = GetComponent<VarjoViewCamera>().CameraId;

        Camera cam = GetComponent<VarjoViewCamera>().GetComponent<Camera>();
        //cam.SetReplacementShader(this.postprocessMaterial.shader, null);
        //cam.RenderWithShader(this.postprocessMaterial.shader, null);

    }

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

    // //method which is automatically called by unity after the camera is done rendering
    // void OnRenderImage(RenderTexture source, RenderTexture destination)
    // {
    //     // draws the pixels from the source texture to the destination texture  
    //     RenderTexture temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
    //     Graphics.Blit(source, temporaryTexture, postprocessMaterial, 0);
    //     Graphics.Blit(temporaryTexture, destination, postprocessMaterial, 1);

    //     // save for debug
    //     if(!saved)
    //     {
    //         SaveTexture(source, "_SOURCE");
    //         SaveTexture(temporaryTexture, "_TEMP");
    //         saved = true;
    //     }
    // }

    public void SaveTexture(RenderTexture rt, string targetName)
    {
        string name = this.varjoCameraId + targetName;
        Debug.Log("Saving frame from " + name);
        byte[] bytes = toTexture2D(rt).EncodeToPNG();
        System.IO.File.WriteAllBytes("C:/Users/Lukas/Documents/SavedScreen" + name +".png", bytes);
    }

    Texture2D toTexture2D(RenderTexture rt)
    {
        if(rt != null)
        {
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            return tex;
        }
        return null;
    }
}
