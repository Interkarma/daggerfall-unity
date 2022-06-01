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
    /// <summary>
    /// Interface to a single page of in-game effect settings UI.
    /// </summary>
    public interface IGameEffectConfigPage
    {
        string Key { get; }
        string Title { get; }
        void Setup(Panel parent);
        void ReadSettings();
        void DeploySettings();
        void SetDefaults();
    }

    public abstract class GameEffectConfigPage : IGameEffectConfigPage
    {
        protected Rect aboutPanelRect = new Rect(0, 0, 145, 6);
        protected readonly Vector2 settingsStartPos = new Vector2(2, 10);
        protected int yIncrement = 12;

        protected Color aboutPanelBackgroundColor = new Color(0.4f, 0.4f, 0.4f, 0.65f);
        protected Color aboutPanelTextColor = Color.cyan;
        protected Color aboutPanelTextShadowColor = Color.cyan / 2;

        public abstract string Key { get; }
        public abstract string Title { get; }
        public abstract void Setup(Panel parent);
        public abstract void ReadSettings();
        public abstract void DeploySettings();
        public abstract void SetDefaults();

        #region Helpers

        protected Panel AddTipPanel(Panel parent, string text)
        {
            Panel panel = new Panel();
            panel.EnableBorder = true;
            panel.BackgroundColor = aboutPanelBackgroundColor;
            panel.Position = aboutPanelRect.position;
            panel.Size = aboutPanelRect.size;
            panel.VerticalAlignment = VerticalAlignment.Top;
            parent.Components.Add(panel);

            TextLabel label = new TextLabel();
            label.Text = text;
            label.TextColor = aboutPanelTextColor;
            label.ShadowColor = aboutPanelTextShadowColor;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Middle;
            label.TextScale = 0.75f;
            panel.Components.Add(label);

            return panel;
        }

        protected TextLabel AddLabel(Panel parent, string text, ref Vector2 position)
        {
            TextLabel label = new TextLabel();
            label.Text = text;
            label.Position = position;
            parent.Components.Add(label);

            return label;
        }

        protected Checkbox AddCheckbox(Panel parent, string label, ref Vector2 position)
        {
            Checkbox check = new Checkbox();
            check.Label.Text = label;
            check.IsChecked = false;
            check.Position = position;
            parent.Components.Add(check);
            position.y += yIncrement;

            return check;
        }

        protected HorizontalSlider AddSlider(Panel parent, string label, int count, ref Vector2 position)
        {
            const int sliderWidth = 70;
            const int sliderHeight = 6;

            // Slider label
            AddLabel(parent, label, ref position);
            position.y += 6;

            // Slider
            HorizontalSlider slider = new HorizontalSlider();
            slider.Size = new Vector2(sliderWidth, sliderHeight);
            slider.Position = position;
            slider.BackgroundColor = Color.black;
            slider.TotalUnits = count;
            slider.DisplayUnits = count;
            slider.ScrollIndex = 0;
            parent.Components.Add(slider);

            position.y += yIncrement;

            return slider;
        }

        protected void StyleIndicator(HorizontalSlider slider)
        {
            slider.IndicatorOffset = 2;
            slider.Indicator.TextColor = Color.white;
            slider.Indicator.ShadowPosition = Vector2.zero;
        }

        #endregion
    }
}