using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlobalSettings : MonoBehaviour
{
    // textures
    public Texture2D maskLeftContext;
    public Texture2D maskLeftFocus;
    public Texture2D maskRightContext;
    public Texture2D maskRightFocus;

    // effect modifiers contrast / brightness
    [Range(0.1f, 1.0f)]
    public FloatParameter contrastMultiplier = new FloatParameter { value = 0.5f };
    [Range(-1.0f, 1.0f)]
    public FloatParameter brightnessModifier = new FloatParameter { value = 0.0f };

    // effect modifiers blur
    [Range(5, 10)]
    public int kernelSize1 = 5;
    [Range(15, 75)]
    public int kernelSize2 = 50;
    [Range(50, 120)]
    public int kernelSize3 = 100;

    // eye side indicator
    public int eyeLeft = -1;
    public int eyeRight = 1;

    // context display settings
    public int screenContext = -1;
    public float aspectContext = 0.9f;
    public float scaleFactorContext = 1.0f;
    public float offsetContextLeftX, offsetContextRightX, offsetContextLeftY, offsetContextRightY = 0;

    // focus display scales
    public int screenFocus = 1;
    public float aspectFocus = 1.77f;

    [Range(6.0f, 6.2f)]
    public float scaleFactorFocus = 6.093f;
    [Range(0.3f, 0.4f)]
    public FloatParameter offsetFocusLeftX = new FloatParameter { value = 0.354f };
    [Range(-0.7f, -0.6f)]
    public FloatParameter offsetFocusRightX = new FloatParameter { value = -0.645f };
    [Range(0.4f, 0.5f)]
    public FloatParameter offsetFocusLeftY = new FloatParameter { value = 0.418f };
    [Range(-0.6f, -0.5f)]
    public FloatParameter offsetFocusRightY = new FloatParameter { value = -0.582f };



    
    
    // default gaze direction (straight from cam)
    public Vector3 gazeDirectionStraight = new Vector3(0.0f, 0.0f, 1.0f);

}