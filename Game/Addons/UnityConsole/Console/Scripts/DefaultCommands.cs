using UnityEngine;
using Wenzil.Console.Commands;
using DaggerfallWorkshop.Game;
using System.Linq;
using System;
using DaggerfallConnect.Arena2;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop;

namespace Wenzil.Console
{
    public class DefaultCommands : MonoBehaviour
    {


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
            ConsoleCommandsDatabase.RegisterCommand(SetWalkSpeed.name, SetWalkSpeed.description, SetWalkSpeed.usage, SetWalkSpeed.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetMouseSensitivity.name, SetMouseSensitivity.description, SetMouseSensitivity.usage, SetMouseSensitivity.Execute);
            //ConsoleCommandsDatabase.RegisterCommand(SetMouseSmoothing.name, SetMouseSmoothing.description, SetMouseSmoothing.usage, SetMouseSmoothing.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetVSync.name, SetVSync.description, SetVSync.usage, SetVSync.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetRunSpeed.name, SetRunSpeed.description, SetRunSpeed.usage, SetRunSpeed.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetJumpSpeed.name, SetJumpSpeed.description, SetJumpSpeed.usage, SetJumpSpeed.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ToggleAirControl.name, ToggleAirControl.description, ToggleAirControl.usage, ToggleAirControl.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetTimeScale.name, SetTimeScale.description, SetTimeScale.usage, SetTimeScale.Execute);
            ConsoleCommandsDatabase.RegisterCommand(SetGravity.name, SetGravity.description, SetGravity.usage, SetGravity.Execute);

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
            public static readonly string name = "tdg";
            public static readonly string error = "Failed to toggle debug string setting - StreamingWorld object not found?";
            public static readonly string description = "Toggles if the debug information is displayed";
            public static readonly string usage = "tdg";

            public static string Execute(params string[] args)
            {
                DaggerfallWorkshop.StreamingWorld streamingWorld = GameObject.FindObjectOfType<DaggerfallWorkshop.StreamingWorld>();
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = GameObject.FindObjectOfType<DaggerfallWorkshop.DaggerfallUnity>();

                if (!streamingWorld || !daggerfallUnity)
                {
                    return error;

                }
                else
                {

                    streamingWorld.ShowDebugString = !streamingWorld.ShowDebugString;
                    daggerfallUnity.WorldTime.ShowDebugString = !daggerfallUnity.WorldTime.ShowDebugString;
                    return string.Format("Debug string: {0}", streamingWorld.ShowDebugString);

                }



            }
        }




        /// <summary>
        /// This is not added yet.
        /// </summary>
        private static class SetHealth
        {
            public static readonly string name = "set_health";
            public static readonly string error = "Failed to set health - invalid setting or Player health object not found";
            public static readonly string description = "Set Health";
            public static readonly string usage = "set_Health [#]";

            public static string Execute(params string[] args)
            {
                int health;
                PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();

                if (args.Length < 1)
                {
                    return HelpCommand.Execute(SetHealth.name);

                }
                else if (playerHealth == null || int.TryParse(args[0], out health))
                {
                    return error;
                }
                else
                {
                    return "blah";
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
                PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
                if (playerHealth == null)
                {
                    return error;
                }
                else
                {
                    playerHealth.SendMessage("RemoveHealth", 999999);       //don't know how to get player health anymore :/
                    return "Are you still there?";
                }



            }
        }

        private static class SetWeather
        {
            public static readonly string name = "set_weather";
            public static readonly string error = "Failed to set weathear - invalid argument or no Player Weather object";
            public static readonly string description = "Sets the weather to indicated type";
            public static readonly string usage = "set_weather [0 = none, 1 = rain, 2 = snow]";

            public static string Execute(params string[] args)
            {
                PlayerWeather playerWeather = GameObject.FindObjectOfType<PlayerWeather>();

                if (args.Length < 1)
                {
                    return usage;

                }
                else if (playerWeather == null)
                {
                    return error;

                }
                //else if(Enum.IsDefined(typeof(PlayerWeather.WeatherTypes), args[0]))
                else
                {
                    int type;
                    int.TryParse(args[0], out type);
                    if (Enum.IsDefined(typeof(PlayerWeather.WeatherTypes), type))
                    {
                        playerWeather.WeatherType = (PlayerWeather.WeatherTypes)Enum.Parse(typeof(PlayerWeather.WeatherTypes), args[0], true);
                        return "Weather set";
                    }
                    else
                    {
                        return "Error when trying to set weather";
                    }

                }




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
                if (args.Length < 1)
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
                if (args.Length < 1)
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
                if (args.Length < 1)
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
                if (args.Length < 1)
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
                if (args.Length < 1)
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
                EnemyHealth[] enemies = GameObject.FindObjectsOfType<EnemyHealth>();
                int count = 0;
                for (int i = 0; i < enemies.Count(); i++)
                {
                    try
                    {
                        enemies[i].RemoveHealth(enemies[i].gameObject, enemies[i].Health * 2, Vector3.zero);
                        count++;
                    }
                    catch
                    {
                        return "Unknown Error";

                    }

                }

                return string.Format("Finished.  Killed {0} out of {1} enemies", count, enemies.Count());



            }


        }

    }
}
