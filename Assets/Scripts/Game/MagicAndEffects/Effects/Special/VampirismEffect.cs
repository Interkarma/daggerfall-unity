// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich, Hazelnut
// 
// Notes:
//

using FullSerializer;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage two curse effect for vampirism deployed after stage one infection completed.
    /// Handles buffs and other long-running vampire effects.
    /// Note: This effect should only be assigned to player entity by stage one disease effect or classic character import.
    /// </summary>
    public class VampirismEffect : RacialOverrideEffect
    {
        #region Fields

        public const string VampirismCurseKey = "Vampirism-Curse";

        RaceTemplate compoundRace;
        VampireClans vampireClan = VampireClans.Lyrezi;
        uint lastTimeFed;
        bool hasStartedInitialVampireQuest;

        #endregion

        #region Constructors

        public VampirismEffect()
        {
            VampireConsoleCommands.RegisterCommands();
        }

        #endregion

        #region Overrides

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
            UpdateSatiation();

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
            entityBehaviour.Entity.MinMetalToHit = WeaponMaterialTypes.Silver;
        }

        public override void MagicRound()
        {
            base.MagicRound();
            ApplyVampireAdvantages();
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

        public override bool GetCustomRaceGenderAttackSoundData(PlayerEntity entity, out SoundClips soundClipOut)
        {
            switch (entity.Gender)
            {
                default:
                case Genders.Male:
                    soundClipOut = SoundClips.EnemyVampireAttack;
                    break;
                case Genders.Female:
                    soundClipOut = SoundClips.EnemyFemaleVampireAttack;
                    break;
            }

            return true;
        }

        public override void OnWeaponHitEnemy(PlayerEntity playerEntity, EnemyEntity enemyEntity)
        {
            // Player just needs to strike enemy with any weapon (including melee) to register a feeding strike
            UpdateSatiation();
        }

        public override bool CheckFastTravel(PlayerEntity playerEntity)
        {
            if (DaggerfallUnity.Instance.WorldTime.Now.IsDay)
            {
                DaggerfallMessageBox mb = new DaggerfallMessageBox(DaggerfallUI.Instance.UserInterfaceManager);
                mb.PreviousWindow = DaggerfallUI.Instance.UserInterfaceManager.TopWindow;
                mb.ClickAnywhereToClose = true;
                mb.SetText(TextManager.Instance.GetText(textDatabase, "vampireFastTravelDay"));
                mb.Show();
                return false;
            }

            return true;
        }

        public override bool CheckStartRest(PlayerEntity playerEntity)
        {
            const int notSatedTextID = 36;

            if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() - lastTimeFed > DaggerfallDateTime.MinutesPerDay)
            {
                DaggerfallMessageBox mb = new DaggerfallMessageBox(DaggerfallUI.Instance.UserInterfaceManager);
                mb.PreviousWindow = DaggerfallUI.Instance.UserInterfaceManager.TopWindow;
                mb.ClickAnywhereToClose = true;
                mb.SetTextTokens(notSatedTextID);
                mb.Show();
                return false;
            }

            return true;
        }

        public override void StartQuest(bool isCureQuest)
        {
            // More about vampire clan quests can found here:
            // https://en.uesp.net/wiki/Daggerfall:Quests#Vampire_Clans

            if (isCureQuest)
            {
                if (DFRandom.random_range_inclusive(10, 100) < 30)
                    QuestMachine.Instance.InstantiateQuest("$CUREVAM");
            }
            else if (hasStartedInitialVampireQuest)
            {
                // Get an appropriate quest for player's level?
                if (DFRandom.random_range_inclusive(1, 100) < 50)
                {
                    // Get the regional vampire clan faction id for affecting reputation on success/failure, and current rep
                    int factionId = (int)vampireClan;
                    int reputation = GameManager.Instance.PlayerEntity.FactionData.GetReputation(factionId);

                    // Select a quest at random from appropriate pool
                    Quest offeredQuest = GameManager.Instance.QuestListsManager.GetGuildQuest(
                        FactionFile.GuildGroups.Vampires,
                        MembershipStatus.Nonmember,
                        factionId,
                        reputation,
                        GameManager.Instance.PlayerEntity.Level);
                    if (offeredQuest != null)
                        QuestMachine.Instance.InstantiateQuest(offeredQuest);
                }
            }
            else if (DFRandom.random_range_inclusive(1, 100) < 50)
            {
                QuestMachine.Instance.InstantiateQuest("P0A01L00");
                hasStartedInitialVampireQuest = true;
            }
        }

        public override void End()
        {
            base.End();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Remove player metal immunity
            entityBehaviour.Entity.MinMetalToHit = WeaponMaterialTypes.None;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets vampire thirst sated from current point in time.
        /// </summary>
        public void UpdateSatiation()
        {
            lastTimeFed = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
        }

        /// <summary>
        /// Cure vampirism and allow this racial override effect to expire.
        /// Game time is raised by one minute so effect payload expires almost immediately.
        /// </summary>
        public void CureVampirism()
        {
            forcedRoundsRemaining = 0;
            ResignAsIncumbent();
            DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.RaiseTime(60);
            GameManager.Instance.PlayerEntity.PreviousVampireClan = vampireClan;
            // TODO: End all vampire quests that might be running other than cure quest
        }

        /// <summary>
        /// Gets name of vampire clan from Races text database.
        /// </summary>
        public string GetClanName()
        {
            return TextManager.Instance.GetText(racesTextDatabase, vampireClan.ToString().ToLower());
        }

        #endregion

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

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public RaceTemplate compoundRace;
            public VampireClans vampireClan;
            public uint lastTimeFed;
            public bool hasStartedInitialVampireQuest;
        }

        public override object GetSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.compoundRace = compoundRace;
            data.vampireClan = vampireClan;
            data.lastTimeFed = lastTimeFed;
            data.hasStartedInitialVampireQuest = hasStartedInitialVampireQuest;

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
            hasStartedInitialVampireQuest = data.hasStartedInitialVampireQuest;
        }

        #endregion

        #region Console Commands

        public static class VampireConsoleCommands
        {
            public static void RegisterCommands()
            {
                try
                {
                    ConsoleCommandsDatabase.RegisterCommand(FeedMe.name, FeedMe.description, FeedMe.usage, FeedMe.Execute);
                    ConsoleCommandsDatabase.RegisterCommand(CureMe.name, CureMe.description, CureMe.usage, CureMe.Execute);
                }
                catch (System.Exception ex)
                {
                    DaggerfallUnity.LogMessage(ex.Message, true);
                }
            }

            private static class FeedMe
            {
                public static readonly string name = "vamp_feedme";
                public static readonly string description = "Vampire thirst becomes sated.";
                public static readonly string usage = "vamp_feedme";

                public static string Execute(params string[] args)
                {
                    if (GameManager.Instance.PlayerEffectManager.HasVampirism())
                    {
                        (GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect() as VampirismEffect).UpdateSatiation();
                        return "Your thirst has been sated.";
                    }
                    else
                        return "Player is not a vampire.";
                }
            }

            private static class CureMe
            {
                public static readonly string name = "vamp_cureme";
                public static readonly string description = "Player is cured of vampirism effect at start of next magic round (1 game minute).";
                public static readonly string usage = "vamp_cureme";

                public static string Execute(params string[] args)
                {
                    if (GameManager.Instance.PlayerEffectManager.HasVampirism())
                    {
                        GameManager.Instance.PlayerEffectManager.EndVampirism();
                        return "You have been cured of vampirism.";
                    }
                    else
                        return "Player is not a vampire.";
                }
            }
        }

        #endregion
    }
}