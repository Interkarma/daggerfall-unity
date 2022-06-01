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
    public class DitherConfigPage : GameEffectConfigPage
    {
        const string key = "dither";

        Checkbox enableCheckbox;

        public override string Key => key;

        public override string Title => TextManager.Instance.GetLocalizedText(Key);

        public override void Setup(Panel parent)
        {
            Vector2 pos = settingsStartPos;

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("ditherTip"));

            // Enable toggle
            enableCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("enable"), ref pos);
            enableCheckbox.OnToggleState += EnableCheckbox_OnToggleState;
        }

        public override void ReadSettings()
        {
            enableCheckbox.IsChecked = DaggerfallUnity.Settings.DitherEnable;
        }

        public override void DeploySettings()
        {
            GameManager.Instance.StartGameBehaviour.DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.Dither);
        }

        public override void SetDefaults()
        {
            DaggerfallUnity.Settings.DitherEnable = false;
        }

        private void EnableCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.DitherEnable = enableCheckbox.IsChecked;
            DeploySettings();
        }
    }
}