using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(DrawCrossRenderer), PostProcessEvent.AfterStack, "Debug/DrawCross")]
public sealed class DrawCross : PostProcessEffectSettings
{
    [Range(0f, 0.1f), Tooltip("Left x offset.")]
    public FloatParameter offsetLeftX = new FloatParameter { value = 0.0925f };
    [Range(0f, 0.1f), Tooltip("Right x offset.")]

    public FloatParameter offsetRightX = new FloatParameter { value = 0.0865f };
    [Range(-0.1f, 0f), Tooltip("y offset.")]
    public FloatParameter offsetY = new FloatParameter { value = 0.0034f };
}

public sealed class DrawCrossRenderer : PostProcessEffectRenderer<DrawCross>
{
    public override void Render(PostProcessRenderContext context)
    {
        switch (context.camera.name)
        {
            case "Varjo Left Context":
                setOffset(context, new Tuple<float, float>(settings.offsetLeftX, settings.offsetY));
                break;
            case "Varjo Left Focus":
                setOffset(context, new Tuple<float, float>(0.0f, 0.0f));
                break;
            case "Varjo Right Context":
                setOffset(context, new Tuple<float, float>(-settings.offsetRightX, settings.offsetY));
                break;
            case "Varjo Right Focus":
                setOffset(context, new Tuple<float, float>(0.0f, 0.0f));
                break;
        }
    }

    void setOffset(PostProcessRenderContext context, Tuple<float, float> offset)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Debug/DrawCross"));
        sheet.properties.SetFloat("offsetContextX", offset.Item1);
        sheet.properties.SetFloat("offsetContextY", offset.Item2);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
