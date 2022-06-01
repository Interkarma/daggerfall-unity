// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//

using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Plays one of the ANIMXXXX.VID videos for quest
    /// </summary>
    public class PlayVideo : ActionTemplate
    {
        const string vidPrefix = "ANIM";
        const string vidSuffix = ".VID";

        public string videoName;

        public override string Pattern
        {
            get { return @"play video (?<vidNum>\d+)"; }
        }

        public PlayVideo(Quest parentQuest) : base(parentQuest) {}

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            PlayVideo action = new PlayVideo(parentQuest);

            try
            {
                var num = match.Groups["vidNum"].Value;

                if (string.IsNullOrEmpty(num) || num.Length > 4)
                    return null;

                action.videoName = vidPrefix;

                for (int i = 0; i < (4 - num.Length); i++)
                {
                    action.videoName += 0;
                }

                action.videoName += num + vidSuffix;
                Debug.LogWarning("vid name: " + action.videoName);
            }
            catch(System.Exception ex)
            {
                Debug.LogError("PlayVideo.Create() failed with exception: " + ex.Message);
                action = null;
            }

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            DaggerfallVidPlayerWindow vidPlayerWindow = (DaggerfallVidPlayerWindow)
                UIWindowFactory.GetInstanceWithArgs(UIWindowType.VidPlayer, new object[] { DaggerfallUI.UIManager, videoName });
            vidPlayerWindow.EndOnAnyKey = false;
            DaggerfallUI.UIManager.PushWindow(vidPlayerWindow);

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public string videoName;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.videoName = videoName;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            videoName = data.videoName;
        }

        #endregion
    }
}
