// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes: All additions or modifications that differ from the source code copyright (c) 2021-2022 Osorkon
//

using UnityEngine;
using System;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Daggerfall skills collection for every entity.
    /// </summary>
    [Serializable]
    public class DaggerfallSkills
    {
        #region Fields

        public const int Count = (int)DFCareer.Skills.Count;
        public const int PrimarySkillsCount = 3;
        public const int MajorSkillsCount = 3;
        public const int MinorSkillsCount = 6;

        // [OSORKON] This changes what level Miscellaneous Skills start at. They now start at skill
        // levels 1-4 rather than vanilla's 3-6. Why have a 1-100 skill range where 1 and 2 are never
        // used? That didn't make sense to me, so I lowered Miscellaneous Skill starting levels.
        const int minDefaultValue = 1;
        const int maxDefaultValue = 4;

        // Current permanent skill values
        [SerializeField] short Medical;
        [SerializeField] short Etiquette;
        [SerializeField] short Streetwise;
        [SerializeField] short Jumping;
        [SerializeField] short Orcish;
        [SerializeField] short Harpy;
        [SerializeField] short Giantish;
        [SerializeField] short Dragonish;
        [SerializeField] short Nymph;
        [SerializeField] short Daedric;
        [SerializeField] short Spriggan;
        [SerializeField] short Centaurian;
        [SerializeField] short Impish;
        [SerializeField] short Lockpicking;
        [SerializeField] short Mercantile;
        [SerializeField] short Pickpocket;
        [SerializeField] short Stealth;
        [SerializeField] short Swimming;
        [SerializeField] short Climbing;
        [SerializeField] short Backstabbing;
        [SerializeField] short Dodging;
        [SerializeField] short Running;
        [SerializeField] short Destruction;
        [SerializeField] short Restoration;
        [SerializeField] short Illusion;
        [SerializeField] short Alteration;
        [SerializeField] short Thaumaturgy;
        [SerializeField] short Mysticism;
        [SerializeField] short ShortBlade;
        [SerializeField] short LongBlade;
        [SerializeField] short HandToHand;
        [SerializeField] short Axe;
        [SerializeField] short BluntWeapon;
        [SerializeField] short Archery;
        [SerializeField] short CriticalStrike;

        // Mods are temporary changes to skill values from effects
        // Default is 0 - effects can raise/lower mod values during their lifecycle
        // This is designed so that effects are never operating on permanent skill values
        int[] mods = new int[Count];

        #endregion

        #region Constructors

        public DaggerfallSkills()
        {
            SetDefaults();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set default value to each skill.
        /// </summary>
        public void SetDefaults()
        {
            for (int i = 0; i < Count; i++)
            {
                SetPermanentSkillValue(i, (short)UnityEngine.Random.Range(minDefaultValue, maxDefaultValue + 1));
            }
            Array.Clear(mods, 0, Count);
        }

        /// <summary>
        /// Copy contents of another DaggerfallSkills into this one.
        /// Does not copy active effect mods.
        /// </summary>
        /// <param name="other">Skilla collection to copy from.</param>
        public void Copy(DaggerfallSkills other)
        {
            for (int i = 0; i < Count; i++)
            {
                SetPermanentSkillValue(i, other.GetPermanentSkillValue(i));
            }
        }

        /// <summary>
        /// Create a new copy of this stat collection.
        /// Does not copy active effect mods.
        /// </summary>
        /// <returns>New DaggerfallStats which is a copy of this DaggerfallStats.</returns>
        public DaggerfallSkills Clone()
        {
            DaggerfallSkills newSkills = new DaggerfallSkills();
            newSkills.Copy(this);

            return newSkills;
        }

        #endregion

        #region Getters

        /// <summary>
        /// Gets live skill value by enum, including effect mods.
        /// </summary>
        /// <param name="skill">Skill to get.</param>
        /// <returns>Skill value.</returns>
        public short GetLiveSkillValue(DFCareer.Skills skill)
        {
            int mod = mods[(int)skill];
            int value = GetPermanentSkillValue(skill) + mod;

            // TODO: Any other clamping or processing

            return (short)value;
        }

        /// <summary>
        /// Gets live skill value by index, including effect mods.
        /// </summary>
        /// <param name="index">Index of skill.</param>
        /// <returns>Skill value.</returns>
        public short GetLiveSkillValue(int index)
        {
            if (index < 0 || index >= Count)
                return 0;

            return GetLiveSkillValue((DFCareer.Skills)index);
        }

        /// <summary>
        /// Gets permanent skill value by index, does not include effect mods.
        /// </summary>
        /// <param name="index">Index of skill.</param>
        /// <returns>Skill value.</returns>
        public short GetPermanentSkillValue(int index)
        {
            if (index < 0 || index >= Count)
                return 0;

            return GetPermanentSkillValue((DFCareer.Skills)index);
        }

        /// <summary>
        /// Gets permanent skill value by enum, does not include effect mods.
        /// </summary>
        /// <param name="skill">Skill to get.</param>
        /// <returns>Skill value.</returns>
        public short GetPermanentSkillValue(DFCareer.Skills skill)
        {
            switch (skill)
            {
                case DFCareer.Skills.Medical:
                    return Medical;
                case DFCareer.Skills.Etiquette:
                    return Etiquette;
                case DFCareer.Skills.Streetwise:
                    return Streetwise;
                case DFCareer.Skills.Jumping:
                    return Jumping;
                case DFCareer.Skills.Orcish:
                    return Orcish;
                case DFCareer.Skills.Harpy:
                    return Harpy;
                case DFCareer.Skills.Giantish:
                    return Giantish;
                case DFCareer.Skills.Dragonish:
                    return Dragonish;
                case DFCareer.Skills.Nymph:
                    return Nymph;
                case DFCareer.Skills.Daedric:
                    return Daedric;
                case DFCareer.Skills.Spriggan:
                    return Spriggan;
                case DFCareer.Skills.Centaurian:
                    return Centaurian;
                case DFCareer.Skills.Impish:
                    return Impish;
                case DFCareer.Skills.Lockpicking:
                    return Lockpicking;
                case DFCareer.Skills.Mercantile:
                    return Mercantile;
                case DFCareer.Skills.Pickpocket:
                    return Pickpocket;
                case DFCareer.Skills.Stealth:
                    return Stealth;
                case DFCareer.Skills.Swimming:
                    return Swimming;
                case DFCareer.Skills.Climbing:
                    return Climbing;
                case DFCareer.Skills.Backstabbing:
                    return Backstabbing;
                case DFCareer.Skills.Dodging:
                    return Dodging;
                case DFCareer.Skills.Running:
                    return Running;
                case DFCareer.Skills.Destruction:
                    return Destruction;
                case DFCareer.Skills.Restoration:
                    return Restoration;
                case DFCareer.Skills.Illusion:
                    return Illusion;
                case DFCareer.Skills.Alteration:
                    return Alteration;
                case DFCareer.Skills.Thaumaturgy:
                    return Thaumaturgy;
                case DFCareer.Skills.Mysticism:
                    return Mysticism;
                case DFCareer.Skills.ShortBlade:
                    return ShortBlade;
                case DFCareer.Skills.LongBlade:
                    return LongBlade;
                case DFCareer.Skills.HandToHand:
                    return HandToHand;
                case DFCareer.Skills.Axe:
                    return Axe;
                case DFCareer.Skills.BluntWeapon:
                    return BluntWeapon;
                case DFCareer.Skills.Archery:
                    return Archery;
                case DFCareer.Skills.CriticalStrike:
                    return CriticalStrike;
                default:
                    return 0;
            }
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets permanent skill value by enum, does not change effect mods.
        /// </summary>
        /// <param name="skill">Skill to set.</param>
        /// <param name="value">Skill value.</param>
        public void SetPermanentSkillValue(DFCareer.Skills skill, short value)
        {
            switch (skill)
            {
                case DFCareer.Skills.Medical:
                    Medical = value;
                    break;
                case DFCareer.Skills.Etiquette:
                    Etiquette = value;
                    break;
                case DFCareer.Skills.Streetwise:
                    Streetwise = value;
                    break;
                case DFCareer.Skills.Jumping:
                    Jumping = value;
                    break;
                case DFCareer.Skills.Orcish:
                    Orcish = value;
                    break;
                case DFCareer.Skills.Harpy:
                    Harpy = value;
                    break;
                case DFCareer.Skills.Giantish:
                    Giantish = value;
                    break;
                case DFCareer.Skills.Dragonish:
                    Dragonish = value;
                    break;
                case DFCareer.Skills.Nymph:
                    Nymph = value;
                    break;
                case DFCareer.Skills.Daedric:
                    Daedric = value;
                    break;
                case DFCareer.Skills.Spriggan:
                    Spriggan = value;
                    break;
                case DFCareer.Skills.Centaurian:
                    Centaurian = value;
                    break;
                case DFCareer.Skills.Impish:
                    Impish = value;
                    break;
                case DFCareer.Skills.Lockpicking:
                    Lockpicking = value;
                    break;
                case DFCareer.Skills.Mercantile:
                    Mercantile = value;
                    break;
                case DFCareer.Skills.Pickpocket:
                    Pickpocket = value;
                    break;
                case DFCareer.Skills.Stealth:
                    Stealth = value;
                    break;
                case DFCareer.Skills.Swimming:
                    Swimming = value;
                    break;
                case DFCareer.Skills.Climbing:
                    Climbing = value;
                    break;
                case DFCareer.Skills.Backstabbing:
                    Backstabbing = value;
                    break;
                case DFCareer.Skills.Dodging:
                    Dodging = value;
                    break;
                case DFCareer.Skills.Running:
                    Running = value;
                    break;
                case DFCareer.Skills.Destruction:
                    Destruction = value;
                    break;
                case DFCareer.Skills.Restoration:
                    Restoration = value;
                    break;
                case DFCareer.Skills.Illusion:
                    Illusion = value;
                    break;
                case DFCareer.Skills.Alteration:
                    Alteration = value;
                    break;
                case DFCareer.Skills.Thaumaturgy:
                    Thaumaturgy = value;
                    break;
                case DFCareer.Skills.Mysticism:
                    Mysticism = value;
                    break;
                case DFCareer.Skills.ShortBlade:
                    ShortBlade = value;
                    break;
                case DFCareer.Skills.LongBlade:
                    LongBlade = value;
                    break;
                case DFCareer.Skills.HandToHand:
                    HandToHand = value;
                    break;
                case DFCareer.Skills.Axe:
                    Axe = value;
                    break;
                case DFCareer.Skills.BluntWeapon:
                    BluntWeapon = value;
                    break;
                case DFCareer.Skills.Archery:
                    Archery = value;
                    break;
                case DFCareer.Skills.CriticalStrike:
                    CriticalStrike = value;
                    break;
            }
        }

        /// <summary>
        /// Sets permanent skill value by index, does not change effect mods.
        /// </summary>
        /// <param name="index">Index of skill.</param>
        /// <param name="value">Skill value.</param>
        public void SetPermanentSkillValue(int index, short value)
        {
            if (index < 0 || index >= Count)
                return;

            SetPermanentSkillValue((DFCareer.Skills)index, value);
        }

        /// <summary>
        /// Assign mods from effect manager.
        /// </summary>
        public void AssignMods(int[] skillMods)
        {
            Array.Copy(skillMods, mods, Count);
        }

        #endregion

        #region Static Methods

        public static DFCareer.Stats GetPrimaryStat(DFCareer.Skills skill)
        {
            switch (skill)
            {
                case DFCareer.Skills.Medical:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Etiquette:
                    return DFCareer.Stats.Personality;
                case DFCareer.Skills.Streetwise:
                    return DFCareer.Stats.Personality;
                case DFCareer.Skills.Jumping:
                    return DFCareer.Stats.Strength;
                case DFCareer.Skills.Orcish:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Harpy:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Giantish:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Dragonish:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Nymph:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Daedric:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Spriggan:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Centaurian:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Impish:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Lockpicking:
                    return DFCareer.Stats.Intelligence;
                case DFCareer.Skills.Mercantile:
                    return DFCareer.Stats.Personality;
                case DFCareer.Skills.Pickpocket:
                    return DFCareer.Stats.Agility;
                case DFCareer.Skills.Stealth:
                    return DFCareer.Stats.Agility;
                case DFCareer.Skills.Swimming:
                    return DFCareer.Stats.Endurance;
                case DFCareer.Skills.Climbing:
                    return DFCareer.Stats.Strength;
                case DFCareer.Skills.Backstabbing:
                    return DFCareer.Stats.Agility;
                case DFCareer.Skills.Dodging:
                    return DFCareer.Stats.Speed;
                case DFCareer.Skills.Running:
                    return DFCareer.Stats.Speed;
                case DFCareer.Skills.Destruction:
                    return DFCareer.Stats.Willpower;
                case DFCareer.Skills.Restoration:
                    return DFCareer.Stats.Willpower;
                case DFCareer.Skills.Illusion:
                    return DFCareer.Stats.Willpower;
                case DFCareer.Skills.Alteration:
                    return DFCareer.Stats.Willpower;
                case DFCareer.Skills.Thaumaturgy:
                    return DFCareer.Stats.Willpower;
                case DFCareer.Skills.Mysticism:
                    return DFCareer.Stats.Willpower;
                case DFCareer.Skills.ShortBlade:
                    return DFCareer.Stats.Agility;
                case DFCareer.Skills.LongBlade:
                    return DFCareer.Stats.Agility;
                case DFCareer.Skills.HandToHand:
                    return DFCareer.Stats.Agility;
                case DFCareer.Skills.Axe:
                    return DFCareer.Stats.Strength;
                case DFCareer.Skills.BluntWeapon:
                    return DFCareer.Stats.Strength;
                case DFCareer.Skills.Archery:
                    return DFCareer.Stats.Agility;
                case DFCareer.Skills.CriticalStrike:
                    return DFCareer.Stats.Agility;
                default:
                    return (DFCareer.Stats)(-1);
            }
        }

        public static DFCareer.Stats GetPrimaryStat(int index)
        {
            return GetPrimaryStat((DFCareer.Skills)index);
        }

        public static int GetAdvancementMultiplier(DFCareer.Skills skill)
        {
            // [OSORKON] Most skills in BOSSFALL are much harder to level, as I favor longer playthroughs
            // and I thought most skills (especially weapon and spell skills) were much too easy to raise.
            // I doubt most people want to play with Skill Difficulty = Extreme, especially if they don't
            // have a lot of time, so at the moment I don't have very high hopes for BOSSFALL's popularity.
            // This is, however, the way I prefer to play, and for now that's the way it will stay. I did
            // counterbalance the increased skill difficulty by increasing the Training cap to 95, though.
            switch (skill)
            {
                // [OSORKON] Raised Medical from vanilla's 12. I don't know if this change is necessary.
                // I'll have to test a longer playthrough and see if Medical levels too slowly.
                case DFCareer.Skills.Medical:
                    return 16;
                case DFCareer.Skills.Etiquette:
                case DFCareer.Skills.Streetwise:
                    return 1;

                // [OSORKON] Greatly raised Jumping from vanilla's 5. Unfortunately the fastest way
                // to level this skill is jumping in a doorway with the "Jumping" spell active. I'll
                // never try to nerf that particular behavior, so to compensate for this skill's
                // cheesy training method I made the skill extremely hard to level.
                case DFCareer.Skills.Jumping:
                    return 20;

                // [OSORKON] Greatly lowered Orcish from vanilla's 15. Orcs appear quite frequently
                // compared to the other monsters with languages so Orcish is 2 rather than 1.
                case DFCareer.Skills.Orcish:
                    return 2;

                // [OSORKON] Greatly lowered monster language difficulty from vanilla's 15. I play "linguist"
                // characters so my weapon and spell skills start off very low. I do this to make the
                // game as hard as possible. Unfortunately, monster language skills were impossible to level
                // past vanilla's level 50 skill training cap, which was a hard limit to my character's
                // progression. I fixed that obstacle by greatly reducing monster language skill difficulty.
                case DFCareer.Skills.Harpy:
                case DFCareer.Skills.Giantish:
                case DFCareer.Skills.Dragonish:
                case DFCareer.Skills.Nymph:
                case DFCareer.Skills.Daedric:
                case DFCareer.Skills.Spriggan:
                case DFCareer.Skills.Centaurian:
                case DFCareer.Skills.Impish:
                    return 1;
                case DFCareer.Skills.Lockpicking:
                    return 2;

                // [OSORKON] Greatly increased Mercantile from vanilla's 1. I play with "Climates & Calories"
                // and that mod requires a lot of little purchases, which raises this skill very quickly.
                // Because it's so easy to train, I made it much harder to level.
                case DFCareer.Skills.Mercantile:
                    return 9;
                case DFCareer.Skills.Pickpocket:
                    return 2;

                // [OSORKON] This skill will skyrocket in vanilla whenever player is around enemies (basically
                // all the time). Because it's so easy to train, I made it much harder to level. With the
                // excellent "Skulduggery" mod, Stealth becomes extremely hard to level. I don't know if I've
                // found the right balance for the Stealth skill yet.
                case DFCareer.Skills.Stealth:
                    return 12;

                // [OSORKON] Doubled from vanilla's 1. Not sure if this change is necessary, as there's no
                // cheesy way to level this skill. I may revert this change later.
                case DFCareer.Skills.Swimming:
                    return 2;

                // [OSORKON] Greatly increased from vanilla's 2. Now that I think about it, I may have gone
                // way overboard, as I removed cheesy ways to level the skill - Rappel mode no longer gives
                // Climbing skill tallies. Without that easy method of leveling Climbing, 20 may be too high.
                case DFCareer.Skills.Climbing:
                    return 20;

                // [OSORKON] Tripled from vanilla's 1. This skill is hard to exercise as Backstabbing is
                // quite difficult to do (unless player has a long-lasting Invisibility True spell). I may
                // revert this change later.
                case DFCareer.Skills.Backstabbing:
                    return 3;

                // [OSORKON] Greatly increased from vanilla's 4. It was even higher in BOSSFALL v1.1 but
                // I'm still not sure it's where I want. I'll have to test Dodging with a longer
                // playthrough and see if it levels too slowly.
                case DFCareer.Skills.Dodging:
                    return 24;

                // [OSORKON] In BOSSFALL v1.1 I set Running to 400. This had disastrous results as there is a
                // skill tally limit of 20,000 set in another script. That same skill tally limit is defined as
                // a "short" (a 16-bit integer) which means its maximum value is 32,767. I tried raising the
                // skill tally cap to 32,767 but that still wasn't enough for Running to level up at higher
                // Running skill levels - I effectively broke the skill. Fortunately, I discovered another method
                // to make Running progression more difficult. Thus, Running difficulty in this script is unchanged
                // from vanilla, but it will now level up roughly 7 times slower.
                case DFCareer.Skills.Running:
                    return 50;

                // [OSORKON] All spells greatly increased from vanilla's 1 and 2. Thaumaturgy and Restoration
                // no longer level slower than the rest. 8 may be too high - I'll revisit skill difficulty in
                // a later version of BOSSFALL.
                case DFCareer.Skills.Destruction:
                case DFCareer.Skills.Restoration:
                case DFCareer.Skills.Illusion:
                case DFCareer.Skills.Alteration:
                case DFCareer.Skills.Thaumaturgy:
                case DFCareer.Skills.Mysticism:
                    return 8;

                // [OSORKON] All weapons greatly increased from vanilla's 1 and 2. Archery no longer levels
                // faster than the rest. I have a feeling 12 is too high, but I haven't taken the time to do
                // a longer playthrough to find out for sure.
                case DFCareer.Skills.ShortBlade:
                case DFCareer.Skills.LongBlade:
                case DFCareer.Skills.HandToHand:
                case DFCareer.Skills.Axe:
                case DFCareer.Skills.BluntWeapon:
                case DFCareer.Skills.Archery:
                    return 12;

                // [OSORKON] Greatly increased from vanilla's 8. I'm pretty sure I set this way too high, and
                // I'll likely lower it at a later date. 
                case DFCareer.Skills.CriticalStrike:
                    return 48;
                default:
                    return 0;
            }
        }

        public static bool IsLanguageSkill(DFCareer.Skills skill)
        {
            switch (skill)
            {
                case DFCareer.Skills.Orcish:
                case DFCareer.Skills.Harpy:
                case DFCareer.Skills.Giantish:
                case DFCareer.Skills.Dragonish:
                case DFCareer.Skills.Nymph:
                case DFCareer.Skills.Daedric:
                case DFCareer.Skills.Spriggan:
                case DFCareer.Skills.Centaurian:
                case DFCareer.Skills.Impish:
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }
}
