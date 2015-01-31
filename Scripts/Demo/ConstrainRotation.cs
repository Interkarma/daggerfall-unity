// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Constrains rotation.
    /// Used to stop particle effects from rotating with player.
    /// </summary>
    public class ConstrainRotation : MonoBehaviour
    {
        void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}