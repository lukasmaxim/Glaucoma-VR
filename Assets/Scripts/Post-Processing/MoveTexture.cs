using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(MoveTextureRenderer), PostProcessEvent.AfterStack, "Debug/MoveTexture")]
public sealed class MoveTexture : PostProcessEffectSettings
{
    public Vector3 gazeDirectionStraight = new Vector3(0.0f, 0.0f, 1.0f);

    // context settings
    public int screenContext = -1;
    public float aspectContext = 0.9f;
    public float scaleFactorContext = 1.0f;
    public float offsetContextLeftX, offsetContextRightX, offsetContextLeftY, offsetContextRightY = 0;

    // focus settings
    public int screenFocus = 1;
    public float aspectFocus = 1.77f;
    [Range(6.0f, 6.2f)]
    public FloatParameter scaleFactorFocus = new FloatParameter { value = 6.093f };
    [Range(0.3f, 0.4f)]
    public FloatParameter offsetFocusLeftX = new FloatParameter { value = 0.354f };
    [Range(-0.7f, -0.6f)]
    public FloatParameter offsetFocusRightX = new FloatParameter { value = -0.645f };
    [Range(0.4f, 0.5f)]
    public FloatParameter offsetFocusLeftY = new FloatParameter { value = 0.418f };
    [Range(-0.6f, -0.5f)]
    public FloatParameter offsetFocusRightY = new FloatParameter { value = -0.582f };
}

public sealed class MoveTextureRenderer : PostProcessEffectRenderer<MoveTexture>
{
    // dummy transform to transform gaze from object to world coords
    Transform transform = GameObject.Find("Dummy Transform").transform;
    Vector3 gazeOriginLeft, gazeDirectionLeft, gazeOriginRight, gazeDirectionRight, gazeDirectionStraight;
    bool leftInvalid, rightInvalid;
    Vector2 offsetContextLeft, offsetContextRight, offsetFocusLeft, offsetFocusRight;

    // called at the end of each frame's rendering pipe
    public override void Render(PostProcessRenderContext context)
    {
        // get gaze validity
        leftInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;
        rightInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;

        // get hmd pose
        VarjoPlugin.Matrix matrix;
        VarjoManager.Instance.GetPose(VarjoPlugin.PoseType.CENTER, out matrix);
        if(Double3ToVector3(matrix.value) != Vector3.zero) // this is so we don't get annoying console logs when we are in editor mode...
        {
            transform.rotation = VarjoManager.Instance.GetHMDOrientation(VarjoPlugin.PoseType.CENTER);
            transform.position = VarjoManager.Instance.GetHMDPosition(VarjoPlugin.PoseType.CENTER);
        }

        // offset
        offsetFocusLeft = new Vector2(settings.offsetFocusLeftX, settings.offsetFocusLeftY);
        offsetFocusRight = new Vector2(settings.offsetFocusRightX, settings.offsetFocusRightY);
        offsetContextLeft = new Vector2(settings.offsetContextLeftX, settings.offsetContextLeftY);
        offsetContextRight = new Vector2(settings.offsetContextRightX, settings.offsetContextRightY);

        // gaze
        gazeOriginLeft = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().left.position));
        gazeDirectionLeft = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().left.forward));
        gazeOriginRight = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().left.position));
        gazeDirectionRight = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().left.forward));

        // default gaze
        gazeDirectionStraight = transform.TransformPoint(settings.gazeDirectionStraight);

        // switch based on eye
        switch (context.camera.name)
        {
            case "Varjo Left Context":
                moveTexture(context, leftInvalid, gazeOriginLeft, gazeDirectionLeft, settings.aspectContext, settings.scaleFactorContext, offsetContextLeft, settings.screenContext);
                break;
            case "Varjo Left Focus":
                moveTexture(context, leftInvalid, gazeOriginLeft, gazeDirectionLeft, settings.aspectFocus, settings.scaleFactorFocus, offsetFocusLeft, settings.screenFocus);
                break;
            case "Varjo Right Context":
                moveTexture(context, rightInvalid, gazeOriginRight, gazeDirectionRight, settings.aspectContext, settings.scaleFactorContext, offsetContextRight, settings.screenContext);
                break;
            case "Varjo Right Focus":
                moveTexture(context, rightInvalid, gazeOriginRight, gazeDirectionRight, settings.aspectFocus, settings.scaleFactorFocus, offsetFocusRight, settings.screenFocus);
                break;
        }
    }

    // move texture for one camera
    void moveTexture(PostProcessRenderContext context, bool invalid, Vector3 gazeOrigin, Vector3 gazeDirection, float aspect, float scaleFactor, Vector2 offset, int screen)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Debug/MoveTexture"));

        // check if gaze data is valid
        if (!invalid)
        {
            sheet.properties.SetVector("gaze", gazeOrigin + gazeDirection);
            sheet.properties.SetFloat("scaleFactor", scaleFactor);
            sheet.properties.SetFloat("aspect", aspect);
            sheet.properties.SetVector("offset", offset);
            sheet.properties.SetInt("screen", screen);
        }
        else
        {
            sheet.properties.SetVector("gaze", gazeDirectionStraight);
            sheet.properties.SetFloat("scaleFactor", scaleFactor);
            sheet.properties.SetFloat("aspect", aspect);
            sheet.properties.SetVector("offset", offset);
            sheet.properties.SetInt("screen", screen);
        }
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    Vector3 Double3ToVector3(double[] doubles)
    {
        return new Vector3((float)doubles[0], (float)doubles[1], (float)doubles[2]);
    }
}
