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
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Levitate.
    /// </summary>
    public class Levitate : IncumbentEffect
    {
        public static readonly string EffectKey = "Levitate";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(14, 255);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "levitate");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1562);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1262);
            properties.SupportDuration = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.DurationCosts = MakeEffectCosts(60, 100);
        }

        public override void SetPotionProperties()
        {
            PotionRecipe levitation = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "levitation"),
                125,
                DefaultEffectSettings(),
                (int)Items.MiscellaneousIngredients1.Pure_water,
                (int)Items.MiscellaneousIngredients1.Nectar,
                (int)Items.CreatureIngredients1.Ectoplasm);

            AssignPotionRecipes(levitation);
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();
            StartLevitating();
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartLevitating();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartLevitating();
        }

        public override void End()
        {
            base.End();
            StopLevitating();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is Levitate);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        void StartLevitating()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Enable levitation for player or enemies
            if (entityBehaviour.EntityType == EntityTypes.Player)
            {
                GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>().IsLevitating = true;
            }
            else if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
            {
                EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                if (enemyMotor)
                    enemyMotor.IsLevitating = true;
            }
        }

        void StopLevitating()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Disable levitation for player or enemies
            if (entityBehaviour.EntityType == EntityTypes.Player)
            {
                GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>().IsLevitating = false;
            }
            else if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
            {
                EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                if (enemyMotor)
                    enemyMotor.IsLevitating = false;
            }
        }
    }
}
