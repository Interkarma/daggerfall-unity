using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ColorBoostRenderer), PostProcessEvent.AfterStack, "Daggerfall/PostProcess/ColorBoost")]
public sealed class ColorBoost : PostProcessEffectSettings
{
    [Range(0.1f, 1f), Tooltip("Radius")]
    public FloatParameter radius = new FloatParameter { value = 0.2f };

    [Range(0f, 10f), Tooltip("Dungeon boost intensity")]
    public FloatParameter dungeonBoostIntensity = new FloatParameter { value = 3.0f };
}

public sealed class ColorBoostRenderer : PostProcessEffectRenderer<ColorBoost>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Daggerfall/PostProcess/ColorBoost"));
        sheet.properties.SetFloat("_Radius", settings.radius);
        sheet.properties.SetFloat("_Intensity", settings.dungeonBoostIntensity);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}