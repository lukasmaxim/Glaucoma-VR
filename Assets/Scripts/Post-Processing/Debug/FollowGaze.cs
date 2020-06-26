using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

[Serializable]
[PostProcess(typeof(FollowGazeRenderer), PostProcessEvent.AfterStack, "Debug/FollowGaze")]
public sealed class FollowGaze : PostProcessMaskSettings
{
}

public sealed class FollowGazeRenderer : PostProcessMaskRenderer
{
    public override void SetInitialEffectProperties()
    {
        sheet = context.propertySheets.Get(Shader.Find("Debug/FollowGaze"));
    }
    
    public override void SetEffectProperties()
    {
    }
}
