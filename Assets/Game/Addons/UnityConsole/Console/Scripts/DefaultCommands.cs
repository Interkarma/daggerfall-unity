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
using DaggerfallWorkshop.Game.Magic;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility.AssetInjection;

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
            ConsoleCommandsDatabase.RegisterCommand(Suicide.name, Suicide.description, Suicide.usage, Suicide.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ShowDebugStrings.name, ShowDebugStrings.description, ShowDebugStrings.usage, ShowDebugStrings.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetWeather.name, SetWeather.description, SetWeather.usage, SetWeather.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToMapPixel.name, TeleportToMapPixel.description, TeleportToMapPixel.usage, TeleportToMapPixel.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToDungeonDoor.name, TeleportToDungeonDoor.description, TeleportToDungeonDoor.usage, TeleportToDungeonDoor.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToQuestSpawnMarker.name, TeleportToQuestSpawnMarker.description, TeleportToQuestSpawnMarker.usage, TeleportToQuestSpawnMarker.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TeleportToQuestItemMarker.name, TeleportToQuestItemMarker.description, TeleportToQuestItemMarker.usage, TeleportToQuestItemMarker.Execute);
            ConsoleCommandsDatabase.RegisterCommand(GetAllQuestItems.name, GetAllQuestItems.description, GetAllQuestItems.usage, GetAllQuestItems.Execute);
            ConsoleCommandsDatabase.RegisterCommand(EndDebugQuest.name, EndDebugQuest.description, EndDebugQuest.usage, EndDebugQuest.Execute);
            ConsoleCommandsDatabase.RegisterCommand(EndQuest.name, EndQuest.description, EndQuest.usage, EndQuest.Execute);
            ConsoleCommandsDatabase.RegisterCommand(PurgeAllQuests.name, PurgeAllQuests.description, PurgeAllQuests.usage, PurgeAllQuests.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ModNPCRep.name, ModNPCRep.description, ModNPCRep.usage, ModNPCRep.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ClearMQState.name, ClearMQState.description, ClearMQState.usage, ClearMQState.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetMQStage.name, SetMQStage.description, SetMQStage.usage, SetMQStage.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetLevel.name, SetLevel.description, SetLevel.usage, SetLevel.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Levitate.name, Levitate.description, Levitate.usage, Levitate.Execute);
            ConsoleCommandsDatabase.RegisterCommand(OpenAllDoors.name, OpenAllDoors.description, OpenAllDoors.usage, OpenAllDoors.Execute);
            ConsoleCommandsDatabase.RegisterCommand(OpenDoor.name, OpenDoor.description, OpenDoor.usage, OpenDoor.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ActivateAction.name, ActivateAction.description, ActivateAction.usage, ActivateAction.Execute);
            ConsoleCommandsDatabase.RegisterCommand(KillAllEnemies.name, KillAllEnemies.description, KillAllEnemies.usage, KillAllEnemies.Execute);
            ConsoleCommandsDatabase.RegisterCommand(TransitionToExterior.name, TransitionToExterior.description, TransitionToExterior.usage, TransitionToExterior.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetHealth.name, SetHealth.description, SetHealth.usage, SetHealth.Execute);

            ConsoleCommandsDatabase.RegisterCommand(SetWalkSpeed.name, SetWalkSpeed.description, SetWalkSpeed.usage, SetWalkSpeed.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetMouseSensitivity.name, SetMouseSensitivity.description, SetMouseSensitivity.usage, SetMouseSensitivity.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ToggleMouseSmoothing.name, ToggleMouseSmoothing.description, ToggleMouseSmoothing.usage, ToggleMouseSmoothing.Execute);

            //ConsoleCommandsDatabase.RegisterCommand(SetMouseSmoothing.name, SetMouseSmoothing.description, SetMouseSmoothing.usage, SetMouseSmoothing.Execute);
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
            ConsoleCommandsDatabase.RegisterCommand(ShowBankWindow.name, ShowBankWindow.description, ShowBankWindow.usage, ShowBankWindow.Execute);
            ConsoleCommandsDatabase.RegisterCommand(StartQuest.name, StartQuest.usage, StartQuest.description, StartQuest.Execute);

            ConsoleCommandsDatabase.RegisterCommand(CastEffect.name, CastEffect.usage, CastEffect.description, CastEffect.Execute);

            ConsoleCommandsDatabase.RegisterCommand(DumpBlock.name, DumpBlock.description, DumpBlock.usage, DumpBlock.Execute);
            ConsoleCommandsDatabase.RegisterCommand(DumpLocBlocks.name, DumpLocBlocks.description, DumpLocBlocks.usage, DumpLocBlocks.Execute);
            ConsoleCommandsDatabase.RegisterCommand(DumpBuilding.name, DumpBuilding.description, DumpBuilding.usage, DumpBuilding.Execute);
        }

        private static class DumpBlock
        {
            public static readonly string name = "dumpblock";
            public static readonly string error = "Failed to dump block";
            public static readonly string usage = "dumpblock blockName";
            public static readonly string description = "Dump a block to json file";

            public static string Execute(params string[] args)
            {
                if (args.Length == 0)
                {
                    return HelpCommand.Execute(DumpBlock.name);
                }
                else
                {
                    DFBlock blockData;
                    if (RMBLayout.GetBlockData(args[0], out blockData))
                    {
                        string blockJson = SaveLoadManager.Serialize(blockData.GetType(), blockData);
                        File.WriteAllText(Path.Combine(Application.persistentDataPath, args[0]), blockJson);
                        return "Block data json written to " + Path.Combine(Application.persistentDataPath, args[0]);
                    }
                    return error;
                }
            }
        }

        private static class DumpLocBlocks
        {
            public static readonly string name = "dumplocblocks";
            public static readonly string error = "Failed to dump locations";
            public static readonly string usage = "dumplocblocks [blockName]";
            public static readonly string description = "Dump the names of blocks for each location, or locations for a block, to json file";

            public static string Execute(params string[] args)
            {
                MapsFile mapFileReader = DaggerfallUnity.Instance.ContentReader.MapFileReader;
                if (args.Length > 1)
                {
                    return HelpCommand.Execute(DumpBlock.name);
                }
                else if (args.Length == 0)
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
                    string fileName = Path.Combine(Application.persistentDataPath, "LocationBlockNames.json");
                    File.WriteAllText(fileName, locJson);
                    return "Location block names json written to " + fileName;
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
                                if (blockName == args[0])
                                    locs.Add(dfLoc.Name);
                        }
                    }
                    string locJson = SaveLoadManager.Serialize(regionLocs.GetType(), regionLocs);
                    string fileName = Path.Combine(Application.persistentDataPath, args[0] + "-locations.json");
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
                    BuildingReplacementData buildingData = new BuildingReplacementData() {
                        RmbSubRecord = blockData.RmbBlock.SubRecords[recordIndex]
                    };
                    string buildingJson = SaveLoadManager.Serialize(buildingData.GetType(), buildingData);
                    File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), buildingJson);
                    return "Building data written to " + Path.Combine(Application.persistentDataPath, fileName);
                }
                return error;
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
            public static readonly string description = "Set walk speed. Set to -1 to return to default speed.";
            public static readonly string usage = "set_walkspeed [#]";

            public static string Execute(params string[] args)
            {
                int speed;
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;//GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current Walk Speed: {0}", playerMotor.GetWalkSpeed(GameManager.Instance.PlayerEntity)));
                        return HelpCommand.Execute(SetWalkSpeed.name);

                    }
                    catch
                    {
                        return HelpCommand.Execute(SetWalkSpeed.name);
                    }

                }
                else if (!int.TryParse(args[0], out speed))
                    return error;
                else if (speed == -1)
                {
                    playerMotor.useWalkSpeedOverride = false;
                    return string.Format("Walk speed set to default.");
                }
                else
                {
                    playerMotor.useWalkSpeedOverride = true;
                    playerMotor.walkSpeedOverride = speed;
                    return string.Format("Walk speed set to: {0}", speed);
                }

            }
        }

        private static class SetRunSpeed
        {
            public static readonly string name = "set_runspeed";
            public static readonly string error = "Failed to set run speed - invalid setting or PlayerMotor object not found";
            public static readonly string description = "Set run speed. Set to -1 to return to default speed.";
            public static readonly string usage = "set_runspeed [#]";

            public static string Execute(params string[] args)
            {
                int speed;
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;//GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current RunSpeed: {0}", playerMotor.GetRunSpeed(playerMotor.GetWalkSpeed(GameManager.Instance.PlayerEntity))));
                        return HelpCommand.Execute(SetRunSpeed.name);

                    }
                    catch
                    {
                        return HelpCommand.Execute(SetRunSpeed.name);
                    }


                }
                else if (!int.TryParse(args[0], out speed))
                {
                    return error;
                }
                else if (speed == -1)
                {
                    playerMotor.useRunSpeedOverride = false;
                    return string.Format("Run speed set to default.");
                }
                else
                {
                    playerMotor.runSpeedOverride = speed;
                    playerMotor.useRunSpeedOverride = true;
                    return string.Format("Run speed set to: {0}", speed);
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


        private static class ToggleMouseSmoothing
        {
            public static readonly string name = "tmsmooth";
            public static readonly string error = "Failed to toggle mouse smoothing - PlayerMouseLook object not found?";
            public static readonly string description = "Toggle mouse smoothing.";
            public static readonly string usage = "tmsmooth";

            public static string Execute(params string[] args)
            {
                PlayerMouseLook mLook = GameManager.Instance.PlayerMouseLook;//GameObject.FindObjectOfType<PlayerMouseLook>();
                if (mLook == null)
                    return error;
                else
                {
                    //mLook.smoothing = new Vector2(speed, speed);
                    mLook.enableSmoothing = !mLook.enableSmoothing;
                    return string.Format("Mouse smoothing is on: {0}", mLook.enableSmoothing.ToString());
                }
            }

        }


        private static class SetMouseSmoothing
        {
            public static readonly string name = "set_msmooth";
            public static readonly string error = "Failed to set mouse smoothing - invalid setting or PlayerMouseLook object not found";
            public static readonly string description = "Set mouse smoothing. Default is 3";
            public static readonly string usage = "set_msmooth [#]";

            public static string Execute(params string[] args)
            {
                PlayerMouseLook mLook = GameManager.Instance.PlayerMouseLook;//GameObject.FindObjectOfType<PlayerMouseLook>();
                float speed = 0;
                if (args == null || args.Length < 1 || !float.TryParse(args[0], out speed))
                {
                    if (mLook)
                        Console.Log(string.Format("Current mouse smoothing: {0}", mLook.smoothing));
                    return HelpCommand.Execute(SetMouseSmoothing.name);
                }
                else if (mLook == null)
                    return error;
                else
                {
                    mLook.smoothing = new Vector2(speed, speed);
                    return string.Format("Set mouse smoothing to: {0}", mLook.smoothing.ToString());
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
            public static readonly string description = "Set gravity. Default is 20";
            public static readonly string usage = "set_grav [#]";

            public static string Execute(params string[] args)
            {
                int gravity = 0;
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;//GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current gravity: {0}", playerMotor.gravity));
                        return HelpCommand.Execute(SetGravity.name);
                    }
                    catch
                    {
                        return HelpCommand.Execute(SetGravity.name);
                    }

                }
                else if (!int.TryParse(args[0], out gravity))
                    return error;
                else
                {
                    playerMotor.gravity = gravity;
                    return string.Format("Gravity set to: {0}", playerMotor.gravity);
                }

            }
        }

        private static class SetJumpSpeed
        {
            public static readonly string name = "set_jump";
            public static readonly string error = "Failed to set jump speed - invalid setting or PlayerMotor object not found";
            public static readonly string description = "Set jump speed. Default is 8";
            public static readonly string usage = "set_jump [#]";

            public static string Execute(params string[] args)
            {
                int speed;
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;//GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                {
                    return error;
                }
                if (args == null || args.Length < 1)
                {
                    try
                    {
                        Console.Log(string.Format("Current Jump Speed: {0}", playerMotor.jumpSpeed));
                        return HelpCommand.Execute(SetJumpSpeed.name);
                    }
                    catch
                    {
                        return HelpCommand.Execute(SetJumpSpeed.name);
                    }
                }
                else if (!int.TryParse(args[0], out speed))
                    return error;
                else
                {
                    playerMotor.jumpSpeed = speed;
                    return string.Format("Jump speed set to: {0}", playerMotor.jumpSpeed);
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

                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;//GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                    return error;
                else
                {
                    playerMotor.airControl = !playerMotor.airControl;
                    return string.Format("air control set to: {0}", playerMotor.airControl);
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
                                    xpos = UnityEngine.Random.Range(0, MapsFile.MaxMapPixelX - 1);
                                    ypos = UnityEngine.Random.Range(0, MapsFile.MaxMapPixelY - 1);
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


        private static class TeleportToQuestSpawnMarker
        {
            public static readonly string name = "tele2qspawn";
            public static readonly string error = "Could not find quest spawn marker at current location";
            public static readonly string description = "Teleport player to quest spawn marker (monster, NPC placement)";
            public static readonly string usage = "tele2qspawn";

            public static string Execute(params string[] args)
            {
                QuestMarker spawnMarker;
                Vector3 buildingOrigin;
                bool result = QuestMachine.Instance.GetCurrentLocationQuestMarker(MarkerTypes.QuestSpawn, out spawnMarker, out buildingOrigin);
                if (!result)
                    return error;

                Vector3 dungeonBlockPosition = new Vector3(spawnMarker.dungeonX * RDBLayout.RDBSide, 0, spawnMarker.dungeonZ * RDBLayout.RDBSide);
                GameManager.Instance.PlayerEnterExit.transform.localPosition = dungeonBlockPosition + spawnMarker.flatPosition + buildingOrigin;
                GameManager.Instance.PlayerMotor.FixStanding();

                return "Finished";
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
                QuestMarker itemMarker;
                Vector3 buildingOrigin;
                bool result = QuestMachine.Instance.GetCurrentLocationQuestMarker(MarkerTypes.QuestItem, out itemMarker, out buildingOrigin);
                if (!result)
                    return error;

                Vector3 dungeonBlockPosition = new Vector3(itemMarker.dungeonX * RDBLayout.RDBSide, 0, itemMarker.dungeonZ * RDBLayout.RDBSide);
                GameManager.Instance.PlayerEnterExit.transform.localPosition = dungeonBlockPosition + itemMarker.flatPosition + buildingOrigin;
                GameManager.Instance.PlayerMotor.FixStanding();

                return "Finished";
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

        private static class ClearMQState
        {
            public static readonly string name = "clearmqstate";
            public static readonly string error = "Could not clear main quest state.";
            public static readonly string description = "Clears all main quest state. CAUTION: Will purge all active quests, clear all reputations to 0, and reset all global variables.";
            public static readonly string usage = "clearmqstate";

            public static string Execute(params string[] args)
            {
                QuestMachine.Instance.ClearMainQuestState();

                return "Finished";
            }
        }

        private static class SetMQStage
        {
            public static readonly string name = "setmqstage";
            public static readonly string error = "Could not set main quest stage.";
            public static readonly string description = "Configure quest system for a particular stage of main quest for testing. CAUTION: Will also call 'resetmqstate' and purge all other quest state.";
            public static readonly string usage = "setmqstage <stage>";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length != 1)
                    return HelpCommand.Execute(SetMQStage.name);

                int stage;
                if (!int.TryParse(args[0], out stage))
                    return HelpCommand.Execute(SetMQStage.name);

                int stageSet = QuestMachine.Instance.SetMainQuestStage(stage);

                return string.Format("Set main quest stage to {0}", stageSet);
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

                FakeLevitate fakeLevitate = GameManager.Instance.PlayerMotor.GetComponent<FakeLevitate>();
                if (!fakeLevitate)
                    return "Could not find FakeLevitate component peered with PlayerMotor.";

                string state = args[0];
                if (string.Compare(state, "on", true) == 0)
                {
                    fakeLevitate.IsLevitating = true;
                    return "Player is now levitating";
                }
                else if (string.Compare(state, "off", true) == 0)
                {
                    fakeLevitate.IsLevitating = false;
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
                    DaggerfallActionDoor[] doors = GameObject.FindObjectsOfType<DaggerfallActionDoor>();
                    int count = 0;
                    for (int i = 0; i < doors.Length; i++)
                    {
                        if (!doors[i].IsOpen)
                        {
                            doors[i].SetOpen(true, false, true);
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
                DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
                int count = 0;
                for (int i = 0; i < entityBehaviours.Length; i++)
                {
                    DaggerfallEntityBehaviour entityBehaviour = entityBehaviours[i];
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
                GameManager.Instance.PlayerMotor.ClearFallingDamage();
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
            public static readonly string usage = "add (book|weapon|armor|cloth|ingr|relig) [n]";

            public static string Execute(params string[] args)
            {
                if (args.Length < 1) return "see usage";

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ItemCollection items = playerEntity.Items;
                DaggerfallUnityItem newItem = null;

                int n = 1;
                if (args.Length >= 2)
                {
                    Int32.TryParse(args[1], out n);
                }

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
                            newItem = ItemBuilder.CreateRandomArmor(playerEntity.Level, RandomEnumValue<Genders>(), RandomEnumValue<Races>());
                            break;
                        case "cloth":
                            newItem = ItemBuilder.CreateRandomClothing(RandomEnumValue<Genders>(), RandomEnumValue<Races>());
                            break;
                        case "ingr":
                            newItem = ItemBuilder.CreateRandomIngredient();
                            break;
                        case "relig":
                            newItem = ItemBuilder.CreateRandomReligiousItem();
                            break;
                        default:
                            return "unrecognized keyword. see usage";
                    }
                    items.AddItem(newItem);
                }
                return "success";

            }
            private static T RandomEnumValue<T> ()
            {
                var v = Enum.GetValues(typeof(T));
                return (T) v.GetValue(UnityEngine.Random.Range(0,v.Length-1));
            }
        }


        private static class Groundme
        {
            public static readonly string name = "groundme";
            public static readonly string description = "Move back to last known good position";
            public static readonly string usage = "groundme";

            public static string Execute(params string[] args)
            {
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
                CharacterController cc = GameManager.Instance.PlayerController;
                playerMotor.ClearFallingDamage();

                RaycastHit hitInfo;
                Vector3 origin = playerMotor.ContactPoint;
                origin.y += cc.height;
                Ray ray = new Ray(origin, Vector3.down);
                if (!(Physics.Raycast(ray, out hitInfo, cc.height * 2)))
                {
                    return "Failed to reposition - try Teleport or if inside tele2exit";
                }
                else
                {
                    GameManager.Instance.PlayerObject.transform.position = playerMotor.ContactPoint;
                    playerMotor.FixStanding(cc.height / 2);
                    return "Finished - moved to last known good location at " + playerMotor.ContactPoint.ToString();
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
                if(bankWindow == null)
                    bankWindow = new DaggerfallWorkshop.Game.UserInterface.DaggerfallBankingWindow(DaggerfallUI.UIManager);
                DaggerfallUI.UIManager.PushWindow(bankWindow);
                return "Finished";

            }

        }

        private static class StartQuest
        {
            public static readonly string name = "startquest";
            public static readonly string description = "Starts the specified quest";
            public static readonly string usage = "startquest {quest name}";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return usage;
                DaggerfallWorkshop.Game.Questing.QuestMachine.Instance.InstantiateQuest(args[0]);
                return "Finished";
            }
        }

        private static class CastEffect
        {
            public static readonly string name = "casteffect";
            public static readonly string description = "Cast single effect on self or target. Target is any entity under crosshair.";
            public static readonly string usage = "casteffect self|target groupIndex subgroupIndex";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length != 3)
                    return usage;

                // Get self or target gameobject
                GameObject result = null;
                if (args[0] == "self")
                    result = GameManager.Instance.PlayerObject;
                else if (args[0] == "target")
                    return "'casteffect target' not implemented yet.";  // TODO: Cast ray to find target object
                else
                    return "must be 'casteffect self' or 'casteffect target'";

                // Get group and subgroup indices
                int groupIndex = -1, subgroupIndex = -1;
                if (!int.TryParse(args[1], out groupIndex) || !int.TryParse(args[2], out subgroupIndex))
                    return usage;

                // Must have a gameobject by now
                if (!result)
                    return "casteffect could not find any GameObject";

                // The gameobject must be an entity
                DaggerfallEntityBehaviour entityBehaviour = result.GetComponent<DaggerfallEntityBehaviour>();
                if (!entityBehaviour)
                    return "casteffect could not find DaggerfallEntityBehaviour on specified GameObject (you must target an enemy or other valid entity)";

                // The gameobject must have a magic manager
                MagicManager magicManager = result.GetComponent<MagicManager>();
                if (!magicManager)
                    return "casteffect could not find MagicManager on specified GameObject";

                // TODO: Send effect to resulting magic manager or error out if effect does not exist
                
                return string.Format("Cast effect {0} {1} on entity type {2} with name {3}", groupIndex, subgroupIndex, entityBehaviour.EntityType.ToString(), result.name);
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

    }
}
