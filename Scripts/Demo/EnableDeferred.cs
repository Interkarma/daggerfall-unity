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
    /// Enables deferred where pro license available.
    /// Daggerfall uses so many point lights that deferred is preferred.
    /// This component is used by Workshop demo scenes.
    /// </summary>
    public class EnableDeferred : MonoBehaviour
    {
        void Awake()
        {
            if (Application.HasProLicense())
            {
                Camera.main.renderingPath = RenderingPath.DeferredLighting;
            }
        }
    }
}