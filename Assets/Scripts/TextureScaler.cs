using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScaler : MonoBehaviour
{
    [SerializeField]
    ComputeShader bicubicShader;
    [SerializeField]
    ComputeShader testShader;
    [SerializeField]
    Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        ScaleTexture();
        //TestComputeShader();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ScaleTexture()
    {
        int kernelHandle = bicubicShader.FindKernel("CSMain");
        RenderTexture rTex = new RenderTexture(texture.width, texture.height, 32, RenderTextureFormat.Default);

        rTex.enableRandomWrite = true;
        rTex.Create();

        bicubicShader.SetTexture(kernelHandle, "Result", rTex);
        bicubicShader.SetTexture(kernelHandle, "InputTexture", texture);
        bicubicShader.Dispatch(kernelHandle, 1024, 1024, 32);

        Save(rTex);
    }

    void TestComputeShader()
    {
        int kernelHandle = testShader.FindKernel("CSMain");
        RenderTexture rTex = new RenderTexture(256,256,24);
        rTex.enableRandomWrite = true;
        rTex.Create();

        testShader.SetTexture(kernelHandle, "Result", rTex);
        testShader.Dispatch(kernelHandle, 1024/8, 256/8, 1);

        Save(rTex);
    }

    Texture2D RenderTextureToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    void Save(RenderTexture rTex)
    {
        byte[] bytes = RenderTextureToTexture2D(rTex).EncodeToPNG();
        System.IO.File.WriteAllBytes("C:/Users/Lukas/SavedScreen.png", bytes);
    }
}
