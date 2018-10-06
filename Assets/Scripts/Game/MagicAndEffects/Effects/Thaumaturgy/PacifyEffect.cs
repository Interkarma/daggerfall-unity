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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Pacify - Animal/Undead/Humanoid/Daedra
    /// </summary>
    public class PacifyEffect : BaseEntityEffect
    {
        #region Fields

        const int totalVariants = 4;
        const string textDatabase = "ClassicEffects";
        readonly string[] subGroupTextKeys = new string[] { "animal", "undead", "humanoid", "daedra" };

        VariantProperties[] variantProperties = new VariantProperties[totalVariants];

        #endregion

        #region Structs

        struct VariantProperties
        {
            public MobileAffinity targetAffinity;
            public EffectProperties effectProperties;
        }

        #endregion

        #region Properties

        public override EffectProperties Properties
        {
            get { return variantProperties[currentVariant].effectProperties; }
        }

        public MobileAffinity TargetAffinity
        {
            get { return variantProperties[currentVariant].targetAffinity; }
        }

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            // Set properties shared by all variants
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "pacify");
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Other;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;

            // Set unique variant properties
            variantCount = totalVariants;
            SetVariantProperties(MobileAffinity.Animal, 0);
            SetVariantProperties(MobileAffinity.Undead, 1);
            SetVariantProperties(MobileAffinity.Human, 2);
            SetVariantProperties(MobileAffinity.Daedra, 3);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Set target non-hostile
            if (IsAffinityMatch(entityBehaviour.Entity))
            {
                EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                if (enemyMotor)
                {
                    enemyMotor.IsHostile = false;
                    Debug.LogFormat("Enemy {0} is now non-hostile", enemyMotor.name);
                }
            }
        }

        #endregion

        #region Private Methods

        void SetVariantProperties(MobileAffinity affinity, int variantIndex)
        {
            string name = TextManager.Instance.GetText("ClassicEffects", subGroupTextKeys[variantIndex]);

            VariantProperties vp = new VariantProperties();
            vp.effectProperties = properties;
            vp.effectProperties.Key = string.Format("ElementalResistance-{0}", name);
            vp.effectProperties.ClassicKey = MakeClassicKey(33, (byte)variantIndex);
            vp.effectProperties.SubGroupName = name;
            vp.effectProperties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1527 + variantIndex);
            vp.effectProperties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1227 + variantIndex);
            vp.targetAffinity = affinity;

            if (affinity == MobileAffinity.Animal)
                properties.ChanceCosts = MakeEffectCosts(60, 100, 160);
            else if (affinity == MobileAffinity.Daedra)
                properties.ChanceCosts = MakeEffectCosts(60, 120, 36);
            else if (affinity == MobileAffinity.Human || affinity == MobileAffinity.Undead)
                properties.ChanceCosts = MakeEffectCosts(80, 140, 60);

            variantProperties[variantIndex] = vp;
        }

        bool IsAffinityMatch(DaggerfallEntity entity)
        {
            // Validate entity
            if (entity == null || !(entity is EnemyEntity))
                return false;

            // Check affinity match
            EnemyEntity enemyEntity = entity as EnemyEntity;
            if (TargetAffinity == enemyEntity.MobileEnemy.Affinity)
                return true;

            // TODO: Should orcs also be considered humanoid for purposes of pacify?

            return false;
        }

        #endregion  
    }
}