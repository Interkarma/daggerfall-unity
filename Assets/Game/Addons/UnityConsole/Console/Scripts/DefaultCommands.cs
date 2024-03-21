using UnityEngine;
using Wenzil.Console.Commands;
using DaggerfallWorkshop.Game;
using System.Linq;
using System;
using DaggerfallConnect.Arena2;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Entity;
using System.IO;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Weather;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

namespace Wenzil.Console
{
    public class DefaultCommands : MonoBehaviour
    {
        public static bool showDebugStrings = false;

        void Start()
        {
            ConsoleCommandsDatabase.RegisterCommand(QuitCommand.name, QuitCommand.description, QuitCommand.usage, QuitCommand.Execute);
            ConsoleCommandsDatabase.RegisterCommand(HelpCommand.name, HelpCommand.description, HelpCommand.usage, HelpCommand.Execute);
            ConsoleCommandsDatabase.RegisterCommand(LoadCommand.name, LoadCommand.description, LoadCommand.usage, LoadCommand.Execute);
            ConsoleCommandsDatabase.RegisterCommand(GodCommand.name, GodCommand.description, GodCommand.usage, GodCommand.Execute);
            ConsoleCommandsDatabase.RegisterCommand(NoClipCommand.name, NoClipCommand.description, NoClipCommand.usage, NoClipCommand.Execute);
            ConsoleCommandsDatabase.RegisterCommand(NoTargetCommand.name, NoTargetCommand.description, NoTargetCommand.usage, NoTargetCommand.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ToggleAICommand.name, ToggleAICommand.description, ToggleAICommand.usage, ToggleAICommand.Execute);
            ConsoleCommandsDatabase.RegisterCommand(CreateMobileCommand.name, CreateMobileCommand.description, CreateMobileCommand.usage, CreateMobileCommand.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Suicide.name, Suicide.description, Suicide.usage, Suicide.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ShowDebugStrings.name, ShowDebugStrings.description, ShowDebugStrings.usage, ShowDebugStrings.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetWeather.name, SetWeather.description, SetWeather.usage, SetWeather.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToMapPixel.name, TeleportToMapPixel.description, TeleportToMapPixel.usage, TeleportToMapPixel.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToDungeonDoor.name, TeleportToDungeonDoor.description, TeleportToDungeonDoor.usage, TeleportToDungeonDoor.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToQuestSpawnMarker.name, TeleportToQuestSpawnMarker.description, TeleportToQuestSpawnMarker.usage, TeleportToQuestSpawnMarker.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToQuestItemMarker.name, TeleportToQuestItemMarker.description, TeleportToQuestItemMarker.usage, TeleportToQuestItemMarker.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToQuestMarker.name, TeleportToQuestMarker.description, TeleportToQuestMarker.usage, TeleportToQuestMarker.Execute);
            ConsoleCommandsDatabase.RegisterCommand(GetAllQuestItems.name, GetAllQuestItems.description, GetAllQuestItems.usage, GetAllQuestItems.Execute);
            ConsoleCommandsDatabase.RegisterCommand(EndDebugQuest.name, EndDebugQuest.description, EndDebugQuest.usage, EndDebugQuest.Execute);
            ConsoleCommandsDatabase.RegisterCommand(EndQuest.name, EndQuest.description, EndQuest.usage, EndQuest.Execute);
            ConsoleCommandsDatabase.RegisterCommand(PurgeAllQuests.name, PurgeAllQuests.description, PurgeAllQuests.usage, PurgeAllQuests.Execute);
            ConsoleCommandsDatabase.RegisterCommand(PurgeNonStoryQuests.name, PurgeNonStoryQuests.description, PurgeNonStoryQuests.usage, PurgeNonStoryQuests.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ModNPCRep.name, ModNPCRep.description, ModNPCRep.usage, ModNPCRep.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetLevel.name, SetLevel.description, SetLevel.usage, SetLevel.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Levitate.name, Levitate.description, Levitate.usage, Levitate.Execute);
            ConsoleCommandsDatabase.RegisterCommand(OpenAllDoors.name, OpenAllDoors.description, OpenAllDoors.usage, OpenAllDoors.Execute);
            ConsoleCommandsDatabase.RegisterCommand(OpenDoor.name, OpenDoor.description, OpenDoor.usage, OpenDoor.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ActivateAction.name, ActivateAction.description, ActivateAction.usage, ActivateAction.Execute);
            ConsoleCommandsDatabase.RegisterCommand(KillAllEnemies.name, KillAllEnemies.description, KillAllEnemies.usage, KillAllEnemies.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TransitionToExterior.name, TransitionToExterior.description, TransitionToExterior.usage, TransitionToExterior.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetHealth.name, SetHealth.description, SetHealth.usage, SetHealth.Execute);
            ConsoleCommandsDatabase.RegisterCommand(RerollMaxHealth.name, RerollMaxHealth.description, RerollMaxHealth.usage, RerollMaxHealth.Execute);

            ConsoleCommandsDatabase.RegisterCommand(ResetAssets.name, ResetAssets.description, ResetAssets.usage, ResetAssets.Execute);
            ConsoleCommandsDatabase.RegisterCommand(RetryAssetImports.name, RetryAssetImports.description, RetryAssetImports.usage, RetryAssetImports.Execute);

            ConsoleCommandsDatabase.RegisterCommand(SetWalkSpeed.name, SetWalkSpeed.description, SetWalkSpeed.usage, SetWalkSpeed.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetMouseSensitivity.name, SetMouseSensitivity.description, SetMouseSensitivity.usage, SetMouseSensitivity.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddPopupText.name, AddPopupText.description, AddPopupText.usage, AddPopupText.Execute);

            ConsoleCommandsDatabase.RegisterCommand(SetVSync.name, SetVSync.description, SetVSync.usage, SetVSync.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetRunSpeed.name, SetRunSpeed.description, SetRunSpeed.usage, SetRunSpeed.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetJumpSpeed.name, SetJumpSpeed.description, SetJumpSpeed.usage, SetJumpSpeed.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ToggleAirControl.name, ToggleAirControl.description, ToggleAirControl.usage, ToggleAirControl.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetTimeScale.name, SetTimeScale.description, SetTimeScale.usage, SetTimeScale.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetGravity.name, SetGravity.description, SetGravity.usage, SetGravity.Execute);

            ConsoleCommandsDatabase.RegisterCommand(GotoLocation.name, GotoLocation.description, GotoLocation.usage, GotoLocation.Execute);
            ConsoleCommandsDatabase.RegisterCommand(GetLocationMapPixel.name, GetLocationMapPixel.description, GetLocationMapPixel.usage, GetLocationMapPixel.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Teleport.name, Teleport.description, Teleport.usage, Teleport.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Groundme.name, Groundme.description, Groundme.usage, Groundme.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ExecuteScript.name, ExecuteScript.description, ExecuteScript.usage, ExecuteScript.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddInventoryItem.name, AddInventoryItem.description, AddInventoryItem.usage, AddInventoryItem.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddArtifact.name, AddArtifact.description, AddArtifact.usage, AddArtifact.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddWeapon.name, AddWeapon.description, AddWeapon.usage, AddWeapon.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddArmor.name, AddArmor.description, AddArmor.usage, AddArmor.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddClothing.name, AddClothing.description, AddClothing.usage, AddClothing.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddAllEquip.name, AddAllEquip.description, AddAllEquip.usage, AddAllEquip.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddBook.name, AddBook.description, AddBook.usage, AddBook.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ShowBankWindow.name, ShowBankWindow.description, ShowBankWindow.usage, ShowBankWindow.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ShowSpellmakerWindow.name, ShowSpellmakerWindow.description, ShowSpellmakerWindow.usage, ShowSpellmakerWindow.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ShowItemMakerWindow.name, ShowItemMakerWindow.description, ShowItemMakerWindow.usage, ShowItemMakerWindow.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ShowPotionMakerWindow.name, ShowPotionMakerWindow.description, ShowPotionMakerWindow.usage, ShowPotionMakerWindow.Execute);
            ConsoleCommandsDatabase.RegisterCommand(AddSpellBook.name, AddSpellBook.description, AddSpellBook.usage, AddSpellBook.Execute);
            ConsoleCommandsDatabase.RegisterCommand(StartQuest.name, StartQuest.usage, StartQuest.description, StartQuest.Execute);

            ConsoleCommandsDatabase.RegisterCommand(DiseasePlayer.name, DiseasePlayer.usage, DiseasePlayer.description, DiseasePlayer.Execute);
            ConsoleCommandsDatabase.RegisterCommand(PoisonPlayer.name, PoisonPlayer.usage, PoisonPlayer.description, PoisonPlayer.Execute);

            ConsoleCommandsDatabase.RegisterCommand(DumpRegion.name, DumpRegion.description, DumpRegion.usage, DumpRegion.Execute);
            ConsoleCommandsDatabase.RegisterCommand(DumpLocation.name, DumpLocation.description, DumpLocation.usage, DumpLocation.Execute);
            ConsoleCommandsDatabase.RegisterCommand(DumpBlock.name, DumpBlock.description, DumpBlock.usage, DumpBlock.Execute);
            ConsoleCommandsDatabase.RegisterCommand(DumpLocBlocks.name, DumpLocBlocks.description, DumpLocBlocks.usage, DumpLocBlocks.Execute);
            ConsoleCommandsDatabase.RegisterCommand(DumpBuilding.name, DumpBuilding.description, DumpBuilding.usage, DumpBuilding.Execute);
            ConsoleCommandsDatabase.RegisterCommand(IngredientUsage.name, IngredientUsage.description, IngredientUsage.usage, IngredientUsage.Execute);

            ConsoleCommandsDatabase.RegisterCommand(PlayFLC.name, PlayFLC.description, PlayFLC.usage, PlayFLC.Execute);
            ConsoleCommandsDatabase.RegisterCommand(PlayVID.name, PlayVID.description, PlayVID.usage, PlayVID.Execute);
            ConsoleCommandsDatabase.RegisterCommand(PrintCrimeCommitted.name, PrintCrimeCommitted.description, PrintCrimeCommitted.usage, PrintCrimeCommitted.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ClearCrimeCommitted.name, ClearCrimeCommitted.description, ClearCrimeCommitted.usage, ClearCrimeCommitted.Execute);
            ConsoleCommandsDatabase.RegisterCommand(PrintLegalRep.name, PrintLegalRep.description, PrintLegalRep.usage, PrintLegalRep.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ClearNegativeLegalRep.name, ClearNegativeLegalRep.description, ClearNegativeLegalRep.usage, ClearNegativeLegalRep.Execute);
            ConsoleCommandsDatabase.RegisterCommand(RefreshBuildingNames.name, RefreshBuildingNames.description, RefreshBuildingNames.usage, RefreshBuildingNames.Execute);

            ConsoleCommandsDatabase.RegisterCommand(PrintQuests.name, PrintQuests.description, PrintQuests.usage, PrintQuests.Execute);

            ConsoleCommandsDatabase.RegisterCommand(SummonDaedra.name, SummonDaedra.description, SummonDaedra.usage, SummonDaedra.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ChangeModSettings.name, ChangeModSettings.description, ChangeModSettings.usage, ChangeModSettings.Execute);
        }

        private static class DumpRegion
        {
            public static readonly string name = "dumpregion";
            public static readonly string error = "Player not in a region, unable to dump";
            public static readonly string usage = "dumpregion";
            public static readonly string description = "Dump the current region (from MAPS.BSA) that the player is currently in to json file";

