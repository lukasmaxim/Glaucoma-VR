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

    [Range(0.5f, 5f)]
    public FloatParameter t = new FloatParameter{ value = 0.5f };
}

public sealed class FollowGazeRenderer : PostProcessEffectRenderer<FollowGaze>
{
    bool debug = false;
    Transform transform = new GameObject("My Transform").transform;

    public override void Render(PostProcessRenderContext context)
    {
        bool leftInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;
        bool rightInvalid = VarjoPlugin.GetGaze().leftStatus == VarjoPlugin.GazeEyeStatus.EYE_INVALID;

        VarjoViewCamera varjoViewCamera;
        Camera cam;
        VarjoPlugin.ViewInfo viewInfo;
        Matrix4x4 projMat, viewMat;
        float distance;
        Vector3 gazeOrigin, gazeDirection, planeForward, gazePoint;
        Tuple<float, float> offset;

        //transform = new GameObject("My Transform").transform;
        // transform.rotation = VarjoManager.Instance.GetHMDOrientation(VarjoPlugin.PoseType.CENTER);
        // transform.position = VarjoManager.Instance.GetHMDPosition(VarjoPlugin.PoseType.CENTER);
        // Fetch head pose
        // transform.position = VarjoManager.Instance.HeadTransform.position;
        // transform.rotation = VarjoManager.Instance.HeadTransform.rotation;

        switch (context.camera.name)
        {
            case "Varjo Left Context":
                varjoViewCamera = GameObject.Find("Varjo Left Context").GetComponent<VarjoViewCamera>();
                cam = varjoViewCamera.GetCamera();
                viewInfo = VarjoManager.Instance.GetViewInfo((int)varjoViewCamera.CameraId);
                viewMat = VarjoMatrixUtils.ViewMatrixToUnity(viewInfo.invViewMatrix);
                projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, cam.nearClipPlane, cam.farClipPlane);
                distance = projMat.m11;
                gazeOrigin = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().left.position));
                gazeDirection = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().left.forward)*0.1f);
                planeForward = transform.TransformVector(-cam.transform.forward);
                //offset = new Tuple<float, float>(0f, 0f);
                offset = new Tuple<float, float>(settings.offsetLeftX, settings.offsetY);
                handleGazeDebug(projMat, viewMat, context, varjoViewCamera, leftInvalid, distance, offset, gazeOrigin, gazeDirection, planeForward);
                break;
            case "Varjo Left Focus":
                varjoViewCamera = GameObject.Find("Varjo Left Focus").GetComponent<VarjoViewCamera>();
                cam = varjoViewCamera.GetCamera();
                viewInfo = VarjoManager.Instance.GetViewInfo((int)varjoViewCamera.CameraId);
                viewMat = VarjoMatrixUtils.ViewMatrixToUnity(viewInfo.invViewMatrix);
                projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, cam.nearClipPlane, cam.farClipPlane);
                distance = projMat.m11;
                gazeOrigin = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().left.position));
                gazeDirection = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().left.forward)*0.1f);
                planeForward = transform.TransformVector(-cam.transform.forward);
                offset = new Tuple<float, float>(0f, 0f);
                handleGazeDebug(projMat, viewMat, context, varjoViewCamera, leftInvalid, distance, offset, gazeOrigin, gazeDirection, planeForward);
                break;
            case "Varjo Right Context":
                varjoViewCamera = GameObject.Find("Varjo Right Context").GetComponent<VarjoViewCamera>();
                cam = varjoViewCamera.GetCamera();
                viewInfo = VarjoManager.Instance.GetViewInfo((int)varjoViewCamera.CameraId);
                viewMat = VarjoMatrixUtils.ViewMatrixToUnity(viewInfo.invViewMatrix);
                projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, cam.nearClipPlane, cam.farClipPlane);
                distance = projMat.m11;
                gazeOrigin = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().right.position));
                gazeDirection = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().right.forward)*0.1f);
                planeForward = transform.TransformVector(-cam.transform.forward);
                offset = new Tuple<float, float>(settings.offsetRightX, settings.offsetY);
                //offset = new Tuple<float, float>(0f, 0f);
                handleGazeDebug(projMat, viewMat, context, varjoViewCamera, rightInvalid, distance, offset, gazeOrigin, gazeDirection, planeForward);
                break;
            case "Varjo Right Focus":
                varjoViewCamera = GameObject.Find("Varjo Right Focus").GetComponent<VarjoViewCamera>();
                cam = varjoViewCamera.GetCamera();
                viewInfo = VarjoManager.Instance.GetViewInfo((int)varjoViewCamera.CameraId);
                viewMat = VarjoMatrixUtils.ViewMatrixToUnity(viewInfo.invViewMatrix);
                projMat = VarjoMatrixUtils.ProjectionMatrixToUnity(viewInfo.projectionMatrix, cam.nearClipPlane, cam.farClipPlane);
                distance = projMat.m11;



                gazeOrigin = transform.TransformPoint(Double3ToVector3(VarjoPlugin.GetGaze().right.position));
                gazeDirection = transform.TransformVector(Double3ToVector3(VarjoPlugin.GetGaze().right.forward)*0.1f);
                planeForward = transform.TransformVector(-cam.transform.forward);

                Debug.Log("origin: " + gazeDirection + " direction: " + gazeDirection);


                offset = new Tuple<float, float>(0f, 0f);

                handleGazeDebug(projMat, viewMat, context, varjoViewCamera, rightInvalid, distance, offset, gazeOrigin, gazeDirection, planeForward);
                break;
        }
    }

    void handleGazeDebug(Matrix4x4 projMat, Matrix4x4 viewMat, PostProcessRenderContext context, VarjoViewCamera varjoViewCamera, bool invalid, float distance, Tuple<float, float> offset, Vector3 gazeOrigin, Vector3 gazeForward, Vector3 planeForward)
    {

        // if(varjoViewCamera.name == "Varjo Right Context")
        // {
        //     Debug.Log("distance: " + distance + " d: " + Vector3.Dot(varjoViewCamera.GetCamera().transform.TransformVector(varjoViewCamera.GetCamera().transform.position), planeForward));
        // }


        var sheet = context.propertySheets.Get(Shader.Find("Custom/FollowGaze"));
        if(!invalid)
        {
            // HARDCODED SETTINGS
            // GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            // plane.transform.SetParent(transform);
            // plane.transform.forward = new Vector3(1,1,-1);
            // planeForward = plane.transform.forward;
            // distance = 1.01156f;

            //Debug.Log("gaze forward: " + gazeForward.ToString("F4") + " plane forward " + planeForward.ToString("F4"));
            float denominator = Vector3.Dot(gazeForward, planeForward);

            if (Math.Abs(denominator) > 0.001f)
            {
                //float t = -(distance + Vector3.Dot(gazeOrigin, planeForward)) / denominator;
                //float t = -(Vector3.Dot(varjoViewCamera.GetCamera().transform.TransformVector(varjoViewCamera.GetCamera().transform.position) - gazeOrigin, planeForward))
                //Vector3 hit = gazeOrigin + t * gazeForward;

                //Vector3 hit = projMat.MultiplyVector(gazeOrigin + gazeForward)/2 + new Vector3(0.5f, 1f, 0);
                Vector3 dir = projMat.MultiplyPoint(gazeForward);
                Vector3 pos = projMat.MultiplyPoint(gazeOrigin);

                //Vector3 hit = Vector3.Normalize(pos + dir) / 2 + new Vector3(0.5f, 0.5f, 0);
                Vector3 hit = ((pos+dir) / -2.0f) + new Vector3(0.5f, 0.5f, 0); // MINUS 2 because the dot will move diametrically to the gaze direction

                //Vector3 response = Vector3.ProjectOnPlane((gazeOrigin + gazeForward)/2 + new Vector3(0.5f, 0.5f, 0), planeForward);
                //Debug.Log(hit);


                //if(varjoViewCamera.name == "Varjo Right Focus")
                //{
                //hit = Vector3.Normalize(varjoViewCamera.GetCamera().transform.InverseTransformVector(hit)) + new Vector3(0.5f, 0.5f, 0f);
                //}

                sheet.properties.SetVector("gaze", new Vector2(hit.x, hit.y));
                //sheet.properties.SetVector("gaze", new Vector2(response.x, response.y));
                //sheet.properties.SetVector("gaze", new Vector2(hit.x + offset.Item1, hit.y + offset.Item2));
                //sheet.properties.SetVector("gaze", new Vector2(response.x + offset.Item1, response.y + offset.Item2));
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
