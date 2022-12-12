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

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Levitate.
    /// </summary>
    public class Levitate : IncumbentEffect
    {
        public static readonly string EffectKey = "Levitate";

        LevitateMotor levitateMotor;
        EnemyMotor enemyMotor;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(14, 255);
            properties.SupportDuration = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.DurationCosts = MakeEffectCosts(60, 100);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("levitate");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1562);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1262);

        public override void SetPotionProperties()
        {
            PotionRecipe levitation = new PotionRecipe(
                "levitation",
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
                SetLevitateMotor(true);
            }
            else if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
            {
                SetEnemyMotor(true);
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
                SetLevitateMotor(GameManager.Instance.PlayerEntity.NoClipMode);
            }
            else if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
            {
                SetEnemyMotor(false);
            }
        }

        void SetLevitateMotor(bool state)
        {
            if (levitateMotor)
                levitateMotor.IsLevitating = state;
            else
            {
                levitateMotor = GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>();
                if (levitateMotor)
                    levitateMotor.IsLevitating = state;
            }
        }

        void SetEnemyMotor(bool state)
        {
            if (enemyMotor)
                enemyMotor.IsLevitating = state;
            else
            {
                DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
                if (entityBehaviour)
                {
                    enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                    if (enemyMotor)
                        enemyMotor.IsLevitating = state;
                }
            }
        }
    }
}
