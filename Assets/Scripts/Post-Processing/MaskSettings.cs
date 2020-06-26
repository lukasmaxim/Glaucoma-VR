using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MaskSettings : MonoBehaviour
{
    // textures
    [Header("Textures")]
    public Texture2D maskLeftContext;
    public Texture2D maskLeftFocus;
    public Texture2D maskRightContext;
    public Texture2D maskRightFocus;

    [Header("Contrast / Brightness")]
    // effect modifiers contrast / brightness
    [Range(0.1f, 1.0f)]
    public float contrastMultiplier =  0.5f;
    [Range(-1.0f, 1.0f)]
    public float brightnessModifier = 0.0f;

    [Header("Blur")]
    // effect modifiers blur
    [Range(5, 10)]
    public int kernelSize1 = 5;
    [Range(15, 75)]
    public int kernelSize2 = 50;
    [Range(50, 120)]
    public int kernelSize3 = 100;

    // eye side indicator
    [HideInInspector]
    public int eyeLeft = -1;
    [HideInInspector]
    public int eyeRight = 1;

    // context display settings
    [HideInInspector]
    public int screenContext = -1;
    [Header("Context Screen")]
    [Range(0.5f, 1.2f)]
    public float aspectContext = 0.9f;
    [Range(0.5f, 1.5f)]
    public float scaleFactorContext = 1.0f;
    [Range(-0.5f, 0.5f)]
    public float offsetContextLeftX, offsetContextRightX, offsetContextLeftY, offsetContextRightY = 0;

    // focus display scales
    [HideInInspector]
    public int screenFocus = 1;
    [Header("Focus Screen")]    
    [Range(1.7f, 1.9f)]
    public float aspectFocus = 1.77f;
    [Range(6.0f, 6.2f)]
    public float scaleFactorFocus = 6.093f;
    [Range(0.3f, 0.4f)]
    public float offsetFocusLeftX =  0.354f;
    [Range(0.4f, 0.5f)]
    public float offsetFocusLeftY = 0.418f;
    
    [HideInInspector]
    // default gaze direction (straight from cam)
    public Vector3 gazeDirectionStraight = new Vector3(0.0f, 0.0f, 1.0f);

    [Header("Debug")]
    public Color overlayColor = Color.yellow;
    [Range(0.0f, 1.0f)]
    public float alphaCutoff = 0.4f;
}
