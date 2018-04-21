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

using System;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Spell class used to transport and recast spells.
    /// Primarily a container for effect bundle settings which define the actual "magic effects" used by spell.
    /// </summary>
    public class Spell
    {
        EffectBundleSettings settings;

        public EffectBundleSettings Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Spell(EffectBundleSettings settings)
        {
            this.settings = settings;
        }
    }
}