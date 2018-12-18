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

using FullSerializer;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage two curse effect for vampirism deployed after stage one infection completed.
    /// Handles buffs and other long-running vampire effects.
    /// Note: This effect should only be assigned to player entity by stage one disease effect.
    ///
    /// TODO:
    ///  * Get clan from infection (DONE)
    ///  * Display "death is not eternal" popup (DONE)
    ///  * Assign cheap vampire spells and clan-specific spells (DONE)
    ///  * Support for cheaper spells in casting system (DONE)
    ///  * Clear guild memberships and reset reputations
    ///  * Non-clan modifiers (DONE)
    ///  * Clan-based modifiers (DONE)
    ///  * Damage from sunlight
    ///  * Damage from holy places
    ///  * Fast travel to arrive in early evening instead of early morning
    ///  * Must feed once per day to rest
    ///  * Can't rest in daylight or holy places
    ///  * Deploy vampire questline
    /// </summary>
    public class VampirismEffect : RacialOverrideEffect
    {
        public const string VampirismCurseKey = "Vampirism-Curse";

        VampireClans vampireClan;
        uint lastTimeFed;

        public VampireClans VampireClan
        {
            get { return vampireClan; }
        }

        public override void SetProperties()
        {
            properties.Key = VampirismCurseKey;
            properties.ShowSpellIcon = false;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            const int deathIsNotEternalTextID = 401;

            base.Start(manager, caster);

            // Get vampire clan from stage one disease
            VampirismInfection infection = (VampirismInfection)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<VampirismInfection>();
            if (infection == null)
            {
                End();
                return;
            }
            vampireClan = infection.InfectionVampireClan;

            // Assign vampire spells to spellbook
            GameManager.Instance.PlayerEntity.AssignPlayerVampireSpells(vampireClan);

            // Considered well fed on first start
            lastTimeFed = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            // Our dark transformation is complete - cure everything on player (including stage one disease)
            GameManager.Instance.PlayerEffectManager.CureAll();

            // Display popup
            DaggerfallMessageBox mb = DaggerfallUI.MessageBox(deathIsNotEternalTextID);
            mb.Show();
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

            // Set clan stat mods
            if (vampireClan == VampireClans.Anthotis)
                SetStatMod(DaggerfallConnect.DFCareer.Stats.Intelligence, statModAmount);
        }

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public VampireClans vampireClan;
            public uint lastTimeFed;
        }

        public override object GetSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.vampireClan = vampireClan;
            data.lastTimeFed = lastTimeFed;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            CustomSaveData_v1 data = (CustomSaveData_v1)dataIn;
            vampireClan = data.vampireClan;
            lastTimeFed = data.lastTimeFed;
        }

        #endregion
    }
}