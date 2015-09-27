// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Serialization
{
    public class SaveLoad : MonoBehaviour
    {
        #region Fields
        #endregion

        #region Singleton

        static SaveLoad instance = null;
        public static SaveLoad Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindSingleton(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "SaveLoad";
                        instance = go.AddComponent<SaveLoad>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        #endregion

        #region Unity

        void Start()
        {
            SetupSingleton();
        }

        #endregion

        #region Public Static Methods

        public static bool FindSingleton(out SaveLoad singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType(typeof(SaveLoad)) as SaveLoad;
            if (singletonOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate SaveLoad GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple SaveLoad instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        #endregion
    }
}