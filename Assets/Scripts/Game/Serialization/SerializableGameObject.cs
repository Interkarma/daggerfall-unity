// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyldf@gmail.com), Hazelnut, Numidium
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Weather;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.MagicAndEffects;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Serialization
{
    /// <summary>
    /// Enum of stateful game object types that implement <see cref="ISerializableGameObject"/>.
    /// To add a new type of stateful game object:
    ///     - add type name here
    ///     - add a condition to SerializableStateManager.GetStatefulGameObjectType()
    ///     - add serializer methods
    ///     - add to SerializableStateManager.CacheScene() and RestoreCachedScene()
    /// </summary>
    public enum StatefulGameObjectTypes
    {
        LootContainer,
        ActionDoor,
        ActionObject,
        Enemy,
    }

    /// <summary>
    /// Implement this interface with any MonoBehaviour-derived class that can save/load state.
    /// Classes implementing this interface must also register/deregister themselves to SerializableStateManager.
    /// Only registered objects will be serialized/deserialized. If a deserialized object of the specified
    /// LoadID cannot be found then that object will not have any state restored.
    /// </summary>
    public interface ISerializableGameObject
    {
        /// <summary>
        /// ID used to match serialized objects to runtime objects.
        /// Must be unique for its data type and always reference the same object when procedural scene is recreated.
        /// Object will not be serialized if left at default value of 0 or if LoadID collision detected.
        /// Some objects are unique and can set LoadID to 1 or other unique value.
        /// Serialization class may not have enough information by itself to generate LoadID.
        /// e.g. It may be necessary for scene builder to create a unique LoadID during procedural layout.
        /// </summary>
        ulong LoadID { get; }

        /// <summary>
        /// Return true if object should be saved.
        /// Can return false if object is equivalent to a new instance.
        /// e.g. Only save doors which are unlocked or open, no need to save every single door.
        /// </summary>
        bool ShouldSave { get; }

        /// <summary>
        /// Get object state data to serialize.
        /// </summary>
        object GetSaveData();

        /// <summary>
        /// Restore object state from serialized data onto a fresh procedural layout.
        /// It will usually be necessary to adjust runtime state to match saved state.
        /// e.g. Open doors must be set open, dead enemies must be despawned.
        /// </summary>
        void RestoreSaveData(object dataIn);
    }

    #region Root Data

    [fsObject("v1")]
    public class SaveData_v1
    {
        public SaveDataDescription_v1 header;
        public ulong currentUID;
        public DateAndTime_v1 dateAndTime;
        public PlayerData_v1 playerData;
        public DungeonData_v1 dungeonData;
        public EnemyData_v1[] enemyData;
        public LootContainerData_v1[] lootContainers;
        public BankRecordData_v1[] bankAccounts;
        public BankDeedData_v1 bankDeeds;
        public FaceDetails[] escortingFaces;
        public SceneCache_v1 sceneCache;
        public TravelMapSaveData travelMapData;
        public AdvancedClimbingData_v1 advancedClimbingState;
        public ModInfo_v1[] modInfoData;
    }

    #endregion

    #region Header Data

    [fsObject("v1")]
    public class SaveDataDescription_v1
    {
        public string description = "Daggerfall Unity Save Game v1";
    }

    #endregion

    #region World Data

    [fsObject("v1")]
    public class DateAndTime_v1
    {
        public ulong gameTime;
        public long realTime;
    }

    [fsObject("v1")]
    public class SceneCache_v1
    {
        public SceneCacheEntry_v1[] sceneCache;
        public string[] permanentScenes;
    }

    [fsObject("v1")]
    public class SceneCacheEntry_v1
    {
        public string sceneName;
        public LootContainerData_v1[] lootContainers;
        public ActionDoorData_v1[] actionDoors;
    }

    #endregion

    #region Player Data

    [fsObject("v1")]
    public class PlayerData_v1
    {
        public PlayerPositionData_v1 playerPosition;
        public PlayerEntityData_v1 playerEntity;
        public bool weaponDrawn;
        public bool usingLeftHand;
        public TransportModes transportMode;
        public PlayerPositionData_v1 boardShipPosition;  // Holds the player position from before boarding a ship.
        public Dictionary<int, GuildMembership_v1> guildMemberships;
        public Dictionary<int, GuildMembership_v1> vampireMemberships;
        public List<string> oneTimeQuestsAccepted;
    }

    [fsObject("v1")]
    public class PlayerEntityData_v1
    {
        public Genders gender;
        public int faceIndex;
        public RaceTemplate raceTemplate;
        public DFCareer careerTemplate;
        public PlayerReflexes reflexes;
        public string name;
        public int level;
        public DaggerfallStats stats;
        public DaggerfallSkills skills;
        public DaggerfallResistances resistances;
        public int maxHealth;
        public int currentHealth;
        public int currentFatigue;
        public int currentMagicka;
        public int currentBreath;
        public short[] skillUses;
        public uint timeOfLastSkillIncreaseCheck;
        public uint[] skillsRecentlyRaised;
        public int startingLevelUpSkillSum;
        public int currentLevelUpSkillSum;
        public ulong[] equipTable;
        public ItemData_v1[] items;
        public ItemData_v1[] wagonItems;
        public ItemData_v1[] otherItems;
        public int goldPieces;
        public GlobalVar[] globalVars;
        public WeaponMaterialTypes minMetalToHit;
        public int biographyResistDiseaseMod;
        public int biographyResistMagicMod;
        public int biographyAvoidHitMod;
        public int biographyResistPoisonMod;
        public int biographyFatigueMod;
        public int biographyReactionMod;
        public uint timeForThievesGuildLetter;
        public uint timeForDarkBrotherhoodLetter;
        public int thievesGuildRequirementTally;
        public int darkBrotherhoodRequirementTally;
        public uint timeToBecomeVampireOrWerebeast;
        public uint lastTimePlayerAteOrDrankAtTavern;
        public uint timeOfLastSkillTraining;
        public PlayerEntity.RegionDataRecord[] regionData;
        public RoomRental_v1[] rentedRooms;
        public EffectBundleSettings[] spellbook;
        public EntityEffectManager.EffectBundleSaveData_v1[] instancedEffectBundles;
        public PlayerEntity.Crimes crimeCommitted;
        public bool haveShownSurrenderToGuardsDialogue;
        public ulong lightSourceUID;
        public short reputationCommoners;
        public short reputationMerchants;
        public short reputationNobility;
        public short reputationScholars;
        public short reputationUnderworld;
        public short reputationSGroup5;
        public short reputationSupernaturalBeings;
        public short reputationGuildMembers;
        public short reputationSGroup8;
        public short reputationSGroup9;
        public short reputationSGroup10;
        public VampireClans previousVampireClan;
        public int daedraSummonDay;
        public int daedraSummonIndex;
    }

    [fsObject("v1")]
    public class PlayerPositionData_v1
    {
        public Vector3 position;
        public Vector3 worldCompensation;
        public WorldContext worldContext;
        public int floatingOriginVersion;
        public float yaw;
        public float pitch;
        public bool isCrouching;
        public int worldPosX;
        public int worldPosZ;
        public bool insideDungeon;
        public bool insideBuilding;
        public bool insideOpenShop;
        public bool insideTavern;
        public bool insideResidence;
        public string terrainSamplerName;
        public int terrainSamplerVersion;
        public QuestSmallerDungeonsState smallerDungeonsState;
        public StaticDoor[] exteriorDoors;
        public PlayerGPS.DiscoveredBuilding buildingDiscoveryData;
        public WeatherType weather;
    }

    [fsObject("v1")]
    public class GuildMembership_v1
    {
        public int rank;
        public int lastRankChange;
        public int variant;
        public int flags;
    }

    [fsObject("v1")]
    public class RoomRental_v1
    {
        public string name;
        public int mapID;
        public int buildingKey;
        public int allocatedBedIndex;
        public ulong expiryTime;
    }

    [fsObject("v1")]
    public class ItemData_v1
    {
        public ulong uid;
        public string shortName;
        public int nativeMaterialValue;
        public DyeColors dyeColor;
        public float weightInKg;
        public int drawOrder;
        public int value1;
        public int value2;
        public int hits1;
        public int hits2;
        public int hits3;
        public int stackCount;
        public int enchantmentPoints;
        public int message;
        public int[] legacyMagic;
        public CustomEnchantment[] customMagic;
        public int playerTextureArchive;
        public int playerTextureRecord;
        public int worldTextureArchive;
        public int worldTextureRecord;
        public ItemGroups itemGroup;
        public int groupIndex;
        public int currentVariant;
        public bool isQuestItem;
        public ulong questUID;
        public Symbol questItemSymbol;
        public MobileTypes trappedSoulType;
        public string className;
        public Poisons poisonType = Poisons.None;
        public int potionRecipe;
        public ItemRepairData_v1 repairData;
        public uint timeForItemToDisappear;
        public uint timeHealthLeechLastUsed;
    }

    [fsObject("v1")]
    public class ItemRepairData_v1
    {
        public string sceneName;
        public ulong timeStarted;
        public int repairTime;
    }

    #endregion

    #region Dungeon Data

    [fsObject("v1")]
    public class DungeonData_v1
    {
        public ActionDoorData_v1[] actionDoors;
        public ActionObjectData_v1[] actionObjects;
    }

    #endregion

    #region ActionDoor Data

    [fsObject("v1")]
    public class ActionDoorData_v1
    {
        public ulong loadID;
        public int currentLockValue;
        public Quaternion currentRotation;
        public ActionState currentState;
        public float actionPercentage;
        public short lockpickFailedSkillLevel;
    }

    #endregion

    #region Action Data

    [fsObject("v1")]
    public class ActionObjectData_v1
    {
        public ulong loadID;
        public Vector3 currentPosition;
        public Quaternion currentRotation;
        public ActionState currentState;
        public float actionPercentage;
    }

    #endregion

    #region Enemy Data

    [fsObject("v1")]
    public class EnemyData_v1
    {
        public ulong loadID;
        public string gameObjectName;
        public Vector3 currentPosition;
        public Vector3 localPosition;
        public Quaternion currentRotation;
        public WorldContext worldContext;
        public Vector3 worldCompensation;
        public bool isDead;
        public int startingHealth;
        public int currentHealth;
        public int currentFatigue;
        public int currentMagicka;
        public EntityTypes entityType;
        public string careerName;
        public int careerIndex;
        public bool isHostile;
        public bool hasEncounteredPlayer;
        public bool questSpawn;
        public MobileGender mobileGender;
        public ItemData_v1[] items;
        public ulong[] equipTable;
        public QuestResourceBehaviour.QuestResourceSaveData_v1 questResource;
        public EntityEffectManager.EffectBundleSaveData_v1[] instancedEffectBundles;
        public bool alliedToPlayer;
        public int questFoeSpellQueueIndex;
        public int questFoeItemQueueIndex;
        public bool wabbajackActive;
        public int team;
        public bool specialTransformationCompleted;
    }

    #endregion

    #region Loot Data

    [fsObject("v1")]
    public class LootContainerData_v1
    {
        public ulong loadID;
        public WorldContext worldContext;
        public LootContainerTypes containerType;
        public InventoryContainerImages containerImage;
        public Vector3 currentPosition;
        public Vector3 localPosition;
        public Vector3 worldCompensation;
        public float heightScale;
        public int textureArchive;
        public int textureRecord;
        public string lootTableKey;
        public string entityName;
        public int stockedDate;
        public bool playerOwned;
        public bool customDrop;
        public bool isEnemyClass;
        public ItemData_v1[] items;
    }

    #endregion

    #region Faction Data

    [fsObject("v1")]
    public class FactionData_v1
    {
        public Dictionary<int, FactionFile.FactionData> factionDict;
        public Dictionary<string, int> factionNameToIDDict;
    }

    [fsObject("v2", typeof(FactionData_v1))]
    public class FactionData_v2
    {
        public Dictionary<int, FactionFile.FactionData> factionDict;
        public Dictionary<string, int> factionNameToIDDict;

        public FactionData_v2()
        {
        }

        public FactionData_v2(FactionData_v1 v1)
        {
            FactionFile.RelinkChildren(v1.factionDict);
            factionDict = v1.factionDict;
            factionNameToIDDict = v1.factionNameToIDDict;
            Debug.Log("Migrated FactionData_v1 to FactionData_v2 and relinked children.");
        }
    }

    #endregion

    #region Save Info

    [fsObject("v1")]
    public class SaveInfo_v1
    {
        public int saveVersion;
        public string saveName;
        public string characterName;
        public DateAndTime_v1 dateAndTime;
        public string dfuVersion;
    }

    [fsObject("v1")]
    public class ModInfo_v1
    {
        public string fileName;
        public string title;
        public string guid;
        public string version;
        public int loadPriority;
    }

    #endregion

    #region Bank Data

    [fsObject("v1")]
    public class BankDeedData_v1
    {
        public int shipType;
        public HouseData_v1[] houses;
    }

    [fsObject("v1")]
    public class HouseData_v1
    {
        public string location;
        public int mapID;
        public int buildingKey;
        public int regionIndex;
    }

    [fsObject("v1")]
    public class BankRecordData_v1
    {
        public int accountGold;
        public int loanTotal;
        public uint loanDueDate;
        public int regionIndex;
        public bool hasDefaulted;
    }

    [fsObject("v1")]
    public class TravelMapSaveData
    {
        public bool filterDungeons;
        public bool filterTemples;
        public bool filterHomes;
        public bool filterTowns;
        public bool sleepInn = true;
        public bool speedCautious = true;
        public bool travelShip = true;
    }

    #endregion

    #region Climbing Data

    [fsObject("v1")]
    public struct AdvancedClimbingData_v1
    {
        public bool isClimbing;
        public float climbingStartTimer;
        public float climbingContinueTimer;
        public Vector3 wallDirection;
        public Vector3 myLedgeDirection;
    }

    #endregion
}