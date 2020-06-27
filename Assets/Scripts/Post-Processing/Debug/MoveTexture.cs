using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(MoveTextureRenderer), PostProcessEvent.AfterStack, "Debug/MoveTexture")]
public sealed class MoveTexture : PostProcessMaskSettings
{
}

public sealed class MoveTextureRenderer : PostProcessMaskRenderer
{
    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Debug/MoveTexture"));
    }
    
    public override void SetEffectProperties()
    {
    }

    // special case: override since we don't want the eye textures here
    public override void SetEyeTextures()
    {
    }
}
