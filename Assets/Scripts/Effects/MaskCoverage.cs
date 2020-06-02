﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(MaskCoverageRenderer), PostProcessEvent.AfterStack, "Custom/MaskCoverage")]
public sealed class MaskCoverage : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Coverage effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
}

public sealed class MaskCoverageRenderer : PostProcessEffectRenderer<MaskCoverage>
{
    List<VarjoPlugin.GazeData> dataSinceLastUpdate;

    public override void Render(PostProcessRenderContext context)
    {
        if (VarjoPlugin.GetGaze().status == VarjoPlugin.GazeStatus.VALID)
        {
            // Get all gaze data since last update
            dataSinceLastUpdate = VarjoPlugin.GetGazeList();
            foreach (var data in dataSinceLastUpdate)
            {
                Debug.Log(Double3ToString(data.gaze.forward));
            }
        }

        var sheet = context.propertySheets.Get(Shader.Find("Custom/MaskCoverage"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    public static string Double3ToString(double[] doubles)
    {
        return doubles[0].ToString() + ", " + doubles[1].ToString() + ", " + doubles[2].ToString();
    }
}