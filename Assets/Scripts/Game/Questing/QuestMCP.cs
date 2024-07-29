// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.Utility;

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
            {   // %n %nam
                DFRandom.Seed = (uint)parent.UID;
                return MacroHelper.GetRandomFullName();
            }

            public override string FemaleName()
            {   // %fn
                DFRandom.Seed = (uint)parent.UID;
                NameHelper.BankTypes nameBank = (NameHelper.BankTypes)MapsFile.RegionRaces[GameManager.Instance.PlayerGPS.CurrentRegionIndex];
                return DaggerfallUnity.Instance.NameHelper.FullName(nameBank, Genders.Female);
            }

            public override string MaleName()
            {   // %mn
                DFRandom.Seed = (uint)parent.UID + 3457;
                NameHelper.BankTypes nameBank = (NameHelper.BankTypes)MapsFile.RegionRaces[GameManager.Instance.PlayerGPS.CurrentRegionIndex];
                return DaggerfallUnity.Instance.NameHelper.FullName(nameBank, Genders.Male);
            }

            public override string FactionOrderName()
            {
                // Only used for knightly order quests, %kno macro. (removing 'The ' prefix from name for readability)
                FactionFile.FactionData factionData;
                if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(parent.FactionId, out factionData))
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

            // His/Her
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
                        return TextManager.Instance.GetLocalizedText("pronounHer2");
                }
            }

            // His/Hers
            public override string Pronoun4()
            {
                if (parent.LastResourceReferenced == null)
                    return TextManager.Instance.GetLocalizedText("pronounHis");

                switch (parent.LastResourceReferenced.Gender)
                {
                    default:
                    case Game.Entity.Genders.Male:
                        return TextManager.Instance.GetLocalizedText("pronounHis2");
                    case Game.Entity.Genders.Female:
                        return TextManager.Instance.GetLocalizedText("pronounHers");
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

            // Oaths are declared by NPC race according to the race index used in FACTION.TXT.
            // Unfortunately, classic never uses this faction race ID but, instead, uses the
            // hardcoded index race of each region, which is not the same. In classic, this
            // results in all NPCs from High Rock saying Nord oaths, while all NPCs in Hammerfell
            // will say Khajiit oaths.
            // Instead, DFU uses the faction race ID to return the correct oath.
            // For the list of oaths, see https://www.imperial-library.info/content/daggerfall-oaths-and-expletives
            public override string Oath()
            {
                // Get the questor race to find the correct oath
                Races race = Races.None;
                Symbol[] questors = parent.GetQuestors();
                if (questors.Length > 0)
                    race = parent.GetPerson(questors[0]).Race;
                else if (QuestMachine.Instance.LastNPCClicked != null)
                {
                    // %oth is used in some of the main quests before the questor is actually set. In this
                    // case try to use the data from the last clicked NPC, which should be the questor.
                    race = QuestMachine.Instance.LastNPCClicked.Data.race;
                }

                // Fallback to race of current region
                if (race == Races.None)
                    race = GameManager.Instance.PlayerGPS.GetRaceOfCurrentRegion();

                int oathId = (int)RaceTemplate.GetFactionRaceFromRace(race);
                return DaggerfallUnity.Instance.TextProvider.GetRandomText(201 + oathId);
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

                if (factionId == 0 || factionId == (int)FactionFile.FactionIDs.The_Fighters_Guild)
                {
                    // Classic returns "BLANK" if no temple is found, here we return a random deity name.
                    // We do the same for Fighters Guild halls, which are are considered temples in some areas.
                    var god = GetRandomDivine();

                    return god.ToString();
                }

                Temple.Divines divine = Temple.GetDivine(factionId);
                return TextManager.Instance.GetLocalizedText(divine.ToString());
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

            private static FactionFile.FactionIDs GetRandomDivine()
            {
                switch (UnityEngine.Random.Range(0, 9))
                {
                    case 0: return FactionFile.FactionIDs.Arkay;
                    case 1: return FactionFile.FactionIDs.Zen;
                    case 2: return FactionFile.FactionIDs.Mara;
                    case 3: return FactionFile.FactionIDs.Ebonarm;
                    case 4: return FactionFile.FactionIDs.Akatosh;
                    case 5: return FactionFile.FactionIDs.Julianos;
                    case 6: return FactionFile.FactionIDs.Dibella;
                    case 7: return FactionFile.FactionIDs.Stendarr;
                    default: return FactionFile.FactionIDs.Kynareth;
                }
            }
        }
    }
}
