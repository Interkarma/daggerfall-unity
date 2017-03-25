using System;
using DaggerfallWorkshop.Utility;

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
        /// Constructor.
        /// </summary>
        /// <param name="parentQuest">Parent quest owning this resource. Can be null.</param>
        public QuestResource(Quest parentQuest)
        {
            this.parentQuest = parentQuest;
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
    }
}