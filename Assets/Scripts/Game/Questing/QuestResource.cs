using System;
using DaggerfallWorkshop.Utility;
using System.Text.RegularExpressions;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// All quest resources hosted by a quest must inherit from this base class.
    /// Symbol will be set by inheriting class after parsing source text.
    /// </summary>
    public abstract class QuestResource
    {
        Quest parentQuest = null;
        Symbol symbol;
        int infoMessageID = -1;
        int usedMessageID = -1;
        int rumorsMessageID = -1;

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
        /// Allows quest resources like Clock to perform some action.
        /// Quest will not call Tick() on ActionTemplate items, they should override Update() instead.
        /// </summary>
        public virtual void Tick(Quest caller)
        {
        }

        /// <summary>
        /// Parse optional message tags from this resource.
        /// </summary>
        /// <param name="line"></param>
        protected void ParseMessageTags(string line)
        {
            string matchStr = @"anyInfo (?<info>\d+)|used (?<used>\d+)|rumors (?<rumors>\d+)";

            // Get message tag matches
            MatchCollection matches = Regex.Matches(line, matchStr);
            if (matches.Count == 0)
                return;

            // Grab tag values
            foreach (Match match in matches)
            {
                // Match info message id
                Group info = match.Groups["info"];
                if (info.Success)
                    infoMessageID = Parser.ParseInt(info.Value);

                // Match used message id
                Group used = match.Groups["used"];
                if (used.Success)
                    usedMessageID = Parser.ParseInt(used.Value);

                // Match rumors message id
                Group rumors = match.Groups["rumors"];
                if (rumors.Success)
                    rumorsMessageID = Parser.ParseInt(rumors.Value);
            }
        }
    }
}