using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(BoxBlurMaskRenderer), PostProcessEvent.AfterStack, "Impairment/BoxBlurMask")]
public sealed class BoxBlurMask : PostProcessMaskSettings
{
}

public sealed class BoxBlurMaskRenderer : PostProcessMaskRenderer
{
    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Impairment/BoxBlurMask"));
    }

    public override void SetEffectProperties()
    {
        sheet.properties.SetInt("_KernelSize1", maskSettings.kernelSize1);
        sheet.properties.SetInt("_KernelSize2", maskSettings.kernelSize2);
        sheet.properties.SetInt("_KernelSize3", maskSettings.kernelSize3);
    }
}
