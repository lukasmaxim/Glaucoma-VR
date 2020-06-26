using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(MaskCoverageRenderer), PostProcessEvent.AfterStack, "Debug/MaskCoverage")]
public sealed class MaskCoverage : MaskSettings
{
}

public sealed class MaskCoverageRenderer : MaskRenderer
{
    public override void SetEffectProperties()
    {
        sheet.properties.SetColor("_OverlayColor", globalSettings.overlayColor);
    }

    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Debug/MaskCoverage"));
        sheet.properties.SetFloat("_AlphaCutoff", globalSettings.alphaCutoff); 
    }
}
