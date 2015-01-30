// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores information about a block record. This is fundamentally equivalent to native block data,
    ///  but is type-safe and structured for use.
    /// </summary>
    public struct DFBlock
    {
        #region Structure Variables

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
            internal Byte[] Section2UnknownData;

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
            internal Byte TileBitfield;

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
            internal RmbBlockSection3Record[] BlockSection3Records;

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

            /// <summary>Unknown.</summary>
            internal UInt32 NullValue2;

            /// <summary>Y rotation.</summary>
            public Int16 YRotation;

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

            /// <summary>Unknown.</summary>
            internal Int16 Unknown1;

            /// <summary>Unknown.</summary>
            internal Byte Unknown2;
        }

        /// <summary>
        /// Records of an unknown type.
        /// Only appears in interiors and forms a grid-like pattern over floor.
        /// Most likely path-finding waypoints.
        /// </summary>
        internal struct RmbBlockSection3Record
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

            /// <summary>NPC type.</summary>
            public Int16 NpcType;

            /// <summary>Unknown.</summary>
            internal Byte Unknown1;
        }

        /// <summary>
        /// Doors the player can open and close.
        /// </summary>
        public struct RmbBlockDoorRecord
        {
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

            /// <summary>Unknown.</summary>
            internal Int16 Unknown3;

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
        /// </summary>
        [Flags]
        public enum RdbActionFlags
        {
            /// <summary>Action unspecified.</summary>
            None = 0x00,

            /// <summary>Translation.</summary>
            Translation = 0x01,

            /// <summary>Unknown.</summary>
            Unknown1 = 0x02,

            /// <summary>Unknown.</summary>
            unknown2 = 0x04,

            /// <summary>Rotation.</summary>
            Rotation = 0x08,

            /// <summary>Unknown.</summary>
            Unknown3 = 0x10,

            /// <summary>Unknown.</summary>
            Unknown4 = 0x20,

            /// <summary>Unknown.</summary>
            Unknown5 = 0x40,

            /// <summary>Unknown.</summary>
            Unknown6 = 0x80,
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
            public RdbUnknownObject[] UnknownObjectList;
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
            public Int32 This;

            /// <summary>Offset to next object from start of RDB record. Not required unless you are extending the block reader.</summary>
            public Int32 Next;

            /// <summary>Index of this object in the objects array.</summary>
            public Int32 Index;

            /// <summary>Offset to unknown data from start of RDB record. Not required unless you are extending the block reader.</summary>
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
        public struct RdbObject
        {
            /// <summary>Offset of this object from start of RDB record. Not required unless you are extending the block reader.</summary>
            internal Int32 This;

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

            /// <summary>Unknown.</summary>
            internal UInt32 Unknown1;

            /// <summary>ID of sound to play when action is executed.</summary>
            public Byte SoundId;

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
            /// <summary>Index of texture compressed to a bitfield.</summary>
            internal UInt16 TextureBitfield;

            /// <summary>Texture archive from bitfield. Used to determine which texture file to load (e.g. TEXTURE.210).</summary>
            public int TextureArchive;

            /// <summary>Texture record from bitfield. Used to determine which texture record to load from archive.</summary>
            public int TextureRecord;

            /// <summary>NPC gender (if any).</summary>
            public RdbFlatGenders Gender;

            /// <summary>
            /// FactionID/MobileID bitfield. (ID & 0xFF for mobile ID).
            /// Range 0-42 is index to monster in MONSTER.BSA.
            /// Range 128-146 is index to humanoid mobile type.
            /// </summary>
            public UInt16 FactionMobileId;

            /// <summary>Further data about this flat resource.</summary>
            public RdbFlatData FlatData;
        }

        /// <summary>
        /// Mostly unknown data about an RDB flat resource.
        /// </summary>
        public struct RdbFlatData
        {
            public Byte Unknown1;
            public Byte Unknown2;
            public Byte Unknown3;
            public Byte Unknown4;

            /// <summary>0 = Hostile, 99 = Passive (e.g. guards in castles).</summary>
            public Byte Reaction;
        }

        /// <summary>
        /// Action resource.
        /// </summary>
        public struct RdbActionResource
        {
            /// <summary>Position in stream to find this data.</summary>
            public long Position;

            /// <summary>About which the object should rotate or translate.</summary>
            public RdbActionAxes Axis;

            /// <summary>Length of time the object takes to reach its final state.</summary>
            public UInt16 Duration;

            /// <summary>The amount to translate/rotate around the specified axis.</summary>
            public UInt16 Magnitude;

            /// <summary>
            /// Offset from start of RDB record to an object that should be activated
            ///  directly after this object. This allows actions to be chained together.
            /// </summary>
            internal Int32 NextObjectOffset;

            /// <summary>
            /// Index of previous model in RdbObject array that linked to this model
            ///  in an action chain. This allows action records to be chained
            ///  backwards to the root action.
            /// </summary>
            public int PreviousObjectIndex;

            /// <summary>
            /// Index of model in RdbObject array that should be activated
            ///  directly after this object. This allows actions to be chained
            ///  forwards through multiple objects.
            /// </summary>
            public Int32 NextObjectIndex;

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
    }
}
