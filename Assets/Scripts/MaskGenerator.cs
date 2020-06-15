using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MaskGenerator
{
    string inputPath = Application.dataPath + "/PatientData/";
    int[,] input = new int[10, 10];
    int[,] invertedInput = new int[10, 10];
    int highestValue = 33;
    Texture2D blurMaskContext, blurMaskFocus;
    List<Texture2D> Masks = new List<Texture2D>();
    Tuple<int, int> contextDimensions = new Tuple<int, int>(1600, 1600);
    Tuple<int, int> focusDimensions = new Tuple<int, int>(2048, 2048);
    string filePath = null;

    public List<Texture2D> Generate(string filePath, bool save)
    {
        Debug.Log("Generating masks.");
        this.filePath = filePath;
        ReadFile();
        InvertValues();
        GenerateTexture();
        ResizeTexture();
        if(save) {
            SaveTexture();
        }
        return Masks;
    }

    // reads a .txt file with csv data
    void ReadFile()
    {
        StreamReader file = new StreamReader(inputPath + this.filePath);
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
        Masks.Add(new Texture2D(input.GetLength(0), input.GetLength(1)));
        Masks.Add(new Texture2D(input.GetLength(0), input.GetLength(1)));
    }

    // TODO this should be log()
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
        for (int i = 0; i < Masks[0].width; i++)
        {
            for (int j = 0; j < Masks[0].height; j++)
            {
                float alpha = (float) invertedInput[i, j] / highestValue;
                Color color = new Color(1, 1, 1, alpha);
                Masks[0].SetPixel(i, Masks[0].height - j - 1, color);
                Masks[1].SetPixel(i, Masks[1].height - j - 1, color);
            }
        }
    }

    // scales the masks to specified dimensions
    void ResizeTexture()
    {
        TextureScale.Bilinear(Masks[0], contextDimensions.Item1, contextDimensions.Item2);
        TextureScale.Bilinear(Masks[1], contextDimensions.Item1, contextDimensions.Item2);
    }

    // debug texture save
    void SaveTexture()
    {
        foreach(Texture2D mask in Masks)
        {
            string[] fileName = this.filePath.Split('/');
            string path = Application.dataPath + "/Textures/";
            byte[] bytes = mask.EncodeToPNG();

            try {
                string screen = Masks.IndexOf(mask) == 0 ? "context" : "focus";
                File.WriteAllBytes(path + fileName[1] + "_" + screen + ".png", bytes);
                Debug.Log("Successfully saved mask to png.");
            } catch(Exception e) {
                Debug.LogError("Failed saving mask to png. " + e);
            }
        }
    }
}
