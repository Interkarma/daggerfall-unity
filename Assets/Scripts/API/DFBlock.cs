// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyl@dfworkshop.net)
// 
// Notes:
//

#region Using Statements
using FullSerializer;
using System;
using System.Collections.Generic;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores information about a block record. This is fundamentally equivalent to native block data,
    ///  but is type-safe and structured for use.
    /// </summary>
    public struct DFBlock
    {
        public const int RMBTilesPerBlock = 16;

        #region Structure Variables

        /// <summary>
        /// Starting offset of the block in BLOCKS.BSA.
        /// </summary>
        public long Position;

        /// <summary>
        /// Index of the block in BLOCKS.BSA.
        /// </summary>
        public int Index;

        /// <summary>
        /// Name of the block in BLOCKS.BSA.
        /// </summary>
        public string Name;

        /// <summary>
        /// Defines the type of block data stored.
        /// </summary>
        public BlockTypes Type;

        /// <summary>
        /// Contains RMB (city) block data.
        /// </summary>
        public RmbBlockDesc RmbBlock;

        /// <summary>
        /// Contains RDB (dungeon) block data.
        /// </summary>
        public RdbBlockDesc RdbBlock;

        /// <summary>
        /// Contains RDI (unknown) block data.
        /// </summary>
        public RdiBlockDesc RdiBlock;

        #endregion

        #region Child Structures

        /// <summary>
        /// Possible block types enumeration.
        /// </summary>
        public enum BlockTypes
        {
            /// <summary>Block type is not defined.</summary>
            Unknown,

            /// <summary>A city block.</summary>
            Rmb,

            /// <summary>A dungeon block.</summary>
            Rdb,

            /// <summary>RDI blocks are currently unsupported and should be ignored.</summary>
            Rdi,
        }

        /// <summary>
        /// RDB block types, derived from first letter of block name.
        ///  RdbTypes.Start is a special case as this can only be derived from
        ///  map data. The value is here in case you want to track start blocks.
        /// </summary>
        public enum RdbTypes
        {
            /// <summary>Rdb type is not defined.</summary>
            Unknown,

            /// <summary>Border block used to seal dungeon.</summary>
            Border,

            /// <summary>Normal block.</summary>
            Normal,

            /// <summary>Flooded block.</summary>
            Wet,

            /// <summary>Used in main quest.</summary>
            Quest,

            /// <summary>Crypt block.</summary>
            Mausoleum,

            /// <summary>Optional. Can be used when tracking start block.</summary>
            Start,
        }

        public enum EnemyReactionTypes
        {
            /// <summary>Enemy will attack player on sight.</summary>
            Hostile = 0,

            /// <summary>Enemy will only attack when provoked (e.g. guards in castles).</summary>
            Passive = 99,
        }

        public enum EnemyGenders
        {
            Unspecified = 0,
            Female = 1,
            Male = 2,
        }

        #endregion

        #region RMB Structures

        /// <summary>
        /// An RMB Block has this basic structure.
        /// </summary>
        public struct RmbBlockDesc
        {
            /// <summary>Fixed length data (FLD) header.</summary>
            public RmbBlockFld FldHeader;

            /// <summary>Contains block subrecords for exterior and interior object.</summary>
            public RmbSubRecord[] SubRecords;

            /// <summary>Defines additional 3D objects positioned arbitrarily inside block.</summary>
            public RmbBlock3dObjectRecord[] Misc3dObjectRecords;

            /// <summary>Defines additional flat (billboard) objects positioned arbitrarily inside block.</summary>
            public RmbBlockFlatObjectRecord[] MiscFlatObjectRecords;
        }

        /// <summary>
        /// Fixed length data (FLD) header of block record.
        /// </summary>
        public struct RmbBlockFld
        {
            /// <summary>Count of block records.</summary>
            public Byte NumBlockDataRecords;

            /// <summary>Count of miscellaneous 3D object records.</summary>
            public Byte NumMisc3dObjectRecords;

            /// <summary>Count of miscellaneous flat object records.</summary>
            public Byte NumMiscFlatObjectRecords;

            /// <summary>Position of block record in 3D space (array is 32 records long, but only up to numBlockDataRecords have valid data).</summary>
            public RmbFldBlockPositions[] BlockPositions;

            /// <summary>Building data defining quality of building, etc.</summary>
            public DFLocation.BuildingData[] BuildingDataList;

            /// <summary>Unknown data.</summary>
            internal UInt32[] Section2UnknownData;
            //internal Byte[] Section2UnknownData;

            /// <summary>Length of block record data in bytes (array is 32 records long, but only up to numBlockDataRecords have valid data).</summary>
            public Int32[] BlockDataSizes;

            /// <summary>Ground data to draw under a city block, such as ground textures, trees, rocks, etc.</summary>
            public RmbFldGroundData GroundData;

            /// <summary>A 64x64 pixel automap image.</summary>
            public Byte[] AutoMapData;

            /// <summary>Name of this block. This always matches the BSA record name for this block.</summary>
            public String Name;

            /// <summary>Array of 32 other names for this block. Unknown purpose.</summary>
            public String[] OtherNames;
        }

        /// <summary>
        /// Position of the block in 3D space.
        /// </summary>
        public struct RmbFldBlockPositions
        {
            /// <summary>Unknown.</summary>
            internal UInt32 Unknown1;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown2;

            /// <summary>X position of block in 3D space.</summary>
            public Int32 XPos;

            /// <summary>Z position of block in 3D space.</summary>
            public Int32 ZPos;

            /// <summary>Y rotation.</summary>
            public Int32 YRotation;
        }

        /// <summary>
        /// Describes a single ground tile and how to orient the texture. The texture archive to use is based on the region.
        /// </summary>
        public struct RmbGroundTiles
        {
            /// <summary>Texture and alignment data compressed to a bitfield.</summary>
            public Byte TileBitfield;

            /// <summary>Texture record from bitfield. Used to determine which texture record to load from regional archive.</summary>
            public Int32 TextureRecord;

            /// <summary>When true the texture should be rotated 90 degrees so its width becomes it height.</summary>
            public Boolean IsRotated;

            /// <summary>When true the texture should be flipped in both X and Y directions.</summary>
            public Boolean IsFlipped;
        }

        /// <summary>
        /// Describes a single flat scenery item (tree, rock, etc.) for this block.
        /// </summary>
        public struct RmbGroundScenery
        {
            /// <summary>Texture and some unknown data compressed into a bitfield</summary>
            internal Byte TileBitfield;

            /// <summary>Unknown data.</summary>
            internal Int32 Unknown1;

            /// <summary>
            /// Texture record from bitfield. Used to determine which texture record to load from regional archive.
            ///  A negative value means no scenery in this spot.
            /// </summary>
            public Int32 TextureRecord;
        }

        /// <summary>
        /// Ground data to draw under a city block, such as ground textures, trees, rocks, etc.
        /// </summary>
        [fsObject(Converter = typeof(RmbGroundDataConverter))]   // FullSerializer can't do 2D arrays out of the box
        public struct RmbFldGroundData
        {
            /// <summary>Header with unknown data.</summary>
            public Byte[] Header;

            /// <summary>A 16x16 array of ground tiles. Each tile is 256x256 world units.</summary>
            public RmbGroundTiles[,] GroundTiles;

            /// <summary>
            /// A 16x16 array of ground scenery data (trees, rocks, etc.). These are spaced every 256 world units
            ///  (left to right, top to bottom). In effect, the scenery is placed in the "corners" of the ground tiles.
            /// </summary>
            public RmbGroundScenery[,] GroundScenery;
        }

        #endregion

        #region RMB Resources

        /// <summary>
        /// An RMB subrecord has a repeating set of data.
        ///  The first set is for the exterior of the block (e.g. a tavern exterior).
        ///  The second is for the interior (e.g. the walls, tables, and chairs inside the tavern).
        /// </summary>
        public struct RmbSubRecord
        {
            /// <summary>X position of block in 3D space.</summary>
            public Int32 XPos;

            /// <summary>Z position of block in 3D space.</summary>
            public Int32 ZPos;

            /// <summary>Y rotation.</summary>
            public Int32 YRotation;

            /// <summary>Exterior block data records.</summary>
            public RmbBlockData Exterior;

            /// <summary>Interior block data records.</summary>
            public RmbBlockData Interior;
        }

        /// <summary>
        /// Defines block resources stored in this subrecord.
        ///  Check header for how many resources of any type are to be found in this definition.
        /// </summary>
        public struct RmbBlockData
        {
            /// <summary>Header data.</summary>
            public RmbBlockHeader Header;

            /// <summary>3D objects to position around the block.</summary>
            public RmbBlock3dObjectRecord[] Block3dObjectRecords;

            /// <summary>Flat (billboard) objects to position around the block.</summary>
            public RmbBlockFlatObjectRecord[] BlockFlatObjectRecords;

            /// <summary>Unknown data.</summary>
            public RmbBlockSection3Record[] BlockSection3Records;

            /// <summary>People (NPCs) to position around the block.</summary>
            public RmbBlockPeopleRecord[] BlockPeopleRecords;

            /// <summary>Doors the player can open and close to position around the block.</summary>
            public RmbBlockDoorRecord[] BlockDoorRecords;
        }

        /// <summary>
        /// RMB block data header.
        /// </summary>
        public struct RmbBlockHeader
        {
            /// <summary>The start position in the BSA record data stream. Primarily used by block readers.</summary>
            internal long Position;

            /// <summary>Count of 3D object records.</summary>
            public Byte Num3dObjectRecords;

            /// <summary>Count of flat (billboard) object records.</summary>
            public Byte NumFlatObjectRecords;

            /// <summary>Count of unknown data records.</summary>
            public Byte NumSection3Records;

            /// <summary>Count of people (NPC) records.</summary>
            public Byte NumPeopleRecords;

            /// <summary>Count of door records.</summary>
            public Byte NumDoorRecords;

            /// <summary>Unknown.</summary>
            internal Int16 Unknown1;

            /// <summary>Unknown.</summary>
            internal Int16 Unknown2;

            /// <summary>Unknown.</summary>
            internal Int16 Unknown3;

            /// <summary>Unknown.</summary>
            internal Int16 Unknown4;

            /// <summary>Unknown.</summary>
            internal Int16 Unknown5;

            /// <summary>Unknown.</summary>
            internal Int16 Unknown6;
        }

        /// <summary>
        /// 3D object data, such as buildings, walls, tables, cages, etc.
        /// </summary>
        ///
        [fsObject(Processor = typeof(RmbBlock3dObjectRecordProcessor))]
        public struct RmbBlock3dObjectRecord
        {
            /// <summary>ID of model to be loaded.</summary>
            public String ModelId;

            /// <summary>Model ID parsed to UInt32.</summary>
            public UInt32 ModelIdNum;

            /// <summary>Used to find the ObjectID of the mesh to use in conjuction with objectId2.</summary>
            internal Int16 ObjectId1;

            /// <summary>Used to find the ObjectID of the mesh to use in conjuction with objectId1.</summary>
            internal Byte ObjectId2;

            /// <summary>Object type for an unknown enumeration. 0x03 = misc indoor objects.</summary>
            public Byte ObjectType;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown1;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown2;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown3;

            /// <summary>Unknown.</summary>
            internal UInt64 NullValue1;

            /// <summary>Unknown X position.</summary>
            internal Int32 XPos1;

            /// <summary>Unknown Y position.</summary>
            internal Int32 YPos1;

            /// <summary>Unknown Z position.</summary>
            internal Int32 ZPos1;

            /// <summary>X position in 3D space.</summary>
            public Int32 XPos;

            /// <summary>Y position in 3D space.</summary>
            public Int32 YPos;

            /// <summary>Z position in 3D space.</summary>
            public Int32 ZPos;

            /// <summary>X scale</summary>
            public float XScale;

            /// <summary>Y scale</summary>
            public float YScale;

            /// <summary>Z scale</summary>
            public float ZScale;

            /// <summary>Unknown.</summary>
            internal UInt32 NullValue2;

            /// <summary>X rotation.</summary>
            public Int16 XRotation;

            /// <summary>Y rotation.</summary>
            public Int16 YRotation;

            /// <summary>Z rotation.</summary>
            public Int16 ZRotation;

            /// <summary>Unknown.</summary>
            internal UInt16 Unknown4;

            /// <summary>Unknown.</summary>
            internal UInt32 NullValue3;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown5;

            /// <summary>Unknown.</summary>
            internal UInt16 NullValue4;
        }

        /// <summary>
        /// Flat object (billboard) data, such as haystacks and horses.
        /// </summary>
        public struct RmbBlockFlatObjectRecord
        {
            /// <summary>Position of this record in stream.</summary>
            public long Position;

            /// <summary>X position in 3D space.</summary>
            public Int32 XPos;

            /// <summary>Y position in 3D space.</summary>
            public Int32 YPos;

            /// <summary>Z position in 3D space.</summary>
            public Int32 ZPos;

            /// <summary>Index of texture compressed to a bitfield.</summary>
            internal UInt16 TextureBitfield;

            /// <summary>Texture archive from bitfield. Used to determine which texture file to load (e.g. TEXTURE.210).</summary>
            public int TextureArchive;

            /// <summary>Texture record from bitfield. Used to determine which texture record to load from archive.</summary>
            public int TextureRecord;

            /// <summary>NPC faction. (for exterior NPCs)</summary>
            public Int16 FactionID;

            /// <summary> NPC flags. (Same as RmbBlockPeopleRecord, but for exterior NPCs)</summary>
            public Byte Flags;
        }

        /// <summary>
        /// Records of an unknown type.
        /// Only appears in interiors and forms a grid-like pattern over floor.
        /// Most likely path-finding waypoints.
        /// </summary>
        public struct RmbBlockSection3Record
        {
            /// <summary>X position in 3D space.</summary>
            public Int32 XPos;

            /// <summary>Y position in 3D space.</summary>
            public Int32 YPos;

            /// <summary>Z position in 3D space.</summary>
            public Int32 ZPos;

            /// <summary>Unknown.</summary>
            internal Byte Unknown1;

            /// <summary>Unknown.</summary>
            internal Byte Unknown2;

            /// <summary>Unknown.</summary>
            internal Int16 Unknown3;
        }

        /// <summary>
        /// People (NPCs), such as shopkeepers and quest givers.
        /// </summary>
        public struct RmbBlockPeopleRecord
        {
            /// <summary>Position of this record in stream.</summary>
            public long Position;

            /// <summary>X position in 3D space.</summary>
            public Int32 XPos;

            /// <summary>Y position in 3D space.</summary>
            public Int32 YPos;

            /// <summary>Z position in 3D space.</summary>
            public Int32 ZPos;

            /// <summary>Index of texture compressed to a bitfield.</summary>
            internal UInt16 TextureBitfield;

            /// <summary>Texture archive from bitfield. Used to determine which texture file to load (e.g. TEXTURE.210).</summary>
            public int TextureArchive;

            /// <summary>Texture record from bitfield. Used to determine which texture record to load from archive.</summary>
            public int TextureRecord;

            /// <summary>NPC faction.</summary>
            public Int16 FactionID;

            /// <summary>
            /// NPC flags. Known so far:
            /// 00X00000    : X=0 is male, X=1 is female
            /// </summary>
            public Byte Flags;
        }

        /// <summary>
        /// Doors the player can open and close.
        /// </summary>
        public struct RmbBlockDoorRecord
        {
            /// <summary>Offset of this object from start of RMB record. Not required unless you are extending the block reader.</summary>
            public Int32 Position;

            /// <summary>X position in 3D space.</summary>
            public Int32 XPos;

            /// <summary>Y position in 3D space.</summary>
            public Int32 YPos;

            /// <summary>Z position in 3D space.</summary>
            public Int32 ZPos;

            /// <summary>Y Rotation at starting position.</summary>
            public Int16 YRotation;

            /// <summary>Angle to rotate door into open position.</summary>
            public Int16 OpenRotation;

            /// <summary>Model index to use for door as offset from base door ID.</summary>
            public Byte DoorModelIndex;

            /// <summary>Unknown.</summary>
            internal Byte Unknown;

            /// <summary>Unknown.</summary>
            internal Byte NullValue1;
        }

        #endregion

        #region RDB Structures

        /// <summary>
        /// Resource types enumeration.
        /// </summary>
        public enum RdbResourceTypes
        {
            /// <summary>3D model resource.</summary>
            Model = 0x01,

            /// <summary>Light resource.</summary>
            Light = 0x02,

            /// <summary>Flat resource.</summary>
            Flat = 0x03,
        }

        /// <summary>
        /// Gender enumeration for NPC flats.
        /// </summary>
        public enum RdbFlatGenders
        {
            /// <summary>No gender specified.</summary>
            Unspecified = 0,

            /// <summary>Mobile is female</summary>
            FemaleMobile = 1,

            /// <summary>Mobile is male</summary>
            MaleMobile = 2,

            /// <summary>NPC is male.</summary>
            Male = 0x1200,

            /// <summary>NPC is female.</summary>
            Female = 0x3200,
        }

        /// <summary>
        /// Action axis enumeration.
        /// </summary>
        public enum RdbActionAxes
        {
            /// <summary>Axis unspecified.</summary>
            None = 0x00,

            /// <summary>Negative X axis.</summary>
            NegativeX = 0x01,

            /// <summary>Positive X axis.</summary>
            PositiveX = 0x02,

            /// <summary>Negative Y axis.</summary>
            NegativeY = 0x03,

            /// <summary>Positive Y axis.</summary>
            PositiveY = 0x04,

            /// <summary>Negative Y axis.</summary>
            NegativeZ = 0x05,

            /// <summary>Positive Z axis.</summary>
            PositiveZ = 0x06,

        }

        /// <summary>
        /// Action flags enumeration.
        /// These are Still being researched, and will be updated/changed in future.
        /// </summary>
        [Flags]
        public enum RdbActionFlags
        {

            ///<summary>None </summary>
            None = 0x00,

            ///<summary>1 Translation. </summary>
            Translation = 0x01,

            ///<summary>2 Unknown. </summary>
            PositiveX = 0x02,

            ///<summary>3 Unknown </summary>
            NegativeX = 0x03,

            ///<summary>4 Unknown - seems to just be translation?</summary>
            PositiveY = 0x04,

            ///<summary>5 Unknown </summary>
            NegativeY = 0x05,

            ///<summary>6 Unknown </summary>
            PositiveZ = 0x06,

            ///<summary>7 Unknown </summary>
            NegativeZ = 0x07,

            ///<summary>8 Rotation. </summary>
            Rotation = 0x08,

            ///<summary>9 Cast spell / spell effect. </summary>
            CastSpell = 0x09,

            ///<summary>11 Appears to display a text on activation </summary>
            ShowText = 0x0B,

            ///<summary>12 Shows text and gets input, the guard in castle daggerfall, the banner in Shedugant </summary>
            ShowTextWithInput = 0x0C,

            ///<summary>14 Teleport - needs target object </summary>
            Teleport = 0x0E,

            ///<summary>16 Locks door on activation, doesn't close.</summary>
            LockDoor = 0x10,

            ///<summary>17 Unlock door - only activates once? </summary>
            UnlockDoor = 0x11,

            ///<summary>18 Unlock + open door, like the first 2 doors in Daggerfall Castle - only activates once? </summary>
            OpenDoor = 0x12,

            ///<summary>20 Close door, lock if it has a starting lock value </summary>
            CloseDoor = 0x14,

            ///<summary>21 Hurt Player, random range </summary>
            Hurt21 = 0x15,

            ///<summary>22 Hurt player, damage = level * magnitude </summary>
            Hurt22 = 0x16,

            ///<summary>23 Hurt player, damage = level * magnitude</summary>
            Hurt23 = 0x17,

            ///<summary>24 Hurt player, damage = level * magnitude</summary>
            Hurt24 = 0x18,

            ///<summary>25 Hurt player, damage = level * magnitude</summary>
            Hurt25 = 0x19,

            ///<summary>26 Seems to Poison Player</summary>
            Poison = 0x1A,

            ///<summary>27 Unknown</summary>
            Unknown27 = 0x1B,

            ///<summary>28 drain magicka. 1 magnitude = 1 pt magica </summary>
            DrainMagicka = 0x1C,

            ///<summary>29 Dialogue. Seems to ignore trigger flag</summary>
            Dialogue = 0x1D,

            ///<summary>30 Activate </summary>
            Activate = 0x1E,

            ///<summary>Sets a global variable in quest system - only used on 2 main quest objects.</summary>
            SetGlobalVar = 0x1F,

            ///<summary>32  Unknown, only on 4 objects </summary>
            Unknown32 = 0x20,

            ///<summary>50 Unknown </summary>
            Unknown50 = 0x32,

            ///<summary>99 Displays text at the top of the screen when clicked in info mode.
            ///            Can cause castle guards to go hostile if clicked outside of info mode.</summary>
            DoorText = 0x63,

            ///<summary>100 Unknown, only on 2 objects</summary>
            Unknown100 = 0x64,
        }

        /// <summary>
        /// Control how actions are activated. 
        /// These are Still being researched, and will be updated/changed in future.
        /// </summary>
        [Flags]
        public enum RdbTriggerFlags
        {

            /// <summary> None</summary>
            None = 0x00,

            /// <summary> Activated by collision / walking on; flats + models </summary>
            Collision01 = 0x01,

            /// <summary> Activated by clicking on; flats + models </summary>
            Direct = 0x02,

            /// <summary> Activated by colliding with (not walking on) - models only </summary>
            Collision03 = 0x03,

            /// <summary> Activated by attacking, flats + models </summary>
            Attack = 0x05,

            /// <summary> Seems to work just like flag 2; this appears to be only doors w/ actions; flats + models </summary>
            Direct6 = 0x06,

            /// <summary> Activated by clicking, attacking or colliding with action obj; flats + models</summary>
            MultiTrigger = 0x08,

            /// <summary> Activated by collision / walking on; flats + models </summary>
            Collision09 = 0x09,

            /// <summary> Activated by door opening/closing; doors only (have only tested standard doors - need to check trap doors etc) </summary>
            Door = 0x0A,
        }

        /// <summary>
        /// Unknown model data.
        /// </summary>
        internal struct RdbModelData
        {
            /// <summary>Unknown.</summary>
            internal UInt32 Unknown1;
        }

        /// <summary>
        /// An RDB block has this general structure.
        /// </summary>
        [fsObject(Processor = typeof(RdbBlockDescProcessor))]
        public struct RdbBlockDesc
        {
            /// <summary>Position in stream to find this data.</summary>
            internal long Position;

            /// <summary>RDB block header.</summary>
            internal RdbBlockHeader Header;

            /// <summary>List of 750 sequential model indices.</summary>
            public RdbModelReference[] ModelReferenceList;

            /// <summary>List of 750 4-byte values of unknown use.</summary>
            internal RdbModelData[] ModelDataList;

            /// <summary>Object section header.</summary>
            internal RdbObjectHeader ObjectHeader;

            /// <summary>List of offsets to root of linked list describing objects in this block.</summary>
            public RdbObjectRoot[] ObjectRootList;

            /// <summary>List of unknown objects from RdbObjectHeader.</summary>
            internal RdbUnknownObject[] UnknownObjectList;
        }

        /// <summary>
        /// Block header. Total number of objects in list from root offset is width*height.
        /// </summary>
        internal struct RdbBlockHeader
        {
            /// <summary>Unknown.</summary>
            internal UInt32 Unknown1;

            /// <summary>Width of dungeon block in unknown grid layout.</summary>
            internal UInt32 Width;

            /// <summary>Height of dungeon block in unknown grid layout.</summary>
            internal UInt32 Height;

            /// <summary>Offset to start of object root section.</summary>
            internal UInt32 ObjectRootOffset;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown2;
        }

        /// <summary>
        /// Model reference. Used to locate a specific model in ARCH3D.BSA.
        /// </summary>
        public struct RdbModelReference
        {
            /// <summary>ID of model to be loaded.</summary>
            public String ModelId;

            /// <summary>Model ID parsed to UInt32.</summary>
            public UInt32 ModelIdNum;

            /// <summary>Three letter description of model.</summary>
            public String Description;
        }

        /// <summary>
        /// RDB object header.
        /// </summary>
        internal struct RdbObjectHeader
        {
            /// <summary>Offset to linked list of unknown purpose.</summary>
            internal UInt32 UnknownOffset;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown1;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown2;

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown3;

            /// <summary>Length of dungeon record in bytes.</summary>
            internal UInt32 Length;

            /// <summary>Unknown.</summary>
            internal Byte[] Unknown4;

            /// <summary>Usually the string "DAGR".</summary>
            public String Dagr;

            /// <summary>Unknown.</summary>
            internal Byte[] Unknown5;
        }

        /// <summary>
        /// Unknown object from linked list in RdbObjectHeader.
        /// </summary>
        public struct RdbUnknownObject
        {
            /// <summary>Offset of this object from start of RDB record. Not required unless you are extending the block reader.</summary>
            public Int32 Position;

            /// <summary>Offset to next object from start of RDB record. Not required unless you are extending the block reader.</summary>
            public Int32 Next;

            /// <summary>Index of this object in the objects array.</summary>
            public Int32 Index;

            /// <summary>Offset to 23 bytes of unknown data from start of RDB record. Not required unless you are extending the block reader.</summary>
            public UInt32 UnknownOffset;
        }

        /// <summary>
        /// Array of objects to load into the scene.
        /// </summary>
        public struct RdbObjectRoot
        {
            /// <summary>Offset to root of object list from start of RDB record. Not required unless you are extending the block reader.</summary>
            internal Int32 RootOffset;

            /// <summary>List of objects.</summary>
            public RdbObject[] RdbObjects;
        }

        /// <summary>
        /// A single RDB object has this structure.
        /// </summary>
        [fsObject(Processor = typeof(RdbObjectProcessor))]
        public struct RdbObject
        {
            /// <summary>Offset of this object from start of RDB record. Not required unless you are extending the block reader.</summary>
            public Int32 Position;

            /// <summary>Offset to next object from start of RDB record. Not required unless you are extending the block reader.</summary>
            internal Int32 Next;

            /// <summary>Offset to previous object from start of RDB record. Not required unless you are extending the block reader.</summary>
            internal Int32 Previous;

            /// <summary>Index of this object in the objects array.</summary>
            public int Index;

            /// <summary>X position in 3D space.</summary>
            public Int32 XPos;

            /// <summary>Y position in 3D space.</summary>
            public Int32 YPos;

            /// <summary>Z position in 3D space.</summary>
            public Int32 ZPos;

            /// <summary>Type of resource.</summary>
            public RdbResourceTypes Type;

            /// <summary>Offset to resource data from start of RDB record. Not required unless you are extending the block reader.</summary>
            internal UInt32 ResourceOffset;

            /// <summary>Resource data. Check type for the specific resource to use.</summary>
            public RdbResources Resources;
        }

        #endregion

        #region RDB Resources

        /// <summary>
        /// Aggregated resources. Only one resource will be valid.
        /// </summary>
        public struct RdbResources
        {
            /// <summary>3D object resource.</summary>
            public RdbModelResource ModelResource;

            /// <summary>Light resource.</summary>
            public RdbLightResource LightResource;

            /// <summary>Flat (billboard) resource.</summary>
            public RdbFlatResource FlatResource;
        }

        /// <summary>
        /// A model resource has this structure.
        /// </summary>
        public struct RdbModelResource
        {
            /// <summary>X rotation.</summary>
            public Int32 XRotation;

            /// <summary>Y rotation.</summary>
            public Int32 YRotation;

            /// <summary>Z rotation.</summary>
            public Int32 ZRotation;

            /// <summary>Index into ModelReferenceList array.</summary>
            public UInt16 ModelIndex;

            /// <summary>Trigger flag and starting lock for doors.</summary>
            public UInt32 TriggerFlag_StartingLock;

            /// <summary>ID of sound to play when action is executed. Also used for spell and text index.</summary>
            public Byte SoundIndex;

            /// <summary>Offset to action resource from start of RDB record. Not required unless you are extending the block reader.</summary>
            internal Int32 ActionOffset;

            /// <summary>Action resource.</summary>
            public RdbActionResource ActionResource;
        }

        /// <summary>
        /// The light resource structure is currently unknown.
        /// </summary>
        public struct RdbLightResource
        {
            /// <summary>Unknown.</summary>
            public UInt32 Unknown1;

            /// <summary>Unknown.</summary>
            public UInt32 Unknown2;

            /// <summary>Seems to be light radius, not sure about attenuation model.</summary>
            public UInt16 Radius;
        }

        /// <summary>
        /// A flat (billboard) resource has this structure.
        /// </summary>
        public struct RdbFlatResource
        {
            /// <summary>Position in stream to find this data.</summary>
            public long Position;

            /// <summary>Index of texture compressed to a bitfield.</summary>
            internal UInt16 TextureBitfield;

            /// <summary>Texture archive from bitfield. Used to determine which texture file to load (e.g. TEXTURE.210).</summary>
            public int TextureArchive;

            /// <summary>Texture record from bitfield. Used to determine which texture record to load from archive.</summary>
            public int TextureRecord;

            /// <summary>Flags for action, NPC, etc.</summary>
            public UInt16 Flags;

            /// <summary>Damage, distance to move etc.</summary>
            public byte Magnitude;

            /// <summary>Sound index, also used for spell and text index.</summary>
            public byte SoundIndex;

            /// <summary>
            /// FactionID/MobileID bitfield. (<c>ID &amp; 0xFF</c> for mobile ID).
            /// Range 0-42 is index to monster in MONSTER.BSA.
            /// Range 128-146 is index to humanoid mobile type.
            /// </summary>
            public UInt16 FactionOrMobileId;

            /// <summary>Next object in action chain.</summary>
            public Int32 NextObjectOffset;

            /// <summary>Action flag.</summary>
            public Byte Action;
        }

        /// <summary>
        /// Action resource.
        /// </summary>
        public struct RdbActionResource
        {
            /// <summary>Position in stream to find this data.</summary>
            public long Position;

            /// <summary> 
            ///  About which the object should rotate or translate.
            ///  used for distance in action types 2-7, as well as damage calculations
            /// </summary>
            public byte Axis;

            /// <summary>Length of time the object takes to reach its final state.</summary>
            public UInt16 Duration;

            /// <summary>The amount to translate/rotate around the specified axis.</summary>
            public UInt16 Magnitude;

            /// <summary>
            /// Offset from start of RDB record to an object that should be activated
            ///  directly after this object. This allows actions to be chained together.
            /// </summary>
            public Int32 NextObjectOffset;

            /// <summary>Offset from start of RDB record to an object that should be activated before this object</summary>
            public Int32 PreviousObjectOffset;

            /// <summary>
            /// Index of model in RdbObject array that should be activated
            ///  directly after this object. This allows actions to be chained
            ///  forwards through multiple objects.
            /// </summary>
            public Int32 NextObjectIndex;       //unused?

            /// <summary>Actions to perform.</summary>
            public int Flags;
        }

        #endregion

        #region RDI Structures

        /// <summary>
        /// Monolithic RDI bytes.
        ///  Format of Data is currently unknown.
        /// </summary>
        public struct RdiBlockDesc
        {
            /// <summary>
            /// 512 bytes of unknown data.
            /// </summary>
            public byte[] Data;
        }

        #endregion

        #region FullSerializer Custom Processors (used when serializing world data structures)

        public class RmbGroundDataConverter : fsDirectConverter<RmbFldGroundData>
        {
            protected override fsResult DoSerialize(RmbFldGroundData rmbFldGroundData, Dictionary<string, fsData> serialized)
            {
                if (rmbFldGroundData.Header != null)
                {
                    SerializeMember(serialized, null, "Header", rmbFldGroundData.Header);

                    // Flatten and serialize the ground data 2D arrays
                    List<RmbGroundTiles> groundTilesFlattened = new List<RmbGroundTiles>(RMBTilesPerBlock * RMBTilesPerBlock);
                    for (int tileY = 0; tileY < RMBTilesPerBlock; tileY++)
                    {
                        for (int tileX = 0; tileX < RMBTilesPerBlock; tileX++)
                        {
                            groundTilesFlattened.Add(rmbFldGroundData.GroundTiles[tileX, tileY]);
                        }
                    }
                    List<RmbGroundScenery> groundSceneryFlattened = new List<RmbGroundScenery>(RMBTilesPerBlock * RMBTilesPerBlock);
                    for (int tileY = 0; tileY < RMBTilesPerBlock; tileY++)
                    {
                        for (int tileX = 0; tileX < RMBTilesPerBlock; tileX++)
                        {
                            groundSceneryFlattened.Add(rmbFldGroundData.GroundScenery[tileX, tileY]);
                        }
                    }
                    SerializeMember(serialized, null, "GroundTiles", groundTilesFlattened.ToArray());
                    SerializeMember(serialized, null, "GroundScenery", groundSceneryFlattened.ToArray());
                }
                return fsResult.Success;
            }

            protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref RmbFldGroundData rmbFldGroundData)
            {
                var result = fsResult.Success;
                if (data.Count == 3)
                {
                    if ((result += DeserializeMember(data, null, "Header", out rmbFldGroundData.Header)).Failed) return result;

                    fsData groundTilesData;
                    if ((result += CheckKey(data, "GroundTiles", out groundTilesData)).Failed) return result;
                    fsData groundSceneryData;
                    if ((result += CheckKey(data, "GroundScenery", out groundSceneryData)).Failed) return result;

                    // Un-flatten and deserialize the ground data 2D arrays
                    rmbFldGroundData.GroundTiles = new RmbGroundTiles[RMBTilesPerBlock, RMBTilesPerBlock];
                    List<fsData> groundTilesFlattened = groundTilesData.AsList;
                    int i = 0;
                    for (int tileY = 0; tileY < RMBTilesPerBlock; tileY++)
                    {
                        for (int tileX = 0; tileX < RMBTilesPerBlock; tileX++)
                        {
                            Dictionary<string, fsData> groundTile = groundTilesFlattened[i++].AsDictionary;
                            RmbGroundTiles rmbGroundTile = new RmbGroundTiles();
                            DeserializeMember(groundTile, null, "TextureRecord", out rmbGroundTile.TextureRecord);
                            DeserializeMember(groundTile, null, "TileBitfield", out rmbGroundTile.TileBitfield);
                            DeserializeMember(groundTile, null, "IsRotated", out rmbGroundTile.IsRotated);
                            DeserializeMember(groundTile, null, "IsFlipped", out rmbGroundTile.IsFlipped);
                            rmbFldGroundData.GroundTiles[tileX, tileY] = rmbGroundTile;
                        }
                    }
                    rmbFldGroundData.GroundScenery = new RmbGroundScenery[RMBTilesPerBlock, RMBTilesPerBlock];
                    List<fsData> groundSceneryFlattened = groundSceneryData.AsList;
                    i = 0;
                    for (int tileY = 0; tileY < RMBTilesPerBlock; tileY++)
                    {
                        for (int tileX = 0; tileX < RMBTilesPerBlock; tileX++)
                        {
                            Dictionary<string, fsData> groundScenery = groundSceneryFlattened[i++].AsDictionary;
                            RmbGroundScenery rmbGroundScenery = new RmbGroundScenery();
                            DeserializeMember(groundScenery, null, "TextureRecord", out rmbGroundScenery.TextureRecord);
                            rmbFldGroundData.GroundScenery[tileX, tileY] = rmbGroundScenery;
                        }
                    }
                }
                return result;
            }
        }

        public class RdbBlockDescProcessor : fsObjectProcessor
        {
            // Invoked after serialization has finished. Update any state inside of instance, modify the output data, etc.
            public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
            {
                // Truncate ModelReferenceList at the first null entry in array[750].
                fsData modelRefDict = data.AsDictionary["ModelReferenceList"];
                if (!modelRefDict.IsNull)
                {
                    List<fsData> modelRefList = modelRefDict.AsList;
                    for (int i = 0; i < modelRefList.Count; i++)
                    {
                        if (modelRefList[i].AsDictionary["Description"].AsString == "\uFFFD\uFFFD\uFFFD")
                        {
                            modelRefList.RemoveRange(i, modelRefList.Count - i);
                            break;
                        }
                    }
                }
            }
        }

        public class RdbObjectProcessor : fsObjectProcessor
        {
            // Invoked after serialization has finished. Update any state inside of instance, modify the output data, etc.
            public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
            {
                // Only write relevant type resource data for Rdb Objects.
                Dictionary<string, fsData> rdbObject = data.AsDictionary;
                DFBlock.RdbResourceTypes type = (DFBlock.RdbResourceTypes)Enum.Parse(typeof(DFBlock.RdbResourceTypes), rdbObject["Type"].AsString);
                Dictionary<string, fsData> resources = rdbObject["Resources"].AsDictionary;

                if (type == DFBlock.RdbResourceTypes.Flat || type == DFBlock.RdbResourceTypes.Light)
                    resources.Remove("ModelResource");
                if (type == DFBlock.RdbResourceTypes.Flat || type == DFBlock.RdbResourceTypes.Model)
                    resources.Remove("LightResource");
                if (type == DFBlock.RdbResourceTypes.Model || type == DFBlock.RdbResourceTypes.Light)
                    resources.Remove("FlatResource");
            }
        }

        public class RmbBlock3dObjectRecordProcessor : fsObjectProcessor
        {
            // Invoked after serialization has finished. Update any state inside of instance, modify the output data, etc.
            public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
            {
                // Remove any unused (zero) scale values from serialized form.
                Dictionary<string, fsData> rmb3dObj = data.AsDictionary;
                if (rmb3dObj["XScale"].AsDouble == 0)
                    rmb3dObj.Remove("XScale");
                if (rmb3dObj["YScale"].AsDouble == 0)
                    rmb3dObj.Remove("YScale");
                if (rmb3dObj["ZScale"].AsDouble == 0)
                    rmb3dObj.Remove("ZScale");
            }
        }

        #endregion
    }
}
