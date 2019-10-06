// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Basic definitions for weapon animations.
    /// Use this as a starting point.
    /// </summary>
    public static class WeaponBasics
    {
        #region Weapon Animations

        // Weapon animation speeds in frames-per-second
        public static int IdleAnimSpeed = 10;
        public static int StrikeDownAnimSpeed = 10;
        public static int StrikeDownLeftAnimSpeed = 10;
        public static int StrikeLeftAnimSpeed = 10;
        public static int StrikeRightAnimSpeed = 10;
        public static int StrikeDownRightAnimSpeed = 10;
        public static int StrikeUpAnimSpeed = 10;
        public static int WereStrikeAnimSpeed = 20;
        public static int BowAnimSpeed = 10;

        // Animations for melee - offset and aligment changes
        public static WeaponAnimation[] MeleeWeaponAnims = new WeaponAnimation[]
        {
            new WeaponAnimation() {Record = 0, NumFrames = 1, FramePerSecond = IdleAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0.15f},
            new WeaponAnimation() {Record = 1, NumFrames = 5, FramePerSecond = StrikeDownAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0f},
            new WeaponAnimation() {Record = 2, NumFrames = 5, FramePerSecond = StrikeDownLeftAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0f},
            new WeaponAnimation() {Record = 3, NumFrames = 5, FramePerSecond = StrikeLeftAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0f},
            new WeaponAnimation() {Record = 4, NumFrames = 5, FramePerSecond = StrikeRightAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0f},
            new WeaponAnimation() {Record = 5, NumFrames = 5, FramePerSecond = StrikeDownRightAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0f},
            new WeaponAnimation() {Record = 6, NumFrames = 5, FramePerSecond = StrikeUpAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0f},
        };

        // General animations for most weapons
        public static WeaponAnimation[] GeneralWeaponAnims = new WeaponAnimation[]
        {
            new WeaponAnimation() {Record = 0, NumFrames = 1, FramePerSecond = IdleAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 1, NumFrames = 5, FramePerSecond = StrikeDownAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 2, NumFrames = 5, FramePerSecond = StrikeDownLeftAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 3, NumFrames = 5, FramePerSecond = StrikeLeftAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 4, NumFrames = 5, FramePerSecond = StrikeRightAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0f},
            new WeaponAnimation() {Record = 5, NumFrames = 5, FramePerSecond = StrikeDownRightAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0f},
            new WeaponAnimation() {Record = 6, NumFrames = 5, FramePerSecond = StrikeUpAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
        };

        // Animations for dagger - offset and alignment changes
        public static WeaponAnimation[] DaggerWeaponAnims = new WeaponAnimation[]
        {
            new WeaponAnimation() {Record = 0, NumFrames = 1, FramePerSecond = IdleAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0.04f},
            new WeaponAnimation() {Record = 1, NumFrames = 5, FramePerSecond = StrikeDownAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 2, NumFrames = 5, FramePerSecond = StrikeDownLeftAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 3, NumFrames = 5, FramePerSecond = StrikeLeftAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 4, NumFrames = 5, FramePerSecond = StrikeRightAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0f},
            new WeaponAnimation() {Record = 5, NumFrames = 5, FramePerSecond = StrikeDownRightAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0f},
            new WeaponAnimation() {Record = 6, NumFrames = 5, FramePerSecond = StrikeUpAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
        };

        // Animations for staff - offset changes
        public static WeaponAnimation[] StaffWeaponAnims = new WeaponAnimation[]
        {
            new WeaponAnimation() {Record = 0, NumFrames = 1, FramePerSecond = IdleAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0.02f},
            new WeaponAnimation() {Record = 1, NumFrames = 5, FramePerSecond = StrikeDownAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 2, NumFrames = 5, FramePerSecond = StrikeDownLeftAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 3, NumFrames = 5, FramePerSecond = StrikeLeftAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0f},
            new WeaponAnimation() {Record = 4, NumFrames = 5, FramePerSecond = StrikeRightAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0f},
            new WeaponAnimation() {Record = 5, NumFrames = 5, FramePerSecond = StrikeDownRightAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0f},
            new WeaponAnimation() {Record = 6, NumFrames = 5, FramePerSecond = StrikeUpAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
        };

        // Weapon animations for magic battle-axe (some records are flipped around)
        public static WeaponAnimation[] MagicBattleAxeWeaponAnims = new WeaponAnimation[]
        {
            new WeaponAnimation() {Record = 0, NumFrames = 1, FramePerSecond = IdleAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 1, NumFrames = 5, FramePerSecond = StrikeDownAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 2, NumFrames = 5, FramePerSecond = StrikeDownLeftAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 4, NumFrames = 5, FramePerSecond = StrikeLeftAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 5, NumFrames = 5, FramePerSecond = StrikeRightAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0f},
            new WeaponAnimation() {Record = 3, NumFrames = 5, FramePerSecond = StrikeDownRightAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0f},
            new WeaponAnimation() {Record = 6, NumFrames = 5, FramePerSecond = StrikeUpAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
        };

        // Animations for bow
        public static WeaponAnimation[] BowWeaponAnims = new WeaponAnimation[]
        {
            new WeaponAnimation() {Record = 0, NumFrames = 1, FramePerSecond = IdleAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 0, NumFrames = 7, FramePerSecond = BowAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 0, NumFrames = 7, FramePerSecond = BowAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 0, NumFrames = 7, FramePerSecond = BowAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 0, NumFrames = 7, FramePerSecond = BowAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 0, NumFrames = 7, FramePerSecond = BowAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 0, NumFrames = 4, FramePerSecond = BowAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
        };

        // Animations for werecreature - alignment changes
        public static WeaponAnimation[] WerecreatureWeaponAnims = new WeaponAnimation[]
        {
            new WeaponAnimation() {Record = 0, NumFrames = 1, FramePerSecond = IdleAnimSpeed, Alignment = WeaponAlignment.Center, Offset = 0.02f},
            new WeaponAnimation() {Record = 1, NumFrames = 5, FramePerSecond = WereStrikeAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0.2f},
            new WeaponAnimation() {Record = 2, NumFrames = 5, FramePerSecond = WereStrikeAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 3, NumFrames = 5, FramePerSecond = WereStrikeAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 4, NumFrames = 5, FramePerSecond = WereStrikeAnimSpeed, Alignment = WeaponAlignment.Right, Offset = 0f},
            new WeaponAnimation() {Record = 5, NumFrames = 5, FramePerSecond = WereStrikeAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0f},
            new WeaponAnimation() {Record = 6, NumFrames = 5, FramePerSecond = WereStrikeAnimSpeed, Alignment = WeaponAlignment.Left, Offset = 0.2f},
        };

        #endregion

        #region Helpers

        public static WeaponAnimation[] GetWeaponAnims(WeaponTypes weaponType)
        {
            if (weaponType == WeaponTypes.Melee)
                return MeleeWeaponAnims;
            else if (weaponType == WeaponTypes.Dagger || weaponType == WeaponTypes.Dagger_Magic)
                return DaggerWeaponAnims;
            else if (weaponType == WeaponTypes.Staff || weaponType == WeaponTypes.Staff_Magic)
                return StaffWeaponAnims;
            else if (weaponType == WeaponTypes.Battleaxe_Magic)
                return MagicBattleAxeWeaponAnims;
            else if (weaponType == WeaponTypes.Bow)
                return BowWeaponAnims;
            else if (weaponType == WeaponTypes.Werecreature)
                return WerecreatureWeaponAnims;
            else
                return GeneralWeaponAnims;
        }

        public static string GetWeaponFilename(WeaponTypes weaponType)
        {
            switch (weaponType)
            {
                case WeaponTypes.LongBlade:
                    return "WEAPON04.CIF";
                case WeaponTypes.LongBlade_Magic:
                    return "WEAPO104.CIF";
                case WeaponTypes.Staff:
                    return "WEAPON01.CIF";
                case WeaponTypes.Staff_Magic:
                    return "WEAPO101.CIF";
                case WeaponTypes.Dagger:
                    return "WEAPON02.CIF";
                case WeaponTypes.Dagger_Magic:
                    return "WEAPO102.CIF";
                case WeaponTypes.Mace:
                    return "WEAPON05.CIF";
                case WeaponTypes.Mace_Magic:
                    return "WEAPO105.CIF";
                case WeaponTypes.Flail:
                    return "WEAPON06.CIF";
                case WeaponTypes.Flail_Magic:
                    return "WEAPO106.CIF";
                case WeaponTypes.Warhammer:
                    return "WEAPON07.CIF";
                case WeaponTypes.Warhammer_Magic:
                    return "WEAPO107.CIF";
                case WeaponTypes.Battleaxe:
                    return "WEAPON08.CIF";
                case WeaponTypes.Battleaxe_Magic:
                    return "WEAPO108.CIF";
                case WeaponTypes.Bow:
                    return "WEAPON09.CIF";
                case WeaponTypes.Melee:
                    return "WEAPON10.CIF";
                case WeaponTypes.Werecreature:
                    return "WEAPON11.CIF";
                default:
                    throw new Exception("Unknown weapon type.");
            }
        }

        public static string GetMagicAnimFilename(ElementTypes elementType)
        {
            switch (elementType)
            {
                case ElementTypes.Fire:
                    return "FIRE00C6.CIF";
                case ElementTypes.Cold:
                    return "FRST00C6.CIF";
                case ElementTypes.Poison:
                    return "POIS00C6.CIF";
                case ElementTypes.Shock:
                    return "SHOK00C6.CIF";
                case ElementTypes.Magic:
                    return "MJIC00C6.CIF";
                default:
                    throw new Exception("Unsupported element type.");
            }
        }

        #endregion
    }
}
