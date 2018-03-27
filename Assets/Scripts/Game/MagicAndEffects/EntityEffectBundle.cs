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
    /// Most effects operate on a target entity (the receiver).
    /// But may or may not have a caster entity (the sender).
    /// Actual implementation is up to each effect script, the bundle simply carries effects
    /// from sender to receiver by way of items, spell missiles, touch, area of effect, etc.
    /// and handles their execution, settings, and lifespan.
    /// Spells are an example of effect bundles that carry effects from one entity to another.
    /// </summary>
    public class EntityEffectBundle
    {
        #region Fields

        EffectBundleSettings settings;
        List<IEntityEffect> effects = new List<IEntityEffect>();
        DaggerfallEntityBehaviour casterEntityBehaviour = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets default effect bundle settings.
        /// Default is target: none, duration: 1, chance: 100%, magnitude: 1 + 1 per level.
        /// </summary>
        public EffectBundleSettings DefaultSettings
        {
            get { return GetDefaultSettings(); }
        }

        /// <summary>
        /// Gets or sets current effect bundle settings.
        /// </summary>
        public EffectBundleSettings CurrentSettings
        {
            get { return settings; }
            set { ApplySettings(value); }
        }

        /// <summary>
        /// Gets or sets caster entity (sender) of effect bundle where one is present.
        /// Example of caster entity is an enemy mage or monster.
        /// </summary>
        public DaggerfallEntityBehaviour CasterEntityBehaviour
        {
            get { return casterEntityBehaviour; }
            set { casterEntityBehaviour = value; }
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

            // No target or magic type
            defaultSettings.TargetType = TargetTypes.None;
            defaultSettings.MagicType = MagicTypes.None;

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