// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
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
    public class RetroModeConfigPage : GameEffectConfigPage
    {
        const string key = "retroMode";

        HorizontalSlider modeSlider;
        HorizontalSlider postProcessSlider;

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("retroModeTip"));

            // Mode slider
            string[] modes = new string[]
            {
                TextManager.Instance.GetLocalizedText("retroModeOff"),
                TextManager.Instance.GetLocalizedText("retroMode320x200"),
                TextManager.Instance.GetLocalizedText("retroMode640x400"),
            };
            modeSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("mode"), modes.Length, ref pos);
            modeSlider.OnScroll += ModeSlider_OnScroll;
            modeSlider.SetIndicator(modes, DaggerfallUnity.Settings.RetroRenderingMode);
            StyleIndicator(modeSlider);

            // PostProcess Slider
            string[] postProcessModes = new string[]
            {
                TextManager.Instance.GetLocalizedText("off"),
                TextManager.Instance.GetLocalizedText("posterizationFull"),
                TextManager.Instance.GetLocalizedText("posterizationMinusSky"),
                TextManager.Instance.GetLocalizedText("palettizationFull"),
                TextManager.Instance.GetLocalizedText("palettizationMinusSky"),
            };
            postProcessSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("postProcess"), postProcessModes.Length, ref pos);
            postProcessSlider.OnScroll += PostProcessSlider_OnScroll;
            postProcessSlider.SetIndicator(postProcessModes, DaggerfallUnity.Settings.PostProcessingInRetroMode);
            StyleIndicator(postProcessSlider);
        }

        public override void ReadSettings()
        {
            modeSlider.ScrollIndex = DaggerfallUnity.Settings.RetroRenderingMode;
            postProcessSlider.ScrollIndex = DaggerfallUnity.Settings.PostProcessingInRetroMode;
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.RetroMode);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.RetroRenderingMode = 0;
            DaggerfallUnity.Settings.PostProcessingInRetroMode = 0;
        }

        private void ModeSlider_OnScroll()
        {
            DaggerfallUnity.Settings.RetroRenderingMode = modeSlider.ScrollIndex;
            DeploySettings();
        }

        private void PostProcessSlider_OnScroll()
        {
            DaggerfallUnity.Settings.PostProcessingInRetroMode = postProcessSlider.ScrollIndex;
            DeploySettings();
        }
    }
}