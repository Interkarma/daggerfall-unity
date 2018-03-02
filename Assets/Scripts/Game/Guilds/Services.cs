// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using System;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Guilds
{
    /// <summary>
    /// Supported guild services.
    /// </summary>
    public enum GuildServices
    {
        Training,
        Quests,

        Repair,
        Identify,

        Donate,
        CureDisease,

        BuyPotions,
        MakePotions,

        BuySpells,
        MakeSpells,

        BuyMagicItems,
        MakeMagicItems,
        SellMagicItems,

        Teleport,
        DaedraSummoning,

        Spymaster,
        BuySoulgems,

    }

    /// <summary>
    /// Npc factionId mapped to Guild service offered.
    /// Note this duplicates data from faction.txt mainly because guild flags are not consistent, and also for readability.
    /// </summary>
    public enum GuildNpcServices
    {
        // Mages Guild:
        MG_BuySpells = 60,
        MG_Training = 61,
        MG_Teleportation = 62,
        MG_Quests = 63,
        MG_MakeSpells = 64,
        MG_BuyMagicItems = 65,
        MG_DaedraSummoning = 66,
        MG_Identify = 801,
        MG_MakeMagicItems = 802,

        // Fighters Guild:
        FG_Training = 849,
        FG_Repair = 850,
        FG_Quests = 851,

        // Thieves Guild:
        TG_Training = 803,
        TG_Quests = 804,
        TG_SellMagicItems = 805,
        TG_Spymaster = 806,

        // Dark Brotherhood:
        DB_Quests = 807,
        DB_Training = 839,
        DB_MakePotions = 840,
        DB_BuyPotions = 841,
        DB_Spymaster = 842,
        DB_BuySoulgems = 843,

        // Temples, generic:
        T_Quests = 240,
        T_MakeDonation = 810,
        T_CureDiseases = 813,

        // Temples, specific:
        // Akatosh:
        TAk_Training = 247,
        TAk_BuyPotions = 473,
        TAk_MakePotions = 474,
        TAk_DaedraSummoning = 475,
        // Arkay:
        TAr_Training = 241,
        TAr_BuyPotions = 453,
        TAr_MakePotions = 454,
        TAr_DaedraSummoning = 456,
        TAr_BuySoulgems = 455,
        // Dibella:
        TDi_Training = 250,
        TDi_BuyPotions = 485,
        TDi_MakePotions = 487,
        TDi_DaedraSummoning = 488,
        // Julianos:
        TJu_Training = 249,
        TJu_BuyMagicItems = 480,
        TJu_MakeMagicItems = 481,
        TJu_DaedraSummoning = 482,
        // Kynareth:
        TKy_Training = 254,
        TKy_BuySpells = 496,
        TKy_MakeSpells = 497,
        TKy_DaedraSummoning = 498,
        // Mara:
        TMa_Training = 245,
        TMa_BuyPotions = 468,
        TMa_MakePotions = 469,
        TMa_DaedraSummoning = 470,
        // Stendarr
        TSt_Training = 252,
        TSt_BuyPotions = 490,
        TSt_MakePotions = 491,
        TSt_DaedraSummoning = 492,
        // Zenithar:
        TZe_Training = 243,
        TZe_BuyPotions = 462,
        TZe_MakePotions = 463,
        TZe_DaedraSummoning = 464,

        // Knightly orders:
        KO_Quests = 846,
        KO_Smith = 845,
        KO_Seneschal = 848,

    }

    public static class Services
    {
        public delegate void CustomGuildService();

        // Store for extra guild NPC services (i.e. from mods)
        private static Dictionary<int, GuildServices> guildNpcServices = new Dictionary<int, GuildServices>();
        private static Dictionary<int, string> customNpcServiceNames = new Dictionary<int, string>();
        private static Dictionary<int, CustomGuildService> customNpcServices = new Dictionary<int, CustomGuildService>();

        public static bool HasGuildService(int npcFactionId)
        {
            return (Enum.IsDefined(typeof(GuildNpcServices), npcFactionId) ||
                    guildNpcServices.ContainsKey(npcFactionId) ||
                    customNpcServices.ContainsKey(npcFactionId));
        }

        public static bool RegisterGuildService(int npcFactionId, GuildServices service)
        {
            if (!guildNpcServices.ContainsKey(npcFactionId))
            {
                guildNpcServices.Add(npcFactionId, service);
                return true;
            }
            return false;
        }

        public static bool RegisterGuildService(int npcFactionId, CustomGuildService service, string serviceName)
        {
            if (!customNpcServices.ContainsKey(npcFactionId))
            {
                customNpcServices.Add(npcFactionId, service);
                customNpcServiceNames.Add(npcFactionId, serviceName);
                return true;
            }
            return false;
        }

        public static GuildServices GetService(GuildNpcServices guildNpcService)
        {
            switch (guildNpcService)
            {
                case GuildNpcServices.MG_Training:
                case GuildNpcServices.FG_Training:
                case GuildNpcServices.TG_Training:
                case GuildNpcServices.DB_Training:
                case GuildNpcServices.TAk_Training:
                case GuildNpcServices.TAr_Training:
                case GuildNpcServices.TDi_Training:
                case GuildNpcServices.TJu_Training:
                case GuildNpcServices.TKy_Training:
                case GuildNpcServices.TMa_Training:
                case GuildNpcServices.TSt_Training:
                case GuildNpcServices.TZe_Training:
                    return GuildServices.Training;

                case GuildNpcServices.FG_Quests:
                case GuildNpcServices.MG_Quests:
                case GuildNpcServices.TG_Quests:
                case GuildNpcServices.DB_Quests:
                case GuildNpcServices.T_Quests:
                    return GuildServices.Quests;

                case GuildNpcServices.FG_Repair:
                    return GuildServices.Repair;

                case GuildNpcServices.MG_Identify:
                    return GuildServices.Identify;

                case GuildNpcServices.T_MakeDonation:
                    return GuildServices.Donate;

                case GuildNpcServices.T_CureDiseases:
                    return GuildServices.CureDisease;

                case GuildNpcServices.DB_BuyPotions:
                case GuildNpcServices.TAk_BuyPotions:
                case GuildNpcServices.TAr_BuyPotions:
                case GuildNpcServices.TDi_BuyPotions:
                case GuildNpcServices.TMa_BuyPotions:
                case GuildNpcServices.TSt_BuyPotions:
                case GuildNpcServices.TZe_BuyPotions:
                    return GuildServices.BuyPotions;

                case GuildNpcServices.DB_MakePotions:
                case GuildNpcServices.TAk_MakePotions:
                case GuildNpcServices.TAr_MakePotions:
                case GuildNpcServices.TDi_MakePotions:
                case GuildNpcServices.TMa_MakePotions:
                case GuildNpcServices.TSt_MakePotions:
                case GuildNpcServices.TZe_MakePotions:
                    return GuildServices.MakePotions;

                case GuildNpcServices.MG_BuySpells:
                case GuildNpcServices.TKy_BuySpells:
                    return GuildServices.BuySpells;

                case GuildNpcServices.MG_MakeSpells:
                case GuildNpcServices.TKy_MakeSpells:
                    return GuildServices.MakeSpells;

                case GuildNpcServices.MG_BuyMagicItems:
                case GuildNpcServices.TJu_BuyMagicItems:
                    return GuildServices.BuyMagicItems;

                case GuildNpcServices.MG_MakeMagicItems:
                case GuildNpcServices.TJu_MakeMagicItems:
                    return GuildServices.MakeMagicItems;

                case GuildNpcServices.TG_SellMagicItems:
                    return GuildServices.BuyMagicItems;

                case GuildNpcServices.MG_Teleportation:
                    return GuildServices.Teleport;

                case GuildNpcServices.MG_DaedraSummoning:
                case GuildNpcServices.TAk_DaedraSummoning:
                case GuildNpcServices.TAr_DaedraSummoning:
                case GuildNpcServices.TDi_DaedraSummoning:
                case GuildNpcServices.TJu_DaedraSummoning:
                case GuildNpcServices.TKy_DaedraSummoning:
                case GuildNpcServices.TMa_DaedraSummoning:
                case GuildNpcServices.TSt_DaedraSummoning:
                case GuildNpcServices.TZe_DaedraSummoning:
                    return GuildServices.DaedraSummoning;

                case GuildNpcServices.TG_Spymaster:
                case GuildNpcServices.DB_Spymaster:
                    return GuildServices.Spymaster;

                case GuildNpcServices.DB_BuySoulgems:
                case GuildNpcServices.TAr_BuySoulgems:
                    return GuildServices.BuySoulgems;
            }

            GuildServices service;
            if (guildNpcServices.TryGetValue((int) guildNpcService, out service))
                return service;
            else
                return (GuildServices) guildNpcService;
        }

        public static bool GetCustomGuildService(int npcFactionId, out CustomGuildService customGuildService)
        {
            return customNpcServices.TryGetValue(npcFactionId, out customGuildService);
        }

        public static string GetServiceLabelText(GuildServices service)
        {
            switch (service)
            {
                case GuildServices.Training:
                    return HardStrings.serviceTraining;
                case GuildServices.Quests:
                    return HardStrings.serviceQuests;
                case GuildServices.Repair:
                    return HardStrings.serviceRepairs;
                case GuildServices.Identify:
                    return HardStrings.serviceIdentify;
                case GuildServices.Donate:
                    return HardStrings.serviceDonate;
                case GuildServices.CureDisease:
                    return HardStrings.serviceCure;
                case GuildServices.BuyPotions:
                    return HardStrings.serviceBuyPotions;
                case GuildServices.MakePotions:
                    return HardStrings.serviceMakePotions;
                case GuildServices.BuySpells:
                    return HardStrings.serviceBuySpells;
                case GuildServices.MakeSpells:
                    return HardStrings.serviceMakeSpells;
                case GuildServices.BuyMagicItems:
                    return HardStrings.serviceBuyMagicItems;
                case GuildServices.MakeMagicItems:
                    return HardStrings.serviceMakeMagicItems;
                case GuildServices.SellMagicItems:
                    return HardStrings.serviceBuyMagicItems;
                case GuildServices.Teleport:
                    return HardStrings.serviceTeleport;
                case GuildServices.DaedraSummoning:
                    return HardStrings.serviceDaedraSummon;
                case GuildServices.Spymaster:
                    return HardStrings.serviceSpymaster;
                case GuildServices.BuySoulgems:
                    return HardStrings.serviceBuySoulgems;
                default:
                    string serviceName;
                    if (customNpcServiceNames.TryGetValue((int)service, out serviceName))
                        return serviceName;
                    else
                        return "?";
            }
        }
    }
}
