// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using System;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// SymbolsDataSource context sensitive methods for quests in Daggerfall Unity.
    /// </summary>
    public partial class Quest : MacroDataSource, IDisposable
    {
        // He/She
        public override string Pronoun()
        {
            if (LastPersonReferenced == null)
                return HardStrings.pronounHe;

            switch (LastPersonReferenced.Gender)
            {
                default:
                case Game.Entity.Genders.Male:
                    return HardStrings.pronounHe;
                case Game.Entity.Genders.Female:
                    return HardStrings.pronounShe;
            }
        }
        // Him/Her
        public override string Pronoun2()
        {
            if (LastPersonReferenced == null)
                return HardStrings.pronounHim;

            switch (LastPersonReferenced.Gender)
            {
                default:
                case Game.Entity.Genders.Male:
                    return HardStrings.pronounHim;
                case Game.Entity.Genders.Female:
                    return HardStrings.pronounHer;
            }
        }

        // Himself/Herself
        public override string Pronoun2self()
        {
            if (LastPersonReferenced == null)
                return HardStrings.pronounHimself;

            switch (LastPersonReferenced.Gender)
            {
                default:
                case Game.Entity.Genders.Male:
                    return HardStrings.pronounHimself;
                case Game.Entity.Genders.Female:
                    return HardStrings.pronounHerself;
            }
        }

        // His/Hers
        public override string Pronoun3()
        {
            if (LastPersonReferenced == null)
                return HardStrings.pronounHis;

            switch (LastPersonReferenced.Gender)
            {
                default:
                case Game.Entity.Genders.Male:
                    return HardStrings.pronounHis;
                case Game.Entity.Genders.Female:
                    return HardStrings.pronounHers;
            }
        }

        public override string QuestDate()
        {
            return QuestStartTime.DateString();
        }

        /// <summary>
        /// Oaths by race.
        /// </summary>
        enum RacialOaths
        {
            None = 0,
            Nord = 201,
            Khajiit = 202,
            Redguard = 203,
            Breton = 204,
            Argonian = 205,
            Bosmer = 206,
            Altmer = 207,
            Dunmer = 208,
        }

        // Oaths seem to be declared by NPC race
        // Daggerfall NPCs have a limited range of races (usually Breton or Redguard).
        // Have seen Nord oaths used in Daggerfall (e.g. Mages guild questor in Gothway Garden)
        // Suspect NPCs with race: -1 (e.g. #63) get a random humanoid race within reason
        // Just returning Nord oaths for now until ready to build this out properly
        // https://www.imperial-library.info/content/daggerfall-oaths-and-expletives
        public override string Oath()
        {
            return DaggerfallUnity.Instance.TextProvider.GetRandomText((int)RacialOaths.Nord);
        }

        public override string Region()
        {
            return (LastPersonReferenced != null) ? LastPersonReferenced.HomeRegionName : "";
        }

        public override string God()
        {
            // Get god of current NPC or fallback
            if (LastPersonReferenced != null)
                return LastPersonReferenced.GodName;
            else
                return "Arkay";
        }

    }
}