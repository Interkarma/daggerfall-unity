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
    public class VignetteConfigPage : GameEffectConfigPage
    {
        const string key = "vignette";

        Checkbox enableCheckbox;
        HorizontalSlider intensitySlider;
        HorizontalSlider smoothnessSlider;
        HorizontalSlider roundnessSlider;
        Checkbox roundedCheckbox;

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("vignetteTip"));

            // Enable toggle
            enableCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("enable"), ref pos);
            enableCheckbox.OnToggleState += EnableCheckbox_OnToggleState;

            // Intensity slider
            intensitySlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("intensity"), 100, ref pos);
            intensitySlider.OnScroll += IntensitySlider_OnScroll;
            intensitySlider.SetIndicator(0.0f, 1.0f, DaggerfallUnity.Settings.VignetteIntensity);
            StyleIndicator(intensitySlider);

            // Smoothness slider
            smoothnessSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("smoothness"), 100, ref pos);
            smoothnessSlider.OnScroll += SmoothnessSlider_OnScroll;
            smoothnessSlider.SetIndicator(0.0f, 1.0f, DaggerfallUnity.Settings.VignetteSmoothness);
            StyleIndicator(smoothnessSlider);

            // Roundness slider
            roundnessSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("roundness"), 100, ref pos);
            roundnessSlider.OnScroll += RoundnessSlider_OnScroll;
            roundnessSlider.SetIndicator(0.0f, 1.0f, DaggerfallUnity.Settings.VignetteRoundness);
            StyleIndicator(roundnessSlider);

            // Rounded toggle
            roundedCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("rounded"), ref pos);
            roundedCheckbox.OnToggleState += RoundedCheckbox_OnToggleState;
        }

        public override void ReadSettings()
        {
            enableCheckbox.IsChecked = DaggerfallUnity.Settings.VignetteEnable;
            intensitySlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.VignetteIntensity * 10);
            smoothnessSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.VignetteSmoothness * 10);
            roundnessSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.VignetteRoundness * 10);
            roundedCheckbox.IsChecked = DaggerfallUnity.Settings.VignetteRounded;
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.Vignette);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.VignetteEnable = false;
            DaggerfallUnity.Settings.VignetteIntensity = 0.6f;
            DaggerfallUnity.Settings.VignetteSmoothness = 0.3f;
            DaggerfallUnity.Settings.VignetteRoundness = 0.5f;
            DaggerfallUnity.Settings.VignetteRounded = false;
        }

        private void EnableCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.VignetteEnable = enableCheckbox.IsChecked;
            DeploySettings();
        }

        private void IntensitySlider_OnScroll()
        {
            DaggerfallUnity.Settings.VignetteIntensity = intensitySlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void SmoothnessSlider_OnScroll()
        {
            DaggerfallUnity.Settings.VignetteSmoothness = smoothnessSlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void RoundnessSlider_OnScroll()
        {
            DaggerfallUnity.Settings.VignetteRoundness = roundnessSlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void RoundedCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.VignetteRounded = roundedCheckbox.IsChecked;
            DeploySettings();
        }
    }
}