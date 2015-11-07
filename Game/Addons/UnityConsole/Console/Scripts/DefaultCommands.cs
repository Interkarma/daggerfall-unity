﻿using UnityEngine;
using Wenzil.Console.Commands;
using DaggerfallWorkshop.Game;
using System.Linq;
using System;
using DaggerfallConnect.Arena2;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Entity;

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
        }


        private static class GodCommand
        {
            public static readonly string name = "tgm";
            public static readonly string error = "Failed to set God Mode - Player health object not found?";
            public static readonly string usage = "tgm";
            public static readonly string description = "Toggle god mode";

            public static string Execute(params string[] args)
            {
                PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
                if (playerHealth)
                {
                    playerHealth.GodMode = !playerHealth.GodMode;
                    return string.Format("Godmode enabled: {0}", playerHealth.GodMode);
                }
                else
                {
                    return error;
                }


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
                DaggerfallWorkshop.StreamingWorld streamingWorld = GameObject.FindObjectOfType<DaggerfallWorkshop.StreamingWorld>();
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = GameObject.FindObjectOfType<DaggerfallWorkshop.DaggerfallUnity>();
                DaggerfallSongPlayer songPlayer = GameObject.FindObjectOfType<DaggerfallSongPlayer>();

                DefaultCommands.showDebugStrings = !DefaultCommands.showDebugStrings;
                bool show = DefaultCommands.showDebugStrings;
                if (streamingWorld)
                    streamingWorld.ShowDebugString = show;
                if (daggerfallUnity)
                    daggerfallUnity.WorldTime.ShowDebugString = show;
                if (songPlayer)
                    songPlayer.ShowDebugString = show;
                return string.Format("Debug string show: {0}", show);





            }
        }




        /// <summary>
        /// This is not added yet.
        /// </summary>
        private static class SetHealth
        {
            public static readonly string name = "set_health";
            public static readonly string error = "Failed to set health - invalid setting or player object not found";
            public static readonly string description = "Set Health";
            public static readonly string usage = "set_Health [#]";

            public static string Execute(params string[] args)
            {

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                DaggerfallEntityBehaviour playerBehavior;
                int health = 0;
                if (args == null || args.Length < 1 || !int.TryParse(args[0], out health))
                {
                    return HelpCommand.Execute(SetHealth.name);

                }
                else if (player != null)
                {
                    playerBehavior = player.GetComponent<DaggerfallEntityBehaviour>();
                    playerBehavior.Entity.SetHealth(health);

                    return string.Format("Set health to: {0}", playerBehavior.Entity.CurrentHealth);
                }
                else
                {
                    return error;

                }

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
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                DaggerfallEntityBehaviour playerBehavior;

                if (player == null)
                {
                    return error;
                }
                else
                {
                    playerBehavior = player.GetComponent<DaggerfallEntityBehaviour>();
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
                {
                    return HelpCommand.Execute(SetWeather.name);

                }
                else if (weatherManager == null)
                {
                    return HelpCommand.Execute(SetWeather.name);

                }
                else if(int.TryParse(args[0], out weatherType))
                {
                    if(weatherType >= 0 && weatherType < 4)
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
                PlayerMotor playerMotor = GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                {
                    return error;

                }
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
                {
                    return error;
                }
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
                PlayerMotor playerMotor = GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                {
                    return error;

                }
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
            public static readonly string description = "Set Timescale.  Setting it too high can have adverse affects";
            public static readonly string usage = "set_timescale [#]";

            public static string Execute(params string[] args)
            {
                int speed;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = GameObject.FindObjectOfType<DaggerfallWorkshop.DaggerfallUnity>();

                if (daggerfallUnity == null)
                {
                    return error;

                }
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
                {
                    return error;
                }
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
                PlayerMouseLook mLook = GameObject.FindObjectOfType<PlayerMouseLook>();
                float speed = 0;
                if (args == null || args.Length < 1 || !float.TryParse(args[0], out speed))
                {
                    if (mLook)
                        Console.Log(string.Format("Current mouse sensitivity: {0}", mLook.sensitivity));
                    return HelpCommand.Execute(SetMouseSensitivity.name);
                }
                else if (mLook == null)
                {
                    return error;
                }
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
                PlayerMouseLook mLook = GameObject.FindObjectOfType<PlayerMouseLook>();
                if (mLook == null)
                {
                    return error;
                }
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
                PlayerMouseLook mLook = GameObject.FindObjectOfType<PlayerMouseLook>();
                float speed = 0;
                if (args == null || args.Length < 1 || !float.TryParse(args[0], out speed))
                {
                    if (mLook)
                        Console.Log(string.Format("Current mouse smoothing: {0}", mLook.smoothing));
                    return HelpCommand.Execute(SetMouseSmoothing.name);
                }
                else if (mLook == null)
                {
                    return error;
                }
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
                {
                    return error;
                }


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
                PlayerMotor playerMotor = GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                {
                    return error;

                }
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
                {
                    return error;
                }
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
                PlayerMotor playerMotor = GameObject.FindObjectOfType<PlayerMotor>();

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
                {
                    return error;
                }
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

                PlayerMotor playerMotor = GameObject.FindObjectOfType<PlayerMotor>();

                if (playerMotor == null)
                {
                    return error;
                }
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
                DaggerfallWorkshop.StreamingWorld streamingWorld = GameObject.FindObjectOfType<DaggerfallWorkshop.StreamingWorld>();
                PlayerEnterExit playerEE = GameObject.FindObjectOfType<PlayerEnterExit>();

                if (args == null || args.Length < 2)
                {
                    return HelpCommand.Execute(TeleportToMapPixel.name);

                }
                else if (streamingWorld == null)
                {
                    return "Could not locate Streaming world object";

                }
                else if (playerEE == null || playerEE.IsPlayerInside)
                {
                    return "PlayerEnterExit could not be found or player inside";

                }
                else if (int.TryParse(args[0], out x) && int.TryParse(args[1], out y))
                {
                    if (x < 0 || y < 0)
                        return "Invalid Coordinates";
                    else if (x >= MapsFile.MaxMapPixelX || y >= MapsFile.MaxMapPixelY)
                        return "Invalid coordiantes";
                    else
                    {
                        streamingWorld.TeleportToCoordinates(x, y);
                        return string.Format("Teleporting player to: {0} {1}", x, y);

                    }

                }

                return "Invalid coordiantes";
            }


        }

        private static class GotoLocation
        {
            public static readonly string name = "location";
            public static readonly string description = "Send the player to the predefined location";
            public static readonly string usage = "loaction [n]; where n is between 0 & 9:\n0 ... random location\n1 ... Daggerfall/Daggerfall\n2 ... Wayrest/Wayrest\n3 ... Sentinel/Sentinel\n4 ... Orsinium Area/Orsinium\n5 ... Tulune/The Old Copperham Place\n6 ... Pothago/The Stronghold of Cirden\n7 ... Daggerfall/Privateer's Hold\n8 ... Wayrest/Merwark Hollow\n9 ... Isle of Balfiera/Direnni Tower\n";

            public static string Execute(params string[] args)
            {
                int n = 0;
                DaggerfallWorkshop.StreamingWorld streamingWorld = GameObject.FindObjectOfType<DaggerfallWorkshop.StreamingWorld>();
                PlayerEnterExit playerEE = GameObject.FindObjectOfType<PlayerEnterExit>();

                if (args == null || args.Length < 1)
                {
                    return HelpCommand.Execute(GotoLocation.name);

                }
                else if (streamingWorld == null)
                {
                    return "Could not locate Streaming world object";

                }
                else if (playerEE == null || playerEE.IsPlayerInside)
                {
                    return "PlayerEnterExit could not be found or player inside";

                }
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
                                streamingWorld.TeleportToCoordinates(207, 213);
                                return ("Teleported player to Daggerfall/Daggerfall");
                            case 2:
                                streamingWorld.TeleportToCoordinates(859, 244);
                                return ("Teleported player to Wayrest/Wayrest");
                            case 3:
                                streamingWorld.TeleportToCoordinates(397, 343);
                                return ("Teleported player to Sentinel/Sentinel");
                            case 4:
                                streamingWorld.TeleportToCoordinates(892, 146);
                                return ("Teleported player to Orsinium Area/Orsinium");
                            case 5:
                                streamingWorld.TeleportToCoordinates(67, 119);
                                return ("Teleported player to Tulune/The Old Copperham Place");
                            case 6:
                                streamingWorld.TeleportToCoordinates(254, 408);
                                return ("Teleported player to Pothago/The Stronghold of Cirden");
                            case 7:
                                streamingWorld.TeleportToCoordinates(109, 158);
                                return ("Teleported player to Daggerfall/Privateer's Hold");
                            case 8:
                                streamingWorld.TeleportToCoordinates(860, 245);
                                return ("Teleported player to Wayrest/Merwark Hollow");
                            case 9:
                                streamingWorld.TeleportToCoordinates(718, 204);
                                return ("Teleported player to Isle of Balfiera/Direnni Tower");
                            default:
                                break;
                        }
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
                PlayerEnterExit playerEnterExit = GameObject.FindObjectOfType<PlayerEnterExit>();
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
                PlayerEnterExit playerEnterExit = GameObject.FindObjectOfType<PlayerEnterExit>();
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player") as GameObject;
                if (playerObj == null || playerEnterExit == null || !playerEnterExit.IsPlayerInsideDungeon)
                {
                    return error;
                }
                else
                {
                    try
                    {
                        playerObj.transform.position = playerEnterExit.Dungeon.EnterMarker.transform.position;
                        return "Transitioning to door positon";


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
            public static readonly string error = "Player not inside, or couldn't locate PlayerEnterExit object";
            public static readonly string description = "Opens all doors in an interior or dungeon, regardless of locked state";
            public static readonly string usage = "openalldoors";

            public static string Execute(params string[] args)
            {
                PlayerEnterExit playerEnterExit = GameObject.FindObjectOfType<PlayerEnterExit>();

                if (playerEnterExit == null || !playerEnterExit.IsPlayerInside)
                {

                    return error;
                }
                else if (playerEnterExit.IsPlayerInside)
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

                    return string.Format("Finished.  Opened {0} doors out of {1}", count, doors.Length);


                }
                else
                {
                    return error;

                }
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






    }
}
