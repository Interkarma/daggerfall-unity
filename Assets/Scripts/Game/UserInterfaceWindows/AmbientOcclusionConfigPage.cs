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
    public class AmbientOcclusionConfigPage : GameEffectConfigPage
    {
        const string key = "ambientOcclusion";

        Checkbox enableCheckbox;
        HorizontalSlider aoMethodSlider;
        HorizontalSlider intensitySlider;
        HorizontalSlider thicknessSlider;
        HorizontalSlider radiusSlider;
        HorizontalSlider qualitySlider;
        Panel method0Panel = new Panel();
        Panel method1Panel = new Panel();

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("ambientOcclusionTip"));

            // Enable toggle
            enableCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("enable"), ref pos);
            enableCheckbox.OnToggleState += EnableCheckbox_OnToggleState;

            // Method slider
            string[] aoMethods = GetSupportedMethods();
            aoMethodSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("method"), aoMethods.Length, ref pos);
            aoMethodSlider.OnScroll += AoMethodSlider_OnScroll;
            aoMethodSlider.SetIndicator(aoMethods, DaggerfallUnity.Settings.AmbientOcclusionMethod);
            StyleIndicator(aoMethodSlider);

            // Intensity slider
            intensitySlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("intensity"), 40, ref pos);
            intensitySlider.OnScroll += IntensitySlider_OnScroll;
            intensitySlider.SetIndicator(0.0f, 4.0f, DaggerfallUnity.Settings.AmbientOcclusionIntensity);
            StyleIndicator(intensitySlider);

            // Setup two panels to hold settings unique to each method
            method0Panel.Position = method1Panel.Position = pos;
            parent.Components.Add(method0Panel);
            parent.Components.Add(method1Panel);

            // Method0 - Radius slider
            pos = Vector2.zero;
            radiusSlider = AddSlider(method0Panel, TextManager.Instance.GetLocalizedText("radius"), 20, ref pos);
            radiusSlider.OnScroll += RadiusSlider_OnScroll; ;
            radiusSlider.SetIndicator(0.0f, 2.0f, DaggerfallUnity.Settings.AmbientOcclusionRadius);
            StyleIndicator(radiusSlider);

            // Method0 - Quality slider
            string[] qualityLevels = new string[]
            {
                TextManager.Instance.GetLocalizedText("lowest"),
                TextManager.Instance.GetLocalizedText("low"),
                TextManager.Instance.GetLocalizedText("medium"),
                TextManager.Instance.GetLocalizedText("high"),
                TextManager.Instance.GetLocalizedText("ultra")
            };
            qualitySlider = AddSlider(method0Panel, TextManager.Instance.GetLocalizedText("quality"), aoMethods.Length, ref pos);
            qualitySlider.OnScroll += QualitySlider_OnScroll;
            qualitySlider.SetIndicator(qualityLevels, DaggerfallUnity.Settings.AmbientOcclusionQuality);
            StyleIndicator(qualitySlider);

            // Method1 - Thickness slider
            pos = Vector2.zero;
            thicknessSlider = AddSlider(method1Panel, TextManager.Instance.GetLocalizedText("thickness"), 100, ref pos);
            thicknessSlider.SetIndicator(1.0f, 10.0f, DaggerfallUnity.Settings.AmbientOcclusionThickness);
            thicknessSlider.OnScroll += ThicknessSlider_OnScroll;
            StyleIndicator(thicknessSlider);
        }

        public override void ReadSettings()
        {
            enableCheckbox.IsChecked = DaggerfallUnity.Settings.AmbientOcclusionEnable;
            aoMethodSlider.ScrollIndex = SystemInfo.graphicsShaderLevel >= 45 ? DaggerfallUnity.Settings.AmbientOcclusionMethod : 0;
            intensitySlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.AmbientOcclusionIntensity * 10);
            thicknessSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.AmbientOcclusionThickness * 10);
            radiusSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.AmbientOcclusionRadius * 10);
            qualitySlider.ScrollIndex = DaggerfallUnity.Settings.AmbientOcclusionQuality;
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.AmbientOcclusion);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.AmbientOcclusionEnable = false;
            DaggerfallUnity.Settings.AmbientOcclusionMethod = 0;
            DaggerfallUnity.Settings.AmbientOcclusionIntensity = 1.2f;
            DaggerfallUnity.Settings.AmbientOcclusionThickness = 1.0f;
            DaggerfallUnity.Settings.AmbientOcclusionRadius = 0.3f;
            DaggerfallUnity.Settings.AmbientOcclusionQuality = 2;
        }

        string[] GetSupportedMethods()
        {
            string[] methods;
            if (SystemInfo.graphicsShaderLevel >= 45)
            {
                methods = new string[]
                {
                    TextManager.Instance.GetLocalizedText("scalableAmbient"),
                    TextManager.Instance.GetLocalizedText("multiScaleVolumetric")
                };
            }
            else
            {
                methods = new string[]
                {
                    TextManager.Instance.GetLocalizedText("scalableAmbient"),
                };
            }

            return methods;
        }

        private void EnableCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.AmbientOcclusionEnable = enableCheckbox.IsChecked;
            DeploySettings();
        }

        private void AoMethodSlider_OnScroll()
        {
            // Toggle panels based on method
            switch (aoMethodSlider.ScrollIndex)
            {
                case 0:
                    method0Panel.Enabled = true;
                    method1Panel.Enabled = false;
                break;
                case 1:
                    method0Panel.Enabled = false;
                    method1Panel.Enabled = true;
                    break;
            }

            DaggerfallUnity.Settings.AmbientOcclusionMethod = aoMethodSlider.ScrollIndex;
            DeploySettings();
        }

        private void IntensitySlider_OnScroll()
        {
            DaggerfallUnity.Settings.AmbientOcclusionIntensity = intensitySlider.ScrollIndex / 10f;
            DeploySettings();
        }

        private void QualitySlider_OnScroll()
        {
            DaggerfallUnity.Settings.AmbientOcclusionQuality = qualitySlider.ScrollIndex;
            DeploySettings();
        }

        private void ThicknessSlider_OnScroll()
        {
            DaggerfallUnity.Settings.AmbientOcclusionThickness = thicknessSlider.ScrollIndex / 10f + 1f;
            DeploySettings();
        }

        private void RadiusSlider_OnScroll()
        {
            DaggerfallUnity.Settings.AmbientOcclusionRadius = radiusSlider.ScrollIndex / 10f;
            DeploySettings();
        }
    }
}