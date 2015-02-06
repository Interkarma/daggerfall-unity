// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to BLOCKS.BSA to enumerate and extract city and dungeon blocks.
    /// </summary>
    public class BlocksFile
    {
        #region Class Variables

        /// <summary>
        /// Auto-discard behaviour enabled or disabled.
        /// </summary>
        private bool autoDiscardValue = true;

        /// <summary>
        /// The last record opened. Used by auto-discard logic.
        /// </summary>
        private int lastBlock = -1;

        /// <summary>
        /// The BsaFile representing BLOCKS.BSA.
        /// </summary>
        private BsaFile bsaFile = new BsaFile();

        /// <summary>
        /// Array of decomposed block records.
        /// </summary>
        internal BlockRecord[] blocks;

        /// <summary>
        /// Name to index lookup dictionary.
        /// </summary>
        private Dictionary<String, int> blockNameLookup = new Dictionary<String, int>();

        #endregion

        #region Class Structures

        /// <summary>
        /// Represents a single block record.
        /// </summary>
        internal struct BlockRecord
        {
            public string Name;
            public FileProxy MemoryFile;
            public DFBlock DFBlock;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets managed BSA file.
        /// </summary>
        internal BsaFile BsaFile
        {
            get { return bsaFile; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlocksFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to BLOCKS.BSA.</param>
        /// <param name="usage">Determines if the BSA file will read from disk or memory.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public BlocksFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// If true then decomposed block records will be destroyed every time a different block is fetched.
        ///  If false then decomposed block records will be maintained until DiscardRecord() or DiscardAllRecords() is called.
        ///  Turning off auto-discard will speed up block retrieval times at the expense of RAM. For best results, disable
        ///  auto-discard and impose your own caching scheme using LoadBlock() and DiscardBlock() based on your application
        ///  needs.
        /// </summary>
        public bool AutoDiscard
        {
            get { return autoDiscardValue; }
            set { autoDiscardValue = value; }
        }

        /// <summary>
        /// Number of BSA records in BLOCKS.BSA.
        /// </summary>
        public int Count
        {
            get { return bsaFile.Count; }
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default BLOCKS.BSA filename.
        /// </summary>
        static public string Filename
        {
            get { return "BLOCKS.BSA"; }
        }

        /// <summary>
        /// Gets rotation divisor used when rotating
        ///  block records and models into place.
        /// </summary>
        static public float RotationDivisor
        {
            get { return 5.68888888888889f;}
        }

        /// <summary>
        /// Gets dimension of a single RMB block.
        /// </summary>
        static public float RMBDimension
        {
            get { return 4096f; }
        }

        /// <summary>
        /// Gets dimension of a single RMB ground tile.
        /// </summary>
        static public float TileDimension
        {
            get { return 256f; }
        }

        /// <summary>
        /// Gets dimension of a single RDB block.
        /// </summary>
        static public float RDBDimension
        {
            get { return 2048f; }
        }

        /// <summary>
        /// Gets scale divisor for billboards.
        /// </summary>
        static public float ScaleDivisor
        {
            get { return 256f; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load BLOCKS.BSA file.
        /// </summary>
        /// <param name="filePath">Absolute path to BLOCKS.BSA file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            filePath = filePath.ToUpper();
            if (!filePath.EndsWith("BLOCKS.BSA"))
                return false;

            // Load file
            if (!bsaFile.Load(filePath, usage, readOnly))
                return false;

            // Create records array
            blocks = new BlockRecord[bsaFile.Count];

            return true;
        }

        /// <summary>
        /// Gets name of specified block. Does not change the currently loaded block.
        /// </summary>
        /// <param name="block">Index of block.</param>
        /// <returns>Name of the block.</returns>
        public string GetBlockName(int block)
        {
            return bsaFile.GetRecordName(block);
        }

        /// <summary>
        /// Gets the type of specified block. Does not change the currently loaded block.
        /// </summary>
        /// <param name="block">Index of block.</param>
        /// <returns>DFBlock.blockTypes object.</returns>
        public DFBlock.BlockTypes GetBlockType(int block)
        {
            // Determine record type from extension of name
            string name = GetBlockName(block);
            if (name.EndsWith(".RMB"))
                return DFBlock.BlockTypes.Rmb;
            else if (name.EndsWith(".RDB"))
                return DFBlock.BlockTypes.Rdb;
            else if (name.EndsWith(".RDI"))
                return DFBlock.BlockTypes.Rdi;
            else
                return DFBlock.BlockTypes.Unknown;
        }

        /// <summary>
        /// Get RDB block type (quest, normal, wet, etc.)
        ///  Does not return RdbTypes.Start as this can only be derived from
        ///  map data.
        /// </summary>
        /// <param name="blockName">Name of RDB block.</param>
        /// <returns>DFBlock.RdbTypes object.</returns>
        public DFBlock.RdbTypes GetRdbType(string blockName)
        {
            // Determine block type
            if (blockName.StartsWith("B"))
                return DFBlock.RdbTypes.Border;
            else if (blockName.StartsWith("W"))
                return DFBlock.RdbTypes.Wet;
            else if (blockName.StartsWith("S"))
                return DFBlock.RdbTypes.Quest;
            else if (blockName.StartsWith("M"))
                return DFBlock.RdbTypes.Mausoleum;
            else if (blockName.StartsWith("N"))
                return DFBlock.RdbTypes.Normal;
            else
                return DFBlock.RdbTypes.Unknown;
        }

        /// <summary>
        /// Gets index of block with specified name. Does not change the currently loaded block.
        ///  Uses a dictionary to map name to index so this method will be faster on subsequent calls.
        /// </summary>
        /// <param name="name">Name of block.</param>
        /// <returns>Index of found block, or -1 if not found.</returns>
        public int GetBlockIndex(string name)
        {
            // Return known value if already indexed
            if (blockNameLookup.ContainsKey(name))
                return blockNameLookup[name];

            // Otherwise find and store index by searching for name
            for (int i = 0; i < Count; i++)
            {
                if (GetBlockName(i) == name)
                {
                    // Found the block, add to dictionary and return
                    blockNameLookup.Add(name, i);
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Load a block into memory and decompose it for use.
        /// </summary>
        /// <param name="block">Index of block to load.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool LoadBlock(int block)
        {
            // Validate
            if (block < 0 || block >= bsaFile.Count)
                return false;

            // Exit if file has already been opened
            if (blocks[block].MemoryFile != null )
                return true;

            // Auto discard previous record
            if (autoDiscardValue && lastBlock != -1)
                DiscardBlock(lastBlock);

            // Load record data
            blocks[block].MemoryFile = bsaFile.GetRecordProxy(block);
            if (blocks[block].MemoryFile == null)
                return false;

            // Set record name
            blocks[block].Name = bsaFile.GetRecordName(block);
            blocks[block].DFBlock.Name = bsaFile.GetRecordName(block);

            // Set record type
            blocks[block].DFBlock.Type = GetBlockType(block);

            // Set self index
            blocks[block].DFBlock.Index = block;

            // Read record
            if (!Read(block))
            {
                DiscardBlock(block);
                return false;
            }

            // Store in lookup dictionary
            if (!blockNameLookup.ContainsKey(blocks[block].Name))
                blockNameLookup.Add(blocks[block].Name, block);

            // Set previous record
            lastBlock = block;

            return true;
        }

        /// <summary>
        /// Discard a block from memory.
        /// </summary>
        /// <param name="block">Index of block to discard.</param>
        public void DiscardBlock(int block)
        {
            // Validate
            if (block >= bsaFile.Count)
                return;

            // Discard memory file and other data
            blocks[block].Name = string.Empty;
            blocks[block].DFBlock.Type = DFBlock.BlockTypes.Unknown;
            blocks[block].MemoryFile = null;
            blocks[block].DFBlock.RmbBlock.Misc3dObjectRecords = null;
            blocks[block].DFBlock.RmbBlock.MiscFlatObjectRecords = null;
            blocks[block].DFBlock.RmbBlock.SubRecords = null;
            blocks[block].DFBlock.RdbBlock.ModelDataList = null;
            blocks[block].DFBlock.RdbBlock.ModelReferenceList = null;
            blocks[block].DFBlock.RdbBlock.ObjectRootList = null;
        }

        /// <summary>
        /// Discard all block records.
        /// </summary>
        public void DiscardAllBlocks()
        {
            for (int block = 0; block < bsaFile.Count; block++)
            {
                DiscardBlock(block);
            }
        }

        /// <summary>
        /// Gets a DFBlock representation of a record.
        /// </summary>
        /// <param name="block">Index of block to load.</param>
        /// <returns>DFBlock object.</returns>
        public DFBlock GetBlock(int block)
        {
            // Load the record
            if (!LoadBlock(block))
                return new DFBlock();

            return blocks[block].DFBlock;
        }

        /// <summary>
        /// Gets a DFBlock by name.
        /// </summary>
        /// <param name="name">Name of block.</param>
        /// <returns>DFBlock object.</returns>
        public DFBlock GetBlock(string name)
        {
            // Look for block index
            int index = GetBlockIndex(name);
            if (index == -1)
            {
                // Not found, search for alternate name
                string alternateName = SearchAlternateRMBName(ref name);
                if (!string.IsNullOrEmpty(alternateName))
                    index = GetBlockIndex(alternateName);
            }

            return GetBlock(index);
        }

        /// <summary>
        /// Gets block AutoMap by name.
        /// </summary>
        /// <param name="name">Name of block.</param>
        /// <param name="removeGroundFlats">Filters ground flat "speckles" from the AutoMap.</param>
        /// <returns>DFBitmap object.</returns>
        public DFBitmap GetBlockAutoMap(string name, bool removeGroundFlats)
        {
            // Test block is valid
            DFBlock dfBlock = GetBlock(name);
            if (string.IsNullOrEmpty(dfBlock.Name))
                return new DFBitmap();

            // Create DFBitmap and copy data
            DFBitmap dfBitmap = new DFBitmap();
            dfBitmap.Data = dfBlock.RmbBlock.FldHeader.AutoMapData;
            dfBitmap.Width = 64;
            dfBitmap.Height = 64;
            dfBitmap.Stride = 64;
            dfBitmap.Format = DFBitmap.Formats.Indexed;

            // Filter ground flats if specified
            if (removeGroundFlats)
            {
                for (int i = 0; i < dfBitmap.Data.Length; i++)
                {
                    if (dfBitmap.Data[i] == 0xfb)
                        dfBitmap.Data[i] = 0x00;
                }
            }

            return dfBitmap;
        }

        /// <summary>
        /// Checks block name and substitutes fixed name if possible.
        /// </summary>
        /// <param name="name">Block name.</param>
        /// <returns>Block name or String.Empty if no fix possible.</returns>
        public string CheckName(string name)
        {
            // Check name resolves to valid index
            int index = GetBlockIndex(name);
            if (index == -1)
            {
                // Name not found, search for alternate name
                string alternateName = SearchAlternateRMBName(ref name);
                return alternateName;
            }

            return name;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Not all RMB block names can be resolved.
        ///  This method attempts to find a suitable match for these cases
        ///  by matching prefix and suffix of failed block name to a 
        ///  valid block name.
        /// </summary>
        /// <param name="name">Name of invalid block.</param>
        /// <returns>Valid block name with same prefix and suffix, or string.Empty if no match found.</returns>
        private string SearchAlternateRMBName(ref string name)
        {
            string found = string.Empty;
            string prefix = name.Substring(0, 4);
            string suffix = name.Substring(name.Length - 6, 6);
            for (int block = 0; block < Count; block++)
            {
                string test = GetBlockName(block);
                if (test.StartsWith(prefix) && test.EndsWith(suffix))
                    found = test;
            }

            return found;
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read a block record.
        /// </summary>
        /// <param name="block">The block index to read.</param>
        /// <returns>True if successful, otherwise false.</returns>
        private bool Read(int block)
        {
            try
            {
                // Read memory file
                BinaryReader reader = blocks[block].MemoryFile.GetReader();
                ReadBlock(reader, block);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Choose to read an RMB, RDB, or RDI block record. Other block types will be discarded.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="block">Destination block index.</param>
        private void ReadBlock(BinaryReader reader, int block)
        {
            // Step through file based on type
            reader.BaseStream.Position = 0;
            if (blocks[block].DFBlock.Type == DFBlock.BlockTypes.Rmb)
            {
                // Read RMB data
                ReadRmbFldHeader(reader, block);
                ReadRmbBlockData(reader, block);
                ReadRmbMisc3dObjects(reader, block);
                ReadRmbMiscFlatObjectRecords(reader, block);
            }
            else if (blocks[block].DFBlock.Type == DFBlock.BlockTypes.Rdb)
            {
                // Read RDB data
                ReadRdbHeader(reader, block);
                ReadRdbModelReferenceList(reader, block);
                ReadRdbModelDataList(reader, block);
                ReadRdbObjectSectionHeader(reader, block);
                ReadRdbUnknownLinkedList(reader, block);
                ReadRdbObjectSectionRootList(reader, block);
                ReadRdbObjectLists(reader, block);
            }
            else if (blocks[block].DFBlock.Type == DFBlock.BlockTypes.Rdi)
            {
                // Read RDI data
                ReadRdiRecord(reader, block);
            }
            else
            {
                DiscardBlock(block);
                return;
            }
        }

        #endregion

        #region RMB Readers

        /// <summary>
        /// Read the fixed length data (FLD) header of an RMB record
        /// </summary>
        /// <param name="reader">A binary reader to file</param>
        /// <param name="block">Destination block index</param>
        private void ReadRmbFldHeader(BinaryReader reader, int block)
        {
            // Record counts
            blocks[block].DFBlock.RmbBlock.FldHeader.NumBlockDataRecords = reader.ReadByte();
            blocks[block].DFBlock.RmbBlock.FldHeader.NumMisc3dObjectRecords = reader.ReadByte();
            blocks[block].DFBlock.RmbBlock.FldHeader.NumMiscFlatObjectRecords = reader.ReadByte();

            // Block positions
            blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions = new DFBlock.RmbFldBlockPositions[32];
            for (int i = 0; i < 32; i++)
            {
                blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions[i].Unknown1 = reader.ReadUInt32();
                blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions[i].Unknown2 = reader.ReadUInt32();
                blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions[i].XPos = reader.ReadInt32();
                blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions[i].ZPos = reader.ReadInt32();
                blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions[i].YRotation = reader.ReadInt32();
            }

            // Building data list
            blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList = new DFLocation.BuildingData[32];
            for (int i = 0; i < 32; i++)
            {
                blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList[i].NameSeed = reader.ReadUInt16();
                blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList[i].NullValue1 = reader.ReadUInt64();
                blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList[i].NullValue2 = reader.ReadUInt64();
                blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList[i].FactionId = reader.ReadUInt16();
                blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList[i].Sector = reader.ReadInt16();
                blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList[i].LocationId = reader.ReadUInt16();
                blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList[i].BuildingType = (DFLocation.BuildingTypes)reader.ReadByte();
                blocks[block].DFBlock.RmbBlock.FldHeader.BuildingDataList[i].Quality = reader.ReadByte();
            }

            // Section2 unknown data
            blocks[block].DFBlock.RmbBlock.FldHeader.Section2UnknownData = reader.ReadBytes(128);

            // Block data sizes
            blocks[block].DFBlock.RmbBlock.FldHeader.BlockDataSizes = new Int32[32];
            for (int i = 0; i < 32; i++)
            {
                blocks[block].DFBlock.RmbBlock.FldHeader.BlockDataSizes[i] = reader.ReadInt32();
            }

            // Ground data
            blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.Header = reader.ReadBytes(8);
            ReadRmbGroundTilesData(reader, block);
            ReadRmbGroundSceneryData(reader, block);

            // Automap
            blocks[block].DFBlock.RmbBlock.FldHeader.AutoMapData = reader.ReadBytes(64 * 64);

            // Filenames
            blocks[block].DFBlock.RmbBlock.FldHeader.Name = blocks[block].MemoryFile.ReadCString(reader, 13);
            blocks[block].DFBlock.RmbBlock.FldHeader.OtherNames = new string[32];
            for (int i = 0; i < 32; i++)
            {
                blocks[block].DFBlock.RmbBlock.FldHeader.OtherNames[i] = blocks[block].MemoryFile.ReadCString(reader, 13);
            }
        }

        /// <summary>
        /// Read ground tile data for this block.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="block">Destination block index.</param>
        private void ReadRmbGroundTilesData(BinaryReader reader, int block)
        {
            // Create new array
            blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundTiles = new DFBlock.RmbGroundTiles[16, 16];

            // Read in data
            Byte bitfield;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    // Read source bitfield
                    bitfield = reader.ReadByte();

                    // Store data
                    blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundTiles[x, y].TileBitfield = bitfield;
                    blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundTiles[x, y].TextureRecord = bitfield & 0x3f;
                    blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundTiles[x, y].IsRotated = ((bitfield & 0x40) == 0x40) ? true : false;
                    blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundTiles[x, y].IsFlipped = ((bitfield & 0x80) == 0x80) ? true : false;
                }
            }
        }


        /// <summary>
        /// Read ground scenery data for this block.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="block">Destination block index.</param>
        private void ReadRmbGroundSceneryData(BinaryReader reader, int block)
        {
            // Create new array
            blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundScenery = new DFBlock.RmbGroundScenery[16, 16];

            // Read in data
            Byte bitfield;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    // Read source bitfield
                    bitfield = reader.ReadByte();

                    // Store data
                    blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundScenery[x, y].TileBitfield = bitfield;
                    if (bitfield < 255)
                    {
                        blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundScenery[x, y].Unknown1 = bitfield & 0x03;
                        blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundScenery[x, y].TextureRecord = bitfield / 0x04 - 1;
                    }
                    else
                    {
                        blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundScenery[x, y].Unknown1 = 0;
                        blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundScenery[x, y].TextureRecord = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Read RMB block data, i.e. the outside and inside repeating sections.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="block">Destination block index.</param>
        private void ReadRmbBlockData(BinaryReader reader, int block)
        {
            // Read block data
            int recordCount = blocks[block].DFBlock.RmbBlock.FldHeader.NumBlockDataRecords;
            blocks[block].DFBlock.RmbBlock.SubRecords = new DFBlock.RmbSubRecord[recordCount];
            long position = reader.BaseStream.Position;
            for (int i = 0; i < recordCount; i++)
            {
                // Copy XZ position and Y rotation into subrecord data for convenience
                blocks[block].DFBlock.RmbBlock.SubRecords[i].XPos = blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions[i].XPos;
                blocks[block].DFBlock.RmbBlock.SubRecords[i].ZPos = blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions[i].ZPos;
                blocks[block].DFBlock.RmbBlock.SubRecords[i].YRotation = blocks[block].DFBlock.RmbBlock.FldHeader.BlockPositions[i].YRotation;

                // Read outside and inside block data
                ReadRmbBlockSubRecord(reader, ref blocks[block].DFBlock.RmbBlock.SubRecords[i].Exterior);
                ReadRmbBlockSubRecord(reader, ref blocks[block].DFBlock.RmbBlock.SubRecords[i].Interior);

                // Offset to next position (this ignores padding byte and ensures block reading is correctly stepped)
                position += blocks[block].DFBlock.RmbBlock.FldHeader.BlockDataSizes[i];
                reader.BaseStream.Position = position;
            }
        }

        /// <summary>
        /// Read miscellaneous block 3D objects.
        /// </summary>
        /// <param name="reader">A binary reader to file</param>
        /// <param name="block">Destination block index</param>
        private void ReadRmbMisc3dObjects(BinaryReader reader, int block)
        {
            // Read misc block 3d objects
            blocks[block].DFBlock.RmbBlock.Misc3dObjectRecords = new DFBlock.RmbBlock3dObjectRecord[blocks[block].DFBlock.RmbBlock.FldHeader.NumMisc3dObjectRecords];
            ReadRmbModelRecords(reader, ref blocks[block].DFBlock.RmbBlock.Misc3dObjectRecords);
        }

        /// <summary>
        /// Read miscellaneous block flat objects.
        /// </summary>
        /// <param name="reader">A binary reader to file</param>
        /// <param name="block">Destination block index</param>
        private void ReadRmbMiscFlatObjectRecords(BinaryReader reader, int block)
        {
            // Read misc flat object records
            blocks[block].DFBlock.RmbBlock.MiscFlatObjectRecords = new DFBlock.RmbBlockFlatObjectRecord[blocks[block].DFBlock.RmbBlock.FldHeader.NumMiscFlatObjectRecords];
            ReadRmbFlatObjectRecords(reader, ref blocks[block].DFBlock.RmbBlock.MiscFlatObjectRecords);
        }

        /// <summary>
        /// Read block subrecords, i.e. the resources embedded in block data.
        /// </summary>
        /// <param name="reader">A binary reader to file</param>
        /// <param name="blockData">Destination record index</param>
        private void ReadRmbBlockSubRecord(BinaryReader reader, ref DFBlock.RmbBlockData blockData)
        {
            // Header
            blockData.Header.Position = reader.BaseStream.Position;
            blockData.Header.Num3dObjectRecords = reader.ReadByte();
            blockData.Header.NumFlatObjectRecords = reader.ReadByte();
            blockData.Header.NumSection3Records = reader.ReadByte();
            blockData.Header.NumPeopleRecords = reader.ReadByte();
            blockData.Header.NumDoorRecords = reader.ReadByte();
            blockData.Header.Unknown1 = reader.ReadInt16();
            blockData.Header.Unknown2 = reader.ReadInt16();
            blockData.Header.Unknown3 = reader.ReadInt16();
            blockData.Header.Unknown4 = reader.ReadInt16();
            blockData.Header.Unknown5 = reader.ReadInt16();
            blockData.Header.Unknown6 = reader.ReadInt16();

            // 3D object records
            blockData.Block3dObjectRecords = new DFBlock.RmbBlock3dObjectRecord[blockData.Header.Num3dObjectRecords];
            ReadRmbModelRecords(reader, ref blockData.Block3dObjectRecords);

            // Flat object record
            blockData.BlockFlatObjectRecords = new DFBlock.RmbBlockFlatObjectRecord[blockData.Header.NumFlatObjectRecords];
            ReadRmbFlatObjectRecords(reader, ref blockData.BlockFlatObjectRecords);

            // Section3 records
            int numSection3Records = blockData.Header.NumSection3Records;
            blockData.BlockSection3Records = new DFBlock.RmbBlockSection3Record[numSection3Records];
            for (int i = 0; i < numSection3Records; i++)
            {
                blockData.BlockSection3Records[i].XPos = reader.ReadInt32();
                blockData.BlockSection3Records[i].YPos = reader.ReadInt32();
                blockData.BlockSection3Records[i].ZPos = reader.ReadInt32();
                blockData.BlockSection3Records[i].Unknown1 = reader.ReadByte();
                blockData.BlockSection3Records[i].Unknown2 = reader.ReadByte();
                blockData.BlockSection3Records[i].Unknown3 = reader.ReadInt16();
            }

            // People records
            int numPeopleRecords = blockData.Header.NumPeopleRecords;
            blockData.BlockPeopleRecords = new DFBlock.RmbBlockPeopleRecord[numPeopleRecords];
            for (int i = 0; i < numPeopleRecords; i++)
            {
                blockData.BlockPeopleRecords[i].XPos = reader.ReadInt32();
                blockData.BlockPeopleRecords[i].YPos = reader.ReadInt32();
                blockData.BlockPeopleRecords[i].ZPos = reader.ReadInt32();
                blockData.BlockPeopleRecords[i].TextureBitfield = reader.ReadUInt16();
                blockData.BlockPeopleRecords[i].TextureArchive = blockData.BlockPeopleRecords[i].TextureBitfield >> 7;
                blockData.BlockPeopleRecords[i].TextureRecord = blockData.BlockPeopleRecords[i].TextureBitfield & 0x7f;
                blockData.BlockPeopleRecords[i].NpcType = reader.ReadInt16();
                blockData.BlockPeopleRecords[i].Unknown1 = reader.ReadByte();
            }

            // Door records
            int numDoorRecords = blockData.Header.NumDoorRecords;
            blockData.BlockDoorRecords = new DFBlock.RmbBlockDoorRecord[numDoorRecords];
            for (int i = 0; i < numDoorRecords; i++)
            {
                blockData.BlockDoorRecords[i].XPos = reader.ReadInt32();
                blockData.BlockDoorRecords[i].YPos = reader.ReadInt32();
                blockData.BlockDoorRecords[i].ZPos = reader.ReadInt32();
                blockData.BlockDoorRecords[i].YRotation = reader.ReadInt16();
                blockData.BlockDoorRecords[i].OpenRotation = reader.ReadInt16();
                blockData.BlockDoorRecords[i].Unknown3 = reader.ReadInt16();
                blockData.BlockDoorRecords[i].NullValue1 = reader.ReadByte();
            }
        }

        /// <summary>
        /// Read a 3D object subrecord.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="recordsOut">Destination object.</param>
        private void ReadRmbModelRecords(BinaryReader reader, ref DFBlock.RmbBlock3dObjectRecord[] recordsOut)
        {
            // Read all 3d object records into array
            for (int i = 0; i < recordsOut.Length; i++)
            {
                Int16 objectId1 = reader.ReadInt16();
                Byte objectId2 = reader.ReadByte();
                recordsOut[i].ObjectId1 = objectId1;
                recordsOut[i].ObjectId2 = objectId2;
                recordsOut[i].ModelId = ((objectId1 * 100) + objectId2).ToString();
                recordsOut[i].ModelIdNum = UInt32.Parse(recordsOut[i].ModelId);
                recordsOut[i].ObjectType = reader.ReadByte();
                recordsOut[i].Unknown1 = reader.ReadUInt32();
                recordsOut[i].Unknown2 = reader.ReadUInt32();
                recordsOut[i].Unknown3 = reader.ReadUInt32();
                recordsOut[i].NullValue1 = reader.ReadUInt64();
                recordsOut[i].XPos1 = reader.ReadInt32();
                recordsOut[i].YPos1 = reader.ReadInt32();
                recordsOut[i].ZPos1 = reader.ReadInt32();
                recordsOut[i].XPos = reader.ReadInt32();
                recordsOut[i].YPos = reader.ReadInt32();
                recordsOut[i].ZPos = reader.ReadInt32();
                recordsOut[i].NullValue2 = reader.ReadUInt32();
                recordsOut[i].YRotation = reader.ReadInt16();
                recordsOut[i].Unknown4 = reader.ReadUInt16();
                recordsOut[i].NullValue3 = reader.ReadUInt32();
                recordsOut[i].Unknown5 = reader.ReadUInt32();
                recordsOut[i].NullValue4 = reader.ReadUInt16();
            }
        }

        /// <summary>
        /// Read a flat object subrecord.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="recordsOut">Destination object.</param>
        private void ReadRmbFlatObjectRecords(BinaryReader reader, ref DFBlock.RmbBlockFlatObjectRecord[] recordsOut)
        {
            // Read all flat object records into array
            for (int i = 0; i < recordsOut.Length; i++)
            {
                recordsOut[i].XPos = reader.ReadInt32();
                recordsOut[i].YPos = reader.ReadInt32();
                recordsOut[i].ZPos = reader.ReadInt32();
                recordsOut[i].TextureBitfield = reader.ReadUInt16();
                recordsOut[i].TextureArchive = recordsOut[i].TextureBitfield >> 7;
                recordsOut[i].TextureRecord = recordsOut[i].TextureBitfield & 0x7f;
                recordsOut[i].Unknown1 = reader.ReadInt16();
                recordsOut[i].Unknown2 = reader.ReadByte();
            }
        }

        #endregion

        #region RDB Readers

        private void ReadRdbHeader(BinaryReader reader, int block)
        {
            // Read header
            blocks[block].DFBlock.RdbBlock.Position = reader.BaseStream.Position;
            blocks[block].DFBlock.RdbBlock.Header.Unknown1 = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.Header.Width = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.Header.Height = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.Header.ObjectRootOffset = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.Header.Unknown2 = reader.ReadUInt32();
        }

        private void ReadRdbModelReferenceList(BinaryReader Reader, int Block)
        {
            // Read model reference list
            blocks[Block].DFBlock.RdbBlock.ModelReferenceList = new DFBlock.RdbModelReference[750];
            for (int i = 0; i < 750; i++)
            {
                blocks[Block].DFBlock.RdbBlock.ModelReferenceList[i].ModelId = blocks[Block].MemoryFile.ReadCString(Reader, 5);
                UInt32.TryParse(blocks[Block].DFBlock.RdbBlock.ModelReferenceList[i].ModelId, out blocks[Block].DFBlock.RdbBlock.ModelReferenceList[i].ModelIdNum);
                blocks[Block].DFBlock.RdbBlock.ModelReferenceList[i].Description = blocks[Block].MemoryFile.ReadCString(Reader, 3);
            }
        }

        private void ReadRdbModelDataList(BinaryReader reader, int block)
        {
            // Read unknown model data list
            blocks[block].DFBlock.RdbBlock.ModelDataList = new DFBlock.RdbModelData[750];
            for (int i = 0; i < 750; i++)
            {
                blocks[block].DFBlock.RdbBlock.ModelDataList[i].Unknown1 = reader.ReadUInt32();
            }
        }

        private void ReadRdbObjectSectionHeader(BinaryReader reader, int block)
        {
            // Read object section header
            blocks[block].DFBlock.RdbBlock.ObjectHeader.UnknownOffset = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.ObjectHeader.Unknown1 = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.ObjectHeader.Unknown2 = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.ObjectHeader.Unknown3 = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.ObjectHeader.Length = reader.ReadUInt32();
            blocks[block].DFBlock.RdbBlock.ObjectHeader.Unknown4 = reader.ReadBytes(32);
            blocks[block].DFBlock.RdbBlock.ObjectHeader.Dagr = blocks[block].MemoryFile.ReadCString(reader, 4);
            blocks[block].DFBlock.RdbBlock.ObjectHeader.Unknown5 = reader.ReadBytes(456);
        }

        private void ReadRdbUnknownLinkedList(BinaryReader reader, int block)
        {
            // Store current reader position
            long position = reader.BaseStream.Position;
            
            // Go to first unknown object
            reader.BaseStream.Position = blocks[block].DFBlock.RdbBlock.ObjectHeader.UnknownOffset;

            // Iterate over list
            int count = 0;
            while (true)
            {
                // Read object data
                DFBlock.RdbUnknownObject obj = new DFBlock.RdbUnknownObject();
                obj.This = (Int32)reader.BaseStream.Position;
                obj.Next = reader.ReadInt32();
                obj.Index = reader.ReadInt16();
                obj.UnknownOffset = (UInt32)reader.ReadInt32();

                // Create array for first time
                if (blocks[block].DFBlock.RdbBlock.UnknownObjectList == null)
                {
                    // Index counts backwards, so first item index is also length of list
                    blocks[block].DFBlock.RdbBlock.UnknownObjectList = new DFBlock.RdbUnknownObject[obj.Index];
                }

                // Exit if finished
                if (obj.Next < 0)
                    break;

                // Store object
                blocks[block].DFBlock.RdbBlock.UnknownObjectList[count++] = obj;

                // Go to next position
                reader.BaseStream.Position = obj.Next;
            }

            // Revert reader position
            reader.BaseStream.Position = position;
        }

        private void ReadRdbObjectSectionRootList(BinaryReader reader, int block)
        {
            // Handle improper position in stream
            if (reader.BaseStream.Position != blocks[block].DFBlock.RdbBlock.Header.ObjectRootOffset)
                throw(new Exception("Start of ObjectRoot section does not match header offset."));

            // Read object section root list
            UInt32 width = blocks[block].DFBlock.RdbBlock.Header.Width;
            UInt32 height = blocks[block].DFBlock.RdbBlock.Header.Height;
            blocks[block].DFBlock.RdbBlock.ObjectRootList = new DFBlock.RdbObjectRoot[width * height];
            for (int i = 0; i < width * height; i++)
            {
                blocks[block].DFBlock.RdbBlock.ObjectRootList[i].RootOffset = reader.ReadInt32();
            }
        }

        private void ReadRdbObjectLists(BinaryReader reader, int block)
        {
            // Read all objects starting from each root position
            for (int i = 0; i < blocks[block].DFBlock.RdbBlock.ObjectRootList.Length; i++)
            {
                // Skip if no data present
                if (blocks[block].DFBlock.RdbBlock.ObjectRootList[i].RootOffset < 0)
                    continue;

                // Pre-count number of objects in linked list before allocating array, skip if no objects
                int objectCount = CountRdbObjects(reader, ref blocks[block].DFBlock.RdbBlock.ObjectRootList[i]);
                if (objectCount == 0)
                    continue;

                // Create object array
                blocks[block].DFBlock.RdbBlock.ObjectRootList[i].RdbObjects = new DFBlock.RdbObject[objectCount];

                // Read object array
                ReadRdbObjects(reader, ref blocks[block].DFBlock.RdbBlock.ObjectRootList[i]);
            }
        }

        private int CountRdbObjects(BinaryReader reader, ref DFBlock.RdbObjectRoot objectRoot)
        {
            // Go to root of object linked list
            reader.BaseStream.Position = objectRoot.RootOffset;

            // Count objects in list
            int objectCount = 0;
            while(true)
            {
                // Increment object count
                objectCount++;

                // Get next position and exit if finished
                long next = reader.ReadInt32();
                if (next < 0) break;

                // Go to next offset in list
                reader.BaseStream.Position = next;
            }

            return objectCount;
        }

        private void ReadRdbObjects(BinaryReader reader, ref DFBlock.RdbObjectRoot objectRoot)
        {
            // Go to root of object linked list
            reader.BaseStream.Position = objectRoot.RootOffset;

            // Iterate through RDB objects linked list to build managed
            // array and collect baseline information about all objects.
            int index = 0;
            while (true)
            {
                // Read object data
                objectRoot.RdbObjects[index].This = (Int32)reader.BaseStream.Position;
                objectRoot.RdbObjects[index].Next = reader.ReadInt32();
                objectRoot.RdbObjects[index].Previous = reader.ReadInt32();
                objectRoot.RdbObjects[index].Index = index;
                objectRoot.RdbObjects[index].XPos = reader.ReadInt32();
                objectRoot.RdbObjects[index].YPos = reader.ReadInt32();
                objectRoot.RdbObjects[index].ZPos = reader.ReadInt32();
                objectRoot.RdbObjects[index].Type = (DFBlock.RdbResourceTypes)reader.ReadByte();
                objectRoot.RdbObjects[index].ResourceOffset = reader.ReadUInt32();
                objectRoot.RdbObjects[index].Resources.ModelResource.ActionResource.PreviousObjectIndex = -1;
                objectRoot.RdbObjects[index].Resources.ModelResource.ActionResource.NextObjectIndex = -1;

                // Exit if finished
                if (objectRoot.RdbObjects[index].Next < 0)
                    break;

                // Go to next offset in list
                reader.BaseStream.Position = objectRoot.RdbObjects[index].Next;

                // Increment index
                index++;
            }

            // Iterate through managed array to read specific resources for each object
            for (int i = 0; i < objectRoot.RdbObjects.Length; i++)
            {
                // Read resource-specific data
                switch (objectRoot.RdbObjects[i].Type)
                {
                    case DFBlock.RdbResourceTypes.Model:
                        ReadRdbModelResource(reader, ref objectRoot.RdbObjects[i], objectRoot.RdbObjects);
                        break;

                    case DFBlock.RdbResourceTypes.Flat:
                        ReadRdbFlatResource(reader, ref objectRoot.RdbObjects[i]);
                        break;

                    case DFBlock.RdbResourceTypes.Light:
                        ReadRdbLightResource(reader, ref objectRoot.RdbObjects[i]);
                        break;

                    default:
                        throw (new Exception("Unknown RDB resource type encountered."));
                }
            }
        }

        private void ReadRdbModelResource(BinaryReader reader, ref DFBlock.RdbObject rdbObject, DFBlock.RdbObject[] rdbObjects)
        {
            // Go to resource offset
            reader.BaseStream.Position = rdbObject.ResourceOffset;

            // Read model data
            rdbObject.Resources.ModelResource.XRotation = reader.ReadInt32();
            rdbObject.Resources.ModelResource.YRotation = reader.ReadInt32();
            rdbObject.Resources.ModelResource.ZRotation = reader.ReadInt32();
            rdbObject.Resources.ModelResource.ModelIndex = reader.ReadUInt16();
            rdbObject.Resources.ModelResource.Unknown1 = reader.ReadUInt32();
            rdbObject.Resources.ModelResource.SoundId = reader.ReadByte();
            rdbObject.Resources.ModelResource.ActionOffset = reader.ReadInt32();

            // Read action data
            if (rdbObject.Resources.ModelResource.ActionOffset > 0)
                ReadRdbModelActionRecords(reader, ref rdbObject, rdbObjects);
        }

        private void ReadRdbModelActionRecords(BinaryReader reader, ref DFBlock.RdbObject rdbObject, DFBlock.RdbObject[] rdbObjects)
        {
            // Go to action offset
            reader.BaseStream.Position = rdbObject.Resources.ModelResource.ActionOffset;

            // Read action data
            rdbObject.Resources.ModelResource.ActionResource.Position = reader.BaseStream.Position;
            rdbObject.Resources.ModelResource.ActionResource.Axis = (DFBlock.RdbActionAxes)reader.ReadByte();
            rdbObject.Resources.ModelResource.ActionResource.Duration = reader.ReadUInt16();
            rdbObject.Resources.ModelResource.ActionResource.Magnitude = reader.ReadUInt16();
            rdbObject.Resources.ModelResource.ActionResource.NextObjectOffset = reader.ReadInt32();
            rdbObject.Resources.ModelResource.ActionResource.Flags = reader.ReadByte();

            // Exit if no action target
            if (rdbObject.Resources.ModelResource.ActionResource.NextObjectOffset < 0)
                return;

            // Find index of action offset in object array
            int index = 0;
            foreach (DFBlock.RdbObject obj in rdbObjects)
            {
                if (obj.This ==
                    rdbObject.Resources.ModelResource.ActionResource.NextObjectOffset)
                {
                    // Set target and and parent indices
                    rdbObject.Resources.ModelResource.ActionResource.NextObjectIndex = index;
                    rdbObjects[index].Resources.ModelResource.ActionResource.PreviousObjectIndex = rdbObject.Index;
                    break;
                }
                index++;
            }
        }

        private void ReadRdbFlatResource(BinaryReader reader, ref DFBlock.RdbObject rdbObject)
        {
            // Go to resource offset
            reader.BaseStream.Position = rdbObject.ResourceOffset;

            // Read flat data
            rdbObject.Resources.FlatResource.TextureBitfield = reader.ReadUInt16();
            rdbObject.Resources.FlatResource.TextureArchive = rdbObject.Resources.FlatResource.TextureBitfield >> 7;
            rdbObject.Resources.FlatResource.TextureRecord = rdbObject.Resources.FlatResource.TextureBitfield & 0x7f;
            rdbObject.Resources.FlatResource.Gender = (DFBlock.RdbFlatGenders)reader.ReadUInt16();
            rdbObject.Resources.FlatResource.FactionMobileId = reader.ReadUInt16();
            rdbObject.Resources.FlatResource.FlatData.Unknown1 = reader.ReadByte();
            rdbObject.Resources.FlatResource.FlatData.Unknown2 = reader.ReadByte();
            rdbObject.Resources.FlatResource.FlatData.Unknown3 = reader.ReadByte();
            rdbObject.Resources.FlatResource.FlatData.Unknown4 = reader.ReadByte();
            rdbObject.Resources.FlatResource.FlatData.Reaction = reader.ReadByte();
        }

        private void ReadRdbLightResource(BinaryReader reader, ref DFBlock.RdbObject rdbObject)
        {
            // Go to resource offset
            reader.BaseStream.Position = rdbObject.ResourceOffset;

            // Read light data
            rdbObject.Resources.LightResource.Unknown1 = reader.ReadUInt32();
            rdbObject.Resources.LightResource.Unknown2 = reader.ReadUInt32();
            rdbObject.Resources.LightResource.Radius = reader.ReadUInt16();
        }

        #endregion

        #region RDI Readers

        /// <summary>
        /// RDI data is currently an unknown format of 512 bytes in length.
        /// </summary>
        /// <param name="reader">BinaryReader to start of data.</param>
        /// <param name="block">Block index.</param>
        private void ReadRdiRecord(BinaryReader reader, int block)
        {
            // Each RDI block is 512 bytes of unknown data
            blocks[block].DFBlock.RdiBlock.Data = reader.ReadBytes(512);
        }

        #endregion
    }
}
