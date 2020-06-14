using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(MoveTextureRenderer), PostProcessEvent.AfterStack, "Debug/MoveTexture")]
public sealed class MoveTexture : PostProcessEffectSettings
{
    public Vector3 defaultVector = new Vector3(0, 0, 1);
    public float aspectFocus = 1.77f;
    public float aspectContext = 0.9f;
    public float scaleFactorFocus = 5.7f;
    public float scaleFactorContext = 1.0f;


    [Range(0f, 0.1f), Tooltip("Left x offset.")]
    public FloatParameter offsetLeftX = new FloatParameter { value = -0.0925f };
    [Range(0f, 0.1f), Tooltip("Right x offset.")]

    public FloatParameter offsetRightX = new FloatParameter { value = 0.0865f };
    [Range(-0.1f, 0f), Tooltip("y offset.")]
    public FloatParameter offsetY = new FloatParameter { value = 0.0034f };

    public int screenContext = -1;
    public int screenFocus = 1;

    [Range(0f, 1)]
    public FloatParameter offsetFocusX = new FloatParameter { value = 0.0034f };
    [Range(0f, 1)]
    public FloatParameter offsetFocusY = new FloatParameter { value = 0.0034f };
    [Range(3f, 6f)]
    public FloatParameter scaleFactor = new FloatParameter { value = 0.3f };
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
                moveTexture(context, leftInvalid, gazeOriginLeft, gazeDirectionLeft, settings.aspectFocus, settings.scaleFactorFocus, new Vector2(settings.offsetFocusX, settings.offsetFocusY), settings.screenFocus);
                break;
            case "Varjo Right Context":
                moveTexture(context, rightInvalid, gazeOriginRight, gazeDirectionRight, settings.aspectContext, settings.scaleFactorContext, new Vector2(settings.offsetRightX, settings.offsetY), settings.screenContext);
                break;
            case "Varjo Right Focus":
                moveTexture(context, rightInvalid, gazeOriginRight, gazeDirectionRight, settings.aspectFocus, settings.scaleFactorFocus, new Vector2(settings.offsetFocusX, settings.offsetFocusY), settings.screenFocus);
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
            sheet.properties.SetVector("gaze", settings.defaultVector);
            sheet.properties.SetFloat("scaleFactor", settings.scaleFactor);
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
