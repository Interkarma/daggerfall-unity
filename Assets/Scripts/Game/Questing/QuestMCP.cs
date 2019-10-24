// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.Questing
{
    public partial class Quest : IMacroContextProvider
    {
        public MacroDataSource GetMacroDataSource()
        {
            return new QuestMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for quests in Daggerfall Unity.
        /// </summary>
        private class QuestMacroDataSource : MacroDataSource
        {
            private Quest parent;
            public QuestMacroDataSource(Quest parent)
            {
                this.parent = parent;
            }

            public override string Name()
            {
                // Set seed to the quest UID before falling through to random name generation. (See t=2108)
                DFRandom.srand((int) parent.UID);
                return null;
            }

            public override string FactionOrderName()
            {
                // Only used for knightly order quests, %kno macro. (removing 'The ' prefix from name for readability)
                FactionFile.FactionData factionData;
                if (DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(parent.FactionId, out factionData))
                    return factionData.name.StartsWith("The ") ? factionData.name.Substring(4) : factionData.name;
                else
                    return null;
            }

            // He/She
            public override string Pronoun()
            {
                if (parent.LastResourceReferenced == null)
                    return HardStrings.pronounHe;

                switch (parent.LastResourceReferenced.Gender)
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
                if (parent.LastResourceReferenced == null)
                    return HardStrings.pronounHim;

                switch (parent.LastResourceReferenced.Gender)
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
                if (parent.LastResourceReferenced == null)
                    return HardStrings.pronounHimself;

                switch (parent.LastResourceReferenced.Gender)
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
                if (parent.LastResourceReferenced == null)
                    return HardStrings.pronounHis;

                switch (parent.LastResourceReferenced.Gender)
                {
                    default:
                    case Game.Entity.Genders.Male:
                        return HardStrings.pronounHis;
                    case Game.Entity.Genders.Female:
                        return HardStrings.pronounHer;
                }
            }

            public override string VampireNpcClan()
            {
                // %vcn
                if (parent.LastResourceReferenced == null && !(parent.LastResourceReferenced is Person))
                    return null;

                // Use home Place region to determine vampire clan of NPC
                // Fallback to current region if unable to determine home region of NPC
                // Also fallback to using generic "vampire" faction name if clan not resolved
                Person person = (Person)parent.LastResourceReferenced;
                if (person.FactionData.type == (int)FactionFile.FactionTypes.VampireClan)
                {
                    int regionIndex = person.HomeRegionIndex;
                    if (regionIndex == -1)
                        regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
                    VampireClans vampireClan = FormulaHelper.GetVampireClan(regionIndex);
                    if (vampireClan == VampireClans.None)
                        return person.FactionData.name;
                    else
                        return TextManager.Instance.GetText("Races", vampireClan.ToString().ToLower());
                }

                return null;
            }

            public override string QuestDate()
            {
                return parent.GetCurrentLogMessageTime().DateString();
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

            public override string HomeRegion()
            {
                // Must be a Person
                if (parent.lastResourceReferenced is Person)
                {
                    return (parent.lastResourceReferenced != null) ? (parent.lastResourceReferenced as Person).HomeRegionName : string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }

            public override string God()
            {
                // Get god of last Person referenced by dialog stream
                if (parent.lastResourceReferenced is Person)
                {
                    if (parent.lastResourceReferenced != null)
                        return (parent.lastResourceReferenced as Person).GodName;
                }

                // Attempt to get god of questor Person
                // This fixes a crash when handing in quest and expanding questor dialog without a previous Person reference
                Symbol[] questors = parent.GetQuestors();
                if (questors != null && questors.Length > 0)
                    return parent.GetPerson(questors[0]).GodName;

                // Otherwise just provide a random god name for whomever is speaking to player
                // Better just to provide something than let let game loop crash
                return Person.GetRandomGodName();

                // Fall-through to non-quest macro handling
                // Disabling this at it causes exception stream from null MCP
                //return "%god";
            }

            public override string Direction()
            {
                Place questLastPlaceReferenced = parent.LastPlaceReferenced;

                if (questLastPlaceReferenced.Scope == Place.Scopes.Remote)
                {
                    if (questLastPlaceReferenced == null)
                    {
                        QuestMachine.Log(parent, "Trying to get direction to quest location when no location has been referenced in the quest.");
                        return TextManager.Instance.GetText("ConversationText", "resolvingError");
                    }

                    return GameManager.Instance.TalkManager.GetLocationCompassDirection(questLastPlaceReferenced);
                }
                else if (questLastPlaceReferenced.Scope == Place.Scopes.Local)
                {
                    if (questLastPlaceReferenced.SiteDetails.locationName == GameManager.Instance.PlayerGPS.CurrentLocation.Name)
                    {
                        return GameManager.Instance.TalkManager.GetBuildingCompassDirection(questLastPlaceReferenced.SiteDetails.buildingKey);
                    }
                    else
                    {
                        string result = questLastPlaceReferenced.SiteDetails.locationName;
                        result += TextManager.Instance.GetText(TalkManager.TextDatabase, "comma");                        
                        result += GameManager.Instance.TalkManager.GetLocationCompassDirection(questLastPlaceReferenced);
                        return result;
                    }
                }

                return TextManager.Instance.GetText(TalkManager.TextDatabase, "resolvingError");
            }
        }
    }
}
