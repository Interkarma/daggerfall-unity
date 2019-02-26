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
        readonly string[] subGroupTextKeys = { "animal", "undead", "humanoid", "daedra" };

        readonly VariantProperties[] variantProperties = new VariantProperties[totalVariants];

        #endregion

        #region Structs

        struct VariantProperties
        {
            public DFCareer.EnemyGroups targetGroup;
            public EffectProperties effectProperties;
        }

        #endregion

        #region Properties

        public override EffectProperties Properties
        {
            get { return variantProperties[currentVariant].effectProperties; }
        }

        public DFCareer.EnemyGroups TargetGroup
        {
            get { return variantProperties[currentVariant].targetGroup; }
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
            SetVariantProperties(DFCareer.EnemyGroups.Animals, 0);
            SetVariantProperties(DFCareer.EnemyGroups.Undead, 1);
            SetVariantProperties(DFCareer.EnemyGroups.Humanoid, 2);
            SetVariantProperties(DFCareer.EnemyGroups.Daedra, 3);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Set target non-hostile
            if (IsGroupMatch(entityBehaviour.Entity))
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

        void SetVariantProperties(DFCareer.EnemyGroups targetGroup, int variantIndex)
        {
            string name = TextManager.Instance.GetText("ClassicEffects", subGroupTextKeys[variantIndex]);

            VariantProperties vp = new VariantProperties();
            vp.effectProperties = properties;
            vp.effectProperties.Key = string.Format("Pacify-{0}", name);
            vp.effectProperties.ClassicKey = MakeClassicKey(33, (byte)variantIndex);
            vp.effectProperties.SubGroupName = name;
            vp.effectProperties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1585 + variantIndex);
            vp.effectProperties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1285 + variantIndex);
            vp.targetGroup = targetGroup;

            if (targetGroup == DFCareer.EnemyGroups.Animals)
                properties.ChanceCosts = MakeEffectCosts(60, 100, 160);
            else if (targetGroup == DFCareer.EnemyGroups.Daedra)
                properties.ChanceCosts = MakeEffectCosts(60, 120, 36);
            else if (targetGroup == DFCareer.EnemyGroups.Humanoid || targetGroup == DFCareer.EnemyGroups.Undead)
                properties.ChanceCosts = MakeEffectCosts(80, 140, 60);

            variantProperties[variantIndex] = vp;
        }

        bool IsGroupMatch(DaggerfallEntity entity)
        {
            // Validate entity
            if (entity == null || !(entity is EnemyEntity))
                return false;

            // Check target match
            // Note: Pacify Humanoid only operates on humanoid monsters (not enemy classes)
            DFCareer.EnemyGroups enemyGroup = (entity as EnemyEntity).GetEnemyGroup();
            return TargetGroup == enemyGroup;
        }

        #endregion  
    }
}
