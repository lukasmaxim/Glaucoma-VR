using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

public abstract class PostProcessMaskSettings : PostProcessEffectSettings
{
}

public abstract class PostProcessMaskRenderer : PostProcessEffectRenderer<PostProcessMaskSettings>
{
    // dummy transform to transform gaze from object to world coords
    Transform transform = GameObject.Find("Dummy Transform").transform;
    Vector3 gazeOriginLeft, gazeDirectionLeft, gazeOriginRight, gazeDirectionRight, gazeDirectionStraight, gazeVector;
    bool leftInvalid, rightInvalid, setup = false;
    Vector2 offsetContextLeft, offsetContextRight, offsetFocusLeft, offsetFocusRight, offset;
    public PostProcessRenderContext context;
    public PropertySheet sheet;
    public MaskSettings maskSettings;
    int eye, screen;
    bool invalid;
    float scaleFactor, aspect;

    // called every frame after done rendering
    public override void Render(PostProcessRenderContext context)
    {
        Setup(context);
        GetCurrentData();
        SetProperties();
    }

    // initial setup
    void Setup(PostProcessRenderContext context)
    {
        // only do this once
        if (!setup)
        {
            this.context = context;
            maskSettings = GameObject.Find("VarjoCamera").GetComponent<MaskSettings>();
            SetInitialEffectProperties();
            SetInitialCommonProperties();
            setup = true;
        }
    }

    // sets initial common shader properties
    void SetInitialCommonProperties()
    {
        sheet.properties.SetTexture("_MaskTexLeftContext", this.maskSettings.maskLeftContext);
        sheet.properties.SetTexture("_MaskTexLeftFocus", this.maskSettings.maskLeftFocus);
        sheet.properties.SetTexture("_MaskTexRightContext", this.maskSettings.maskRightContext);
        sheet.properties.SetTexture("_MaskTexRightFocus", this.maskSettings.maskRightFocus);
    }

    // sets initial effect shader properties
    public abstract void SetInitialEffectProperties();

    // gets and sets the data for the current frame (gaze, hmd pose, offset)
    void GetCurrentData()
    {
        // === gaze ===
        // validity
        leftInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;
        rightInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;
        // origin & direction
        gazeOriginLeft = transform.TransformPoint(Utils.Double3ToVector3(VarjoPlugin.GetGaze().left.position));
        gazeDirectionLeft = transform.TransformVector(Utils.Double3ToVector3(VarjoPlugin.GetGaze().left.forward));
        gazeOriginRight = transform.TransformPoint(Utils.Double3ToVector3(VarjoPlugin.GetGaze().left.position));
        gazeDirectionRight = transform.TransformVector(Utils.Double3ToVector3(VarjoPlugin.GetGaze().left.forward));
        // default
        gazeDirectionStraight = transform.TransformPoint(maskSettings.gazeDirectionStraight);

        // === hmd pose ===
        VarjoPlugin.Matrix matrix;
        VarjoManager.Instance.GetPose(VarjoPlugin.PoseType.CENTER, out matrix);
        if (Utils.Double3ToVector3(matrix.value) != Vector3.zero) // this is so we don't get annoying console logs when we are in editor mode...
        {
            transform.rotation = VarjoManager.Instance.GetHMDOrientation(VarjoPlugin.PoseType.CENTER);
            transform.position = VarjoManager.Instance.GetHMDPosition(VarjoPlugin.PoseType.CENTER);
        }

        // === offset ===
        offsetFocusLeft = new Vector2(maskSettings.offsetFocusLeftX, maskSettings.offsetFocusLeftY);
        offsetFocusRight = new Vector2(-(1-maskSettings.offsetFocusLeftX), -(1-maskSettings.offsetFocusLeftY));
        offsetContextLeft = new Vector2(maskSettings.offsetContextLeftX, maskSettings.offsetContextLeftY);
        offsetContextRight = new Vector2(maskSettings.offsetContextLeftX, maskSettings.offsetContextLeftY);
    }

    // sets shader properties for the current frame
    void SetProperties()
    {
        // set properties based on camera used
        switch (context.camera.name)
        {
            case "Varjo Left Context":
                eye = maskSettings.eyeLeft;
                screen = maskSettings.screenContext;
                invalid = leftInvalid;
                gazeVector = !invalid ? gazeOriginLeft + gazeDirectionLeft : gazeDirectionStraight;
                scaleFactor = maskSettings.scaleFactorContext;
                aspect = maskSettings.aspectContext;
                offset = offsetContextLeft;
                SetPropertiesForCamera();
                break;
            case "Varjo Left Focus":
                eye = maskSettings.eyeLeft;
                screen = maskSettings.screenFocus;
                invalid = leftInvalid;
                gazeVector = !invalid ? gazeOriginLeft + gazeDirectionLeft : gazeDirectionStraight;
                scaleFactor = maskSettings.scaleFactorFocus;
                aspect = maskSettings.aspectFocus;
                offset = offsetFocusLeft;
                SetPropertiesForCamera();
                break;
            case "Varjo Right Context":
                eye = maskSettings.eyeRight;
                screen = maskSettings.screenContext;
                invalid = rightInvalid;
                gazeVector = !invalid ? gazeOriginLeft + gazeDirectionLeft : gazeDirectionStraight;
                scaleFactor = maskSettings.scaleFactorContext;
                aspect = maskSettings.aspectContext;
                offset = offsetContextRight;
                SetPropertiesForCamera();
                break;
            case "Varjo Right Focus":
                eye = maskSettings.eyeRight;
                screen = maskSettings.screenFocus;
                invalid = rightInvalid;
                gazeVector = !invalid ? gazeOriginLeft + gazeDirectionLeft : gazeDirectionStraight;
                scaleFactor = maskSettings.scaleFactorFocus;
                aspect = maskSettings.aspectFocus;
                offset = offsetFocusRight;
                SetPropertiesForCamera();
                break;
        }
    }

    // sets shader properties for specific camera
    void SetPropertiesForCamera()
    {
        SetCommonProperties();
        SetEffectProperties();
        Blit();
    }

    // sets per frame common shader properties
    void SetCommonProperties()
    {
        sheet.properties.SetInt("eye", eye);
        sheet.properties.SetInt("screen", screen);
        sheet.properties.SetFloat("scaleFactor", scaleFactor);
        sheet.properties.SetFloat("aspect", aspect);
        sheet.properties.SetVector("offset", offset);
        sheet.properties.SetVector("gaze", gazeVector);
    }

    // sets effect shader properties for a frame
    public abstract void SetEffectProperties();

    // finally blits current frame with shader output
    void Blit()
    {
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
