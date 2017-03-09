// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Contains live state of quests in play.
    /// Quests are instantiated from text source and executed inside quest machine.
    /// Each quest is assigned a unique id (UID) as its possible for same question to be instantiated multiple times,
    /// such as a basic fetch quest to two different dungeons. The name of quest cannot not be used for unique identification.
    /// Child resources generally will not care about quest UID, but this is used by quest machine.
    /// </summary>
    public class Quest
    {
        #region Fields

        // Quest object collections
        Dictionary<int, Message> messages = new Dictionary<int, Message>();
        Dictionary<string, Clock> clocks = new Dictionary<string, Clock>();
        Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        Dictionary<string, Place> places = new Dictionary<string, Place>();

        ulong uid;
        bool questComplete = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets quest UID assigned at create time.
        /// </summary>
        public ulong UID
        {
            get { return uid; }
        }

        /// <summary>
        /// True when quest has completed and will be deleted from quest machine.
        /// </summary>
        public bool QuestComplete
        {
            get { return questComplete; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Quest()
        {
            uid = DaggerfallUnity.NextUID;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update quest.
        /// </summary>
        public void Update()
        {
            // Do nothing if complete
            // Now waiting to be removed from quest machine
            if (questComplete)
                return;

            // Update tasks
            foreach(Task task in tasks.Values)
            {
                task.Update();
            }
        }

        public void EndQuest()
        {
            questComplete = true;
        }

        #endregion

        #region Resource Query Methods

        public Message GetMessage(int messageID)
        {
            if (messages.ContainsKey(messageID))
                return messages[messageID];
            else
                return null;
        }

        #endregion

        #region Resource Allocation Methods

        public void AddMessage(int messageID, Message message)
        {
            messages.Add(messageID, message);
        }

        public void AddClock(string symbol, Clock clock)
        {
            clocks.Add(symbol, clock);
        }

        public void AddPlace(string symbol, Place place)
        {
            places.Add(symbol, place);
        }

        public void AddTask(string symbol, Task task)
        {
            tasks.Add(symbol, task);
        }

        #endregion

        #region Private Methods
        #endregion
    }
}