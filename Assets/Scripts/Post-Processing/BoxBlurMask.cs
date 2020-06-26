using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(BoxBlurMaskRenderer), PostProcessEvent.AfterStack, "Impairment/BoxBlurMask")]
public sealed class BoxBlurMask : MaskSettings
{
}

public sealed class BoxBlurMaskRenderer : MaskRenderer
{
    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Impairment/BoxBlurMask"));
    }

    public override void SetEffectProperties()
    {
        sheet.properties.SetInt("_KernelSize1", globalSettings.kernelSize1);
        sheet.properties.SetInt("_KernelSize2", globalSettings.kernelSize2);
        sheet.properties.SetInt("_KernelSize3", globalSettings.kernelSize3);
    }
}
