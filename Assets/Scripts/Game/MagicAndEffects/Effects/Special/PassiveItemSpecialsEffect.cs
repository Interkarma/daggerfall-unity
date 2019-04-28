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

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            CacheReferences();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            CacheReferences();
        }

        public override void End()
        {
            base.End();
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Execute constant advantages/disadvantages
            if (entityBehaviour && enchantedItem != null)
                ConstantEnchantments();
        }

        #endregion

        #region Private Methods

        void CacheReferences()
        {
            // Cache reference to item carrying enchantments for this effect
            if (ParentBundle == null || ParentBundle.fromEquippedItem != null)
                enchantedItem = ParentBundle.fromEquippedItem;

            // Cache reference to peered entity behaviour
            if (!entityBehaviour)
                entityBehaviour = GetPeeredEntityBehaviour(manager);
        }

        void ConstantEnchantments()
        {
            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //long startTime = stopwatch.ElapsedMilliseconds;

            // Constant enchantments tick every frame
            for (int i = 0; i < enchantedItem.LegacyEnchantments.Length; i++)
            {
                switch (enchantedItem.LegacyEnchantments[i].type)
                {
                    case EnchantmentTypes.AbsorbsSpells:
                        entityBehaviour.Entity.IsAbsorbingSpells = true;
                        break;
                }
            }

            //long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            //Debug.LogFormat("Time to run PassiveItemSpecialsEffect.ConstantEnchantments(): {0}ms", totalTime);
        }

        #endregion
    }
}