// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Uber-effect used to deliver unique item powers and side-effects to entities.
    /// Not incumbent as most item powers are stackable and entity could have multiple instances of this effect running.
    /// NOTES:
    ///  * Now that enchantment system is built, some or all of these will be moved to their corresponding effect.
    ///  * Future item enchantment payloads should be implemented with their own effect class.
    /// </summary>
    public class PassiveItemSpecialsEffect : BaseEntityEffect
    {
        #region Fields

        public static readonly string EffectKey = "Passive-Item-Specials";

        DaggerfallUnityItem enchantedItem;
        DaggerfallEntityBehaviour entityBehaviour;

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            bypassSavingThrows = true;
        }

        #endregion
    }
}