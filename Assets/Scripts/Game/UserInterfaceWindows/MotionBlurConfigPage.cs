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
    public class MotionBlurConfigPage : GameEffectConfigPage
    {
        const string key = "motionBlur";

        Checkbox enableCheckbox;
        HorizontalSlider shutterAngleSlider;
        HorizontalSlider sampleCountSlider;

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("motionBlurTip"));

            // Enable toggle
            enableCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("enable"), ref pos);
            enableCheckbox.OnToggleState += EnableCheckbox_OnToggleState;

            // Shutter angle slider
            shutterAngleSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("shutterAngle"), 360, ref pos);
            shutterAngleSlider.OnScroll += ShutterAngleSlider_OnScroll;
            shutterAngleSlider.SetIndicator(0, 360, DaggerfallUnity.Settings.MotionBlurShutterAngle);
            StyleIndicator(shutterAngleSlider);

            // Sample count slider
            sampleCountSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("sampleCount"), 32, ref pos);
            sampleCountSlider.OnScroll += SampleCountSlider_OnScroll;
            sampleCountSlider.SetIndicator(4, 32, DaggerfallUnity.Settings.MotionBlurSampleCount);
            StyleIndicator(sampleCountSlider);
        }

        public override void ReadSettings()
        {
            enableCheckbox.IsChecked = DaggerfallUnity.Settings.MotionBlurEnable;
            shutterAngleSlider.Value = DaggerfallUnity.Settings.MotionBlurShutterAngle;
            sampleCountSlider.Value = DaggerfallUnity.Settings.MotionBlurSampleCount;
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.MotionBlur);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.MotionBlurEnable = false;
            DaggerfallUnity.Settings.MotionBlurShutterAngle = 270;
            DaggerfallUnity.Settings.MotionBlurSampleCount = 8;
        }

        private void EnableCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.MotionBlurEnable = enableCheckbox.IsChecked;
            DeploySettings();
        }

        private void ShutterAngleSlider_OnScroll()
        {
            DaggerfallUnity.Settings.MotionBlurShutterAngle = shutterAngleSlider.ScrollIndex;
            DeploySettings();
        }

        private void SampleCountSlider_OnScroll()
        {
            DaggerfallUnity.Settings.MotionBlurSampleCount = sampleCountSlider.ScrollIndex + 4;
            DeploySettings();
        }
    }
}