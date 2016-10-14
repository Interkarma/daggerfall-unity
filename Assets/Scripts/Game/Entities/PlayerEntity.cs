// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Implements DaggerfallEntity with properties specific to a Player.
    /// </summary>
    public class PlayerEntity : DaggerfallEntity
    {
        #region Fields

        const int testPlayerLevel = 1;
        const string testPlayerName = "Nameless";

        protected RaceTemplate raceTemplate;
        protected int faceIndex;
        protected PlayerReflexes reflexes;
        protected ItemCollection wagonItems = new ItemCollection();
        protected ItemCollection otherItems = new ItemCollection();
        protected int goldPieces = 0;

        #endregion

        #region Properties

        public Races Race { get { return (Races)RaceTemplate.ID; } }
        public RaceTemplate RaceTemplate { get { return raceTemplate; } set { raceTemplate = value; } }
        public int FaceIndex { get { return faceIndex; } set { faceIndex = value; } }
        public PlayerReflexes Reflexes { get { return reflexes; } set { reflexes = value; } }
        public ItemCollection WagonItems { get { return wagonItems; } set { wagonItems.ReplaceAll(value); } }
        public ItemCollection OtherItems { get { return otherItems; } set { otherItems.ReplaceAll(value); } }
        public int GoldPieces { get { return goldPieces; } set { goldPieces = value; } }

        #endregion

        #region Constructors

        public PlayerEntity()
            :base()
        {
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets entity to initial state.
        /// </summary>
        public void Reset()
        {
            equipTable.Clear();
            items.Clear();
            wagonItems.Clear();
            otherItems.Clear();
            SetEntityDefaults();
            goldPieces = 0;
        }

        /// <summary>
        /// Assigns player entity settings from a character document.
        /// </summary>
        public void AssignCharacter(CharacterDocument character, int level = 1, int maxHealth = 0)
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

            if (maxHealth <= 0)
                this.maxHealth = FormulaHelper.RollMaxHealth(level, stats.Endurance, career.HitPointsPerLevelOrMonsterLevel);
            else
                this.maxHealth = maxHealth;

            FillVitalSigns();

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

                // Add to local inventory or wagon
                DaggerfallUnityItem newItem = new DaggerfallUnityItem((ItemRecord)record);
                if (containerRecord.IsWagon)
                    wagonItems.AddItem(newItem);
                else
                    items.AddItem(newItem);

                // Equip to player if equipped in save
                for (int i = 0; i < characterRecord.ParsedData.equippedItems.Length; i++)
                {
                    if (characterRecord.ParsedData.equippedItems[i] == (record.RecordRoot.RecordID >> 8))
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
                stats.SetFromCareer(career);
                level = testPlayerLevel;
                maxHealth = FormulaHelper.RollMaxHealth(level, stats.Endurance, career.HitPointsPerLevelOrMonsterLevel);
                name = testPlayerName;
                stats.SetDefaults();
                skills.SetDefaults();
                FillVitalSigns();
            }
        }

        #endregion

        #region Event Handlers

        private void StartGameBehaviour_OnNewGame()
        {
            Reset();
        }

        #endregion
    }
}