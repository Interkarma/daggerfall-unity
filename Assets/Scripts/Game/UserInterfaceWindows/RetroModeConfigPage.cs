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
    public class RetroModeConfigPage : GameEffectConfigPage
    {
        const string key = "retroMode";

        HorizontalSlider modeSlider;
        HorizontalSlider postProcessSlider;
        Checkbox aspectCorrectionOff;
        Checkbox aspectCorrectionFourThree;
        Checkbox aspectCorrectionSixteenTen;

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

            // Aspect Correction Checkboxes
            // Not using a slider as the sudden rescale in UI can cause aspect to bounce back and forth based on mouse position while dragging slider thumb
            AddLabel(parent, TextManager.Instance.GetLocalizedText("retroModeAspectCorrection"), ref pos);
            pos.y += yIncrement;
            aspectCorrectionOff = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("off"), ref pos);
            aspectCorrectionOff.OnToggleState += AspectCorrectionOff_OnToggleState;
            aspectCorrectionFourThree = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("FourThree"), ref pos);
            aspectCorrectionFourThree.OnToggleState += AspectCorrectionFourThree_OnToggleState;
            aspectCorrectionSixteenTen = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("SixteenTen"), ref pos);
            aspectCorrectionSixteenTen.OnToggleState += AspectCorrectionSixteenTen_OnToggleState;
            UpdateAspectButtons();
        }

        void UpdateAspectButtons()
        {
            // Fake radio buttons
            aspectCorrectionOff.IsChecked = aspectCorrectionFourThree.IsChecked = aspectCorrectionSixteenTen.IsChecked = false;
            switch((RetroModeAspects)DaggerfallUnity.Settings.RetroModeAspectCorrection)
            {
                case RetroModeAspects.Off:
                    aspectCorrectionOff.IsChecked = true;
                    break;
                case RetroModeAspects.FourThree:
                    aspectCorrectionFourThree.IsChecked = true;
                    break;
                case RetroModeAspects.SixteenTen:
                    aspectCorrectionSixteenTen.IsChecked = true;
                    break;
            }
        }

        public override void ReadSettings()
        {
            modeSlider.ScrollIndex = DaggerfallUnity.Settings.RetroRenderingMode;
            postProcessSlider.ScrollIndex = DaggerfallUnity.Settings.PostProcessingInRetroMode;
            UpdateAspectButtons();
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.RetroMode);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.RetroRenderingMode = 0;
            DaggerfallUnity.Settings.PostProcessingInRetroMode = 0;
            DaggerfallUnity.Settings.RetroModeAspectCorrection = 0;
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

        private void AspectCorrectionOff_OnToggleState()
        {
            aspectCorrectionOff.IsChecked = true;
            aspectCorrectionFourThree.IsChecked = false;
            aspectCorrectionSixteenTen.IsChecked = false;
            DaggerfallUnity.Settings.RetroModeAspectCorrection = (int)RetroModeAspects.Off;
            DeploySettings();
        }

        private void AspectCorrectionFourThree_OnToggleState()
        {
            aspectCorrectionOff.IsChecked = false;
            aspectCorrectionFourThree.IsChecked = true;
            aspectCorrectionSixteenTen.IsChecked = false;
            DaggerfallUnity.Settings.RetroModeAspectCorrection = (int)RetroModeAspects.FourThree;
            DeploySettings();
        }

        private void AspectCorrectionSixteenTen_OnToggleState()
        {
            aspectCorrectionOff.IsChecked = false;
            aspectCorrectionFourThree.IsChecked = false;
            aspectCorrectionSixteenTen.IsChecked = true;
            DaggerfallUnity.Settings.RetroModeAspectCorrection = (int)RetroModeAspects.SixteenTen;
            DeploySettings();
        }
    }
}