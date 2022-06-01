// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    DFIronman, Hazelnut
// 
// Notes:
//

using UnityEngine;
using System.Text.RegularExpressions;
using System;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Plays a song from MIDI.BSA using SongFiles enum.
    /// </summary>
    public class PlaySong : ActionTemplate
    {
        SongFiles songFile;

        public override string Pattern
        {
            get { return @"play song (?<song>[a-zA-Z0-9_-]+)"; }
        }

        public PlaySong(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Trim source end or trailing white space will be split to an empty symbol at end of array
            source = source.TrimEnd();

            // Factory new action
            PlaySong action = new PlaySong(parentQuest);
            string song = match.Groups["song"].Value;
            if (!Enum.IsDefined(typeof(SongFiles), song))
            {
                SetComplete();
                throw new Exception(string.Format("PlaySong: Song file {0} is not a known song from MIDI.BSA", song));
            }
            action.songFile = (SongFiles)Enum.Parse(typeof(SongFiles), song);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            GameObject go = GameObject.Find("SongPlayer");
            if (go != null)
            {
                DaggerfallSongPlayer player = go.GetComponent<DaggerfallSongPlayer>();
                if (player != null)
                    player.Play(songFile);
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public SongFiles song;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.song = songFile;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            songFile = data.song;
        }

        #endregion
    }
}