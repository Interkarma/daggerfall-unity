// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Allofich
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//

using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Azura's Star is a powerful soul trap.
    /// </summary>
    public class AzurasStarEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Azuras_Star.ToString();

        const int soulReleasedID = 32;
        const int noSoulToReleaseID = 20;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held | EnchantmentPayloadFlags.Used;
        }

        #region Payloads

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Set flag that Azura's Star is equipped and active
            if (entityBehaviour.EntityType == EntityTypes.Player)
            {
                (entityBehaviour.Entity as PlayerEntity).IsAzurasStarEquipped = true;
            }
        }

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem, sourceDamage);

            // Validate
            if (sourceItem == null)
                return null;

            // Used payload
            if (context == EnchantmentPayloadFlags.Used)
            {
                if (sourceItem.TrappedSoulType != MobileTypes.None)
                {
                    sourceItem.TrappedSoulType = MobileTypes.None;
                    DaggerfallUI.MessageBox(soulReleasedID);
                }
                else
                {
                    DaggerfallUI.MessageBox(noSoulToReleaseID);
                }
            }

            return null;
        }

        #endregion
    }
}
