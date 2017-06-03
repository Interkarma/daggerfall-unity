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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Index of a single Daggerfall texture.
    /// </summary>
    [Serializable]
    public struct DaggerfallTextureIndex
    {
        public int archive;
        public int record;
        public int frame;
    }

    /// <summary>
    /// Settings in for GetTexture methods.
    /// </summary>
    public struct GetTextureSettings
    {
        public int archive;                         // Texture archive index (e.g. 210 for TEXTURE.210)
        public int record;                          // Texture record index
        public int frame;                           // Texture frame index for animated textures
        public int alphaIndex;                      // Native index to receive transparent alpha (-1 to disable)
        public int emissionIndex;                   // Native index to become emissive on emission maps when enabled
        public int borderSize;                      // Number of pixels border to add around image
        public bool copyToOppositeBorder;           // Copy texture edges to opposite border. Requires border, mutually exclusive with dilate
        public bool dilate;                         // Blend texture into surrounding empty pixels. Requires border
        public bool stayReadable;                   // Texture will remain in memory and can be read after creation
        public bool sharpen;                        // Sharpen image
        public bool createNormalMap;                // Normal map will be created based on strength
        public bool createEmissionMap;              // Emission map will also be created based on emissionIndex
        public float normalStrength;                // Strength of generated normals
        public int atlasPadding;                    // Number of pixels padding around each sub-texture
        public int atlasMaxSize;                    // Max size of atlas
        public int atlasShrinkUVs;                  // Number of extra pixels to shrink UV rect
        public bool autoEmission;                   // Automatically create emission map for known textures
        public bool autoEmissionForWindows;         // Automatically create emission map for window textures
        public TextureFile textureFile;             // Provide an already open texture file to save time
    }

    /// <summary>
    /// Results out for GetTexture methods.
    /// </summary>
    public struct GetTextureResults
    {
        public Texture2D albedoMap;                 // Albedo texture out for all colour textures
        public Texture2D normalMap;                 // Normal texture out when normals are enabled
        public Texture2D emissionMap;               // Emission texture out for emissive textures
        public List<int> atlasFrameCounts;          // List of atlas frame counts for each texture
        public Rect singleRect;                     // Receives UV rect for texture inside border
        public List<Rect> atlasRects;               // List of rects, one for each record sub-texture and frame
        public List<RecordIndex> atlasIndices;      // List of record indices into rect array, accounting for animation frames
        public List<Vector2> atlasSizes;            // List of sizes for each texture
        public List<Vector2> atlasScales;           // List of scales for each texture
        public List<Vector2> atlasOffsets;          // List of offsets for each texture
        public bool isWindow;                       // Flag is raised if this is a window texture, for single textures only
        public bool isEmissive;                     // Flag is raised is this texture is emissive
        public bool isAtlasAnimated;                // Atlas texture has one or more animations
        public TextureFile textureFile;             // Texture file used for last read operation.
    }

    /// <summary>
    /// Some information about a climate texture, returned by climate parser.
    /// </summary>
    [Serializable]
    public struct ClimateTextureInfo
    {
        public DFLocation.ClimateTextureGroup textureGroup;
        public DFLocation.ClimateTextureSet textureSet;
        public bool supportsWinter;
        public bool supportsRain;
    }

    /// <summary>
    /// Defines a single cached material and related properties.
    /// Marked as serializable but not currently serialized.
    /// </summary>
    [Serializable]
    public struct CachedMaterial
    {
        // Keys
        public int key;                             // Key of this material
        public int keyGroup;                        // Group of this material

        // Textures
        public Texture2D albedoMap;                 // Albedo texture of material
        public Texture2D normalMap;                 // Normal texture of material
        public Texture2D emissionMap;               // Emission texture of material

        // Material
        public Material material;                   // Shared material
        public Rect singleRect;                     // Rect for single material
        public Rect[] atlasRects;                   // Array of rects for atlased materials
        public RecordIndex[] atlasIndices;          // Array of record indices into atlas rect array
        public FilterMode filterMode;               // Filter mode of this material
        public int singleFrameCount;                // Number of frames in single animated material
        public int[] atlasFrameCounts;              // Array of frame counts for animated materials

        // Windows
        public bool isWindow;                       // True if this is a window material
        public Color windowColor;                   // Colour of this window
        public float windowIntensity;               // Intensity of this window

        // Size and scale
        public Vector2[] recordSizes;               // Size of texture records
        public Vector2[] recordScales;              // Scale of texture records
        public Vector2[] recordOffsets;             // Offset of texture records
    }

    /// <summary>
    /// Data package returned by ImageHelper.GetImageData() methods.
    /// Supplied Texture2D will always be readable.
    /// </summary>
    public struct ImageData
    {
        public ImageTypes type;                 // Original type
        public string filename;                 // Original filename
        public int record;                      // Original record
        public int frame;                       // Original frame
        public bool hasAlpha;                   // Original loaded with alpha cutout
        public int width;                       // Original image width
        public int height;                      // Original image height
        public DFBitmap dfBitmap;               // Original indexed bitmap
        public Texture2D texture;               // Generated texture
        public DFPosition offset;               // Custom Daggerfall offset position for paper doll inventory, etc.
        public DFSize scale;                    // Custom Daggerfall size for scaling sprites
        public DFSize size;                     // Size of image
    }

    /// <summary>
    /// Defines animation setup for mobile enemies.
    /// </summary>
    [Serializable]
    public struct MobileAnimation
    {
        public int Record;                          // Index of this animation
        public int NumFrames;                       // Number of frames in this animation
        public int FramePerSecond;                  // Speed at which this animation plays
        public bool FlipLeftRight;                  // True if animation flipped left-to-right
    }

    /// <summary>
    /// Defines basic properties of mobile enemies.
    /// </summary>
    [Serializable]
    public struct MobileEnemy
    {
        public int ID;                              // ID of this mobile
        public string Name;                         // In-game name of this mobile
        public MobileBehaviour Behaviour;           // Behaviour of mobile
        public MobileAffinity Affinity;             // Affinity of mobile
        public MobileGender Gender;                 // Gender of mobile
        public MobileReactions Reactions;           // Mobile reaction setting
        public MobileCombatFlags CombatFlags;       // Mobile combat flags
        public int MaleTexture;                     // Texture archive index for male sprite
        public int FemaleTexture;                   // Texture archive index for female sprite
        public int CorpseTexture;                   // Corpse texture archive:record bits
        public bool HasIdle;                        // Has standard Idle animation group
        public bool HasRangedAttack1;               // Has RangedAttack1 animation group
        public bool HasRangedAttack2;               // Has RangedAttack2 animation group
        public bool CanOpenDoors;                   // Enemy can open doors to pursue player
        public bool PrefersRanged;                  // Enemy prefers ranged attacks and spells over melee
        public int BloodIndex;                      // Index in TEXTURE.380 for blood splash 
        public int MoveSound;                       // Index of enemy "moving around" sound
        public int BarkSound;                       // Index of enemy "bark" or "shout" sound
        public int AttackSound;                     // Index of enemy "attack" sound
        public int SightModifier;                   // +/- range of vision for acute/impaired sight
        public int HearingModifier;                 // +/- range of hearing for acute/impaired hearing
        public MetalTypes MinMetalToHit;            // Minimum metal type required to hit enemy
        public int MinDamage;                       // Minimum damage per hit
        public int MaxDamage;                       // Maximum damage per hit
        public int MinHealth;                       // Minimum health
        public int MaxHealth;                       // Maximum health
        public string LootTableKey;                 // Key to use when generating loot
    }

    /// <summary>
    /// Defines a list of random encounters based on dungeon type (Crypt, Orc Stronghold, etc.).
    /// </summary>
    [Serializable]
    public struct RandomEncounterTable
    {
        public DFRegion.DungeonTypes DungeonType;
        public MobileTypes[] Enemies;
    }

    /// <summary>
    /// A record index for cached materials.
    /// Used to align indices in atlased textures.
    /// </summary>
    [Serializable]
    public struct RecordIndex
    {
        public int startIndex;                      // Index of first frame in atlas rect array
        public int frameCount;                      // Number of frames in this record
        public int width;                           // Width in pixels of this record, excluding border and padding
        public int height;                          // Height in pixels of this record, excluding border and padding
    }

    /// <summary>
    /// Defines model data.
    /// </summary>
    [Serializable]
    public struct ModelData
    {
        public DFMesh DFMesh;                       // Native ngon geometry as read from ARCH3D.BSA
        public Vector3[] Vertices;                  // Vector3 array containing position data
        public Vector3[] Normals;                   // Vector3 array containing normal data
        public Vector2[] UVs;                       // Vector2 array containing UV data
        public int[] Indices;                       // Index array describing the triangles of this mesh
        public SubMeshData[] SubMeshes;             // Data for each SubMesh, grouped by texture
        public ModelDoor[] Doors;                   // Doors found in this model

        /// <summary>
        /// Defines submesh data.
        /// </summary>
        [Serializable]
        public struct SubMeshData
        {
            public int StartIndex;                  // Location in the index array at which to start reading vertices
            public int PrimitiveCount;              // Number of primitives in this submesh
            public int TextureArchive;              // Texture archive index
            public int TextureRecord;               // Texture record index
        }
    }

    /// <summary>
    /// Defines a door found in model data.
    /// </summary>
    [Serializable]
    public struct ModelDoor
    {
        public int Index;                           // Index of this door in model data
        public DoorTypes Type;                      // Type of door found (building, dungeon, etc.)
        public Vector3 Vert0;                       // Vertex 0
        public Vector3 Vert1;                       // Vertex 1
        public Vector3 Vert2;                       // Vertex 2
        public Vector3 Normal;                      // Normal facing away from door
    }

    /// <summary>
    /// Defines animation setup for weapons.
    /// </summary>
    [Serializable]
    public struct WeaponAnimation
    {
        public int Record;                          // Index of this animation
        public int NumFrames;                       // Number of frames in this animation
        public int FramePerSecond;                  // Speed at which this animation plays
        public WeaponAlignment Alignment;           // Side of screen to align animation
        public float Offset;                        // Offset from edge of screen in 0-1 range, ignored for WeaponAlignment.Center
    }

    /// <summary>
    /// Defines a static door for hit tests inside a scene.
    /// </summary>
    [Serializable]
    public struct StaticDoor
    {
        public int buildingKey;                     // Unique key of building this door will access
        public Vector3 ownerPosition;               // World position of door owner
        public Quaternion ownerRotation;            // Rotation of door owner
        public Matrix4x4 buildingMatrix;            // Matrix of individual building owning this door
        public DoorTypes doorType;                  // Type of door
        public int blockIndex;                      // Block index in BLOCKS.BSA
        public int recordIndex;                     // Record index for interior
        public int doorIndex;                       // Door index for individual building/record (most buildings have only 1-2 doors)
        public Vector3 centre;                      // Door centre in model space
        public Vector3 size;                        // Door size in model space
        public Vector3 normal;                      // Normal pointing away from door
    }

    /// <summary>
    /// Defines a static building for hit tests inside scene.
    /// </summary>
    [Serializable]
    public struct StaticBuilding
    {
        public int buildingKey;                     // Building key unique to parent location
        public Matrix4x4 modelMatrix;               // Matrix of individual model for this building
        public int recordIndex;                     // Record index for building
        public Vector3 centre;                      // Building centre in model space
        public Vector3 size;                        // Building size
    }

    /// <summary>
    /// Detailed information about a building for directory lookups.
    /// </summary>
    [Serializable]
    public struct BuildingSummary
    {
        public int buildingKey;                         // Building key unique to parent location
        public int NameSeed;                            // Name seed of building - not set at block level
        public int FactionId;                           // Faction ID of building
        public DFLocation.BuildingTypes BuildingType;   // Type of building
        public int Quality;                             // Quality of building
        public Vector3 Position;                        // Position of building
        public Vector3 Rotation;                        // Rotation of building
        public Matrix4x4 Matrix;                        // Transform matrix of building
    }

    /// <summary>
    /// Information about a single map pixel for streaming world.
    /// </summary>
    [Serializable]
    public struct MapPixelData
    {
        public bool inWorld;                        // True if map pixel is inside world area
        public int mapPixelX;                       // Map pixel X coordinate
        public int mapPixelY;                       // Map pixel Y coordinate
        public int worldHeight;                     // Height of this pixel (not scaled)
        public int worldClimate;                    // Climate of this pixel
        public int worldPolitic;                    // Politics of this pixel
        public bool hasLocation;                    // True if location present
        public int mapRegionIndex;                  // Map region index (if location present)
        public int mapLocationIndex;                // Map location index (if location present)
        public int locationID;                      // Location ID (if location present)
        public string locationName;                 // Location name (if location present)
        public float averageHeight;                 // Average height of terrain for location placement
        public float maxHeight;                     // Max height of terrain for location placement
        public Rect locationRect;                   // Rect of location tiles in sample are
        
        [HideInInspector, NonSerialized]
        public TilemapSample[,] tilemapSamples;     // Tilemap samples for terrain

        [HideInInspector, NonSerialized]
        public float[,] heightmapSamples;           // Heightmap samples for terrain - indexed [y,x] for Terrain.SetHeights
    }

    /// <summary>
    /// Full site details of a site/location generated by Place quest resource.
    /// </summary>
    [Serializable]
    public struct SiteDetails
    {
        public int mapId;                           // MapID of this location
        public uint locationId;                     // LocationID of this location
        public string regionName;                   // Name of region containing this location
        public string locationName;                 // Name of exterior location itself
        public bool isBuilding;                     // True when target site is a specific building
        public int buildingKey;                     // Key of building site in this location
        public string buildingName;                 // Name of target building, e.g. 'The Odd Blades'
    }

    /// <summary>
    /// Details of a quest NPC generated by Person quest resource.
    /// </summary>
    [Serializable]
    public struct PersonDetails
    {
        public int factionId;                       // Faction ID to lookup NPC details
        public bool atHome;                         // Individual NPC is at home or away
    }

    /// <summary>
    /// Describes a single tilemap sample.
    /// </summary>
    public struct TilemapSample
    {
        public int record;                          // Record index into texture atlas
        public bool flip;                           // Flip texture UVs
        public bool rotate;                         // Rotate texture UVs
        public bool location;                       // True if location tile present
        public int nature;                          // Index of nature flat at this point (0 is nothing)
    }

    /// <summary>
    /// Used to create matrix of chances by loot table key.
    /// </summary>
    public struct LootChanceMatrix
    {
        public string key;
        public int MinGold;
        public int MaxGold;
        public int P1;      // PlantIngredients1
        public int P2;      // PlantIngredients2
        public int C1;      // CreatureIngredients1
        public int C2;      // CreatureIngredients2
        public int C3;      // CreatureIngredients3
        public int M1;      // MiscellaneousIngredients1
        public int AM;      // Armor
        public int WP;      // Weapons
        public int MI;      // Magic item
        public int CL;      // Clothing
        public int BK;      // Book
        public int M2;      // MiscellaneousIngredients2
        public int RL;      // ReligiousItems
    }
}