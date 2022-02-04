// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
// 
// Notes:
//

using UnityEngine;
using System;
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
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Implements DaggerfallEntity with properties specific to a Player.
    /// </summary>
    public class PlayerEntity : DaggerfallEntity
    {
        #region Fields

        public const string vampireSpellTag = "vampire";
        public const string lycanthropySpellTag = "lycanthrope";

        bool godMode = false;
        bool noTargetMode = false;
        bool preventEnemySpawns = false;
        bool preventNormalizingReputations = false;
        bool isResting = false;

        //bool hasStartedInitialVampireQuest = false;
        //byte vampireClan = 0;
        //uint lastTimeVampireNeedToKillSatiated = 0;

        const int testPlayerLevel = 1;
        const string testPlayerName = "Nameless";

        protected RaceTemplate raceTemplate;
        protected int faceIndex;
        protected PlayerReflexes reflexes;
        protected ItemCollection wagonItems = new ItemCollection();
        protected ItemCollection otherItems = new ItemCollection();
        protected DaggerfallUnityItem lightSource;
        protected int goldPieces = 0;
        protected PersistentFactionData factionData = new PersistentFactionData();
        protected PersistentGlobalVars globalVars = new PersistentGlobalVars();
        protected PlayerNotebook notebook = new PlayerNotebook();

        protected short[] skillUses;
        protected uint[] skillsRecentlyRaised = new uint[2];
        protected uint timeOfLastSkillIncreaseCheck = 0;
        protected uint timeOfLastSkillTraining = 0;

        protected uint timeOfLastStealthCheck = 0;

        protected int startingLevelUpSkillSum = 0;
        protected int currentLevelUpSkillSum = 0;
        protected bool readyToLevelUp = false;
        protected bool oghmaLevelUp = false;

        protected short[] sGroupReputations = new short[11];

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
        private const int InviteSent = 100;

        protected uint timeToBecomeVampireOrWerebeast = 0;
        protected uint lastTimePlayerAteOrDrankAtTavern = 0;

        protected RegionDataRecord[] regionData = new RegionDataRecord[62];

        protected Crimes crimeCommitted = 0;
        protected bool haveShownSurrenderToGuardsDialogue = false;
        protected bool arrested = false;
        protected short halfOfLegalRepPlayerLostFromCrime = 0;

        private List<RoomRental_v1> rentedRooms = new List<RoomRental_v1>();

        // Fatigue loss per in-game minute
        public const int DefaultFatigueLoss = 11;
        public const int ClimbingFatigueLoss = 22;
        public const int RunningFatigueLoss = 88;
        public const int SwimmingFatigueLoss = 44;
        public const int JumpingFatigueLoss = 11;

        private int breathUpdateTally = 0;
        private int runningTallyCounter = 0;
        private float guardsArriveCountdown = 0;
        DaggerfallLocation guardsArriveCountdownLocation;

        private bool CheckedCurrentJump = false;

        PlayerMotor playerMotor = null;
        ClimbingMotor climbingMotor = null;
        protected uint lastGameMinutes = 0;         // Being tracked in order to perform updates based on changes in the current game minute
        private bool gameStarted = false;
        bool displayingExhaustedPopup = false;

        const int socialGroupCount = 11;
        int[] reactionMods = new int[socialGroupCount];     // Indices map to FactionFile.SocialGroups 0-10 - do not serialize, set by live effects

        bool enemyAlertActive = false;
        uint lastEnemyAlertTime;

        // Player-only constant effects
        // Note: These properties are intentionally not serialized. They should only be set by live effects.
        public bool IsAzurasStarEquipped { get; set; }

        #endregion

        #region Properties

        public bool GodMode { get { return godMode; } set { godMode = value; } }
        public bool NoTargetMode { get { return noTargetMode; } set { noTargetMode = value; } }
        public bool PreventEnemySpawns { get { return preventEnemySpawns; } set { preventEnemySpawns = value; } }
        public bool PreventNormalizingReputations { get { return preventNormalizingReputations; } set { preventNormalizingReputations = value; } }
        public bool IsResting { get { return isResting; } set { isResting = value; } }
        public bool IsLoitering { get; set; }
        public DaggerfallRestWindow.RestModes CurrentRestMode { get; set; }
        public Races Race { get { return (Races)RaceTemplate.ID; } }
        public RaceTemplate RaceTemplate { get { return GetLiveRaceTemplate(); } }
        public RaceTemplate BirthRaceTemplate { get { return raceTemplate; } set { raceTemplate = value; } }
        public int FaceIndex { get { return faceIndex; } set { faceIndex = value; } }
        public PlayerReflexes Reflexes { get { return reflexes; } set { reflexes = value; } }
        public ItemCollection WagonItems { get { return wagonItems; } set { wagonItems.ReplaceAll(value); } }
        public ItemCollection OtherItems { get { return otherItems; } set { otherItems.ReplaceAll(value); } }
        public DaggerfallUnityItem LightSource { get { return lightSource; } set { lightSource = value; } }
        public int GoldPieces { get { return goldPieces; } set { goldPieces = value; } }
        public PersistentFactionData FactionData { get { return factionData; } }
        public PersistentGlobalVars GlobalVars { get { return globalVars; } }
        public PlayerNotebook Notebook { get { return notebook; } }
        public short[] SkillUses { get { return skillUses; } set { skillUses = value; } }
        public uint[] SkillsRecentlyRaised { get { return skillsRecentlyRaised; } set { skillsRecentlyRaised = value; } }
        public uint TimeOfLastSkillIncreaseCheck { get { return timeOfLastSkillIncreaseCheck; } set { timeOfLastSkillIncreaseCheck = value; } }
        public uint TimeOfLastSkillTraining { get { return timeOfLastSkillTraining; } set { timeOfLastSkillTraining = value; } }
        public uint TimeOfLastStealthCheck { get { return timeOfLastStealthCheck; } set { timeOfLastStealthCheck = value; } }
        public int StartingLevelUpSkillSum { get { return startingLevelUpSkillSum; } set { startingLevelUpSkillSum = value; } }
        public int CurrentLevelUpSkillSum { get { return currentLevelUpSkillSum; } internal set { currentLevelUpSkillSum = value; } }
        public bool ReadyToLevelUp { get { return readyToLevelUp; } set { readyToLevelUp = value; } }
        public bool OghmaLevelUp { get { return oghmaLevelUp; } set { oghmaLevelUp = value; } }
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
        public uint TimeToBecomeVampireOrWerebeast { get { return timeToBecomeVampireOrWerebeast; } set { timeToBecomeVampireOrWerebeast = value; } }
        public uint LastTimePlayerAteOrDrankAtTavern { get { return lastTimePlayerAteOrDrankAtTavern; } set { lastTimePlayerAteOrDrankAtTavern = value; } }
        public float CarriedWeight { get { return Items.GetWeight() + (goldPieces * DaggerfallBankManager.goldUnitWeightInKg); } }
        public float WagonWeight { get { return WagonItems.GetWeight(); } }
        public RegionDataRecord[] RegionData { get { return regionData; } set { regionData = value; } }
        public uint LastGameMinutes { get { return lastGameMinutes; } set { lastGameMinutes = value; } }
        public List<RoomRental_v1> RentedRooms { get { return rentedRooms; } set { rentedRooms = value; } }
        public Crimes CrimeCommitted { get { return crimeCommitted; } set { SetCrimeCommitted(value); } }
        public bool HaveShownSurrenderToGuardsDialogue { get { return haveShownSurrenderToGuardsDialogue; } set { haveShownSurrenderToGuardsDialogue = value; } }
        public bool Arrested { get { return arrested; } set { arrested = value; } }
        public bool InPrison { get ; set ; }
        public bool IsInBeastForm { get; set; }
        public List<string> BackStory { get; set; }
        public VampireClans PreviousVampireClan { get; set; }
        public bool EnemyAlertActive { get { return enemyAlertActive; } }
        public int DaedraSummonDay { get; set; }
        public int DaedraSummonIndex { get; set; }

        #endregion

        #region Constructors

        public PlayerEntity(DaggerfallEntityBehaviour entityBehaviour)
            : base(entityBehaviour)
        {
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
            OnExhausted += PlayerEntity_OnExhausted;
            PlayerGPS.OnExitLocationRect += PlayerGPS_OnExitLocationRect;
            DaggerfallTravelPopUp.OnPostFastTravel += DaggerfallTravelPopUp_OnPostFastTravel;
        }

        #endregion

        #region Public Methods

        public bool GetSkillRecentlyIncreased(DFCareer.Skills skill)
        {
            return (skillsRecentlyRaised[(int)skill / 32] & (1 << ((int)skill % 32))) != 0;
        }

        public void SetSkillRecentlyIncreased(int index)
        {
            skillsRecentlyRaised[index / 32] |= (uint)(1 << (index % 32));
        }

        public void ResetSkillsRecentlyRaised()
        {
            Array.Clear(skillsRecentlyRaised, 0, 2);
        }

        public RaceTemplate GetLiveRaceTemplate()
        {
            // Look for racial override effect
            RacialOverrideEffect racialOverrideEffect = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverrideEffect != null)
                return racialOverrideEffect.CustomRace;
            else
                return raceTemplate;
        }

        public RoomRental_v1 GetRentedRoom(int mapId, int buildingKey)
        {
            foreach (RoomRental_v1 room in rentedRooms)
                if (room.mapID == mapId && room.buildingKey == buildingKey)
                    return room;

            return null;
        }

        public List<RoomRental_v1> GetRentedRooms(int mapId)
        {
            return rentedRooms.FindAll(r => r.mapID == mapId);
        }

        public void RemoveExpiredRentedRooms()
        {
            rentedRooms.RemoveAll(r => {
                if (GetRemainingHours(r) < 1) {
                    SaveLoadManager.StateManager.RemovePermanentScene(DaggerfallInterior.GetSceneName(r.mapID, r.buildingKey));
                    return true;
                } else
                    return false;
            });
        }

        public static int GetRemainingHours(RoomRental_v1 room)
        {
            if (room == null)
                return -1;

            double remainingSecs = (double)(room.expiryTime - DaggerfallUnity.Instance.WorldTime.Now.ToSeconds());
            return (int)Math.Ceiling((remainingSecs / DaggerfallDateTime.SecondsPerHour));
        }

        public void ChangeReactionMod(FactionFile.SocialGroups socialGroup, int amount)
        {
            int index = (int)socialGroup;
            if (index >= 0 && index < reactionMods.Length)
                reactionMods[index] += amount;
        }

        public int GetReactionMod(FactionFile.SocialGroups socialGroup)
        {
            int index = (int)socialGroup;
            if (index >= 0 && index < reactionMods.Length)
                return reactionMods[index];
            else
                return 0;
        }

        /// <summary>
        /// Enemy alert is raised by hostile enemies or attempting to rest near hostile enemies.
        /// Enemy alert is lowered when killing a hostile enemy or after some time has passed.
        /// </summary>
        public void SetEnemyAlert(bool alert)
        {
            enemyAlertActive = alert;
            if (alert)
                lastEnemyAlertTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
        }

        public override void FixedUpdate()
        {
            // Handle events that are called by classic's update loop
            if (GameManager.ClassicUpdate && playerMotor)
            {
                // Tally running skill. Running tallies so quickly in classic that it might be a bug or oversight.
                // Here we use a rate of 1/4 that observed for classic.
                if (playerMotor.IsRunning && !playerMotor.IsRiding)
                {
                    if (runningTallyCounter == 3)
                    {
                        TallySkill(DFCareer.Skills.Running, 1);
                        runningTallyCounter = 0;
                    }
                    else
                        runningTallyCounter++;
                }

                // Handle breath when underwater and not water breathing
                if (GameManager.Instance.PlayerEnterExit.IsPlayerSubmerged && !GameManager.Instance.PlayerEntity.IsWaterBreathing)
                {
                    if (currentBreath == 0)
                    {
                        currentBreath = GameManager.Instance.GuildManager.DeepBreath(MaxBreath);
                    }
                    if (breathUpdateTally > 18)
                    {
                        --currentBreath;
                        if (Race == Races.Argonian && (UnityEngine.Random.Range(0, 2) == 1))
                            ++currentBreath;
                        breathUpdateTally = 0;
                    }
                    else
                        ++breathUpdateTally;

                    if (currentBreath <= 0)
                        SetHealth(0);
                }
                else
                    currentBreath = 0;
            }
        }

        public override void Update(DaggerfallEntityBehaviour sender)
        {
            if (SaveLoadManager.Instance.LoadInProgress)
                return;

            if (CurrentHealth <= 0)
                return;

            if (guardsArriveCountdown > 0)
            {
                guardsArriveCountdown -= Time.deltaTime;
                if (guardsArriveCountdown <= 0 && guardsArriveCountdownLocation == GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject)
                    SpawnCityGuards(true);
            }

            if (playerMotor == null)
                playerMotor = GameManager.Instance.PlayerMotor;
            if (climbingMotor == null)
                climbingMotor = GameManager.Instance.ClimbingMotor;

            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (gameMinutes < lastGameMinutes)
            {
                throw new Exception(string.Format("lastGameMinutes {0} greater than gameMinutes: {1}", lastGameMinutes, gameMinutes));
            }

            // Wait until game has started and the game time has been set.
            // If the game time is taken before then "30" is returned, which causes an initial player fatigue loss
            // after loading or starting a game with a non-30 minute.
            if (!gameStarted && !GameManager.Instance.StateManager.GameInProgress)
                return;
            else if (!gameStarted)
                gameStarted = true;

            // Lower active enemy alert if more than 8 hours have passed since alert was raised
            const int alertDecayMinutes = 8 * DaggerfallDateTime.MinutesPerHour;
            if (enemyAlertActive && (gameMinutes - lastEnemyAlertTime) > alertDecayMinutes)
                SetEnemyAlert(false);

            if (playerMotor != null)
            {
                // Values are < 1 so fatigue loss is slower
                const float atleticismMultiplier = 0.9f;
                const float improvedAtleticismMultiplier = 0.8f;

                // UESP describes Athleticism relating to fatigue/stamina as "decreases slower when running, jumping, climbing, and swimming."
                // https://en.uesp.net/wiki/Daggerfall:ClassMaker#Special_Advantages
                // In this implementation, players with athleticism will lose fatigue 10% slower, otherwise at normal rate
                // If player also has improved athleticism enchantment, they will lose fatigue 20% slower
                // TODO: Determine actual improvement multiplier to fatigue loss, possibly move to FormulaHelper
                float fatigueLossMultiplier = 1.0f;
                if (career.Athleticism)
                    fatigueLossMultiplier = (ImprovedAthleticism) ? improvedAtleticismMultiplier : atleticismMultiplier;

                // Apply per-minute events
                if (lastGameMinutes != gameMinutes)
                {
                    // Apply fatigue loss to the player
                    int amount = (int)(DefaultFatigueLoss * fatigueLossMultiplier);
                    if (climbingMotor != null && climbingMotor.IsClimbing)
                        amount = (int)(ClimbingFatigueLoss * fatigueLossMultiplier);
                    else if (playerMotor.IsRunning && !playerMotor.IsStandingStill)
                        amount = (int)(RunningFatigueLoss * fatigueLossMultiplier);
                    else if (GameManager.Instance.PlayerEnterExit.IsPlayerSwimming)
                    {
                        if (Race != Races.Argonian && Dice100.FailedRoll(Skills.GetLiveSkillValue(DFCareer.Skills.Swimming)))
                            amount = (int)(SwimmingFatigueLoss * fatigueLossMultiplier);
                        TallySkill(DFCareer.Skills.Swimming, 1);
                    }

                    if (!isResting)
                        DecreaseFatigue(amount);

                    // Make magically-created items that have expired disappear
                    items.RemoveExpiredItems();
                }

                // Reduce fatigue when jumping and tally jumping skill
                if (!CheckedCurrentJump && playerMotor.IsJumping)
                {
                    DecreaseFatigue((int)(JumpingFatigueLoss * fatigueLossMultiplier));
                    TallySkill(DFCareer.Skills.Jumping, 1);
                    CheckedCurrentJump = true;
                }

                // Reset jump fatigue check when grounded
                if (CheckedCurrentJump && !playerMotor.IsJumping)
                {
                    CheckedCurrentJump = false;
                }
            }

            // Adjust regional prices and update climate weathers and diseases whenever the date has changed.
            uint lastDay = lastGameMinutes / 1440;
            uint currentDay = gameMinutes / 1440;
            int daysPast = (int)(currentDay - lastDay);

            if (daysPast > 0)
            {
                FormulaHelper.UpdateRegionalPrices(ref regionData, daysPast);
                GameManager.Instance.WeatherManager.SetClimateWeathers();
                GameManager.Instance.WeatherManager.UpdateWeatherFromClimateArray = true;
                RemoveExpiredRentedRooms();
                LoanChecker.CheckOverdueLoans(lastGameMinutes);
            }

            // Normalize legal reputation and update faction power and regional conditions every certain number of days
            uint minutesPassed = gameMinutes - lastGameMinutes;
            for (int i = 0; i < minutesPassed; ++i)
            {
                // Normalize legal reputations towards 0
                if (((i + lastGameMinutes) % 161280) == 0 && !preventNormalizingReputations) // 112 days
                    NormalizeReputations();

                // Update faction powers
                if (((i + lastGameMinutes) % 10080) == 0) // 7 days
                    RegionPowerAndConditionsUpdate(false);

                // Update regional conditions
                // In classic this version of the function is called within the conditional for the above, when both
                // (i + lastGameMinutes) % 10080) == 0 and (i + lastGameMinutes) % 54720) == 0) are true, or every 266 days.
                // I'm pretty sure it was supposed to be every 38 days.
                if (((i + lastGameMinutes) % 54720) == 0) // 38 days
                {
                    RegionPowerAndConditionsUpdate(true);
                    StartRacialOverrideQuest(false);
                }

                if (((i + lastGameMinutes) % 120960) == 0) // 84 days
                    StartRacialOverrideQuest(true);
            }

            // Enemy spawns are prevented after time is raised for fast travel, jail time, and vampire transformation
            // Classic also prevents enemy spawns during loitering,
            // but this seems counterintuitive so it's not implemented in DF Unity for now
            if (!preventEnemySpawns)
            {
                bool updatedGuards = false;

                for (uint l = 0; l < (gameMinutes - lastGameMinutes); ++l)
                {
                    // Catch up time and break if something spawns. Don't spawn encounters while player is swimming in water or on ship (same as classic).
                    if (!GameManager.Instance.PlayerEnterExit.IsPlayerSwimming && 
                        !GameManager.Instance.TransportManager.IsOnShip() && 
                        IntermittentEnemySpawn(l + lastGameMinutes + 1))
                        break;

                    // Confirm regionData is available
                    if (regionData == null || regionData.Length == 0)
                        break;

                    // Handle guards appearing for low-legal rep player
                    int regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
                    if (regionData[regionIndex].LegalRep < -10 && Dice100.SuccessRoll(5))
                    {
                        crimeCommitted = Crimes.Criminal_Conspiracy;
                        SpawnCityGuards(false);
                    }

                    // Handle guards appearing for banished player
                    if ((regionData[regionIndex].SeverePunishmentFlags & 1) != 0 && Dice100.SuccessRoll(10))
                    {
                        crimeCommitted = Crimes.Criminal_Conspiracy;
                        SpawnCityGuards(false);
                    }

                    // If enemy guards have been spawned, any new NPC guards should be made into enemyMobiles
                    if (!updatedGuards)
                        MakeNPCGuardsIntoEnemiesIfGuardsSpawned();

                    updatedGuards = true;
                }
            }

            lastGameMinutes = gameMinutes;

            // Allow enemy spawns again if they have been disabled
            if (preventEnemySpawns)
                preventEnemySpawns = false;

            // Allow normalizing reputations again if it was disabled
            if (preventNormalizingReputations)
                preventNormalizingReputations = false;

            HandleStartingCrimeGuildQuests();

            // Reset surrender to guards dialogue if no guards are nearby
            if (haveShownSurrenderToGuardsDialogue && GameManager.Instance.HowManyEnemiesOfType(MobileTypes.Knight_CityWatch, true) == 0)
            {
                haveShownSurrenderToGuardsDialogue = false;
            }
        }

        void StartRacialOverrideQuest(bool isCureQuest)
        {
            RacialOverrideEffect racialEffect = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialEffect != null)
                racialEffect.StartQuest(isCureQuest);
        }

        public bool IntermittentEnemySpawn(uint Minutes)
        {
            // Define minimum distance from player based on spawn locations
            const int minDungeonDistance = 8;
            const int minLocationDistance = 10;
            const int minWildernessDistance = 10;

            //TODO: if (InOutsideWater)
            //          return;

            // Do not allow spawns if not enough time has passed or spawns are suppressed for any reason
            // Note - should not need the preventEnemySpawns check here as IntermittentEnemySpawn() is only called by Update() !preventEnemySpawns
            bool timeForSpawn = ((Minutes / 12) % 12) == 0;
            if (!timeForSpawn || preventEnemySpawns)
                return false;

            // Spawns when player is outside
            if (!GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                uint timeOfDay = Minutes % 1440; // 1440 minutes in a day
                if (GameManager.Instance.PlayerGPS.IsPlayerInLocationRect)
                {
                    if (timeOfDay < 360 || timeOfDay > 1080)
                    {
                        // In a location area at night
                        if (FormulaHelper.RollRandomSpawn_LocationNight() == 0)
                        {
                            GameObjectHelper.CreateFoeSpawner(true, RandomEncounters.ChooseRandomEnemy(false), 1, minLocationDistance);
                            return true;
                        }
                    }
                }
                else
                {
                    if (timeOfDay >= 360 && timeOfDay <= 1080)
                    {
                        // Wilderness during day
                        if (FormulaHelper.RollRandomSpawn_WildernessDay() != 0)
                            return false;
                    }
                    else
                    {
                        // Wilderness at night
                        if (FormulaHelper.RollRandomSpawn_WildernessNight() != 0)
                            return false;
                    }

                    GameObjectHelper.CreateFoeSpawner(true, RandomEncounters.ChooseRandomEnemy(false), 1, minWildernessDistance);
                    return true;
                }
            }

            // Spawns when player is inside
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                // Spawns when player is inside a dungeon
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                {
                    if (isResting)
                    {
                        if (FormulaHelper.RollRandomSpawn_Dungeon() == 0)
                        {
                            // TODO: Not sure how enemy type is chosen here.
                            GameObjectHelper.CreateFoeSpawner(false, RandomEncounters.ChooseRandomEnemy(false), 1, minDungeonDistance);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // Recreation of guard spawning based on classic
        public void SpawnCityGuards(bool immediateSpawn)
        {
            const int maxActiveGuardSpawns = 5;

            // Only spawn if player is not in a dungeon, and if there are fewer than max active guards
            if (!GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon && GameManager.Instance.HowManyEnemiesOfType(MobileTypes.Knight_CityWatch, false, true) <= maxActiveGuardSpawns)
            {
                // Handle indoor guard spawning
                var enterExit = GameManager.Instance.PlayerEnterExit;
                if (enterExit.IsPlayerInside && (enterExit.IsPlayerInsideOpenShop || enterExit.IsPlayerInsideTavern || enterExit.IsPlayerInsideResidence))
                {
                    if (enterExit.Interior.FindLowestOuterInteriorDoor(out Vector3 lowestDoorPos, out Vector3 lowestDoorNormal))
                    {
                        lowestDoorPos += lowestDoorNormal * (GameManager.Instance.PlayerController.radius + 0.1f);
                        int guardCount = UnityEngine.Random.Range(2, 6);
                        for (int i = 0; i < guardCount; i++)
                        {
                            SpawnCityGuard(lowestDoorPos, Vector3.forward);
                        }
                    }
                    return;
                }

                DaggerfallLocation dfLocation = GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject;
                if (dfLocation == null)
                    return;

                PopulationManager populationManager = dfLocation.GetComponent<PopulationManager>();
                if (populationManager == null)
                    return;

                // If immediateSpawn, then guards will be created without any countdown
                if (immediateSpawn)
                {
                    int guardsSpawnedFromNPCs = 0;
                    // Try to spawn guards from nearby NPCs. This has the benefits that the spawn position will be valid,
                    // and that guards will tend to appear from the more crowded areas and not from nothing.
                    // Note: Classic disables the NPC that the guard is spawned from. Classic guards do not spawn looking at the player.
                    for (int i = 0; i < populationManager.PopulationPool.Count; i++)
                    {
                        if (!populationManager.PopulationPool[i].npc.isActiveAndEnabled)
                            continue;

                        Vector3 directionToMobile = populationManager.PopulationPool[i].npc.Motor.transform.position - GameManager.Instance.PlayerMotor.transform.position;
                        if (directionToMobile.magnitude <= 77.5f)
                        {
                            // Spawn from guard mobile NPCs first
                            if (populationManager.PopulationPool[i].npc.IsGuard)
                            {
                                SpawnCityGuard(populationManager.PopulationPool[i].npc.transform.position, populationManager.PopulationPool[i].npc.transform.forward);
                                populationManager.PopulationPool[i].npc.gameObject.SetActive(false);
                                ++guardsSpawnedFromNPCs;
                            }
                            // Next try non-guards
                            else if (Vector3.Angle(directionToMobile, GameManager.Instance.PlayerMotor.transform.forward) >= 105.469
                                && UnityEngine.Random.Range(0, 4) == 0)
                            {
                                SpawnCityGuard(populationManager.PopulationPool[i].npc.transform.position, populationManager.PopulationPool[i].npc.transform.forward);
                                ++guardsSpawnedFromNPCs;
                            }
                        }
                    }

                    // If no guards spawned from nearby NPCs, spawn randomly with a foeSpawner
                    if (guardsSpawnedFromNPCs == 0)
                    {
                        GameObjectHelper.CreateFoeSpawner(true, MobileTypes.Knight_CityWatch, UnityEngine.Random.Range(2, 5 + 1), 12.8f, 51.2f);
                    }
                }
                else
                // Spawn guards if player seen by an NPC
                {
                    bool seen = false;
                    bool seenByGuard = false;
                    for (int i = 0; i < populationManager.PopulationPool.Count; i++)
                    {
                        if (!populationManager.PopulationPool[i].npc.isActiveAndEnabled)
                            continue;

                        Vector3 toPlayer = GameManager.Instance.PlayerMotor.transform.position - populationManager.PopulationPool[i].npc.Motor.transform.position;
                        if (toPlayer.magnitude <= 77.5f && Vector3.Angle(toPlayer, populationManager.PopulationPool[i].npc.Motor.transform.forward) <= 95)
                        {
                            // Check if line of sight to target
                            RaycastHit hit;

                            // Set origin of ray to approximate eye position
                            Vector3 eyePos = populationManager.PopulationPool[i].npc.Motor.transform.position;
                            eyePos.y += .7f;

                            // Set destination to the player's approximate eye position
                            CharacterController controller = GameManager.Instance.PlayerEntityBehaviour.transform.GetComponent<CharacterController>();
                            Vector3 playerEyePos = GameManager.Instance.PlayerMotor.transform.position;
                            playerEyePos.y += controller.height / 3;

                            // Check if npc sees player
                            Vector3 eyeToTarget = playerEyePos - eyePos;
                            Ray ray = new Ray(eyePos, eyeToTarget.normalized);
                            if (Physics.Raycast(ray, out hit, 77.5f))
                            {
                                // Check if hit was player
                                DaggerfallEntityBehaviour entity = hit.transform.gameObject.GetComponent<DaggerfallEntityBehaviour>();
                                if (entity == GameManager.Instance.PlayerEntityBehaviour)
                                    seen = true;
                                if (populationManager.PopulationPool[i].npc.IsGuard)
                                    seenByGuard = true;
                            }
                        }

                        if (seenByGuard)
                        {
                            SpawnCityGuard(populationManager.PopulationPool[i].npc.transform.position, populationManager.PopulationPool[i].npc.transform.forward);
                            populationManager.PopulationPool[i].npc.gameObject.SetActive(false);
                        }
                    }

                    // Player seen by a non-guard NPC but not by any guard NPCs. Start a countdown until guards arrive.
                    if (!seenByGuard && seen)
                    {
                        guardsArriveCountdown = UnityEngine.Random.Range(5, 10 + 1);
                        // Also track location so guards don't appear if player leaves during countdown
                        guardsArriveCountdownLocation = dfLocation;
                    }
                }
            }
        }

        public GameObject SpawnCityGuard(Vector3 position, Vector3 direction)
        {
            GameObject[] cityWatch = GameObjectHelper.CreateFoeGameObjects(position, MobileTypes.Knight_CityWatch, 1);
            cityWatch[0].transform.forward = direction;
            EnemyMotor enemyMotor = cityWatch[0].GetComponent<EnemyMotor>();

            // Classic does not do anything special to make guards aware of the player, but since they're responding to a crime, standing around
            // unaware of the player can be perceived as a bug.
            enemyMotor.MakeEnemyHostileToAttacker(GameManager.Instance.PlayerEntityBehaviour);

            // Set a longer giveUpTimer than usual in case it takes more than the usual timer to get to the player position
            enemyMotor.GiveUpTimer *= 3;
            cityWatch[0].SetActive(true);

            return cityWatch[0];
        }

        void MakeNPCGuardsIntoEnemiesIfGuardsSpawned()
        {
            if (GameManager.Instance.HowManyEnemiesOfType(MobileTypes.Knight_CityWatch, true) > 0)
            {
                DaggerfallLocation dfLocation = GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject;
                if (dfLocation == null)
                    return;

                PopulationManager populationManager = dfLocation.GetComponent<PopulationManager>();
                if (populationManager == null)
                    return;

                for (int i = 0; i < populationManager.PopulationPool.Count; i++)
                {
                    if (!populationManager.PopulationPool[i].npc.isActiveAndEnabled)
                        continue;

                    // Spawn from guard mobile NPCs
                    if (populationManager.PopulationPool[i].npc.IsGuard)
                    {
                        SpawnCityGuard(populationManager.PopulationPool[i].npc.transform.position, populationManager.PopulationPool[i].npc.transform.forward);
                        populationManager.PopulationPool[i].npc.gameObject.SetActive(false);
                    }
                }
            }
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
            lightSource = null;
            spellbook.Clear();
            factionData.Reset();
            globalVars.Reset();
            notebook.Clear();
            SetEntityDefaults();
            startingLevelUpSkillSum = 0;
            currentLevelUpSkillSum = 0;
            goldPieces = 0;
            timeOfLastSkillIncreaseCheck = 0;
            timeOfLastSkillTraining = 0;
            rentedRooms.Clear();
            crimeCommitted = Crimes.None;
            DaedraSummonDay = DaedraSummonIndex = 0;
            if (skillUses != null)
                System.Array.Clear(skillUses, 0, skillUses.Length);

            // Clear any world variation this player entity has triggered
            WorldDataVariants.Clear();
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
            this.sGroupReputations[2] = character.reputationScholars;
            this.sGroupReputations[3] = character.reputationNobility;
            this.sGroupReputations[4] = character.reputationUnderworld;
            this.currentFatigue = character.currentFatigue;
            this.skillUses = character.skillUses;
            this.skillsRecentlyRaised[0] = character.skillsRaisedThisLevel1;
            this.skillsRecentlyRaised[1] = character.skillsRaisedThisLevel2;
            this.startingLevelUpSkillSum = character.startingLevelUpSkillSum;
            this.minMetalToHit = (WeaponMaterialTypes)character.minMetalToHit;
            this.armorValues = character.armorValues;
            this.timeToBecomeVampireOrWerebeast = character.timeToBecomeVampireOrWerebeast;
            this.lastTimePlayerAteOrDrankAtTavern = character.lastTimePlayerAteOrDrankAtTavern;
            this.timeOfLastSkillTraining = character.lastTimePlayerBoughtTraining;
            this.timeForThievesGuildLetter = character.timeForThievesGuildLetter;
            this.timeForDarkBrotherhoodLetter = character.timeForDarkBrotherhoodLetter;
            this.darkBrotherhoodRequirementTally = character.darkBrotherhoodRequirementTally;
            this.thievesGuildRequirementTally = character.thievesGuildRequirementTally;
            //this.hasStartedInitialVampireQuest = character.hasStartedInitialVampireQuest != 0;
            //this.vampireClan = character.vampireClan;
            //this.lastTimeVampireNeedToKillSatiated = character.lastTimeVampireNeedToKillSatiated;

            // Trim name strings as these might contain trailing whitespace characters from classic save
            name = name.Trim();
            career.Name = career.Name.Trim();

            BackStory = character.backStory;

            if (maxHealth <= 0)
                this.maxHealth = FormulaHelper.RollMaxHealth(this);
            else
                this.maxHealth = maxHealth;

            if (fillVitals)
                FillVitalSigns();

            timeOfLastSkillIncreaseCheck = DaggerfallUnity.Instance.WorldTime.Now.ToClassicDaggerfallTime();

            DaggerfallUnity.LogMessage("Assigned character " + this.name, true);
        }

        /// <summary>
        /// Assigns character items and spells from classic save tree.
        /// Spells are stored as a child of spellbook item container, which is why they are imported along with items.
        /// </summary>
        public void AssignItemsAndSpells(SaveTree saveTree)
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

                // Some (most likely hacked) classic items have 0 or 65535 in image data bitfield
                // Discard these items as they will likely have other bad attributes such as an impossible weight
                // The goal here is just to prevent game from crashing due to bad item data
                if ((record as ItemRecord).ParsedData.image1 == 0 || (record as ItemRecord).ParsedData.image1 == 0xffff)
                    continue;

                // Create item
                DaggerfallUnityItem newItem = new DaggerfallUnityItem((ItemRecord)record);

                // Import spells if this is a spellbook record
                try
                {
                    if (newItem.ItemGroup == ItemGroups.MiscItems && newItem.GroupIndex == 0)
                        ImportSpells(containerRecord);
                }
                catch (Exception ex)
                {
                    // Failed to import spellbook - log and give player empty spellbook
                    Debug.LogWarningFormat("Failed to import spellbook record. Exception {0}", ex.Message);
                    GameManager.Instance.ItemHelper.AddSpellbookItem(GameManager.Instance.PlayerEntity);
                }

                // Grabbed trapped soul if needed
                if (newItem.ItemGroup == ItemGroups.MiscItems && newItem.GroupIndex == 1)
                {
                    if (record.Children.Count > 0)
                    {
                        TrappedSoulRecord soulRecord = (TrappedSoulRecord)record.Children[0];
                        newItem.TrappedSoulType = (MobileTypes)soulRecord.RecordRoot.SpriteIndex;
                    }
                    else
                        newItem.TrappedSoulType = MobileTypes.None;
                }

                // Add existence time limit if item is flagged as having been made through the "Create Item" effect
                if (((record as ItemRecord).ParsedData.flags & 0x1000) != 0)
                {
                    newItem.TimeForItemToDisappear = record.RecordRoot.Time;
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
        /// Assigns spells from spellbook item.
        /// </summary>
        void ImportSpells(ContainerRecord record)
        {
            // Must have a populated spellbook container with spells
            if (record == null || record.Children.Count == 0 ||
                record.Children[0].Children == null || record.Children[0].Children.Count == 0)
            {
                return;
            }

            // Read spell records in spellbook container
            foreach(SpellRecord spell in record.Children[0].Children)
            {
                spell.ReadNativeSpellData();
                EffectBundleSettings bundle;
                if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spell.ParsedData, BundleTypes.Spell, out bundle))
                {
                    Debug.LogErrorFormat("Failed to create effect bundle while importing classic spell '{0}'.", spell.ParsedData.spellName);
                    continue;
                }
                AddSpell(bundle);
            }
        }

        /// <summary>
        /// Assigns guild memberships to player from classic save tree.
        /// </summary>
        public void AssignGuildMemberships(SaveTree saveTree, bool vampire = false)
        {
            // Find character record, should always be a singleton
            CharacterRecord characterRecord = (CharacterRecord)saveTree.FindRecord(RecordTypes.Character);
            if (characterRecord == null)
                return;

            // Find all guild memberships, and add Daggerfall Unity guild memberships
            List<SaveTreeBaseRecord> guildMembershipRecords = saveTree.FindRecords(RecordTypes.GuildMembership, characterRecord);
            List<SaveTreeBaseRecord> oldMembershipRecords = saveTree.FindRecords(RecordTypes.OldGuild, characterRecord);

            if (vampire)
            {
                GameManager.Instance.GuildManager.ImportMembershipData(guildMembershipRecords, true);
                GameManager.Instance.GuildManager.ImportMembershipData(oldMembershipRecords);
            }
            else
            {
                GameManager.Instance.GuildManager.ImportMembershipData(guildMembershipRecords);
                GameManager.Instance.GuildManager.ImportMembershipData(oldMembershipRecords, true);
            }
        }

        /// <summary>
        /// Assigns diseases and poisons to player from classic save tree.
        /// </summary>
        public void AssignDiseasesAndPoisons(SaveTree saveTree, out LycanthropyTypes lycanthropyType)
        {
            lycanthropyType = LycanthropyTypes.None;

            // Find character record, should always be a singleton
            CharacterRecord characterRecord = (CharacterRecord)saveTree.FindRecord(RecordTypes.Character);
            if (characterRecord == null)
                return;

            // Find all diseases and poisons
            List<SaveTreeBaseRecord> diseaseAndPoisonRecords = saveTree.FindRecords(RecordTypes.DiseaseOrPoison, characterRecord);

            // Add Daggerfall Unity diseases and poisons
            foreach (var record in diseaseAndPoisonRecords)
            {
                byte diseaseID = (record as DiseaseOrPoisonRecord).ParsedData.ID;
                if (diseaseID < 100) // is a disease
                {
                    // TODO: Import classic disease effect and poisons to player effect manager and set properties
                    //DaggerfallDisease_Deprecated newDisease = new DaggerfallDisease_Deprecated((DiseaseOrPoisonRecord)record);
                    //DaggerfallDisease_Deprecated has been removed. Conversion from classic data record, which gets all necessary data, was as follows:
                    //public DaggerfallDisease_Deprecated(DiseaseOrPoisonRecord record)
                    //{
                    //    diseaseType = (Diseases)record.ParsedData.ID;
                    //    if (record.ParsedData.incubationOver == 1)
                    //        incubationOver = true;
                    //    daysOfSymptomsLeft = (byte)record.ParsedData.daysOfSymptomsLeft;
                    //}
                }
                else
                {
                    switch (diseaseID)
                    {
                        case 101:
                            lycanthropyType = LycanthropyTypes.Werewolf;
                            break;
                        case 102:
                            lycanthropyType = LycanthropyTypes.Wereboar;
                            break;
                    }
                }
            }
        }

        public void AssignPlayerVampireSpells(VampireClans clan)
        {
            // IDs from SPELLS.STD record ID https://en.uesp.net/wiki/Daggerfall:SPELLS.STD_indices
            int levitateID = 4;
            int charmMortalID = 90;
            int calmHumanoidID = 91;
            int nimblenessID = 85;
            int paralysisID = 50;
            int healID = 64;
            int shieldID = 17;
            int resistColdID = 11;
            int resistFireID = 12;
            int resistShockID = 13;
            int silenceID = 23;
            int invisibilityID = 6;
            int iceStormID = 20;
            int wildfireID = 33;
            int recallID = 94;

            // Common spells
            AssignVampireSpell(levitateID);
            AssignVampireSpell(charmMortalID);
            AssignVampireSpell(calmHumanoidID);

            // Clan spells
            switch (clan)
            {
                case VampireClans.Vraseth:
                    AssignVampireSpell(nimblenessID);
                    break;
                case VampireClans.Khulari:
                    AssignVampireSpell(paralysisID);
                    break;
                case VampireClans.Montalion:
                    AssignVampireSpell(recallID);
                    break;
                case VampireClans.Thrafey:
                    AssignVampireSpell(healID);
                    break;
                case VampireClans.Garlythi:
                    AssignVampireSpell(shieldID);
                    break;
                case VampireClans.Selenu:
                    AssignVampireSpell(resistColdID);
                    AssignVampireSpell(resistFireID);
                    AssignVampireSpell(resistShockID);
                    break;
                case VampireClans.Lyrezi:
                    AssignVampireSpell(silenceID);
                    AssignVampireSpell(invisibilityID);
                    break;
                case VampireClans.Haarvenu:
                    AssignVampireSpell(iceStormID);
                    AssignVampireSpell(wildfireID);
                    break;
            }
        }

        void AssignVampireSpell(int id)
        {
            // Get spell record data
            SpellRecord.SpellRecordData recordData;
            if (!GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(id, out recordData))
                return;

            // Get effect bundle settings
            EffectBundleSettings bundleSettings;
            if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(recordData, BundleTypes.Spell, out bundleSettings))
                return;

            // Assign to player entity spellbook with some custom settings
            bundleSettings.MinimumCastingCost = true;
            bundleSettings.Tag = vampireSpellTag;
            GameManager.Instance.PlayerEntity.AddSpell(bundleSettings);
        }

        public void AssignPlayerLycanthropySpell()
        {
            const int lycanthropyID = 92;

            // Get spell record data
            SpellRecord.SpellRecordData recordData;
            if (!GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(lycanthropyID, out recordData))
                return;

            // Remove ! from start of spell name
            if (recordData.spellName.StartsWith("!"))
                recordData.spellName = recordData.spellName.Substring(1);

            // Get effect bundle settings
            EffectBundleSettings bundleSettings;
            if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(recordData, BundleTypes.Spell, out bundleSettings))
                return;

            // Assign to player entity spellbook with some custom settings
            bundleSettings.MinimumCastingCost = true;
            bundleSettings.NoCastingAnims = true;
            bundleSettings.Tag = lycanthropySpellTag;
            GameManager.Instance.PlayerEntity.AddSpell(bundleSettings);
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
                maxHealth = FormulaHelper.RollMaxHealth(this);
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
        public override int SetHealth(int amount, bool restoreMode = false)
        {
            if (godMode)
                return currentHealth = MaxHealth;

            currentHealth = (restoreMode) ? amount : Mathf.Clamp(amount, 0, MaxHealth);
            if (currentHealth <= 0)
            {
                // Players can have avoid death benefit from guild memberships, leaves them on 10% hp
                if (GameManager.Instance.GuildManager.AvoidDeath())
                    return currentHealth = (int)(MaxHealth * 0.1f);
                else
                    RaiseOnDeathEvent();
            }
            return currentHealth;
        }

        /// <summary>
        /// Sets new fatigue value.
        /// Override for godmode support.
        /// </summary>
        public override int SetFatigue(int amount, bool restoreMode = false)
        {
            if (godMode)
                return currentFatigue = MaxFatigue;
            else
                return base.SetFatigue(amount, restoreMode);
        }

        /// <summary>
        /// Sets new magicka value.
        /// Override for godmode support.
        /// </summary>
        public override int SetMagicka(int amount, bool restoreMode = false)
        {
            if (godMode)
                return currentMagicka = MaxMagicka;
            else
                return base.SetMagicka(amount, restoreMode);
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
            catch (Exception ex)
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
                if (timeForThievesGuildLetter == 0 && thievesGuildRequirementTally != InviteSent)
                {
                    // Tally is set to 100 when the Thieves Guild quest line starts
                    thievesGuildRequirementTally += amount;
                    if (thievesGuildRequirementTally >= 10)
                    {
                        uint currentMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                        timeForThievesGuildLetter = currentMinutes + 4320; // 3 days
                    }
                }
            }
            else // murder
            {
                if (timeForDarkBrotherhoodLetter == 0 && darkBrotherhoodRequirementTally != InviteSent)
                {
                    // Tally is set to 100 when the Dark Brotherhood quest line starts
                    darkBrotherhoodRequirementTally += amount;
                    if (darkBrotherhoodRequirementTally >= 15)
                    {
                        uint currentMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                        timeForDarkBrotherhoodLetter = currentMinutes + 4320; // 3 days
                    }
                }
            }
        }

        /// <summary>
        /// Clear player-only constant effects.
        /// </summary>
        public override void ClearConstantEffects()
        {
            base.ClearConstantEffects();
            IsAzurasStarEquipped = false;
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
            const int youAreNowAMasterOfTextID = 4020;

            if (GameManager.Instance.PlayerDeath.DeathInProgress)
                return;

            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
            if ((now.ToClassicDaggerfallTime() - timeOfLastSkillIncreaseCheck) <= 360)
                return;

            timeOfLastSkillIncreaseCheck = now.ToClassicDaggerfallTime();

            for (short i = 0; i < skillUses.Length; i++)
            {
                int skillAdvancementMultiplier = DaggerfallSkills.GetAdvancementMultiplier((DFCareer.Skills)i);
                float careerAdvancementMultiplier = Career.AdvancementMultiplier;
                int usesNeededForAdvancement = FormulaHelper.CalculateSkillUsesForAdvancement(skills.GetPermanentSkillValue(i), skillAdvancementMultiplier, careerAdvancementMultiplier, level);
                int reflexesMod = 0x10000 - (((int)reflexes - 2) << 13);
                int calculatedSkillUses = (skillUses[i] * reflexesMod) >> 16;

                if (calculatedSkillUses >= usesNeededForAdvancement)
                {
                    skillUses[i] = 0;

                    if (skills.GetPermanentSkillValue(i) < 100 && (skills.GetPermanentSkillValue(i) < 95 || !AlreadyMasteredASkill()))
                    {
                        skills.SetPermanentSkillValue(i, (short)(skills.GetPermanentSkillValue(i) + 1));
                        SetSkillRecentlyIncreased(i);
                        SetCurrentLevelUpSkillSum();
                        DaggerfallUI.Instance.PopupMessage(TextManager.Instance.GetLocalizedText("skillImprove").Replace("%s", DaggerfallUnity.Instance.TextProvider.GetSkillName((DFCareer.Skills)i)));
                        if (skills.GetPermanentSkillValue(i) == 100)
                        {
                            List<DFCareer.Skills> primarySkills = GetPrimarySkills();
                            if (primarySkills.Contains((DFCareer.Skills)i))
                            {
                                ITextProvider textProvider = DaggerfallUnity.Instance.TextProvider;
                                TextFile.Token[] tokens;
                                tokens = textProvider.GetRSCTokens(youAreNowAMasterOfTextID);
                                if (tokens != null && tokens.Length > 0)
                                {
                                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
                                    messageBox.SetTextTokens(tokens);
                                    messageBox.ClickAnywhereToClose = true;
                                    messageBox.ParentPanel.BackgroundColor = Color.clear;
                                    messageBox.Show();
                                }
                                DaggerfallUI.Instance.PlayOneShot(SoundClips.ArenaFanfareLevelUp);
                            }
                        }
                    }
                }
            }

            if (CheckForLevelUp())
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenCharacterSheetWindow);
        }

        /// <summary>
        /// Gets whether the player has already become master of a skill.
        /// </summary>
        public bool AlreadyMasteredASkill()
        {
            bool mastered = false;
            List<DFCareer.Skills> primarySkills = GetPrimarySkills();
            foreach (DFCareer.Skills skill in primarySkills)
            {
                if (skills.GetPermanentSkillValue(skill) == 100)
                {
                    mastered = true;
                    break;
                }
            }

            return mastered;
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
            if (thievesGuildRequirementTally != InviteSent
                && timeForThievesGuildLetter > 0
                && timeForThievesGuildLetter < DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                && !GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>().IsPlayerInside)
            {
                thievesGuildRequirementTally = InviteSent;
                timeForThievesGuildLetter = 0;
                Questing.QuestMachine.Instance.StartQuest(ThievesGuild.InitiationQuestName, ThievesGuild.FactionId);
            }
            if (darkBrotherhoodRequirementTally != InviteSent
                && timeForDarkBrotherhoodLetter > 0
                && timeForDarkBrotherhoodLetter < DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime()
                && !GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>().IsPlayerInside)
            {
                darkBrotherhoodRequirementTally = InviteSent;
                timeForDarkBrotherhoodLetter = 0;
                Questing.QuestMachine.Instance.StartQuest(DarkBrotherhood.InitiationQuestName, DarkBrotherhood.FactionId);
            }
        }

        /// <summary>
        /// Releases a quest item carried by player so it can be assigned back again by quest script.
        /// This ensures item is properly unequipped and optionally makes permanent.
        /// </summary>
        /// <param name="questUID">Quest UID owning Item.</param>
        /// <param name="item">Quest Item to release.</param>
        /// <param name="makePermanent">True to make item permanent.</param>
        public void ReleaseQuestItemForReoffer(ulong questUID, Item item, bool makePermanent = false)
        {
            if (item == null || item.Symbol == null)
                return;

            // Always set prototype item permanent when requested
            // This handles cases where prototype is given directly to player
            if (makePermanent)
                item.DaggerfallUnityItem.MakePermanent();

            // Get all player held quest items matching this quest and item symbol
            // This can include items cloned from prototype to a Foe resource then picked up by player
            DaggerfallUnityItem[] items = GameManager.Instance.PlayerEntity.Items.ExportQuestItems(questUID, item.Symbol);
            if (items == null || items.Length == 0)
                return;

            // Process all matching items
            foreach (DaggerfallUnityItem dfitem in items)
            {
                // Unequip item if player is wearing it
                if (GameManager.Instance.PlayerEntity.ItemEquipTable.UnequipItem(dfitem))
                {
                    // If item was actually unequipped then update armour values
                    GameManager.Instance.PlayerEntity.UpdateEquippedArmorValues(dfitem, false);
                }

                // Remove quest from inventory so it can be offered back to player
                GameManager.Instance.PlayerEntity.Items.RemoveItem(dfitem);

                // Optionally make permanent
                if (makePermanent)
                    dfitem.MakePermanent();
            }
        }

        public void ClearReactionMods()
        {
            Array.Clear(reactionMods, 0, socialGroupCount);
        }

        #endregion

        #region RegionData
        public struct RegionDataRecord
        {
            public byte[] Values; // Unused?
            public bool[] Flags; // Region condition flags
            public bool[] Flags2; // Flags for groupings of the region condition flags. If a condition is set its group flag will also be set.
            public short LegalRep;
            public byte PrecipitationOverride; // Never set by classic. In classic if this is non-0 the screen precipitation effect for classicWeatherTypes[PrecipitationOverride - 1] is forced.
            public byte SeverePunishmentFlags; // 1 = player was banished from region, 2 = player was sent to dungeon for execution by region
            public ushort IDOfPersecutedTemple;
            public ushort PriceAdjustment;
        }

        public enum RegionDataFlags
        {
            WarBeginning = 0,
            WarOngoing = 1,
            WarWon = 2,
            WarLost = 3,
            PlagueBeginning = 4,
            PlagueOngoing = 5,
            PlagueEnding = 6,
            FamineBeginning = 7,
            FamineOngoing = 8,
            FamineEnding = 9,
            WitchBurnings = 10,
            CrimeWave = 11,
            NewRuler = 12,
            BadHarvest = 13,
            TGMInJail = 14,
            TGMSetFree = 15,
            TGMExecuted = 16,
            NewTGM = 17,
            PersecutedTemple = 18,
            PricesHigh = 19,
            PricesLow = 20,
            HappyHoliday = 21,
            ScaryHoliday = 22,
            HolyHoliday = 23,
            MadWizardNearby = 24,
            MadWizardDies = 25,
            Condition26 = 26, // Unused
            Condition27 = 27, // Unused
            Condition28 = 28, // Unused
            Condition29 = 29, // Unused
        }

        readonly byte[] flagsToFlags2Map = { 0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 6, 7, 8, 9, 3, 3, 3, 3, 10, 4, 4, 11, 12, 13, 5, 5, 0, 0, 0, 0 };

        /// <summary>
        /// Update regional power and conditions. Called every certain number of game days.
        /// </summary>
        public void RegionPowerAndConditionsUpdate(bool updateConditions)
        {
            // Note: For some reason rumor updating is disabled in classic while the player is serving jail time. There's no clear reason for this,
            // so not replicating that here.
            GameManager.Instance.TalkManager.RefreshRumorMill();

            List<int> keys = new List<int>(factionData.FactionDict.Keys);
            foreach (int key in keys)
            {
                if (isFactionValidForRumorMill(factionData.FactionDict[key]))
                {
                    // Get power mod from allies
                    int[] allies = { factionData.FactionDict[key].ally1, factionData.FactionDict[key].ally2, factionData.FactionDict[key].ally3 };
                    int alliesPower = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        FactionFile.FactionData ally;
                        if (FactionData.GetFactionData(allies[i], out ally))
                            alliesPower += ally.power;
                    }
                    int alliesPowerMod = alliesPower / 10;

                    // Get power mod from enemies
                    int[] enemies = { factionData.FactionDict[key].enemy1, factionData.FactionDict[key].enemy2, factionData.FactionDict[key].enemy3 };
                    int enemiesPower = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        FactionFile.FactionData enemy;
                        if (FactionData.GetFactionData(enemies[i], out enemy))
                            enemiesPower += enemy.power;
                    }
                    int enemiesPowerMod = enemiesPower / 10;

                    // Get power mod from parent
                    FactionFile.FactionData parent;
                    int parentPowerMod = 0;
                    if (factionData.FactionDict[key].parent != 0)
                    {
                        FactionData.GetFactionData(factionData.FactionDict[key].parent, out parent);
                        parentPowerMod = parent.power / 10;
                    }

                    // Raise or lower power based on power of parent, allies, random power bonus and enemies
                    if (Dice100.FailedRoll(parentPowerMod + alliesPowerMod + factionData.FactionDict[key].rulerPowerBonus - enemiesPowerMod))
                        factionData.ChangePower(factionData.FactionDict[key].id, -1);
                    else
                        factionData.ChangePower(factionData.FactionDict[key].id, 1);

                    // Raise power if children more powerful
                    if (factionData.FactionDict[key].children != null)
                    {
                        foreach (int childID in factionData.FactionDict[key].children)
                        {
                            FactionFile.FactionData child;
                            if (FactionData.GetFactionData(childID, out child) && child.power > factionData.FactionDict[key].power)
                            {
                                factionData.ChangePower(factionData.FactionDict[key].id, 1);
                                break;
                            }
                        }
                    }

                    // Update conditions
                    if (updateConditions)
                    {
                        // Chance to end faction alliances
                        int factionPowerMod = factionData.FactionDict[key].power / 5;
                        for (int i = 0; i < 3; ++i)
                        {
                            if (allies[i] != 0)
                            {
                                int powerSum = factionPowerMod + factionData.FactionDict[key].rulerPowerBonus;
                                if (Dice100.FailedRoll((powerSum + factionData.GetNumberOfCommonAlliesAndEnemies(factionData.FactionDict[key].id, allies[i]) * 3) / 5 + 70))
                                {
                                    factionData.EndFactionAllies(factionData.FactionDict[key].id, allies[i]);
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, allies[i], -1, 100, 1402); // End faction allies
                                }
                            }
                        }

                        // Refresh allies array as it may have changed
                        allies[0] = factionData.FactionDict[key].ally1;
                        allies[1] = factionData.FactionDict[key].ally2;
                        allies[2] = factionData.FactionDict[key].ally3;

                        // Chance to end faction rivalries
                        for (int i = 0; i < 3; ++i)
                        {
                            FactionFile.FactionData enemy;
                            if (FactionData.GetFactionData(enemies[i], out enemy) && !factionData.IsEnemyStatePermanentUntilWarOver(factionData.FactionDict[key], enemy))
                            {
                                int powerSum = factionPowerMod + factionData.FactionDict[key].rulerPowerBonus;
                                if (Dice100.SuccessRoll((powerSum + factionData.GetNumberOfCommonAlliesAndEnemies(factionData.FactionDict[key].id, enemies[i]) * 3) / 5))
                                {
                                    factionData.EndFactionEnemies(factionData.FactionDict[key].id, enemies[i]);
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, enemies[i], -1, 100, 1403); // End faction enemies
                                }
                            }
                        }

                        // Refresh enemies array as it may have changed
                        enemies[0] = factionData.FactionDict[key].enemy1;
                        enemies[1] = factionData.FactionDict[key].enemy2;
                        enemies[2] = factionData.FactionDict[key].enemy3;

                        // Chance to start new alliances
                        for (int i = 0; i < 3; ++i)
                        {
                            if (allies[i] == 0)
                            {
                                FactionFile.FactionData random;
                                List<int> keys2 = new List<int>(factionData.FactionDict.Keys);
                                int count = keys2.Count;

                                do
                                {
                                    int randomKeyIndex = UnityEngine.Random.Range(0, count);
                                    FactionData.GetFactionData(factionData.FactionDict[keys2[randomKeyIndex]].id, out random);
                                    keys2.Remove(keys[randomKeyIndex]);
                                    count--;
                                }
                                while (!isFactionValidForRumorMill(random) && count > 0);

                                if (!factionData.IsFaction2AnAllyOfFaction1(factionData.FactionDict[key].id, random.id) &&
                                    !factionData.IsFaction2AnEnemyOfFaction1(factionData.FactionDict[key].id, random.id) &&
                                    !factionData.IsFaction2AnEnemyOfFaction1(factionData.FactionDict[key].ally1, random.id) &&
                                    !factionData.IsFaction2AnEnemyOfFaction1(factionData.FactionDict[key].ally2, random.id) &&
                                    !factionData.IsFaction2AnEnemyOfFaction1(factionData.FactionDict[key].ally3, random.id) &&
                                    !factionData.IsFaction2AnAllyOfFaction1(factionData.FactionDict[key].enemy1, random.id) &&
                                    !factionData.IsFaction2AnAllyOfFaction1(factionData.FactionDict[key].enemy2, random.id) &&
                                    !factionData.IsFaction2AnAllyOfFaction1(factionData.FactionDict[key].enemy3, random.id) &&
                                    factionData.GetFaction2RelationToFaction1(factionData.FactionDict[key].id, random.id) == -1)
                                {
                                    int powerSum = factionPowerMod + factionData.FactionDict[key].rulerPowerBonus;
                                    if (Dice100.SuccessRoll((powerSum + factionData.GetNumberOfCommonAlliesAndEnemies(factionData.FactionDict[key].id, random.id) * 3) / 5))
                                    {
                                        if (factionData.FactionDict[key].type == (int)FactionFile.FactionTypes.Province && factionData.FactionDict[key].region != -1)
                                            GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, random.id, factionData.FactionDict[key].region, 26, 1481); // Factions start alliance sign message
                                        if (random.type == (int)FactionFile.FactionTypes.Province && random.region != -1)
                                            GameManager.Instance.TalkManager.AddNonQuestRumor(random.id, factionData.FactionDict[key].id, random.region, 26, 1481); // Factions start alliance sign message
                                        factionData.StartFactionAllies(factionData.FactionDict[key].id, i, random.id);
                                        GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, random.id, -1, 100, 1400); // Factions start alliance
                                    }
                                }
                                break;
                            }
                        }

                        // Refresh allies array as it may have changed
                        allies[0] = factionData.FactionDict[key].ally1;
                        allies[1] = factionData.FactionDict[key].ally2;
                        allies[2] = factionData.FactionDict[key].ally3;

                        int warEnemyID = 0;
                        if (factionData.IsFaction2APotentialWarEnemyOfFaction1(factionData.FactionDict[key].id, factionData.FactionDict[key].enemy1))
                            warEnemyID = factionData.FactionDict[key].enemy1;
                        else if (factionData.IsFaction2APotentialWarEnemyOfFaction1(factionData.FactionDict[key].id, factionData.FactionDict[key].enemy2))
                            warEnemyID = factionData.FactionDict[key].enemy2;
                        else if (factionData.IsFaction2APotentialWarEnemyOfFaction1(factionData.FactionDict[key].id, factionData.FactionDict[key].enemy3))
                            warEnemyID = factionData.FactionDict[key].enemy3;

                        if (warEnemyID != 0)
                        {
                            FactionFile.FactionData warEnemy;
                            if (FactionData.GetFactionData(warEnemyID, out warEnemy))
                            {
                                if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.WarWon] || regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.WarLost])
                                {
                                    ResetWarDataForRegion(factionData.FactionDict[key].id);
                                    ResetWarDataForRegion(warEnemyID);

                                    factionData.EndFactionEnemies(factionData.FactionDict[key].id, warEnemyID);
                                }
                                else if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.WarBeginning])
                                {
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, warEnemyID, factionData.FactionDict[key].region, 0, 1479); // War started sign message
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(warEnemyID, factionData.FactionDict[key].id, warEnemy.region, 0, 1479); // War started sign message
                                    TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.WarOngoing);
                                    TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.WarOngoing);
                                }
                                else if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.WarOngoing])
                                {
                                    if (Dice100.FailedRoll(5))
                                    {
                                        int combinedPower = factionData.FactionDict[key].power + alliesPower / 5;

                                        // Get power of enemy's allies
                                        int[] warEnemyallies = { warEnemy.ally1, warEnemy.ally2, warEnemy.ally3 };
                                        int warEnemyAlliesPower = 0;
                                        for (int i = 0; i < 3; i++)
                                        {
                                            FactionFile.FactionData ally;
                                            if (FactionData.GetFactionData(warEnemyallies[i], out ally))
                                                warEnemyAlliesPower += ally.power;
                                        }

                                        int combinedEnemyPower = warEnemy.power + warEnemyAlliesPower / 5;

                                        int powerLoss = UnityEngine.Random.Range(1, combinedEnemyPower / 10 + 1);
                                        int enemyPowerLoss = UnityEngine.Random.Range(1, combinedPower / 10 + 1);

                                        factionData.ChangePower(factionData.FactionDict[key].id, powerLoss);
                                        factionData.ChangePower(warEnemy.id, enemyPowerLoss);

                                        combinedPower = -powerLoss;
                                        combinedEnemyPower -= enemyPowerLoss;

                                        if (combinedPower - combinedEnemyPower > combinedEnemyPower)
                                        {
                                            GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, warEnemy.id, -1, 100, 1408); // War over
                                            factionData.ChangePower(factionData.FactionDict[key].id, warEnemy.power / 2);
                                            TurnOnConditionFlag(warEnemy.region, RegionDataFlags.WarLost);
                                            TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.WarWon);
                                        }
                                        else if (combinedEnemyPower - combinedPower > combinedPower)
                                        {
                                            GameManager.Instance.TalkManager.AddNonQuestRumor(warEnemy.id, factionData.FactionDict[key].id, -1, 100, 1408); // War over
                                            factionData.ChangePower(warEnemy.id, factionData.FactionDict[key].power / 2);
                                            TurnOnConditionFlag(warEnemy.region, RegionDataFlags.WarWon);
                                            TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.WarLost);
                                        }
                                        else
                                            GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, warEnemy.id, -1, 100, 1407); // War started/ongoing
                                    }
                                    else
                                    {
                                        TurnOffConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.WarOngoing);
                                        TurnOffConditionFlag(warEnemy.region, RegionDataFlags.WarOngoing);
                                    }
                                }
                            }
                        }

                        // Refresh enemies array as it may have changed
                        enemies[0] = factionData.FactionDict[key].enemy1;
                        enemies[1] = factionData.FactionDict[key].enemy2;
                        enemies[2] = factionData.FactionDict[key].enemy3;

                        // Chance to start new rivalry and, if it's a region, a war
                        for (int i = 0; i < 3; ++i)
                        {
                            if (enemies[i] == 0)
                            {
                                FactionFile.FactionData random;
                                List<int> keys2 = new List<int>(factionData.FactionDict.Keys);
                                int count = keys2.Count;

                                do
                                {
                                    int randomKeyIndex = UnityEngine.Random.Range(0, count);
                                    FactionData.GetFactionData(factionData.FactionDict[keys2[randomKeyIndex]].id, out random);
                                    keys2.Remove(keys[randomKeyIndex]);
                                    count--;
                                }
                                while (!isFactionValidForRumorMill(random) && count > 0);

                                if (!factionData.IsFaction2AnAllyOfFaction1(factionData.FactionDict[key].id, random.id) &&
                                    !factionData.IsFaction2AnEnemyOfFaction1(factionData.FactionDict[key].id, random.id) &&
                                    !factionData.IsFaction2AnEnemyOfFaction1(factionData.FactionDict[key].ally1, random.id) &&
                                    !factionData.IsFaction2AnEnemyOfFaction1(factionData.FactionDict[key].ally2, random.id) &&
                                    !factionData.IsFaction2AnEnemyOfFaction1(factionData.FactionDict[key].ally3, random.id) &&
                                    !factionData.IsFaction2AnAllyOfFaction1(factionData.FactionDict[key].enemy1, random.id) &&
                                    !factionData.IsFaction2AnAllyOfFaction1(factionData.FactionDict[key].enemy2, random.id) &&
                                    !factionData.IsFaction2AnAllyOfFaction1(factionData.FactionDict[key].enemy3, random.id))
                                {
                                    int relation = factionData.GetFaction2RelationToFaction1(factionData.FactionDict[key].id, random.id);
                                    if (relation == -1 || relation == 2)
                                    {
                                        int mod = 0;
                                        if (relation == 2)
                                            mod = 10;
                                        int powerSum = factionPowerMod + factionData.FactionDict[key].rulerPowerBonus;
                                        if (Dice100.FailedRoll(mod + (powerSum + factionData.GetNumberOfCommonAlliesAndEnemies(factionData.FactionDict[key].id, random.id) * 3) / 5 + 70))
                                        {
                                            if (factionData.FactionDict[key].region != -1 && factionData.FactionDict[key].type == (int)FactionFile.FactionTypes.Province)
                                                GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, random.id, factionData.FactionDict[key].region, 27, 1482); // War started sign message
                                            if (random.region != -1 && random.type == (int)FactionFile.FactionTypes.Province)
                                                GameManager.Instance.TalkManager.AddNonQuestRumor(random.id, factionData.FactionDict[key].id, random.region, 27, 1482); // Enemy faction sign message
                                            factionData.StartFactionEnemies(factionData.FactionDict[key].id, i, random.id);
                                            GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, random.id, -1, 100, 1401); // Faction rivalry started
                                            if (factionData.FactionDict[key].region != -1 && factionData.FactionDict[key].type == (int)FactionFile.FactionTypes.Province
                                                && random.region != -1 && random.type == (int)FactionFile.FactionTypes.Province
                                                && factionData.IsEnemyStatePermanentUntilWarOver(factionData.FactionDict[key], random))
                                            {
                                                GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, random.id, -1, 100, 1407); // War started/ongoing
                                                GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, random.id, factionData.FactionDict[key].region, 28, 1479); // War started sign message
                                                GameManager.Instance.TalkManager.AddNonQuestRumor(random.id, factionData.FactionDict[key].id, random.region, 28, 1479); // War started sign message
                                                TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.WarBeginning);
                                                TurnOnConditionFlag(random.region, RegionDataFlags.WarBeginning);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }

                        // Chance for new ruler
                        if (!factionData.GetFlag(key, FactionFile.Flags.RulerImmune))
                        {
                            int mod = factionData.FactionDict[key].rulerPowerBonus / 3;
                            if (Dice100.FailedRoll(mod + 70))
                            {
                                if (factionData.FactionDict[key].region != -1 && factionData.FactionDict[key].type == (int)FactionFile.FactionTypes.Province)
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, -1, 12, 1480); // New ruler. Although unused, a regionID is defined for this rumor in classic.
                                factionData.SetNewRulerData(factionData.FactionDict[key].id);
                                // if ( PlayerIsRelatedToFaction(factionData.FactionDict[key]) )
                                // AddNewRumor 1406 // New faction leader
                            }
                        }

                        // Handle other conditions
                        if (factionData.FactionDict[key].region != -1 && factionData.FactionDict[key].type == (int)FactionFile.FactionTypes.Province)
                        {
                            // Get power mod from allies
                            alliesPower = 0;
                            for (int i = 0; i < 3; i++)
                            {
                                FactionFile.FactionData ally;
                                if (FactionData.GetFactionData(allies[i], out ally))
                                    alliesPower += ally.power;
                            }
                            alliesPowerMod = alliesPower / 10;

                            // Famine
                            if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.FamineEnding])
                                TurnOffConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.FamineEnding);
                            else if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.FamineOngoing])
                            {
                                if (Dice100.SuccessRoll(factionData.FactionDict[key].rulerPowerBonus / 5 + alliesPowerMod + factionData.FactionDict[key].power / 5))
                                {
                                    TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.FamineEnding);
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, factionData.FactionDict[key].region, 7, 1477); // Famine sign message
                                }
                            }
                            else if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.FamineBeginning])
                            {
                                TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.FamineOngoing);
                                GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, factionData.FactionDict[key].region, 7, 1477); // Famine sign message
                            }
                            else if (Dice100.SuccessRoll(2) && Dice100.FailedRoll(factionData.FactionDict[key].rulerPowerBonus + alliesPowerMod))
                            {
                                TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.FamineBeginning);
                                GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, factionData.FactionDict[key].region, 7, 1477); // Famine sign message
                            }

                            // Plague
                            FactionFile.FactionData temple;
                            FactionData.GetFactionData(MapsFile.RegionTemples[factionData.FactionDict[key].region], out temple);

                            if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.PlagueEnding])
                                TurnOffConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.PlagueEnding);
                            else if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.PlagueOngoing])
                            {
                                if (temple.id != 0)
                                    factionData.ChangePower(temple.id, -1);
                                factionData.ChangePower(factionData.FactionDict[key].id, -1);
                                if (Dice100.SuccessRoll(factionData.FactionDict[key].power / 5 + factionData.FactionDict[key].rulerPowerBonus / 5 + alliesPowerMod))
                                {
                                    TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.PlagueEnding);
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, factionData.FactionDict[key].region, 4, 1478); // Plague sign message
                                }
                            }
                            else if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.PlagueBeginning])
                            {
                                if (temple.id != 0)
                                    factionData.ChangePower(temple.id, -1);
                                factionData.ChangePower(factionData.FactionDict[key].id, -1);
                                GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, factionData.FactionDict[key].region, 4, 1478); // Plague sign message
                                TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.PlagueOngoing);
                            }
                            else if (Dice100.SuccessRoll(2) && Dice100.FailedRoll(factionData.FactionDict[key].rulerPowerBonus + alliesPowerMod))
                            {
                                if (temple.id != 0)
                                    factionData.ChangePower(temple.id, -1);
                                factionData.ChangePower(factionData.FactionDict[key].id, -1);
                                GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, factionData.FactionDict[key].region, 4, 1478); // Plague sign message
                                TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.PlagueBeginning);
                            }

                            // Persecuted temple
                            if (MapsFile.RegionTemples[factionData.FactionDict[key].region] != 0)
                            {
                                if (Dice100.FailedRoll((temple.power - factionData.FactionDict[key].power + 5) / 5))
                                    TurnOffConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.PersecutedTemple);
                                else if (temple.power >= 2 * factionData.FactionDict[key].power)
                                    TurnOffConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.PersecutedTemple);
                                else
                                {
                                    regionData[factionData.FactionDict[key].region].IDOfPersecutedTemple = (ushort)temple.id;
                                    TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.PersecutedTemple);
                                    factionData.ChangePower(temple.id, -1);
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, factionData.FactionDict[key].region, 18, 1476); // Persecuted temple sign message
                                }
                            }

                            // Crime wave
                            if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.CrimeWave])
                                factionData.ChangePower(factionData.FactionDict[key].id, -1);

                            FactionFile.FactionData thievesGuild;
                            FactionData.GetFactionData((int)FactionFile.FactionIDs.The_Thieves_Guild, out thievesGuild);

                            FactionFile.FactionData darkBrotherhood;
                            FactionData.GetFactionData((int)FactionFile.FactionIDs.The_Dark_Brotherhood, out darkBrotherhood);

                            if (Dice100.FailedRoll(((thievesGuild.power + darkBrotherhood.power) / 2 - factionData.FactionDict[key].power + 5) / 5))
                                TurnOffConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.CrimeWave);
                            else
                            {
                                TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.CrimeWave);
                                factionData.ChangePower(factionData.FactionDict[key].id, -1);
                                GameManager.Instance.TalkManager.AddNonQuestRumor(0, 0, factionData.FactionDict[key].region, 11, 1410); // Crime wave
                            }

                            // Witch burnings
                            FactionFile.FactionData witches;
                            FactionData.FindFactionByTypeAndRegion(8, factionData.FactionDict[key].region, out witches);
                            if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.WitchBurnings])
                                factionData.ChangePower(witches.id, -1);
                            if (witches.id != 0)
                            {
                                if (Dice100.FailedRoll((witches.power - factionData.FactionDict[key].power + 5) / 5))
                                    TurnOffConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.WitchBurnings);
                                else
                                {
                                    TurnOnConditionFlag(factionData.FactionDict[key].region, RegionDataFlags.WitchBurnings);
                                    factionData.ChangePower(witches.id, -1);
                                    GameManager.Instance.TalkManager.AddNonQuestRumor(factionData.FactionDict[key].id, 0, factionData.FactionDict[key].region, 10, 1475); // Witch burnings sign message
                                }
                            }
                        }

                        int numberOfCrimeWaves = 0;
                        int numberOfPricesHigh = 0;
                        int numberOfPricesLow = 0;
                        int numberOfFamines = 0;
                        int numberOfWars = 0;

                        if (factionData.FactionDict[key].region != -1 && factionData.FactionDict[key].type == (int)FactionFile.FactionTypes.Province)
                        {
                            if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.CrimeWave])
                                ++numberOfCrimeWaves;
                            if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.PricesHigh])
                                ++numberOfPricesHigh;
                            if (regionData[factionData.FactionDict[key].region].Flags[(int)RegionDataFlags.PricesLow])
                                ++numberOfPricesLow;
                            if (regionData[factionData.FactionDict[key].region].Flags2[2])
                                ++numberOfFamines;
                            if (regionData[factionData.FactionDict[key].region].Flags2[0])
                                ++numberOfWars;
                        }

                        FactionFile.FactionData thievesGuild2;
                        FactionData.GetFactionData((int)FactionFile.FactionIDs.The_Thieves_Guild, out thievesGuild2);
                        factionData.ChangePower(thievesGuild2.id, numberOfCrimeWaves);

                        FactionFile.FactionData darkBrotherhood2;
                        FactionData.GetFactionData((int)FactionFile.FactionIDs.The_Dark_Brotherhood, out darkBrotherhood2);
                        factionData.ChangePower(darkBrotherhood2.id, numberOfCrimeWaves);

                        FactionFile.FactionData merchants;
                        FactionData.GetFactionData((int)FactionFile.FactionIDs.The_Merchants, out merchants);
                        if (numberOfPricesHigh >= 3)
                            factionData.ChangePower(merchants.id, 1);

                        if (numberOfPricesLow >= 3)
                            factionData.ChangePower(merchants.id, -1);

                        if (numberOfFamines >= 3)
                            factionData.ChangePower(merchants.id, -1);

                        if (numberOfWars >= 3)
                            factionData.ChangePower(merchants.id, 1);
                    }
                }
            }

            if (updateConditions)
            {
                // These rumors are always available
                GameManager.Instance.TalkManager.AddNonQuestRumor(0, 0, -1, 100, 1450);
                GameManager.Instance.TalkManager.AddNonQuestRumor(0, 0, -1, 100, 1451);
                GameManager.Instance.TalkManager.AddNonQuestRumor(0, 0, -1, 100, 1452);
                GameManager.Instance.TalkManager.AddNonQuestRumor(0, 0, -1, 100, 1453);
                GameManager.Instance.TalkManager.AddNonQuestRumor(0, 0, -1, 100, 1454);
                GameManager.Instance.TalkManager.AddNonQuestRumor(0, 0, -1, 100, 1455);
                GameManager.Instance.TalkManager.AddNonQuestRumor(0, 0, -1, 100, 1456);
            }
        }

        private static bool isFactionValidForRumorMill(FactionFile.FactionData factionData)
        {
            // Classic does not do any check on factions included in rumor mill.
            // Here we exclude all generic factions and those which should clearly not be
            // included like Oblivion or The Septim Empire.
            return ((factionData.type == (int) FactionFile.FactionTypes.Province ||
                     factionData.type == (int) FactionFile.FactionTypes.Group ||
                     factionData.type == (int) FactionFile.FactionTypes.Subgroup) &&
                    factionData.id != (int) FactionFile.FactionIDs.Oblivion &&
                    factionData.id != (int) FactionFile.FactionIDs.Children &&
                    factionData.id != (int) FactionFile.FactionIDs.Generic_Knightly_Order &&
                    factionData.id != (int) FactionFile.FactionIDs.Smiths &&
                    factionData.id != (int) FactionFile.FactionIDs.Questers &&
                    factionData.id != (int) FactionFile.FactionIDs.Healers &&
                    factionData.id != (int) FactionFile.FactionIDs.Seneschal &&
                    factionData.id != (int) FactionFile.FactionIDs.Temple_Missionaries &&
                    factionData.id != (int) FactionFile.FactionIDs.Temple_Treasurers &&
                    factionData.id != (int) FactionFile.FactionIDs.Temple_Healers &&
                    factionData.id != (int) FactionFile.FactionIDs.Temple_Blessers &&
                    factionData.id != (int) FactionFile.FactionIDs.Random_Ruler &&
                    factionData.id != (int) FactionFile.FactionIDs.The_Septim_Empire);
        }

        public void TurnOnConditionFlag(int regionID, RegionDataFlags flagID)
        {
            byte[] valuesMin = { 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x01, 0x0A, 0x0A, 0x01, 0x01, 0x01, 0x01, 0x0A, 0x0A, 0x0A, 0x01, 0x01, 0x01, 0x05, 0x01 };
            byte[] valuesMax = { 0x0A, 0x0A, 0x0A, 0x0A, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x0A, 0x0A, 0x64, 0x14, 0x14, 0x01, 0x01, 0x01, 0x64, 0x14, 0x14, 0x01, 0x01, 0x01, 0x1E, 0x01 };

            // Turn off other flags in same group
            if (regionData[regionID].Flags2[flagsToFlags2Map[(int)flagID]])
            {
                for (int i = 0; i < 29; ++i)
                {
                    if (flagsToFlags2Map[(int)flagID] == flagsToFlags2Map[i])
                        regionData[regionID].Flags[i] = false;
                }
            }

            byte value = (byte)UnityEngine.Random.Range(valuesMin[(int)flagID], valuesMax[(int)flagID] + 1);
            regionData[regionID].Values[(int)flagID] = value;
            regionData[regionID].Flags[(int)flagID] = true;
            regionData[regionID].Flags2[flagsToFlags2Map[(int)flagID]] = true;
        }

        public void TurnOffConditionFlag(int regionID, RegionDataFlags flagID)
        {
            regionData[regionID].Flags[(int)flagID] = false;
            regionData[regionID].Flags2[flagsToFlags2Map[(int)flagID]] = false;
        }

        private void ResetWarDataForRegion(int factionID)
        {
            FactionFile.FactionData faction;
            if (FactionData.GetFactionData(factionID, out faction))
            {
                int regionID = faction.region;

                if (regionID != -1 && faction.type == (int)FactionFile.FactionTypes.Province)
                {
                    regionData[regionID].Flags[(int)RegionDataFlags.WarBeginning] = false;
                    regionData[regionID].Flags[(int)RegionDataFlags.WarOngoing] = false;
                    regionData[regionID].Flags[(int)RegionDataFlags.WarWon] = false;
                    regionData[regionID].Flags[(int)RegionDataFlags.WarLost] = false;
                    regionData[regionID].Flags2[flagsToFlags2Map[(int)RegionDataFlags.WarBeginning]] = false;
                    regionData[regionID].Values[(int)RegionDataFlags.WarOngoing] = 0;
                }
            }
        }

        /// <summary>
        /// Initialize region data arrays.
        /// </summary>
        public void InitializeRegionData()
        {
            FormulaHelper.RandomizeInitialRegionalPrices(ref regionData);
            for (int i = 0; i < regionData.Length; i++)
            {
                regionData[i].Values = new byte[29];

                for (int j = 0; j < 29; j++)
                    regionData[i].Values[j] = 0;

                regionData[i].Flags = new bool[29];
                for (int j = 0; j < 29; j++)
                    regionData[i].Flags[j] = false;

                regionData[i].Flags2 = new bool[14];
                for (int j = 0; j < 14; j++)
                    regionData[i].Flags2[j] = false;

                regionData[i].LegalRep = 0;
                regionData[i].PrecipitationOverride = 0;
                regionData[i].SeverePunishmentFlags = 0;
                regionData[i].IDOfPersecutedTemple = 0;
            }

            for (int i = 0; i < 12; ++i)
            {
                RegionPowerAndConditionsUpdate(false);
                RegionPowerAndConditionsUpdate(true);
            }
        }

        /// <summary>
        /// Normalize reputations towards 0. Called every certain number of game days.
        /// </summary>
        public void NormalizeReputations()
        {
            ClampLegalReputations();

            for (int i = 0; i < 62; ++i)
            {
                if (regionData[i].LegalRep < 0)
                    ++regionData[i].LegalRep;
                else if (regionData[i].LegalRep > 0)
                    --regionData[i].LegalRep;
            }

            List<int> keys = new List<int>(factionData.FactionDict.Keys);
            foreach (int key in keys)
            {
                if (factionData.FactionDict[key].rep < 0)
                    factionData.ChangeReputation(factionData.FactionDict[key].id, 1);
                else if (factionData.FactionDict[key].rep > 0)
                    factionData.ChangeReputation(factionData.FactionDict[key].id, -1);
            }
        }

        public void ClampLegalReputations()
        {
            const short legalRepMin = -100;
            const short legalRepMax = 100;

            for (int i = 0; i < 62; ++i)
            {
                if (regionData[i].LegalRep < legalRepMin)
                    regionData[i].LegalRep = legalRepMin;
                else if (regionData[i].LegalRep > legalRepMax)
                    regionData[i].LegalRep = legalRepMax;
            }
        }

        #endregion

        #region Crime

        public enum Crimes
        {
            None = 0,
            Attempted_Breaking_And_Entering = 1,
            Trespassing = 2,
            Breaking_And_Entering = 3,
            Assault = 4,
            Murder = 5,
            Tax_Evasion = 6,
            Criminal_Conspiracy = 7,
            Vagrancy = 8,
            Smuggling = 9,
            Piracy = 10,
            High_Treason = 11,
            Pickpocketing = 12,
            Theft = 13,
            Treason = 14,
            LoanDefault = 15,
        }

        // Values after index 0 are from FALL.EXE. It does not seem to have a valid value for the last crime "Treason," so just using half of "High Treason" value here.
        readonly short[] reputationLossPerCrime = { 0x00, 0x0A, 0x05, 0x0A, 0x08, 0x14, 0x0A, 0x02, 0x01, 0x02, 0x02, 0x4B, 0x02, 0x08, 0x24, 0x0A };

        public void LowerRepForCrime()
        {
            LowerRepForCrime(GameManager.Instance.PlayerGPS.CurrentRegionIndex, crimeCommitted);
        }

        public void LowerRepForCrime(int regionIndex, Crimes crime)
        {
            regionData[regionIndex].LegalRep -= reputationLossPerCrime[(int)crime];

            FactionFile.FactionData peopleFaction;
            FactionData.FindFactionByTypeAndRegion(15, regionIndex, out peopleFaction);

            FactionData.ChangeReputation(peopleFaction.id, -(reputationLossPerCrime[(int)crime] / 2), true);
        }

        public void RaiseReputationForDoingSentence()
        {
            int regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            regionData[regionIndex].LegalRep += (short)(halfOfLegalRepPlayerLostFromCrime - 1);

            FactionFile.FactionData peopleFaction;
            FactionData.FindFactionByTypeAndRegion(15, regionIndex, out peopleFaction);

            // Classic changes reputation here by (1 - halfOfLegalRepPlayerLostFromCrime) / 2). Probably a bug.
            FactionData.ChangeReputation(peopleFaction.id, (halfOfLegalRepPlayerLostFromCrime - 1) / 2, true);
        }

        public bool SurrenderToCityGuards(bool voluntarySurrender)
        {
            int regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            short legalRep = regionData[regionIndex].LegalRep;

            if (CurrentHealth <= 0)
                return false;

            SetHealth(1);
            if (legalRep < -20 && !voluntarySurrender)
                return false;
            else if (legalRep < -20 || legalRep > 0)
                CourtWindow();
            else if ((DFRandom.rand() & 1) != 0 && !voluntarySurrender)
                return false;
            else
                CourtWindow();

            return true;
        }

        public void CourtWindow()
        {
            // Transition outside if player inside or they are hurled into void
            // Doing this before showing court window so exterior transition takes place first
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
                GameManager.Instance.PlayerEnterExit.TransitionExterior(true);

            arrested = true;
            halfOfLegalRepPlayerLostFromCrime = (short)(reputationLossPerCrime[(int)crimeCommitted] / 2);
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenCourtWindow);
        }

        void SetCrimeCommitted(Crimes crime)
        {
            // Racial override can suppress crimes, e.g. transformed lycanthrope
            RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            bool suppressCrime = racialOverride != null && racialOverride.SuppressCrime;

            crimeCommitted = (!suppressCrime) ? crime : Crimes.None;
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

            // Do nothing if already displaying exhausted popup
            // This prevents rapid-fire exhaustion events from stacking multiple windows
            // Example is Somnalius poison after first exhaustion drop
            // Subsequent exhaustion events would otherwise stack on popup per poison round remaining
            // This can make player think game has stalled until they click through all stacked popups
            if (displayingExhaustedPopup)
                return;

            // Do nothing if player already dead - prevents popup repeat if OnExhausted event is queued multiple times just prior to death
            if (CurrentHealth == 0)
                return;

            bool enemiesNearby = GameManager.Instance.AreEnemiesNearby();

            ITextProvider textProvider = DaggerfallUnity.Instance.TextProvider;
            TextFile.Token[] tokens;

            GameManager.Instance.PlayerMotor.CancelMovement = true;

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);

            if (GameManager.Instance.PlayerEnterExit.IsPlayerSwimming)
                messageBox.SetText(TextManager.Instance.GetLocalizedText("exhaustedInWater"));
            else
            {
                if (!enemiesNearby)
                    tokens = textProvider.GetRSCTokens(youDropToTheGround1);
                else
                    tokens = textProvider.GetRSCTokens(youDropToTheGround2);

                if (tokens != null && tokens.Length > 0)
                {
                    messageBox.SetTextTokens(tokens);
                }
            }
            messageBox.ClickAnywhereToClose = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;
            messageBox.OnClose += ExhaustedMessageBox_OnClose;
            displayingExhaustedPopup = true;
            messageBox.Show();

            if (!enemiesNearby && !GameManager.Instance.PlayerEnterExit.IsPlayerSwimming)
            {
                // TODO: Duplicates rest code in rest window. Should be unified.
                DaggerfallUnity.Instance.WorldTime.Now.RaiseTime(1 * DaggerfallDateTime.SecondsPerHour);
                int healthRecoveryRate = FormulaHelper.CalculateHealthRecoveryRate(this);
                int fatigueRecoveryRate = FormulaHelper.CalculateFatigueRecoveryRate(MaxFatigue);
                int spellPointRecoveryRate = FormulaHelper.CalculateSpellPointRecoveryRate(this);

                CurrentHealth += healthRecoveryRate;
                CurrentFatigue += fatigueRecoveryRate;
                CurrentMagicka += spellPointRecoveryRate;

                TallySkill(DFCareer.Skills.Medical, 1);
            }
            else
                SetHealth(0);
        }

        private void ExhaustedMessageBox_OnClose()
        {
            displayingExhaustedPopup = false;
        }

        private void PlayerGPS_OnExitLocationRect()
        {
            // Clear crime state when exiting location rect
            CrimeCommitted = Crimes.None;
        }

        private void DaggerfallTravelPopUp_OnPostFastTravel()
        {
            // Clear crime state post fast travel
            CrimeCommitted = Crimes.None;
        }

        #endregion
    }
}
