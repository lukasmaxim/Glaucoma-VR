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
                followGazePlane(context, leftInvalid, gazeOriginLeft, gazeDirectionLeft, settings.aspectContext, settings.scaleFactorContext);
                break;
            case "Varjo Left Focus":
                followGazePlane(context, leftInvalid, gazeOriginLeft, gazeDirectionLeft, settings.aspectFocus, settings.scaleFactorFocus);
                break;
            case "Varjo Right Context":
                followGazePlane(context, rightInvalid, gazeOriginRight, gazeDirectionRight, settings.aspectContext, settings.scaleFactorContext);
                break;
            case "Varjo Right Focus":
                followGazePlane(context, rightInvalid, gazeOriginRight, gazeDirectionRight, settings.aspectFocus, settings.scaleFactorFocus);
                break;
        }
    }

    void followGazePlane(PostProcessRenderContext context, bool invalid, Vector3 gazeOrigin, Vector3 gazeDirection, float aspect, float scaleFactor)
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
            sheet.properties.SetFloat("scaleFactor", scaleFactor);
            sheet.properties.SetFloat("aspect", aspect);
        }
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    Vector3 Double3ToVector3(double[] doubles)
    {
        return new Vector3((float)doubles[0], (float)doubles[1], (float)doubles[2]);
    }
}
