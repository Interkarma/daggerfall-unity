using UnityEngine;
using System;
using System.Collections;
using DaggerfallWorkshop.Game.Weather;
using Wenzil.Console;

//auto update moon phase
namespace EnhancedSky
{
    public static class EnhancedSkyConsoleCommands
    {
        
        public static void RegisterCommands()
        {
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(ToggleEnhancedSky.name, ToggleEnhancedSky.description, ToggleEnhancedSky.usage, ToggleEnhancedSky.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ToggleSunFlare.name, ToggleSunFlare.description, ToggleSunFlare.usage, ToggleSunFlare.Execute);
                ConsoleCommandsDatabase.RegisterCommand(SetCloudQuality.name, SetCloudQuality.description, SetCloudQuality.usage, SetCloudQuality.Execute);
                ConsoleCommandsDatabase.RegisterCommand(SetCloudSeed.name, SetCloudSeed.description, SetCloudSeed.usage, SetCloudSeed.Execute);
                ConsoleCommandsDatabase.RegisterCommand(SetMoonPhases.name, SetMoonPhases.description, SetMoonPhases.usage, SetMoonPhases.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ToggleSkyObjectSize.name, ToggleSkyObjectSize.description, ToggleSkyObjectSize.usage, ToggleSkyObjectSize.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ToggleMoonPhaseUpdating.name, ToggleMoonPhaseUpdating.description, ToggleMoonPhaseUpdating.usage, ToggleMoonPhaseUpdating.Execute);
                ConsoleCommandsDatabase.RegisterCommand(RefreshClouds.name, RefreshClouds.description, RefreshClouds.usage, RefreshClouds.Execute);

            }
            catch(Exception ex)
            {
                Debug.LogError(string.Format("Error Registering Enhanced Sky Console commands: {0}", ex.Message));

            }


        }



        private static class ToggleEnhancedSky
        {
            public static readonly string name = "esky_toggle";
            public static readonly string description = "Toggles between enhanced sky addon and vanilla daggerfall sky";
            public static readonly string usage = "esky_toggle";

            

            public static string Execute(params string[] args)
            {
                SkyManager skyMan = SkyManager.instance;
                if(skyMan == null)
                {
                    return "SkyManager instance not found";
                }
                else
                {
                    skyMan.ToggleEnhancedSky(!skyMan.EnhancedSkyCurrentToggle);
                    return string.Format("SkyManager toggled; current state on: {0}", skyMan.EnhancedSkyCurrentToggle);
                }

            }


        }


        private static class ToggleSunFlare
        {
            public static readonly string name          = "esky_tflare";
            public static readonly string description   = "Toggles the sun flare on and off";
            public static readonly string usage         = "esky_tflare";



            public static string Execute(params string[] args)
            {
                SkyManager skyMan = SkyManager.instance;

                if (skyMan == null)
                {
                    return "SkyManager instance not found";
                }
                else
                {
                    skyMan.UseSunFlare = !skyMan.UseSunFlare;
                    return string.Format("SkyManager: Sun flare toggled; current state: {0}", skyMan.UseSunFlare);
                }
            }

        }


       private static class SetCloudQuality
       {
           public static readonly string name = "esky_set_cldqual";
           public static readonly string description = "Sets cloud texture resolution.  Change will not take place until clouds are updated.  Default is 600";
           public static readonly string usage = string.Format("esky_set_cldqual [#]; Between {0} - {1} ", PresetContainer.MINCLOUDDIMENSION, PresetContainer.MAXCLOUDDIMENSION);

           public static string Execute(params string[] args)
           {
               SkyManager skyMan = SkyManager.instance;
               //Cloud cloud = GameObject.FindObjectOfType<Cloud>();

               int quality = 1;

               if(args == null || args.Length < 1 || skyMan == null)
               {
                   return Wenzil.Console.Commands.HelpCommand.Execute(SetCloudQuality.name);

               }
               else if(!int.TryParse(args[0], out quality))
               {
                   return Wenzil.Console.Commands.HelpCommand.Execute(SetCloudQuality.name);

               }
               else
               {
                   //skyMan.cloudQuality = quality;
                   skyMan.SetCloudTextureResolution(quality);
                   return string.Format("Set cloud quality to : {0}", skyMan.cloudQuality);
               }
     
           }

       }


       private static class SetCloudSeed
       {
           public static readonly string name = "esky_set_seed";
           public static readonly string description = "Sets the int seed used to generate clouds. A negative seed will generate a random seed for each cloud.";
           public static readonly string usage = "esky_set_seed [#] ";

