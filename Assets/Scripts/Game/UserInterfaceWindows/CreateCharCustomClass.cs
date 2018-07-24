// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements custom class creator window.
    /// </summary>
    public class CreateCharCustomClass : DaggerfallPopupWindow
    {
        const string nativeImgName = "CUST00I0.IMG";

        Texture2D nativeTexture;
        DaggerfallFont font;
        StatsRollout statsRollout;
        TextBox textBox = new TextBox();
        DFCareer createdClass;
        int lastSkill;

        public DFCareer CreatedClass
        {
            get { return createdClass; }
        }

        #region UI Rects

        Rect[] skillButtonRects = new Rect[]
        {
            new Rect(66, 31, 108, 8),
            new Rect(66, 41, 108, 8),
            new Rect(66, 51, 108, 8),
            new Rect(66, 80, 108, 8),
            new Rect(66, 90, 108, 8),
            new Rect(66, 100, 108, 8),
            new Rect(66, 129, 108, 8),
            new Rect(66, 139, 108, 8),
            new Rect(66, 149, 108, 8),
            new Rect(66, 159, 108, 8),
            new Rect(66, 169, 108, 8),
            new Rect(66, 179, 108, 8)
        };

        #endregion

        #region Buttons

        Button[] skillButtons = new Button[12];

        #endregion

        #region Text Labels

        TextLabel[] skillLabels = new TextLabel[12];

        #endregion

        public CreateCharCustomClass(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #region Setup Methods

        protected override void Setup()
        {
            if (IsSetup)
                return;

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharCustomClass: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add stats rollout
            statsRollout = new StatsRollout(false, true);
            statsRollout.Position = new Vector2(0, 0);
            NativePanel.Components.Add(statsRollout);

            // Add name textbox
            textBox.Position = new Vector2(100, 5);
            textBox.Size = new Vector2(214, 7);
            NativePanel.Components.Add(textBox);

            // Initialize character class
            createdClass = new DFCareer();

            // Initiate UI components
            font = DaggerfallUI.DefaultFont;
            SetupButtons();

            IsSetup = true;
        }

        protected void SetupButtons()
        {
            // Add skill selector buttons
            for (int i = 0; i < skillButtons.Length; i++) 
            {
                skillButtons[i] = DaggerfallUI.AddButton(skillButtonRects[i], NativePanel);
                skillButtons[i].Tag = i;
                skillButtons[i].OnMouseClick += skillButton_OnMouseClick;
                skillLabels[i] = DaggerfallUI.AddTextLabel(font, new Vector2(3, 2), string.Empty, skillButtons[i]);
            }
        }

        #endregion

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Event Handlers

        void skillButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            DaggerfallListPickerWindow skillPicker = new DaggerfallListPickerWindow(uiManager, this);
            skillPicker.OnItemPicked += SkillPicker_OnItemPicked;
            foreach (DFCareer.Skills skill in Enum.GetValues(typeof(DFCareer.Skills)))
                skillPicker.ListBox.AddItem(DaggerfallUnity.Instance.TextProvider.GetSkillName(skill));
            lastSkill = (int)sender.Tag;
            uiManager.PushWindow(skillPicker);
        }

        void SkillPicker_OnItemPicked(int index, string skillName)
        {
            CloseWindow();
            skillLabels[lastSkill].Text = skillName;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }    
}