// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{

    public class PlaySound : ActionTemplate
    {
        /// Plays sound for quests (only used in Sx977...vengence)
        /// See Quests-Sounds.txt for valid sounds
        /// Unlike message posts, the play sound command performs until task is cleared
        /// can be in the form of:
        /// play sound (soundname) every x minutes y times
        /// or play sound (soundname) x y
        /// the second number is not currently used for anything - purpose is unkown.

        public string   soundName;              //used to lookup sound index in sound table
        public int      soundIndex;
        public uint     interval;               //how often to play; measured in game minutes
        public int      unknown;                //according to Tipton's documentation, doesn't do anything
        public ulong    lastTimePlayed = 0;     //last time sound was played
        public AudioClip clip;                  //preloading


        /// <summary>
        /// Data to serialize action state can be placed in a struct.
        /// It's a good idea to version struct in case you need to migrate between very different state setups in future.
        /// For basic changes (e.g. just adding a new field with a sensible default) no migration should be needed.
        /// See FullSerializer versioning docs for more: https://github.com/jacobdufault/fullserializer/wiki/Versioning
        /// </summary>
        [fsObject("v1")]
        public struct MySaveData
        {
            public string soundName;
            public int soundIndex;
            public uint interval;
            public ulong lastTimePlayed;
        }

        public override string Pattern
        {
            get 
            {
                return @"play sound (?<sound>\w+)( every)? (?<n1>\d+)( minutes)? (?<n2>\d+)";
            }
        }

        /// <summary>
        /// Constructor must set parent quest.
        /// </summary>
        /// <param name="quest">Quest this action belongs to. Can be null for template.</param>
        public PlaySound(Quest quest) : base(quest) { }


        public override IQuestAction CreateNew(string source, Quest quest)
        {
            base.CreateNew(source, quest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            PlaySound playSoundAction = new PlaySound(quest);

            try
            {
                playSoundAction.soundName       = match.Groups["sound"].Value;
                playSoundAction.interval        = (uint)Parser.ParseInt(match.Groups["n1"].Value) * 60;
                playSoundAction.unknown         = Parser.ParseInt(match.Groups["n2"].Value);
                playSoundAction.lastTimePlayed  = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
                var soundID                     = (uint)QuestMachine.Instance.SoundsTable.GetInt("id", match.Groups["sound"].Value);
                playSoundAction.soundIndex      = (int)DaggerfallUnity.Instance.SoundReader.GetSoundIndex(soundID);
                playSoundAction.clip = DaggerfallUnity.Instance.SoundReader.GetAudioClip(playSoundAction.soundIndex);
            }
            catch (System.Exception ex)
            {
                DaggerfallUnity.LogMessage("PlaySound.Create() failed with exception: " + ex.Message, true);
                playSoundAction = null;
            }

            return playSoundAction;
        }

        /// <summary>
        /// Update is called by owning task once per frame as part of quest machine tick.
        /// Update is only called by task if active conditions are met.
        /// Perform any updates required here.
        /// </summary>
        /// <param name="caller">Task hosting this action.</param>
        public override void Update(Task caller)
        {
            var gameSeconds = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();

            // Attempt to reload clip if not available
            // This can happen if player loads a game after action created
            if (clip == null && soundIndex != 0)
            {
                clip = DaggerfallUnity.Instance.SoundReader.GetAudioClip(soundIndex);
            }

            if (lastTimePlayed + interval <= gameSeconds)
            {
                DaggerfallAudioSource source = QuestMachine.Instance.GetComponent<DaggerfallAudioSource>();
                if (source != null && !source.IsPlaying())
                {
                    source.PlayOneShot(soundIndex, DaggerfallUnity.Settings.SoundVolume);
                    lastTimePlayed = gameSeconds;
                }
            }
            // Unlike message posts, the play sound command performs until task is cleared
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public string soundName;
            public int soundIndex;
            public uint interval;
            public int unknown;
            public ulong lastTimePlayed;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.soundName = soundName;
            data.soundIndex = soundIndex;
            data.interval = interval;
            data.unknown = unknown;
            data.lastTimePlayed = lastTimePlayed;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            soundName = data.soundName;
            soundIndex = data.soundIndex;
            interval = data.interval;
            unknown = data.unknown;
            lastTimePlayed = data.lastTimePlayed;
        }

        #endregion
    }
}
