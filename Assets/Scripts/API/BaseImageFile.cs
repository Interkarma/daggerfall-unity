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
using System.IO;
using DaggerfallConnect.Utility;
using UnityEngine;
using Unity.Profiling;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Provides base image handling for all Daggerfall image files.
    ///  This class is inherited from and extended by Arena2.TextureFile, Arena2.ImgFile, etc.
    /// </summary>
    public abstract class BaseImageFile
    {
        #region Class Variables

        /// <summary>
        /// Palette for building image data
        /// </summary>
        protected DFPalette myPalette = new DFPalette();

        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        #endregion

        #region Profiler Markers

        static readonly ProfilerMarker
            ___m_GetColor32 = new ProfilerMarker(nameof(GetColor32)),
            ___m_GetColor32Async = new ProfilerMarker(nameof(GetColor32Async)),
            ___m_GetWindowColors32 = new ProfilerMarker(nameof(GetWindowColors32));

        #endregion

        #region Class Structures

        /// <summary>
        /// Compression formats enumeration. This is shared as internal as most image formats use some kind of compression.
        /// </summary>
        internal enum CompressionFormats
        {
            Uncompressed = 0x0000,
            RleCompressed = 0x0002,
            ImageRle = 0x0108,
            RecordRle = 0x1108,
        }

        /// <summary>
        /// IMG File header. This is shared as internal as the IMG structure is also used in CIF files.
        ///  The CifFile class will use this file header while reading most records.
        /// </summary>
        internal struct ImgFileHeader
        {
            public long Position;
            public Int16 XOffset;
            public Int16 YOffset;
            public Int16 Width;
            public Int16 Height;
            public CompressionFormats Compression;
            public UInt16 PixelDataLength;
            public int FrameCount;
            public long DataPosition;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseImageFile()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets full path of managed file.
        /// </summary>
        public string FilePath
        {
            get { return managedFile.FilePath; }
        }

        /// <summary>
        /// Gets file name only of managed file.
        /// </summary>
        public string FileName
        {
            get { return Path.GetFileName(FilePath); }
        }

        /// <summary>
        /// Gets or sets palette for building images.
        /// </summary>
        public DFPalette Palette
        {
            get { return myPalette; }
            set { myPalette = value; }
        }

        /// <summary>
        /// Gets total number of records in this file.
        /// </summary>
        public abstract int RecordCount
        {
            get;
        }

        /// <summary>
        /// Gets description of this file.
        /// </summary>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// Gets correct palette name for this file.
        /// </summary>
        public abstract string PaletteName
        {
            get;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads an image file.
        /// </summary>
        /// <param name="filePath">Absolute path to file</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public abstract bool Load(string filePath, FileUsage usage, bool readOnly);

        /// <summary>
        /// Gets number of frames in specified record.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Number of frames.</returns>
        public abstract int GetFrameCount(int record);

        /// <summary>
        /// Gets width and height of specified record. All frames of this record are the same dimensions.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>DFSize object.</returns>
        public abstract DFSize GetSize(int record);

        /// <summary>
        /// Gets bitmap data as indexed 8-bit byte array for specified record and frame.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <param name="frame">Index of frame.</param>
        /// <returns>DFBitmap object.</returns>
        public abstract DFBitmap GetDFBitmap(int record, int frame);

        /// <summary>
        /// Loads a Daggerfall palette that will be used for building images.
        /// </summary>
        /// <param name="filePath">Absolute path to Daggerfall palette.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool LoadPalette(string filePath)
        {
            return myPalette.Load(filePath);
        }

        /// <summary>
        /// Gets a Color32 array for engine with minimum options.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <param name="frame">Index of frame.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColor32(int record, int frame, int alphaIndex = -1)
        {
            DFBitmap srcBitmap = GetDFBitmap(record, frame);
            return GetColor32(srcBitmap, alphaIndex, 0, out var sz);
        }

        /// <summary>
        /// Gets a Color32 array for engine with minimum options.
        /// </summary>
        /// <param name="srcBitmap">Source DFBitmap.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColor32(DFBitmap srcBitmap, int alphaIndex = -1)
        {
            return GetColor32(srcBitmap, alphaIndex, 0, out var sz);
        }

        /// <summary>
        /// Gets a Color32 array for engine with a border.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="border">Number of pixels border to add around image.</param>
        /// <param name="sizeOut">Receives image dimensions with borders included.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColor32(int record, int frame, int alphaIndex, int border, out DFSize sizeOut)
        {
            // Get source bitmap
            DFBitmap srcBitmap = GetDFBitmap(record, frame);

            return GetColor32(srcBitmap, alphaIndex, border, out sizeOut);
        }

        /// <summary>
        /// Gets a Color32 array for engine.
        /// </summary>
        /// <param name="srcBitmap">Source DFBitmap.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="border">Number of pixels border to add around image.</param>
        /// <param name="sizeOut">Receives image dimensions with borders included.</param>
        /// <param name="emissionIndex">Force matching emissive index to non-transparent.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColor32(DFBitmap srcBitmap, int alphaIndex, int border, out DFSize sizeOut, int emissionIndex = -1, int customAlphaValue = 255)
        {
            ___m_GetColor32.Begin();

            // Calculate dimensions
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            int dstWidth = srcWidth + border * 2;
            int dstHeight = srcHeight + border * 2;

            Color32[] colors = new Color32[dstWidth * dstHeight];
            byte[] paletteBuffer = myPalette.PaletteBuffer;
            int paletteHeaderLength = myPalette.HeaderLength;

            for (int y = 0; y < srcHeight; y++)
            {
                // Get row position
                int srcRow = y * srcWidth;
                int dstRow = (dstHeight - 1 - border - y) * dstWidth;

                // Write data for this row
                for (int x = 0; x < srcWidth; x++)
                {
                    int index = srcBitmap.Data[srcRow + x];
                    int offset = paletteHeaderLength + index * 3;
                    Color32 c = new Color32(
                        paletteBuffer[offset],
                        paletteBuffer[offset + 1],
                        paletteBuffer[offset + 2],
                        (byte)customAlphaValue
                    );
                    if(alphaIndex == index) c.a = 0;// General cutout alpha with custom alpha output
                    if(emissionIndex == index) c.a = 255;// Make emissive parts fully non-transparent alpha
                    colors[dstRow + border + x] = c;
                }
            }

            sizeOut = new DFSize(dstWidth, dstHeight);

            ___m_GetColor32.End();
            return colors;
        }
        public JobHandle GetColor32Async(
            DFBitmap srcBitmap,
            int alphaIndex,
            int border,
            out DFSize sizeOut,
            out NativeArray<Color32> colors,
            int emissionIndex = -1,
            byte customAlphaValue = 255,
            Allocator colorsAllocator = Allocator.TempJob,
            JobHandle dependency = default
        )
        {
            ___m_GetColor32Async.Begin();

            // Calculate dimensions
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            int dstWidth = srcWidth + border * 2;
            int dstHeight = srcHeight + border * 2;

            sizeOut = new DFSize(dstWidth, dstHeight);
            colors = new NativeArray<Color32>(dstWidth * dstHeight, colorsAllocator);

            var job = new GetColor32RowJob
            {
                Colors = colors,
                PaletteData = myPalette.PaletteBuffer.AsNativeArray(out ulong gcHandlePalette),
                SrcBitmapData = srcBitmap.Data.AsNativeArray(out ulong gcHandleBitmap),
                SrcWidth = srcWidth,
                DstWidth = dstWidth,
                DstHeight = dstHeight,
                Border = border,
                AlphaIndex = alphaIndex,
                EmissionIndex = emissionIndex,
                PaletteHeaderLength = myPalette.HeaderLength,
                customAlpha = customAlphaValue,
            };
            var jobHandle = job.Schedule(srcHeight, JobUtility.OptimalLoopBatchCount(srcHeight), dependency);
            JobUtility.ReleaseGCObject(gcHandlePalette, jobHandle);
            JobUtility.ReleaseGCObject(gcHandleBitmap, jobHandle);

            ___m_GetColor32Async.End();
            return jobHandle;
        }

        [Unity.Burst.BurstCompile]
        struct GetColor32RowJob : IJobParallelFor
        {
            [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<Color32> Colors;
            [ReadOnly] public NativeArray<byte> SrcBitmapData;
            [ReadOnly] public NativeArray<byte> PaletteData;
            public int SrcWidth, DstWidth, DstHeight, Border, AlphaIndex, EmissionIndex, PaletteHeaderLength;
            public byte customAlpha;
            void IJobParallelFor.Execute(int y)
            {
                // Get row position
                int srcRow = y * SrcWidth;
                int dstRow = (DstHeight - 1 - Border - y) * DstWidth;

                // Write data for this row
                for (int x = 0; x < SrcWidth; x++)
                {
                    int index = SrcBitmapData[srcRow + x];
                    int offset = PaletteHeaderLength + index * 3;
                    var c = new Color32(PaletteData[offset], PaletteData[offset + 1], PaletteData[offset + 2], (byte)customAlpha);
                    if (AlphaIndex == index) c.a = 0;// General cutout alpha with custom alpha output
                    if (EmissionIndex == index) c.a = 255;// Make emissive parts fully non-transparent alpha
                    Colors[dstRow + Border + x] = c;
                }
            }
        }

        /// <summary>
        /// Gets a Color32 array as emission map based for window textures.
        /// </summary>
        /// <param name="srcBitmap">Source bitmap.</param>
        /// <param name="emissionIndex">Index to receive emission colour.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetWindowColors32(DFBitmap srcBitmap, int emissionIndex = 0xff)
        {
            ___m_GetWindowColors32.Begin();

            // Create target array
            DFSize sz = new DFSize(srcBitmap.Width, srcBitmap.Height);
            Color32[] emissionColors = new Color32[sz.Width * sz.Height];

            // Generate emissive parts of texture based on index
            int index, srcRow, dstRow;
            for (int y = 0; y < sz.Height; y++)
            {
                // Get row position
                srcRow = y * sz.Width;
                dstRow = (sz.Height - 1 - y) * sz.Width;

                // Write data for this row
                for (int x = 0; x < sz.Width; x++)
                {
                    index = srcBitmap.Data[srcRow + x];
                    if (index == emissionIndex)
                        emissionColors[dstRow + x] = Color.white;
                }
            }

            ___m_GetWindowColors32.End();
            return emissionColors;
        }
        public JobHandle GetWindowColors32Async(
            DFBitmap srcBitmap,
            out NativeArray<Color32> results,
            int emissionIndex = 0xff,
            Allocator resultsAllocator = Allocator.TempJob,
            JobHandle dependency = default
        )
        {
            ___m_GetWindowColors32.Begin();

            results = new NativeArray<Color32>(srcBitmap.Width * srcBitmap.Height, resultsAllocator);

            var job = new GetWindowColors32RowJob
            {
                Results = results,
                SrcBitmapData = srcBitmap.Data.AsNativeArray(out ulong gcHandle),
                SrcBitmapWidth = srcBitmap.Width,
                SrcBitmapHeight = srcBitmap.Height,
                EmissionIndex = (byte)emissionIndex,
            };
            int numRows = srcBitmap.Height;
            var jobHandle = job.Schedule(numRows, JobUtility.OptimalLoopBatchCount(numRows));
            JobUtility.ReleaseGCObject(gcHandle, jobHandle);

            ___m_GetWindowColors32.End();
            return jobHandle;
        }
        [Unity.Burst.BurstCompile]
        struct GetWindowColors32RowJob : IJobParallelFor
        {
            [WriteOnly] public NativeArray<Color32> Results;
            [ReadOnly] public NativeArray<byte> SrcBitmapData;
            public int
                SrcBitmapWidth,
                SrcBitmapHeight;
            public byte
                EmissionIndex;
            void IJobParallelFor.Execute(int y)
            {
                // Get row position
                int srcRow = y * SrcBitmapWidth;
                int dstRow = (SrcBitmapHeight - 1 - y) * SrcBitmapWidth;

                // Write data for this row
                for (int x = 0; x < SrcBitmapWidth; x++)
                {
                    byte index = SrcBitmapData[srcRow + x];
                    if (index == EmissionIndex)
                        Results[dstRow + x] = Color.white;
                }
            }
        }

        /// <summary>
        /// Gets a Color32 array as emission map for spectral textures (glowing eyes).
        /// Needs extra handling for borders as spectral textures might be atlased.
        /// </summary>
        /// <param name="srcBitmap">Source bitmap.</param>
        /// <param name="borderSize">Size of border for atlased textures.</param>
        /// <param name="eyesEmissionIndex">Index to receive emission colour.</param>
        /// <param name="eyeEmission">Amount of emission to apply to other parts of body (black=none, white=full).</param>
        /// <param name="otherEmission">Amount of emission to apply to other parts of body (black=none, white=full).</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetSpectralEmissionColors32(DFBitmap srcBitmap, Color32[] albedoColors, int borderSize, int eyesEmissionIndex, Color eyeEmission, Color otherEmission)
        {
            // Create target array
            DFSize sz = new DFSize(srcBitmap.Width + borderSize * 2, srcBitmap.Height + borderSize * 2);
            Color32[] emissionColors = new Color32[sz.Width * sz.Height];

            // Generate emissive parts of texture based on index
            for (int y = 0; y < srcBitmap.Height; y++)
            {
                // Get row position
                int srcRow = y * srcBitmap.Width;
                int dstRow = (sz.Height - 1 - borderSize - y) * sz.Width;

                // Write data for this row
                for (int x = 0; x < srcBitmap.Width; x++)
                {
                    int index = srcBitmap.Data[srcRow + x];
                    int i = dstRow + borderSize + x;
                    if (index == eyesEmissionIndex)
                        emissionColors[i] = eyeEmission;
                    else
                    {
                        // Feed albedoColors into emission to really pop lighter features like ribs and skulls
                        // Use otherEmission (with Color.black) for flatter more stealthy ghost
                        var albedo = albedoColors[i];
                        Color.RGBToHSV(albedo, out float H, out float S, out float V);
                        float emission = Mathf.Pow(V, 1.9f);
                        emissionColors[i] = Color.Lerp(otherEmission, albedo, emission);
                    }
                }
            }

            return emissionColors;
        }
        
        public JobHandle GetSpectralEmissionColors32Async(
            DFBitmap srcBitmap,
            Color32[] albedoColors,
            int borderSize,
            int eyesEmissionIndex,
            Color eyeEmission,
            Color otherEmission,
            out NativeArray<Color32> results,
            Allocator resultsAllocator = Allocator.TempJob,
            JobHandle dependency = default
        )
        {
            int
                srcBitmapWidth = srcBitmap.Width,
                srcBitmapHeight = srcBitmap.Height,
                szWidth = srcBitmapWidth + borderSize * 2,
                szHeight = srcBitmapHeight + borderSize * 2;

            results = new NativeArray<Color32>(szWidth * szHeight, resultsAllocator);

            var job = new GetSpectralEmissionColors32Job
            {
                EmissionColors = results,
                SrcBitmapData = srcBitmap.Data.AsNativeArray(out ulong gcHandleBitmap),
                AlbedoColors = albedoColors.AsNativeArray(out ulong gcHandleAlbedo),
                SrcBitmapWidth = srcBitmapWidth,
                SrcBitmapHeight = srcBitmapHeight,
                SzWidth = szWidth,
                SzHeight = szHeight,
                BorderSize = borderSize,
                EyesEmissionIndex = (byte)eyesEmissionIndex,
                EyeEmission = eyeEmission,
                OtherEmission = otherEmission,
            };
            var jobHandle = job.Schedule(srcBitmapHeight, JobUtility.OptimalLoopBatchCount(srcBitmapHeight), dependency);
            JobUtility.ReleaseGCObject(gcHandleBitmap, jobHandle);
            JobUtility.ReleaseGCObject(gcHandleAlbedo, jobHandle);

            return jobHandle;
        }
        [Unity.Burst.BurstCompile]
        struct GetSpectralEmissionColors32Job : IJobParallelFor
        {
            [WriteOnly][NativeDisableParallelForRestriction] public NativeArray<Color32> EmissionColors;
            [ReadOnly] public NativeArray<byte> SrcBitmapData;
            [ReadOnly] public NativeArray<Color32> AlbedoColors;
            public int
                SrcBitmapWidth,
                SrcBitmapHeight,
                SzWidth,
                SzHeight,
                BorderSize;
            public byte
                EyesEmissionIndex;
            public Color32
                EyeEmission,
                OtherEmission;
            void IJobParallelFor.Execute(int y)
            {
                // Generate emissive parts of texture based on index
                
                // Get row position
                int srcRow = y * SrcBitmapWidth;
                int dstRow = (SzHeight - 1 - BorderSize - y) * SzWidth;

                // Write data for this row
                for (int x = 0; x < SrcBitmapWidth; x++)
                {
                    int index = SrcBitmapData[srcRow + x];
                    int i = dstRow + BorderSize + x;
                    if (index == EyesEmissionIndex)
                        EmissionColors[i] = EyeEmission;
                    else
                    {
                        // Feed albedoColors into emission to really pop lighter features like ribs and skulls
                        // Use otherEmission (with Color.black) for flatter more stealthy ghost
                        Color32 albedo = AlbedoColors[i];
                        float V = GetBrightness(albedo);
                        float emission = math.pow(V, 1.9f);
                        // EmissionColors[i] = Color.Lerp(OtherEmission, albedo, math.saturate(emission));

                        int4 rgba = (int4)math.round(math.lerp(
                            new float4(OtherEmission.r, OtherEmission.g, OtherEmission.b, OtherEmission.a),
                            new float4(albedo.r, albedo.g, albedo.b, albedo.a),
                            math.saturate(emission)
                        ));
                        EmissionColors[i] = new Color32((byte)rgba.x, (byte)rgba.y, (byte)rgba.z, (byte)rgba.w);
                    }
                }
            }
        }

        public Color32[] GetFireWallColors32(ref Color32[] srcTexture, int width, int height, Color neutralColor, float scale)
        {
            Color32[] emissionColors = new Color32[width * height];

            for (int i = 0; i < emissionColors.Length; i++)
                emissionColors[i] = Color32.Lerp(neutralColor, srcTexture[i], scale);

            return emissionColors;
        }
        public JobHandle GetFireWallColors32Async(
            Color32[] srcTexture,
            Color neutralColor,
            float scale,
            out NativeArray<Color32> results,
            Allocator resultsAllocator = Allocator.TempJob,
            JobHandle dependency = default
        )
        {
            int length = srcTexture.Length;

            results = new NativeArray<Color32>(length, resultsAllocator);
            var job = new GetFireWallColors32Job
            {
                SrcTexture = srcTexture.AsNativeArray(out ulong gcHandle),
                EmissionColors = results,
                NeutralColor = neutralColor,
                Scale = scale,
            };
            var jobHandle = job.Schedule(length, JobUtility.OptimalLoopBatchCount(length), dependency);
            JobUtility.ReleaseGCObject(gcHandle,jobHandle);

            return jobHandle;
        }
        [Unity.Burst.BurstCompile]
        struct GetFireWallColors32Job : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Color32> SrcTexture;
            [WriteOnly] public NativeArray<Color32> EmissionColors;
            public Color32 NeutralColor;
            public float Scale;
            void IJobParallelFor.Execute(int index)
            {
                float4 a = new float4(NeutralColor.r, NeutralColor.g, NeutralColor.b, NeutralColor.a);
                var src = SrcTexture[index];
                float4 b = new float4(src.r, src.g, src.b, src.a);
                int4 c = (int4)math.lerp(a, b, Scale);
                EmissionColors[index] = new Color32((byte)c.x, (byte)c.y, (byte)c.z, (byte)c.w);
            }
        }

        public static float GetBrightness(Color32 color)
        {
            float3 rgb = new float3(color.r, color.g, color.b) * (1f / 255f);
            return math.max(math.max(rgb.z, rgb.y), rgb.x);
        }
        public static float GetBrightness(Color color)
        {
            return math.max(math.max(color.b, color.g), color.r);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Reads a standard IMG file header from the source stream into the desination header struct.
        ///  This header is found in multiple image files which is why it's implemented here in the base.
        /// </summary>
        /// <param name="reader">Source reader positioned at start of header data.</param>
        /// <param name="header">Destination header structure.</param>
        internal void ReadImgFileHeader(ref BinaryReader reader, ref ImgFileHeader header)
        {
            // Read IMG header data
            header.Position = reader.BaseStream.Position;
            header.XOffset = reader.ReadInt16();
            header.YOffset = reader.ReadInt16();
            header.Width = reader.ReadInt16();
            header.Height = reader.ReadInt16();
            header.Compression = (CompressionFormats)reader.ReadUInt16();
            header.PixelDataLength = reader.ReadUInt16();
            header.FrameCount = 1;
            header.DataPosition = reader.BaseStream.Position;
        }

        /// <summary>
        /// Reads RLE compressed data from source reader to destination writer.
        /// </summary>
        /// <param name="reader">Source reader positioned at start of input data.</param>
        /// <param name="length">Length of source data.</param>
        /// <param name="writer">Destination writer positioned at start of output data.</param>
        /// <returns>True if succeeded, otherwise false.</returns>
        internal void ReadRleData(ref BinaryReader reader, int length, ref BinaryWriter writer)
        {
            // Read image bytes
            byte pixel = 0;
            byte code = 0;
            int pos = 0;
            do
            {
                code = reader.ReadByte();
                if (code > 127)
                {
                    pixel = reader.ReadByte();
                    for (int i = 0; i < code - 127; i++)
                    {
                        writer.Write(pixel);
                        pos++;
                    }
                }
                else
                {
                    writer.Write(reader.ReadBytes(code + 1));
                    pos += (code + 1);
                }
            } while (pos < length);
        }

        #endregion
    }
}
