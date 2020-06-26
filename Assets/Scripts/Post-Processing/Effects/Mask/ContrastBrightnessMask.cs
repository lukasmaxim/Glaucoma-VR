using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(ContrastBrightnessMaskRenderer), PostProcessEvent.AfterStack, "Impairment/ContrastBrightnessMask")]
public sealed class ContrastBrightnessMask : PostProcessMaskSettings
{
}

public sealed class ContrastBrightnessMaskRenderer : PostProcessMaskRenderer
{
    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Impairment/ContrastBrightnessMask"));
    }

    public override void SetEffectProperties()
    {
        sheet.properties.SetFloat("_ContrastMultiplier", maskSettings.contrastMultiplier);
        sheet.properties.SetFloat("_BrightnessModifier", maskSettings.brightnessModifier);
    }
}