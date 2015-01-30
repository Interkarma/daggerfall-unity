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
using DaggerfallConnect.InternalTypes;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to ARCH3D.BSA to enumerate and extract 3D mesh data.
    /// </summary>
    public class Arch3dFile
    {
        #region Class Variables

        // Buffer lengths used during decomposition
        private const int cornerBufferLength = 16;
        private const int uniqueTextureBufferLength = 32;
        private const int subMeshBufferLength = 32;
        private const int planeBufferLength = 512;
        private const int pointBufferLength = 16;
        private const int indexBufferLength = 16;
        private const int calculatedUVBufferLength = 24;

        // Divisors for points and textures
        private const float pointDivisor = 256.0f;
        private const float textureDivisor = 16.0f;

        // Buffer arrays used during decomposition
        private int[] cornerPointBuffer = new int[cornerBufferLength];
        private TextureIndex[] uniqueTextureBuffer = new TextureIndex[uniqueTextureBufferLength];
        private DFSubMeshBuffer[] subMeshBuffer = new DFSubMeshBuffer[subMeshBufferLength];
        private FaceUVTool.DFPurePoint[] calculatedUVBuffer = new FaceUVTool.DFPurePoint[calculatedUVBufferLength];

        /// <summary>
        /// Index lookup dictionary.
        /// </summary>
        private Dictionary<UInt32, int> recordIndexLookup = new Dictionary<UInt32, int>();

        /// <summary>
        /// Auto-discard behaviour enabled or disabled.
        /// </summary>
        private bool autoDiscardValue = true;

        /// <summary>
        /// The last record opened. Used by autoDiscard logic
        /// </summary>
        private int lastRecord = -1;

        /// <summary>
        /// The BsaFile representing ARCH3D.BSA
        /// </summary>
        private BsaFile bsaFile = new BsaFile();

        /// <summary>
        /// Array of decomposed mesh records.
        /// </summary>
        private MeshRecord[] records;

        /// <summary>
        /// Object for calculating UV values of face points
        /// </summary>
        private FaceUVTool faceUVTool = new FaceUVTool();

        #endregion

        #region Class Structures

        /// <summary>
        /// Possible mesh versions enumeration.
        /// </summary>
        internal enum MeshVersions
        {
            Unknown,
            Version25,
            Version26,
            Version27,
        }

        /// <summary>
        /// Represents ARCH3D.BSA file header.
        /// </summary>
        internal struct FileHeader
        {
            public long Position;
            public String Version;
            public Int32 PointCount;
            public Int32 PlaneCount;
            public UInt32 Radius;
            public UInt64 NullValue1;
            public Int32 PlaneDataOffset;
            public Int32 ObjectDataOffset;
            public Int32 ObjectDataCount;
            public UInt32 Unknown2;
            public UInt64 NullValue2;
            public Int32 PointListOffset;
            public Int32 NormalListOffset;
            public UInt32 Unknown3;
            public Int32 PlaneListOffset;
        }

        ///// <summary>
        ///// Unknown object data.
        ///// </summary>
        //public struct ObjectDataRecord
        //{
        //    public ObjectDataHeader Header;
        //    public ObjectDataSubRecord[] SubRecords;
        //}

        ///// <summary>
        ///// Header to unknown object data.
        ///// </summary>
        //public struct ObjectDataHeader
        //{
        //    public Int32 N1;
        //    public Int32 N2;
        //    public Int32 N3;
        //    public Int32 N4;
        //    public Int16 SubRecordCount;
        //}

        ///// <summary>
        ///// Unknown object data sub-record.
        ///// </summary>
        //public struct ObjectDataSubRecord
        //{
        //    public byte[] Unknown1;
        //}

        /// <summary>
        /// A single mesh record.
        /// </summary>
        internal struct MeshRecord
        {
            public UInt32 ObjectId;
            public MeshVersions Version;
            public FileProxy MemoryFile;
            public FileHeader Header;
            public PureMesh PureMesh;
            //public ObjectDataRecord[] ObjectDataRecords;
            public DFMesh DFMesh;
        }

        /// <summary>
        /// Native plane (face) header.
        /// </summary>
        internal struct PlaneHeader
        {
            public long Position;
            public Byte PlanePointCount;
            public Byte Unknown1;
            public UInt16 Texture;
            public UInt32 Unknown2;
        }

        /// <summary>
        /// A texture index.
        /// </summary>
        internal struct TextureIndex
        {
            public int Archive;
            public int Record;
        }

        /// <summary>
        /// Native mesh.
        /// </summary>
        internal struct PureMesh
        {
            public TextureIndex[] UniqueTextures;
            public PurePlane[] Planes;
        }

        /// <summary>
        /// A single native plane (face).
        /// </summary>
        internal struct PurePlane
        {
            public PlaneHeader Header;
            public TextureIndex TextureIndex;
            public byte[] PlaneData;
            public FaceUVTool.DFPurePoint[] Points;
        }

        /// <summary>
        /// A submesh. All planes of this submesh are grouped by unique texture index.
        /// </summary>
        private struct DFSubMeshBuffer
        {
            public int TextureArchive;
            public int TextureRecord;
            public int planeCount;
            public DFPlaneBuffer[] PlaneBuffer;
        }

        /// <summary>
        /// Buffer for storing plane data during decomposition.
        /// </summary>
        private struct DFPlaneBuffer
        {
            public int PointCount;
            public DFMesh.DFPoint[] PointBuffer;
            public DFMesh.UVGenerationMethods UVGenerationMethod;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Arch3dFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to ARCH3D.BSA file.</param>
        /// <param name="usage">Determines if the BSA file will read from disk or memory.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public Arch3dFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets mesh record array.
        /// </summary>
        internal MeshRecord[] MeshRecords
        {
            get { return records; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// If true then decomposed mesh records will be destroyed every time a different record is fetched.
        ///  If false then decomposed mesh records will be maintained until discardRecord() or discardAllRecords() is called.
        ///  Turning off auto-discard will speed up mesh retrieval times at the expense of RAM. For best results, disable
        ///  auto-discard and impose your own caching scheme using LoadRecord() and DiscardRecord() based on your application's
        ///  needs.
        /// </summary>
        public bool AutoDiscard
        {
            get { return autoDiscardValue; }
            set { autoDiscardValue = value; }
        }

        /// <summary>
        /// Number of BSA records in ARCH3D.BSA.
        /// </summary>
        public int Count
        {
            get {return bsaFile.Count;}
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default ARCH3D.BSA filename.
        /// </summary>
        static public string Filename
        {
            get { return "ARCH3D.BSA"; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load ARCH3D.BSA file.
        /// </summary>
        /// <param name="filePath">Absolute path to ARCH3D.BSA file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            filePath = filePath.ToUpper();
            if (!filePath.EndsWith("ARCH3D.BSA"))
                return false;

            // Load file
            if (!bsaFile.Load(filePath, usage, readOnly))
                return false;

            // Create records array
            records = new MeshRecord[bsaFile.Count];

            return true;
        }

        /// <summary>
        /// Gets index of mesh record with specified id. Does not change the currently loaded record.
        ///  Uses a dictionary to map ID to index so this method will be faster on subsequent calls.
        /// </summary>
        /// <param name="id">ID of mesh.</param>
        /// <returns>Index of found mesh, or -1 if not found.</returns>
        public int GetRecordIndex(uint id)
        {
            // Return known value if already indexed
            if (recordIndexLookup.ContainsKey(id))
                return recordIndexLookup[id];

            // Otherwise find and store index by searching for id
            for (int i = 0; i < Count; i++)
            {
                if (bsaFile.GetRecordId(i) == id)
                {
                    recordIndexLookup.Add(id, i);
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets ID of record from index.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>ID of mesh.</returns>
        public uint GetRecordId(int record)
        {
            return bsaFile.GetRecordId(record);
        }

        /// <summary>
        /// Load a mesh record into memory and decompose it for use.
        /// </summary>
        /// <param name="record">Index of record to load.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool LoadRecord(int record)
        {
            // Validate
            if (record < 0 || record >= bsaFile.Count)
                return false;

            // Exit if file has already been opened
            if (records[record].MemoryFile != null && records[record].PureMesh.Planes != null)
                return true;

            // Auto discard previous record
            if (autoDiscardValue && lastRecord != -1)
                DiscardRecord(lastRecord);

            // Load record
            records[record].MemoryFile = bsaFile.GetRecordProxy(record);
            if (records[record].MemoryFile == null)
                return false;

            // Read record
            if (!Read(record))
            {
                DiscardRecord(record);
                return false;
            }

            // Store in lookup dictionary
            if (!recordIndexLookup.ContainsKey(records[record].ObjectId))
                recordIndexLookup.Add(records[record].ObjectId, record);

            // Set previous record
            lastRecord = record;

            return true;
        }

        /// <summary>
        /// Discard a mesh record from memory.
        /// </summary>
        /// <param name="record">Index of record to discard.</param>
        public void DiscardRecord(int record)
        {
            // Validate
            if (record < 0 || record >= bsaFile.Count)
                return;

            // Discard mesh data stored in memory
            records[record].ObjectId = 0;
            records[record].Version = MeshVersions.Unknown;
            records[record].MemoryFile = null;
            records[record].PureMesh.Planes = null;
            records[record].DFMesh.ObjectId = 0;
            records[record].DFMesh.TotalVertices = 0;
            records[record].DFMesh.SubMeshes = null;
        }

        /// <summary>
        /// Discard all mesh records.
        /// </summary>
        public void DiscardAllRecords()
        {
            // Clear all records
            for (int record = 0; record < bsaFile.Count; record++)
            {
                DiscardRecord(record);
            }
        }

        /// <summary>
        /// Benchmark mesh decomposition time. Forces on auto-discard behaviour and discards all existing records before starting.
        /// </summary>
        /// <returns>Time to decompose Count meshes in milliseconds.</returns>
        public int Benchmark()
        {
            // Must be ready
            if (bsaFile.Count == 0)
                return -1;

            // Force on autoDiscard and discard all records
            bool curAutoDiscardValue = autoDiscardValue;
            autoDiscardValue = true;
            DiscardAllRecords();

            // Get time in milliseconds for parsing all objects
            int recordCount = bsaFile.Count;
            long startTicks = DateTime.Now.Ticks;
            for (int i = 0; i < recordCount; i++)
            {
                LoadRecord(i);
            }
            long elapsedTicks = (DateTime.Now.Ticks - startTicks);

            // Restore previous autodiscard
            autoDiscardValue = curAutoDiscardValue;

            return (int)elapsedTicks / 10000;
        }

        /// <summary>
        /// Get a DFMesh representation of a record.
        /// </summary>
        /// <param name="record">Index of record to load.</param>
        /// <returns>DFMesh object.</returns>
        public DFMesh GetMesh(int record)
        {
            // Load the record
            if (!LoadRecord(record))
                return new DFMesh();

            return records[record].DFMesh;
        }

        ///// <summary>
        ///// Gets unknown object data records.
        ///// </summary>
        ///// <param name="record">Index of record to get.</param>
        ///// <param name="recordsOut">Record array out.</param>
        ///// <returns>Number of records in array. Can be zero.</returns>
        //public int GetObjectDataRecords(int record, out ObjectDataRecord[] recordsOut)
        //{
        //    // Load the record
        //    if (!LoadRecord(record))
        //    {
        //        recordsOut = null;
        //        return 0;
        //    }

        //    recordsOut = records[record].ObjectDataRecords;
        //    return records[record].Header.ObjectDataCount;
        //}

        #endregion

        #region Private Methods

        /// <summary>
        /// Convert a string to member of meshVersions enumeration.
        /// </summary>
        /// <param name="version">Version of mesh as string</param>
        /// <returns>Member of meshVersions enumeration</returns>
        private MeshVersions GetVersion(string version)
        {
            if (version == "v2.7")
                return MeshVersions.Version27;
            else if (version == "v2.6")
                return MeshVersions.Version26;
            else if (version == "v2.5")
                return MeshVersions.Version25;
            else
                return MeshVersions.Unknown;
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read and decompose a mesh record.
        /// </summary>
        /// <param name="record">The record index to read.</param>
        /// <returns>True if successful, otherwise false</returns>
        private bool Read(int record)
        {
            try
            {
                // Read header
                BinaryReader reader = records[record].MemoryFile.GetReader();
                ReadHeader(reader, record);

                // Store name and version
                records[record].ObjectId = bsaFile.GetRecordId(record);
                records[record].Version = GetVersion(records[record].Header.Version);

                // Read mesh
                if (!ReadMesh(record))
                    return false;

                // Decompose this mesh
                if (!DecomposeMesh(record))
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read ARCH3D.BSA file header.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="record">Destination record index.</param>
        private void ReadHeader(BinaryReader reader, int record)
        {
            reader.BaseStream.Position = 0;
            records[record].Header.Position = 0;
            records[record].Header.Version = records[record].MemoryFile.ReadCString(reader, 4);
            records[record].Header.PointCount = reader.ReadInt32();
            records[record].Header.PlaneCount = reader.ReadInt32();
            records[record].Header.Radius = reader.ReadUInt32();
            records[record].Header.NullValue1 = reader.ReadUInt64();
            records[record].Header.PlaneDataOffset = reader.ReadInt32();
            records[record].Header.ObjectDataOffset = reader.ReadInt32();
            records[record].Header.ObjectDataCount = reader.ReadInt32();
            records[record].Header.Unknown2 = reader.ReadUInt32();
            records[record].Header.NullValue2 = reader.ReadUInt64();
            records[record].Header.PointListOffset = reader.ReadInt32();
            records[record].Header.NormalListOffset = reader.ReadInt32();
            records[record].Header.Unknown3 = reader.ReadUInt32();
            records[record].Header.PlaneListOffset = reader.ReadInt32();
        }

        /// <summary>
        /// Read mesh data to record array.
        /// </summary>
        /// <param name="record">Destination record index.</param>
        private bool ReadMesh(int record)
        {
            // Create empty mesh
            records[record].PureMesh = new PureMesh();

            // Create plane array
            int faceCount = records[record].Header.PlaneCount;
            records[record].PureMesh.Planes = new PurePlane[faceCount];

            // Get reader for normal data
            long normalPosition = records[record].Header.NormalListOffset;
            BinaryReader normalReader = records[record].MemoryFile.GetReader(normalPosition);

            // Read native data into plane array
            int uniqueTextureCount = 0;
            MeshVersions version = records[record].Version;
            long position = records[record].Header.PlaneListOffset;
            BinaryReader reader = records[record].MemoryFile.GetReader(position);
            BinaryReader pointReader = records[record].MemoryFile.GetReader();
            BinaryReader planeDataReader = records[record].MemoryFile.GetReader();
            for (int plane = 0; plane < faceCount; plane++)
            {
                // Read plane header
                records[record].PureMesh.Planes[plane].Header.Position = reader.BaseStream.Position;
                records[record].PureMesh.Planes[plane].Header.PlanePointCount = reader.ReadByte();
                records[record].PureMesh.Planes[plane].Header.Unknown1 = reader.ReadByte();
                records[record].PureMesh.Planes[plane].Header.Texture = reader.ReadUInt16();
                records[record].PureMesh.Planes[plane].Header.Unknown2 = reader.ReadUInt32();

                // Read the normal data for this plane
                Int32 nx = normalReader.ReadInt32();
                Int32 ny = normalReader.ReadInt32();
                Int32 nz = normalReader.ReadInt32();

                // Build list of unique textures across all planes - this will be used later to create submesh buffers
                UInt16 textureBitfield = records[record].PureMesh.Planes[plane].Header.Texture;
                int textureArchive = textureBitfield >> 7;
                int textureRecord = textureBitfield & 0x7f;
                bool foundTexture = false;
                for (int i = 0; i < uniqueTextureCount; i++)
                {
                    if (uniqueTextureBuffer[i].Archive == textureArchive && uniqueTextureBuffer[i].Record == textureRecord)
                    {
                        foundTexture = true;
                        break;
                    }
                }
                if (!foundTexture)
                {
                    uniqueTextureBuffer[uniqueTextureCount].Archive = textureArchive;
                    uniqueTextureBuffer[uniqueTextureCount].Record = textureRecord;
                    uniqueTextureCount++;
                }
                
                // Store texture index for this plane
                records[record].PureMesh.Planes[plane].TextureIndex.Archive = textureArchive;
                records[record].PureMesh.Planes[plane].TextureIndex.Record = textureRecord;

                // Read plane points
                int pointCount = records[record].PureMesh.Planes[plane].Header.PlanePointCount;
                records[record].PureMesh.Planes[plane].Points = new FaceUVTool.DFPurePoint[pointCount];
                for (int point = 0; point < pointCount; point++)
                {
                    // Read offset
                    int pointOffset = reader.ReadInt32();

                    // Read UV data
                    Int16 u = reader.ReadInt16();
                    Int16 v = reader.ReadInt16();

                    // Fix certain UV coordinates that are
                    // packed oddly, or aligned outside of poly.
                    int threshold = 14335;
                    while (u > threshold)
                    {
                        u = (Int16)(0x4000 - u);
                    }
                    while (u < -threshold)
                    {
                        u = (Int16)(0x4000 + u);
                    }
                    while (v > threshold)
                    {
                        v = (Int16)(0x4000 - v);
                    }
                    while (v < -threshold)
                    {
                        v = (Int16)(0x4000 + v);
                    }

                    // Store UV coordinates
                    records[record].PureMesh.Planes[plane].Points[point].u = u;
                    records[record].PureMesh.Planes[plane].Points[point].v = v;

                    // Get point position
                    long pointPosition = records[record].Header.PointListOffset;
                    switch (version)
                    {
                        case MeshVersions.Version27:
                        case MeshVersions.Version26:
                            pointPosition += pointOffset;
                            break;
                        case MeshVersions.Version25:
                            pointPosition += (pointOffset * 3);
                            break;
                    }

                    // Store native point values
                    pointReader.BaseStream.Position = pointPosition;
                    records[record].PureMesh.Planes[plane].Points[point].x = pointReader.ReadInt32();
                    records[record].PureMesh.Planes[plane].Points[point].y = pointReader.ReadInt32();
                    records[record].PureMesh.Planes[plane].Points[point].z = pointReader.ReadInt32();

                    // Store native normal values for each vertex
                    records[record].PureMesh.Planes[plane].Points[point].nx = nx;
                    records[record].PureMesh.Planes[plane].Points[point].ny = ny;
                    records[record].PureMesh.Planes[plane].Points[point].nz = nz;
                }

                // Read unknown plane data
                planeDataReader.BaseStream.Position = records[record].Header.PlaneDataOffset + plane * 24;
                records[record].PureMesh.Planes[plane].PlaneData = planeDataReader.ReadBytes(24);
            }

            //// Read unknown object data, but ignore known non-conforming objects
            //if (records[record].Header.ObjectDataCount > 0 &&
            //    records[record].ObjectId != 4722 &&
            //    records[record].ObjectId != 7614)
            //{
            //    // Create object data record array
            //    records[record].ObjectDataRecords = new ObjectDataRecord[records[record].Header.ObjectDataCount];

            //    // Start reading
            //    reader.BaseStream.Position = records[record].Header.ObjectDataOffset;
            //    for (int i = 0; i < records[record].Header.ObjectDataCount; i++)
            //    {
            //        // Read object data record header
            //        records[record].ObjectDataRecords[i].Header.N1 = reader.ReadInt32();
            //        records[record].ObjectDataRecords[i].Header.N2 = reader.ReadInt32();
            //        records[record].ObjectDataRecords[i].Header.N3 = reader.ReadInt32();
            //        records[record].ObjectDataRecords[i].Header.N4 = reader.ReadInt32();
            //        records[record].ObjectDataRecords[i].Header.SubRecordCount = reader.ReadInt16();

            //        // Read unknown sub-records
            //        records[record].ObjectDataRecords[i].SubRecords = new ObjectDataSubRecord[records[record].ObjectDataRecords[i].Header.SubRecordCount];
            //        for (int j = 0; j < records[record].ObjectDataRecords[i].Header.SubRecordCount; j++)
            //        {
            //            records[record].ObjectDataRecords[i].SubRecords[j].Unknown1 = reader.ReadBytes(6);
            //        }
            //    }
            //}

            // Copy valid part of unique texture list into pureMesh data and create plane buffer for decomposition
            records[record].PureMesh.UniqueTextures = new TextureIndex[uniqueTextureCount];
            for (int i = 0; i < uniqueTextureCount; i++)
            {
                records[record].PureMesh.UniqueTextures[i] = uniqueTextureBuffer[i];
                subMeshBuffer[i].TextureArchive = uniqueTextureBuffer[i].Archive;
                subMeshBuffer[i].TextureRecord = uniqueTextureBuffer[i].Record;
                subMeshBuffer[i].planeCount = 0;
                subMeshBuffer[i].PlaneBuffer = new DFPlaneBuffer[planeBufferLength];
            }

            return true;
        }

        /// <summary>
        /// Decompose pure mesh into submeshes grouped by texture and containing a triangle-friendly point strip per plane.
        /// </summary>
        /// <param name="record">Destination record index.</param>
        private bool DecomposeMesh(int record)
        {
            // Create mesh and submesh records
            int uniquetextureCount = records[record].PureMesh.UniqueTextures.Length;
            records[record].DFMesh = new DFMesh();
            records[record].DFMesh.SubMeshes = new DFMesh.DFSubMesh[uniquetextureCount];
            records[record].DFMesh.ObjectId = (int)records[record].ObjectId;
            records[record].DFMesh.Radius = records[record].Header.Radius / pointDivisor;

            // Decompose each plane of this mesh into a buffer
            int planeCount = records[record].PureMesh.Planes.Length;
            for (int plane = 0; plane < planeCount; plane++)
            {
                // Determine which submesh group this plane belongs to based on texture
                int subMeshIndex = GetSubMesh(ref records[record].PureMesh, records[record].PureMesh.Planes[plane].TextureIndex.Archive, records[record].PureMesh.Planes[plane].TextureIndex.Record);

                // Decompose plane into a strip based on number of points
                WritePlane(subMeshIndex, ref records[record].PureMesh.Planes[plane]);
            }

            // Copy valid mesh data from buffer into mesh record
            int totalTriangles = 0;
            for (int submesh = 0; submesh < uniquetextureCount; submesh++)
            {
                // Store texture information
                records[record].DFMesh.SubMeshes[submesh].TextureArchive = subMeshBuffer[submesh].TextureArchive;
                records[record].DFMesh.SubMeshes[submesh].TextureRecord = subMeshBuffer[submesh].TextureRecord;

                // Store plane data
                int bufferPlaneCount = subMeshBuffer[submesh].planeCount;
                records[record].DFMesh.SubMeshes[submesh].Planes = new DFMesh.DFPlane[bufferPlaneCount];
                for (int plane = 0; plane < bufferPlaneCount; plane++)
                {
                    // Store point data for this plane
                    int bufferPointCount = subMeshBuffer[submesh].PlaneBuffer[plane].PointCount;
                    records[record].DFMesh.TotalVertices += bufferPointCount;
                    records[record].DFMesh.SubMeshes[submesh].TotalTriangles += bufferPointCount - 2;
                    records[record].DFMesh.SubMeshes[submesh].Planes[plane].Points = new DFMesh.DFPoint[bufferPointCount];
                    for (int point = 0; point < bufferPointCount; point++)
                    {
                        records[record].DFMesh.SubMeshes[submesh].Planes[plane].Points[point] = subMeshBuffer[submesh].PlaneBuffer[plane].PointBuffer[point];
                        records[record].DFMesh.SubMeshes[submesh].Planes[plane].UVGenerationMethod = subMeshBuffer[submesh].PlaneBuffer[plane].UVGenerationMethod;
                    }
                }

                // Increment total triangle for this submesh
                totalTriangles += records[record].DFMesh.SubMeshes[submesh].TotalTriangles;
            }

            // Store total triangle across whole mesh
            records[record].DFMesh.TotalTriangles = totalTriangles;

            return true;
        }

        /// <summary>
        /// Write points of a plane.
        /// </summary>
        /// <param name="subMeshIndex">Index of the submesh (texture group) to work with.</param>
        /// <param name="planeIn">Source plane.</param>
        private int WritePlane(int subMeshIndex, ref PurePlane planeIn)
        {
            // Handle planes with greater than 3 points
            if (planeIn.Points.Length > 3)
                return WriteVariablePlane(subMeshIndex, ref planeIn);

            // Add new point buffer to submesh buffer
            int planeIndex = subMeshBuffer[subMeshIndex].planeCount;
            subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex].PointBuffer = new DFMesh.DFPoint[pointBufferLength];
            subMeshBuffer[subMeshIndex].planeCount++;

            // Calculate UV coordinates for points 1, 2 (points 1 & 2 are deltas that are added to the previous point)
            planeIn.Points[1].u += planeIn.Points[0].u;
            planeIn.Points[1].v += planeIn.Points[0].v;
            planeIn.Points[2].u += planeIn.Points[1].u;
            planeIn.Points[2].v += planeIn.Points[1].v;

            // Write the 3 points
            WritePoint(ref planeIn.Points[0], ref subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex]);
            WritePoint(ref planeIn.Points[1], ref subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex]);
            WritePoint(ref planeIn.Points[2], ref subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex]);

            // Store UV generation method of this plane
            subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex].UVGenerationMethod = DFMesh.UVGenerationMethods.TriangleOnly;

            return planeIndex;
        }

        /// <summary>
        /// Write a N-point triangle fan to buffer by finding corners.
        /// </summary>
        /// <param name="subMeshIndex">Index of the submesh (texture group) to work with.</param>
        /// <param name="planeIn">Source plane.</param>
        private int WriteVariablePlane(int subMeshIndex, ref PurePlane planeIn)
        {
            // Add new point buffer to submesh buffer
            int planeIndex = subMeshBuffer[subMeshIndex].planeCount;
            subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex].PointBuffer = new DFMesh.DFPoint[pointBufferLength];
            subMeshBuffer[subMeshIndex].planeCount++;

            // Find corner points
            int cornerCount = GetCornerPoints(ref planeIn.Points);

            // Calculate UV coordinates of all points
            if (faceUVTool.ComputeFaceUVCoordinates(ref planeIn.Points, ref calculatedUVBuffer))
            {
                // Copy calculated UV coordinates
                for (int pt = 0; pt < planeIn.Points.Length; pt++)
                {
                    planeIn.Points[pt].u = calculatedUVBuffer[pt].u;
                    planeIn.Points[pt].v = calculatedUVBuffer[pt].v;
                }

                // Store UV generation method of this plane
                subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex].UVGenerationMethod = DFMesh.UVGenerationMethods.ModifedMatrixGenerator;
            }

            // Write first 3 points
            int cornerPos = 0;
            WritePoint(ref planeIn.Points[cornerPointBuffer[cornerPos++]], ref subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex]);
            WritePoint(ref planeIn.Points[cornerPointBuffer[cornerPos++]], ref subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex]);
            WritePoint(ref planeIn.Points[cornerPointBuffer[cornerPos++]], ref subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex]);

            // Write remaining points
            while (cornerPos < cornerCount)
            {
                WritePoint(ref planeIn.Points[cornerPointBuffer[cornerPos++]], ref subMeshBuffer[subMeshIndex].PlaneBuffer[planeIndex]);
            }

            return planeIndex;
        }

        /// <summary>
        /// Write a single point to buffer.
        /// </summary>
        /// <param name="srcPoint">Source point.</param>
        /// <param name="dstPlane">Destination plane buffer.</param>
        /// <implementation>
        /// Vector coordinates are divided by 256.0f, and texture coordinates by 16.0f.
        /// </implementation>
        private int WritePoint(ref FaceUVTool.DFPurePoint srcPoint, ref DFPlaneBuffer dstPlane)
        {
            // Copy point data
            int pointPos = dstPlane.PointCount;
            dstPlane.PointBuffer[pointPos].X = srcPoint.x / pointDivisor;
            dstPlane.PointBuffer[pointPos].Y = srcPoint.y / pointDivisor;
            dstPlane.PointBuffer[pointPos].Z = srcPoint.z / pointDivisor;
            dstPlane.PointBuffer[pointPos].NX = srcPoint.nx / pointDivisor;
            dstPlane.PointBuffer[pointPos].NY = srcPoint.ny / pointDivisor;
            dstPlane.PointBuffer[pointPos].NZ = srcPoint.nz / pointDivisor;
            dstPlane.PointBuffer[pointPos].U = srcPoint.u / textureDivisor;
            dstPlane.PointBuffer[pointPos].V = srcPoint.v / textureDivisor;
            dstPlane.PointCount++;

            return pointPos;
        }

        /// <summary>
        /// Find submesh this archive/record combo will belong.
        /// </summary>
        /// <param name="mesh">Source mesh to search.</param>
        /// <param name="textureArchive">Texture archive value to match.</param>
        /// <param name="textureRecord">Texture index value to match.</param>
        /// <returns>Index of submesh matching this texture.</returns>
        private int GetSubMesh(ref PureMesh mesh, int textureArchive, int textureRecord)
        {
            for (int i = 0; i < mesh.UniqueTextures.Length; i++)
            {
                if (mesh.UniqueTextures[i].Archive == textureArchive && mesh.UniqueTextures[i].Record == textureRecord)
                    return i;
            }

            throw new Exception("GetSubMesh() index not found.");
        }

        /// <summary>
        /// Find corner points from a pure face - this reduces the number of points in the final strip.
        /// </summary>
        /// <param name="pointsIn">Source points to find corners of.</param>
        /// <returns>Number of corners found in this point array.</returns>
        private int GetCornerPoints(ref FaceUVTool.DFPurePoint[] pointsIn)
        {
            int cornerCount = 0;
            Vector3 v0, v1, v2, l0, l1;
            int pointCount = pointsIn.Length;
            for (int point = 0; point < pointCount; point++)
            {
                // Determine angle between this point and next two points
                int cornerIndex;
                if (point < pointCount - 2)
                {
                    v0 = new Vector3(pointsIn[point].x, pointsIn[point].y, pointsIn[point].z);
                    v1 = new Vector3(pointsIn[point + 1].x, pointsIn[point + 1].y, pointsIn[point + 1].z);
                    v2 = new Vector3(pointsIn[point + 2].x, pointsIn[point + 2].y, pointsIn[point + 2].z);
                    cornerIndex = point + 1;
                }
                else if (point < pointCount - 1)
                {
                    v0 = new Vector3(pointsIn[point].x, pointsIn[point].y, pointsIn[point].z);
                    v1 = new Vector3(pointsIn[point + 1].x, pointsIn[point + 1].y, pointsIn[point + 1].z);
                    v2 = new Vector3(pointsIn[0].x, pointsIn[0].y, pointsIn[0].z);
                    cornerIndex = point + 1;
                }
                else
                {
                    v0 = new Vector3(pointsIn[point].x, pointsIn[point].y, pointsIn[point].z);
                    v1 = new Vector3(pointsIn[0].x, pointsIn[0].y, pointsIn[0].z);
                    v2 = new Vector3(pointsIn[1].x, pointsIn[1].y, pointsIn[1].z);
                    cornerIndex = 0;
                }

                // Construct direction vectors
                l0 = v1 - v0;
                l1 = v2 - v0;

                // Check angle between direction vectors
                double angle = l0.Angle(l1);
                if (angle > 0.001f)
                {
                    // Write corner point to buffer
                    cornerPointBuffer[cornerCount++] = cornerIndex;
                }
            }

            return cornerCount;
        }

        #endregion
    }
}