           public static string Execute(params string[] args)
           {
               int seed = 1;

               if (args == null || args.Length < 1)
               {
                   return Wenzil.Console.Commands.HelpCommand.Execute(SetCloudSeed.name);

               }
               else if (!int.TryParse(args[0], out seed))
               {
                   return Wenzil.Console.Commands.HelpCommand.Execute(SetCloudSeed.name);

               }
               else
               {
                   SkyManager.instance.cloudSeed = seed;
                   return string.Format("Set cloud seed to : {0}", SkyManager.instance.cloudSeed);

               }

           }




       }

       private static class ToggleSkyObjectSize
       {
           public static readonly string name = "esky_tsize";
           public static readonly string description = "Toggle between normal & larger moons & sun";
           public static readonly string usage = "esky_tsize";

           public static string Execute(params string[] args)
           {
               SkyManager skyMan = SkyManager.instance;
               if (SkyManager.instance == null)
               {
                   return "SkyManager instance not found";
               }
               else
               {
                   switch (skyMan.SkyObjectSizeSetting)
                   {
                       case SkyObjectSize.Normal:
                           {
                               skyMan.SkyObjectSizeChange(SkyObjectSize.Large);
                           }
                           break;
                       case SkyObjectSize.Large:
                           {
                               skyMan.SkyObjectSizeChange(SkyObjectSize.Normal);

                           }
                           break;
                       default:
                           break;
                   }

               }

               return string.Format("Set sky object size normal: {0} ", skyMan.SkyObjectSizeSetting);

           }


       }

       private static class ToggleMoonPhaseUpdating
       {
           public static readonly string name = "esky_tmoonupdate";
           public static readonly string description = "Toggle the moon phase updates.  This is entirely cosmetic";
           public static readonly string usage = "esky_tmoonupdate";

           public static string Execute(params string[] args)
           {
               MoonController moon = GameObject.FindObjectOfType<MoonController>();
               if (!SkyManager.instance.EnhancedSkyCurrentToggle || moon == null)
               {
                   return "Enhanced Sky Must be enabled to set this";
                   
               }
               else
               {
                   moon.autoUpdatePhase = !moon.autoUpdatePhase;
                   return string.Format("Toggled moon phase updates; current state: {0} ", moon.autoUpdatePhase);
               }

           }

       }

       private static class RefreshClouds
       {
           public static readonly string name = "esky_updcld";
           public static readonly string description = "Refreshes the cloud texture.";
           public static readonly string usage = "esky_updcld";

           public static string Execute(params string[] args)
           {
               if (!SkyManager.instance.EnhancedSkyCurrentToggle)
               {
                   return "Enhanced Sky Must be enabled to set this";
               }
               else
               {
                   SkyManager.instance.WeatherManagerSkyEventsHandler(WeatherType.Sunny);
                   return string.Format("Finished updating cloud.");
               }
           }

       }


       private static class SetMoonPhases   //player must be outside w/ Esky active currently, as moon controller won't be found when disabled
       {                                    //could setup an event in Skymanager to trigger this
           public static readonly string name = "esky_set_moons";
           public static readonly string description = "Sets current phase of each moon. First argument is for Masser & second is for Secunda. ";
           public static readonly string usage = string.Format("esky_set_moons [# #]; \n0: New Moon \n1: One Waxing \n2: Half Waxing \n3: Three Waxing \n4: Full \n5: Three wane \n6: Half Wane \n7: One wane ");

           public static string Execute(params string[] args)
           {
               MoonController moon = GameObject.FindObjectOfType<MoonController>();

               int masserPhase = 1;
               int secundaPhase = 1;

               if(!SkyManager.instance.EnhancedSkyCurrentToggle)
               {
                   return "Enhanced Sky Must be enabled to set this";
               }
               else if (args == null || args.Length < 2 || moon == null)
               {
                   return Wenzil.Console.Commands.HelpCommand.Execute(SetMoonPhases.name);

               }
               else if (!int.TryParse(args[0],  out masserPhase) || !int.TryParse(args[1], out secundaPhase))
               {
                   return Wenzil.Console.Commands.HelpCommand.Execute(SetMoonPhases.name);

               }
               else if(!Enum.IsDefined(typeof(MoonController.MoonPhases), masserPhase) || !Enum.IsDefined(typeof(MoonController.MoonPhases), secundaPhase))
               {
                   return Wenzil.Console.Commands.HelpCommand.Execute(SetMoonPhases.name);
               }
               else
               {
                   moon.SetPhase((MoonController.MoonPhases)masserPhase, (MoonController.MoonPhases)secundaPhase);
                   return string.Format("Set moon phases.  Masser phase: {0} Secunda phase: {1}", moon.MasserPhase.ToString(), moon.SecundaPhase.ToString());
               }



           }




       }
        

       
    }
}
