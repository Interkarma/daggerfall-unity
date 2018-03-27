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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Groups one or more effects for transport and execution.
    /// A spell is one example of an effect bundle that carries multiple effects.
    /// </summary>
    public class EntityEffectBundle
    {
        #region Fields

        EffectBundleSettings settings;
        List<IEntityEffect> effects = new List<IEntityEffect>();

        #endregion

        #region Properties

        public EffectBundleSettings DefaultSettings
        {
            get { return GetDefaultSettings(); }
        }

        public EffectBundleSettings CurrentSettings
        {
            get { return settings; }
            set { ApplySettings(value); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EntityEffectBundle()
        {
            ApplySettings(DefaultSettings);
        }

        /// <summary>
        /// Settings constructor.
        /// </summary>
        public EntityEffectBundle(EffectBundleSettings settings)
        {
            ApplySettings(settings);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Empty bundle. Clears all effects and restores default settings.
        /// </summary>
        public void Clear()
        {
            effects.Clear();
            ApplySettings(DefaultSettings);
        }

        /// <summary>
        /// Add a single effect to bundle.
        /// </summary>
        public void AddEffect(IEntityEffect effect)
        {
            effects.Add(effect);
        }

        /// <summary>
        /// Ticks bundle and updates effects.
        /// </summary>
        /// <param name="caller">The effect manage owning this bundle.</param>
        public void Tick(EntityEffectManager caller)
        {

        }

        #endregion

        #region Private Methods

        // Applies default settings when not specified
        EffectBundleSettings GetDefaultSettings()
        {
            EffectBundleSettings defaultSettings = new EffectBundleSettings();

            // Default duration is 1 second
            defaultSettings.DurationBase = 1;

            // Default chance is 100%
            defaultSettings.ChanceBase = 100;

            // Default magnitude is 1-1 + 1-1 per level
            defaultSettings.MagnitudeBaseMin = 1;
            defaultSettings.MagnitudeBonusMax = 1;
            defaultSettings.MagnitudeBonusMin = 1;
            defaultSettings.MagnitudeBonusMax = 1;
            defaultSettings.MagnitudeBonusPerLevel = 1;

            return defaultSettings;
        }

        // Change current settings
        public void ApplySettings(EffectBundleSettings settings)
        {
            this.settings = settings;
        }

        #endregion
    }
}