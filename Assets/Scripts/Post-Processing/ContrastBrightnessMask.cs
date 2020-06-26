using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(ContrastBrightnessMaskRenderer), PostProcessEvent.AfterStack, "Impairment/ContrastBrightnessMask")]
public sealed class ContrastBrightnessMask : MaskSettings
{
}

public sealed class ContrastBrightnessMaskRenderer : MaskRenderer
{
    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Impairment/ContrastBrightnessMask"));
    }

    public override void SetEffectProperties()
    {
        sheet.properties.SetFloat("_ContrastMultiplier", globalSettings.contrastMultiplier);
        sheet.properties.SetFloat("_BrightnessModifier", globalSettings.brightnessModifier);
    }
}