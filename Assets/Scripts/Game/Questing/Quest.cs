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
    /// </summary>
    public class Quest
    {
        #region Fields

        // Quest object collections
        Dictionary<int, Message> messages = new Dictionary<int, Message>();
        Dictionary<string, Clock> clocks = new Dictionary<string, Clock>();
        Dictionary<string, Task> tasks = new Dictionary<string, Task>();

        #endregion

        #region Properties
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Quest()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update quest.
        /// </summary>
        public void Update()
        {
            // Iteratively update each task
            foreach(Task task in tasks.Values)
            {
                task.Update(this);
            }
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

        public void AddTask(string symbol, Task task)
        {
            tasks.Add(symbol, task);
        }

        #endregion

        #region Private Methods
        #endregion
    }
}