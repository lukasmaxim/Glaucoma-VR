using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(FollowGazeNewRenderer), PostProcessEvent.AfterStack, "Custom/FollowGazeNew")]
public sealed class FollowGazeNew : PostProcessEffectSettings
{
    // empirical offset values through DrawCross.cs
    public float offsetLeftX = 0.0925f;
    public float offsetRightX = -0.0865f;
    public float offsetY = -0.0034f;

    public float camNearClipPlane = 0.01f;
    public float camFarClipPlane = 1000f;
    public float worldScreenRatio = 0.1f;
}

public sealed class FollowGazeNewRenderer : PostProcessEffectRenderer<FollowGazeNew>
{
    Transform transform = new GameObject("Dummy Transform").transform;
    VarjoViewCamera varjoViewCameraLeftContext, varjoViewCameraLeftFocus, varjoViewCameraRightContext, varjoViewCameraRightFocus;
    VarjoPlugin.ViewInfo viewInfoLeftContext, viewInfoLeftFocus, viewInfoRightContext, viewInfoRightFocus;
    Vector3 gazeOriginLeft, gazeDirectionLeft, gazeOriginRight, gazeDirectionRight;
    bool leftInvalid, rightInvalid;

    public override void Render(PostProcessRenderContext context)
    {
        GetViewInfo();

        // get gaze status
        leftInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;
        rightInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;

        // get hmd pose
        transform.rotation = VarjoManager.Instance.GetHMDOrientation(VarjoPlugin.PoseType.CENTER);
        transform.position = VarjoManager.Instance.GetHMDPosition(VarjoPlugin.PoseType.CENTER);

        // get gaze
        gazeOriginLeft = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().left.position));
        gazeDirectionLeft = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().left.forward) * settings.worldScreenRatio);
        gazeOriginRight = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().left.position));
        gazeDirectionRight = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().left.forward) * settings.worldScreenRatio);

        // switch based on eye
        switch (context.camera.name)
        {
            case "Varjo Left Context":
                handleGazeDebug(context, viewInfoLeftContext, leftInvalid, gazeOriginLeft, gazeDirectionLeft);
                break;
            case "Varjo Left Focus":
                handleGazeDebug(context, viewInfoLeftFocus, leftInvalid, gazeOriginLeft, gazeDirectionLeft);
                break;
            case "Varjo Right Context":
                handleGazeDebug(context, viewInfoRightContext, rightInvalid, gazeOriginRight, gazeDirectionRight);
                break;
            case "Varjo Right Focus":
                handleGazeDebug(context, viewInfoRightFocus, rightInvalid, gazeOriginRight, gazeDirectionRight);
                break;
        }
    }

    void handleGazeDebug(PostProcessRenderContext context, VarjoPlugin.ViewInfo viewInfo, bool invalid, Vector3 gazeOrigin, Vector3 gazeDirection)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/FollowGaze"));

        if (!invalid)
        {
            Matrix4x4 viewMat = VarjoMatrixUtils.ViewMatrixToUnity(viewInfo.invViewMatrix);
            Matrix4x4 projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, settings.camNearClipPlane, settings.camFarClipPlane);

            Vector3 dir = projMat.MultiplyPoint(gazeDirection);
            Vector3 pos = projMat.MultiplyPoint(gazeOrigin);

            Vector3 hit = ((pos + dir) / -2.0f) + new Vector3(0.5f, 0.5f, 0); // MINUS 2 because the dot will move diametrically to the gaze direction

            sheet.properties.SetVector("gaze", new Vector2(hit.x, hit.y));
        }
        else
        {
            sheet.properties.SetVector("gaze", new Vector2(0.5f, 0.5f));
        }
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    void GetViewInfo()
    {
        if (!varjoViewCameraLeftContext)
        {
            varjoViewCameraLeftContext = GameObject.Find("Varjo Left Context").GetComponent<VarjoViewCamera>();
            viewInfoLeftContext = VarjoManager.Instance.GetViewInfo((int)varjoViewCameraLeftContext.CameraId);
        }
        if (!varjoViewCameraLeftFocus)
        {
            varjoViewCameraLeftFocus = GameObject.Find("Varjo Left Focus").GetComponent<VarjoViewCamera>();
            viewInfoLeftFocus = VarjoManager.Instance.GetViewInfo((int)varjoViewCameraLeftFocus.CameraId);
        }
        if (!varjoViewCameraRightContext)
        {
            varjoViewCameraRightContext = GameObject.Find("Varjo Right Context").GetComponent<VarjoViewCamera>();
            viewInfoRightContext = VarjoManager.Instance.GetViewInfo((int)varjoViewCameraRightContext.CameraId);
        }
        if (!varjoViewCameraRightFocus)
        {
            varjoViewCameraRightFocus = GameObject.Find("Varjo Right Focus").GetComponent<VarjoViewCamera>();
            viewInfoRightFocus = VarjoManager.Instance.GetViewInfo((int)varjoViewCameraRightFocus.CameraId);
        }
    }

    Vector3 Double3ToVector3(double[] doubles)
    {
        return new Vector3((float)doubles[0], (float)doubles[1], (float)doubles[2]);
    }
}
