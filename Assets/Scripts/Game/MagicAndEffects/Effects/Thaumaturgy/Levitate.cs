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
using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Levitate.
    /// </summary>
    public class Levitate : IncumbentEffect
    {
        public override void SetProperties()
        {
            properties.Key = "Levitate";
            properties.ClassicKey = MakeClassicKey(14, 255);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "levitate");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1562);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1262);
            properties.SupportDuration = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.DurationCosts = MakeEffectCosts(60, 100, 3);
        }

        public override void MagicRound()
        {
            base.MagicRound();
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

        protected override void BecomeIncumbent()
        {
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

            // Enable levitaion based on entity type
            if (entityBehaviour.EntityType == EntityTypes.Player)
                GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>().IsLevitating = true;

            // TODO: Support changing monsters to levitating by adjusting behaviour to a flying creature
            //if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
        }

        void StopLevitating()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Disable levitaion based on entity type
            if (entityBehaviour.EntityType == EntityTypes.Player)
                GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>().IsLevitating = false;

            // TODO: Stop monster from levitating by restoring normal state
            //if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
        }
    }
}