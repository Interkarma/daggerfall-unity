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
                    playerHealth.GodMode = !playerHealth.GodMode;
                    return string.Format("Godmode enabled: {0}", playerHealth.GodMode);
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
                DaggerfallSongPlayer songPlayer = GameObject.FindObjectOfType<DaggerfallSongPlayer>();

                DefaultCommands.showDebugStrings = !DefaultCommands.showDebugStrings;
                bool show = DefaultCommands.showDebugStrings;
                if (streamingWorld)
                    streamingWorld.ShowDebugString = show;
                if (daggerfallUnity)
                    daggerfallUnity.WorldTime.ShowDebugString = show;
                if (songPlayer)
                    songPlayer.ShowDebugString = show;
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
            public static readonly string usage = "set_weather [#] \n0 = none \n1 = rain \n2 = storm \n3 = snow";

            public static string Execute(params string[] args)
            {
                WeatherManager weatherManager = GameManager.Instance.WeatherManager;
                int weatherType = 0;

                if (args == null || args.Length < 1)
                    return HelpCommand.Execute(SetWeather.name);

                else if (weatherManager == null)
                {
                    return HelpCommand.Execute(SetWeather.name);

                }
                else if (int.TryParse(args[0], out weatherType))
                {
                    if (weatherType >= 0 && weatherType < 4)
                    {
                        weatherManager.ClearAllWeather();

                        if (weatherType == 1)
                            weatherManager.StartRaining();
                        else if (weatherType == 2)
                            weatherManager.StartStorming();
                        else if (weatherType == 3)
                            weatherManager.StartSnowing();
                        return "Set weather.";
                    }

                }

                return HelpCommand.Execute(SetWeather.name);

            }
        }

        private static class SetWalkSpeed
        {
            public static readonly string name = "set_walkspeed";
            public static readonly string error = "Failed to set walk speed - invalid setting or PlayerMotor object not found";
            public static readonly string description = "Set walk speed. Default is 3";
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
                        Console.Log(string.Format("Current Walk Speed: {0}", playerMotor.walkSpeed));
                        return HelpCommand.Execute(SetWalkSpeed.name);

                    }
                    catch
                    {
                        return HelpCommand.Execute(SetWalkSpeed.name);
                    }

                }
                else if (!int.TryParse(args[0], out speed))
                    return error;
                else
                {
                    playerMotor.walkSpeed = speed;
                    return string.Format("Walk speed set to: {0}", speed);
                }

            }
        }

        private static class SetRunSpeed
        {
            public static readonly string name = "set_runspeed";
            public static readonly string error = "Failed to set run speed - invalid setting or PlayerMotor object not found";
            public static readonly string description = "Set run speed. Default is 7";
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
                        Console.Log(string.Format("Current RunSpeed: {0}", playerMotor.runSpeed));
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
                else
                {
                    playerMotor.runSpeed = speed;
                    return string.Format("Run speed set to: {0}", speed);
                }
            }
        }

        private static class SetTimeScale
        {
            public static readonly string name = "set_timescale";
            public static readonly string error = "Failed to set timescale - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Set Timescale; Default 10.  Setting it too high can have adverse affects";
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
                        entityBehaviour.transform.SendMessage("Die");
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

        private static class ExecuteScript
        {
            public static readonly string name = "execute";
            public static readonly string description = "compiles source files (and instanties objects when possible) from streaming assets path";
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

                System.Reflection.Assembly assembly = Compiler.CompileFiles(files.ToArray());

                var assaignable = Compiler.GetLoadableTypes(assembly);
                foreach (Type t in assaignable)
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
                        object newThing = Activator.CreateInstance(t); //only works if object has a default constructor
                    }
                }

                return "Finished";
            }
        }

    }
}
