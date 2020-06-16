using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MaskGenerator
{
    string inputPath = Application.dataPath + "/PatientData/";
    // TODO replace with ArrayLists for dynamic resizing if input is not 10x10
    int[,] input, invertedInput;
    int highestValue = 33;
    Texture2D blurMaskContext, blurMaskFocus;
    List<Texture2D> masks;
    Tuple<int, int> contextDimensions = new Tuple<int, int>(1600, 1600);
    Tuple<int, int> focusDimensions = new Tuple<int, int>(2048, 2048);
    string filePath = null;
    bool save = false;

    public List<Texture2D> Generate(string filePath, bool save)
    {
        Debug.Log("Generating masks.");
        this.save = save;
        this.filePath = filePath;
        ReadFile();
        InvertValues();
        GenerateMasks();
        ResizeMasks();
        SaveMasks();
        return masks;
    }

    // reads a .txt file with csv data
    void ReadFile()
    {
        // TODO replace with ArrayLists for dynamic resizing if input is not 10x10
        input = new int[10, 10];
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
    }

    // TODO this should be log()
    // inverts each value of the input file by subtracting with the highest value
    void InvertValues()
    {
        // TODO replace with ArrayLists for dynamic resizing if input is not 10x10
        invertedInput = new int[10, 10];

        for (int i = 0; i < invertedInput.GetLength(0); i++)
        {
            for (int j = 0; j < invertedInput.GetLength(1); j++)
            {
                invertedInput[i, j] = highestValue - input[i, j];
            }
        }
    }

    // creates 2 masks from the inverted input and maps the values to the alpha channel
    void GenerateMasks()
    {
        // create 2 dummy textures
        masks = new List<Texture2D>();
        Texture2D maskContext = new Texture2D(invertedInput.GetLength(0), invertedInput.GetLength(1));
        masks.Add(new Texture2D(invertedInput.GetLength(0), invertedInput.GetLength(1)));
        masks.Add(new Texture2D(invertedInput.GetLength(0), invertedInput.GetLength(1)));

        // fill textures with alpha values
        foreach (Texture2D mask in masks)
        {
            Texture2D currentMask = masks[masks.IndexOf(mask)];
            for (int i = 0; i < currentMask.width; i++)
            {
                for (int j = 0; j < currentMask.height; j++)
                {
                    float alpha = (float) invertedInput[i, j] / highestValue;
                    Color color = new Color(1, 1, 1, alpha);
                    currentMask.SetPixel(i, currentMask.height - j - 1, color);
                }
            }
        }
    }

    // scales 2 masks to specified dimensions
    void ResizeMasks()
    {
        TextureScale.Bilinear(masks[0], contextDimensions.Item1, contextDimensions.Item2);
        TextureScale.Bilinear(masks[1], focusDimensions.Item1, focusDimensions.Item2);
    }

    // debug mask save
    void SaveMasks()
    {
        if(save)
        {
            foreach(Texture2D mask in masks)
            {
                string[] fileName = this.filePath.Split('/');
                string path = Application.dataPath + "/Textures/";
                byte[] bytes = mask.EncodeToPNG();

                try {
                    string screen = masks.IndexOf(mask) == 0 ? "context" : "focus";
                    File.WriteAllBytes(path + fileName[1] + "_" + screen + ".png", bytes);
                    Debug.Log("Successfully saved mask to png.");
                } catch(Exception e) {
                    Debug.LogError("Failed saving mask to png. " + e);
                }
            }
        }
    }
}
