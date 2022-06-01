// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//

using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by Mehrunes Razor. Kills target instantly if it does not save vs. magic.
    /// </summary>
    public class MehrunesRazorEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Mehrunes_Razor.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Strikes;
        }

        #region Payloads

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem, sourceDamage);

            // Validate
            if (context != EnchantmentPayloadFlags.Strikes || targetEntity == null || sourceItem == null)
                return null;

            // Entity must save vs magic
            if (FormulaHelper.SavingThrow(DFCareer.Elements.Magic, DFCareer.EffectFlags.Magic, targetEntity.Entity, 0) != 0)
            {
                // Kill target instantly - durability loss is equal to target health removed
                int healthRemoved = targetEntity.Entity.CurrentHealth;
                return new PayloadCallbackResults()
                {
                    strikesModulateDamage = healthRemoved,
                    durabilityLoss = healthRemoved,
                };
            }

            return null;
        }

        #endregion
    }
}
