using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MaskGenerator
{
    string inputPath = Application.dataPath + "/Textures/";
    int[,] input = new int[10, 10];
    int[,] invertedInput = new int[10, 10];
    int highestValue = 33;
    Texture2D blurMask;
    int dimension = 512;

    public Texture2D Generate(string fileName)
    {
        ReadFile(fileName);
        InvertValues();
        GenerateTexture();
        ResizeTexture();
        return blurMask;
    }

    // reads a .txt file with csv data
    void ReadFile(string fileName)
    {
        StreamReader file = new StreamReader(inputPath + fileName);
        int i = 0;
        String line = String.Empty;

        while((line = file.ReadLine()) != null)
        {
            String[] values = line.Split(',');
            for(int j = 0; j < values.Length; j++){
                values[j] = values[j].Trim();
                input[j,i] = int.Parse(values[j]);
            }
            i++;
        }
        blurMask = new Texture2D(input.GetLength(0), input.GetLength(1));
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

    // creates a texture from the inverted input and maps the values to the alpha channel
    void GenerateTexture()
    {
        for (int i = 0; i < blurMask.width; i++)
        {
            for (int j = 0; j < blurMask.height; j++)
            {
                float alpha = (float) invertedInput[i, j] / highestValue;
                Color color = new Color(1, 1, 1, alpha);
                blurMask.SetPixel(i, blurMask.height - j - 1, color);
            }
        }
    }

    // scales the image to specified dimensions
    void ResizeTexture()
    {
        TextureScale.Bilinear(blurMask, dimension, dimension);
    }

    // OBSOLETE ?
    void SaveTexture()
    {
        string path = Application.dataPath + "/Textures/";
        byte[] bytes = blurMask.EncodeToPNG();
        File.WriteAllBytes(path + "Mask.png", bytes);
    }
}
