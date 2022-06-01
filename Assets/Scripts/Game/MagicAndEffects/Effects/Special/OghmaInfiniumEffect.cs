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
    /// Oghma Infinium artifact gives player additional attribute points to distribute.
    /// </summary>
    public class OghmaInfiniumEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Oghma_Infinium.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Used;
        }

        #region Payloads

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem, sourceDamage);

            // Validate
            if (context != EnchantmentPayloadFlags.Used || sourceEntity == null || sourceEntity.EntityType != EntityTypes.Player || sourceItem == null)
                return null;

            // Start Oghma level up and remove item
            GameManager.Instance.PlayerEntity.ReadyToLevelUp = true;
            GameManager.Instance.PlayerEntity.OghmaLevelUp = true;
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenCharacterSheetWindow);
            GameManager.Instance.PlayerEntity.Items.RemoveItem(sourceItem);

            return new PayloadCallbackResults()
            {
                removeItem = true
            };
        }

        #endregion
    }
}
