// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class ColorBoostConfigPage : GameEffectConfigPage
    {
        const string key = "colorBoost";

        Checkbox enableCheckbox;
        HorizontalSlider radiusSlider;
        HorizontalSlider intensitySlider;
        HorizontalSlider dungeonScaleSlider;
        HorizontalSlider interiorScaleSlider;
        HorizontalSlider exteriorScaleSlider;
        HorizontalSlider dungeonFalloffSlider;

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("colorBoostTip"));

            // Enable toggle
            enableCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("enable"), ref pos);
            enableCheckbox.OnToggleState += EnableCheckbox_OnToggleState;

            // Radius slider
            radiusSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("radius"), 500, ref pos);
            radiusSlider.OnScroll += RadiusSlider_OnScroll;
            radiusSlider.SetIndicator(0.1f, 50.0f, DaggerfallUnity.Settings.ColorBoostRadius);
            StyleIndicator(radiusSlider);

            // Intensity slider
            intensitySlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("intensity"), 10, ref pos);
            intensitySlider.OnScroll += IntensitySlider_OnScroll;
            intensitySlider.SetIndicator(0.0f, 1.0f, DaggerfallUnity.Settings.ColorBoostIntensity);
            StyleIndicator(intensitySlider);

            // Dungeon Scale slider
            dungeonScaleSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("dungeonScale"), 80, ref pos);
            dungeonScaleSlider.OnScroll += DungeonScaleSlider_OnScroll;
            dungeonScaleSlider.SetIndicator(0.0f, 8.0f, DaggerfallUnity.Settings.ColorBoostDungeonScale);
            StyleIndicator(dungeonScaleSlider);

            // Interior Scale slider
            interiorScaleSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("interiorScale"), 80, ref pos);
            interiorScaleSlider.OnScroll += InteriorScaleSlider_OnScroll;
            interiorScaleSlider.SetIndicator(0.0f, 8.0f, DaggerfallUnity.Settings.ColorBoostInteriorScale);
            StyleIndicator(interiorScaleSlider);

            // Exterior Scale slider
            exteriorScaleSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("exteriorScale"), 80, ref pos);
            exteriorScaleSlider.OnScroll += ExteriorScaleSlider_OnScroll;
            exteriorScaleSlider.SetIndicator(0.0f, 8.0f, DaggerfallUnity.Settings.ColorBoostExteriorScale);
            StyleIndicator(exteriorScaleSlider);

            // Dungeon falloff
            dungeonFalloffSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("dungeonFalloff"), 80, ref pos);
            dungeonFalloffSlider.OnScroll += DungeonFalloffSlider_OnScroll;
            dungeonFalloffSlider.SetIndicator(0.0f, 8.0f, DaggerfallUnity.Settings.ColorBoostDungeonFalloff);
            StyleIndicator(dungeonFalloffSlider);
        }

        public override void ReadSettings()
        {
            enableCheckbox.IsChecked = DaggerfallUnity.Settings.ColorBoostEnable;
            radiusSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.ColorBoostRadius * 10);
            intensitySlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.ColorBoostIntensity * 10);
            dungeonScaleSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.ColorBoostDungeonScale * 10);
            interiorScaleSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.ColorBoostInteriorScale * 10);
            exteriorScaleSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.ColorBoostExteriorScale * 10);
            dungeonFalloffSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.ColorBoostDungeonFalloff * 10);
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.ColorBoost);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.ColorBoostEnable = false;
            DaggerfallUnity.Settings.ColorBoostRadius = 25.0f;
            DaggerfallUnity.Settings.ColorBoostIntensity = 1.0f;
            DaggerfallUnity.Settings.ColorBoostDungeonScale = 1.5f;
            DaggerfallUnity.Settings.ColorBoostInteriorScale = 0.5f;
            DaggerfallUnity.Settings.ColorBoostExteriorScale = 0.2f;
            DaggerfallUnity.Settings.ColorBoostDungeonFalloff = 0.0f;
        }

        private void EnableCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.ColorBoostEnable = enableCheckbox.IsChecked;
            DeploySettings();
        }

        private void RadiusSlider_OnScroll()
        {
            DaggerfallUnity.Settings.ColorBoostRadius = radiusSlider.ScrollIndex / 10f + 0.1f;
            DeploySettings();
        }

        private void IntensitySlider_OnScroll()
        {
            DaggerfallUnity.Settings.ColorBoostIntensity = intensitySlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void DungeonScaleSlider_OnScroll()
        {
            DaggerfallUnity.Settings.ColorBoostDungeonScale = dungeonScaleSlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void InteriorScaleSlider_OnScroll()
        {
            DaggerfallUnity.Settings.ColorBoostInteriorScale = interiorScaleSlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void ExteriorScaleSlider_OnScroll()
        {
            DaggerfallUnity.Settings.ColorBoostExteriorScale = exteriorScaleSlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void DungeonFalloffSlider_OnScroll()
        {
            DaggerfallUnity.Settings.ColorBoostDungeonFalloff = dungeonFalloffSlider.ScrollIndex / 10f;
            DeploySettings();
        }
    }
}