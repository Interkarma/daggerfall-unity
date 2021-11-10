using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DaggerfallWorkshop.Game;

[Serializable]
[PostProcess(typeof(ColorBoostRenderer), PostProcessEvent.AfterStack, "Daggerfall/PostProcess/ColorBoost")]
public sealed class ColorBoost : PostProcessEffectSettings
{
    [Range(0.1f, 50f), Tooltip("Radius")]
    public FloatParameter radius = new FloatParameter { value = 25.0f };

    [Range(0f, 1f), Tooltip("Global Intensity")]
    public FloatParameter globalIntensity = new FloatParameter { value = 1.0f };

    [Range(0f, 1f), Tooltip("Dungeon Scale")]
    public FloatParameter dungeonScale = new FloatParameter { value = 0.5f };

    [Range(0f, 1f), Tooltip("Exterior Scale")]
    public FloatParameter exteriorScale = new FloatParameter { value = 0.1f };

    [Range(0f, 1f), Tooltip("Interior Scale")]
    public FloatParameter interiorScale = new FloatParameter { value = 0.1f };
}

public sealed class ColorBoostRenderer : PostProcessEffectRenderer<ColorBoost>
{
    public override void Render(PostProcessRenderContext context)
    {
        // Get scale based on dungeon/exterior/interior
        float localScale = 0;
        if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
            localScale = settings.dungeonScale;
        else if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
            localScale = settings.interiorScale;
        else
            localScale = settings.exteriorScale;

        var sheet = context.propertySheets.Get(Shader.Find("Daggerfall/PostProcess/ColorBoost"));
        sheet.properties.SetFloat("_Radius", settings.radius);
        sheet.properties.SetFloat("_Intensity", settings.globalIntensity * localScale);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}