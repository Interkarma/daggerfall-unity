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

using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Stores effect settings for transport to an entity.
    /// </summary>
    public class EntityEffectBundle
    {
        #region Fields

        EffectBundleSettings settings;
        DaggerfallEntityBehaviour casterEntityBehaviour = null;
        DaggerfallUnityItem fromEquippedItem = null;
        DaggerfallUnityItem castByItem = null;
        int reflectedCount = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets caster entity (sender) of effect bundle where one is present.
        /// Example of caster entity is an enemy mage or monster.
        /// </summary>
        public DaggerfallEntityBehaviour CasterEntityBehaviour
        {
            get { return casterEntityBehaviour; }
            set { casterEntityBehaviour = value; }
        }

        /// <summary>
        /// Gets or sets effect bundle settings.
        /// </summary>
        public EffectBundleSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        /// <summary>
        /// True if bundle is from an equipped source item.
        /// These bundles will last until player unequips or item breaks (which also unequips item).
        /// </summary>
        public bool IsFromEquippedItem
        {
            get { return (fromEquippedItem != null); }
        }

        /// <summary>
        /// Gets or sets the equipped item providing this bundle.
        /// </summary>
        public DaggerfallUnityItem FromEquippedItem
        {
            get { return fromEquippedItem; }
            set { fromEquippedItem = value; }
        }

        public DaggerfallUnityItem CastByItem
        {
            get { return castByItem; }
            set { castByItem = value; }
        }

        /// <summary>
        /// Gets the number of times this bundle has been reflected.
        /// </summary>
        public int ReflectedCount
        {
            get { return reflectedCount; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EntityEffectBundle()
        {
        }

        /// <summary>
        /// Settings + caster constructor.
        /// </summary>
        /// <param name="settings">Settings of this effect bundle.</param>
        /// <param name="casterEntityBehaviour">Caster of this effect bundle (optional).</param>
        public EntityEffectBundle(EffectBundleSettings settings, DaggerfallEntityBehaviour casterEntityBehaviour = null)
        {
            this.settings = settings;
            this.casterEntityBehaviour = casterEntityBehaviour;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if effect bundle contains an effect matching a classic effect record.
        /// </summary>
        /// <param name="effectRecord">Effect record to compare with native bundle effects.</param>
        /// <returns>True if bundle contains effect matching classic effect record.</returns>
        public bool HasMatchForClassicEffect(SpellRecord.EffectRecordData effectRecord)
        {
            int classicKey = BaseEntityEffect.MakeClassicKey((byte)effectRecord.type, (byte)effectRecord.subType);
            foreach(EffectEntry entry in settings.Effects)
            {
                IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(entry.Key);
                if (effectTemplate.Properties.ClassicKey == classicKey)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Increments reflection count.
        /// </summary>
        public void IncrementReflectionCount()
        {
            reflectedCount++;
        }

        public void AddRuntimeFlags(BundleRuntimeFlags flags)
        {
            settings.RuntimeFlags |= flags;
        }

        public void RemoveRuntimeFlags(BundleRuntimeFlags flags)
        {
            settings.RuntimeFlags &= ~flags;
        }

        #endregion
    }
}
