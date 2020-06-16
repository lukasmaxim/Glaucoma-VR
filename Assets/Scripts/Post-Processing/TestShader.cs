using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(TestShaderRenderer), PostProcessEvent.AfterStack, "Custom/BoxBlur")]
public sealed class TestShader : PostProcessEffectSettings
{
}

public sealed class TestShaderRenderer : PostProcessEffectRenderer<TestShader>
{


    // called at the end of each frame's rendering pipe
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/BoxBlur"));
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
