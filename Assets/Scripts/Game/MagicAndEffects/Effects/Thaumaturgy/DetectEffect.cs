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

using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Detect effect base.
    /// Provides functionality common to detection effects.
    /// </summary>
    public abstract class DetectEffect : IncumbentEffect
    {
        const string textDatabase = "ClassicEffects";

        public List<PlayerGPS.NearbyObject> DetectedObjects { get; protected set; }

        public int DetectedObjectsCount
        {
            get { return (DetectedObjects != null && DetectedObjects.Count > 0) ? DetectedObjects.Count : 0; }
        }
    }
}