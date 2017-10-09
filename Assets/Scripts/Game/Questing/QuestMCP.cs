// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using System;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

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

            // He/She
            public override string Pronoun()
            {
                if (parent.LastPersonReferenced == null)
                    return HardStrings.pronounHe;

                switch (parent.LastPersonReferenced.Gender)
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
                if (parent.LastPersonReferenced == null)
                    return HardStrings.pronounHim;

                switch (parent.LastPersonReferenced.Gender)
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
                if (parent.LastPersonReferenced == null)
                    return HardStrings.pronounHimself;

                switch (parent.LastPersonReferenced.Gender)
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
                if (parent.LastPersonReferenced == null)
                    return HardStrings.pronounHis;

                switch (parent.LastPersonReferenced.Gender)
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
                return parent.QuestStartTime.DateString();
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
                return (parent.LastPersonReferenced != null) ? parent.LastPersonReferenced.HomeRegionName : "";
            }

            public override string God()
            {
                // Get god of current NPC or fallback
                if (parent.LastPersonReferenced != null)
                    return parent.LastPersonReferenced.GodName;
                else
                    return "Arkay";
            }

            public override string LocationDirection()
            {
                Vector2 positionPlayer;
                Vector2 positionLocation = Vector2.zero;

                DFPosition position = new DFPosition();
                PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
                if (playerGPS)
                    position = playerGPS.CurrentMapPixel;
                
                positionPlayer = new Vector2(position.X, position.Y);

                int region = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(position.X, position.Y) - 128;
                if (region < 0 || region >= DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount)
                    region = -1;
                
                DFRegion.RegionMapTable locationInfo = new DFRegion.RegionMapTable();

                DFRegion currentDFRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(region);

                //string[] locations = currentDFRegion.MapNames;
                

                /*
                for (int i = 0; i < locations.Length; i++)
                {
                    if (locations[i].ToLower() == name) // Valid location found with exact name
                    {
                        if (currentDFRegion.MapNameLookup.ContainsKey(locations[i]))
                        {
                            int index = currentDFRegion.MapNameLookup[locations[i]];
                            locationInfo = currentDFRegion.MapTable[index];
                            position = MapsFile.LongitudeLatitudeToMapPixel((int)locationInfo.Longitude, (int)locationInfo.Latitude);
                            positionLocation = new Vector2(position.X, position.Y);
                        }
                    }
                }
                */
                DFLocation locationOut;
                DaggerfallUnity.Instance.ContentReader.GetLocation(region, (int)this.parent.LastPlaceReferenced.SiteDetails.locationId, out locationOut);
                locationInfo = locationOut.MapTableData;
                position = MapsFile.LongitudeLatitudeToMapPixel((int)locationInfo.Longitude, (int)locationInfo.Latitude);
                positionLocation = new Vector2(position.X, position.Y);

                string name = locationOut.Name.ToLower();

                if (positionLocation != Vector2.zero)
                {
                    Vector2 vecDirectionToTarget = positionLocation - positionPlayer;
                    vecDirectionToTarget.y = -vecDirectionToTarget.y; // invert y axis
                    return GameManager.Instance.TalkManager.DirectionVector2DirectionHintString(vecDirectionToTarget);
                }
                else
                {
                    return "... never mind ...";
                }
            }
        }
    }
}
