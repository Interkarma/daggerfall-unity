// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by the Ring of Namira. Reflects damage back at an attacker at a cost to the item's durability.
    /// </summary>
    public class RingOfNamiraEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Ring_of_Namira.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.None;
        }

        #region Payloads

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem, sourceDamage);

            EnemyEntity enemy = (EnemyEntity)targetEntity.Entity;
            int reflectedDamage;
            switch (enemy.MobileEnemy.Team)
            {
                // Animals and Spriggans
                case MobileTeams.Vermin:
                case MobileTeams.Spriggans:
                case MobileTeams.Bears:
                case MobileTeams.Tigers:
                case MobileTeams.Spiders:
                case MobileTeams.Scorpions:
                    return new PayloadCallbackResults();
                case MobileTeams.Daedra:
                    reflectedDamage = sourceDamage / 2;
                    break;
                case MobileTeams.Undead:
                    reflectedDamage = sourceDamage * 2;
                    break;
                // Everyone else (i.e. Humanoids and Monsters)
                default:
                    reflectedDamage = sourceDamage;
                    break;
            }
            enemy.CurrentHealth -= reflectedDamage;

            // Durability loss for this effect
            return new PayloadCallbackResults()
            {
                durabilityLoss = reflectedDamage,
            };
        }

        #endregion
    }
}
