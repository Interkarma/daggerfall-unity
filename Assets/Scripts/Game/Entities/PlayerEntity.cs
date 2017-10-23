// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using UnityEngine;
using System;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Implements DaggerfallEntity with properties specific to a Player.
    /// </summary>
    public class PlayerEntity : DaggerfallEntity
    {
        #region Fields

        bool godMode = false;

        const int testPlayerLevel = 1;
        const string testPlayerName = "Nameless";

        protected RaceTemplate raceTemplate;
        protected int faceIndex;
        protected PlayerReflexes reflexes;
        protected ItemCollection wagonItems = new ItemCollection();
        protected ItemCollection otherItems = new ItemCollection();
        protected int goldPieces = 0;
        protected PersistentFactionData factionData = new PersistentFactionData();
        protected PersistentGlobalVars globalVars = new PersistentGlobalVars();

        protected short[] skillUses;
        protected uint timeOfLastSkillIncreaseCheck = 0;
        protected uint timeOfLastSkillTraining = 0;

        protected int startingLevelUpSkillSum = 0;
        protected int currentLevelUpSkillSum = 0;
        protected bool readyToLevelUp = false;

        protected short[] sGroupReputations = new short[5];

        protected int biographyResistDiseaseMod = 0;
        protected int biographyResistMagicMod = 0;
        protected int biographyAvoidHitMod = 0;
        protected int biographyResistPoisonMod = 0;
        protected int biographyFatigueMod = 0;
        protected int biographyReactionMod = 0;

        protected uint timeForThievesGuildLetter = 0;
        protected uint timeForDarkBrotherhoodLetter = 0;
        protected int thievesGuildRequirementTally = 0;
        protected int darkBrotherhoodRequirementTally = 0;

        protected RegionDataRecord[] regionData = new RegionDataRecord[62];

        // Fatigue loss per in-game minute
        public const int DefaultFatigueLoss = 11;
        //public const int ClimbingFatigueLoss = 22;
        public const int RunningFatigueLoss = 88;
        //public const int SwimmingFatigueLoss = 44;

        private float runningTallyTimer = 0f;
        private float runningTallyInterval = 0.0625f; // Tally every 1/16 second of running. The rate at which the running skill
                                                      // is tallied in classic varies by how fast the game is being processed.
                                                      // Based on some tests, around every 1/16 of a second is a typical rate.

        private int JumpingFatigueLoss = 11;        // According to DF Chronicles and verified in classic
        private bool CheckedCurrentJump = false;

        PlayerMotor playerMotor = null;
        protected uint lastGameMinutes = 0;         // Being tracked in order to perform updates based on changes in the current game minute
        private bool gameStarted = false;

        #endregion

        #region Properties

        public bool GodMode { get { return godMode; } set { godMode = value; } }
        public Races Race { get { return (Races)RaceTemplate.ID; } }
        public RaceTemplate RaceTemplate { get { return raceTemplate; } set { raceTemplate = value; } }
        public int FaceIndex { get { return faceIndex; } set { faceIndex = value; } }
        public PlayerReflexes Reflexes { get { return reflexes; } set { reflexes = value; } }
        public ItemCollection WagonItems { get { return wagonItems; } set { wagonItems.ReplaceAll(value); } }
        public ItemCollection OtherItems { get { return otherItems; } set { otherItems.ReplaceAll(value); } }
        public int GoldPieces { get { return goldPieces; } set { goldPieces = value; } }
        public PersistentFactionData FactionData { get { return factionData; } }
        public PersistentGlobalVars GlobalVars { get { return globalVars; } }
        public short[] SkillUses { get { return skillUses; } set { skillUses = value; } }
        public uint TimeOfLastSkillIncreaseCheck { get { return timeOfLastSkillIncreaseCheck; } set { timeOfLastSkillIncreaseCheck = value; } }
        public uint TimeOfLastSkillTraining { get { return timeOfLastSkillTraining; } set { timeOfLastSkillTraining = value; } }
        public int StartingLevelUpSkillSum { get { return startingLevelUpSkillSum; } set { startingLevelUpSkillSum = value; } }
        public int CurrentLevelUpSkillSum {  get { return currentLevelUpSkillSum; } }
        public bool ReadyToLevelUp { get { return readyToLevelUp; } set { readyToLevelUp = value; } }
        public short[] SGroupReputations { get { return sGroupReputations; } set { sGroupReputations = value; } }
        public int BiographyResistDiseaseMod { get { return biographyResistDiseaseMod; } set { biographyResistDiseaseMod = value; } }
        public int BiographyResistMagicMod { get { return biographyResistMagicMod; } set { biographyResistMagicMod = value; } }
        public int BiographyAvoidHitMod { get { return biographyAvoidHitMod; } set { biographyAvoidHitMod = value; } }
        public int BiographyResistPoisonMod { get { return biographyResistPoisonMod; } set { biographyResistPoisonMod = value; } }
        public int BiographyFatigueMod { get { return biographyFatigueMod; } set { biographyFatigueMod = value; } }
        public int BiographyReactionMod { get { return biographyReactionMod; } set { biographyReactionMod = value; } }
        public uint TimeForThievesGuildLetter { get { return timeForThievesGuildLetter; } set { timeForThievesGuildLetter = value; } }
        public uint TimeForDarkBrotherhoodLetter { get { return timeForDarkBrotherhoodLetter; } set { timeForDarkBrotherhoodLetter = value; } }
        public int ThievesGuildRequirementTally { get { return thievesGuildRequirementTally; } set { thievesGuildRequirementTally = value; } }
        public int DarkBrotherhoodRequirementTally { get { return darkBrotherhoodRequirementTally; } set { darkBrotherhoodRequirementTally = value; } }
        public float CarriedWeight { get { return Items.GetWeight() + ((float)goldPieces / DaggerfallBankManager.gold1kg); } }
        public float WagonWeight { get { return WagonItems.GetWeight(); } }
        public RegionDataRecord[] RegionData { get { return regionData; } set { regionData = value; } }
        public uint LastGameMinutes { get { return lastGameMinutes; } set { lastGameMinutes = value; } }

        #endregion

        #region Constructors

        public PlayerEntity()
            :base()
        {
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
            OnExhausted += PlayerEntity_OnExhausted;
        }

        #endregion

        #region Public Methods

        public override void Update(DaggerfallEntityBehaviour sender)
        {
            if (playerMotor == null)
                playerMotor = GameManager.Instance.PlayerMotor;

            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            // Wait until game has started and the game time has been set.
            // If the game time is taken before then "30" is returned, which causes an initial player fatigue loss
            // after loading or starting a game with a non-30 minute.
            if (!gameStarted && !GameManager.Instance.StateManager.GameInProgress)
                return;
            else if (!gameStarted)
                gameStarted = true;
            if (playerMotor != null)
            {
                // Tally running skill
                if (playerMotor.IsRunning && !playerMotor.IsRiding)
                {
                    if (runningTallyTimer < runningTallyInterval)
                        runningTallyTimer += Time.deltaTime;
                    else
                    {
                        TallySkill(DFCareer.Skills.Running, 1);
                        runningTallyTimer = 0f;
                    }
                }

                // Every game minute, apply fatigue loss to the player
                if (lastGameMinutes != gameMinutes)
                {
                    if (playerMotor.IsRunning)
                        DecreaseFatigue(RunningFatigueLoss);
                    else
                        DecreaseFatigue(DefaultFatigueLoss);
                }
                // Reduce fatigue when jumping and tally jumping skill
                if (!CheckedCurrentJump && playerMotor.IsJumping)
                {
                    DecreaseFatigue(JumpingFatigueLoss);
                    TallySkill(DFCareer.Skills.Jumping, 1);
                    CheckedCurrentJump = true;
                }

                // Reset jump fatigue check when grounded
                if (CheckedCurrentJump && !playerMotor.IsJumping)
                {
                    CheckedCurrentJump = false;
                }
            }

            // Adjust regional prices each time a day passes.
            uint lastDay = lastGameMinutes / 1440;
            uint currentDay = gameMinutes / 1440;
            int daysPast = (int)(currentDay - lastDay);

            if (daysPast > 0)
                FormulaHelper.ModifyPriceAdjustmentByRegion(ref regionData, daysPast);

            lastGameMinutes = gameMinutes;

            //HandleStartingCrimeGuildQuests(Entity as PlayerEntity);
        }

        /// <summary>
        /// Resets entity to initial state.
        /// </summary>
        public void Reset()
        {
            equipTable.Clear();
            items.Clear();
            wagonItems.Clear();
            otherItems.Clear();
            factionData.Reset();
            globalVars.Reset();
            SetEntityDefaults();
            startingLevelUpSkillSum = 0;
            currentLevelUpSkillSum = 0;
            goldPieces = 0;
            timeOfLastSkillIncreaseCheck = 0;
            timeOfLastSkillTraining = 0;
            if (skillUses != null)
                System.Array.Clear(skillUses, 0, skillUses.Length);
        }

        /// <summary>
        /// Assigns player entity settings from a character document.
        /// </summary>
        public void AssignCharacter(CharacterDocument character, int level = 1, int maxHealth = 0, bool fillVitals = true)
        {
            if (character == null)
            {
                SetEntityDefaults();
                return;
            }

            this.level = level;
            this.gender = character.gender;
            this.raceTemplate = character.raceTemplate;
            this.career = character.career;
            this.name = character.name;
            this.faceIndex = character.faceIndex;
            this.stats = character.workingStats;
            this.skills = character.workingSkills;
            this.reflexes = character.reflexes;
            this.maxHealth = character.maxHealth;
            this.currentHealth = character.currentHealth;
            this.currentMagicka = character.currentSpellPoints;
            this.sGroupReputations[0] = character.reputationCommoners;
            this.sGroupReputations[1] = character.reputationMerchants;
            this.sGroupReputations[2] = character.reputationNobility;
            this.sGroupReputations[3] = character.reputationScholars;
            this.sGroupReputations[4] = character.reputationUnderworld;
            this.currentFatigue = character.currentFatigue;
            this.skillUses = character.skillUses;
            this.startingLevelUpSkillSum = character.startingLevelUpSkillSum;
            this.minMetalToHit = (WeaponMaterialTypes)character.minMetalToHit;
            this.armorValues = character.armorValues;
            this.timeOfLastSkillTraining = character.lastTimePlayerBoughtTraining;
            this.timeForThievesGuildLetter = character.timeForThievesGuildLetter;
            this.timeForDarkBrotherhoodLetter = character.timeForDarkBrotherhoodLetter;
            this.darkBrotherhoodRequirementTally = character.darkBrotherhoodRequirementTally;
            this.thievesGuildRequirementTally = character.thievesGuildRequirementTally;

            SetCurrentLevelUpSkillSum();

            if (startingLevelUpSkillSum <= 0) // new character
            {
                startingLevelUpSkillSum = currentLevelUpSkillSum;
            }

            if (maxHealth <= 0)
                this.maxHealth = FormulaHelper.RollMaxHealth(level, career.HitPointsPerLevel);
            else
                this.maxHealth = maxHealth;

            if (fillVitals)
                FillVitalSigns();

            timeOfLastSkillIncreaseCheck = DaggerfallUnity.Instance.WorldTime.Now.ToClassicDaggerfallTime();

            DaggerfallUnity.LogMessage("Assigned character " + this.name, true);
        }

        /// <summary>
        /// Assigns character items from classic save tree.
        /// </summary>
        public void AssignItems(SaveTree saveTree)
        {
            // Find character record, should always be a singleton
            CharacterRecord characterRecord = (CharacterRecord)saveTree.FindRecord(RecordTypes.Character);
            if (characterRecord == null)
                return;

            // Find all character-owned items
            List<SaveTreeBaseRecord> itemRecords = saveTree.FindRecords(RecordTypes.Item, characterRecord);

            // Filter for container-based inventory items
            List<SaveTreeBaseRecord> filteredRecords = saveTree.FilterRecordsByParentType(itemRecords, RecordTypes.Container);

            // Add interim Daggerfall Unity items
            foreach (var record in filteredRecords)
            {
                // Get container parent
                ContainerRecord containerRecord = (ContainerRecord)record.Parent;

                // Create item, grabbing trapped soul if needed
                DaggerfallUnityItem newItem = new DaggerfallUnityItem((ItemRecord)record);
                if (newItem.ItemGroup == ItemGroups.MiscItems && newItem.GroupIndex == 1)
                {
                    if (record.Children.Count > 0)
                    {
                        TrappedSoulRecord soulRecord = (TrappedSoulRecord) record.Children[0];
                        newItem.TrappedSoulType = (MobileTypes) soulRecord.RecordRoot.SpriteIndex;
                    }
                    else
                        newItem.TrappedSoulType = MobileTypes.None;
                }
                // Add to local inventory or wagon
                if (containerRecord.IsWagon)
                    wagonItems.AddItem(newItem);
                else
                    items.AddItem(newItem);

                // Equip to player if equipped in save
                for (int i = 0; i < characterRecord.ParsedData.equippedItems.Length; i++)
                {
                    if (characterRecord.ParsedData.equippedItems[i] == record.RecordRoot.RecordID)
                        equipTable.EquipItem(newItem, true, false);
                }
            }
        }

        /// <summary>
        /// Assigns default entity settings.
        /// </summary>
        public override void SetEntityDefaults()
        {
            // TODO: Add some bonus points to stats
            career = DaggerfallEntity.GetClassCareerTemplate(ClassCareers.Mage);
            if (career != null)
            {
                raceTemplate = CharacterDocument.GetRaceTemplate(Races.Breton);
                faceIndex = 0;
                reflexes = PlayerReflexes.Average;
                gender = Genders.Male;
                stats.SetPermanentFromCareer(career);
                level = testPlayerLevel;
                maxHealth = FormulaHelper.RollMaxHealth(level, career.HitPointsPerLevel);
                name = testPlayerName;
                stats.SetDefaults();
                skills.SetDefaults();
                FillVitalSigns();
                for (int i = 0; i < ArmorValues.Length; i++)
                {
                    ArmorValues[i] = 100;
                }
            }
        }

        /// <summary>
        /// Sets new health value.
        /// Override for godmode support.
        /// </summary>
        public override int SetHealth(int amount)
        {
            if (godMode)
                return currentHealth = MaxHealth;
            else
                return base.SetHealth(amount);
        }

        /// <summary>
        /// Sets new fatigue value.
        /// Override for godmode support.
        /// </summary>
        public override int SetFatigue(int amount)
        {
            if (godMode)
                return currentFatigue = MaxFatigue;
            else
                return base.SetFatigue(amount);
        }

        /// <summary>
        /// Sets new magicka value.
        /// Override for godmode support.
        /// </summary>
        public override int SetMagicka(int amount)
        {
            if (godMode)
                return currentMagicka = MaxMagicka;
            else
                return base.SetMagicka(amount);
        }

        /// <summary>
        /// Tally skill usage.
        /// </summary>
        public override void TallySkill(DFCareer.Skills skill, short amount)
        {
            int skillId = (int)skill;

            try
            {
                skillUses[skillId] += amount;
                if (skillUses[skillId] > 20000)
                    skillUses[skillId] = 20000;
                else if (skillUses[skillId] < 0)
                {
                    skillUses[skillId] = 0;
                }
            }
            catch(Exception ex)
            {
                string error = string.Format("Caught exception {0} with skillId {1}.", ex.Message, skillId);

                if (skillUses == null || skillUses.Length == 0)
                    error += " skillUses is null or empty.";

                Debug.Log(error);
            }
        }

        /// <summary>
        /// Tally thefts/break-ins and murders for starting Thieves Guild and Dark Brotherhood quests
        /// </summary>
        public void TallyCrimeGuildRequirements(bool thievingCrime, byte amount)
        {
            if (thievingCrime)
            {
                if (timeForThievesGuildLetter == 0)
                {
                    if (thievesGuildRequirementTally != 100) // Tally is set to 100 when the thieves guild quest line has already started
                    {
                        thievesGuildRequirementTally += amount;
                        if (thievesGuildRequirementTally >= 10)
                        {
                            uint currentMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                            timeForThievesGuildLetter = currentMinutes + 4320; // 3 days
                       }
                    }
                }
            }
            else // murder
            {
                if (timeForDarkBrotherhoodLetter == 0)
                {
                    if (darkBrotherhoodRequirementTally != 100) // Tally is set to 100 when the thieves guild quest line has already started
                    {
                        darkBrotherhoodRequirementTally += amount;
                        if (darkBrotherhoodRequirementTally >= 15)
                        {
                            uint currentMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                            timeForDarkBrotherhoodLetter = currentMinutes + 4320; // 3 days
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the amount of gold carried by the player. (gold pieces + letters of credit)
        /// </summary>
        public int GetGoldAmount()
        {
            return goldPieces + items.GetCreditAmount();
        }

        /// <summary>
        /// Deducts the amount of gold specified from the player. (check GoldAmount first)
        /// Gold pieces first if enough, then letters of credit, then gold pieces.
        /// </summary>
        /// <param name="amount">Amount to deduct</param>
        /// <returns>Amount remaining to be paid if not enough funds.</returns>
        public int DeductGoldAmount(int amount)
        {
            if (amount <= goldPieces) {
                goldPieces -= amount;
            } else {
                while (amount > 0)
                {
                    DaggerfallUnityItem loc = items.GetItem(ItemGroups.MiscItems, (int)MiscItems.Letter_of_credit);
                    if (loc == null) {
                        break;
                    } else if (amount <= loc.value) {
                        loc.value -= amount;
                        amount = 0;
                    } else {
                        amount -= loc.value;
                        items.RemoveItem(loc);
                    }
                }

                if (amount > 0) {
                    if (amount <= goldPieces) {
                        goldPieces -= amount;
                    } else {     // Underpaid.
                        amount -= goldPieces;
                        goldPieces = 0;
                        return amount;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Raise skills if conditions are met.
        /// </summary>
        public void RaiseSkills()
        {
            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
            if ((now.ToClassicDaggerfallTime() - timeOfLastSkillIncreaseCheck) <= 360)
                return;

            timeOfLastSkillIncreaseCheck = now.ToClassicDaggerfallTime();

            for (short i = 0; i < skillUses.Length; i++)
            {
                int skillAdvancementMultiplier = DaggerfallSkills.GetAdvancementMultiplier((DFCareer.Skills)i);
                float careerAdvancementMultiplier = Career.AdvancementMultiplier;
                int usesNeededForAdvancement = FormulaHelper.CalculateSkillUsesForAdvancement(skills.GetPermanentSkillValue(i), skillAdvancementMultiplier, careerAdvancementMultiplier, level);
                if (skillUses[i] >= usesNeededForAdvancement)
                {
                    skillUses[i] = 0;
                    skills.SetPermanentSkillValue(i, (short)(skills.GetPermanentSkillValue(i) + 1));
                    SetCurrentLevelUpSkillSum();
                    DaggerfallUI.Instance.PopupMessage(HardStrings.skillImprove.Replace("%s", DaggerfallUnity.Instance.TextProvider.GetSkillName((DaggerfallConnect.DFCareer.Skills)i)));
                }
            }

            if (CheckForLevelUp())
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenCharacterSheetWindow);
        }

        /// <summary>
        /// Calculate current sum of skills used for determining player level.
        /// </summary>
        public void SetCurrentLevelUpSkillSum()
        {
            short sum = 0;
            short lowestMajorSkillValue = 0;
            short highestMinorSkillValue = 0;
            List<DaggerfallConnect.DFCareer.Skills> primarySkills = GetPrimarySkills();
            List<DaggerfallConnect.DFCareer.Skills> majorSkills = GetMajorSkills();
            List<DaggerfallConnect.DFCareer.Skills> minorSkills = GetMinorSkills();
            for (int i = 0; i < primarySkills.Count; i++)
            {
                sum += skills.GetPermanentSkillValue(primarySkills[i]);
            }

            for (int i = 0; i < majorSkills.Count; i++)
            {
                short value = skills.GetPermanentSkillValue(majorSkills[i]);
                sum += value;
                if (i == 0)
                    lowestMajorSkillValue = value;
                else if (value < lowestMajorSkillValue)
                    lowestMajorSkillValue = value;
            }

            sum -= lowestMajorSkillValue;

            for (int i = 0; i < minorSkills.Count; i++)
            {
                short value = skills.GetPermanentSkillValue(minorSkills[i]);
                if (i == 0)
                    highestMinorSkillValue = value;
                else if (value > highestMinorSkillValue)
                    highestMinorSkillValue = value;
            }

            sum += highestMinorSkillValue;
            currentLevelUpSkillSum = sum;
        }

        /// <summary>
        /// Estimate the starting sum of skills used for determining player level.
        /// </summary>
        public void EstimateStartingLevelUpSkillSum()
        {
            float estimatedLevel = level + 0.5f; // Assume the player is halfway through advancing a level
            int estimatedSum = (int)((estimatedLevel * 15) - (28 + currentLevelUpSkillSum)) * -1;
            if ((currentLevelUpSkillSum > 0) && (startingLevelUpSkillSum > currentLevelUpSkillSum))
                startingLevelUpSkillSum = currentLevelUpSkillSum;
            else
                startingLevelUpSkillSum = estimatedSum;
        }

        /// <summary>
        /// Calculate level and check for level up.
        /// </summary>
        public bool CheckForLevelUp()
        {
            int calculatedLevel = FormulaHelper.CalculatePlayerLevel(startingLevelUpSkillSum, currentLevelUpSkillSum);
            bool levelUp = (level < calculatedLevel);
            if (levelUp)
                readyToLevelUp = true;

            return levelUp;
        }

        void HandleStartingCrimeGuildQuests()
        {
            if (thievesGuildRequirementTally != 100
                && timeForThievesGuildLetter > 0
                && timeForThievesGuildLetter < DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                && !GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>().IsPlayerInside)
            {
                thievesGuildRequirementTally = 100;
                timeForThievesGuildLetter = 0;
                Questing.QuestMachine.Instance.InstantiateQuest("O0A0AL00");
            }
            if (darkBrotherhoodRequirementTally != 100
                && timeForDarkBrotherhoodLetter > 0
                && timeForDarkBrotherhoodLetter < DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                && !GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>().IsPlayerInside)
            {
                darkBrotherhoodRequirementTally = 100;
                timeForDarkBrotherhoodLetter = 0;
                Questing.QuestMachine.Instance.InstantiateQuest("L0A01L00");
            }
        }

        /// <summary>
        /// Releases a quest item carried by player so it can be assigned back again by quest script.
        /// This ensures item is properly unequipped and optionally makes permanent.
        /// </summary>
        /// <param name="item">Item to release.</param>
        /// <param name="makePermanent">True to make item permanent.</param>
        public void ReleaseQuestItemForReoffer(DaggerfallUnityItem item, bool makePermanent = false)
        {
            if (item == null)
                return;

            // Unequip item if player is wearing it
            if (GameManager.Instance.PlayerEntity.ItemEquipTable.UnequipItem(item))
            {
                // If item was actually unequipped then update armour values
                GameManager.Instance.PlayerEntity.UpdateEquippedArmorValues(item, false);
            }

            // Remove quest from inventory so it can be offered back to player
            GameManager.Instance.PlayerEntity.Items.RemoveItem(item);

            // Optionally make permanent
            if (makePermanent)
                item.MakePermanent();
        }

        #endregion

        #region RegionData
        public struct RegionDataRecord // 80 bytes long
        {
            public byte[] Values; // 29 bytes long
            public bool[] Flags; // 29 bytes long
            public bool[] Flags2; // 14 bytes long
            // bytes 72 to 74 unknown
            public short LegalRep; // bytes 74 to 76
            public ushort Unknown; // bytes 76 to 78
            public ushort PriceAdjustment; // bytes 78 to 80
        }

        public void InitializeRegionPrices()
        {
            FormulaHelper.RandomizeInitialPriceAdjustments(ref regionData);
        }

        #endregion

        #region Event Handlers

        private void StartGameBehaviour_OnNewGame()
        {
            Reset();
        }

        private void PlayerEntity_OnExhausted(DaggerfallEntity entity)
        {
            const int youDropToTheGround1 = 1071;
            const int youDropToTheGround2 = 1072;

            bool enemiesNearby = GameManager.Instance.AreEnemiesNearby();

            ITextProvider textProvider = DaggerfallUnity.Instance.TextProvider;
            TextFile.Token[] tokens;

            GameManager.Instance.PlayerMotor.CancelMovement = true;

            if (!enemiesNearby)
                tokens = textProvider.GetRSCTokens(youDropToTheGround1);
            else
                tokens = textProvider.GetRSCTokens(youDropToTheGround2);

            if (tokens != null && tokens.Length > 0)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
                messageBox.SetTextTokens(tokens);
                messageBox.ClickAnywhereToClose = true;
                messageBox.ParentPanel.BackgroundColor = Color.clear;
                messageBox.Show();
            }

            if (!enemiesNearby)
            {
                // TODO: Duplicates rest code in rest window. Should be unified.
                DaggerfallUnity.Instance.WorldTime.Now.RaiseTime(1 * DaggerfallDateTime.SecondsPerHour);
                int healthRecoveryRate = FormulaHelper.CalculateHealthRecoveryRate(this);
                int fatigueRecoveryRate = FormulaHelper.CalculateFatigueRecoveryRate(MaxFatigue);
                int spellPointRecoveryRate = FormulaHelper.CalculateSpellPointRecoveryRate(MaxMagicka);

                CurrentHealth += healthRecoveryRate;
                CurrentFatigue += fatigueRecoveryRate;
                CurrentMagicka += spellPointRecoveryRate;

                TallySkill(DFCareer.Skills.Medical, 1);
            }
            else
                SetHealth(0);
        }

        #endregion
    }
}