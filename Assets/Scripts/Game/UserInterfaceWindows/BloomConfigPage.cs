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
    public class BloomConfigPage : GameEffectConfigPage
    {
        const string key = "bloom";

        Checkbox enableCheckbox;
        HorizontalSlider intensitySlider;
        HorizontalSlider thresholdSlider;
        HorizontalSlider diffusionSlider;
        Checkbox fastModeCheckbox;

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("bloomTip"));

            // Enable toggle
            enableCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("enable"), ref pos);
            enableCheckbox.OnToggleState += EnableCheckbox_OnToggleState;

            // Intensity slider
            intensitySlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("intensity"), 500, ref pos);
            intensitySlider.OnScroll += IntensitySlider_OnScroll;
            intensitySlider.SetIndicator(0.0f, 50.0f, DaggerfallUnity.Settings.BloomIntensity);
            StyleIndicator(intensitySlider);

            // Threshold slider
            thresholdSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("threshold"), 100, ref pos);
            thresholdSlider.OnScroll += ThresholdSlider_OnScroll; ;
            thresholdSlider.SetIndicator(0.1f, 10.0f, DaggerfallUnity.Settings.BloomThreshold);
            StyleIndicator(thresholdSlider);

            // Diffusion slider
            diffusionSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("diffusion"), 100, ref pos);
            diffusionSlider.SetIndicator(1.0f, 10.0f, DaggerfallUnity.Settings.BloomDiffusion);
            diffusionSlider.OnScroll += DiffusionSlider_OnScroll;
            StyleIndicator(diffusionSlider);

            // Fast mode
            fastModeCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("bloomFastMode"), ref pos);
            fastModeCheckbox.OnToggleState += FastModeCheckbox_OnToggleState;
        }

        public override void ReadSettings()
        {
            enableCheckbox.IsChecked = DaggerfallUnity.Settings.BloomEnable;
            intensitySlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.BloomIntensity * 10);
            thresholdSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.BloomThreshold * 10);
            diffusionSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.BloomDiffusion * 10);
            fastModeCheckbox.IsChecked = DaggerfallUnity.Settings.BloomFastMode;
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.Bloom);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.BloomEnable = false;
            DaggerfallUnity.Settings.BloomIntensity = 5;
            DaggerfallUnity.Settings.BloomThreshold = 0.7f;
            DaggerfallUnity.Settings.BloomDiffusion = 6;
            DaggerfallUnity.Settings.BloomFastMode = false;
        }

        private void EnableCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.BloomEnable = enableCheckbox.IsChecked;
            DeploySettings();
        }

        private void IntensitySlider_OnScroll()
        {
            DaggerfallUnity.Settings.BloomIntensity = intensitySlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void ThresholdSlider_OnScroll()
        {
            DaggerfallUnity.Settings.BloomThreshold = thresholdSlider.ScrollIndex / 10f + 0.1f;
            DeploySettings();
        }

        private void DiffusionSlider_OnScroll()
        {
            DaggerfallUnity.Settings.BloomDiffusion = diffusionSlider.ScrollIndex / 10f + 1f;
            DeploySettings();
        }

        private void FastModeCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.BloomFastMode = fastModeCheckbox.IsChecked;
            DeploySettings();
        }
    }
}