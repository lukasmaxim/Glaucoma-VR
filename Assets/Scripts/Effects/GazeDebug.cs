using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(GazeDebugRenderer), PostProcessEvent.AfterStack, "Custom/FollowGaze")]
public sealed class GazeDebug : PostProcessEffectSettings
{
}

public sealed class GazeDebugRenderer : PostProcessEffectRenderer<GazeDebug>
{
    bool debug = false;

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/FollowGaze"));

        float distance =  GameObject.Find("Varjo Left Focus").GetComponent<Camera>().focalLength;
        Debug.Log("focal length: " + distance);

        var vvc = GameObject.Find("Varjo Left Focus").GetComponent<VarjoViewCamera>();
        if(vvc)
        {
            Debug.Log("other focal length: " + vvc.t);
        }
        

        if (VarjoPlugin.GetGaze().status == VarjoPlugin.GazeStatus.VALID)
        {
            Vector3 gazeOrigin = Double3ToVector3(VarjoPlugin.GetGaze().gaze.position);
            Vector3 gazeForward = Double3ToVector3(VarjoPlugin.GetGaze().gaze.forward);

            Vector3 camForward = GameObject.Find("Varjo Left Focus").GetComponent<Camera>().transform.forward;

            float denominator = Vector3.Dot(gazeForward, camForward);

            if (Math.Abs(denominator) > 0.001f)
            {
                float t = -(6.3f + Vector3.Dot(gazeOrigin, camForward)) / denominator;
                Vector3 hit = gazeOrigin + t * gazeForward;

                sheet.properties.SetVector("gaze", new Vector2(-hit.x/2+0.5f, -hit.y/2+0.5f));
            }
            else
            {
                sheet.properties.SetVector("gaze", new Vector2(0.5f, 0.5f));
            }
        }
        else
        {
            sheet.properties.SetVector("gaze", new Vector2(0.5f, 0.5f));
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
