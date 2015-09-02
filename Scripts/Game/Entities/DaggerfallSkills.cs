// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Daggerfall skills collection for every entity.
    /// </summary>
    [Serializable]
    public struct DaggerfallSkills
    {
        public const int Count = 35;
        public const int PrimarySkillsCount = 3;
        public const int MajorSkillsCount = 3;
        public const int MinorSkillsCount = 6;

        const int minDefaultValue = 3;
        const int maxDefaultValue = 6;

        public short Medical;
        public short Etiquette;
        public short Streetwise;
        public short Jumping;
        public short Orcish;
        public short Harpy;
        public short Giantish;
        public short Dragonish;
        public short Nymph;
        public short Daedric;
        public short Spriggan;
        public short Centaurian;
        public short Impish;
        public short Lockpicking;
        public short Mercantile;
        public short Pickpocket;
        public short Stealth;
        public short Swimming;
        public short Climbing;
        public short Backstabbing;
        public short Dodging;
        public short Running;
        public short Destruction;
        public short Restoration;
        public short Illusion;
        public short Alteration;
        public short Thaumaturgy;
        public short Mysticism;
        public short ShortBlade;
        public short LongBlade;
        public short HandToHand;
        public short Axe;
        public short BluntWeapon;
        public short Archery;
        public short CriticalStrike;

        public void SetDefaults()
        {
            for (int i = 0; i < Count; i++)
            {
                SetSkillValue(i, (short)UnityEngine.Random.Range(minDefaultValue, maxDefaultValue + 1));
            }
        }

        public void Copy(DaggerfallSkills other)
        {
            for (int i = 0; i < Count; i++)
            {
                SetSkillValue(i, other.GetSkillValue(i));
            }
        }

        public short GetSkillValue(DFClass.Skills skill)
        {
            switch (skill)
            {
                case DFClass.Skills.Medical:
                    return Medical;
                case DFClass.Skills.Etiquette:
                    return Etiquette;
                case DFClass.Skills.Streetwise:
                    return Streetwise;
                case DFClass.Skills.Jumping:
                    return Jumping;
                case DFClass.Skills.Orcish:
                    return Orcish;
                case DFClass.Skills.Harpy:
                    return Harpy;
                case DFClass.Skills.Giantish:
                    return Giantish;
                case DFClass.Skills.Dragonish:
                    return Dragonish;
                case DFClass.Skills.Nymph:
                    return Nymph;
                case DFClass.Skills.Daedric:
                    return Daedric;
                case DFClass.Skills.Spriggan:
                    return Spriggan;
                case DFClass.Skills.Centaurian:
                    return Centaurian;
                case DFClass.Skills.Impish:
                    return Impish;
                case DFClass.Skills.Lockpicking:
                    return Lockpicking;
                case DFClass.Skills.Mercantile:
                    return Mercantile;
                case DFClass.Skills.Pickpocket:
                    return Pickpocket;
                case DFClass.Skills.Stealth:
                    return Stealth;
                case DFClass.Skills.Swimming:
                    return Swimming;
                case DFClass.Skills.Climbing:
                    return Climbing;
                case DFClass.Skills.Backstabbing:
                    return Backstabbing;
                case DFClass.Skills.Dodging:
                    return Dodging;
                case DFClass.Skills.Running:
                    return Running;
                case DFClass.Skills.Destruction:
                    return Destruction;
                case DFClass.Skills.Restoration:
                    return Restoration;
                case DFClass.Skills.Illusion:
                    return Illusion;
                case DFClass.Skills.Alteration:
                    return Alteration;
                case DFClass.Skills.Thaumaturgy:
                    return Thaumaturgy;
                case DFClass.Skills.Mysticism:
                    return Mysticism;
                case DFClass.Skills.ShortBlade:
                    return ShortBlade;
                case DFClass.Skills.LongBlade:
                    return LongBlade;
                case DFClass.Skills.HandToHand:
                    return HandToHand;
                case DFClass.Skills.Axe:
                    return Axe;
                case DFClass.Skills.BluntWeapon:
                    return BluntWeapon;
                case DFClass.Skills.Archery:
                    return Archery;
                case DFClass.Skills.CriticalStrike:
                    return CriticalStrike;
                default:
                    return 0;
            }
        }

        public void SetSkillValue(DFClass.Skills skill, short value)
        {
            switch (skill)
            {
                case DFClass.Skills.Medical:
                    Medical = value;
                    break;
                case DFClass.Skills.Etiquette:
                    Etiquette = value;
                    break;
                case DFClass.Skills.Streetwise:
                    Streetwise = value;
                    break;
                case DFClass.Skills.Jumping:
                    Jumping = value;
                    break;
                case DFClass.Skills.Orcish:
                    Orcish = value;
                    break;
                case DFClass.Skills.Harpy:
                    Harpy = value;
                    break;
                case DFClass.Skills.Giantish:
                    Giantish = value;
                    break;
                case DFClass.Skills.Dragonish:
                    Dragonish = value;
                    break;
                case DFClass.Skills.Nymph:
                    Nymph = value;
                    break;
                case DFClass.Skills.Daedric:
                    Daedric = value;
                    break;
                case DFClass.Skills.Spriggan:
                    Spriggan = value;
                    break;
                case DFClass.Skills.Centaurian:
                    Centaurian = value;
                    break;
                case DFClass.Skills.Impish:
                    Impish = value;
                    break;
                case DFClass.Skills.Lockpicking:
                    Lockpicking = value;
                    break;
                case DFClass.Skills.Mercantile:
                    Mercantile = value;
                    break;
                case DFClass.Skills.Pickpocket:
                    Pickpocket = value;
                    break;
                case DFClass.Skills.Stealth:
                    Stealth = value;
                    break;
                case DFClass.Skills.Swimming:
                    Swimming = value;
                    break;
                case DFClass.Skills.Climbing:
                    Climbing = value;
                    break;
                case DFClass.Skills.Backstabbing:
                    Backstabbing = value;
                    break;
                case DFClass.Skills.Dodging:
                    Dodging = value;
                    break;
                case DFClass.Skills.Running:
                    Running = value;
                    break;
                case DFClass.Skills.Destruction:
                    Destruction = value;
                    break;
                case DFClass.Skills.Restoration:
                    Restoration = value;
                    break;
                case DFClass.Skills.Illusion:
                    Illusion = value;
                    break;
                case DFClass.Skills.Alteration:
                    Alteration = value;
                    break;
                case DFClass.Skills.Thaumaturgy:
                    Thaumaturgy = value;
                    break;
                case DFClass.Skills.Mysticism:
                    Mysticism = value;
                    break;
                case DFClass.Skills.ShortBlade:
                    ShortBlade = value;
                    break;
                case DFClass.Skills.LongBlade:
                    LongBlade = value;
                    break;
                case DFClass.Skills.HandToHand:
                    HandToHand = value;
                    break;
                case DFClass.Skills.Axe:
                    Axe = value;
                    break;
                case DFClass.Skills.BluntWeapon:
                    BluntWeapon = value;
                    break;
                case DFClass.Skills.Archery:
                    Archery = value;
                    break;
                case DFClass.Skills.CriticalStrike:
                    CriticalStrike = value;
                    break;
            }
        }

        public short GetSkillValue(int index)
        {
            return GetSkillValue((DFClass.Skills)index);
        }

        public void SetSkillValue(int index, short value)
        {
            SetSkillValue((DFClass.Skills)index, value);
        }
    }
}