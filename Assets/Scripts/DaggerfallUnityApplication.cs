// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2024 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: kaboissonneault
// Contributors:    
// 
// Notes:
//

//#define SEPARATE_DEV_PERSISTENT_PATH

using System;
using System.IO;
using UnityEngine;

public static class DaggerfallUnityApplication
{
    static string persistentDataPath;
    private static bool? isPortableInstall;

    public static bool IsPortableInstall
    {
        get
        {
            if (isPortableInstall == null)
            {
                isPortableInstall = !Application.isEditor && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Portable.txt"));
            }

            return isPortableInstall.Value;
        }
    }

    public static string PersistentDataPath
    {
        get
        {
            if (persistentDataPath == null)
            {
                InitializePersistentPath();
            }

            return persistentDataPath;
        }
    }

    private static void InitializePersistentPath()
    {
#if UNITY_EDITOR && SEPARATE_DEV_PERSISTENT_PATH
        persistentDataPath = String.Concat(Application.persistentDataPath, ".devenv");
        Directory.CreateDirectory(persistentDataPath);
#else
        if (IsPortableInstall)
        {
            persistentDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PortableAppdata");
            Directory.CreateDirectory(persistentDataPath);
        }
        else
        {
            persistentDataPath = Application.persistentDataPath;
        }
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void SubsystemInit()
    {
        if (persistentDataPath == null)
        {
            InitializePersistentPath();
        }
        InitLog();
    }


    public class LogHandler : ILogHandler, IDisposable
    {
        private StreamWriter streamWriter;

        public LogHandler()
        {
            string filePath = Path.Combine(persistentDataPath, "Player.log");

            string errorMessage = null;
            try
            {
                if(File.Exists(filePath))
                {
                    string prevPath = Path.Combine(persistentDataPath, "Player-prev.log");
                    File.Delete(prevPath);
                    File.Move(filePath, prevPath);
                }
            }
            catch(Exception e)
            {
                errorMessage = $"Could not preserve previous log: {e.Message}";
            }
                        
            streamWriter = File.CreateText(filePath);
            streamWriter.AutoFlush = true;

            if(!string.IsNullOrEmpty(errorMessage))
            {
                streamWriter.WriteLine(errorMessage);
            }
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            streamWriter.WriteLine(exception.ToString());
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            string prefix = "";
            switch(logType)
            {
                case LogType.Error:
                    prefix = "[Error] ";
                    break;

                case LogType.Warning:
                    prefix = "[Warning] ";
                    break;

                case LogType.Assert:
                    prefix = "[Assert] ";
                    break;

                case LogType.Exception:
                    prefix = "[Exception] ";
                    break;
            }

            streamWriter.WriteLine(prefix + string.Format(format, args));
        }

        public void Dispose()
        {
            streamWriter.Close();
        }
    }

    static void InitLog()
    {
        if (Application.isPlaying && Application.installMode != ApplicationInstallMode.Editor)
        {
            Debug.unityLogger.logHandler = new LogHandler();
        }
    }
}
