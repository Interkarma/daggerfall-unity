// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    public static class ClimateSwaps
    {
        /// <summary>
        /// Converts an archive index to new climate and season.
        /// Will return same index if climate or season not supported.
        /// </summary>
        /// <param name="archive">Archive index of starting texture.</param>
        /// <param name="climate">Climate base to apply.</param>
        /// <param name="season">Climate season to apply</param>
        /// <returns>Archive index of new texture.</returns>
        public static int ApplyClimate(int archive, int record, ClimateBases climate, ClimateSeason season)
        {
            // Get climate texture info
            ClimateTextureInfo ci = ClimateSwaps.GetClimateTextureInfo(archive);

            // Ignore non-climate textures
            if (ci.textureSet == DFLocation.ClimateTextureSet.None)
                return archive;

            // Handle missing Swamp textures
            if (climate == ClimateBases.Swamp)
            {
                switch (ci.textureSet)
                {
                    case DFLocation.ClimateTextureSet.Interior_TempleInt:
                    case DFLocation.ClimateTextureSet.Interior_MarbleFloors:
                        return archive;
                }
            }

            // Bypass winter swaps in desert climates entirely
            // There are too many bad swaps, and you never see this variant in game
            if (climate == ClimateBases.Desert)
                ci.supportsWinter = false;

            // Handle swamp climate sets with missing winter textures
            if (climate == ClimateBases.Swamp)
            {
                switch (ci.textureSet)
                {
                    case DFLocation.ClimateTextureSet.Exterior_Castle:
                    case DFLocation.ClimateTextureSet.Exterior_MagesGuild:
                        ci.supportsWinter = false;
                        break;
                }
            }

            // Handle archives with missing winter textures
            if (archive == 82 && record > 1 ||
                archive == 77)
            {
                ci.supportsWinter = false;
            }

            // Flag to suppress climate index
            // Certain textures have a winter variant but are climate-specific
            bool suppressClimateIndex = false;
            switch (archive)
            {
                case 75:
                case 76:
                case 77:
                case 79:
                case 80:
                case 82:
                case 83:
                    suppressClimateIndex = true;
                    break;
            }

            // Calculate new index
            int climateIndex = 0;
            if (archive < 500 && !suppressClimateIndex)
            {
                climateIndex = (int)FromUnityClimateBase(climate) + (int)ci.textureSet;
                if (season == ClimateSeason.Winter && ci.supportsWinter)
                    climateIndex += (int)DFLocation.ClimateWeather.Winter;
                else if (season == ClimateSeason.Rain && ci.supportsRain)
                    climateIndex += (int)DFLocation.ClimateWeather.Rain;
            }
            else
            {
                climateIndex = archive;
                if (season == ClimateSeason.Winter && ci.supportsWinter)
                    climateIndex += (int)DFLocation.ClimateWeather.Winter;
            }

            return climateIndex;
        }

        /// <summary>
        /// Get climate texture information.
        /// </summary>
        /// <param name="archive">Texture archive index.</param>
        /// <returns>ClimateTextureInfo.</returns>
        public static ClimateTextureInfo GetClimateTextureInfo(int archive)
        {
            // Create new climate texture information
            ClimateTextureInfo ci = new ClimateTextureInfo();
            ci.textureSet = DFLocation.ClimateTextureSet.None;

            // Handle nature sets
            if (archive > 499)
            {
                ci.textureSet = (DFLocation.ClimateTextureSet)archive;
                ci.textureGroup = DFLocation.ClimateTextureGroup.Nature;
                switch (ci.textureSet)
                {
                    // Nature sets without snow
                    case DFLocation.ClimateTextureSet.Nature_RainForest:
                    case DFLocation.ClimateTextureSet.Nature_SubTropical:
                    case DFLocation.ClimateTextureSet.Nature_Swamp:
                    case DFLocation.ClimateTextureSet.Nature_Desert:
                        break;

                    // Nature sets with snow
                    case DFLocation.ClimateTextureSet.Nature_TemperateWoodland:
                    case DFLocation.ClimateTextureSet.Nature_WoodlandHills:
                    case DFLocation.ClimateTextureSet.Nature_HauntedWoodlands:
                    case DFLocation.ClimateTextureSet.Nature_Mountains:
                        ci.supportsWinter = true;
                        break;

                    // No match
                    default:
                        ci.textureGroup = DFLocation.ClimateTextureGroup.None;
                        ci.textureSet = DFLocation.ClimateTextureSet.None;
                        break;
                }

                return ci;
            }

            // Get general set
            ci.textureSet = (DFLocation.ClimateTextureSet)(archive - (archive / 100) * 100);
            switch (ci.textureSet)
            {
                // Terrain sets (support winter and rain)
                case DFLocation.ClimateTextureSet.Exterior_Terrain:
                    ci.textureGroup = DFLocation.ClimateTextureGroup.Terrain;
                    ci.supportsWinter = true;
                    ci.supportsRain = true;
                    break;

                // Exterior sets (supports winter)
                case DFLocation.ClimateTextureSet.Exterior_Ruins:
                case DFLocation.ClimateTextureSet.Exterior_Castle:
                case DFLocation.ClimateTextureSet.Exterior_CityA:
                case DFLocation.ClimateTextureSet.Exterior_CityB:
                case DFLocation.ClimateTextureSet.Exterior_CityWalls:
                case DFLocation.ClimateTextureSet.Exterior_Farm:
                case DFLocation.ClimateTextureSet.Exterior_Fences:
                case DFLocation.ClimateTextureSet.Exterior_MagesGuild:
                case DFLocation.ClimateTextureSet.Exterior_Manor:
                case DFLocation.ClimateTextureSet.Exterior_MerchantHomes:
                case DFLocation.ClimateTextureSet.Exterior_TavernExteriors:
                case DFLocation.ClimateTextureSet.Exterior_TempleExteriors:
                case DFLocation.ClimateTextureSet.Exterior_Village:
                case DFLocation.ClimateTextureSet.Exterior_Roofs:
                case DFLocation.ClimateTextureSet.Exterior_CitySpec:
                case DFLocation.ClimateTextureSet.Exterior_CitySpec_Snow:
                case DFLocation.ClimateTextureSet.Exterior_CitySpecB:
                case DFLocation.ClimateTextureSet.Exterior_CitySpecB_Snow:
                    ci.textureGroup = DFLocation.ClimateTextureGroup.Exterior;
                    ci.supportsWinter = true;
                    break;

                // Exterior sets (do not support winter)
                case DFLocation.ClimateTextureSet.Exterior_Doors:
                    ci.supportsWinter = false;
                    break;

                // Interior sets (do not support winter)
                case DFLocation.ClimateTextureSet.Interior_PalaceInt:
                case DFLocation.ClimateTextureSet.Interior_CityInt:
                case DFLocation.ClimateTextureSet.Interior_CryptA:
                case DFLocation.ClimateTextureSet.Interior_CryptB:
                case DFLocation.ClimateTextureSet.Interior_DungeonsA:
                case DFLocation.ClimateTextureSet.Interior_DungeonsB:
                case DFLocation.ClimateTextureSet.Interior_DungeonsC:
                case DFLocation.ClimateTextureSet.Interior_DungeonsNEWCs:
                case DFLocation.ClimateTextureSet.Interior_FarmInt:
                case DFLocation.ClimateTextureSet.Interior_MagesGuildInt:
                case DFLocation.ClimateTextureSet.Interior_ManorInt:
                case DFLocation.ClimateTextureSet.Interior_MarbleFloors:
                case DFLocation.ClimateTextureSet.Interior_MerchantHomesInt:
                case DFLocation.ClimateTextureSet.Interior_Mines:
                case DFLocation.ClimateTextureSet.Interior_Caves:
                case DFLocation.ClimateTextureSet.Interior_Paintings:
                case DFLocation.ClimateTextureSet.Interior_TavernInt:
                case DFLocation.ClimateTextureSet.Interior_TempleInt:
                case DFLocation.ClimateTextureSet.Interior_VillageInt:
                case DFLocation.ClimateTextureSet.Interior_Sewer:
                    ci.textureGroup = DFLocation.ClimateTextureGroup.Interior;
                    break;

                // No match found, revert to non-climate settings
                default:
                    ci.textureGroup = DFLocation.ClimateTextureGroup.None;
                    ci.textureSet = DFLocation.ClimateTextureSet.None;
                    break;
            }

            return ci;
        }

        /// <summary>
        /// Checks if the texture is a colour-changing exterior window.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <returns>True if exterior window.</returns>
        public static bool IsExteriorWindow(int archive, int record)
        {
            // Normalise archive index
            archive = (archive - (archive / 100) * 100);

            // First check if texture archive even has a window, based on known archives
            switch (archive)
            {
                // General texture sets have a window at index 3
                case 009:
                case 010:
                case 012:
                case 013:
                case 014:
                case 015:
                case 026:
                case 027:
                case 035:
                case 038:
                case 039:
                case 042:
                case 043:
                case 058:
                case 059:
                case 061:
                case 062:
                case 064:
                case 065:
                    if (record == 3) return true;
                    break;

                // Gable texture sets are all windows
                case 075:
                case 076:
                case 077:
                    return true;

                // CitySpecA has windows at index 0 and 2
                case 79:
                case 80:
                    if (record == 0 || record == 2) return true;
                    break;

                // CitySpecB has windows at index 0
                case 82:
                case 83:
                    if (record == 0) return true;
                    break;
            }

            return false;
        }

        /// <summary>
        /// Convert DaggerfallUnity climate base to API equivalent.
        /// </summary>
        /// <param name="climate">ClimateBases.</param>
        /// <returns>DFLocation.ClimateBaseType.</returns>
        public static DFLocation.ClimateBaseType FromUnityClimateBase(ClimateBases climate)
        {
            switch (climate)
            {
                case ClimateBases.Desert:
                    return DFLocation.ClimateBaseType.Desert;
                case ClimateBases.Mountain:
                    return DFLocation.ClimateBaseType.Mountain;
                case ClimateBases.Temperate:
                    return DFLocation.ClimateBaseType.Temperate;
                case ClimateBases.Swamp:
                    return DFLocation.ClimateBaseType.Swamp;
                default:
                    return DFLocation.ClimateBaseType.None;
            }
        }

        /// <summary>
        /// Convert API climate base over to DaggerfallUnity equivalent.
        /// </summary>
        /// <param name="climate">DFLocation.ClimateBaseType.</param>
        /// <returns>ClimateBases.</returns>
        public static ClimateBases FromAPIClimateBase(DFLocation.ClimateBaseType climate)
        {
            switch (climate)
            {
                case DFLocation.ClimateBaseType.Desert:
                    return ClimateBases.Desert;
                case DFLocation.ClimateBaseType.Mountain:
                    return ClimateBases.Mountain;
                case DFLocation.ClimateBaseType.Temperate:
                    return ClimateBases.Temperate;
                case DFLocation.ClimateBaseType.Swamp:
                    return ClimateBases.Swamp;
                default:
                    return ClimateBases.Temperate;
            }
        }

        /// <summary>
        /// Convert API nature set to DaggerfallUnity equivalent.
        /// </summary>
        /// <param name="set"></param>
        /// <returns>ClimateNatureSets.</returns>
        public static ClimateNatureSets FromAPITextureSet(DFLocation.ClimateTextureSet set)
        {
            switch (set)
            {
                case DFLocation.ClimateTextureSet.Nature_RainForest:
                    return ClimateNatureSets.RainForest;
                case DFLocation.ClimateTextureSet.Nature_SubTropical:
                    return ClimateNatureSets.SubTropical;
                case DFLocation.ClimateTextureSet.Nature_Swamp:
                    return ClimateNatureSets.Swamp;
                case DFLocation.ClimateTextureSet.Nature_Desert:
                    return ClimateNatureSets.Desert;
                case DFLocation.ClimateTextureSet.Nature_TemperateWoodland:
                    return ClimateNatureSets.TemperateWoodland;
                case DFLocation.ClimateTextureSet.Nature_WoodlandHills:
                    return ClimateNatureSets.WoodlandHills;
                case DFLocation.ClimateTextureSet.Nature_HauntedWoodlands:
                    return ClimateNatureSets.HauntedWoodlands;
                case DFLocation.ClimateTextureSet.Nature_Mountains:
                    return ClimateNatureSets.Mountains;
                default:
                    return ClimateNatureSets.TemperateWoodland;
            }
        }

        public static int GetNatureArchive(DFLocation.ClimateTextureSet climateTextureSet, DaggerfallDateTime.Seasons worldSeason)
        {
            ClimateNatureSets natureSet = FromAPITextureSet(climateTextureSet);
            ClimateSeason climateSeason = ClimateSeason.Summer;
            if (worldSeason == DaggerfallDateTime.Seasons.Winter)
                climateSeason = ClimateSeason.Winter;

            return GetNatureArchive(natureSet, climateSeason);
        }

        public static int GetNatureArchive(ClimateNatureSets natureSet, ClimateSeason climateSeason)
        {
            // Get base set
            int archive;
            switch (natureSet)
            {
                case ClimateNatureSets.RainForest:
                    archive = 500;
                    break;
                case ClimateNatureSets.SubTropical:
                    archive = 501;
                    break;
                case ClimateNatureSets.Swamp:
                    archive = 502;
                    break;
                case ClimateNatureSets.Desert:
                    archive = 503;
                    break;
                case ClimateNatureSets.TemperateWoodland:
                    archive = 504;
                    break;
                case ClimateNatureSets.WoodlandHills:
                    archive = 506;
                    break;
                case ClimateNatureSets.HauntedWoodlands:
                    archive = 508;
                    break;
                case ClimateNatureSets.Mountains:
                    archive = 510;
                    break;
                default:
                    archive = 504;
                    break;
            }

            // Winter modifier
            if (climateSeason == ClimateSeason.Winter)
            {
                // Only certain sets have a winter archive
                switch (natureSet)
                {
                    case ClimateNatureSets.TemperateWoodland:
                    case ClimateNatureSets.WoodlandHills:
                    case ClimateNatureSets.HauntedWoodlands:
                    case ClimateNatureSets.Mountains:
                        archive += 1;
                        break;
                }
            }

            return archive;
        }

        /// <summary>
        /// Get ground archive based on climate.
        /// </summary>
        /// <param name="climateBase">Climate base.</param>
        /// <param name="climateSeason">Season.</param>
        /// <returns>Ground archive matching climate and season.</returns>
        public static int GetGroundArchive(ClimateBases climateBase, ClimateSeason climateSeason)
        {
            // Apply climate
            int archive;
            switch (climateBase)
            {
                case ClimateBases.Desert:
                    archive = 2;
                    break;
                case ClimateBases.Mountain:
                    archive = 102;
                    break;
                case ClimateBases.Temperate:
                    archive = 302;
                    break;
                case ClimateBases.Swamp:
                    archive = 402;
                    break;
                default:
                    archive = 302;
                    break;
            }

            // Modify for season
            switch (climateSeason)
            {
                case ClimateSeason.Winter:
                    archive += 1;
                    break;
                case ClimateSeason.Rain:
                    archive += 2;
                    break;
            }

            return archive;
        }
    }
}