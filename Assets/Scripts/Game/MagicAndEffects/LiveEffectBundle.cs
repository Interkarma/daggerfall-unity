// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Stores an instanced effect bundle for executing effects.
    /// </summary>
    public class LiveEffectBundle
    {
        public int version;
        public BundleTypes bundleType;
        public TargetTypes targetType;
        public ElementTypes elementType;
        public BundleRuntimeFlags runtimeFlags;
        public string name;
        public int iconIndex;
        public SpellIcon icon;
        public DaggerfallEntityBehaviour caster;
        public EntityTypes casterEntityType;
        public ulong casterLoadID;
        public DaggerfallUnityItem fromEquippedItem;
        public DaggerfallUnityItem castByItem;
        public List<IEntityEffect> liveEffects;
    }
}
