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

using System;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// How an effect bundle targets entities in world space.
    /// Can be used as flags by effect system to declare supported targets.
    /// </summary>
    [Flags]
    public enum TargetTypes
    {
        None = 0,
        CasterOnly = 1,
        ByTouch = 2,
        SingleTargetAtRange = 4,
        AreaAroundCaster = 8,
        AreaAtRange = 16,
    }

    /// <summary>
    /// How effect bundle manifests for cast animations, billboard effects, resist checks, etc.
    /// Can be used as flags by effect system to declare supported elements.
    /// </summary>
    [Flags]
    public enum ElementTypes
    {
        None = 0,
        Fire = 1,
        Cold = 2,
        Poison = 4,
        Shock = 8,
        Magic = 16,
    }

    /// <summary>
    /// Flags defining which magic crafting stations can serve this effect.
    /// What constitutes a magic crafting station is likely to expand over time.
    /// For example, custom character creator is potentially a type of crafting station for advantages/disadvantages.
    /// Just leaving as main three stations (spellmaker, potionmaker, itemmaker) for now.
    /// </summary>
    [Flags]
    public enum MagicCraftingStations
    {
        None = 0,
        SpellMaker = 1,
        PotionMaker = 2,
        ItemMaker = 4,
    }

    /// <summary>
    /// Defines flags for additional feature support at item maker.
    /// </summary>
    [Flags]
    public enum ItemMakerFlags
    {
        None = 0,
        AllowMultiplePrimaryInstances = 1,
        AllowMultipleSecondaryInstances = 2,
        AlphaSortSecondaryList = 4,
        WeaponOnly = 8,
    }

    /// <summary>
    /// Supported bundle types.
    /// This helps determine lifetime and usage of a bundle.
    /// </summary>
    public enum BundleTypes
    {
        None,
        Spell,
        Disease,
        Poison,
        HeldMagicItem,
        Potion,
    }

    /// <summary>
    /// Special bundle flags to control additional runtime behaviours.
    /// </summary>
    [Flags]
    public enum BundleRuntimeFlags
    {
        /// <summary>No flags.</summary>
        None = 0,
        /// <summary>If bundle is from a held magic item, it will be removed at recast time and recast callbacks executed on item.</summary>
        ItemRecastEnabled = 1,
    }

    /// <summary>
    /// Determines how effect chance will function.
    /// OnCast: is checked at cast time by EntityEffectManager receiving effect - effect is rejected on failure.
    /// Custom: is always allowed by EntityEffectManager, but still generates ChanceSuccess flag on Start().
    /// This allows custom chance handling elsewhere by either by the effect itself or elsewhere in effect back-end.
    /// A Custom chance effect can then decide to check for ChanceSuccess at any time, even do something else entirely.
    /// </summary>
    public enum ChanceFunction
    {
        OnCast,
        Custom,
    }

    /// <summary>
    /// Diseases the player can catch. (msgs 100-116)
    /// </summary>
    public enum Diseases
    {
        None = -1,
        WitchesPox = 0,
        Plague = 1,
        YellowFever = 2,
        StomachRot = 3,
        Consumption = 4,
        BrainFever = 5,
        SwampRot = 6,
        CalironsCurse = 7,
        Cholera = 8,
        Leprosy = 9,
        WoundRot = 10,
        RedDeath = 11,
        BloodRot = 12,
        TyphoidFever = 13,
        Dementia = 14,
        Chrondiasis = 15,
        WizardFever = 16,
    }

    /// <summary>
    /// Flags to modify behaviour of assigning effect bundles.
    /// </summary>
    [Flags]
    public enum AssignBundleFlags
    {
        None = 0,
        ShowNonPlayerFailures = 1,
        BypassSavingThrows = 2,
        SpecialInfection = 4,
        BypassChance = 8,
    }

    /// <summary>
    /// Flags to inform magic framework when enchantment effect should receive callbacks to execute its payload.
    /// As Daggerfall Unity supports custom effects there is more granularity to payload execution than classic.
    /// Note these are distinct from "cast when used", "cast when held", etc. Rather, the CastWhenUsed and CastWhenHeld
    /// effects will deliver their payload from callbacks related to these flags.
    /// </summary>
    [Flags]
    public enum EnchantmentPayloadFlags
    {
        None = 0,
        Enchanted = 1,      // Payload executed only once when item is enchanted at item maker
        Used = 2,           // Payload executed when item is used from inventory or "use item" UI
        Equipped = 4,       // Payload executed when item is equipped - i.e. payload will execute once every time item is equipped
        Unequipped = 8,     // Payload executed when item is unequipped - i.e. payload will execute once every time item is unequipped
        Held = 16,          // Payload executed for duration item is equipped - i.e. effect bundle will be persistently attached to entity until unequipped
        Strikes = 32,       // Payload executed when a weapon item strikes another entity
        Breaks = 64,        // Payload executed when item breaks after durability reaches zero or less
        MagicRound = 128,   // Payload executed once per magic round
        RerollEffect = 256, // Payload executed when effects are recast on item
    }
}
