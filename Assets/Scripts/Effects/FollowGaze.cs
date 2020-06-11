using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(FollowGazeRenderer), PostProcessEvent.AfterStack, "Custom/FollowGaze")]
public sealed class FollowGaze : PostProcessEffectSettings
{
    // empirical offset values through DrawCross.cs
    public float offsetLeftX = 0.0925f;
    public float offsetRightX = -0.0865f;
    public float offsetY = -0.0034f;
}

public sealed class FollowGazeRenderer : PostProcessEffectRenderer<FollowGaze>
{
    bool debug = false;

    public override void Render(PostProcessRenderContext context)
    {
        bool leftInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;
        bool rightInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;

        VarjoViewCamera varjoViewCamera;
        Camera cam;
        VarjoPlugin.ViewInfo viewInfo;
        Matrix4x4 projMat;
        float distance;
        Vector3 gazeOrigin, gazeDirection, planeForward;
        Tuple<float, float> offset;

        switch (context.camera.name)
        {
            case "Varjo Left Context":
                varjoViewCamera = GameObject.Find("Varjo Left Context").GetComponent<VarjoViewCamera>();
                cam = varjoViewCamera.GetCamera();
                viewInfo = VarjoManager.Instance.GetViewInfo((int)varjoViewCamera.CameraId);
                projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, cam.nearClipPlane, cam.farClipPlane);
                distance = projMat.m11;
                gazeOrigin = Double3ToVector3(VarjoPlugin.GetGaze().left.position);
                gazeDirection = Double3ToVector3(VarjoPlugin.GetGaze().left.forward);
                planeForward = -cam.transform.forward;
                offset = new Tuple<float, float>(settings.offsetLeftX, settings.offsetY);
                handleGazeDebug(context, varjoViewCamera, leftInvalid, distance, offset, gazeOrigin, gazeDirection, planeForward);
                break;
            case "Varjo Left Focus":
                varjoViewCamera = GameObject.Find("Varjo Left Focus").GetComponent<VarjoViewCamera>();
                cam = varjoViewCamera.GetCamera();
                viewInfo = VarjoManager.Instance.GetViewInfo((int)varjoViewCamera.CameraId);
                projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, cam.nearClipPlane, cam.farClipPlane);
                distance = projMat.m11;
                gazeOrigin = Double3ToVector3(VarjoPlugin.GetGaze().left.position);
                gazeDirection = Double3ToVector3(VarjoPlugin.GetGaze().left.forward);
                planeForward = -cam.transform.forward;
                offset = new Tuple<float, float>(0f, 0f);
                handleGazeDebug(context, varjoViewCamera, leftInvalid, distance, offset, gazeOrigin, gazeDirection, planeForward);
                break;
            case "Varjo Right Context":
                varjoViewCamera = GameObject.Find("Varjo Right Context").GetComponent<VarjoViewCamera>();
                cam = varjoViewCamera.GetCamera();
                viewInfo = VarjoManager.Instance.GetViewInfo((int)varjoViewCamera.CameraId);
                projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, cam.nearClipPlane, cam.farClipPlane);
                distance = projMat.m11;
                gazeOrigin = cam.transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().right.position));
                gazeDirection = cam.transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().right.forward));
                planeForward = cam.transform.TransformVector(-cam.transform.forward);
                offset = new Tuple<float, float>(settings.offsetRightX, settings.offsetY);
                handleGazeDebug(context, varjoViewCamera, rightInvalid, distance, offset, gazeOrigin, gazeDirection, planeForward);
                break;
            case "Varjo Right Focus":
                varjoViewCamera = GameObject.Find("Varjo Right Focus").GetComponent<VarjoViewCamera>();
                cam = varjoViewCamera.GetCamera();
                viewInfo = VarjoManager.Instance.GetViewInfo((int)varjoViewCamera.CameraId);
                projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, cam.nearClipPlane, cam.farClipPlane);
                distance = projMat.m11;
                gazeOrigin = Double3ToVector3(VarjoPlugin.GetGaze().right.position);
                gazeDirection = Double3ToVector3(VarjoPlugin.GetGaze().right.forward);
                planeForward = -cam.transform.forward;
                offset = new Tuple<float, float>(0f, 0f);
                handleGazeDebug(context, varjoViewCamera, rightInvalid, distance, offset, gazeOrigin, gazeDirection, planeForward);
                break;
        }
    }

    void handleGazeDebug(PostProcessRenderContext context, VarjoViewCamera varjoViewCamera, bool invalid, float distance, Tuple<float, float> offset, Vector3 gazeOrigin, Vector3 gazeForward, Vector3 planeForward)
    {

        // if(varjoViewCamera.name == "Varjo Right Context")
        // {
        //     Debug.Log("distance: " + distance + " d: " + Vector3.Dot(varjoViewCamera.GetCamera().transform.TransformVector(varjoViewCamera.GetCamera().transform.position), planeForward));
        // }

        float d = Vector3.Dot(varjoViewCamera.GetCamera().transform.TransformVector(varjoViewCamera.GetCamera().transform.position), -planeForward);

        var sheet = context.propertySheets.Get(Shader.Find("Custom/FollowGaze"));
        if(!invalid)
        {
            //Debug.Log("gaze forward: " + gazeForward.ToString("F4") + " plane forward " + planeForward.ToString("F4"));
            float denominator = Vector3.Dot(gazeForward, planeForward);

            if (Math.Abs(denominator) > 0.001f)
            {
                float t = -(d + Vector3.Dot(gazeOrigin, planeForward)) / denominator;
                //float t = -(Vector3.Dot(varjoViewCamera.GetCamera().transform.TransformVector(varjoViewCamera.GetCamera().transform.position) - gazeOrigin, planeForward))
                Vector3 hit = gazeOrigin + t * gazeForward;

                sheet.properties.SetVector("gaze", new Vector2(hit.x + offset.Item1, hit.y + offset.Item2));
                //sheet.properties.SetVector("gaze", new Vector2((1-(-hit.x / 2 + offset.Item1)), (1-(-hit.y / 2 + offset.Item2))));
            }
            else
            {
                sheet.properties.SetVector("gaze", new Vector2(0.5f + offset.Item1, 0.5f + offset.Item2));
            }
        }
        else
        {
            sheet.properties.SetVector("gaze", new Vector2(0.5f + offset.Item1, 0.5f + offset.Item2));
        }
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    Vector3 Double3ToVector3(double[] doubles)
    {
        if(this.debug)
        {
            Debug.Log(doubles[0] + " " + doubles[1]);
        }
        return new Vector3((float)doubles[0], (float)doubles[1], (float)doubles[2]);
    }
}
