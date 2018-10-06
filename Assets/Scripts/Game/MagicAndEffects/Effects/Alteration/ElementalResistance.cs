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

using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// ElementalResistance Fire/Frost/Poison/Shock/Magic multi-effect
    /// </summary>
    public class ElementalResistance : IncumbentEffect
    {
        const int totalVariants = 5;
        const string textDatabase = "ClassicEffects";
        string[] subGroupTextKeys = new string[] { "fire", "frost", "poison", "shock", "magicka" };

        VariantProperties[] variantProperties = new VariantProperties[totalVariants];

        struct VariantProperties
        {
            public ElementTypes elementResisted;
            public EffectProperties effectProperties;
        }

        public override EffectProperties Properties
        {
            get { return variantProperties[currentVariant].effectProperties; }
        }

        public ElementTypes ElementResisted
        {
            get { return variantProperties[currentVariant].elementResisted; }
        }

        public override void SetProperties()
        {
            // Set properties shared by all variants
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "elementalResistance");
            properties.SupportDuration = true;
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Alteration;
            properties.DurationCosts = MakeEffectCosts(100, 100);
            properties.ChanceCosts = MakeEffectCosts(8, 100);

            // Set unique variant properties
            variantCount = totalVariants;
            SetVariantProperties(ElementTypes.Fire, 0);
            SetVariantProperties(ElementTypes.Cold, 1);
            SetVariantProperties(ElementTypes.Poison, 2);
            SetVariantProperties(ElementTypes.Shock, 3);
            SetVariantProperties(ElementTypes.Magic, 4);
        }

        void SetVariantProperties(ElementTypes element, int variantIndex)
        {
            string name = TextManager.Instance.GetText("ClassicEffects", subGroupTextKeys[variantIndex]);

            VariantProperties vp = new VariantProperties();
            vp.effectProperties = properties;
            vp.effectProperties.Key = string.Format("ElementalResistance-{0}", name);
            vp.effectProperties.ClassicKey = MakeClassicKey(8, (byte)variantIndex);
            vp.effectProperties.SubGroupName = name;
            vp.effectProperties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1527 + variantIndex);
            vp.effectProperties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1227 + variantIndex);
            vp.elementResisted = element;
            variantProperties[variantIndex] = vp;
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is ElementalResistance && (other as ElementalResistance).ElementResisted == ElementResisted) ? true : false;
        }

        protected override void BecomeIncumbent()
        {
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }
    }
}