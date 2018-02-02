// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
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

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements the select class window.
    /// </summary>
    public class CreateCharClassSelect : DaggerfallListPickerWindow
    {
        const int startClassDescriptionID = 2100;

        List<DFCareer> classList = new List<DFCareer>();
        DFCareer selectedClass;

        public DFCareer SelectedClass
        {
            get { return selectedClass; }
        }

        public CreateCharClassSelect(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            base.Setup();

            // Read all CLASS*.CFG files and add to listbox
            string[] files = Directory.GetFiles(DaggerfallUnity.Instance.Arena2Path, "CLASS*.CFG");
            if (files != null && files.Length > 0)
            {
                for (int i = 0; i < files.Length - 1; i++)
                {
                    ClassFile classFile = new ClassFile(files[i]);
                    classList.Add(classFile.Career);
                    listBox.AddItem(classFile.Career.Name);
                }
            }

            OnItemPicked += DaggerfallClassSelectWindow_OnItemPicked;
        }

        void DaggerfallClassSelectWindow_OnItemPicked(int index, string className)
        {
            selectedClass = classList[index];

            TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(startClassDescriptionID + index);
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetTextTokens(textTokens);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            Button noButton = messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
            noButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            messageBox.OnButtonClick += ConfirmClassPopup_OnButtonClick;
            uiManager.PushWindow(messageBox);

            AudioClip clip = DaggerfallUnity.Instance.SoundReader.GetAudioClip(SoundClips.SelectClassDrums);
            DaggerfallUI.Instance.AudioSource.PlayOneShot(clip);
        }

        void ConfirmClassPopup_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                sender.CloseWindow();
                CloseWindow();
            }
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.No)
            {
                selectedClass = null;
                sender.CancelWindow();
            }
        }
    }
}