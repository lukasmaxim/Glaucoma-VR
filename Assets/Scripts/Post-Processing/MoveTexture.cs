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
    public float aspectFocus = 1.77f;
    public float aspectContext = 0.9f;
    [Range(6.0f, 6.2f)]
    public FloatParameter scaleFactorFocus = new FloatParameter { value = 6f };
    public float scaleFactorContext = 1.0f;

    public float offsetLeftX = 0;
    public float offsetRightX = 0 ;
    public float offsetY = 0;

    public int screenContext = -1;
    public int screenFocus = 1;

    [Range(0.3f, 0.4f)]
    public FloatParameter offsetFocusLeftX = new FloatParameter { value = 0.344f };
    [Range(-0.7f, -0.6f)]
    public FloatParameter offsetFocusRightX = new FloatParameter { value = -0.652f };
    [Range(0.4f, 0.5f)]
    public FloatParameter offsetFocusLeftY = new FloatParameter { value = 0.412f };
    [Range(-0.6f, -0.5f)]
    public FloatParameter offsetFocusRightY = new FloatParameter { value = -0.586f };
}

public sealed class MoveTextureRenderer : PostProcessEffectRenderer<MoveTexture>
{
    // dummy transform to transform gaze from object to world coords
    Transform transform = GameObject.Find("Dummy Transform").transform;
    Vector3 gazeOriginLeft, gazeDirectionLeft, gazeOriginRight, gazeDirectionRight;
    bool leftInvalid, rightInvalid;

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

        // get gaze
        gazeOriginLeft = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().left.position));
        gazeDirectionLeft = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().left.forward));
        gazeOriginRight = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().left.position));
        gazeDirectionRight = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().left.forward));

        // switch based on eye
        switch (context.camera.name)
        {
            case "Varjo Left Context":
                moveTexture(context, leftInvalid, gazeOriginLeft, gazeDirectionLeft, settings.aspectContext, settings.scaleFactorContext, new Vector2(settings.offsetLeftX, settings.offsetY), settings.screenContext);
                break;
            case "Varjo Left Focus":
                moveTexture(context, leftInvalid, gazeOriginLeft, gazeDirectionLeft, settings.aspectFocus, settings.scaleFactorFocus, new Vector2(settings.offsetFocusLeftX, settings.offsetFocusLeftY), settings.screenFocus);
                break;
            case "Varjo Right Context":
                moveTexture(context, rightInvalid, gazeOriginRight, gazeDirectionRight, settings.aspectContext, settings.scaleFactorContext, new Vector2(settings.offsetRightX, settings.offsetY), settings.screenContext);
                break;
            case "Varjo Right Focus":
                moveTexture(context, rightInvalid, gazeOriginRight, gazeDirectionRight, settings.aspectFocus, settings.scaleFactorFocus, new Vector2(settings.offsetFocusRightX, settings.offsetFocusRightY), settings.screenFocus);
                break;
        }
    }

    void moveTexture(PostProcessRenderContext context, bool invalid, Vector3 gazeOrigin, Vector3 gazeDirection, float aspect, float scaleFactor, Vector2 offset, int screen)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Debug/MoveTexture"));

        if (!invalid)
        {
            sheet.properties.SetVector("gaze", gazeOrigin + gazeDirection);
            sheet.properties.SetFloat("scaleFactor", scaleFactor);
            sheet.properties.SetFloat("aspect", aspect);
        }
        else
        {
            sheet.properties.SetVector("gaze", transform.TransformPoint(settings.gazeDirectionStraight));
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
