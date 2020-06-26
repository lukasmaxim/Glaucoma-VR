using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(BoxBlurRenderer), PostProcessEvent.AfterStack, "Custom/BoxBlur")]
public sealed class BoxBlur : PostProcessEffectSettings
{
    [Range(1f, 50f)]
    public FloatParameter kernelSize = new FloatParameter { value = 10.0f };
}

public sealed class BoxBlurRenderer : PostProcessEffectRenderer<BoxBlur>
{
    PropertySheet sheet;

    // called at the end of each frame's rendering pipe
    public override void Render(PostProcessRenderContext context)
    {
        this.sheet = context.propertySheets.Get(Shader.Find("Custom/BoxBlur"));
        sheet.properties.SetFloat("_KernelSize", settings.kernelSize);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
