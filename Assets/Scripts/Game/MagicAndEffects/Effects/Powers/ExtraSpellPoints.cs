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

using System;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Extra Spell Points
    /// </summary>
    public class ExtraSpellPoints : BaseEntityEffect
    {
        const string keyLeft = "ExtraSpellPoints-";
        const int groupIndex = 3;
        const int totalVariants = 11;
        const ClassicEffectFamily family = ClassicEffectFamily.PowersAndSideEffects;
        EffectProperties[] variantProperties = new EffectProperties[totalVariants];

        public override EffectProperties Properties
        {
            get { return variantProperties[currentVariant]; }
        }

        public override void SetProperties()
        {
            variantCount = totalVariants;
            SetVariantProperties(0, "DuringWinter");
            SetVariantProperties(1, "DuringSpring");
            SetVariantProperties(2, "DuringSummer");
            SetVariantProperties(3, "DuringFall");
            SetVariantProperties(4, "DuringFullMoon");
            SetVariantProperties(5, "DuringHalfMoon");
            SetVariantProperties(6, "DuringNewMoon");
            SetVariantProperties(7, "NearUndead");
            SetVariantProperties(8, "NearDaedra");
            SetVariantProperties(9, "NearHumanoids");
            SetVariantProperties(10, "NearAnimals");
        }

        void SetVariantProperties(int variant, string keyRight)
        {
            EffectProperties props = properties;
            props.Key = keyLeft+keyRight;
            props.ClassicKey = MakeClassicKey(groupIndex, (byte)variant, family);
            props.GroupName = HardStrings.itemPowers[groupIndex];
            props.SubGroupName = HardStrings.extraSpellPtsTimes[variant];
            props.DisplayName = string.Format("{0} ({1})", props.GroupName, props.SubGroupName);
            props.AllowedCraftingStations = MagicCraftingStations.ItemMaker;

            variantProperties[variant] = props;
        }
    }
}
