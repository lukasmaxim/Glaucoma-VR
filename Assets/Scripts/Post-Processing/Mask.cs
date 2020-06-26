using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

public abstract class MaskSettings : PostProcessEffectSettings
{
}

public abstract class MaskRenderer : PostProcessEffectRenderer<MaskSettings>
{
    // dummy transform to transform gaze from object to world coords
    Transform transform = GameObject.Find("Dummy Transform").transform;
    Vector3 gazeOriginLeft, gazeDirectionLeft, gazeOriginRight, gazeDirectionRight, gazeDirectionStraight;
    bool leftInvalid, rightInvalid, setup = false;
    Vector2 offsetContextLeft, offsetContextRight, offsetFocusLeft, offsetFocusRight;
    public PostProcessRenderContext context;
    public PropertySheet sheet;
    public GlobalSettings globalSettings;

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
            globalSettings = GameObject.Find("App").GetComponent<GlobalSettings>();
            SetInitialEffectProperties();
            SetInitialCommonProperties();
            setup = true;
        }
    }

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
        gazeDirectionStraight = transform.TransformPoint(globalSettings.gazeDirectionStraight);

        // === hmd pose ===
        VarjoPlugin.Matrix matrix;
        VarjoManager.Instance.GetPose(VarjoPlugin.PoseType.CENTER, out matrix);
        if (Utils.Double3ToVector3(matrix.value) != Vector3.zero) // this is so we don't get annoying console logs when we are in editor mode...
        {
            transform.rotation = VarjoManager.Instance.GetHMDOrientation(VarjoPlugin.PoseType.CENTER);
            transform.position = VarjoManager.Instance.GetHMDPosition(VarjoPlugin.PoseType.CENTER);
        }

        // === offset ===
        offsetFocusLeft = new Vector2(globalSettings.offsetFocusLeftX, globalSettings.offsetFocusLeftY);
        offsetFocusRight = new Vector2(globalSettings.offsetFocusRightX, globalSettings.offsetFocusRightY);
        offsetContextLeft = new Vector2(globalSettings.offsetContextLeftX, globalSettings.offsetContextLeftY);
        offsetContextRight = new Vector2(globalSettings.offsetContextRightX, globalSettings.offsetContextRightY);
    }

    // sets shader properties for the current frame
    void SetProperties()
    {
        // set properties based on camera used
        switch (context.camera.name)
        {
            case "Varjo Left Context":
                SetPropertiesForCamera(sheet, globalSettings.eyeLeft, leftInvalid, gazeOriginLeft, gazeDirectionLeft, globalSettings.aspectContext, globalSettings.scaleFactorContext, offsetContextLeft, globalSettings.screenContext);
                break;
            case "Varjo Left Focus":
                SetPropertiesForCamera(sheet, globalSettings.eyeLeft, leftInvalid, gazeOriginLeft, gazeDirectionLeft, globalSettings.aspectFocus, globalSettings.scaleFactorFocus, offsetFocusLeft, globalSettings.screenFocus);
                break;
            case "Varjo Right Context":
                SetPropertiesForCamera(sheet, globalSettings.eyeRight, rightInvalid, gazeOriginRight, gazeDirectionRight, globalSettings.aspectContext, globalSettings.scaleFactorContext, offsetContextRight, globalSettings.screenContext);
                break;
            case "Varjo Right Focus":
                SetPropertiesForCamera(sheet, globalSettings.eyeRight, rightInvalid, gazeOriginRight, gazeDirectionRight, globalSettings.aspectFocus, globalSettings.scaleFactorFocus, offsetFocusRight, globalSettings.screenFocus);
                break;
        }
    }

    // set shader properties for specific camera
    void SetPropertiesForCamera(PropertySheet sheet, int eye, bool invalid, Vector3 gazeOrigin, Vector3 gazeDirection, float aspect, float scaleFactor, Vector2 offset, int screen)
    {
        SetCommonProperties(sheet, eye, invalid, gazeOrigin, gazeDirection, aspect, scaleFactor, offset, screen);
        SetEffectProperties();
        Blit();
    }

    // set per frame common shader properties
    void SetCommonProperties(PropertySheet sheet, int eye, bool invalid, Vector3 gazeOrigin, Vector3 gazeDirection, float aspect, float scaleFactor, Vector2 offset, int screen)
    {
        sheet.properties.SetInt("eye", eye);
        sheet.properties.SetInt("screen", screen);
        sheet.properties.SetFloat("scaleFactor", scaleFactor);
        sheet.properties.SetFloat("aspect", aspect);
        sheet.properties.SetVector("offset", offset);

        // check if gaze data is valid
        if (!invalid)
        {
            sheet.properties.SetVector("gaze", gazeOrigin + gazeDirection);
        }
        else
        {
            sheet.properties.SetVector("gaze", gazeDirectionStraight);
        }
    }

    // sets effect shader properties for a frame
    public abstract void SetEffectProperties();

    // finally blits current frame with shader output
    void Blit()
    {
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    // set initial common shader properties
    void SetInitialCommonProperties()
    {
        sheet.properties.SetTexture("_MaskTexLeftContext", this.globalSettings.maskLeftContext);
        sheet.properties.SetTexture("_MaskTexLeftFocus", this.globalSettings.maskLeftFocus);
        sheet.properties.SetTexture("_MaskTexRightContext", this.globalSettings.maskRightContext);
        sheet.properties.SetTexture("_MaskTexRightFocus", this.globalSettings.maskRightFocus);
    }

    public abstract void SetInitialEffectProperties();
}
