// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    public class DaggerfallCityGate : MonoBehaviour
    {
        bool isOpen = true;

        public void SetOpen(bool open)
        {
            // Save new state
            isOpen = open;

            // Change model
            DaggerfallMesh mesh = GetComponent<DaggerfallMesh>();
            if (mesh != null)
            {
                // Set open/closed
                if (isOpen)
                    GameObjectHelper.ChangeDaggerfallMeshGameObject(mesh, RMBLayout.CityGateOpenModelID);
                else
                    GameObjectHelper.ChangeDaggerfallMeshGameObject(mesh, RMBLayout.CityGateClosedModelID);

                // Update climate
                mesh.ApplyCurrentClimate();
            }
        }

        public void Toggle()
        {
            SetOpen(!isOpen);
        }

        private void Update()
        {
            // Toggle closed at 18:00 and open again at 06:00
            bool isNight = DaggerfallUnity.Instance.WorldTime.Now.IsNight;
            if (isNight && isOpen || !isNight && !isOpen)
                Toggle();
        }
    }
}