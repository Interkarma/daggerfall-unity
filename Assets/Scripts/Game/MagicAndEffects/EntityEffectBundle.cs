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

using DaggerfallWorkshop.Game.Entity;

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
    }
}