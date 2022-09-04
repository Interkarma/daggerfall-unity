// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Andrzej Łukasik (andrew.r.lukasik)
// 
// Notes:
//

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using Unity.Profiling;
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
        private readonly Dictionary<String, int> blockNameLookup = new Dictionary<String, int>();

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
        internal BsaFile BsaFile => bsaFile;

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
            => Load(filePath, usage, readOnly);

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
        public int Count => bsaFile.Count;

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default BLOCKS.BSA filename.
        /// </summary>
        static public string Filename => "BLOCKS.BSA";

        /// <summary>
        /// Gets rotation divisor used when rotating
        ///  block records and models into place.
        /// </summary>
        static public float RotationDivisor => 5.68888888888889f;

        /// <summary>
        /// Gets dimension of a single RMB block.
        /// </summary>
        static public float RMBDimension => 4096f;

        /// <summary>
        /// Gets dimension of a single RMB ground tile.
        /// </summary>
        static public float TileDimension => 256f;

        /// <summary>
        /// Gets dimension of a single RDB block.
        /// </summary>
        static public float RDBDimension => 2048f;

        /// <summary>
        /// Gets scale divisor for billboards.
        /// </summary>
        static public float ScaleDivisor => 256f;

        #endregion

        #region Profiler Markers

        static readonly ProfilerMarker
            ___GetBlockIndex = new ProfilerMarker(nameof(GetBlockIndex)),
            ___LoadBlock = new ProfilerMarker(nameof(LoadBlock)),
            ___DiscardBlock = new ProfilerMarker(nameof(DiscardBlock)),
            ___DiscardAllBlocks = new ProfilerMarker(nameof(DiscardAllBlocks)),
            ___SearchAlternateRMBName = new ProfilerMarker(nameof(SearchAlternateRMBName)),
            ___FixRdbData = new ProfilerMarker(nameof(FixRdbData)),
            ___ReadBlock = new ProfilerMarker(nameof(ReadBlock)),
            ___ReadRmbFldHeader = new ProfilerMarker(nameof(ReadRmbFldHeader)),
            ___ReadRmbGroundTilesData = new ProfilerMarker(nameof(ReadRmbGroundTilesData)),
            ___ReadRmbGroundSceneryData = new ProfilerMarker(nameof(ReadRmbGroundSceneryData)),
            ___ReadRmbBlockData = new ProfilerMarker(nameof(ReadRmbBlockData)),
            ___ReadRmbBlockSubRecord = new ProfilerMarker(nameof(ReadRmbBlockSubRecord)),
            ___ReadRmbModelRecords = new ProfilerMarker(nameof(ReadRmbModelRecords)),
            ___ReadRmbFlatObjectRecords = new ProfilerMarker(nameof(ReadRmbFlatObjectRecords)),
            ___ReadRdbHeader = new ProfilerMarker(nameof(ReadRdbHeader)),
            ___ReadRdbModelReferenceList = new ProfilerMarker(nameof(ReadRdbModelReferenceList)),
            ___ReadRdbModelDataList = new ProfilerMarker(nameof(ReadRdbModelDataList)),
            ___ReadRdbObjectSectionHeader = new ProfilerMarker(nameof(ReadRdbObjectSectionHeader)),
            ___ReadRdbUnknownLinkedList = new ProfilerMarker(nameof(ReadRdbUnknownLinkedList)),
            ___ReadRdbObjectSectionRootList = new ProfilerMarker(nameof(ReadRdbObjectSectionRootList)),
            ___ReadRdbObjectLists = new ProfilerMarker(nameof(ReadRdbObjectLists)),
            ___CountRdbObjects = new ProfilerMarker(nameof(CountRdbObjects)),
            ___ReadRdbObjects = new ProfilerMarker(nameof(ReadRdbObjects)),
            ___ReadRdbModelResource = new ProfilerMarker(nameof(ReadRdbModelResource)),
            ___ReadRdbModelActionRecords = new ProfilerMarker(nameof(ReadRdbModelActionRecords)),
            ___ReadRdbFlatResource = new ProfilerMarker(nameof(ReadRdbFlatResource)),
            ___ReadRdbLightResource = new ProfilerMarker(nameof(ReadRdbLightResource));
        
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
            if (!filePath.EndsWith("BLOCKS.BSA", StringComparison.InvariantCultureIgnoreCase))
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
            => WorldDataReplacement.GetNewDFBlockName(block) ?? bsaFile.GetRecordName(block);

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
            ___GetBlockIndex.Begin();

            // Return known value if already indexed
            if (blockNameLookup.ContainsKey(name))
            {
                ___GetBlockIndex.End();
                return blockNameLookup[name];
            }

            // Check for any new blocks added next
            int blockIndex = WorldDataReplacement.GetNewDFBlockIndex(name);
            if (blockIndex != -1)
            {
                ___GetBlockIndex.End();
                return blockIndex;
            }

            // Otherwise find and store index by searching for name
            for (int i = 0; i < Count; i++)
            {
                if (GetBlockName(i) == name)
                {
                    // Found the block, add to dictionary and return
                    blockNameLookup.Add(name, i);
                    ___GetBlockIndex.End();
                    return i;
                }
            }

            ___GetBlockIndex.End();
            return -1;
        }

        /// <summary>
        /// Load a block into memory and decompose it for use.
        /// </summary>
        /// <param name="block">Index of block to load.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool LoadBlock(int block)
        {
            ___LoadBlock.Begin();

            // Validate
            if (block < 0 || block >= bsaFile.Count)
            {
                ___LoadBlock.End();
                return false;
            }

            // Exit if file has already been opened
            ref var refBlock = ref blocks[block];
            if (refBlock.MemoryFile != null)
            {
                ___LoadBlock.End();
                return true;
            }

            // Auto discard previous record
            if (autoDiscardValue && lastBlock != -1)
                DiscardBlock(lastBlock);

            // Load record data
            refBlock.MemoryFile = bsaFile.GetRecordProxy(block);
            if (refBlock.MemoryFile == null)
            {
                ___LoadBlock.End();
                return false;
            }

            // Set record name
            refBlock.Name = bsaFile.GetRecordName(block);
            refBlock.DFBlock.Name = bsaFile.GetRecordName(block);

            // Set record type
            refBlock.DFBlock.Type = GetBlockType(block);

            // Set record position and index
            refBlock.DFBlock.Position = bsaFile.GetRecordPosition(block);
            refBlock.DFBlock.Index = block;

            // Read record
            if (!Read(block))
            {
                DiscardBlock(block);
                ___LoadBlock.End();
                return false;
            }

            // Store in lookup dictionary
            if (!blockNameLookup.ContainsKey(refBlock.Name))
                blockNameLookup.Add(refBlock.Name, block);

            // Set previous record
            lastBlock = block;

            ___LoadBlock.End();
            return true;
        }

        /// <summary>
        /// Discard a block from memory.
        /// </summary>
        /// <param name="block">Index of block to discard.</param>
        public void DiscardBlock(int block)
        {
            ___DiscardBlock.Begin();
            
            // Validate
            if (block >= bsaFile.Count)
            {
                ___DiscardBlock.End();
                return;
            }

            // Discard memory file and other data
            ref var refBlock = ref blocks[block];
            refBlock.Name = string.Empty;
            refBlock.DFBlock.Type = DFBlock.BlockTypes.Unknown;
            refBlock.MemoryFile = null;

            ref var refRmbBlock = ref refBlock.DFBlock.RmbBlock;
            refRmbBlock.Misc3dObjectRecords = null;
            refRmbBlock.MiscFlatObjectRecords = null;
            refRmbBlock.SubRecords = null;

            ref var refRdbBlock = ref refBlock.DFBlock.RdbBlock;
            refRdbBlock.ModelDataList = null;
            refRdbBlock.ModelReferenceList = null;
            refRdbBlock.ObjectRootList = null;

            ___DiscardBlock.End();
        }

        /// <summary>
        /// Discard all block records.
        /// </summary>
        public void DiscardAllBlocks()
        {
            ___DiscardAllBlocks.Begin();

            for (int block = 0; block < bsaFile.Count; block++)
                DiscardBlock(block);

            ___DiscardAllBlocks.End();
        }

        /// <summary>
        /// Gets a DFBlock representation of a record.
        /// </summary>
        /// <param name="block">Index of block to load.</param>
        /// <returns>DFBlock object.</returns>
        public DFBlock GetBlock(int block)
        {
            // Check for replacement block data and use it if found
            DFBlock dfBlock;
            if (WorldDataReplacement.GetDFBlockReplacementData(block, GetBlockName(block), out dfBlock))
            {
                if (blocks.Length > block)
                    blocks[block].DFBlock = dfBlock;
                return dfBlock;
            }
            else
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
                string alternateName = SearchAlternateRMBName(name);
                if (!string.IsNullOrEmpty(alternateName))
                    index = GetBlockIndex(alternateName);
            }

            return GetBlock(index);
        }

        /// <summary>
        /// Gets block AutoMap by name.
        /// </summary>
        /// <param name="block">Reference to block.</param>
        /// <param name="removeGroundFlats">Filters ground flat "speckles" from the AutoMap.</param>
        /// <returns>DFBitmap object.</returns>
        static public DFBitmap GetBlockAutoMap(in DFBlock block, bool removeGroundFlats)
        {
            // Create DFBitmap and copy data
            DFBitmap dfBitmap = new DFBitmap();
            dfBitmap.Data = block.RmbBlock.FldHeader.AutoMapData;
            dfBitmap.Width = 64;
            dfBitmap.Height = 64;

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
                string alternateName = SearchAlternateRMBName(name);
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
        private string SearchAlternateRMBName(string name)
        {
            ___SearchAlternateRMBName.Begin();

            string found = string.Empty;
            string prefix = name.Substring(0, 4);
            string suffix = name.Substring(name.Length - 6, 6);
            for (int block = 0; block < Count; block++)
            {
                string test = GetBlockName(block);
                if (test.StartsWith(prefix) && test.EndsWith(suffix))
                    found = test;
            }

            ___SearchAlternateRMBName.End();
            return found;
        }
        [System.Obsolete("just remove `ref` keyword as it is here for no reason")]
        private string SearchAlternateRMBName(ref string name) => SearchAlternateRMBName(name);

        /// <summary>
        /// Fix invalid RDB data that prevents access to other dungeon blocks in some cases:
        /// N0000071.RDB: connect the lower western dead-end to the rest of the block
        /// W0000009.RDB: move the exit to another corner (classic applies another but less clean trick)
        /// W0000018.RDB: connect the upper rooms together and fix a wrong door model
        /// N0000071.RDB: connect the lower western dead-end to the rest of the block
        /// </summary>
        /// <param name="block">The index of the block being read.</param>
        private void FixRdbData(int block)
        {
            ___FixRdbData.Begin();

            var objectRootList = blocks[block].DFBlock.RdbBlock.ObjectRootList;

            if (block == 994) // N0000071.RDB
            {
                // Connect the western dead-en to the rest of the block using a long corridor model
                objectRootList[4].RdbObjects[18].Resources.ModelResource.ModelIndex = 10;
                objectRootList[4].RdbObjects[18].XPos = 640;
                objectRootList[5].RdbObjects[8].Resources.ModelResource.ModelIndex = 11;
            }
            else if (block == 945 || block == 946)  // N0000022.RDB or N0000023.RDB
            {
                objectRootList[2].RdbObjects = null; // Remove bad door
            }
            else if (block == 958)  // N0000035.RDB
            {
                objectRootList[0].RdbObjects[36].YPos = -300; // Correct door height
            }
            else if (block == 975)  // N0000052.RDB
            {
                objectRootList[0].RdbObjects[24].XPos = 543; // Correct lever placement
            }
            else if (block == 1025) // W0000009.RDB
            {
                // Add a brick wall door model
                ushort wallModelIndex = 0;
                var modelList = blocks[block].DFBlock.RdbBlock.ModelReferenceList;
                for (ushort i = 0; i < modelList.Length; ++i)
                {
                    // Replace the first non used model reference by the wall
                    if (modelList[i].ModelIdNum == 0)
                    {
                        var model = new DFBlock.RdbModelReference
                        {
                            ModelId = "72100",
                            ModelIdNum = 72100,
                            Description = "DOR"
                        };
                        modelList[i] = model;
                        wallModelIndex = i;
                        break;
                    }
                }

                // Replace a corner near the original exit with a model having a door
                ref var refRdbObject = ref objectRootList[5].RdbObjects[39];
                refRdbObject.XPos += 64;
                refRdbObject.ZPos -= 64;
                int x = refRdbObject.XPos;
                int y = refRdbObject.YPos;
                int z = refRdbObject.ZPos;
                refRdbObject.Resources.ModelResource.ModelIndex = 35;
                refRdbObject.Resources.ModelResource.YRotation = -512;
                // Move the exit to the corner door position
                refRdbObject = ref objectRootList[8].RdbObjects[0];
                refRdbObject.XPos = x;
                refRdbObject.ZPos = z + 126;
                // Move the start marker near the exit
                refRdbObject = ref objectRootList[4].RdbObjects[5];
                refRdbObject.XPos = x;
                refRdbObject.ZPos = z;

                // Add an wall to seal the door in case the block is not the starting one
                var rdbObjects = new List<DFBlock.RdbObject>(objectRootList[8].RdbObjects);
                var wall = new DFBlock.RdbObject
                {
                    Index = rdbObjects.Count,
                    XPos = x,
                    YPos = y,
                    ZPos = z + 128,
                    Type = DFBlock.RdbResourceTypes.Model
                };
                wall.Resources.ModelResource.ModelIndex = wallModelIndex;
                wall.Resources.ModelResource.YRotation = -512;
                rdbObjects.Add(wall);
                objectRootList[8].RdbObjects = rdbObjects.ToArray();
            }
            else if (block == 1034) // W0000018.RDB
            {
                // Change two upper corners into T junctions
                objectRootList[4].RdbObjects[38].Resources.ModelResource.ModelIndex = 10;
                objectRootList[4].RdbObjects[38].Resources.ModelResource.YRotation = -512;
                objectRootList[5].RdbObjects[40].Resources.ModelResource.ModelIndex = 10;
                objectRootList[5].RdbObjects[40].Resources.ModelResource.YRotation = -2560;

                // Add a corridor to join the above junctions
                List<DFBlock.RdbObject> rdbObjects = new List<DFBlock.RdbObject>(objectRootList[6].RdbObjects);
                DFBlock.RdbObject corridor = new DFBlock.RdbObject
                {
                    Index = rdbObjects.Count,
                    XPos = 1152,
                    YPos = -1792,
                    ZPos = 1920,
                    Type = DFBlock.RdbResourceTypes.Model
                };
                corridor.Resources.ModelResource.ModelIndex = 11;
                corridor.Resources.ModelResource.YRotation = -512;
                rdbObjects.Add(corridor);
                objectRootList[6].RdbObjects = rdbObjects.ToArray();

                // Fix a door
                objectRootList[2].RdbObjects[0].Resources.ModelResource.ModelIndex = 23;
            }
            else if (block == 1036) // W0000020.RDB
            {
                // Replace the lower western dead-end by an open corridor
                objectRootList[4].RdbObjects[15].Resources.ModelResource.ModelIndex = 5;

                // Replace a nearby turn by a T junction
                objectRootList[5].RdbObjects[34].Resources.ModelResource.ModelIndex = 7;
                objectRootList[5].RdbObjects[34].Resources.ModelResource.YRotation = -512;

                // Add a stair to join the above models
                List<DFBlock.RdbObject> rdbObjects = new List<DFBlock.RdbObject>(objectRootList[5].RdbObjects);
                DFBlock.RdbObject corridor = new DFBlock.RdbObject
                {
                    Index = rdbObjects.Count,
                    XPos = 1152,
                    YPos = -384,
                    ZPos = 1408,
                    Type = DFBlock.RdbResourceTypes.Model
                };
                corridor.Resources.ModelResource.ModelIndex = 4;
                corridor.Resources.ModelResource.YRotation = 512;
                rdbObjects.Add(corridor);
                objectRootList[5].RdbObjects = rdbObjects.ToArray();
            }

            ___FixRdbData.End();
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
            ___ReadBlock.Begin();

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
                FixRdbData(block);
            }
            else if (blocks[block].DFBlock.Type == DFBlock.BlockTypes.Rdi)
            {
                // Read RDI data
                ReadRdiRecord(reader, block);
            }
            else
            {
                DiscardBlock(block);
                ___ReadBlock.End();
                return;
            }

            ___ReadBlock.End();
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
            ___ReadRmbFldHeader.Begin();

            ref var refFldHeader = ref blocks[block].DFBlock.RmbBlock.FldHeader;

            // Record counts
            refFldHeader.NumBlockDataRecords = reader.ReadByte();
            refFldHeader.NumMisc3dObjectRecords = reader.ReadByte();
            refFldHeader.NumMiscFlatObjectRecords = reader.ReadByte();

            // Block positions
            refFldHeader.BlockPositions = new DFBlock.RmbFldBlockPositions[32];
            for (int i = 0; i < 32; i++)
            {
                ref var refBlockPositions = ref refFldHeader.BlockPositions[i];
                refBlockPositions.Unknown1 = reader.ReadUInt32();
                refBlockPositions.Unknown2 = reader.ReadUInt32();
                refBlockPositions.XPos = reader.ReadInt32();
                refBlockPositions.ZPos = reader.ReadInt32();
                refBlockPositions.YRotation = reader.ReadInt32();
            }

            // Building data list
            refFldHeader.BuildingDataList = new DFLocation.BuildingData[32];
            for (int i = 0; i < 32; i++)
            {
                ref var refBuildingData = ref refFldHeader.BuildingDataList[i];
                refBuildingData.NameSeed = reader.ReadUInt16();
                refBuildingData.ServiceTimeLimit = reader.ReadUInt32();
                refBuildingData.Unknown = reader.ReadUInt16();
                refBuildingData.Unknown2 = reader.ReadUInt16();
                refBuildingData.Unknown3 = reader.ReadUInt32();
                refBuildingData.Unknown4 = reader.ReadUInt32();
                refBuildingData.FactionId = reader.ReadUInt16();
                refBuildingData.Sector = reader.ReadInt16();
                refBuildingData.LocationId = reader.ReadUInt16();
                refBuildingData.BuildingType = (DFLocation.BuildingTypes)reader.ReadByte();
                refBuildingData.Quality = reader.ReadByte();
            }

            // Section2 unknown data
            //refFldHeader.Section2UnknownData = reader.ReadBytes(128);
            refFldHeader.Section2UnknownData = new UInt32[32];
            for (int i = 0; i < 32; i++)
                refFldHeader.Section2UnknownData[i] = reader.ReadUInt32();

            // Block data sizes
            refFldHeader.BlockDataSizes = new Int32[32];
            for (int i = 0; i < 32; i++)
                refFldHeader.BlockDataSizes[i] = reader.ReadInt32();

            // Ground data
            refFldHeader.GroundData.Header = reader.ReadBytes(8);
            ReadRmbGroundTilesData(reader, block);
            ReadRmbGroundSceneryData(reader, block);

            // Automap
            refFldHeader.AutoMapData = reader.ReadBytes(64 * 64);

            // Filenames
            refFldHeader.Name = FileProxy.ReadCString(reader, 13);
            refFldHeader.OtherNames = new string[32];
            for (int i = 0; i < 32; i++)
                refFldHeader.OtherNames[i] = FileProxy.ReadCString(reader, 13);
            
            ___ReadRmbFldHeader.End();
        }

        /// <summary>
        /// Read ground tile data for this block.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="block">Destination block index.</param>
        private void ReadRmbGroundTilesData(BinaryReader reader, int block)
        {
            ___ReadRmbGroundTilesData.Begin();

            // Create new array
            var array = new DFBlock.RmbGroundTiles[16, 16];

            // Read in data
            for (int y = 0; y < 16; y++)
            for (int x = 0; x < 16; x++)
            {
                // Read source bitfield
                byte bitfield = reader.ReadByte();

                // Store data
                array[x, y] = new DFBlock.RmbGroundTiles
                {
                    TileBitfield = bitfield,
                    TextureRecord = bitfield & 0x3f,
                    IsRotated = ((bitfield & 0x40) == 0x40),
                    IsFlipped = ((bitfield & 0x80) == 0x80),
                };
            }
            blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundTiles = array;

            ___ReadRmbGroundTilesData.End();
        }


        /// <summary>
        /// Read ground scenery data for this block.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="block">Destination block index.</param>
        private void ReadRmbGroundSceneryData(BinaryReader reader, int block)
        {
            ___ReadRmbGroundSceneryData.Begin();

            // Create new array
            var array = new DFBlock.RmbGroundScenery[16, 16];

            // Read in data
            for (int y = 0; y < 16; y++)
            for (int x = 0; x < 16; x++)
            {
                // Read source bitfield
                byte bitfield = reader.ReadByte();

                // Store data
                if (bitfield < 255)
                {
                    array[x, y] = new DFBlock.RmbGroundScenery
                    {
                        TileBitfield = bitfield,
                        Unknown1 = bitfield & 0x03,
                        TextureRecord = bitfield / 0x04 - 1,
                    };
                }
                else
                {
                    array[x, y] = new DFBlock.RmbGroundScenery
                    {
                        TileBitfield = bitfield,
                        Unknown1 = 0,
                        TextureRecord = -1,
                    };
                }
            }
            blocks[block].DFBlock.RmbBlock.FldHeader.GroundData.GroundScenery = array;

            ___ReadRmbGroundSceneryData.End();
        }

        /// <summary>
        /// Read RMB block data, i.e. the outside and inside repeating sections.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="block">Destination block index.</param>
        private void ReadRmbBlockData(BinaryReader reader, int block)
        {
            ___ReadRmbBlockData.Begin();

            // aliases
            ref var refBlock = ref blocks[block];
            ref var refFldHeader = ref refBlock.DFBlock.RmbBlock.FldHeader;
            ref var refRmbRmbBlock = ref refBlock.DFBlock.RmbBlock;

            // Read block data
            int recordCount = refFldHeader.NumBlockDataRecords;
            refRmbRmbBlock.SubRecords = new DFBlock.RmbSubRecord[recordCount];
            long position = reader.BaseStream.Position;
            for (int i = 0; i < recordCount; i++)
            {
                ref var refSubRecord = ref refRmbRmbBlock.SubRecords[i];

                // Check for replacement building data and use it if found
                if (WorldDataReplacement.GetBuildingReplacementData(blocks[block].Name, block, i, out var buildingReplacementData))
                {
                    ref var refBuilding = ref refFldHeader.BuildingDataList[i];

                    refSubRecord = buildingReplacementData.RmbSubRecord;

                    if (buildingReplacementData.FactionId > 0)
                        refBuilding.FactionId = buildingReplacementData.FactionId;
                    refBuilding.BuildingType = (DFLocation.BuildingTypes)buildingReplacementData.BuildingType;
                    if (buildingReplacementData.Quality > 0)
                        refBuilding.Quality = buildingReplacementData.Quality;
                    if (buildingReplacementData.NameSeed > 0)
                        refBuilding.NameSeed = buildingReplacementData.NameSeed;
                    if (buildingReplacementData.AutoMapData != null && buildingReplacementData.AutoMapData.Length == 64 * 64)
                        refFldHeader.AutoMapData = buildingReplacementData.AutoMapData;
                }
                else
                {
                    // Copy XZ position and Y rotation into subrecord data for convenience
                    ref var refBlockPositions = ref refFldHeader.BlockPositions[i];
                    refSubRecord.XPos = refBlockPositions.XPos;
                    refSubRecord.ZPos = refBlockPositions.ZPos;
                    refSubRecord.YRotation = refBlockPositions.YRotation;

                    // Read outside and inside block data
                    ReadRmbBlockSubRecord(reader, ref refSubRecord.Exterior);
                    ReadRmbBlockSubRecord(reader, ref refSubRecord.Interior);
                }

                // Offset to next position (this ignores padding byte and ensures block reading is correctly stepped)
                position += refFldHeader.BlockDataSizes[i];
                reader.BaseStream.Position = position;
            }

            ___ReadRmbBlockData.End();
        }

        /// <summary>
        /// Read miscellaneous block 3D objects.
        /// </summary>
        /// <param name="reader">A binary reader to file</param>
        /// <param name="block">Destination block index</param>
        private void ReadRmbMisc3dObjects(BinaryReader reader, int block)
        {
            // Read misc block 3d objects
            ref var refRmbBlock = ref blocks[block].DFBlock.RmbBlock;
            refRmbBlock.Misc3dObjectRecords = new DFBlock.RmbBlock3dObjectRecord[refRmbBlock.FldHeader.NumMisc3dObjectRecords];
            ReadRmbModelRecords(reader, ref refRmbBlock.Misc3dObjectRecords);
        }

        /// <summary>
        /// Read miscellaneous block flat objects.
        /// </summary>
        /// <param name="reader">A binary reader to file</param>
        /// <param name="block">Destination block index</param>
        private void ReadRmbMiscFlatObjectRecords(BinaryReader reader, int block)
        {
            // Read misc flat object records
            ref var refRmbBlock = ref blocks[block].DFBlock.RmbBlock;
            refRmbBlock.MiscFlatObjectRecords = new DFBlock.RmbBlockFlatObjectRecord[refRmbBlock.FldHeader.NumMiscFlatObjectRecords];
            ReadRmbFlatObjectRecords(reader, refRmbBlock.MiscFlatObjectRecords);
        }

        /// <summary>
        /// Read block subrecords, i.e. the resources embedded in block data.
        /// </summary>
        /// <param name="reader">A binary reader to file</param>
        /// <param name="blockData">Destination record index</param>
        private void ReadRmbBlockSubRecord(BinaryReader reader, ref DFBlock.RmbBlockData blockData)
        {
            ___ReadRmbBlockSubRecord.Begin();

            // Header
            ref var refHeader = ref blockData.Header;
            refHeader.Position = reader.BaseStream.Position;
            refHeader.Num3dObjectRecords = reader.ReadByte();
            refHeader.NumFlatObjectRecords = reader.ReadByte();
            refHeader.NumSection3Records = reader.ReadByte();
            refHeader.NumPeopleRecords = reader.ReadByte();
            refHeader.NumDoorRecords = reader.ReadByte();
            refHeader.Unknown1 = reader.ReadInt16();
            refHeader.Unknown2 = reader.ReadInt16();
            refHeader.Unknown3 = reader.ReadInt16();
            refHeader.Unknown4 = reader.ReadInt16();
            refHeader.Unknown5 = reader.ReadInt16();
            refHeader.Unknown6 = reader.ReadInt16();

            // 3D object records
            blockData.Block3dObjectRecords = new DFBlock.RmbBlock3dObjectRecord[refHeader.Num3dObjectRecords];
            ReadRmbModelRecords(reader, ref blockData.Block3dObjectRecords);

            // Flat object record
            blockData.BlockFlatObjectRecords = new DFBlock.RmbBlockFlatObjectRecord[refHeader.NumFlatObjectRecords];
            ReadRmbFlatObjectRecords(reader, blockData.BlockFlatObjectRecords);

            // Section3 records
            int numSection3Records = refHeader.NumSection3Records;
            blockData.BlockSection3Records = new DFBlock.RmbBlockSection3Record[numSection3Records];
            for (int i = 0; i < numSection3Records; i++)
            {
                ref var refRecord = ref blockData.BlockSection3Records[i];

                refRecord.XPos = reader.ReadInt32();
                refRecord.YPos = reader.ReadInt32();
                refRecord.ZPos = reader.ReadInt32();
                refRecord.Unknown1 = reader.ReadByte();
                refRecord.Unknown2 = reader.ReadByte();
                refRecord.Unknown3 = reader.ReadInt16();
            }

            // People records
            int numPeopleRecords = refHeader.NumPeopleRecords;
            blockData.BlockPeopleRecords = new DFBlock.RmbBlockPeopleRecord[numPeopleRecords];
            for (int i = 0; i < numPeopleRecords; i++)
            {
                ref var refCharacter = ref blockData.BlockPeopleRecords[i];

                refCharacter.Position = reader.BaseStream.Position;
                refCharacter.XPos = reader.ReadInt32();
                refCharacter.YPos = reader.ReadInt32();
                refCharacter.ZPos = reader.ReadInt32();
                refCharacter.TextureBitfield = reader.ReadUInt16();
                refCharacter.TextureArchive = refCharacter.TextureBitfield >> 7;
                refCharacter.TextureRecord = refCharacter.TextureBitfield & 0x7f;
                refCharacter.FactionID = reader.ReadInt16();
                refCharacter.Flags = reader.ReadByte();
            }

            // Door records
            int numDoorRecords = refHeader.NumDoorRecords;
            blockData.BlockDoorRecords = new DFBlock.RmbBlockDoorRecord[numDoorRecords];
            for (int i = 0; i < numDoorRecords; i++)
            {
                ref var refDoor = ref blockData.BlockDoorRecords[i];

                refDoor.Position = (Int32)reader.BaseStream.Position;
                refDoor.XPos = reader.ReadInt32();
                refDoor.YPos = reader.ReadInt32();
                refDoor.ZPos = reader.ReadInt32();
                refDoor.YRotation = reader.ReadInt16();
                refDoor.OpenRotation = reader.ReadInt16();
                refDoor.DoorModelIndex = reader.ReadByte();
                refDoor.Unknown = reader.ReadByte();
                refDoor.NullValue1 = reader.ReadByte();
            }

            ___ReadRmbBlockSubRecord.End();
        }

        /// <summary>
        /// Read a 3D object subrecord.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="recordsOut">Destination object.</param>
        private void ReadRmbModelRecords(BinaryReader reader, ref DFBlock.RmbBlock3dObjectRecord[] recordsOut)
        {
            ___ReadRmbModelRecords.Begin();

            // Read all 3d object records into array
            for (int i = 0; i < recordsOut.Length; i++)
            {
                ref var refRecord = ref recordsOut[i];

                Int16 objectId1 = reader.ReadInt16();
                Byte objectId2 = reader.ReadByte();
                refRecord.ObjectId1 = objectId1;
                refRecord.ObjectId2 = objectId2;
                refRecord.ModelId = ((objectId1 * 100) + objectId2).ToString();
                refRecord.ModelIdNum = UInt32.Parse(refRecord.ModelId);
                refRecord.ObjectType = reader.ReadByte();
                refRecord.Unknown1 = reader.ReadUInt32();
                refRecord.Unknown2 = reader.ReadUInt32();
                refRecord.Unknown3 = reader.ReadUInt32();
                refRecord.NullValue1 = reader.ReadUInt64();
                refRecord.XPos1 = reader.ReadInt32();
                refRecord.YPos1 = reader.ReadInt32();
                refRecord.ZPos1 = reader.ReadInt32();
                refRecord.XPos = reader.ReadInt32();
                refRecord.YPos = reader.ReadInt32();
                refRecord.ZPos = reader.ReadInt32();
                refRecord.NullValue2 = reader.ReadUInt32();
                refRecord.XRotation = 0;
                refRecord.YRotation = reader.ReadInt16();
                refRecord.ZRotation = 0;
                refRecord.Unknown4 = reader.ReadUInt16();
                refRecord.NullValue3 = reader.ReadUInt32();
                refRecord.Unknown5 = reader.ReadUInt32();
                refRecord.NullValue4 = reader.ReadUInt16();
            }

            ___ReadRmbModelRecords.End();
        }

        /// <summary>
        /// Read a flat object subrecord.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="recordsOut">Destination object.</param>
        private void ReadRmbFlatObjectRecords(BinaryReader reader, DFBlock.RmbBlockFlatObjectRecord[] recordsOut)
        {
            ___ReadRmbFlatObjectRecords.Begin();

            // Read all flat object records into array
            for (int i = 0; i < recordsOut.Length; i++)
            {
                ref var refRecord = ref recordsOut[i];

                refRecord.Position = reader.BaseStream.Position;
                refRecord.XPos = reader.ReadInt32();
                refRecord.YPos = reader.ReadInt32();
                refRecord.ZPos = reader.ReadInt32();
                refRecord.TextureBitfield = reader.ReadUInt16();
                refRecord.TextureArchive = refRecord.TextureBitfield >> 7;
                refRecord.TextureRecord = refRecord.TextureBitfield & 0x7f;
                refRecord.FactionID = reader.ReadInt16();
                refRecord.Flags = reader.ReadByte();
            }

            ___ReadRmbFlatObjectRecords.End();
        }
        [System.Obsolete("just remove `ref` keyword as it is here for no reason")]
        private void ReadRmbFlatObjectRecords(BinaryReader reader, ref DFBlock.RmbBlockFlatObjectRecord[] recordsOut) => ReadRmbFlatObjectRecords(reader, recordsOut);

        #endregion

        #region RDB Readers

        private void ReadRdbHeader(BinaryReader reader, int block)
        {
            ___ReadRdbHeader.Begin();

            ref var refRdbBlock = ref blocks[block].DFBlock.RdbBlock;

            // Read header
            refRdbBlock.Position = reader.BaseStream.Position;
            refRdbBlock.Header.Unknown1 = reader.ReadUInt32();
            refRdbBlock.Header.Width = reader.ReadUInt32();
            refRdbBlock.Header.Height = reader.ReadUInt32();
            refRdbBlock.Header.ObjectRootOffset = reader.ReadUInt32();
            refRdbBlock.Header.Unknown2 = reader.ReadUInt32();

            ___ReadRdbHeader.End();
        }

        private void ReadRdbModelReferenceList(BinaryReader Reader, int Block)
        {
            ___ReadRdbModelReferenceList.Begin();

            // Read model reference list
            var array = new DFBlock.RdbModelReference[750];
            for (int i = 0; i < 750; i++)
            {
                ref var refNext = ref array[i];
                refNext.ModelId = FileProxy.ReadCString(Reader, 5);
                UInt32.TryParse(refNext.ModelId, out refNext.ModelIdNum);
                refNext.Description = FileProxy.ReadCString(Reader, 3);
            }
            blocks[Block].DFBlock.RdbBlock.ModelReferenceList = array;

            ___ReadRdbModelReferenceList.End();
        }

        private void ReadRdbModelDataList(BinaryReader reader, int block)
        {
            ___ReadRdbModelDataList.Begin();

            // Read unknown model data list
            var array = new DFBlock.RdbModelData[750];
            for (int i = 0; i < 750; i++)
                array[i].Unknown1 = reader.ReadUInt32();

            blocks[block].DFBlock.RdbBlock.ModelDataList = array;

            ___ReadRdbModelDataList.End();
        }

        private void ReadRdbObjectSectionHeader(BinaryReader reader, int block)
        {
            ___ReadRdbObjectSectionHeader.Begin();

            ref var refObjectHeader = ref blocks[block].DFBlock.RdbBlock.ObjectHeader;

            // Read object section header
            refObjectHeader.UnknownOffset = reader.ReadUInt32();
            refObjectHeader.Unknown1 = reader.ReadUInt32();
            refObjectHeader.Unknown2 = reader.ReadUInt32();
            refObjectHeader.Unknown3 = reader.ReadUInt32();
            refObjectHeader.Length = reader.ReadUInt32();
            refObjectHeader.Unknown4 = reader.ReadBytes(32);
            refObjectHeader.Dagr = FileProxy.ReadCString(reader, 4);
            refObjectHeader.Unknown5 = reader.ReadBytes(456);

            ___ReadRdbObjectSectionHeader.End();
        }

        private void ReadRdbUnknownLinkedList(BinaryReader reader, int block)
        {
            ___ReadRdbUnknownLinkedList.Begin();

            // Store current reader position
            long position = reader.BaseStream.Position;

            // Go to first unknown object
            ref var refRdbBlock = ref blocks[block].DFBlock.RdbBlock;
            reader.BaseStream.Position = refRdbBlock.ObjectHeader.UnknownOffset;

            // Iterate over list
            int count = 0;
            while (true)
            {
                // Read object data
                DFBlock.RdbUnknownObject obj = new DFBlock.RdbUnknownObject();
                obj.Position = (Int32)reader.BaseStream.Position;
                obj.Next = reader.ReadInt32();
                obj.Index = reader.ReadInt16();
                obj.UnknownOffset = (UInt32)reader.ReadInt32();

                // Create array for first time
                if (refRdbBlock.UnknownObjectList == null)
                {
                    // Index counts backwards, so first item index is also length of list
                    refRdbBlock.UnknownObjectList = new DFBlock.RdbUnknownObject[obj.Index];
                }

                // Exit if finished
                if (obj.Next < 0)
                    break;

                // Store object
                refRdbBlock.UnknownObjectList[count++] = obj;

                // Go to next position
                reader.BaseStream.Position = obj.Next;
            }

            // Revert reader position
            reader.BaseStream.Position = position;

            ___ReadRdbUnknownLinkedList.End();
        }

        private void ReadRdbObjectSectionRootList(BinaryReader reader, int block)
        {
            ___ReadRdbObjectSectionRootList.Begin();

            // Handle improper position in stream
            ref var refRdbBlock = ref blocks[block].DFBlock.RdbBlock;
            ref var refHeader = ref refRdbBlock.Header;
            if (reader.BaseStream.Position != refHeader.ObjectRootOffset)
                throw (new Exception("Start of ObjectRoot section does not match header offset."));

            // Read object section root list
            UInt32 width = refHeader.Width;
            UInt32 height = refHeader.Height;
            var array = new DFBlock.RdbObjectRoot[width * height];
            for (int i = 0; i < width * height; i++)
                array[i].RootOffset = reader.ReadInt32();
            refRdbBlock.ObjectRootList = array;

            ___ReadRdbObjectSectionRootList.End();
        }

        private void ReadRdbObjectLists(BinaryReader reader, int block)
        {
            ___ReadRdbObjectLists.Begin();

            // Read all objects starting from each root position
            ref var refObjectRootList = ref blocks[block].DFBlock.RdbBlock.ObjectRootList;
            for (int i = 0; i < refObjectRootList.Length; i++)
            {

                // Skip if no data present
                if (refObjectRootList[i].RootOffset < 0)
                    continue;

                // Pre-count number of objects in linked list before allocating array, skip if no objects
                int objectCount = CountRdbObjects(reader, ref refObjectRootList[i]);
                if (objectCount == 0)
                    continue;

                // Create object array
                refObjectRootList[i].RdbObjects = new DFBlock.RdbObject[objectCount];

                // Read object array
                ReadRdbObjects(reader, ref refObjectRootList[i]);
            }

            ___ReadRdbObjectLists.End();
        }

        private int CountRdbObjects(BinaryReader reader, ref DFBlock.RdbObjectRoot objectRoot)
        {
            ___CountRdbObjects.Begin();

            // Go to root of object linked list
            var baseStream = reader.BaseStream;
            baseStream.Position = objectRoot.RootOffset;

            // Count objects in list
            int objectCount = 0;
            while (true)
            {
                // Increment object count
                objectCount++;

                // Get next position and exit if finished
                long next = reader.ReadInt32();
                if (next < 0) break;

                // Go to next offset in list
                baseStream.Position = next;
            }

            ___CountRdbObjects.End();
            return objectCount;
        }

        private void ReadRdbObjects(BinaryReader reader, ref DFBlock.RdbObjectRoot objectRoot)
        {
            ___ReadRdbObjects.Begin();

            // Go to root of object linked list
            var baseStream = reader.BaseStream;
            baseStream.Position = objectRoot.RootOffset;

            // Iterate through RDB objects linked list to build managed
            // array and collect baseline information about all objects.
            int index = 0;
            while (true)
            {
                // Read object data
                ref var refNext = ref objectRoot.RdbObjects[index];
                refNext.Position = (Int32)baseStream.Position;
                refNext.Next = reader.ReadInt32();
                refNext.Previous = reader.ReadInt32();
                refNext.Index = index;
                refNext.XPos = reader.ReadInt32();
                refNext.YPos = reader.ReadInt32();
                refNext.ZPos = reader.ReadInt32();
                refNext.Type = (DFBlock.RdbResourceTypes)reader.ReadByte();
                refNext.ResourceOffset = reader.ReadUInt32();
                refNext.Resources.ModelResource.ActionResource.PreviousObjectOffset = -1;
                refNext.Resources.ModelResource.ActionResource.NextObjectIndex = -1;

                // Exit if finished
                if (refNext.Next < 0)
                    break;

                // Go to next offset in list
                baseStream.Position = refNext.Next;

                // Increment index
                index++;
            }

            // Iterate through managed array to read specific resources for each object
            int length = objectRoot.RdbObjects.Length;
            for (int i = 0; i < length; i++)
            {
                // Read resource-specific data
                ref var refNext = ref objectRoot.RdbObjects[i];
                switch (refNext.Type)
                {
                    case DFBlock.RdbResourceTypes.Model:
                        ReadRdbModelResource(reader, ref refNext, objectRoot.RdbObjects);
                        break;

                    case DFBlock.RdbResourceTypes.Flat:
                        ReadRdbFlatResource(reader, ref refNext);
                        break;

                    case DFBlock.RdbResourceTypes.Light:
                        ReadRdbLightResource(reader, ref refNext);
                        break;

                    default: throw new Exception("Unknown RDB resource type encountered.");
                }
            }

            ___ReadRdbObjects.End();
        }

        private void ReadRdbModelResource(BinaryReader reader, ref DFBlock.RdbObject rdbObject, DFBlock.RdbObject[] rdbObjects)
        {
            ___ReadRdbModelResource.Begin();

            // Go to resource offset
            reader.BaseStream.Position = rdbObject.ResourceOffset;

            // Read model data
            ref var refModelResource = ref rdbObject.Resources.ModelResource;
            refModelResource.XRotation = reader.ReadInt32();
            refModelResource.YRotation = reader.ReadInt32();
            refModelResource.ZRotation = reader.ReadInt32();
            refModelResource.ModelIndex = reader.ReadUInt16();
            refModelResource.TriggerFlag_StartingLock = reader.ReadUInt32();
            refModelResource.SoundIndex = reader.ReadByte();
            refModelResource.ActionOffset = reader.ReadInt32();

            // Read action data
            if (refModelResource.ActionOffset > 0)
                ReadRdbModelActionRecords(reader, ref rdbObject, rdbObjects);
            
            ___ReadRdbModelResource.End();
        }

        private void ReadRdbModelActionRecords(BinaryReader reader, ref DFBlock.RdbObject rdbObject, DFBlock.RdbObject[] rdbObjects)
        {
            ___ReadRdbModelActionRecords.Begin();
            
            // Go to action offset
            var baseStream = reader.BaseStream;
            baseStream.Position = rdbObject.Resources.ModelResource.ActionOffset;

            // Read action data
            ref var refActionResource = ref rdbObject.Resources.ModelResource.ActionResource;
            refActionResource.Position = baseStream.Position;
            refActionResource.Axis = reader.ReadByte();
            refActionResource.Duration = reader.ReadUInt16();
            refActionResource.Magnitude = reader.ReadUInt16();
            refActionResource.NextObjectOffset = reader.ReadInt32();
            refActionResource.Flags = reader.ReadByte();

            // Exit if no action target
            if (refActionResource.NextObjectOffset < 0)
            {
                ___ReadRdbModelActionRecords.End();
                return;
            }

            // Find index of action offset in object array
            int index = 0;
            foreach (DFBlock.RdbObject obj in rdbObjects)
            {
                if (obj.Position == refActionResource.NextObjectOffset)
                {
                    // Set target and and parent indices
                    refActionResource.NextObjectIndex = index;
                    rdbObjects[index].Resources.ModelResource.ActionResource.PreviousObjectOffset = rdbObject.Position;
                    break;
                }
                index++;
            }

            ___ReadRdbModelActionRecords.End();
        }

        private void ReadRdbFlatResource(BinaryReader reader, ref DFBlock.RdbObject rdbObject)
        {
            ___ReadRdbFlatResource.Begin();

            // Go to resource offset
            var baseStream = reader.BaseStream;
            baseStream.Position = rdbObject.ResourceOffset;

            // Read flat data
            ref var refFlatResource = ref rdbObject.Resources.FlatResource;
            refFlatResource.Position = baseStream.Position;
            refFlatResource.TextureBitfield = reader.ReadUInt16();
            refFlatResource.TextureArchive = refFlatResource.TextureBitfield >> 7;
            refFlatResource.TextureRecord = refFlatResource.TextureBitfield & 0x7f;
            refFlatResource.Flags = reader.ReadUInt16();
            refFlatResource.Magnitude = reader.ReadByte();
            refFlatResource.SoundIndex = reader.ReadByte();
            refFlatResource.FactionOrMobileId = BitConverter.ToUInt16(new byte[] { refFlatResource.Magnitude, refFlatResource.SoundIndex }, 0);
            refFlatResource.NextObjectOffset = reader.ReadInt32();
            refFlatResource.Action = reader.ReadByte();

            ___ReadRdbFlatResource.End();
        }

        private void ReadRdbLightResource(BinaryReader reader, ref DFBlock.RdbObject rdbObject)
        {
            ___ReadRdbLightResource.Begin();

            // Go to resource offset
            reader.BaseStream.Position = rdbObject.ResourceOffset;

            // Read light data
            ref var refLightResource = ref rdbObject.Resources.LightResource;
            refLightResource.Unknown1 = reader.ReadUInt32();
            refLightResource.Unknown2 = reader.ReadUInt32();
            refLightResource.Radius = reader.ReadUInt16();

            ___ReadRdbLightResource.End();
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
