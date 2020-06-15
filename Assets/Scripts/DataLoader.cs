using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class MaskReadyEvent : UnityEvent<List<Texture2D>>
{
}

public class DataLoader : MonoBehaviour
{
    [SerializeField]
    string filePathLeft, filePathRight;
    [SerializeField]
    bool saveMask;
    public MaskReadyEvent masksReadyEvent;
    List<Texture2D> masks = new List<Texture2D>();

    void Awake()
    {
        MaskGenerator maskGenerator = new MaskGenerator();
        masks.AddRange(maskGenerator.Generate(filePathLeft, saveMask));
        masks.AddRange(maskGenerator.Generate(filePathRight, saveMask));
        Debug.Log("Invoking event.");
        masksReadyEvent.Invoke(masks);
    }
}
