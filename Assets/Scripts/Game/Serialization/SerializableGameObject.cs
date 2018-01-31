// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyldf@gmail.com)
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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Serialization
{
    /// <summary>
    /// Enum of stateful game object types that implement ISerializableGameObject.
    /// To add a new type of stateful game object:
    ///     - add type name here
    ///     - add a condition to SerializableStateManager.GetStatefulGameObjectType()
    ///     - add serializer methods
    ///     - add to SerializableStateManager.CacheScene() & RestoreCachedScene()
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
        public TransportModes transportMode;
        public PlayerPositionData_v1 boardShipPosition;  // Holds the player position from before boarding a ship.
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
        public int maxHealth;
        public int currentHealth;
        public int currentFatigue;
        public int currentMagicka;
        public short[] skillUses;
        public uint timeOfLastSkillIncreaseCheck;
        public int startingLevelUpSkillSum;
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
        public uint timeOfLastSkillTraining;
        public PlayerEntity.RegionDataRecord[] regionData;
    }

    [fsObject("v1")]
    public class PlayerPositionData_v1
    {
        public Vector3 position;
        public float yaw;
        public float pitch;
        public bool isCrouching;
        public int worldPosX;
        public int worldPosZ;
        public bool insideDungeon;
        public bool insideBuilding;
        public string terrainSamplerName;
        public int terrainSamplerVersion;
        public StaticDoor[] exteriorDoors;
        public PlayerGPS.DiscoveredBuilding buildingDiscoveryData;
        public WeatherType weather;
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
        public Quaternion currentRotation;
        public bool isDead;
        public int startingHealth;
        public int currentHealth;
        public int currentFatigue;
        public int currentMagicka;
        public EntityTypes entityType;
        public string careerName;
        public int careerIndex;
        public bool isHostile;
        public bool questSpawn;
        public MobileGender mobileGender;
        public ItemData_v1[] items;
        public ulong[] equipTable;
        public QuestResourceBehaviour.QuestResourceSaveData_v1 questResource;
    }

    #endregion

    #region Loot Data

    [fsObject("v1")]
    public class LootContainerData_v1
    {
        public ulong loadID;
        //public int worldKey;
        //public WorldContext worldContext;
        public LootContainerTypes containerType;
        public InventoryContainerImages containerImage;
        public Vector3 currentPosition;
        //public Vector3 localPosition;
        public int textureArchive;
        public int textureRecord;
        public string lootTableKey;
        public string entityName;
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
    }

    #endregion

    #region Bank Data

    [fsObject("v1")]
    public class BankDeedData_v1
    {
        public int shipType;
        public HouseDeedData_v1 houseDeed;
    }

    [fsObject("v1")]
    public class HouseDeedData_v1
    {
        public int houseId;
    }

    [fsObject("v1")]
    public class BankRecordData_v1
    {
        public int accountGold;
        public int loanTotal;
        public uint loanDueDate;
        public int regionIndex;
    }

    #endregion
}