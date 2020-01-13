using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ArrayToTexture : MonoBehaviour
{
    int[,] input = new int[10, 10]{
        {30,30,30,20,17,21,18,30,30,30},
        {30,30,1,0,17,15,21,21,30,30},
        {30,0,0,0,0,0,20,24,28,30},
        {0,0,0,12,29,30,27,20,29,30},
        {0,5,13,28,30,19,12,25,25,29},
        {4,21,22,21,28,32,29,0,29,31},
        {21,26,24,21,24,29,31,30,30,31},
        {30,24,25,27,23,29,33,32,31,30},
        {30,30,20,25,25,27,28,30,30,30},
        {30,30,30,29,27,27,28,30,30,30}
        };
    int[,] invertedInput = new int[10, 10];
    int highestValue = 33;
    Texture2D blurMask;
    [SerializeField] Material postprocessingMaterial;

    void Start()
    {
        blurMask = new Texture2D(10, 10);
        InvertValues();
        CreateTexture();
        SaveTexture();
    }

    // inverts the value
    void InvertValues()
    {
        for (int i = 0; i < invertedInput.GetLength(0); i++)
        {
            for (int j = 0; j < invertedInput.GetLength(1); j++)
            {
                invertedInput[i, j] = highestValue - input[i, j];
            }
        }
    }

    // creates a texture from the inverted input and maps the values on a scale from black (good vision) to white (bad vision)
    void CreateTexture()
    {
        for (int i = 0; i < blurMask.width; i++)
        {
            for (int j = 0; j < blurMask.height; j++)
            {
                float scaledColorChannelValue = (float) invertedInput[i, j] / highestValue;
                print(scaledColorChannelValue);
                Color color = new Color(scaledColorChannelValue, scaledColorChannelValue, scaledColorChannelValue, 1);
                blurMask.SetPixel(i, j, color);
            }
        }
    }

    void SaveTexture()
    {
        byte[] bytes = blurMask.EncodeToPNG();
        File.WriteAllBytes("/Users/lukasmasopust/Desktop/" + "Mask.png", bytes);
    }
}
