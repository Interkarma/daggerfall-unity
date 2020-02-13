using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using System.Text.RegularExpressions;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// All quest resources hosted by a quest must inherit from this base class.
    /// Symbol will be set by inheriting class after parsing source text.
    /// </summary>
    public abstract class QuestResource : IDisposable
    {
        public const string BLANK = "BLANK";

        Quest parentQuest = null;
        Symbol symbol;
        int infoMessageID = -1;
        int usedMessageID = -1;
        int rumorsMessageID = -1;
        bool hasPlayerClicked = false;
        bool isHidden = false;

        [NonSerialized]
        QuestResourceBehaviour questResourceBehaviour = null;

        /// <summary>
        /// Symbol of this quest resource (if any).
        /// </summary>
        public Symbol Symbol
        {
            get { return symbol; }
            protected set { symbol = value; }
        }

        /// <summary>
        /// Parent quest of this quest resource (if any).
        /// </summary>
        public Quest ParentQuest
        {
            get { return parentQuest; }
        }

        /// <summary>
        /// Gets or sets message ID for "tell me about" this resource in dialog.
        /// -1 is default and means no message of this type is associated with this resource.
        /// </summary>
        public int InfoMessageID
        {
            get { return infoMessageID; }
            set { infoMessageID = value; }
        }

        /// <summary>
        /// Gets or sets message ID to display when resource (item) is used by player.
        /// -1 is default and means no message of this type is associated with this resource.
        /// </summary>
        public int UsedMessageID
        {
            get { return usedMessageID; }
            set { usedMessageID = value; }
        }

        /// <summary>
        /// Gets or sets message ID for "any news?" about this resource in dialog.
        /// -1 is default and means no message of this type is associated with this resource.
        /// </summary>
        public int RumorsMessageID
        {
            get { return rumorsMessageID; }
            set { rumorsMessageID = value; }
        }

        /// <summary>
        /// Gets flag stating if player has clicked on this Person resource in world.
        /// If consuming click rearm using RearmPlayerClick().
        /// </summary>
        public bool HasPlayerClicked
        {
            get { return hasPlayerClicked; }
        }

        /// <summary>
        /// Gets or sets flag to hide this quest resource in world.
        /// Has no effect on Foes or quest resources not active inside scene.
        /// </summary>
        public bool IsHidden
        {
            get { return isHidden; }
            set { SetHidden(value); }
        }

        /// <summary>
        /// Gets resource gender where available.
        /// Will default to Male if inheriting class does not override.
        /// </summary>
        public virtual Genders Gender
        {
            get { return Genders.Male; }
        }

        /// <summary>
        /// Gets or sets reference to QuestResourceBehaviour in scene.
        /// This property is not serialized - it should be set at time of injection or deserialization.
        /// </summary>
        public QuestResourceBehaviour QuestResourceBehaviour
        {
            get { return questResourceBehaviour; }
            set
            {
                questResourceBehaviour = value;
                questResourceBehaviour.OnGameObjectDestroy += QuestResourceBehaviour_OnGameObjectDestroy;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentQuest">Parent quest owning this resource. Can be null.</param>
        public QuestResource(Quest parentQuest)
        {
            this.parentQuest = parentQuest;
        }

        /// <summary>
        /// Set the resource source line.
        /// Always call this base method to ensure optional message tags are parsed.
        /// </summary>
        /// <param name="line">Source line for this resource.</param>
        public virtual void SetResource(string line)
        {
            ParseMessageTags(line);
        }

        /// <summary>
        /// Expand a macro from this resource.
        /// </summary>
        /// <param name="macro">Type of macro to expand.</param>
        /// <param name="textOut">Expanded text for this macro type. Empty if macro cannot be expanded.</param>
        /// <returns>True if macro expanded, otherwise false.</returns>
        public virtual bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            textOut = string.Empty;

            return false;
        }

        /// <summary>
        /// Called every Quest tick.
        /// Allows quest resources to perform some action.
        /// Quest will not call Tick() on ActionTemplate items, they should override Update() instead.
        /// </summary>
        public virtual void Tick(Quest caller)
        {
            // Show or hide GameObject for related QuestResourceBehaviour
            if (questResourceBehaviour)
            {
                // Ignore for Foes
                if (this is Foe)
                    return;

                // A destroyed NPC is always hidden
                if (this is Person && (this as Person).IsDestroyed)
                    (this as Person).IsHidden = true;

                // Show or hide GameObject mapped to this QuestResource based on hidden flag
                // This can conflict with other code that has disabled GameObject for other reasons
                questResourceBehaviour.gameObject.SetActive(!IsHidden);
            }
        }

        /// <summary>
        /// Called at the end of every quest tick.
        /// Allows quest resources to perform some cleanup action after actions have run for this tick.
        /// </summary>
        public virtual void PostTick(Quest caller)
        {
            RearmPlayerClick();
        }

        /// <summary>
        /// Parse optional message tags from this resource.
        /// </summary>
        /// <param name="line"></param>
        protected void ParseMessageTags(string line)
        {
            string matchStr = @"anyInfo (?<info>\d+)|used (?<used>\d+)|rumors (?<rumors>\d+)|" +
                              @"anyInfo (?<infoName>\w+)|used (?<usedName>\w+)|rumors (?<rumorsName>\w+)";

            // Get message tag matches
            MatchCollection matches = Regex.Matches(line, matchStr);
            if (matches.Count == 0)
                return;

            // Grab tag values
            foreach (Match match in matches)
            {
                //
                // info
                //

                // Match info message ID
                Group info = match.Groups["info"];
                if (info.Success)
                    infoMessageID = Parser.ParseInt(info.Value);

                // Resolve info message name back to ID
                string infoName = match.Groups["infoName"].Value;
                if (infoMessageID == -1 && !string.IsNullOrEmpty(infoName))
                {
                    Table table = QuestMachine.Instance.StaticMessagesTable;
                    infoMessageID = Parser.ParseInt(table.GetValue("id", infoName));
                }

                //
                // used
                //

                // Match used message ID
                Group used = match.Groups["used"];
                if (used.Success)
                    usedMessageID = Parser.ParseInt(used.Value);

                // Resolve used message name back to ID
                string usedName = match.Groups["usedName"].Value;
                if (usedMessageID == -1 && !string.IsNullOrEmpty(usedName))
                {
                    Table table = QuestMachine.Instance.StaticMessagesTable;
                    usedMessageID = Parser.ParseInt(table.GetValue("id", usedName));
                }

                //
                // rumors
                //

                // Match rumors message ID
                Group rumors = match.Groups["rumors"];
                if (rumors.Success)
                    rumorsMessageID = Parser.ParseInt(rumors.Value);

                // Resolve rumors message name back to ID
                string rumorsName = match.Groups["rumorsName"].Value;
                if (rumorsMessageID == -1 && !string.IsNullOrEmpty(rumorsName))
                {
                    Table table = QuestMachine.Instance.StaticMessagesTable;
                    rumorsMessageID = Parser.ParseInt(table.GetValue("id", rumorsName));
                }
            }
        }

        /// <summary>
        /// Called when quest ends so resource can clean up.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Check if player click has been triggered.
        /// </summary>
        public void SetPlayerClicked()
        {
            if (this is Person && ((this as Person).IsMuted || (this as Person).IsDestroyed))
            {
                QuestMachine.LogFormat("Ignoring click on muted or destroyed Person {0}.", Symbol.Original);
                return;
            }

            hasPlayerClicked = true;
        }

        /// <summary>
        /// Rearm click so player can click again if quest allows it.
        /// </summary>
        public void RearmPlayerClick()
        {
            hasPlayerClicked = false;
        }

        public List<TextFile.Token[]> GetMessage(int messageId)
        {
            Message message = ParentQuest.GetMessage(messageId);
            return message == null ? null : TokenizeMessage(message);
        }

        private static List<TextFile.Token[]> TokenizeMessage(Message message)
        {
            var tokenList = new List<TextFile.Token[]>();
            for (int i = 0; i < message.VariantCount; i++)
            {
                TextFile.Token[] tokens = message.GetTextTokensByVariant(i, false); // do not expand macros here (they will be expanded just in time by TalkManager class)
                tokenList.Add(tokens);
            }

            return tokenList;
        } 

        #region Serialization

        [fsObject("v1")]
        public struct ResourceSaveData_v1
        {
            public Type type;
            public Symbol symbol;
            public int infoMessageID;
            public int usedMessageID;
            public int rumorsMessageID;
            public bool hasPlayerClicked;
            public bool isHidden;
            public object resourceSpecific;
        }

        /// <summary>
        /// Get full resource save data including resource specific data.
        /// </summary>
        public ResourceSaveData_v1 GetResourceSaveData()
        {
            ResourceSaveData_v1 resourceData = new ResourceSaveData_v1();
            resourceData.type = GetType();
            resourceData.symbol = symbol;
            resourceData.infoMessageID = infoMessageID;
            resourceData.usedMessageID = usedMessageID;
            resourceData.hasPlayerClicked = hasPlayerClicked;
            resourceData.isHidden = isHidden;
            resourceData.resourceSpecific = GetSaveData();

            return resourceData;
        }

        /// <summary>
        /// Restore full resource save data including resource specific data.
        /// </summary>
        public void RestoreResourceSaveData(ResourceSaveData_v1 data)
        {
            symbol = data.symbol;
            infoMessageID = data.infoMessageID;
            usedMessageID = data.usedMessageID;
            hasPlayerClicked = data.hasPlayerClicked;
            isHidden = data.isHidden;
            RestoreSaveData(data.resourceSpecific);
        }

        /// <summary>
        /// Request resource-specific save data only for serialization.
        /// Must be handled by implementing class.
        /// </summary>
        public abstract object GetSaveData();

        /// <summary>
        /// Restore resource-specific deserialized data only.
        /// Must be handled by implementing class.
        /// </summary>
        public abstract void RestoreSaveData(object dataIn);

        #endregion

        #region Private Methods

        void SetHidden(bool value)
        {
            // Set hidden flag
            // NOTE: Foes are a one-to-many resource - hiding a Foe will remove ALL spawned instances of that Foe
            isHidden = value;
        }

        #endregion

        #region Event Handlers

        private void QuestResourceBehaviour_OnGameObjectDestroy(QuestResourceBehaviour questResourceBehaviour)
        {
            // Clean up when target GameObject being destroyed
            questResourceBehaviour.OnGameObjectDestroy -= QuestResourceBehaviour_OnGameObjectDestroy;
            questResourceBehaviour = null;
        }

        #endregion
    }
}