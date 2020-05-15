using System;
using System.Globalization;
using UnityEngine;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game
{
    public class FPSConsoleCommands
    {
        const string noInstanceMessage = "FPS Weapon instance not found.";

        public static void RegisterCommands()
        {
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(OffsetDistance.name, OffsetDistance.description, OffsetDistance.usage, OffsetDistance.Execute);
                ConsoleCommandsDatabase.RegisterCommand(DisableSmoothAnimations.name, DisableSmoothAnimations.description, DisableSmoothAnimations.usage, DisableSmoothAnimations.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ChangeAttackSpeed.name, ChangeAttackSpeed.description, ChangeAttackSpeed.usage, ChangeAttackSpeed.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ChangeHorPos.name, ChangeHorPos.description, ChangeHorPos.usage, ChangeHorPos.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ChangeRaycastLerp.name, ChangeRaycastLerp.description, ChangeRaycastLerp.usage, ChangeRaycastLerp.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ChangeWeaponIndex.name, ChangeWeaponIndex.description, ChangeWeaponIndex.usage, ChangeWeaponIndex.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ChangeMovementMods.name, ChangeMovementMods.description, ChangeMovementMods.usage, ChangeMovementMods.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering RealGrass Console commands: {0}", e.Message));
            }
        }

        public class OffsetDistance
        {
            public static readonly string name = "OffsetDistance";
            public static readonly string error = "Failed to set OffsetDistance - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Changes animation offset distance";
            public static readonly string usage = "OffsetDistance";

            public static float offsetDistance { get; private set; }

            public static string Execute(params string[] args)
            {
                float lrange;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;

                if (daggerfallUnity == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    return "Insert a number";
                }
                else if (!float.TryParse(args[0], out lrange))
                    return error;
                else if (lrange < 0)
                    return "Improper number";
                else
                {
                    try
                    {
                        offsetDistance = lrange;
                        return string.Format("Lerp set to: {0}", lrange);
                    }
                    catch
                    {
                        return "Unspecified error; failed to set lerp";
                    }

                }
            }
        }

        public class DisableSmoothAnimations
        {
            public static readonly string name = "DisableSmoothAnimations";
            public static readonly string error = "Failed to set DisableSmoothAnimations - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Enables or disables smooth animations";
            public static readonly string usage = "DisableSmoothAnimations";

            public static bool disableSmoothAnimations { get; private set; }

            public static string Execute(params string[] args)
            {
                bool trigger;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;

                if (daggerfallUnity == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    return "true or false";
                }
                else if (!bool.TryParse(args[0], out trigger))
                    return error;
                else
                {
                    try
                    {
                        disableSmoothAnimations = trigger;
                        return string.Format("trigger set to:" + trigger.ToString());
                    }
                    catch
                    {
                        return "Unspecified error; failed to set lerp";
                    }

                }
            }
        }

        public class ChangeAttackSpeed
        {
            public static readonly string name = "ChangeAttackSpeed";
            public static readonly string error = "Failed to set ChangeAttackSpeed - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Changed AttackSpeed";
            public static readonly string usage = "ChangeAttackSpeed";

            public static float changeAttackSpeed { get; private set; }

            public static string Execute(params string[] args)
            {
                float AttackSpeed;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;

                if (daggerfallUnity == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    return "true or false";
                }
                else if (!float.TryParse(args[0], out AttackSpeed))
                    return error;
                else
                {
                    try
                    {
                        changeAttackSpeed = AttackSpeed;
                        return string.Format("trigger set to:" + AttackSpeed.ToString());
                    }
                    catch
                    {
                        return "Unspecified error; failed to set lerp";
                    }

                }
            }
        }

        public class ChangeWeaponIndex
        {
            public static readonly string name = "WeaponIndex";
            public static readonly string error = "Failed to set WeaponIndex - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Changed the weapon index";
            public static readonly string usage = "WeaponIndex";

            public static int changeWeaponIndex { get; private set; }

            public static string Execute(params string[] args)
            {
                int WeaponIndex;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;

                if (daggerfallUnity == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    return "true or false";
                }
                else if (!int.TryParse(args[0], out WeaponIndex))
                    return error;
                else
                {
                    try
                    {
                        changeWeaponIndex = WeaponIndex;
                        return string.Format("lerpValue set to:" + WeaponIndex.ToString());
                    }
                    catch
                    {
                        return "Unspecified error; failed to set lerp";
                    }

                }
            }
        }

        public class ChangeRaycastLerp
        {
            public static readonly string name = "ChangeRaycastLerp";
            public static readonly string error = "Failed to set ChangeRaycastLerp - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Changed speed that the raycast arc is drawn using the lerp time value";
            public static readonly string usage = "ChangeRaycastLerp";

            public static float changeRaycastLerp { get; private set; }

            public static string Execute(params string[] args)
            {
                float lerpValue;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;

                if (daggerfallUnity == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    return "true or false";
                }
                else if (!float.TryParse(args[0], out lerpValue))
                    return error;
                else
                {
                    try
                    {
                        changeRaycastLerp = lerpValue;
                        return string.Format("lerpValue set to:" + lerpValue.ToString());
                    }
                    catch
                    {
                        return "Unspecified error; failed to set lerp";
                    }

                }
            }
        }

        public class ChangeHorPos
        {
            public static readonly string name = "ChangeHorPos";
            public static readonly string error = "Failed to set ChangeHorPos - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Change attack horizental start position";
            public static readonly string usage = "ChangeHorPos";

            public static float SchangeHorPos { get; private set; }
            public static float EchangeHorPos { get; private set; }

            public static string Execute(params string[] args)
            {
                float SHorPos;
                float EHorPos;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;

                if (daggerfallUnity == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    return "-start value -end value || -s value -e value";
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "-start" || args[i] == "-s")
                        {
                            if (!float.TryParse(args[i + 1], out SHorPos))
                             return error;

                            try
                            {
                                SchangeHorPos = SHorPos;
                            }
                            catch
                            {
                                return "Unspecified error; failed to set lerp";
                            }

                        }

                        if (args[i] == "-end" || args[i] == "-e")
                        {
                            if (!float.TryParse(args[i + 1], out EHorPos))
                                return error;

                            try
                            { 
                                EchangeHorPos = EHorPos;
                            }
                            catch
                            {
                                return "Unspecified error; failed to set lerp";
                            }
                        }                
                    }
                }
                return string.Format("HorPos set to Start: " + SchangeHorPos.ToString() + ", End: " + EchangeHorPos.ToString());
            }
        }

        public class ChangeMovementMods
        {
            public static readonly string name = "ChangeMovementMods";
            public static readonly string error = "Failed to set ChangeMovementMods - invalid setting or DaggerfallUnity singleton object";
            public static readonly string description = "Change sheathed and attack movement modifiers";
            public static readonly string usage = "ChangeMovementMods";

            public static float ESheathedModifier { get; set; }
            public static float EAttackModifier { get; set; }

            public static string Execute(params string[] args)
            {
                float SheathedModifier;
                float AttackModifier;
                DaggerfallWorkshop.DaggerfallUnity daggerfallUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;

                if (daggerfallUnity == null)
                    return error;

                if (args == null || args.Length < 1)
                {
                    return "-sheathed value -attack value";
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "-sheathed")
                        {
                            if (!float.TryParse(args[i + 1], out SheathedModifier))
                                return error;

                            try
                            {
                                ESheathedModifier = SheathedModifier;
                            }
                            catch
                            {
                                return "Unspecified error; failed to set lerp";
                            }

                        }

                        if (args[i] == "-attack")
                        {
                            if (!float.TryParse(args[i + 1], out AttackModifier))
                                return error;

                            try
                            {
                                EAttackModifier = AttackModifier;
                            }
                            catch
                            {
                                return "Unspecified error; failed to set lerp";
                            }
                        }
                    }
                }
                return string.Format("Sheathed Speed Modifier: " + ESheathedModifier.ToString() + ", Attack Speed Modifier: " + EAttackModifier.ToString());
            }
        }
    }
}