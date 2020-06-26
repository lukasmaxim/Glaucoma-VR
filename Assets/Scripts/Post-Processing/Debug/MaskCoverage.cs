using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(MaskCoverageRenderer), PostProcessEvent.AfterStack, "Debug/MaskCoverage")]
public sealed class MaskCoverage : PostProcessMaskSettings
{
}

public sealed class MaskCoverageRenderer : PostProcessMaskRenderer
{
    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Debug/MaskCoverage"));
    }

    public override void SetEffectProperties()
    {
        sheet.properties.SetColor("_OverlayColor", maskSettings.overlayColor);
        sheet.properties.SetFloat("_AlphaCutoff", maskSettings.alphaCutoff); 
    }
}
