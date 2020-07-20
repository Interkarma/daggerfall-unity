// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Guilds;

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
                    return TextManager.Instance.GetLocalizedText("pronounHe");

                switch (parent.LastResourceReferenced.Gender)
                {
                    default:
                    case Game.Entity.Genders.Male:
                        return TextManager.Instance.GetLocalizedText("pronounHe");
                    case Game.Entity.Genders.Female:
                        return TextManager.Instance.GetLocalizedText("pronounShe");
                }
            }
            // Him/Her
            public override string Pronoun2()
            {
                if (parent.LastResourceReferenced == null)
                    return TextManager.Instance.GetLocalizedText("pronounHim");

                switch (parent.LastResourceReferenced.Gender)
                {
                    default:
                    case Game.Entity.Genders.Male:
                        return TextManager.Instance.GetLocalizedText("pronounHim");
                    case Game.Entity.Genders.Female:
                        return TextManager.Instance.GetLocalizedText("pronounHer");
                }
            }

            // Himself/Herself
            public override string Pronoun2self()
            {
                if (parent.LastResourceReferenced == null)
                    return TextManager.Instance.GetLocalizedText("pronounHimself");

                switch (parent.LastResourceReferenced.Gender)
                {
                    default:
                    case Game.Entity.Genders.Male:
                        return TextManager.Instance.GetLocalizedText("pronounHimself");
                    case Game.Entity.Genders.Female:
                        return TextManager.Instance.GetLocalizedText("pronounHerself");
                }
            }

            // His/Hers
            public override string Pronoun3()
            {
                if (parent.LastResourceReferenced == null)
                    return TextManager.Instance.GetLocalizedText("pronounHis");

                switch (parent.LastResourceReferenced.Gender)
                {
                    default:
                    case Game.Entity.Genders.Male:
                        return TextManager.Instance.GetLocalizedText("pronounHis");
                    case Game.Entity.Genders.Female:
                        return TextManager.Instance.GetLocalizedText("pronounHer");
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
                int factionId = 0;

                // Get the temple faction ID if player is inside a temple
                if (GameManager.Instance.IsPlayerInsideBuilding &&
                    GameManager.Instance.PlayerEnterExit.BuildingType == DFLocation.BuildingTypes.Temple)
                {
                    factionId = (int)GameManager.Instance.PlayerEnterExit.FactionID;
                }
                else
                { 
                    factionId = GameManager.Instance.PlayerGPS.GetTempleOfCurrentRegion();
                }

                if (factionId == 0)
                {
                    // Classic returns "BLANK" if no temple is found, here we return a random deity name
                    const int minGodID = 21;
                    const int maxGodID = 35;

                    FactionFile.FactionIDs god = (FactionFile.FactionIDs)UnityEngine.Random.Range(minGodID, maxGodID + 1);
                    return god.ToString();
                }

                Temple temple = (Temple)GameManager.Instance.GuildManager.GetGuild(factionId);
                return temple.Deity.ToString();
            }

            public override string Direction()
            {
                Place questLastPlaceReferenced = parent.LastPlaceReferenced;

                if (questLastPlaceReferenced.Scope == Place.Scopes.Remote)
                {
                    if (questLastPlaceReferenced == null)
                    {
                        QuestMachine.Log(parent, "Trying to get direction to quest location when no location has been referenced in the quest.");
                        return TextManager.Instance.GetLocalizedText("resolvingError");
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
                        result += TextManager.Instance.GetLocalizedText("comma");
                        result += GameManager.Instance.TalkManager.GetLocationCompassDirection(questLastPlaceReferenced);
                        return result;
                    }
                }

                return TextManager.Instance.GetLocalizedText("resolvingError");
            }
        }
    }
}
