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
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage two curse effect for vampirism deployed after stage one infection completed.
    /// Handles buffs and other long-running vampire effects.
    /// Note: This effect should only be assigned to player entity by stage one disease effect.
    ///
    /// TODO:
    ///  * Clear guild memberships and reset reputations
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

        RaceTemplate compoundRace;
        VampireClans vampireClan = VampireClans.Lyrezi;
        uint lastTimeFed;

        public VampireClans VampireClan
        {
            get { return vampireClan; }
            set { vampireClan = value; }
        }

        public override RaceTemplate CustomRace
        {
            get { return GetCompoundRace(); }
        }

        public override void SetProperties()
        {
            properties.Key = VampirismCurseKey;
            properties.ShowSpellIcon = false;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Create compound vampire race from birth race
            CreateCompoundRace();

            // Get vampire clan from stage one disease
            // Otherwise start as Lyrezi by default if no infection found
            // Note: Classic save import will start this effect and set correct clan after load
            VampirismInfection infection = (VampirismInfection)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<VampirismInfection>();
            if (infection != null)
                vampireClan = infection.InfectionVampireClan;

            // Considered well fed on first start
            lastTimeFed = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            // Our dark transformation is complete - cure everything on player (including stage one disease)
            GameManager.Instance.PlayerEffectManager.CureAll();
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

            // Execute advantages and disadvantages
            ApplyVampireAdvantages();
            ApplyVampireDisadvantages();
        }

        public override bool GetCustomHeadImageData(PlayerEntity entity, out ImageData imageDataOut)
        {
            const string vampHeads = "VAMP00I0.CIF";

            // Vampires have a limited set of heads, one per birth race and gender
            // Does not follow same selection rules as standard racial head images
            int index;
            switch (entity.Gender)
            {
                default:
                case Genders.Male:
                    index = 8 + entity.BirthRaceTemplate.ID - 1;
                    break;
                case Genders.Female:
                    index = entity.BirthRaceTemplate.ID - 1;
                    break;
            }

            imageDataOut = ImageReader.GetImageData(vampHeads, index, 0, true);
            return true;
        }

        #region Private Methods

        void CreateCompoundRace()
        {
            // Clone birth race and assign custom settings
            // New compound races will retain almost everything from birth race
            compoundRace = GameManager.Instance.PlayerEntity.BirthRaceTemplate.Clone();
            compoundRace.Name = TextManager.Instance.GetText(racesTextDatabase, "vampire");

            // Set special vampire flags
            compoundRace.ImmunityFlags |= DFCareer.EffectFlags.Paralysis;
            compoundRace.ImmunityFlags |= DFCareer.EffectFlags.Disease;
            compoundRace.SpecialAbilities |= DFCareer.SpecialAbilityFlags.SunDamage;
            compoundRace.SpecialAbilities |= DFCareer.SpecialAbilityFlags.HolyDamage;
        }

        RaceTemplate GetCompoundRace()
        {
            // Create compound race if one doesn't already exist
            if (compoundRace == null)
                CreateCompoundRace();

            return compoundRace;
        }

        void ApplyVampireAdvantages()
        {
            // Set stat mods to all but INT
            const int statModAmount = 20;
            SetStatMod(DFCareer.Stats.Strength, statModAmount);
            SetStatMod(DFCareer.Stats.Willpower, statModAmount);
            SetStatMod(DFCareer.Stats.Agility, statModAmount);
            SetStatMod(DFCareer.Stats.Endurance, statModAmount);
            SetStatMod(DFCareer.Stats.Personality, statModAmount);
            SetStatMod(DFCareer.Stats.Speed, statModAmount);
            SetStatMod(DFCareer.Stats.Luck, statModAmount);

            // Set skill mods
            const int skillModAmount = 30;
            SetSkillMod(DFCareer.Skills.Jumping, skillModAmount);
            SetSkillMod(DFCareer.Skills.Running, skillModAmount);
            SetSkillMod(DFCareer.Skills.Stealth, skillModAmount);
            SetSkillMod(DFCareer.Skills.CriticalStrike, skillModAmount);
            SetSkillMod(DFCareer.Skills.Climbing, skillModAmount);
            SetSkillMod(DFCareer.Skills.HandToHand, skillModAmount);

            // Set clan stat mods
            if (vampireClan == VampireClans.Anthotis)
                SetStatMod(DFCareer.Stats.Intelligence, statModAmount);
        }

        void ApplyVampireDisadvantages()
        {
            // TODO: Damage from sunlight

            // TODO: Damage from holy places
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public RaceTemplate compoundRace;
            public VampireClans vampireClan;
            public uint lastTimeFed;
        }

        public override object GetSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.compoundRace = compoundRace;
            data.vampireClan = vampireClan;
            data.lastTimeFed = lastTimeFed;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            CustomSaveData_v1 data = (CustomSaveData_v1)dataIn;
            compoundRace = data.compoundRace;
            vampireClan = data.vampireClan;
            lastTimeFed = data.lastTimeFed;
        }

        #endregion
    }
}