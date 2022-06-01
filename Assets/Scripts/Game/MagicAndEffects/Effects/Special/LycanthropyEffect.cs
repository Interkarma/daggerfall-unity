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
using FullSerializer;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Questing;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage two curse effect for lycanthropy deployed after stage one infection completed.
    /// Handles buffs and other long-running werebeast effects.
    /// Note: This effect should only be assigned to player entity by stage one disease effect or classic character import.
    /// </summary>
    public class LycanthropyEffect : RacialOverrideEffect
    {
        #region Fields

        public const string LycanthropyCurseKey = "Lycanthropy-Curse";

        const string cureQuestName = "$CUREWER";
        const int paperDollWidth = 110;
        const int paperDollHeight = 184;
        const int needToKillHealthLimitMinimum = 4;
        const int needToKillNotifySeconds = 120;
        const int needToKillPeriod = DaggerfallDateTime.MinutesPerDay * DaggerfallDateTime.DaysPerMonth;
        const float needToKillHealthLossPerMinute = 24.0f / DaggerfallDateTime.MinutesPerDay;

        RaceTemplate compoundRace;
        LycanthropyTypes infectionType = LycanthropyTypes.None;
        uint lastKilledInnocent;
        uint lastCastMorphSelf;
        bool wearingHircineRing;
        bool isTransformed;
        bool isFullMoon;
        bool urgeToKillRising;

        DFSize backgroundFullSize = new DFSize(125, 198);
        Rect backgroundSubRect = new Rect(8, 7, paperDollWidth, paperDollHeight);
        Texture2D backgroundTexture;

        float moveSoundTimer;
        float needToKillNotifyTimer;

        #endregion

        #region Constructors

        public LycanthropyEffect()
        {
            InitMoveSoundTimer();
            LycanthropeConsoleCommands.RegisterCommands();
        }

        #endregion

        #region Properties

        private uint TimeSinceLastInnocentKilled
        {
            get { return DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() - lastKilledInnocent; }
        }

        public LycanthropyTypes InfectionType
        {
            get { return infectionType; }
            set { infectionType = value; }
        }

        public override RaceTemplate CustomRace
        {
            get { return GetCompoundRace(); }
        }

        public bool IsTransformed
        {
            get { return isTransformed; }
        }

        public bool NeedToKill
        {
            get { return urgeToKillRising; }
        }

        /// <summary>
        /// Combat Voices option is suppressed while transformed.
        /// Transformed lycanthropes play custom attack voices on enemy hit.
        /// </summary>
        public override bool SuppressOptionalCombatVoices
        {
            get { return isTransformed; }
        }

        /// <summary>
        /// Lycanthropes only display a custom background while transformed.
        /// </summary>
        public override bool SuppressPaperDollBodyAndItems
        {
            get { return isTransformed; }
        }

        /// <summary>
        /// Lycanthropes are not tagged with crimes while transformed.
        /// </summary>
        public override bool SuppressCrime
        {
            get { return isTransformed; }
        }

        /// <summary>
        /// Do not spawn additional population while transformed.
        /// </summary>
        public override bool SuppressPopulationSpawns
        {
            get { return isTransformed; }
        }

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            properties.Key = LycanthropyCurseKey;
            properties.ShowSpellIcon = false;
            bypassSavingThrows = true;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Create compound lycanthrope race from birth race
            CreateCompoundRace();

            // Get infection type from stage one disease
            // Note: Classic save import will start this effect and set correct type after load
            LycanthropyInfection infection = (LycanthropyInfection)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<LycanthropyInfection>();
            if (infection != null)
                infectionType = infection.InfectionType;

            // Considered sated on first start
            UpdateSatiation();

            // Our transformation is complete - cure everything on player (including stage one disease)
            GameManager.Instance.PlayerEffectManager.CureAll();

            // Refresh head texture after effect starts
            DaggerfallUI.RefreshLargeHUDHeadTexture();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);

            // Refresh head texture after effect resumes
            DaggerfallUI.RefreshLargeHUDHeadTexture();
        }

        public override void End()
        {
            base.End();

            // Refresh head texture after effect ends
            DaggerfallUI.RefreshLargeHUDHeadTexture();
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Assign constant state changes for lycanthropes
            entityBehaviour.Entity.IsImmuneToDisease = true;

            // Assign minimum metal to hit only while transformed
            if (isTransformed)
                entityBehaviour.Entity.MinMetalToHit = WeaponMaterialTypes.Silver;
            else
                entityBehaviour.Entity.MinMetalToHit = WeaponMaterialTypes.Iron;

            // Play move sound while transformed after random amount of time has elapsed
            if (isTransformed)
            {
                moveSoundTimer -= Time.deltaTime;
                if (moveSoundTimer < 0)
                {
                    PlayLycanthropeMoveSound();
                    InitMoveSoundTimer();
                }
            }

            // Lycanthropes must kill an innocent once per month unless wearing Hircine's Ring
            // Storing this outcome so external systems can query via property
            urgeToKillRising = GetNeedToKill();

            // Handle need to kill innocents
            if (urgeToKillRising)
            {
                // Notify player if lycanthrope has not killed in last month
                // We are notifying player more frequently than classic here so they don't forget character is in weakened state
                // But hopefully not so often it becomes annoying - using real-time minutes while game not paused
                needToKillNotifyTimer -= Time.deltaTime;
                if (needToKillNotifyTimer < 0)
                {
                    NotifyNeedToKill();
                    needToKillNotifyTimer = needToKillNotifySeconds;
                }

                // Limit maximum health
                // This gradually decreases max health over time until a limit is reached
                uint urgeDuration = TimeSinceLastInnocentKilled - needToKillPeriod;
                int reduction = Mathf.RoundToInt(urgeDuration * needToKillHealthLossPerMinute);
                int healthLimit = GameManager.Instance.PlayerEntity.RawMaxHealth - reduction;
                if (healthLimit < needToKillHealthLimitMinimum)
                    healthLimit = needToKillHealthLimitMinimum;
                GameManager.Instance.PlayerEntity.SetMaxHealthLimiter(healthLimit);
            }

            // Copy transformed state to player entity - used as a hostile condition by mobile NPCs
            GameManager.Instance.PlayerEntity.IsInBeastForm = isTransformed;
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Check if player is wearing Hircine's Ring at start of each magic round
            // This item will change certain lycanthropy payload behaviours when equipped
            wearingHircineRing = IsWearingHircineRing();

            // Check for full moon in either lunar cycle
            isFullMoon = DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase == LunarPhases.Full || DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase == LunarPhases.Full;

            // Payloads
            ApplyLycanthropeAdvantages();
            ForceTransformDuringFullMoon();

            //// Some temp debug info used during development
            //Debug.LogFormat(
            //    "Lycanthropy MagicRound(). Type={0}, HircineRing={1}, IsTransformed={2}, Massar={3}, Secunda={4}",
            //    infectionType,
            //    wearingHircineRing,
            //    isTransformed,
            //    DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase,
            //    DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase);
        }

        public override bool GetCustomPaperDollBackgroundTexture(PlayerEntity playerEntity, out Texture2D textureOut)
        {
            const string werewolfBackground = "WOLF00I0.IMG";
            const string wereboarBackground = "BOAR00I0.IMG";

            // Do nothing if not transformed
            textureOut = null;
            if (!isTransformed)
                return false;

            // Get source texture based on lycanthropy type
            string filename;
            switch (infectionType)
            {
                case LycanthropyTypes.Werewolf:
                    filename = werewolfBackground;
                    break;
                case LycanthropyTypes.Wereboar:
                    filename = wereboarBackground;
                    break;
                default:
                    return false;
            }

            // Background is cut into sub-texture and cached on first call
            if (!backgroundTexture)
            {
                Texture2D texture = ImageReader.GetTexture(filename, 0, 0, false);
                backgroundTexture = ImageReader.GetSubTexture(texture, backgroundSubRect, backgroundFullSize);
            }

            textureOut = backgroundTexture;
            return true;
        }

        public override bool GetCustomHeadImageData(PlayerEntity playerEntity, out ImageData imageDataOut)
        {
            const string boarHead = "WERE00I0.IMG";
            const string wolfHead = "WERE01I0.IMG";

            // Use standard head if not transformed
            imageDataOut = new ImageData();
            if (!isTransformed)
                return false;

            // Select head based on lycanthropy type
            string filename;
            switch (infectionType)
            {
                case LycanthropyTypes.Werewolf:
                    filename = wolfHead;
                    break;
                case LycanthropyTypes.Wereboar:
                    filename = boarHead;
                    break;
                default:
                    return false;
            }

            imageDataOut = ImageReader.GetImageData(filename, 0, 0, true);
            return true;
        }

        public override bool SetFPSWeapon(FPSWeapon target)
        {
            if (isTransformed)
            {
                target.WeaponType = WeaponTypes.Werecreature;
                target.MetalType = MetalTypes.None;
                target.DrawWeaponSound = SoundClips.None;
                target.SwingWeaponSound = SoundClips.SwingHighPitch;
                target.Reach = WeaponManager.defaultWeaponReach;
                return true;
            }

            return false;
        }

        public override void OnWeaponHitEntity(PlayerEntity playerEntity, DaggerfallEntity targetEntity = null)
        {
            const int chanceOfAttackSound = 10;
            const int chanceOfBarkSound = 20;

            // Check if we killed an innocent and update satiation - do not need to be transformed
            if (KilledInnocent(targetEntity))
                UpdateSatiation();

            // Do nothing further if not transformed
            if (!isTransformed)
                return;

            // Lycanthrope characters emit both attack and bark sounds while attacking
            SoundClips customSound = SoundClips.None;
            if (infectionType == LycanthropyTypes.Werewolf)
            {
                if (Dice100.SuccessRoll(chanceOfAttackSound))
                    customSound = SoundClips.EnemyWerewolfAttack;
                else if (Dice100.SuccessRoll(chanceOfBarkSound))
                    customSound = SoundClips.EnemyWerewolfBark;
            }
            else if (infectionType == LycanthropyTypes.Wereboar)
            {
                if (Dice100.SuccessRoll(chanceOfAttackSound))
                    customSound = SoundClips.EnemyWereboarAttack;
                else if (Dice100.SuccessRoll(chanceOfBarkSound))
                    customSound = SoundClips.EnemyWereboarBark;
            }

            // Play sound through weapon
            FPSWeapon screenWeapon = GameManager.Instance.WeaponManager.ScreenWeapon;
            if (screenWeapon && customSound != SoundClips.None)
                screenWeapon.PlayAttackVoice(customSound);
        }

        bool KilledInnocent(DaggerfallEntity targetEntity)
        {
            // Must have a target entity and behaviour
            if (targetEntity == null || targetEntity.EntityBehaviour == false)
                return false;

            // Check if this is an innocent target (currently mobile NPCs and city watch)
            bool isInnocent = false;
            if (targetEntity.EntityBehaviour.EntityType == EntityTypes.CivilianNPC)
            {
                isInnocent = true;
            }
            else if (targetEntity.EntityBehaviour.EntityType == EntityTypes.EnemyClass)
            {
                EnemyEntity enemyEntity = targetEntity as EnemyEntity;
                if (enemyEntity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                    isInnocent = true;
            }

            // Was that innocent killed?
            if (isInnocent && targetEntity.CurrentHealth <= 0)
                return true;

            return false;
        }

        public override bool GetSuppressInventory(out string suppressInventoryMessage)
        {
            if (isTransformed)
            {
                suppressInventoryMessage = TextManager.Instance.GetLocalizedText("inventoryWhileShapechanged");
                return true;
            }
            else
            {
                suppressInventoryMessage = string.Empty;
                return false;
            }
        }

        public override bool GetSuppressTalk(out string suppressTalkMessage)
        {
            if (isTransformed)
            {
                suppressTalkMessage = TextManager.Instance.GetLocalizedText("youGetNoResponse");
                return true;
            }
            else
            {
                suppressTalkMessage = string.Empty;
                return false;
            }
        }

        public override void StartQuest(bool isCureQuest)
        {
            base.StartQuest(isCureQuest);

            if (isCureQuest && DFRandom.random_range_inclusive(1, 100) < 30)
            {
                // Do nothing if a cure instance already running
                // This is a long-running quest that involves hunters if player not cured by end of time limit
                ulong[] quests = QuestMachine.Instance.FindQuests(cureQuestName);
                if (quests != null && quests.Length > 0)
                    return;

                // Start the cure quest
                QuestMachine.Instance.StartQuest(cureQuestName);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets lycanthrope need to kill sated from current point in time.
        /// </summary>
        public void UpdateSatiation()
        {
            // Store time sated
            lastKilledInnocent = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();  

            // Reset need to kill timer to 0 so player is notified immediately next time
            needToKillNotifyTimer = 0;
            urgeToKillRising = false;
        }

        /// <summary>
        /// Cure lycanthropy and allow this racial override effect to expire.
        /// Game time is raised by one minute so effect payload expires almost immediately.
        /// </summary>
        public void CureLycanthropy()
        {
            // Transform back to humanoid form once last time to perform any race cleanup
            if (isTransformed)
                MorphSelf();

            // Heal player back to full
            GameManager.Instance.PlayerEntity.CurrentHealth = GameManager.Instance.PlayerEntity.RawMaxHealth;

            // End effect and cleanup
            forcedRoundsRemaining = 0;
            ResignAsIncumbent();
            DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.RaiseTime(60);
            GameManager.Instance.PlayerEntity.DeleteTaggedSpells(PlayerEntity.lycanthropySpellTag);
            EndLycanthropyQuests();
        }

        public virtual void MorphSelf(bool forceMorph = false)
        {
            // Do transformation between forms
            if (!isTransformed)
            {
                // Observe 24 hour cooldown on shapechange into beast form unless a full moon is forcing change
                // Player can always cast to exit beast form or with no restrictions while wearing Hircine's Ring
                if (!CanCastMorphSelf() && !forceMorph)
                {
                    string canOnlyCastOncePerDay = TextManager.Instance.GetLocalizedText("canOnlyCastOncePerDay");
                    DaggerfallUI.MessageBox(canOnlyCastOncePerDay);
                    return;
                }

                isTransformed = true;

                // Unequip any items held in hands
                GameManager.Instance.PlayerEntity.ItemEquipTable.UnequipItem(EquipSlots.RightHand);
                GameManager.Instance.PlayerEntity.ItemEquipTable.UnequipItem(EquipSlots.LeftHand);

                // Set race name based on infection type
                if (infectionType == LycanthropyTypes.Werewolf)
                    compoundRace.Name = TextManager.Instance.GetLocalizedText("werewolf");
                else if (infectionType == LycanthropyTypes.Wereboar)
                    compoundRace.Name = TextManager.Instance.GetLocalizedText("wereboar");
                else
                    compoundRace.Name = GameManager.Instance.PlayerEntity.BirthRaceTemplate.Name;

                // Initialise move sound timer
                InitMoveSoundTimer();
            }
            else
            {
                isTransformed = false;

                // Restore birth race name
                compoundRace.Name = GameManager.Instance.PlayerEntity.BirthRaceTemplate.Name;
            }

            // Classic heals player to full each time they transform
            // Not sure if intentional and definitely exploitable during full moons
            // One of those cases where implementing like classic and open to review later
            GameManager.Instance.PlayerEntity.CurrentHealth = GameManager.Instance.PlayerEntity.MaxHealth;

            // Store time whenever cast
            lastCastMorphSelf = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            // Refresh head texture after transform
            DaggerfallUI.RefreshLargeHUDHeadTexture();
        }

        #endregion

        #region Private Methods

        void CreateCompoundRace()
        {
            // Clone birth race and assign custom settings
            // New compound races will retain almost everything from birth race
            compoundRace = GameManager.Instance.PlayerEntity.BirthRaceTemplate.Clone();

            // Set special lycanthropy flags
            compoundRace.ImmunityFlags |= DFCareer.EffectFlags.Disease;
        }

        RaceTemplate GetCompoundRace()
        {
            // Create compound race if one doesn't already exist
            if (compoundRace == null)
                CreateCompoundRace();

            return compoundRace;
        }

        void ApplyLycanthropeAdvantages()
        {
            // Set stat mods
            const int statModAmount = 40;
            SetStatMod(DFCareer.Stats.Strength, statModAmount);
            SetStatMod(DFCareer.Stats.Agility, statModAmount);
            SetStatMod(DFCareer.Stats.Endurance, statModAmount);
            SetStatMod(DFCareer.Stats.Speed, statModAmount);

            // Set skill mods
            const int skillModAmount = 30;
            SetSkillMod(DFCareer.Skills.Swimming, skillModAmount);
            SetSkillMod(DFCareer.Skills.Running, skillModAmount);
            SetSkillMod(DFCareer.Skills.Stealth, skillModAmount);
            SetSkillMod(DFCareer.Skills.CriticalStrike, skillModAmount);
            SetSkillMod(DFCareer.Skills.Climbing, skillModAmount);
            SetSkillMod(DFCareer.Skills.HandToHand, skillModAmount);
            SetSkillMod(DFCareer.Skills.Jumping, skillModAmount);
        }

        void InitMoveSoundTimer(float minTime = 4, float maxTime = 20)
        {
            moveSoundTimer = Random.Range(minTime, maxTime);
        }

        void PlayLycanthropeMoveSound()
        {
            // Get sound based on infection type
            SoundClips customSound = SoundClips.None;
            if (infectionType == LycanthropyTypes.Werewolf)
                customSound = SoundClips.EnemyWerewolfMove;
            else if (infectionType == LycanthropyTypes.Wereboar)
                customSound = SoundClips.EnemyWereboarMove;

            // Play sound through weapon
            FPSWeapon screenWeapon = GameManager.Instance.WeaponManager.ScreenWeapon;
            if (screenWeapon && customSound != SoundClips.None)
                screenWeapon.PlayAttackVoice(customSound);
        }

        void ForceTransformDuringFullMoon()
        {
            // Does not happen if Hircine's Ring equipped
            if (wearingHircineRing)
                return;

            // Player is forced into lycanthrope form every magic round for the whole duration of any full moon
            // In classic, player can switch back to humanoid form briefly (remainder of magic round) before being forced to shapechange again (next magic round)
            // Intentionally reproducing this handling here so player can shift back and forth to loot corpses, etc.
            // I'm not sure if this was intentional in classic or not, but it's not much fun to be shut out of the game for a
            // whole 24 hours every 15 days. I *think* this was intentional so lycanthrope players could at least struggle through.
            // Same goes for other weirdness like renting rooms or handing in quests. It's just not fun to shut out of game completely.
            // Ultimately the player has their own choice to do this or not. They can run free in the wilderness for 24 hours if they prefer.
            if (isFullMoon && !isTransformed)
            {
                string youDreamOfTheMoon = TextManager.Instance.GetLocalizedText("youDreamOfTheMoon");
                DaggerfallUI.AddHUDText(youDreamOfTheMoon, 2);
                MorphSelf(true);
            }
        }

        bool GetNeedToKill()
        {
            return !wearingHircineRing && TimeSinceLastInnocentKilled > needToKillPeriod;
        }

        bool CanCastMorphSelf()
        {
            return wearingHircineRing || DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() - lastCastMorphSelf > DaggerfallDateTime.MinutesPerDay;
        }

        void NotifyNeedToKill()
        {
            string youNeedToKill = TextManager.Instance.GetLocalizedText("youNeedToHuntTheInnocent");
            DaggerfallUI.AddHUDText(youNeedToKill, 2);
        }

        bool IsWearingHircineRing()
        {
            DaggerfallUnityItem[] equipTable = GameManager.Instance.PlayerEntity.ItemEquipTable.EquipTable;
            if (equipTable == null || equipTable.Length == 0)
                return false;

            return IsHircineRingItem(equipTable[(int)EquipSlots.Ring0]) || IsHircineRingItem(equipTable[(int)EquipSlots.Ring1]);
        }

        bool IsHircineRingItem(DaggerfallUnityItem item)
        {
            return
                item != null &&
                item.IsArtifact &&
                item.ContainsEnchantment(EnchantmentTypes.SpecialArtifactEffect, (short)ArtifactsSubTypes.Hircine_Ring);
        }

        void EndLycanthropyQuests()
        {
            // Just the one quest to end and should only be a single instance
            ulong[] quests = QuestMachine.Instance.FindQuests(cureQuestName);
            foreach (ulong id in quests)
            {
                Quest quest = QuestMachine.Instance.GetQuest(id);
                if (quest != null)
                    QuestMachine.Instance.TombstoneQuest(quest);
            }
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public RaceTemplate compoundRace;
            public LycanthropyTypes infectionType;
            public uint lastKilledInnocent;
            public uint lastCastMorphSelf;
            public bool wearingHircineRing;
            public bool isTransformed;
        }

        public override object GetSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.compoundRace = compoundRace;
            data.infectionType = infectionType;
            data.lastKilledInnocent = lastKilledInnocent;
            data.lastCastMorphSelf = lastCastMorphSelf;
            data.wearingHircineRing = wearingHircineRing;
            data.isTransformed = isTransformed;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            CustomSaveData_v1 data = (CustomSaveData_v1)dataIn;
            compoundRace = data.compoundRace;
            infectionType = data.infectionType;
            lastKilledInnocent = data.lastKilledInnocent;
            lastCastMorphSelf = data.lastCastMorphSelf;
            wearingHircineRing = data.wearingHircineRing;
            isTransformed = data.isTransformed;
        }

        #endregion

        #region Console Commands

        public static class LycanthropeConsoleCommands
        {
            public static void RegisterCommands()
            {
                try
                {
                    ConsoleCommandsDatabase.RegisterCommand(SateMe.name, SateMe.description, SateMe.usage, SateMe.Execute);
                    ConsoleCommandsDatabase.RegisterCommand(CureMe.name, CureMe.description, CureMe.usage, CureMe.Execute);
                }
                catch (System.Exception ex)
                {
                    DaggerfallUnity.LogMessage(ex.Message, true);
                }
            }

            private static class SateMe
            {
                public static readonly string name = "were_sateme";
                public static readonly string description = "Lycanthrope urge to kill becomes sated.";
                public static readonly string usage = "were_sateme";

                public static string Execute(params string[] args)
                {
                    if (GameManager.Instance.PlayerEffectManager.HasLycanthropy())
                    {
                        (GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect() as LycanthropyEffect).UpdateSatiation();
                        return "Your urge to kill has been sated.";
                    }
                    else
                        return "Player is not a werewolf/wereboar.";
                }
            }

            private static class CureMe
            {
                public static readonly string name = "were_cureme";
                public static readonly string description = "Player is cured of lycanthropy effect at start of next magic round (1 game minute).";
                public static readonly string usage = "were_cureme";

                public static string Execute(params string[] args)
                {
                    if (GameManager.Instance.PlayerEffectManager.HasLycanthropy())
                    {
                        GameManager.Instance.PlayerEffectManager.EndLycanthropy();
                        return "You have been cured of lycanthropy.";
                    }
                    else
                        return "Player is not a werewolf/wereboar.";
                }
            }
        }

        #endregion
    }
}