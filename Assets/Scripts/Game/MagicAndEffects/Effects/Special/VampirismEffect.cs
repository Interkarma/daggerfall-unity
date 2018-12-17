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

using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage two curse effect for vampirism deployed after stage one infection completed.
    /// Handles buffs and other long-running vampire effects.
    /// Note: This disease should only be assigned to player entity by stage one disease effect.
    ///
    /// TODO:
    ///  * Non-clan modifiers (DONE)
    ///  * Clan-based modifiers
    ///  * Assign cheap vampire spells and clan-specific spells
    ///  * Support for cheaper spells in casting system
    ///  * Damage from sunlight
    ///  * Damage from holy places
    /// </summary>
    public class VampirismEffect : IncumbentEffect
    {
        int forcedRoundsRemaining = 1;

        public override void SetProperties()
        {
            properties.Key = "Vampirism-Effect";
        }

        // Always present at least one round remaining so effect system does not remove
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        // Vampirism effect is permanent until cured so we manage our own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            // Comparing keys should be enough for like-kind test
            // Child classes can override test if they need to
            return (other.Key == Key);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Nothing to do here
            return;
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Assign constant state changes for vampires
            entityBehaviour.Entity.IsImmuneToDisease = true;
            entityBehaviour.Entity.IsImmuneToParalysis = true;
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Set stat mods to all but INT
            const int statModAmount = 20;
            SetStatMod(DaggerfallConnect.DFCareer.Stats.Strength, statModAmount);
            SetStatMod(DaggerfallConnect.DFCareer.Stats.Willpower, statModAmount);
            SetStatMod(DaggerfallConnect.DFCareer.Stats.Agility, statModAmount);
            SetStatMod(DaggerfallConnect.DFCareer.Stats.Endurance, statModAmount);
            SetStatMod(DaggerfallConnect.DFCareer.Stats.Personality, statModAmount);
            SetStatMod(DaggerfallConnect.DFCareer.Stats.Speed, statModAmount);
            SetStatMod(DaggerfallConnect.DFCareer.Stats.Luck, statModAmount);

            // Set skill mods
            const int skillModAmount = 30;
            SetSkillMod(DaggerfallConnect.DFCareer.Skills.Jumping, skillModAmount);
            SetSkillMod(DaggerfallConnect.DFCareer.Skills.Running, skillModAmount);
            SetSkillMod(DaggerfallConnect.DFCareer.Skills.Stealth, skillModAmount);
            SetSkillMod(DaggerfallConnect.DFCareer.Skills.CriticalStrike, skillModAmount);
            SetSkillMod(DaggerfallConnect.DFCareer.Skills.Climbing, skillModAmount);
            SetSkillMod(DaggerfallConnect.DFCareer.Skills.HandToHand, skillModAmount);
        }
    }
}