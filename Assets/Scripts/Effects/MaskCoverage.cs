using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(MaskCoverageRenderer), PostProcessEvent.AfterStack, "Custom/MaskCoverage")]
public sealed class MaskCoverage : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Coverage effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
}

public sealed class MaskCoverageRenderer : PostProcessEffectRenderer<MaskCoverage>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/MaskCoverage"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
