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
    VarjoLayer varjoLayer;

    void Start()
    {
        VarjoViewCamera varjoCamera = GetComponent<VarjoViewCamera>();
        Debug.Log(varjoCamera.CAMERA_ID);
        VarjoLayer varjoLayer = GetComponent<VarjoLayer>();
        //Debug.Log(this.varjoLayer.GetRenderTextureForCamera(varjoCamera.CAMERA_ID));
    }

    void Update()
    {
        //Postprocess(this.cam.targetTexture);
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

    void Postprocess(RenderTexture source)
    {


        // RenderTexture destination = this.cam.targetTexture;
        // RenderTexture temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        // Graphics.Blit(source, temporaryTexture, postprocessMaterial, 0);
        // Graphics.Blit(temporaryTexture, destination, 1);

        if(!saved){
            SaveTexture(source, this.cam.name + "Source");
            // SaveTexture(temporaryTexture, this.cam.name + "Temp");
            saved = true;
        }
        // RenderTexture.ReleaseTemporary(temporaryTexture);
        // this.cam.targetTexture = destination;
    }

    // method which is automatically called by unity after the camera is done rendering
    // void OnRenderImage(RenderTexture source, RenderTexture destination)
    // {
    //     // draws the pixels from the source texture to the destination texture
    //     Graphics.Blit(source, temporaryTexture, postprocessMaterial, 0);
    //     Graphics.Blit(temporaryTexture, destination, postprocessMaterial, 1);

    //     Camera cam = GetComponent<Camera>();


    // }

    public void SaveTexture(RenderTexture rt, string name)
    {
        Debug.Log("Saving frame from " + name);
        byte[] bytes = toTexture2D(rt).EncodeToPNG();
        System.IO.File.WriteAllBytes("C:/Users/Lukas Masopust/Documents/SavedScreen" + name +".png", bytes);
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        if(rTex != null)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }
        return null;
    }
}
