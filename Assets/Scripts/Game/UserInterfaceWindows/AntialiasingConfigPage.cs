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
    public class AntialiasingConfigPage : GameEffectConfigPage
    {
        const string key = "antialiasing";

        HorizontalSlider antialiasingMethodSlider;
        Checkbox fxaaFastModeCheckbox;
        HorizontalSlider smaaQualitySlider;
        HorizontalSlider taaSharpnessSlider;

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("antialiasingTip"));

            // Method slider
            string[] antiAliasingMethods = new string[]
            {
                TextManager.Instance.GetLocalizedText("none"),
                TextManager.Instance.GetLocalizedText("fxaa"),
                TextManager.Instance.GetLocalizedText("smaa"),
                TextManager.Instance.GetLocalizedText("taa")
            };
            antialiasingMethodSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("method"), antiAliasingMethods.Length, ref pos);
            antialiasingMethodSlider.OnScroll += AntialiasingMethodSlider_OnScroll;
            antialiasingMethodSlider.SetIndicator(antiAliasingMethods, DaggerfallUnity.Settings.AntialiasingMethod);
            StyleIndicator(antialiasingMethodSlider);

            // FXAA Fast Mode toggle
            fxaaFastModeCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("fxaaFastMode"), ref pos);
            fxaaFastModeCheckbox.OnToggleState += FxaaFastMostCheckbox_OnToggleState;

            // SMAA Quality slider
            string[] smaaQuality = new string[]
            {
                TextManager.Instance.GetLocalizedText("low"),
                TextManager.Instance.GetLocalizedText("medium"),
                TextManager.Instance.GetLocalizedText("high")
            };
            smaaQualitySlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("smaaQuality"), smaaQuality.Length, ref pos);
            smaaQualitySlider.OnScroll += SmaaQualitySlider_OnScroll;
            smaaQualitySlider.SetIndicator(smaaQuality, DaggerfallUnity.Settings.AntialiasingSMAAQuality);
            StyleIndicator(smaaQualitySlider);

            // TAA Sharpness slider
            taaSharpnessSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("taaSharpness"), 30, ref pos);
            taaSharpnessSlider.OnScroll += TaaSharpnessSlider_OnScroll;
            taaSharpnessSlider.SetIndicator(0.0f, 3.0f, DaggerfallUnity.Settings.AntialiasingTAASharpness);
            StyleIndicator(taaSharpnessSlider);
        }

        public override void ReadSettings()
        {
            antialiasingMethodSlider.ScrollIndex = DaggerfallUnity.Settings.AntialiasingMethod;
            fxaaFastModeCheckbox.IsChecked = DaggerfallUnity.Settings.AntialiasingFXAAFastMode;
            smaaQualitySlider.ScrollIndex = DaggerfallUnity.Settings.AntialiasingSMAAQuality;
            taaSharpnessSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.AntialiasingTAASharpness * 10);
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.Antialiasing);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.AntialiasingMethod = (int)AntiAliasingMethods.None;
            DaggerfallUnity.Settings.AntialiasingFXAAFastMode = false;
            DaggerfallUnity.Settings.AntialiasingSMAAQuality = 1;
            DaggerfallUnity.Settings.AntialiasingTAASharpness = 0.3f;
            DeploySettings();
        }

        private void AntialiasingMethodSlider_OnScroll()
        {
            DaggerfallUnity.Settings.AntialiasingMethod = antialiasingMethodSlider.ScrollIndex;
            DeploySettings();
        }

        private void FxaaFastMostCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.AntialiasingFXAAFastMode = fxaaFastModeCheckbox.IsChecked;
            DeploySettings();
        }

        private void SmaaQualitySlider_OnScroll()
        {
            DaggerfallUnity.Settings.AntialiasingSMAAQuality = smaaQualitySlider.ScrollIndex;
            DeploySettings();
        }

        private void TaaSharpnessSlider_OnScroll()
        {
            DaggerfallUnity.Settings.AntialiasingTAASharpness = taaSharpnessSlider.ScrollIndex / 10f;
            DeploySettings();
        }
    }
}