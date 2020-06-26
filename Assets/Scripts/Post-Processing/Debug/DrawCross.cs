using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(DrawCrossRenderer), PostProcessEvent.AfterStack, "Debug/DrawCross")]
public sealed class DrawCross : PostProcessMaskSettings
{
}

public sealed class DrawCrossRenderer : PostProcessMaskRenderer
{
    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Debug/DrawCross"));
    }

    public override void SetEffectProperties()
    {
    }
}
