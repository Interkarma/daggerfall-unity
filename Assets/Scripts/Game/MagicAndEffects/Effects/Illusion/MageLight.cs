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

using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Creates a soft glow around player based on variant of effect.
    /// This is a primarily a demo class for creating a custom effect.
    /// Effect is intended to be a simple example without being too trivial.
    /// Does not require any serialization. See other effects for examples of how to save effect state.
    /// </summary>
    public class MageLight : IncumbentEffect
    {
        #region Fields

        const int totalVariants = 5;
        string effectKey = "MageLight";
        string[] subGroupKeys = { "Inferno", "Rime", "Venom", "Storm", "Arcane" };
        Color32[] effectColors = {
            new Color32(154, 24, 8, 255),       // Inferno
            new Color32(158, 202, 202, 255),    // Rime
            new Color32(101, 160, 60, 255),     // Venom
            new Color32(68, 124, 192, 255),     // Storm
            new Color32(101, 137, 120, 255)     // Arcane
        };

        Light myLight = null;
        VariantProperties[] variantProperties = new VariantProperties[totalVariants];

        #endregion

        #region Structs & Enums

        // Variant can be stored internally with any format
        // Using a struct here with properties for to effect
        struct VariantProperties
        {
            public Color32 effectColor;
            public EffectProperties effectProperties;
        }

        // A friendly name for each variant - could also just reference by index 0-4
        enum VariantTypes
        {
            Inferno,
            Rime,
            Venom,
            Storm,
            Arcane,
        }

        #endregion

        #region Properties

        // Must override Properties to return correct properties for any variant
        // The currentVariant value is set by magic framework - each variant gets enumerated to its own effect template
        public override EffectProperties Properties
        {
            get { return variantProperties[currentVariant].effectProperties; }
        }

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            // Set properties shared by all variants
            properties.Key = effectKey;
            properties.SupportDuration = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
            properties.DurationCosts = MakeEffectCosts(8, 40);
            properties.DisableReflectiveEnumeration = true;

            // Set variant count so framework knows how many to extract
            variantCount = totalVariants;

            // Set properties unique to each variant
            SetVariantProperties(VariantTypes.Inferno);
            SetVariantProperties(VariantTypes.Rime);
            SetVariantProperties(VariantTypes.Venom);
            SetVariantProperties(VariantTypes.Storm);
            SetVariantProperties(VariantTypes.Arcane);
        }

        public override string GroupName => groupName;
        public override string SubGroupName => subGroupNames[currentVariant];
        public override TextFile.Token[] SpellMakerDescription => GetSpellMakerDescription();
        public override TextFile.Token[] SpellBookDescription => GetSpellBookDescription();

        // Start is called when the effect is first initialised - do any setup work here
        // Note: Start is called even if effect is never assigned to entity
        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartLight();
        }

        // Resume is called when effect is restored from save
        // Note: Start is not called when effect is restored from save, you may need to do some setup again
        //       The setup you do in resume may be different than first-time setup, which is why these are separated
        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartLight();
        }

        // End is called when effect is finished - do any cleanup work here
        public override void End()
        {
            base.End();
            EndLight();
        }

        // MagicRound is called immediately after effect is assigned to an entity and once per game minute
        // If an effect is resisted by target, it will never receive a single MagicRound
        public override void MagicRound()
        {
            base.MagicRound();
        }

        // ConstantEffect is called once per frame - do any work here that needs to be performed more than once per round
        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Keep light positioned on top of player
            if (myLight)
            {
                myLight.transform.position = GameManager.Instance.PlayerObject.transform.position;
            }
        }

        // IsLikeKind checks if another incumbent effect is equal to this effect
        // Can use class type, key, or whatever conditions you need to test equivalency
        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is MageLight;
        }

        // AddState is used to stack something onto an incumbent for this effect - e.g. top-up rounds remaining
        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        #endregion

        #region Effect Payload

        void StartLight()
        {
            // Do nothing if light already started
            if (myLight)
                return;

            // Using a static helper in FoeSpawner to find best scene parent
            // We want the light to be assigned to one of the scene parents so it will be cleaned up on scene reset
            Transform parent = GameObjectHelper.GetBestParent();

            // Create the light object
            GameObject go = new GameObject(effectKey);
            go.transform.parent = parent;
            myLight = go.AddComponent<Light>();
            myLight.type = LightType.Point;
            myLight.color = variantProperties[currentVariant].effectColor;
            myLight.range = 14;
        }

        void EndLight()
        {
            // Destroy the light gameobject when done
            if (myLight)
                GameObject.Destroy(myLight.gameObject);
        }

        #endregion

        #region Variants

        void SetVariantProperties(VariantTypes variant)
        {
            int variantIndex = (int)variant;
            VariantProperties vp = new VariantProperties();
            vp.effectColor = effectColors[variantIndex];
            vp.effectProperties = properties;
            vp.effectProperties.Key = string.Format("{0}-{1}", effectKey, subGroupKeys[variantIndex]);
            variantProperties[variantIndex] = vp;
        }

        #endregion

        #region Text

        // Text strings are hardcoded in this example effect
        // They could also be sourced from text tables or any other text API/method accessible to the game/mod

        const string groupName = "Mage Light";
        string[] subGroupNames = { "Inferno", "Rime", "Venom", "Storm", "Arcane" };
        const string effectDescription = "Creates a soft light around caster.";

        TextFile.Token[] GetSpellMakerDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                groupName,
                effectDescription,
                "Duration: Rounds you will be illuminated.",
                "Chance: N/A",
                "Magnitude: N/A");
        }

        TextFile.Token[] GetSpellBookDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                groupName,
                "Duration: %bdr + %adr per %cld level(s)",
                "Chance: N/A",
                "Magnitude: N/A",
                effectDescription);
        }

        #endregion
    }
}