            public static string Execute(params string[] args)
            {
                if (args.Length != 0)
                {
                    return HelpCommand.Execute(DumpRegion.name);
                }
                else
                {
                    PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
                    if (playerGPS)
                    {
                        DFRegion region = playerGPS.CurrentRegion;

                        string regionJson = SaveLoadManager.Serialize(region.GetType(), region);
                        string fileName = WorldDataReplacement.GetDFRegionReplacementFilename(playerGPS.CurrentRegionIndex);
                        File.WriteAllText(Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName), regionJson);
                        return "Region data json written to " + Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName);
                    }
                    return error;
                }
            }
        }

        private static class DumpLocation
        {
            public static readonly string name = "dumplocation";
            public static readonly string error = "Player not at a location, unable to dump";
            public static readonly string usage = "dumplocation";
            public static readonly string description = "Dump the current location (from MAPS.BSA) that the player is currently in to json file";

            public static string Execute(params string[] args)
            {
                if (args.Length != 0)
                {
                    return HelpCommand.Execute(DumpLocation.name);
                }
                else
                {
                    PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
                    if (playerGPS.HasCurrentLocation)
                    {
                        DFLocation location = playerGPS.CurrentLocation;

                        string locJson = SaveLoadManager.Serialize(location.GetType(), location);
                        string fileName = WorldDataReplacement.GetDFLocationReplacementFilename(location.RegionIndex, location.LocationIndex);
                        File.WriteAllText(Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName), locJson);
                        return "Location data json written to " + Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName);
                    }
                    return error;
                }
            }
        }

        private static class DumpBlock
        {
            public static readonly string name = "dumpblock";
            public static readonly string error = "Failed to dump block";
            public static readonly string usage = "dumpblock [blockName]";
            public static readonly string description = "Dump a block to json file, or index if no block name specified";

            public static string Execute(params string[] args)
            {
                if (args.Length == 0)
                {
                    string blockIndex = "";
                    BsaFile blockBsa = DaggerfallUnity.Instance.ContentReader.BlockFileReader.BsaFile;
                    for (int b = 0; b < blockBsa.Count; b++)
                    {
                        blockIndex += string.Format("{0}: {1}\n", b, blockBsa.GetRecordName(b));
                    }
                    string fileName = Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, "BlockIndex.txt");
                    File.WriteAllText(fileName, blockIndex);
                    return "Block index data written to " + Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName);
                }
                else
                {
                    DFBlock blockData;
                    if (!RMBLayout.GetBlockData(args[0], out blockData))
                    {
                        if (args[0].EndsWith(".RDB", StringComparison.InvariantCultureIgnoreCase))
                        {
                            blockData = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(args[0]);
                        }
                    }

                    if (!string.IsNullOrEmpty(blockData.Name))
                    {
                        string blockJson = SaveLoadManager.Serialize(blockData.GetType(), blockData);
                        string fileName = WorldDataReplacement.GetDFBlockReplacementFilename(blockData.Name);
                        File.WriteAllText(Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName), blockJson);
                        return "Block data json written to " + Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName);
                    }
                    return error;
                }
            }
        }

        private static class DumpLocBlocks
        {
            public static readonly string name = "dumplocblocks";
            public static readonly string error = "Failed to dump locations";
            public static readonly string usage = "dumplocblocks [locindex|(blockName.RMB )*]\nExamples:\ndumplocblocks\ndumplocblocks locindex\ndumplocblocks RESIAM10.RMB GEMSAM02.RMB";
            public static readonly string description = "Dump the names of blocks for each location, location index or locations for list of given block(s), to json file";

            public static string Execute(params string[] args)
            {
                MapsFile mapFileReader = DaggerfallUnity.Instance.ContentReader.MapFileReader;
                if (args.Length == 0)
                {
                    Dictionary<string, string[]> locBlocks = new Dictionary<string, string[]>();
                    for (int region = 0; region < mapFileReader.RegionCount; region++)
                    {
                        DFRegion dfRegion = mapFileReader.GetRegion(region);
                        for (int location = 0; location < dfRegion.LocationCount; location++)
                        {
                            DFLocation dfLoc = mapFileReader.GetLocation(region, location);
                            locBlocks[dfLoc.Name] = dfLoc.Exterior.ExteriorData.BlockNames;
                        }
                    }
                    string locJson = SaveLoadManager.Serialize(locBlocks.GetType(), locBlocks);
                    string fileName = Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, "LocationBlockNames.json");
                    File.WriteAllText(fileName, locJson);
                    return "Location block names json written to " + fileName;
                }
                else if (args.Length == 1 && args[0] == "locindex")
                {
                    string locIndex = "";
                    for (int region = 0; region < mapFileReader.RegionCount; region++)
                    {
                        DFRegion dfRegion = mapFileReader.GetRegion(region);
                        for (int location = 0; location < dfRegion.LocationCount; location++)
                        {
                            DFLocation dfLoc = mapFileReader.GetLocation(region, location);
                            locIndex += string.Format("{0}, {1}: {2}\n", region, dfLoc.LocationIndex, dfLoc.Name);
                        }
                    }
                    string fileName = Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, "LocationIndex.txt");
                    File.WriteAllText(fileName, locIndex);
                    return "Location index written to " + fileName;
                }
                else
                {
                    Dictionary<string, List<string>> regionLocs = new Dictionary<string, List<string>>();
                    for (int region = 0; region < mapFileReader.RegionCount; region++)
                    {
                        DFRegion dfRegion = mapFileReader.GetRegion(region);
                        if (string.IsNullOrEmpty(dfRegion.Name))
                        {
                            Debug.Log("region null: " + region);
                            continue;
                        }
                        List<string> locs;
                        if (regionLocs.ContainsKey(dfRegion.Name))
                            locs = regionLocs[dfRegion.Name];
                        else
                        {
                            locs = new List<string>();
                            regionLocs[dfRegion.Name] = locs;
                        }
                        for (int location = 0; location < dfRegion.LocationCount; location++)
                        {
                            DFLocation dfLoc = mapFileReader.GetLocation(region, location);

                            foreach (string blockName in dfLoc.Exterior.ExteriorData.BlockNames)
                                for (int i = 0; i < args.Length; i++)
                                    if (blockName == args[i])
                                        locs.Add(dfLoc.Name);

                            if (dfLoc.Dungeon.Blocks != null)
                                foreach (DFLocation.DungeonBlock dBlock in dfLoc.Dungeon.Blocks)
                                    for (int i = 0; i < args.Length; i++)
                                        if (dBlock.BlockName == args[i])
                                            locs.Add(dfLoc.Name);
                        }
                    }
                    string locJson = SaveLoadManager.Serialize(regionLocs.GetType(), regionLocs);
                    string blocks = "";
                    for (int i = 0; i < args.Length; i++)
                        blocks = blocks + "-" + args[i];

                    string fileName = Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, blocks.Substring(1) + "-locations.json");
                    File.WriteAllText(fileName, locJson);
                    return "Location block names json written to " + fileName;

                }
            }
        }

        private static class DumpBuilding
        {
            public static readonly string name = "dumpbuilding";
            public static readonly string error = "Failed to dump building";
            public static readonly string usage = "dumpbuilding";
            public static readonly string description = "Dump the current building player is inside to json file";

            public static string Execute(params string[] args)
            {
                DaggerfallInterior interior = GameManager.Instance.PlayerEnterExit.Interior;
                int blockIndex = interior.EntryDoor.blockIndex;
                int recordIndex = interior.EntryDoor.recordIndex;

                DFBlock blockData = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(blockIndex);
                if (blockData.Type == DFBlock.BlockTypes.Rmb)
                {
                    string fileName = WorldDataReplacement.GetBuildingReplacementFilename(blockData.Name, blockIndex, recordIndex);
                    BuildingReplacementData buildingData = new BuildingReplacementData()
                    {
                        RmbSubRecord = blockData.RmbBlock.SubRecords[recordIndex],
                        BuildingType = (int)blockData.RmbBlock.FldHeader.BuildingDataList[recordIndex].BuildingType,
                        Quality = blockData.RmbBlock.FldHeader.BuildingDataList[recordIndex].Quality,
                    };
                    string buildingJson = SaveLoadManager.Serialize(buildingData.GetType(), buildingData);
                    File.WriteAllText(Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName), buildingJson);
                    return "Building data written to " + Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, fileName);
                }
                return error;
            }
        }

        private static class IngredientUsage
        {
            public static readonly string name = "ingredUsage";
            public static readonly string description = "Log an analysis of potion recipe usage of ingredients";
            public static readonly string usage = "ingredUsage";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length > 0)
                    return usage;

                GameManager.Instance.EntityEffectBroker.LogRecipeIngredientUsage();

                return "Finished";
            }
        }

        private static class GodCommand
        {
            public static readonly string name = "tgm";
            public static readonly string error = "Failed to set God Mode - Player health object not found?";
            public static readonly string usage = "tgm";
            public static readonly string description = "Toggle god mode";

            public static string Execute(params string[] args)
            {
                PlayerHealth playerHealth = GameManager.Instance.PlayerHealth;//GameObject.FindObjectOfType<PlayerHealth>();
                if (playerHealth)
                {
                    PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                    playerEntity.GodMode = !playerEntity.GodMode;
                    return string.Format("Godmode enabled: {0}", playerEntity.GodMode);
                }
                else
                    return error;
            }
        }

        private static class NoClipCommand
        {
            public static readonly string name = "tcl";
            public static readonly string error = "Failed to set TCL, PlayerEntity or Levitate could not be found";
            public static readonly string usage = "tcl";
            public static readonly string description = "Toggle noclip by turning off all collisions and activates levitate";

            public static string Execute(params string[] args)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                LevitateMotor levitateMotor = GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>();

                if (playerEntity != null && levitateMotor != null)
                {
                    playerEntity.NoClipMode = !playerEntity.NoClipMode;
                    levitateMotor.IsLevitating = playerEntity.NoClipMode;
                    GameManager.Instance.PlayerController.gameObject.layer = playerEntity.NoClipMode ? LayerMask.NameToLayer("NoclipLayer") : LayerMask.NameToLayer("Player");

                    return string.Format("Noclip enabled: {0}", playerEntity.NoClipMode);
                }
                else
                    return error;
            }
        }

        private static class NoTargetCommand
        {
            public static readonly string name = "nt";
            public static readonly string error = "Failed to set NoTarget mode";
            public static readonly string usage = "nt";
            public static readonly string description = "Toggle NoTarget mode";

            public static string Execute(params string[] args)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                if (playerEntity != null)
                {
                    playerEntity.NoTargetMode = !playerEntity.NoTargetMode;
                    return string.Format("NoTarget enabled: {0}", playerEntity.NoTargetMode);
                }
                else
                    return error;
            }
        }

        private static class ToggleAICommand
        {
            public static readonly string name = "tai";
            public static readonly string error = "Failed to toggle AI";
            public static readonly string usage = "tai";
            public static readonly string description = "Toggles AI on or off";

            public static string Execute(params string[] args)
            {
                GameManager gameManager = GameManager.Instance;
                if (gameManager != null)
                {
                    gameManager.DisableAI = !gameManager.DisableAI;
                    return string.Format("AI disabled: {0}", gameManager.DisableAI);
                }
                else
                    return error;
            }
        }

        private static class CreateMobileCommand
        {
            public static readonly string name = "cm";
            public static readonly string error = "Failed to create mobile";
            public static readonly string usage = "cm [n] [team]";
            public static readonly string description = "Creates a mobile of type [n] on [team]. Omit team argument for default team.";

            public static string Execute(params string[] args)
            {
                if (args.Length < 1) return "See usage.";

                GameObject player = GameManager.Instance.PlayerObject;
                if (player != null)
                {
                    int id = 0;
                    if (!int.TryParse(args[0], out id))
                        return "Invalid mobile ID.";

                    if (!Enum.IsDefined(typeof(MobileTypes), id) && DaggerfallEntity.GetCustomCareerTemplate(id) == null)
                        return "Invalid mobile ID.";

                    int team = 0;
                    if (args.Length > 1)
                    {
                        if (!int.TryParse(args[1], out team))
                            return "Invalid team.";

                        if (!Enum.IsDefined(typeof(MobileTeams), team))
                            return "Invalid team.";
                    }

                    GameObject[] mobile = GameObjectHelper.CreateFoeGameObjects(player.transform.position + player.transform.forward * 2, (MobileTypes)id, 1);

                    DaggerfallEntityBehaviour behaviour = mobile[0].GetComponent<DaggerfallEntityBehaviour>();
                    EnemyEntity entity = behaviour.Entity as EnemyEntity;
                    if (args.Length > 1)
                    {
                        entity.Team = (MobileTeams)team;
                    }
                    else
                        team = (int)entity.Team;

                    mobile[0].transform.LookAt(mobile[0].transform.position + (mobile[0].transform.position - player.transform.position));
                    mobile[0].SetActive(true);

                    return string.Format("Created {0} on team {1}", (MobileTypes)id, (MobileTeams)team);
                }
                else
                    return error;
            }
        }

        private static class ShowDebugStrings
        {
            public static readonly string name = "tdbg";
            public static readonly string error = "Failed to toggle debug string ";
            public static readonly string description = "Toggles if the debug information is displayed";
            public static readonly string usage = "tdbg";

            public static string Execute(params string[] args)
            {
                DaggerfallWorkshop.StreamingWorld streamingWorld = GameManager.Instance.StreamingWorld;//GameObject.FindObjectOfType<DaggerfallWorkshop.StreamingWorld>();
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallUnity.Instance;
                DaggerfallSongPlayer[] songPlayers = GameObject.FindObjectsOfType<DaggerfallSongPlayer>();

                DefaultCommands.showDebugStrings = !DefaultCommands.showDebugStrings;
                bool show = DefaultCommands.showDebugStrings;
                if (streamingWorld)
                    streamingWorld.ShowDebugString = show;
                if (daggerfallUnity)
                    daggerfallUnity.WorldTime.ShowDebugString = show;
                foreach (DaggerfallSongPlayer songPlayer in songPlayers)
                {
                    if (songPlayer && songPlayer.IsPlaying)
                    {
                        songPlayer.ShowDebugString = show;
                        break;
                    }
                }
                if (FPSDisplay.fpsDisplay == null)
                    GameManager.Instance.gameObject.AddComponent<FPSDisplay>();
                FPSDisplay.fpsDisplay.ShowDebugString = show;
                return string.Format("Debug string show: {0}", show);
            }
        }

        private static class SetHealth
        {
            public static readonly string name = "set_health";
            public static readonly string error = "Failed to set health - invalid setting or player object not found";
            public static readonly string description = "Set Health";
            public static readonly string usage = "set_Health [#]";

            public static string Execute(params string[] args)
            {
                DaggerfallEntityBehaviour playerBehavior = GameManager.Instance.PlayerEntityBehaviour;

                int health = 0;
                if (args == null || args.Length < 1 || !int.TryParse(args[0], out health))
                {
                    return HelpCommand.Execute(SetHealth.name);

                }
                else if (playerBehavior != null)
                {
                    playerBehavior.Entity.SetHealth(health);
                    return string.Format("Set health to: {0}", playerBehavior.Entity.CurrentHealth);
                }
                else
                    return error;
            }
        }

        private static class RerollMaxHealth
        {
            public static readonly string name = "reroll_maxhealth";
            public static readonly string error = "";
            public static readonly string description = @"Repair permanently reduced maximum health caused by a level-up bug in 0.10.3 and earlier. " +
                                                        @"This bug is resolved in 0.10.4 and later but some older save games might still need repair. " +
                                                        @"Note that new maximum health is set by forumla and may be slightly higher or lower than before bug.";
            public static readonly string usage = "reroll_maxhealth";

            public static string Execute(params string[] args)
            {
                DaggerfallEntityBehaviour playerBehavior = GameManager.Instance.PlayerEntityBehaviour;

                int rerolledHealth = FormulaHelper.RollMaxHealth(GameManager.Instance.PlayerEntity);
                GameManager.Instance.PlayerEntity.MaxHealth = rerolledHealth;

                return string.Format("Your new maximum health is {0}", rerolledHealth);
            }
        }

        private static class ResetAssets
        {
            public static readonly string name = "reset_assets";
            public static readonly string error = "Could not reset assets";
            public static readonly string description = "Clears mesh and material asset caches then reloads game. Warning: Uses QuickSave slot to reload game in-place. The in-place reload will trigger a longer than usual delay.";
            public static readonly string usage = "reset_assets";

            public static string Execute(params string[] args)
            {
                DaggerfallUnity.Instance.MeshReader.ClearCache();
                DaggerfallUnity.Instance.MaterialReader.ClearCache();

                SaveLoadManager.Instance.QuickSave(true);

                return "Cleared cache and reloaded world.";
            }
        }

        private static class RetryAssetImports
        {
            public static readonly string name = "retry_assets";
            public static readonly string error = "Could not retry asset imports";
            public static readonly string description = "Clears records of import attempts for assets from loose files and mods.";
            public static readonly string usage = "retry_assets";

            public static string Execute(params string[] args)
            {
                MeshReplacement.RetryAssetImports();

                return "Cleared records of import attempts for assets from loose files and mods.";
            }
        }

        private static class Suicide
        {
            public static readonly string name = "suicide";
            public static readonly string error = "Failed to suicide :D";
            public static readonly string description = "Kill self";
            public static readonly string usage = "suicide";

            public static string Execute(params string[] args)
            {
                DaggerfallEntityBehaviour playerBehavior = GameManager.Instance.PlayerEntityBehaviour;

                if (playerBehavior == null)
                    return error;
                else
                {
                    playerBehavior.Entity.SetHealth(0);
                    return "Are you still there?";
                }
            }
        }

        private static class SetWeather
        {
            public static readonly string name = "set_weather";
            public static readonly string description = "Sets the weather to indicated type";
            public static readonly string usage = "set_weather [#] \n0 = Sunny \n1 = Cloudy \n2 = Overcast \n3 = Fog \n4 = Rain \n5 = Thunder \n6 = Snow";

            public static string Execute(params string[] args)
            {
                WeatherManager weatherManager = GameManager.Instance.WeatherManager;
                int weatherCode;

                if (args == null || args.Length < 1)
                    return HelpCommand.Execute(SetWeather.name);

                if (weatherManager == null)
                    return HelpCommand.Execute(SetWeather.name);

                if (int.TryParse(args[0], out weatherCode) && weatherCode >= 0 && weatherCode <= 6)
                {
                    var type = (WeatherType)weatherCode;
                    weatherManager.SetWeather(type);
                    return "Set weather: " + type.ToString();
                }

                return HelpCommand.Execute(SetWeather.name);
            }
        }

        private static class SetWalkSpeed
        {
            public static readonly string name = "set_walkspeed";
            public static readonly string error = "Failed to set walk speed - invalid setting or PlayerMotor object not found";
            public static readonly string description = "Set walk speed by multiplying current walk speed by inserted percentage. Set to -1 to disable or enable speed overrides. Set to -2 to clear out all speed modifiers";
            public static readonly string usage = "set_walkspeed [#]";

            public static string Execute(params string[] args)
            {
                float speed;
                string UID;
                PlayerSpeedChanger speedChanger = GameManager.Instance.SpeedChanger;

                if (speedChanger == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current Walk Speed: ", speedChanger.RefreshWalkSpeed().ToString()));
                        return HelpCommand.Execute(SetWalkSpeed.name);

                    }
                    catch
                    {
                        return HelpCommand.Execute(SetWalkSpeed.name);
                    }

                }
                else if (!float.TryParse(args[0], out speed))
                {
                    if (speedChanger.RemoveSpeedMod(args[0].ToString()))
                        return string.Format("Walk speed modifier " + args[0].ToString() + " removed");
                    else
                        return error + " Walk speed: " + speedChanger.RefreshWalkSpeed().ToString();
                }
                else if (speed != -1 && speed != -2 && speed < 0)
                {
                    return error + " Walk speed: " + speedChanger.RefreshWalkSpeed().ToString();
                }
                else if (speed == -1)
                {
                    speedChanger.walkSpeedOverride = !speedChanger.walkSpeedOverride;
                    return string.Format("Walk speed override set to: " + speedChanger.walkSpeedOverride);
                }
                else if (speed == -2)
                {
                    speedChanger.ResetSpeed(true, false);
                    return string.Format("Walk speed modifiers cleared. Walk speed is " + speedChanger.RefreshWalkSpeed().ToString()); 
                }
                else
                {
                    speedChanger.AddWalkSpeedMod(out UID, speed);
                    return string.Format("Walk speed Modifier added (UID: " + UID + "). Walk speed is " + speedChanger.RefreshWalkSpeed().ToString());
                }

            }
        }

        private static class SetRunSpeed
        {
            public static readonly string name = "set_runspeed";
            public static readonly string error = "Failed to set run speed - invalid setting or PlayerMotor object not found.";
            public static readonly string description = "Set run speed by multiplying current run speed by inserted percentage. Set to -1 to disable or enable speed overrides. Set to -2 to clear out all speed modifiers";
            public static readonly string usage = "set_runspeed [#]";

            public static string Execute(params string[] args)
            {
                float speed;
                string UID;
                PlayerSpeedChanger speedChanger = GameManager.Instance.SpeedChanger;

                if (speedChanger == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current Run Speed: {0}", speedChanger.RefreshRunSpeed().ToString()));
                        return HelpCommand.Execute(SetRunSpeed.name);

                    }
                    catch
                    {
                        return HelpCommand.Execute(SetRunSpeed.name);
                    }


                }
                else if (!float.TryParse(args[0], out speed))
                {
                    if (speedChanger.RemoveSpeedMod(args[0].ToString(), true))
                        return string.Format("Run speed modifier " + args[0].ToString() + " removed");
                    else
                        return error + " Run speed is " + speedChanger.RefreshRunSpeed().ToString();
                }
                else if (speed != -1 && speed != -2 && speed < 0)
                {
                    return error;
                }
                else if (speed == -1)
                {
                    speedChanger.runSpeedOverride = !speedChanger.runSpeedOverride;
                    return string.Format("Run speed override set to: " + speedChanger.runSpeedOverride.ToString());
                }
                else if (speed == -2)
                {
                    speedChanger.ResetSpeed(false, true);
                    return string.Format("Run speed modifiers cleared. Run speed is " + speedChanger.RefreshRunSpeed().ToString());
                }
                else
                {
                    speedChanger.AddRunSpeedMod(out UID, speed);
                    return string.Format("Run speed Modifier added (UID: " + UID + "). Run speed is " + speedChanger.RefreshRunSpeed().ToString());
                }
            }
        }

        private static class SetTimeScale
        {
            public static readonly string name = "set_timescale";
            public static readonly string error = "Failed to set timescale - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Set Timescale; Default 12.  Setting it too high can have adverse affects";
            public static readonly string usage = "set_timescale [#]";

            public static string Execute(params string[] args)
            {
                int speed;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;

                if (daggerfallUnity == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current TimeScale: {0}", DaggerfallWorkshop.DaggerfallUnity.Instance.WorldTime.TimeScale));
                        return HelpCommand.Execute(SetTimeScale.name);
                    }
                    catch
                    {
                        return HelpCommand.Execute(SetTimeScale.name);
                    }

                }
                else if (!int.TryParse(args[0], out speed))
                    return error;
                else if (speed < 0)
                    return "Invalid time scale";
                else
                {
                    try
                    {
                        DaggerfallWorkshop.DaggerfallUnity.Instance.WorldTime.TimeScale = speed;
                        return string.Format("Time Scale set to: {0}", speed);
                    }
                    catch
                    {
                        return "Unspecified error; failed to set timescale";
                    }

                }

            }
        }

        private static class SetMouseSensitivity
        {
            public static readonly string name = "set_mspeed";
            public static readonly string error = "Failed to set mouse sensitivity- invalid setting or PlayerMouseLook object not found";
            public static readonly string description = "Set mouse sensitivity. Default is 1.5";
            public static readonly string usage = "set_mspeed [#]";

            public static string Execute(params string[] args)
            {
                PlayerMouseLook mLook = GameManager.Instance.PlayerMouseLook;//GameObject.FindObjectOfType<PlayerMouseLook>();
                float speed = 0;
                if (args == null || args.Length < 1 || !float.TryParse(args[0], out speed))
                {
                    if (mLook)
                        Console.Log(string.Format("Current mouse sensitivity: {0}", mLook.sensitivity));
                    return HelpCommand.Execute(SetMouseSensitivity.name);
                }
                else if (mLook == null)
                    return error;
                else
                {
                    mLook.sensitivity = new Vector2(speed, speed);
                    return string.Format("Set mouse sensitivity to: {0}", mLook.sensitivity.ToString());
                }
            }

        }

        private static class SetVSync
        {
            public static readonly string name = "set_vsync";
            public static readonly string error = "Failed to toggle vsync";
            public static readonly string description = "Set Vertical Sync count. Must be 0, 1, 2;";
            public static readonly string usage = "set_vsync";

            public static string Execute(params string[] args)
            {
                int count = 0;
                if (args == null || args.Count() < 1)
                {
                    Console.Log(string.Format("Current VSync Count: {0}", UnityEngine.QualitySettings.vSyncCount));
                    return HelpCommand.Execute(SetVSync.name);
                }
                else if (!int.TryParse(args[0], out count))
                {
                    Console.Log(string.Format("Current VSync Count: {0}", UnityEngine.QualitySettings.vSyncCount));
                    return HelpCommand.Execute(SetVSync.name);
                }
                else if (count == 0 || count == 1 || count == 2)
                {
                    UnityEngine.QualitySettings.vSyncCount = count;
                    return string.Format("Set vSyncCount to: {0}", UnityEngine.QualitySettings.vSyncCount.ToString());
                }
                else
                    return error;
            }
        }


        private static class SetGravity
        {
            public static readonly string name = "set_grav";
            public static readonly string error = "Failed to set gravity - invalid setting or PlayerMotor object not found";
            public static readonly string description = string.Format("Set gravity. Use -1 for default (namely, {0})", AcrobatMotor.defaultGravity);
            public static readonly string usage = "set_grav [#]";

            public static string Execute(params string[] args)
            {
                float gravity = 0;
                AcrobatMotor acrobatMotor = GameManager.Instance.AcrobatMotor;

                if (acrobatMotor == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current gravity: {0}", acrobatMotor.gravity));
                        return HelpCommand.Execute(SetGravity.name);
                    }
                    catch
                    {
                        return HelpCommand.Execute(SetGravity.name);
                    }

                }
                else if (!float.TryParse(args[0], out gravity))
                    return error;
                else if (gravity != -1 && gravity < 0)
                    return error;
                else
                {
                    acrobatMotor.gravity = gravity == -1 ? AcrobatMotor.defaultGravity : gravity;
                    return string.Format("Gravity set to: {0}", acrobatMotor.gravity);
                }

            }
        }

        private static class SetJumpSpeed
        {
            public static readonly string name = "set_jump";
            public static readonly string error = "Failed to set jump speed - invalid setting or PlayerMotor object not found";
            public static readonly string description = string.Format("Set jump speed. Use -1 for default (namely, {0})", AcrobatMotor.defaultJumpSpeed);
            public static readonly string usage = "set_jump [#]";

            public static string Execute(params string[] args)
            {
                float speed;
                AcrobatMotor acrobatMotor = GameManager.Instance.AcrobatMotor;

                if (acrobatMotor == null)
                {
                    return error;
                }
                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current Jump Speed: {0}", acrobatMotor.jumpSpeed));
                        return HelpCommand.Execute(SetJumpSpeed.name);
                    }
                    catch
                    {
                        return HelpCommand.Execute(SetJumpSpeed.name);
                    }
                }
                else if (!float.TryParse(args[0], out speed))
                    return error;
                else if (speed != -1 && speed < 0)
                    return error;
                else
                {
                    acrobatMotor.jumpSpeed = speed == -1 ? AcrobatMotor.defaultJumpSpeed : speed;
                    return string.Format("Jump speed set to: {0}", acrobatMotor.jumpSpeed);
                }
            }
        }

        private static class ToggleAirControl
        {
            public static readonly string name = "tac";
            public static readonly string error = "Failed to toggle air control - PlayerMotor object not found?";
            public static readonly string description = "Toggle air control, which allows player to move while in the air.";
            public static readonly string usage = "tac";

            public static string Execute(params string[] args)
            {
                AcrobatMotor acrobatMotor = GameManager.Instance.AcrobatMotor;

                if (acrobatMotor == null)
                    return error;
                else
                {
                    acrobatMotor.airControl = !acrobatMotor.airControl;
                    return string.Format("air control set to: {0}", acrobatMotor.airControl);
                }
            }
        }


        private static class TeleportToMapPixel
        {
            public static readonly string name = "tele2pixel";
            public static readonly string description = "Send the player to the x,y coordinates";
            public static readonly string usage = "tele2pixel [x y]; where x is between 0 & 1000 and y is between 0 & 500";

            public static string Execute(params string[] args)
            {
                int x = 0; int y = 0;
                DaggerfallWorkshop.StreamingWorld streamingWorld = GameManager.Instance.StreamingWorld;//GameObject.FindObjectOfType<DaggerfallWorkshop.StreamingWorld>();
                PlayerEnterExit playerEE = GameManager.Instance.PlayerEnterExit;//GameObject.FindObjectOfType<PlayerEnterExit>();

                if (args == null || args.Length < 2)
                    return HelpCommand.Execute(TeleportToMapPixel.name);

                else if (streamingWorld == null)
                    return "Could not locate Streaming world object";


                else if (playerEE == null || playerEE.IsPlayerInside)
                    return "PlayerEnterExit could not be found or player inside";

                else if (int.TryParse(args[0], out x) && int.TryParse(args[1], out y))
                {
                    if (x <= 0 || y <= 0)
                        return "Invalid Coordinates";
                    else if (x >= MapsFile.MaxMapPixelX || y >= MapsFile.MaxMapPixelY)
                        return "Invalid coordiantes";
                    else
                        streamingWorld.TeleportToCoordinates(x, y);
                    return string.Format("Teleporting player to: {0} {1}", x, y);
                }
                return "Invalid coordiantes";
            }
        }

        private static class GotoLocation
        {
            public static readonly string name = "location";
            public static readonly string description = "Send the player to the predefined location";
            public static readonly string usage = "location [n]; where n is between 0 & 9:\n0 ... random location\n1 ... Daggerfall/Daggerfall\n2 ... Wayrest/Wayrest\n3 ... Sentinel/Sentinel\n4 ... Orsinium Area/Orsinium\n5 ... Tulune/The Old Copperham Place\n6 ... Pothago/The Stronghold of Cirden\n7 ... Daggerfall/Privateer's Hold\n8 ... Wayrest/Merwark Hollow\n9 ... Isle of Balfiera/Direnni Tower\n";
            public static string Execute(params string[] args)
            {
                int n = 0;
                DaggerfallWorkshop.StreamingWorld streamingWorld = GameManager.Instance.StreamingWorld;//GameObject.FindObjectOfType<DaggerfallWorkshop.StreamingWorld>();
                PlayerEnterExit playerEE = GameManager.Instance.PlayerEnterExit;//GameObject.FindObjectOfType<PlayerEnterExit>();

                if (args == null || args.Length < 1)
                    return HelpCommand.Execute(GotoLocation.name);

                else if (streamingWorld == null)
                    return "Could not locate Streaming world object";

                else if (playerEE == null || playerEE.IsPlayerInside)
                    return "PlayerEnterExit could not be found or player inside";


                else if (int.TryParse(args[0], out n))
                {
                    if (n < 0 || n > 9)
                        return "Invalid location index";
                    else
                    {
                        switch (n)
                        {
                            case 0:
                                int xpos, ypos;
                                while (true)
                                {
                                    xpos = UnityEngine.Random.Range(0, MapsFile.MaxMapPixelX);
                                    ypos = UnityEngine.Random.Range(0, MapsFile.MaxMapPixelY);
                                    DaggerfallWorkshop.Utility.ContentReader.MapSummary mapSummary;
                                    if (DaggerfallWorkshop.DaggerfallUnity.Instance.ContentReader.HasLocation(xpos, ypos, out mapSummary))
                                    {
                                        streamingWorld.TeleportToCoordinates(xpos + 1, ypos - 1); // random location - locations always seem to be one pixel to the northern east - so compensate for this (since locations are never at the border - there should not occur a index out of bounds...)
                                        return (string.Format("Teleported player to location at: {0}, {1}", xpos, ypos));
                                    }
                                }
                            case 1:
                                streamingWorld.TeleportToCoordinates(207, 213, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Daggerfall/Daggerfall");
                            case 2:
                                streamingWorld.TeleportToCoordinates(859, 244, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Wayrest/Wayrest");
                            case 3:
                                streamingWorld.TeleportToCoordinates(397, 343, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Sentinel/Sentinel");
                            case 4:
                                streamingWorld.TeleportToCoordinates(892, 146, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Orsinium Area/Orsinium");
                            case 5:
                                streamingWorld.TeleportToCoordinates(67, 119, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Tulune/The Old Copperham Place");
                            case 6:
                                streamingWorld.TeleportToCoordinates(254, 408, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Pothago/The Stronghold of Cirden");
                            case 7:
                                streamingWorld.TeleportToCoordinates(109, 158, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Daggerfall/Privateer's Hold");
                            case 8:
                                streamingWorld.TeleportToCoordinates(860, 245, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Wayrest/Merwark Hollow");
                            case 9:
                                streamingWorld.TeleportToCoordinates(718, 204, StreamingWorld.RepositionMethods.RandomStartMarker);
                                return ("Teleported player to Isle of Balfiera/Direnni Tower");
                            default:
                                break;
                        }
                        return "Teleported successfully.";
                    }

                }
                return "Invalid location index";
            }
        }

        private static class TransitionToExterior
        {
            public static readonly string name = "trans_out";
            public static readonly string error = "Player not inside, or couldn't locate PlayerEnterExit";
            public static readonly string description = "Leave dungeon or building and load exterior area, only works if player inside";
            public static readonly string usage = "trans_out";


            public static string Execute(params string[] args)
            {
                PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;//GameObject.FindObjectOfType<PlayerEnterExit>();
                if (playerEnterExit == null || !playerEnterExit.IsPlayerInside)
                {
                    Console.Log(HelpCommand.Execute(TransitionToExterior.name));
                    return error;
                }
                else
                {
                    try
                    {
                        if (playerEnterExit.IsPlayerInsideDungeon)
                        {
                            playerEnterExit.TransitionDungeonExterior();
                        }
                        else
                        {
                            playerEnterExit.TransitionExterior();
                        }

                        return "Transitioning to exterior";
                    }
                    catch
                    {
                        return "Error on transitioning";
                    }

                }

            }

        }



        private static class TeleportToDungeonDoor
        {
            public static readonly string name = "tele2exit";
            public static readonly string error = "Player not inside dungeon, or couldn't locate player";
            public static readonly string description = "Teleport player to dungeon exit without leaving";
            public static readonly string usage = "tele2exit";


            public static string Execute(params string[] args)
            {
                PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;//GameObject.FindObjectOfType<PlayerEnterExit>();
                GameObject playerObj = GameManager.Instance.PlayerObject;//GameObject.FindGameObjectWithTag("Player") as GameObject;
                if (playerObj == null || playerEnterExit == null || !playerEnterExit.IsPlayerInsideDungeon)
                {
                    return error;
                }
                else
                {
                    try
                    {
                        // Teleport to StartMarker at dungeon entrance
                        playerObj.transform.position = playerEnterExit.Dungeon.StartMarker.transform.position;
                        return "Transitioning to door position";
                    }
                    catch
                    {
                        return "Unspecified Error";
                    }

                }
            }
        }

        private static class TeleportToQuestMarker
        {
            public static readonly string name = "tele2qmarker";
            public static readonly string error = "Could not find quest marker at current location";
            public static readonly string description = "Teleport player to active quest marker (monster, NPC placement, item, etc.)";
            public static readonly string usage = "tele2qmarker";

            public static string Execute(params string[] args)
            {
                QuestMarker spawnMarker;
                Vector3 buildingOrigin;
                bool result = QuestMachine.Instance.GetCurrentLocationQuestMarker(out spawnMarker, out buildingOrigin);
                if (!result)
                    return error;

                if (spawnMarker.targetResources == null || spawnMarker.targetResources.Count == 0)
                    return error;

                if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                {
                    Vector3 dungeonBlockPosition = new Vector3(spawnMarker.dungeonX * RDBLayout.RDBSide, 0, spawnMarker.dungeonZ * RDBLayout.RDBSide);
                    GameManager.Instance.PlayerObject.transform.localPosition = dungeonBlockPosition + spawnMarker.flatPosition;
                }
                else if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
                {
                    // Find active marker type - saves on applying building matrix, etc. to derive position again
                    Vector3 markerPos;
                    if (GameManager.Instance.PlayerEnterExit.Interior.FindClosestMarker(
                        out markerPos,
                        (DaggerfallInterior.InteriorMarkerTypes)spawnMarker.markerType,
                        GameManager.Instance.PlayerObject.transform.position))
                    {
                        GameManager.Instance.PlayerObject.transform.position = markerPos;
                    }
                }
                else
                {
                    return error;
                }
                GameManager.Instance.PlayerMotor.FixStanding();

                return "Finished";
            }
        }

        private static class TeleportToQuestSpawnMarker
        {
            public static readonly string name = "tele2qspawn";
            public static readonly string error = "Could not find quest spawn marker at current location";
            public static readonly string description = "Teleport player to quest spawn marker (monster, NPC placement)";
            public static readonly string usage = "tele2qspawn";

            public static string Execute(params string[] args)
            {
                return TeleportToQuestMarker.Execute() + "...tele2qspawn is deprecated - please use tele2qmarker";
            }
        }

        private static class TeleportToQuestItemMarker
        {
            public static readonly string name = "tele2qitem";
            public static readonly string error = "Could not find quest item marker at current location";
            public static readonly string description = "Teleport player to quest item marker";
            public static readonly string usage = "tele2qitem";

            public static string Execute(params string[] args)
            {
                return TeleportToQuestMarker.Execute() + "...tele2qitem is deprecated - please use tele2qmarker";
            }
        }


        private static class GetAllQuestItems
        {
            public static readonly string name = "getallquestitems";
            public static readonly string error = "Could not find any active quests with Item resources.";
            public static readonly string description = "Immediately give player a copy of any items referenced by active quests (including rewards). This can break quest execution flow.";
            public static readonly string usage = "getallquestitems";

            public static string Execute(params string[] args)
            {
                int itemsFound = 0;
                ulong[] uids = QuestMachine.Instance.GetAllActiveQuests();
                foreach (ulong questUID in uids)
                {
                    Quest quest = QuestMachine.Instance.GetQuest(questUID);
                    if (quest != null)
                    {
                        QuestResource[] itemResources = quest.GetAllResources(typeof(Item));
                        foreach (Item item in itemResources)
                        {
                            GameManager.Instance.PlayerEntity.Items.AddItem(item.DaggerfallUnityItem, ItemCollection.AddPosition.Front);
                            itemsFound++;
                        }
                    }
                }

                if (itemsFound > 0)
                    return string.Format("Transferred {0} items into player inventory", itemsFound);
                else
                    return error;
            }
        }

        private static class EndQuest
        {
            public static readonly string name = "endquest";
            public static readonly string error = "Could not find quest.";
            public static readonly string description = "Tombstone quest. Does not issue reward.";
            public static readonly string usage = "endquest <questUID>";

            public static string Execute(params string[] args)
            {
                if (QuestMachine.Instance.QuestCount == 0)
                    return "No quests are running";

                if (args == null || args.Length != 1)
                    return HelpCommand.Execute(EndQuest.name);

                int questUID;
                if (!int.TryParse(args[0], out questUID))
                    return HelpCommand.Execute(EndQuest.name);

                Quest quest = QuestMachine.Instance.GetQuest((ulong)questUID);
                if (quest == null)
                    return string.Format("Could not find quest {0}", questUID);

                if (quest.QuestTombstoned)
                    return "Quest is already tombstoned";

                QuestMachine.Instance.TombstoneQuest(quest);

                return string.Format("Tombstoned quest {0}", questUID);
            }
        }

        private static class EndDebugQuest
        {
            public static readonly string name = "enddebugquest";
            public static readonly string error = "Could not find debug quest.";
            public static readonly string description = "Tombstone quest currently shown by HUD quest debugger (if any). Does not issue reward.";
            public static readonly string usage = "enddebugquest";

            public static string Execute(params string[] args)
            {
                if (DaggerfallUI.Instance.DaggerfallHUD.QuestDebugger.State == HUDQuestDebugger.DisplayState.Nothing)
                    return "Quest debugger is not open.";

                Quest currentQuest = DaggerfallUI.Instance.DaggerfallHUD.QuestDebugger.CurrentQuest;
                if (currentQuest == null)
                    return "Quest debugger has no quest selected";

                if (currentQuest.QuestTombstoned)
                    return "Quest is already tombstoned";

                // Disallow ending main quest backbone
                if (QuestMachine.IsProtectedQuest(currentQuest))
                {
                    return "Cannot end main quest backbone with 'enddebugquest'. Use 'clearmqstate' instead. Not this will clear ALL quests and ALL global variables.";
                }

                QuestMachine.Instance.TombstoneQuest(currentQuest);

                return string.Format("Tombstoned quest {0}", currentQuest.UID);
            }
        }

        private static class PurgeAllQuests
        {
            public static readonly string name = "purgeallquests";
            public static readonly string error = "Could not find any quests.";
            public static readonly string description = "Immediately tombstones all quests then removes from quest machine. Does not issue rewards.";
            public static readonly string usage = "purgeallquests";

            public static string Execute(params string[] args)
            {
                if (QuestMachine.Instance.QuestCount == 0)
                    return error;

                int count = QuestMachine.Instance.PurgeAllQuests();

                return string.Format("Removed {0} quests.", count);
            }
        }

        private static class PurgeNonStoryQuests
        {
            public static readonly string name = "purgenonstoryquests";
            public static readonly string error = "Could not find any quests.";
            public static readonly string description = "Immediately tombstones all non-story quests then removes from quest machine. Does not issue rewards.";
            public static readonly string usage = "purgenonmqquests";

            public static string Execute(params string[] args)
            {
                if (QuestMachine.Instance.QuestCount == 0)
                    return error;

                int count = QuestMachine.Instance.PurgeAllQuests(true);

                return string.Format("Removed {0} non-story quests.", count);
            }
        }

        private static class ModNPCRep
        {
            public static readonly string name = "modnpcrep";
            public static readonly string error = "You must click an NPC before modifying your reputation with them.";
            public static readonly string description = "Modify reputation with last NPC clicked by a positive or negative amount. Clamped at -100 through 100.";
            public static readonly string usage = "modnpcrep <amount>";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length != 1)
                    return HelpCommand.Execute(ModNPCRep.name);

                int amount;
                if (!int.TryParse(args[0], out amount))
                    return HelpCommand.Execute(ModNPCRep.name);

                // Get faction data of last NPC clicked
                StaticNPC npc = QuestMachine.Instance.LastNPCClicked;
                if (npc == null)
                    return error;

                if (GameManager.Instance.PlayerEntity.FactionData.ChangeReputation(npc.Data.factionID, amount))
                {
                    return string.Format("Changed NPC rep for {0} by {1}", npc.DisplayName, amount);
                }

                return "Could not raise rep - unknown error.";
            }
        }

        private static class SetLevel
        {
            public static readonly string name = "setlevel";
            public static readonly string error = "Could not set player level.";
            public static readonly string description = "Change player level to a value from 1 to 30. Does not allow player to distribute points, only changes level value.";
            public static readonly string usage = "setlevel <level>";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length != 1)
                    return HelpCommand.Execute(SetLevel.name);

                int level;
                if (!int.TryParse(args[0], out level))
                    return HelpCommand.Execute(SetLevel.name);

                GameManager.Instance.PlayerEntity.Level = Mathf.Clamp(level, 1, 30);

                return "Finished";
            }
        }
        private static class Levitate
        {
            public static readonly string name = "levitate";
            public static readonly string error = "Could not start levitating.";
            public static readonly string description = "Start or stop levitating.";
            public static readonly string usage = "levitate on|off";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length != 1)
                    return HelpCommand.Execute(name);

                LevitateMotor levitateMotor = GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>();
                if (!levitateMotor)
                    return "Could not find LevitateMotor component peered with PlayerMotor.";

                string state = args[0];
                if (string.Compare(state, "on", true) == 0)
                {
                    levitateMotor.IsLevitating = true;
                    return "Player is now levitating";
                }
                else if (string.Compare(state, "off", true) == 0)
                {
                    levitateMotor.IsLevitating = false;
                    return "Player is no longer levitating";
                }

                return HelpCommand.Execute(name);
            }
        }

        private static class OpenAllDoors
        {
            public static readonly string name = "openalldoors";
            public static readonly string error = "You are not inside";
            public static readonly string description = "Opens all doors in an interior or dungeon, regardless of locked state";
            public static readonly string usage = "openalldoors";

            public static string Execute(params string[] args)
            {
                if (!GameManager.Instance.IsPlayerInside)
                    return error;
                else
                {
                    int count = 0;
                    IEnumerable<DaggerfallActionDoor> doors = ActiveGameObjectDatabase.GetActiveActionDoors();
                    foreach (DaggerfallActionDoor door in doors)
                    {
                        if (!door.IsOpen)
                        {
                            door.SetOpen(true, false, true);
                            count++;
                        }
                    }
                    return string.Format("Finished.  Opened {0} doors out of {1}", count, doors.Count());

                }

            }
        }

        private static class OpenDoor
        {
            public static readonly string name = "opendoor";
            public static readonly string error = "No door type object found";
            public static readonly string description = "Opens a single door the player is looking at, regardless of locked state";
            public static readonly string usage = "opendoor";

            public static string Execute(params string[] args)
            {
                if (!GameManager.Instance.IsPlayerInside)
                    return "You are not inside";
                else
                {
                    DaggerfallActionDoor door;
                    RaycastHit hitInfo;
                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    if (!(Physics.Raycast(ray, out hitInfo)))
                        return error;
                    else
                    {
                        door = hitInfo.transform.GetComponent<DaggerfallActionDoor>();
                        if (door == null)
                            return error;
                        else
                            door.SetOpen(true, false, true);
                    }
                    return string.Format("Finished");
                }

            }
        }

        private static class ActivateAction
        {
            public static readonly string name = "activate";
            public static readonly string error = "No action object found";
            public static readonly string description = "Triggers an action object regardless of whether it is able to be activated normally";
            public static readonly string usage = "activate";

            public static string Execute(params string[] args)
            {

                DaggerfallAction action;
                RaycastHit hitInfo;
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (!(Physics.Raycast(ray, out hitInfo)))
                    return error;
                else
                {
                    action = hitInfo.transform.GetComponent<DaggerfallAction>();
                    if (action == null)
                        return error;
                    else
                        action.Play(GameManager.Instance.PlayerObject);
                }
                return string.Format("Finished");

            }
        }


        private static class KillAllEnemies
        {
            public static readonly string name = "killall";
            public static readonly string description = "Kills any enemies currently in scene";
            public static readonly string usage = "killall";

            public static string Execute(params string[] args)
            {
                int count = 0;
                foreach(DaggerfallEntityBehaviour entityBehaviour in ActiveGameObjectDatabase.GetActiveEnemyBehaviours())
                {
                    if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                    {
                        entityBehaviour.Entity.SetHealth(0);
                        count++;
                    }
                }
                return string.Format("Finished.  Killed: {0} enemies.", count);
            }
        }


        private static class GetLocationMapPixel
        {
            public static readonly string name = "getlocpixel";
            public static readonly string description = "get pixel coordinates for a location";
            public static readonly string usage = "getlocpixel <region name>/<location name> (no space between region name & location name)";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Count() < 1)
                {
                    return string.Format("Invalid paramaters; \n {0}", usage);
                }

                DaggerfallConnect.DFLocation loc;

                string name = args[0];
                for (int i = 1; i < args.Count(); i++)
                {
                    name += " " + args[i];
                }


                if (DaggerfallWorkshop.Utility.GameObjectHelper.FindMultiNameLocation(name, out loc))
                {
                    DaggerfallConnect.Utility.DFPosition pos = MapsFile.LongitudeLatitudeToMapPixel((int)loc.MapTableData.Longitude, (int)loc.MapTableData.Latitude);
                    return string.Format("{0} found; Pixel Coordinates: \nx: {1} y: {2}", name, pos.X, pos.Y);
                }
                else
                {
                    return "Invalid location.  Check spelling?";
                }
            }

        }

        /// <summary>
        /// Short distance teleport, uses raycast to move player to where they are looking
        /// </summary>
        private static class Teleport
        {
            public static readonly string name = "teleport";
            public static readonly string description = "teleport player to object they are looking at.  God mode recommended";
            public static readonly string usage = "teleport \noptional paramaters: \n{true/false} always teleport if true, even if looking at empty space (default false) \n{max distance}" +
                "max distance to teleport (default 500) \n{up/down/left/right} final position adjustment (default up) \n Examples:\nteleport \n teleport up \n teleport 999 left true";

            public static string Execute(params string[] args)
            {

                bool forceTeleOnNoHit = false;              //teleport maxDistance even if raycast doesn't hit
                float maxDistance = 500;                    //max distance
                int step = 0;
                Vector3 dir = Camera.main.transform.up;
                Vector3 loc;

                if (args != null)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        float temp = 0;
                        if (string.IsNullOrEmpty(args[i]))
                            continue;
                        else if (args[i] == "true" || args[i] == "false")
                            Boolean.TryParse(args[i], out forceTeleOnNoHit);
                        else if (float.TryParse(args[i], out temp))
                            maxDistance = temp;
                        else if (args[i].ToLower() == "up")
                            dir = Camera.main.transform.up;
                        else if (args[i].ToLower() == "down")
                            dir = Camera.main.transform.up * -1;
                        else if (args[i].ToLower() == "right")
                            dir = Camera.main.transform.right;
                        else if (args[i].ToLower() == "left")
                            dir = Camera.main.transform.right * -1;
                        else if (args[i].ToLower() == "forward")
                            dir = Camera.main.transform.forward;
                        else if (args[i].ToLower() == "back")
                            dir = Camera.main.transform.forward * -1;
                    }
                }

                RaycastHit hitInfo;
                Vector3 origin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                Ray ray = new Ray(origin + Camera.main.transform.forward * .2f, Camera.main.transform.forward);
                GameManager.Instance.AcrobatMotor.ClearFallingDamage();
                if (!(Physics.Raycast(ray, out hitInfo, maxDistance)))
                {
                    Console.Log("Didn't hit anything...");
                    if (forceTeleOnNoHit)
                    {
                        GameManager.Instance.PlayerObject.transform.position = ray.GetPoint(maxDistance);
                        Console.Log("...teleporting anyways");
                    }
                }
                else
                {
                    loc = hitInfo.point;
                    while (Physics.CheckCapsule(loc, loc + dir, GameManager.Instance.PlayerController.radius + .1f) && step < 50)
                    {
                        loc = dir + loc;
                        step++;
                    }
                    GameManager.Instance.PlayerObject.transform.position = loc;
                }
                return "Finished";
            }
        }

        private static class AddInventoryItem
        {
            public static readonly string name = "add";
            public static readonly string description = "Adds n inventory items to the character, based on the given keyword. n = 1 by default";
            public static readonly string usage = "add (book|weapon|armor|cloth|ingr|relig|soul|gold|magic|drug|map|torch|potion|recipe|painting) [n]";

            public static string Execute(params string[] args)
            {
                if (args.Length < 1)
                    return usage;

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ItemCollection items = playerEntity.Items;
                DaggerfallUnityItem newItem = null;

                int n = 1;
                if (args.Length >= 2)
                {
                    Int32.TryParse(args[1], out n);
                }

                if (n < 1)
                    return usage;

                if (args[0] == "gold")
                {
                    playerEntity.GoldPieces += n;
                    return string.Format("Added {0} gold pieces", n);
                }

                UnityEngine.Random.InitState(Time.frameCount);
                while (n >= 1)
                {
                    n--;
                    switch (args[0])
                    {
                        case "book":
                            newItem = ItemBuilder.CreateRandomBook();
                            break;
                        case "weapon":
                            newItem = ItemBuilder.CreateRandomWeapon(playerEntity.Level);
                            break;
                        case "armor":
                            newItem = ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                            break;
                        case "cloth":
                            newItem = ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race);
                            break;
                        case "ingr":
                            newItem = ItemBuilder.CreateRandomIngredient();
                            break;
                        case "relig":
                            newItem = ItemBuilder.CreateRandomReligiousItem();
                            break;
                        case "soul":
                            newItem = ItemBuilder.CreateRandomlyFilledSoulTrap();
                            break;
                        case "magic":
                            newItem = ItemBuilder.CreateRandomMagicItem(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                            break;
                        case "drug":
                            newItem = ItemBuilder.CreateRandomDrug();
                            break;
                        case "map":
                            newItem = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Map);
                            break;
                        case "torch":
                            newItem = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Torch);
                            break;
                        case "soultrap":
                            newItem = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);
                            break;
                        case "potion":
                            newItem = ItemBuilder.CreateRandomPotion();
                            break;
                        case "recipe":
                            newItem = ItemBuilder.CreateRandomRecipe();
                            break;
                        case "painting":
                            newItem = ItemBuilder.CreateItem(ItemGroups.Paintings, (int)Paintings.Painting);
                            break;
                        default:
                            return "unrecognized keyword. see usage";
                    }
                    items.AddItem(newItem);
                }
                return "success";

            }
            private static T RandomEnumValue<T>()
            {
                var v = Enum.GetValues(typeof(T));
                return (T)v.GetValue(UnityEngine.Random.Range(0, v.Length));
            }
        }

        private static class AddArtifact
        {
            public static readonly string name = "addArtifact";
            public static readonly string description = "Adds an artifact of ID n to the character";
            public static readonly string usage = "addArtifact [n]";

            public static string Execute(params string[] args)
            {
                if (args.Length < 1) return "See usage.";

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ItemCollection items = playerEntity.Items;
                DaggerfallUnityItem newItem = null;

                switch (args[0])
                {
                    case "0":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Masque_of_Clavicus);
                        break;
                    case "1":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Mehrunes_Razor);
                        break;
                    case "2":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Mace_of_Molag_Bal);
                        break;
                    case "3":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Hircine_Ring);
                        break;
                    case "4":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Sanguine_Rose);
                        break;
                    case "5":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Oghma_Infinium);
                        break;
                    case "6":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Wabbajack);
                        break;
                    case "7":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Ring_of_Namira);
                        break;
                    case "8":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Skull_of_Corruption);
                        break;
                    case "9":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Azuras_Star);
                        break;
                    case "10":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Volendrung);
                        break;
                    case "11":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Warlocks_Ring);
                        break;
                    case "12":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Auriels_Bow);
                        break;
                    case "13":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Necromancers_Amulet);
                        break;
                    case "14":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Chrysamere);
                        break;
                    case "15":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Lords_Mail);
                        break;
                    case "16":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Staff_of_Magnus);
                        break;
                    case "17":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Ring_of_Khajiit);
                        break;
                    case "18":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Ebony_Mail);
                        break;
                    case "19":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Auriels_Shield);
                        break;
                    case "20":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Spell_Breaker);
                        break;
                    case "21":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Skeletons_Key);
                        break;
                    case "22":
                        newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Ebony_Blade);
                        break;
                    default:
                        return "Invalid artifact ID.";
                }
                items.AddItem(newItem);

                return "Success";
            }
        }

        private class AddItemHelper
        {
            readonly Queue<string> args;

            private AddItemHelper(string[] args)
            {
                this.args = new Queue<string>(args);
            }

            public T Parse<T>()
            {
                return (T)Enum.Parse(typeof(T), args.Dequeue());
            }

            public static string Execute(string[] args, Func<AddItemHelper, DaggerfallUnityItem> getItem)
            {
                try
                {
                    DaggerfallUnityItem item = getItem(new AddItemHelper(args));
                    GameManager.Instance.PlayerEntity.Items.AddItem(item);
                    return string.Format("Added {0} to inventory.", item.ItemName);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        private static class AddWeapon
        {
            public static readonly string name = "add_weapon";
            public static readonly string description = "Adds a weapon to inventory.";
            public static readonly string usage = "add_weapon {Weapons} {WeaponMaterialTypes}";

            public static string Execute(params string[] args)
            {
                return args.Length == 2 ?
                    AddItemHelper.Execute(args, x => ItemBuilder.CreateWeapon(x.Parse<Weapons>(), x.Parse<WeaponMaterialTypes>())) :
                    usage;
            }
        }

        private static class AddArmor
        {
            public static readonly string name = "add_armor";
            public static readonly string description = "Adds an armor to inventory.";
            public static readonly string usage = "add_armor {Genders} {Races} {Armor} {ArmorMaterialTypes}";

            public static string Execute(params string[] args)
            {
                return args.Length == 4 ?
                    AddItemHelper.Execute(args, x => ItemBuilder.CreateArmor(x.Parse<Genders>(), x.Parse<Races>(), x.Parse<Armor>(), x.Parse<ArmorMaterialTypes>())) :
                    usage;
            }
        }

        private static class AddClothing
        {
            public static readonly string name = "add_clothing";
            public static readonly string description = "Adds clothing to inventory.";
            public static readonly string usage = "add_clothing {Genders} {MensClothing|WomensClothing} {Races} {DyeColors}";

            public static string Execute(params string[] args)
            {
                if (args.Length != 4)
                    return usage;

                return AddItemHelper.Execute(args, x => x.Parse<Genders>() == Genders.Male ?
                    ItemBuilder.CreateMensClothing(x.Parse<MensClothing>(), x.Parse<Races>(), -1, x.Parse<DyeColors>()) :
                    ItemBuilder.CreateWomensClothing(x.Parse<WomensClothing>(), x.Parse<Races>(), -1, x.Parse<DyeColors>())
                );
            }
        }

        private static class AddAllEquip
        {
            public static readonly string name = "add_all_equip";
            public static readonly string description = "Adds all equippable item types to inventory for the current characters gender & race. (for testing paperdoll images)";
            public static readonly string usage = "add_all_equip (clothing|clothingAllDyes|armor|weapons|magicWeapons)";

            public static ArmorMaterialTypes[] armorMaterials = {
                ArmorMaterialTypes.Leather, ArmorMaterialTypes.Chain, ArmorMaterialTypes.Iron, ArmorMaterialTypes.Steel,
                ArmorMaterialTypes.Silver, ArmorMaterialTypes.Elven, ArmorMaterialTypes.Dwarven, ArmorMaterialTypes.Mithril,
                ArmorMaterialTypes.Adamantium, ArmorMaterialTypes.Ebony, ArmorMaterialTypes.Orcish, ArmorMaterialTypes.Daedric
            };
            public static WeaponMaterialTypes[] weaponMaterials = {
                WeaponMaterialTypes.Iron, WeaponMaterialTypes.Steel, WeaponMaterialTypes.Silver, WeaponMaterialTypes.Elven,
                WeaponMaterialTypes.Dwarven, WeaponMaterialTypes.Mithril, WeaponMaterialTypes.Adamantium, WeaponMaterialTypes.Ebony,
                WeaponMaterialTypes.Orcish, WeaponMaterialTypes.Daedric
            };

            public static List<MensClothing> mensUsableClothing = new List<MensClothing>() {
                MensClothing.Casual_cloak, MensClothing.Formal_cloak, MensClothing.Reversible_tunic, MensClothing.Plain_robes,
                MensClothing.Short_shirt, MensClothing.Short_shirt_with_belt, MensClothing.Long_shirt, MensClothing.Long_shirt_with_belt,
                MensClothing.Short_shirt_closed_top, MensClothing.Short_shirt_closed_top2, MensClothing.Long_shirt_closed_top, MensClothing.Long_shirt_closed_top2
            };
            public static List<WomensClothing> womensUsableClothing = new List<WomensClothing>() {
                WomensClothing.Casual_cloak, WomensClothing.Formal_cloak, WomensClothing.Strapless_dress, WomensClothing.Plain_robes,
                WomensClothing.Short_shirt, WomensClothing.Short_shirt_belt, WomensClothing.Long_shirt, WomensClothing.Long_shirt_belt,
                WomensClothing.Short_shirt_closed, WomensClothing.Short_shirt_closed_belt, WomensClothing.Long_shirt_closed, WomensClothing.Long_shirt_closed_belt
            };

            public static string Execute(params string[] args)
            {
                if (args.Length != 1)
                    return usage;

                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
                ItemCollection items = playerEntity.Items;
                DaggerfallUnityItem newItem = null;

                switch (args[0])
                {
                    case "clothing":
                    case "clothingAllDyes":
                        DyeColors[] clothingDyes = (args[0] == "clothing") ? new DyeColors[] { DyeColors.White } : ItemBuilder.clothingDyes;
                        ItemGroups clothing = (playerEntity.Gender == Genders.Male) ? ItemGroups.MensClothing : ItemGroups.WomensClothing;
                        foreach (DyeColors dye in clothingDyes)
                        {
                            Array enumArray = itemHelper.GetEnumArray(clothing);
                            for (int i = 0; i < enumArray.Length; i++)
                            {
                                ItemTemplate itemTemplate = itemHelper.GetItemTemplate(clothing, i);
                                if ((playerEntity.Gender == Genders.Male && mensUsableClothing.Contains((MensClothing)enumArray.GetValue(i))) ||
                                    womensUsableClothing.Contains((WomensClothing)enumArray.GetValue(i)) || itemTemplate.variants == 0)
                                    itemTemplate.variants = 1;

                                for (int v = 0; v < itemTemplate.variants; v++)
                                {
                                    newItem = new DaggerfallUnityItem(clothing, i);
                                    ItemBuilder.SetRace(newItem, playerEntity.Race);
                                    newItem.dyeColor = dye;
                                    newItem.CurrentVariant = v;
                                    playerEntity.Items.AddItem(newItem);
                                }
                            }
                        }
                        return string.Format("Added all clothing types for a {0} {1}", playerEntity.Gender, playerEntity.Race);

                    case "armor":
                        foreach (ArmorMaterialTypes material in armorMaterials)
                        {
                            Array enumArray = itemHelper.GetEnumArray(ItemGroups.Armor);
                            for (int i = 0; i < enumArray.Length; i++)
                            {
                                Armor armorType = (Armor)enumArray.GetValue(i);
                                int vs = 0;
                                int vf = 0;
                                if (armorType == Armor.Cuirass || armorType == Armor.Left_Pauldron || armorType == Armor.Right_Pauldron)
                                {
                                    if (material == ArmorMaterialTypes.Chain)
                                    {
                                        vs = 4;
                                    }
                                    else if (material >= ArmorMaterialTypes.Iron)
                                    {
                                        vs = 1;
                                        vf = 4;
                                    }
                                }
                                else if (armorType == Armor.Greaves)
                                {
                                    if (material == ArmorMaterialTypes.Leather)
                                    {
                                        vs = 0;
                                        vf = 2;
                                    }
                                    else if (material == ArmorMaterialTypes.Chain)
                                    {
                                        vs = 6;
                                    }
                                    else if (material >= ArmorMaterialTypes.Iron)
                                    {
                                        vs = 2;
                                        vf = 6;
                                    }
                                }
                                else if (armorType == Armor.Gauntlets || armorType == Armor.Boots)
                                {
                                    if (material == ArmorMaterialTypes.Leather)
                                    {
                                        vf = 1;
                                    }
                                    else
                                    {
                                        vs = 1;
                                        vf = itemHelper.GetItemTemplate(ItemGroups.Armor, i).variants;
                                    }
                                }
                                else
                                {
                                    vf = itemHelper.GetItemTemplate(ItemGroups.Armor, i).variants;
                                }
                                if (vf == 0)
                                    vf = vs + 1;

                                for (int v = vs; v < vf; v++)
                                {
                                    newItem = ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, armorType, material, v);
                                    playerEntity.Items.AddItem(newItem);
                                }
                            }
                            int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(ItemGroups.Armor);
                            for (int i = 0; i < customItemTemplates.Length; i++)
                            {
                                newItem = ItemBuilder.CreateItem(ItemGroups.Armor, customItemTemplates[i]);
                                ItemBuilder.ApplyArmorSettings(newItem, playerEntity.Gender, playerEntity.Race, material);
                                playerEntity.Items.AddItem(newItem);
                            }
                        }
                        return string.Format("Added all armor types for a {0} {1}", playerEntity.Gender, playerEntity.Race);

                    case "weapons":
                    case "magicWeapons":
                        foreach (WeaponMaterialTypes material in weaponMaterials)
                        {
                            Array enumArray = itemHelper.GetEnumArray(ItemGroups.Weapons);
                            for (int i = 0; i < enumArray.Length - 1; i++)
                            {
                                newItem = ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(i), material);
                                if (args[0] == "magicWeapons")
                                {
                                    newItem.legacyMagic = new DaggerfallEnchantment[] { new DaggerfallEnchantment() { type = EnchantmentTypes.CastWhenUsed, param = 5 } };
                                    newItem.IdentifyItem();
                                }
                                playerEntity.Items.AddItem(newItem);
                            }
                            int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(ItemGroups.Weapons);
                            for (int i = 0; i < customItemTemplates.Length; i++)
                            {
                                newItem = ItemBuilder.CreateItem(ItemGroups.Weapons, customItemTemplates[i]);
                                ItemBuilder.ApplyWeaponMaterial(newItem, material);
                                playerEntity.Items.AddItem(newItem);
                            }
                        }
                        return string.Format("Added all weapon types for any gender/race.");

                    default:
                        return "No valid equippable item group specified.";
                }

            }
        }

        private static class AddBook
        {
            public static readonly string name = "addbook";
            public static readonly string description = "Gives player the book with the given id or name.";
            public static readonly string usage = "addbook {id|name}";

            public static string Execute(params string[] args)
            {
                if (args.Length != 1)
                    return usage;

                int id;
                DaggerfallUnityItem book = int.TryParse(args[0], out id) ? ItemBuilder.CreateBook(id) : ItemBuilder.CreateBook(args[0]);
                if (book == null)
                    return "Failed to create book";

                GameManager.Instance.PlayerEntity.Items.AddItem(book);
                return string.Format("Added {0} to inventory.", book.ItemName);
            }
        }

        private static class Groundme
        {
            public static readonly string name = "groundme";
            public static readonly string description = "Move back to last known good position";
            public static readonly string usage = "groundme";

            public static string Execute(params string[] args)
            {
                AcrobatMotor acrobatMotor = GameManager.Instance.AcrobatMotor;
                FrictionMotor frictionMotor = GameManager.Instance.FrictionMotor;
                CharacterController cc = GameManager.Instance.PlayerController;
                acrobatMotor.ClearFallingDamage();

                RaycastHit hitInfo;
                Vector3 origin = frictionMotor.ContactPoint;
                origin.y += cc.height;
                Ray ray = new Ray(origin, Vector3.down);
                if (!(Physics.Raycast(ray, out hitInfo, cc.height * 2)))
                {
                    return "Failed to reposition - try Teleport or if inside tele2exit";
                }
                else
                {
                    GameManager.Instance.PlayerObject.transform.position = frictionMotor.ContactPoint;
                    GameManager.Instance.PlayerMotor.FixStanding(cc.height / 2);
                    return "Finished - moved to last known good location at " + frictionMotor.ContactPoint.ToString();
                }
            }
        }

        private static class ShowBankWindow
        {
            public static readonly string name = "showbankwindow";
            public static readonly string description = "Opens a banking window for specified region";
            public static readonly string usage = "showbankwindow {region index}";
            public static DaggerfallWorkshop.Game.UserInterface.DaggerfallBankingWindow bankWindow;

            public static string Execute(params string[] args)
            {
                if (bankWindow == null)
                    bankWindow = new DaggerfallWorkshop.Game.UserInterface.DaggerfallBankingWindow(DaggerfallUI.UIManager);
                DaggerfallUI.UIManager.PushWindow(bankWindow);
                return "Finished";

            }

        }

        private static class ShowSpellmakerWindow
        {
            public static readonly string name = "showspellmaker";
            public static readonly string description = "Opens a spellmaker window for creating spells";
            public static readonly string usage = "showspellmaker";

            public static string Execute(params string[] args)
            {
                DaggerfallUI.UIManager.PostMessage(DaggerfallUIMessages.dfuiOpenSpellMakerWindow);
                return "Finished";
            }
        }

        private static class ShowItemMakerWindow
        {
            public static readonly string name = "showitemmaker";
            public static readonly string description = "Opens a item maker window for enchanting items";
            public static readonly string usage = "showitemmaker";

            public static string Execute(params string[] args)
            {
                DaggerfallUI.UIManager.PostMessage(DaggerfallUIMessages.dfuiOpenItemMakerWindow);
                return "Finished";
            }
        }

        private static class ShowPotionMakerWindow
        {
            public static readonly string name = "showpotionmaker";
            public static readonly string description = "Opens a potion maker window to brew potions";
            public static readonly string usage = "showpotionmaker";

            public static string Execute(params string[] args)
            {
                DaggerfallUI.UIManager.PostMessage(DaggerfallUIMessages.dfuiOpenPotionMakerWindow);
                return "Finished";
            }
        }

        private static class AddSpellBook
        {
            public static readonly string name = "addspellbook";
            public static readonly string description = "Gives player a new spellbook if they do not have one.";
            public static readonly string usage = "addspellbook";

            public static string Execute(params string[] args)
            {
                if (GameManager.Instance.ItemHelper.AddSpellbookItem(GameManager.Instance.PlayerEntity))
                    return "Spellbook added";
                else
                    return "Player already has a spellbook";
            }
        }

        private static class StartQuest
        {
            public static readonly string name = "startquest";
            public static readonly string description = "Starts the specified quest";
            public static readonly string usage = "startquest {quest ID}";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return usage;
                UnityEngine.Random.InitState(Time.frameCount);
                Quest quest = GameManager.Instance.QuestListsManager.GetQuest(args[0]);

                if (quest != null)
                {
                    QuestMachine.Instance.StartQuest(quest);
                    QuestMachine.Instance.RestoreLocalizedQuestMessages(quest.QuestName);
                    return "Started quest '" + quest.DisplayName + "'";
                }
                else
                {
                    return "Quest ID '" + args[0] + "' could not be found";
                }
            }
        }

        private static class DiseasePlayer
        {
            public static readonly string name = "diseaseplayer";
            public static readonly string description = "Infect player with a classic disease.";
            public static readonly string usage = "diseaseplayer index (a number 0-16) OR diseaseplayer vampire|werewolf|wereboar";

            enum CommandDiseaseTypes
            {
                Numerical,
                Vampire,
                Werewolf,
                Wereboar,
            }

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length != 1)
                    return usage;

                // Determine if valid string or expecting index
                CommandDiseaseTypes diseaseType;
                switch (args[0].ToLower())
                {
                    case "vampire":
                        diseaseType = CommandDiseaseTypes.Vampire;
                        break;
                    case "werewolf":
                        diseaseType = CommandDiseaseTypes.Werewolf;
                        break;
                    case "wereboar":
                        diseaseType = CommandDiseaseTypes.Wereboar;
                        break;
                    default:
                        diseaseType = CommandDiseaseTypes.Numerical;
                        break;
                }

                // Disease player by index
                if (diseaseType == CommandDiseaseTypes.Numerical)
                {
                    // Get index and validate range
                    int index = -1;
                    if (!int.TryParse(args[0], out index))
                        return string.Format("Could not parse argument `{0}` to a number", args[0]);
                    if (index < 0 || index > 16)
                        return string.Format("Index {0} is out range. Must be 0-16.", index);

                    // Infect player
                    Diseases disease = (Diseases)index;
                    EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateDisease(disease);
                    GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);
                    return string.Format("Player infected with {0}", disease.ToString());
                }

                // Vampirism/Werewolf/Wereboar
                if (diseaseType == CommandDiseaseTypes.Vampire)
                {
                    // Infect player with vampirism stage one
                    EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateVampirismDisease();
                    GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                    return "Player infected with vampirism.";
                }
                else if (diseaseType == CommandDiseaseTypes.Werewolf)
                {
                    // Infect player with werewolf lycanthropy stage one
                    EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Werewolf);
                    GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                    return "Player infected with werewolf lycanthropy.";
                }
                else if (diseaseType == CommandDiseaseTypes.Wereboar)
                {
                    // Infect player with wereboar lycanthropy stage one
                    EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Wereboar);
                    GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                    return "Player infected with wereboar lycanthropy.";
                }
                else
                {
                    return usage;
                }
            }
        }

        private static class PoisonPlayer
        {
            public static readonly string name = "poisonplayer";
            public static readonly string description = "Infect player with a classic poison.";
            public static readonly string usage = "poisonplayer index (a number 0-11)";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length != 1)
                    return usage;

                // Get index and validate range
                int index;
                if (!int.TryParse(args[0], out index))
                    return string.Format("Could not parse argument `{0}` to a number", args[0]);
                if (index < 0 || index > 11)
                    return string.Format("Index {0} is out range. Must be 0-11.", index);

                // Poison player
                Poisons poisonType = (Poisons)index + 128;
                FormulaHelper.InflictPoison(GameManager.Instance.PlayerEntity, GameManager.Instance.PlayerEntity, poisonType, true);

                return string.Format("Player poisoned with {0}", poisonType.ToString());
            }
        }

        private static class ExecuteScript
        {
            public static readonly string name = "execute";
            public static readonly string description = "compiles source files (and instanties objects when possible) from streaming assets path.";
            public static readonly string error = "invalid paramater.";

            public static readonly string usage = "execute Script00.cs Script01.cs Script02.cs....";


            public static string Execute(params string[] args)
            {
                if (args == null)
                    return error;
                else if (args.Length < 1)
                    return error;

                int count = 0;
                string[] files = new string[args.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    if (string.IsNullOrEmpty(args[i]))
                        continue;

                    string fullName = Path.Combine(Application.streamingAssetsPath, args[i]);

                    if (!fullName.EndsWith(".cs"))   //limiting to only .cs files isn't really necessary - any text file should work fine
                        return error;

                    if (!File.Exists(fullName))
                        return error;
                    else
                    {
                        Console.Log("Found File: " + fullName);
                        files[i] = fullName;
                        count++;
                    }
                }

                if (count < 1)
                    return error;
                //string[] source = new string[files.Length];
                //for (int i = 0; i < files.Length; i++)
                //{
                //    source[i] = File.ReadAllText(files[i]);
                //}

                try
                {
                    System.Reflection.Assembly assembly = DaggerfallWorkshop.Game.Utility.Compiler.CompileSource(files, false);//(files.ToArray(), false);
                    var loadableTypes = DaggerfallWorkshop.Game.Utility.Compiler.GetLoadableTypes(assembly);

                    foreach (Type t in loadableTypes)
                    {
                        bool isAssignable = typeof(Component).IsAssignableFrom(t);
                        bool hasDefaultConstructor = (t.GetConstructor(Type.EmptyTypes) != null && !t.IsAbstract);

                        if (isAssignable)
                        {
                            GameObject newObj = new GameObject(t.Name);
                            newObj.AddComponent(t);
                        }
                        else if (hasDefaultConstructor)
                        {
                            Activator.CreateInstance(t); //only works if has a default constructor
                        }
                    }

                    return "Finished";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        private static class PlayFLC
        {
            public static readonly string name = "playflc";
            public static readonly string description = "Play the specified .FLC or .CEL file";
            public static readonly string usage = "playflc {filename.flc|.cel} (e.g. playflc azura.flc | playflc warrior.cel)";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return usage;

                DemoFLCWindow window = new DemoFLCWindow(DaggerfallUI.Instance.UserInterfaceManager);
                window.Filename = args[0];
                DaggerfallUI.Instance.UserInterfaceManager.PushWindow(window);

                return "Finished";
            }
        }

        private static class PlayVID
        {
            public static readonly string name = "playvid";
            public static readonly string description = "Play the specified .VID file";
            public static readonly string usage = "playvid {filename.vid} (e.g. playvid anim0000.vid)";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return usage;

                // Start video and set to exit on escape only, as enter keypress can fall through and close video instantly
                DaggerfallVidPlayerWindow vidPlayerWindow = new DaggerfallVidPlayerWindow(DaggerfallUI.UIManager, args[0]);
                vidPlayerWindow.EndOnAnyKey = false;
                DaggerfallUI.Instance.UserInterfaceManager.PushWindow(vidPlayerWindow);

                return "Finished";
            }
        }

        private static class PrintCrimeCommitted
        {
            public static readonly string name = "print_crimecommitted";
            public static readonly string description = "Output active crime state";
            public static readonly string usage = "print_crimecommitted";

            public static string Execute(params string[] args)
            {
                return GameManager.Instance.PlayerEntity.CrimeCommitted.ToString();
            }
        }

        private static class ClearCrimeCommitted
        {
            public static readonly string name = "clear_crimecommitted";
            public static readonly string description = "Clear active crime state";
            public static readonly string usage = "clear_crimecommitted";

            public static string Execute(params string[] args)
            {
                GameManager.Instance.PlayerEntity.CrimeCommitted = PlayerEntity.Crimes.None;
                return "Cleared active crime state";
            }
        }

        private static class PrintLegalRep
        {
            public static readonly string name = "print_legalrep";
            public static readonly string description = "Output current legal status and reputation value for all regions";
            public static readonly string usage = "print_legalrep";

            public static string Execute(params string[] args)
            {
                string output = string.Empty;
                if (GameManager.Instance.PlayerEntity != null && GameManager.Instance.PlayerEntity.RegionData != null)
                {
                    for (int region = 0; region < GameManager.Instance.PlayerEntity.RegionData.Length; region++)
                    {
                        string regionName = TextManager.Instance.GetLocalizedRegionName(region);
                        string reputationString = string.Empty;
                        int rep = GameManager.Instance.PlayerEntity.RegionData[region].LegalRep;
                        if (rep > 80)
                            reputationString = TextManager.Instance.GetLocalizedText("revered");
                        else if (rep > 60)
                            reputationString = TextManager.Instance.GetLocalizedText("esteemed");
                        else if (rep > 40)
                            reputationString = TextManager.Instance.GetLocalizedText("honored");
                        else if (rep > 20)
                            reputationString = TextManager.Instance.GetLocalizedText("admired");
                        else if (rep > 10)
                            reputationString = TextManager.Instance.GetLocalizedText("respected");
                        else if (rep > 0)
                            reputationString = TextManager.Instance.GetLocalizedText("dependable");
                        else if (rep == 0)
                            reputationString = TextManager.Instance.GetLocalizedText("aCommonCitizen");
                        else if (rep < -80)
                            reputationString = TextManager.Instance.GetLocalizedText("hated");
                        else if (rep < -60)
                            reputationString = TextManager.Instance.GetLocalizedText("pondScum");
                        else if (rep < -40)
                            reputationString = TextManager.Instance.GetLocalizedText("aVillain");
                        else if (rep < -20)
                            reputationString = TextManager.Instance.GetLocalizedText("aCriminal");
                        else if (rep < -10)
                            reputationString = TextManager.Instance.GetLocalizedText("aScoundrel");
                        else if (rep < 0)
                            reputationString = TextManager.Instance.GetLocalizedText("undependable");

                        output += string.Format("In '{0}' you are {1} [{2}]\n", regionName, reputationString, rep);
                    }
                    output += "Finished";
                }
                else
                {
                    return "Could not read legal reputation data.";
                }

                return output;
            }
        }

        private static class ClearNegativeLegalRep
        {
            public static readonly string name = "clear_negativelegalrep";
            public static readonly string description = "Negative legal reputation and banishment state for all regions is set back to 0. Does not affect positive legal reputation. Does not change factional reputation.";
            public static readonly string usage = "clear_negativelegalrep";

            public static string Execute(params string[] args)
            {
                string output = string.Empty;
                if (GameManager.Instance.PlayerEntity != null && GameManager.Instance.PlayerEntity.RegionData != null)
                {
                    for (int region = 0; region < GameManager.Instance.PlayerEntity.RegionData.Length; region++)
                    {
                        string regionName = TextManager.Instance.GetLocalizedRegionName(region);
                        int rep = GameManager.Instance.PlayerEntity.RegionData[region].LegalRep;
                        if (rep < 0)
                        {
                            GameManager.Instance.PlayerEntity.RegionData[region].LegalRep = 0;
                            GameManager.Instance.PlayerEntity.RegionData[region].SeverePunishmentFlags = 0;
                            output += string.Format("Cleared negative legal reputation for '{0}'.\n", regionName);
                        }
                    }
                    output += "Finished";
                }
                else
                {
                    return "Could not read legal reputation data.";
                }

                return output;
            }
        }

        private static class RefreshBuildingNames
        {
            public static readonly string name = "refresh_buildingnames";
            public static readonly string description = "Refresh discovered building names in current location. Used for testing localization and debugging stale discovery data.";
            public static readonly string usage = "refresh_buildingnames";

            public static string Execute(params string[] args)
            {
                GameManager.Instance.PlayerGPS.RefreshBuildingNamesInCurrentLocation();
                return "Finished";
            }
        }

        private static class PrintQuests
        {
            public static readonly string name = "print_quests";
            public static readonly string description = "Output current quests running on quest machine.";
            public static readonly string usage = "print_quests";

            public static string Execute(params string[] args)
            {
                string output = string.Empty;

                ulong[] allQuests = QuestMachine.Instance.GetAllQuests();
                if (allQuests != null && allQuests.Length > 0)
                {
                    for (int i = 0; i < allQuests.Length; i++)
                    {
                        Quest quest = QuestMachine.Instance.GetQuest(allQuests[i]);

                        string status = string.Empty;
                        if (!quest.QuestComplete)
                        {
                            status = HUDQuestDebugger.questRunning;
                        }
                        else
                        {
                            if (quest.QuestSuccess)
                                status = HUDQuestDebugger.questFinishedSuccess;
                            else
                                status = HUDQuestDebugger.questFinishedEnded;
                        }

                        output += string.Format("{0} '{1}' [UID={2}] - {3}\n", quest.QuestName, quest.DisplayName, quest.UID, status);
                    }
                }
                else
                {
                    output = HUDQuestDebugger.noQuestsRunning;
                }

                return output;
            }
        }

        private static class SummonDaedra
        {
            public static readonly string name = "summondaedra";
            public static readonly string description = "Summons a daedra prince to test quest delivery.";
            public static readonly string usage = "summondaedra [filename] (note: matches the .VID filename for daedea in /arena2 folder.)";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return usage;

                bool found = false;
                string validNames = string.Empty;
                DaggerfallQuestPopupWindow.DaedraData daedraToSummon = new DaggerfallQuestPopupWindow.DaedraData();
                DaggerfallQuestPopupWindow.DaedraData[] daedraData = DaggerfallQuestPopupWindow.daedraData;
                foreach (var daedra in daedraData)
                {
                    string name = daedra.vidFile.Split('.')[0];
                    if (string.Compare(args[0], name, true) == 0)
                    {
                        daedraToSummon = daedra;
                        found = true;
                    }
                    validNames += name + ", ";
                }

                if (!found)
                    return string.Format("Could not find daedra named {0}. Valid names are {1}", args[0], validNames);

                IUserInterfaceManager uiManager = DaggerfallUI.UIManager;
                Quest quest = GameManager.Instance.QuestListsManager.GetQuest(daedraToSummon.quest);
                if (quest != null)
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.DaedraSummoned, new object[] { uiManager, daedraToSummon, quest }));
                else
                    return string.Format("Could not find quest {0} for deadra {1}", daedraToSummon.quest, args[0]);

                return "Finished";
            }
        }

        private static class AddPopupText
        {
            public static readonly string name = "addpopuptext";
            public static readonly string description = "Fill HUD buffer with messages";
            public static readonly string usage = "addpopuptext count";

            public static string Execute(params string[] args)
            {
                if (args.Length < 1) return "see usage";

                int n = 1;
                Int32.TryParse(args[0], out n);

                if (n < 1 || n > 100000)
                    return "Error - see usage";

                for (int i = 1; i <= n; i++)
                    DaggerfallUI.Instance.PopupMessage("message " + i);
                return "Finished";
            }
        }

        private static class ChangeModSettings
        {
            static ConsoleController controller;

            public static readonly string name = "change_modsettings";
            public static readonly string description = "Change mod settings (experimental).";
            public static readonly string usage = "change_modsettings";

            public static string Execute(params string[] args)
            {
                if (!ModManager.Instance)
                    return "ModManager instance not found.";

                string[] modTitles = ModManager.Instance.Mods.Where(x => x.HasSettings && x.LoadSettingsCallback != null).Select(x => x.Title).ToArray();
                if (modTitles.Length == 0)
                    return "There are no mods that support live changes to settings.";

                ModManager.Instance.StartCoroutine(OpenSettingsWindow(modTitles));
                return string.Format("Found {0} mod(s) that support live changes to settings. Close the console to open settings window.", modTitles.Length);
            }

            private static IEnumerator OpenSettingsWindow(string[] modTitles)
            {
                if (!FindController())
                    yield break;

                while (controller.ui.isConsoleOpen)
                    yield return null;

                var userInterfaceManager = DaggerfallUI.Instance.UserInterfaceManager;

                var listPicker = new DaggerfallListPickerWindow(userInterfaceManager);
                listPicker.ListBox.AddItems(modTitles);
                listPicker.OnItemPicked += (index, modTitle) =>
                {
                    listPicker.PopWindow();
                    userInterfaceManager.PushWindow(new ModSettingsWindow(userInterfaceManager, ModManager.Instance.GetMod(modTitle), true));
                };
                userInterfaceManager.PushWindow(listPicker);
            }

            private static bool FindController()
            {
                if (controller)
                    return true;

                GameObject console = GameObject.Find("Console");
                if (console && (controller = console.GetComponent<ConsoleController>()))
                    return true;

                Debug.LogError("Failed to find console controller.");
                return false;
            }
        }
    }
}
