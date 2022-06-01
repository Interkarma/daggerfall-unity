// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

#region Using Statements
using UnityEngine;
using System;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Opens and reads a Daggerfall VID file.
    /// Unlike most API classes, this one uses the UnityEngine namespace to
    /// directly decode image data to a Color32[] frame buffer for efficiency.
    /// </summary>
    public class VidFile
    {
        const string VID = "VID";
        const int sampleRate = 11025;
        //const int delayMultiplier = 185;
        const int paletteDataLength = 768;
        const int paletteColorCount = 256;
        const int paletteMultiplier = 4;
        const float minFrameDelay = (740f / sampleRate);

        readonly FileProxy vidFile = new FileProxy();
        VidHeader header = new VidHeader();
        BinaryReader reader;
        Color32[] palette;
        Color32[] frameBuffer;
        byte[] audioBuffer;
        VidBlockTypes lastBlockType;
        int currentBlock;
        int lastDelay;
        double frameDelay;

        long streamPosition;
        bool endOfFileReached = false;

        public int FrameCount { get { return header.FrameCount; } }
        public int FrameWidth { get { return header.FrameWidth; } }
        public int FrameHeight { get { return header.FrameHeight; } }
        public int SampleRate { get { return sampleRate; } }
        public byte[] AudioBuffer { get { return audioBuffer; } }
        public Color32[] FrameBuffer { get { return frameBuffer; } }
        public double FrameDelay { get { return frameDelay; } }
        public int CurrentBlock { get { return currentBlock; } }
        public VidBlockTypes LastBlockType { get { return lastBlockType; } }
        public bool EndOfFile { get { return endOfFileReached; } }
        public int LastDelay { get { return lastDelay; } }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public VidFile()
        {
        }

        /// <summary>
        /// Open constructor.
        /// </summary>
        /// <param name="filename">Filname of VID to open.</param>
        public VidFile(string filename)
        {
            Open(filename);
        }

        /// <summary>
        /// Open a VID file.
        /// Always uses a memory stream for performance.
        /// </summary>
        /// <param name="filename">Filename of VID to open.</param>
        public bool Open(string filename)
        {
            // Open file proxy
            if (!vidFile.Load(filename, FileUsage.UseMemory, true))
            {
                endOfFileReached = true;
                return false;
            }

            // Close existing reader
            if (reader != null)
                reader.Close();

            // Read header and palette
            reader = vidFile.GetReader(0);
            ReadHeader();
            ReadPalette();

            // Cache reader position
            streamPosition = reader.BaseStream.Position;
            endOfFileReached = false;
            currentBlock = 0;

            return true;
        }

        /// <summary>
        /// Read next block from VID stream.
        /// Daggerfall's VID files are mostly interleaved audio and video streams.
        /// Occasionally there is a null block which must be rejected.
        /// </summary>
        public void ReadNextBlock()
        {
            ReadBlock();
        }

        int ReadBlock()
        {
            if (endOfFileReached)
                return 0;

            reader.BaseStream.Position = streamPosition;

            // Read next block type
            VidBlockTypes blockType = (VidBlockTypes)reader.ReadByte();
            lastBlockType = blockType;
            
            // Handle null block
            if (blockType == VidBlockTypes.Null)
            {
                //Debug.Log("Null block encountered.");
                streamPosition = reader.BaseStream.Position;
                return 0;
            }

            try
            {
                switch (blockType)
                {
                    case VidBlockTypes.Palette:
                        //Debug.Log("Skipping extra palette data");
                        streamPosition += paletteDataLength;
                        break;
                    case VidBlockTypes.Audio_StartFrame:
                        //Debug.Log("Reading Audio_StartFrame");
                        ReadAudioStartFrame();
                        break;
                    case VidBlockTypes.Audio_IncrementalFrame:
                        //Debug.Log("Reading Audio_IncrementalFrame");
                        ReadAudioIncrementalFrame();
                        break;
                    case VidBlockTypes.Video_StartFrame:
                        //Debug.Log("Reading Video_StartFrame");
                        ReadVideoStartFrame();
                        break;
                    case VidBlockTypes.Video_IncrementalFrame:
                        //Debug.Log("Reading Video_IncrementalFrame");
                        ReadVideoIncrementalFrame();
                        break;
                    case VidBlockTypes.Video_IncrementalRowOffsetFrame:
                        //Debug.Log("Reading Video_IncrementalRowOffsetFrame");
                        ReadVideoRowOffsetFrame();
                        break;
                    case VidBlockTypes.EndOfFile:
                        Debug.Log("End of VID file reached");
                        endOfFileReached = true;
                        break;
                    default:
                        throw new Exception("VidFile: Invalid/Unknown block type encountered.");
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return 0;
            }

            // Calculate delay time for this frame
            // Generally equivalent to (float)audioBuffer.Length / (float)sampleRate
            // Seems to differ mainly at beginning and end of video
            //frameDelay = ((float)(header.GlobalDelay + (float)lastDelay) * (float)delayMultiplier) / (float)sampleRate;
            frameDelay = (double)audioBuffer.Length / (double)sampleRate;

            // Daggerfall .VID files always have at least an audioBuffer.Length of 740 while in the middle of audio portions.
            // As the audio portion ends, this length can be much shorter, causing frameDelay to become very short
            // and making playback speed up. Enforcing a minimum based on a length of 740 to fix this.
            // This fixes the ends of ANIM0000.VID and ANIM0005.VID.
            if (frameDelay < minFrameDelay)
            {
                frameDelay = minFrameDelay;
            }

            long bytesRead = reader.BaseStream.Position - streamPosition;
            streamPosition = reader.BaseStream.Position;
            currentBlock++;

            return (int)bytesRead;
        }

        #region Readers

        void ReadHeader()
        {
            // Verify file starts with VID
            header.VID = FileProxy.ReadCString(reader, 3);
            if (header.VID != VID)
                throw new Exception("VidFile: Invalid VID header encountered.");

            // Read remaining header
            header.Unknown1 = reader.ReadUInt16();
            header.FrameCount = reader.ReadUInt16();
            header.FrameWidth = reader.ReadUInt16();
            header.FrameHeight = reader.ReadUInt16();
            header.GlobalDelay = reader.ReadUInt16();
            header.Unknown2 = reader.ReadUInt16();

            // Init frame buffer
            frameBuffer = new Color32[header.FrameWidth * header.FrameHeight];
        }

        void ReadPalette()
        {
            VidBlockTypes blockType = (VidBlockTypes)reader.ReadByte();
            if (blockType != VidBlockTypes.Palette)
                throw new Exception("VidFile: Palette block not encountered after header.");

            int pos = 0;
            byte[] data = reader.ReadBytes(paletteDataLength);
            palette = new Color32[paletteColorCount];
            for (int i = 0; i < paletteColorCount; i++)
            {
                palette[i] = new Color32(
                    (byte)(data[pos++] * paletteMultiplier),
                    (byte)(data[pos++] * paletteMultiplier),
                    (byte)(data[pos++] * paletteMultiplier),
                    255);
            }
        }

        VidAudioStartFrame ReadAudioStartFrame()
        {
            VidAudioStartFrame block = new VidAudioStartFrame();
            block.Unknown1 = reader.ReadUInt16();
            block.PlaybackRate = reader.ReadByte();
            block.DataLength = reader.ReadUInt16();
            audioBuffer = reader.ReadBytes(block.DataLength);

            if (block.PlaybackRate != 166)
                throw new Exception("VidFile: Unknown PlaybackRate encountered.");

            return block;
        }

        VidAudioIncrementalFrame ReadAudioIncrementalFrame()
        {
            VidAudioIncrementalFrame block = new VidAudioIncrementalFrame();
            block.DataLength = reader.ReadUInt16();
            audioBuffer = reader.ReadBytes(block.DataLength);

            return block;
        }

        void ReadVideoStartFrame()
        {
            lastDelay = (int)reader.ReadUInt16();

            ReadVideoFullFrameData();
        }

        void ReadVideoRowOffsetFrame()
        {
            lastDelay = (int)reader.ReadUInt16();
            UInt16 row = reader.ReadUInt16();

            ReadVideoPartialFrameData(row * FrameWidth);
        }

        void ReadVideoIncrementalFrame()
        {
            lastDelay = (int)reader.ReadUInt16();

            ReadVideoPartialFrameData(0);
        }

        void ReadVideoFullFrameData()
        {
            int pos = 0;
            int count = 0;
            while (pos < frameBuffer.Length)
            {
                byte input = reader.ReadByte();
                if (input >= 0x80)
                {
                    count = input - 0x80;
                    input = reader.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        frameBuffer[pos++] = palette[input];
                    }
                }
                else if (input == 0)
                {
                    break;
                }
                else
                {
                    count = input;
                    for (int i = 0; i < count; i++)
                    {
                        byte index = reader.ReadByte();
                        frameBuffer[pos++] = palette[index];
                    }
                }
            }
        }

        void ReadVideoPartialFrameData(int pos)
        {
            int count = 0;
            while (pos < frameBuffer.Length)
            {
                byte input = reader.ReadByte();
                if (input >= 0x80)
                {
                    count = input - 0x80;
                    pos += count;
                }
                else if (input == 0)
                {
                    break;
                }
                else
                {
                    count = input;
                    for (int i = 0; i < count; i++)
                    {
                        byte index = reader.ReadByte();
                        frameBuffer[pos++] = palette[index];
                    }
                }
            }
        }

        #endregion
    }

    #region Structs & Enums

    public enum VidBlockTypes
    {
        Null = 0,
        Video_IncrementalFrame = 1,
        Palette = 2,
        Video_StartFrame = 3,
        Video_IncrementalRowOffsetFrame = 4,
        EndOfFile = 20,
        Audio_StartFrame = 124,
        Audio_IncrementalFrame = 125,
    }

    public struct VidHeader
    {
        public String VID;                          // "VID"
        public UInt16 Unknown1;                     // Always 512
        public UInt16 FrameCount;                   // Total number of frames
        public UInt16 FrameWidth;                   // Width of each frame (256 or 320)
        public UInt16 FrameHeight;                  // Height of each frame (200)
        public UInt16 GlobalDelay;                  // Frame delay
        public UInt16 Unknown2;                     // Always 14
    }

    public struct VidAudioStartFrame
    {
        public UInt16 Unknown1;                     // Always 0
        public Byte PlaybackRate;                   // Unknown format. Value of 166 = 11025Hz
        public UInt16 DataLength;                   // Length of audio data.
    }

    public struct VidAudioIncrementalFrame
    {
        public UInt16 DataLength;                   // Length of audio data.
    }

    #endregion
}