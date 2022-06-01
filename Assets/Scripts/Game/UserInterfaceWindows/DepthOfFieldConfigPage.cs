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
    public class DepthOfFieldConfigPage : GameEffectConfigPage
    {
        const string key = "depthOfField";

        Checkbox enableCheckbox;
        HorizontalSlider focusDistanceSlider;
        HorizontalSlider apertureSlider;
        HorizontalSlider focalLengthSlider;
        HorizontalSlider maxBlurSizeSlider;

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("depthOfFieldTip"));

            // Enable toggle
            enableCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("enable"), ref pos);
            enableCheckbox.OnToggleState += EnableCheckbox_OnToggleState;

            // Focus Distance slider
            focusDistanceSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("focusDistance"), 1000, ref pos);
            focusDistanceSlider.OnScroll += FocusDistanceSlider_OnScroll;
            focusDistanceSlider.SetIndicator(0.1f, 100.0f, DaggerfallUnity.Settings.DepthOfFieldFocusDistance);
            StyleIndicator(focusDistanceSlider);

            // Aperture slider
            apertureSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("aperture"), 320, ref pos);
            apertureSlider.OnScroll += ApertureSlider_OnScroll;
            apertureSlider.SetIndicator(0.1f, 32.0f, DaggerfallUnity.Settings.DepthOfFieldAperture);
            StyleIndicator(apertureSlider);

            // Focal Length slider
            focalLengthSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("focalLength"), 300, ref pos);
            focalLengthSlider.OnScroll += FocalLengthSlider_OnScroll;
            focalLengthSlider.SetIndicator(1, 300, DaggerfallUnity.Settings.DepthOfFieldFocalLength);
            StyleIndicator(focalLengthSlider);

            // Max Blur Size toggle
            string[] blurSizes = new string[]
            {
                TextManager.Instance.GetLocalizedText("small"),
                TextManager.Instance.GetLocalizedText("medium"),
                TextManager.Instance.GetLocalizedText("large"),
                TextManager.Instance.GetLocalizedText("veryLarge"),
            };
            maxBlurSizeSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("maxBlurSize"), blurSizes.Length, ref pos);
            maxBlurSizeSlider.OnScroll += MaxBlurSizeSlider_OnToggleState;
            maxBlurSizeSlider.SetIndicator(blurSizes, DaggerfallUnity.Settings.DepthOfFieldMaxBlurSize);
            StyleIndicator(maxBlurSizeSlider);
        }

        public override void ReadSettings()
        {
            enableCheckbox.IsChecked = DaggerfallUnity.Settings.DepthOfFieldEnable;
            focusDistanceSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.DepthOfFieldFocusDistance * 10);
            apertureSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.DepthOfFieldAperture * 10);
            focalLengthSlider.Value = DaggerfallUnity.Settings.DepthOfFieldFocalLength;
            maxBlurSizeSlider.Value = DaggerfallUnity.Settings.DepthOfFieldMaxBlurSize;
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.DepthOfField);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.DepthOfFieldEnable = false;
            DaggerfallUnity.Settings.DepthOfFieldFocusDistance = 3.8f;
            DaggerfallUnity.Settings.DepthOfFieldAperture = 5.0f;
            DaggerfallUnity.Settings.DepthOfFieldFocalLength = 50;
            DaggerfallUnity.Settings.DepthOfFieldMaxBlurSize = 1;
        }

        private void EnableCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.DepthOfFieldEnable = enableCheckbox.IsChecked;
            DeploySettings();
        }

        private void FocusDistanceSlider_OnScroll()
        {
            DaggerfallUnity.Settings.DepthOfFieldFocusDistance = focusDistanceSlider.ScrollIndex / 10f + 0.1f;
            DeploySettings();
        }

        private void ApertureSlider_OnScroll()
        {
            DaggerfallUnity.Settings.DepthOfFieldAperture = apertureSlider.ScrollIndex / 10f + 0.1f;
            DeploySettings();
        }

        private void FocalLengthSlider_OnScroll()
        {
            DaggerfallUnity.Settings.DepthOfFieldFocalLength = focalLengthSlider.Value;
            DeploySettings();
        }

        private void MaxBlurSizeSlider_OnToggleState()
        {
            DaggerfallUnity.Settings.DepthOfFieldMaxBlurSize = maxBlurSizeSlider.ScrollIndex;
            DeploySettings();
        }
    }
}