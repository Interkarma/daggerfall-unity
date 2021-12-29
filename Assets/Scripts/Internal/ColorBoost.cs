using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DaggerfallWorkshop.Game;

[Serializable]
[PostProcess(typeof(ColorBoostRenderer), PostProcessEvent.BeforeStack, "Daggerfall/PostProcess/ColorBoost")]
public sealed class ColorBoost : PostProcessEffectSettings
{
    [Range(0.1f, 50f), Tooltip("Radius")]
    public FloatParameter radius = new FloatParameter { value = 25.0f };

    [Range(0f, 1f), Tooltip("Global Intensity")]
    public FloatParameter globalIntensity = new FloatParameter { value = 1.0f };

    [Range(0f, 8f), Tooltip("Dungeon Scale")]
    public FloatParameter dungeonScale = new FloatParameter { value = 1.5f };

    [Range(0f, 8f), Tooltip("Exterior Scale")]
    public FloatParameter exteriorScale = new FloatParameter { value = 0.2f };

    [Range(0f, 8f), Tooltip("Interior Scale")]
    public FloatParameter interiorScale = new FloatParameter { value = 0.5f };

    [Range(0f, 8f), Tooltip("Dungeon Falloff")]
    public FloatParameter dungeonFalloff = new FloatParameter { value = 0.0f };
}

public sealed class ColorBoostRenderer : PostProcessEffectRenderer<ColorBoost>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Daggerfall/PostProcess/ColorBoost"));

        // Get scale based on dungeon/exterior/interior
        float localScale = 0;
        if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
        {
            localScale = settings.dungeonScale;
            sheet.properties.SetFloat("_DungeonFalloffIntensity", settings.dungeonFalloff);
        }
        else if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
        {
            localScale = settings.interiorScale;
            sheet.properties.SetFloat("_DungeonFalloffIntensity", 0);
        }
        else
        {
            localScale = settings.exteriorScale;
            sheet.properties.SetFloat("_DungeonFalloffIntensity", 0);
        }

        sheet.properties.SetFloat("_Radius", settings.radius);
        sheet.properties.SetFloat("_Intensity", settings.globalIntensity * localScale);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}