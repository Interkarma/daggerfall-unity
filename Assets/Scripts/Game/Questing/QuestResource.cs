using System;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// All quest resources hosted by a quest must inherit from this base class.
    /// Symbol will be set by inheriting class after parsing source text.
    /// </summary>
    public abstract class QuestResource
    {
        Quest parentQuest = null;
        string symbol;

        /// <summary>
        /// Symbol of this quest resource (if any).
        /// </summary>
        public string Symbol
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
    }
}