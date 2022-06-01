// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (Lypyl@dfworkshop.net)
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to a *.FLC or *.CEL file to extract animation frames.
    /// </summary>
    public class FlcFile
    {
        #region Fields

        const int FRAMEHEADERSIZE = 16;
        const int CHUNKHEADERSIZE = 6;

        FLICHeader header = new FLICHeader();
        readonly FileProxy managedFile = new FileProxy();
        BinaryReader reader;

        #endregion

        #region Properties

        public FLICHeader Header
        {
            get { return header; }
        }

        public Color32[] FrameBuffer { get; private set; }
        public Color32[] Palette { get; private set; }
        public FrameHeader[] FrameHeaders { get; private set; }
        public int CurrentFrame { get; set; }
        public int ColorCount { get; private set; }
        public float FrameDelay { get; private set; }
        public bool FLC_HeaderSet { get; private set; }
        public bool ReadyToPlay { get; private set; }
        public FLIC_Format FlicType { get; private set; }
        public bool Transparency { get; set; }
        public byte TransparentRed { get; set; }
        public byte TransparentGreen { get; set; }
        public byte TransparentBlue { get; set; }

        #endregion

        #region Constructors

        public FlcFile()
        {
        }

        public FlcFile(string filePath)
        {
            Load(filePath);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a *.FLC file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.FLC file</param>
        /// <returns>True if file loaded successfully and is ready to use frame data.</returns>
        public bool Load(string filePath)
        {
            // Exit if this file already loaded
            if (managedFile.FilePath == filePath)
                return true;

            // Validate filename
            string fn = Path.GetFileName(filePath);
            if (!fn.EndsWith(".FLC", StringComparison.InvariantCultureIgnoreCase) &&
                !fn.EndsWith(".CEL", StringComparison.InvariantCultureIgnoreCase))
                return false;

            // Load file
            if (!managedFile.Load(filePath, FileUsage.UseMemory, true))
                return false;

            // Close existing reader
            if (reader != null)
                reader.Close();

            // Start new reader
            reader = managedFile.GetReader(0);

            return Read();
        }

        #endregion

        #region Private Methods

        // Read headers, create buffers
        bool Read()
        {
            // Read header
            ReadHeader();
            FLC_HeaderSet = true;
            FlicType = (FLIC_Format)header.FileID;
            ColorCount = (int)Math.Pow(2, header.PixelDepth);

            // Initialise buffers
            Palette = new Color32[ColorCount];
            FrameBuffer = new Color32[header.Width * header.Height];
            FrameHeaders = new FrameHeader[header.NumOfFrames + 1];

            // Read frame headers
            if (!ReadFrameHeaders())
            {
                Debug.LogWarning("Incompatible format, cannot read");
                ReadyToPlay = false;
                return false;
            }
            else
            {
                ReadyToPlay = true;
            }

            // Set frame delay
            FrameDelay = (float)(header.FrameDelay / (float)CinematicSpeed.FLIC);

            // Store frame 0 in frame buffer
            BufferNextFrame();

            return ReadyToPlay;
        }

        // Read & check all the frame headers
        bool ReadFrameHeaders()
        {
            if (!FLC_HeaderSet)
            {
                Debug.LogWarning("Main Header not read");
                return false;
            }

            for (int i = 0; i <= header.NumOfFrames; i++)
            {
                FrameHeader fh;
                if (!ReadFrameHeader(out fh))
                {
                    Debug.LogError("Invalid frame found, cannot play");
                    return false;
                }
                FrameHeaders[i] = fh;

                if (reader.BaseStream.Position + fh.size - FRAMEHEADERSIZE < reader.BaseStream.Length)
                    reader.BaseStream.Seek(fh.size - FRAMEHEADERSIZE, SeekOrigin.Current);
                else
                    break;
            }

            return true;
        }

        // Parse the Main flic header
        void ReadHeader()
        {
            header.FileSize = reader.ReadInt32();
            header.FileID = reader.ReadInt16();
            header.NumOfFrames = reader.ReadInt16();
            header.Width = reader.ReadInt16();
            header.Height = reader.ReadInt16();
            header.PixelDepth = reader.ReadInt16();
            header.Flags = reader.ReadInt16();
            header.FrameDelay = reader.ReadInt32();
            reader.BaseStream.Seek(2, SeekOrigin.Current);

            header.DateCreated = reader.ReadInt32();
            header.CreatorSN = reader.ReadInt32();
            header.LastUpdate = reader.ReadInt32();
            header.UpdaterSN = reader.ReadInt32();
            header.XAspect = reader.ReadInt16();
            header.YAspect = reader.ReadInt16();
            header.Ext_flags = reader.ReadInt16();
            header.KeyFrames = reader.ReadInt16();
            header.TotalFrames = reader.ReadInt16();
            header.ReqMemory = reader.ReadInt32();
            header.MaxRegions = reader.ReadInt16();
            header.TranspLevels = reader.ReadInt16();
            reader.BaseStream.Seek(24, SeekOrigin.Current);

            header.Frame1Offset = reader.ReadInt32();
            header.Frame2Offset = reader.ReadInt32();
            reader.BaseStream.Seek(40, SeekOrigin.Current);

            // If fli keep reading after header, if flc jump to frame0 offset
            if (header.FileID == (int)FLIC_Format.FLIC)
                reader.BaseStream.Seek(header.Frame1Offset, SeekOrigin.Begin);
        }

        // Reads next frame into buffer
        public bool BufferNextFrame(int frameNum = -1)
        {
            if (!ReadyToPlay)
            {
                Debug.Log("PlayNextFrame() found readyToPlay false; stopping");
                return false;
            }

            frameNum = (frameNum >= 0 && frameNum <= header.NumOfFrames) ? frameNum : CurrentFrame;
            FrameHeader fh = FrameHeaders[frameNum];
            reader.BaseStream.Seek(fh.posInFile + FRAMEHEADERSIZE, SeekOrigin.Begin);  // Go to frame start, skip over header

            for (int i = 0; i < fh.numSubChunks; i++)
            {
                ChunkHeader ch;
                if (!ReadChunkHeader(out ch))
                {
                    Debug.LogError("Invalid chunk type: " + ch.type);
                    return false;
                }
                else
                {
                    //Debug.Log("Chunk Type: " + ch.type.ToString());

                    switch (ch.type)    // Daggerfall .flc's only uses Color_256, Delta_FLC, Byte_Run, PSTAMP chunk types
                    {
                        case ChunkType.COLOR_256:
                            Decode_COLOR();
                            break;
                        case ChunkType.COLOR_64:
                            Decode_COLOR(true);
                            break;
                        case ChunkType.DELTA_FLC:
                            Decode_Delta_FLC();
                            break;
                        case ChunkType.DELTA_FLI:
                            Decode_Delta_FLI();
                            break;
                        case ChunkType.BYTE_RUN:
                            Decode_BYTE_RUN();
                            break;
                        case ChunkType.PSTAMP: // Skip over PSTAMP type - usually first chunk of first frame
                            reader.BaseStream.Seek(ch.size - CHUNKHEADERSIZE, SeekOrigin.Current);
                            break;
                        default: // Skip over unsupported chunk types
                            var skip = reader.BaseStream.Position + ch.size - CHUNKHEADERSIZE;
                            if (skip > header.FileSize)
                            {
                                Debug.LogError("Read error - tried to skip past the end of file");
                                ReadyToPlay = false;
                                return false;

                            }
                            else
                            {
                                reader.BaseStream.Seek(skip, SeekOrigin.Current);
                            }
                            break;
                    }

                }
            }

            // Use ++frameNum for next frame if not at last frame; or skip over frame 0
            CurrentFrame = (++frameNum <= header.NumOfFrames) ? frameNum : 1;

            return true;
        }

        // Read a frame header, returns false if it does't match a known type
        bool ReadFrameHeader(out FrameHeader fh)
        {
            fh = new FrameHeader();
            fh.posInFile = reader.BaseStream.Position;
            fh.size = reader.ReadInt32();
            fh.type = (ChunkType)reader.ReadInt16();
            fh.numSubChunks = reader.ReadInt16();
            reader.BaseStream.Seek(8, SeekOrigin.Current);
            //fh.reserved = reader.ReadBytes(8);

            if (!Enum.IsDefined(typeof(ChunkType), fh.type))
            {
                Debug.LogError("Invalid frame header type: " + fh.type);
                return false;
            }

            return true;
        }

        // Reads headers of sub-chunks that make up the frames
        bool ReadChunkHeader(out ChunkHeader ch)
        {
            ch = new ChunkHeader();
            ch.size = reader.ReadInt32();
            ch.type = (ChunkType)reader.ReadInt16();
            return Enum.IsDefined(typeof(ChunkType), ch.type);
        }

        // Decodes a BRUN type chunk and reads the data into the frame buffer
        bool Decode_COLOR(bool COLOR_64 = false)
        {
            int scale = 1;
            if (COLOR_64)
                scale = 4;

            int numOfPackets = reader.ReadInt16();
            int colorInd = 0;
            for (int i = 0; i < numOfPackets; i++)
            {
                reader.BaseStream.Seek(reader.ReadByte() * 3, SeekOrigin.Current);      // Color skip count
                int numOfColorsToChange = reader.ReadByte();

                if (numOfColorsToChange == 0)
                    numOfColorsToChange = 256;
                for (int cc = 0; cc < numOfColorsToChange; cc++)
                {

                    int r = reader.ReadByte() * scale;
                    int g = reader.ReadByte() * scale;
                    int b = reader.ReadByte() * scale;
                    if (Transparency && r == TransparentRed && g == TransparentGreen && b == TransparentBlue)
                        Palette[colorInd] = new Color32(0, 0, 0, 0);
                    else
                        Palette[colorInd] = new Color32((byte)r, (byte)g, (byte)b, 255);
                    colorInd++;

                }
            }

            return true;
        }

        // Decodes a BRUN type chunk and reads the data into the frame buffer
        void Decode_BYTE_RUN()
        {
            for (int y = 0; y < header.Height; y++)
            {
                // This value is only used in older FLI files
                /*int numOfPackets =*/reader.ReadByte();

                int x = 0;
                while (x < header.Width)
                {
                    int size_type = reader.ReadSByte();


                    if (size_type < 0)          // If the packet type is negative, it is a count of pixels to be copied from the packet to the animation image.
                    {
                        size_type = -size_type;
                        Screen_Copy_Seg(x, y, size_type);

                    }
                    else if (size_type > 0)     // If the packet type is positive, it contains a single pixel that is to be replicated
                    {                           // The absolute value of the packet type is the number of times the pixel is to be replicated.
                        Screen_Repeat_One(x, y, size_type);
                    }
                    x += size_type;
                }
            }
        }

        // Decodes a Delta FLC type chunk and reads the data into the frame buffer
        void Decode_Delta_FLC()
        {
            var lineCount = reader.ReadUInt16();
            var x = 0;
            var y = 0;
            uint packetCount = 0;

            for (int i = 0; i < lineCount; i++)
            {
                while (true)
                {
                    var opcode = reader.ReadInt16();
                    if ((opcode & (1 << 15)) > 0)
                    {
                        if ((opcode & (1 << 14)) > 0)   // Line skip
                            y += Math.Abs(opcode);
                        else                            // Last pixel
                            break;
                    }
                    else if ((opcode & (1 << 14)) == 0) // Packet count
                    {
                        packetCount = (uint)opcode;
                        break;
                    }
                }

                for (int n = 0; n < packetCount; n++)
                {

                    var colSkipCount = reader.ReadByte();
                    int size_type = reader.ReadSByte();
                    x += colSkipCount;

                    if (size_type > 0) // Copy segment
                        Screen_Copy_TwoSeg(x, y, size_type);

                    else if (size_type < 0) // Repeat pixel
                    {
                        size_type = -size_type;
                        Screen_Repeat_Two(x, y, size_type);
                    }

                    x += size_type * 2;
                }

                y++;
                x = 0;
            }
        }

        // Decodes a Delta FLI type chunk and reads the data into the frame buffer
        void Decode_Delta_FLI()
        {
            var firstLine = reader.ReadInt16(); // Contains pos of first line in chunk to start changing
            var numOfLines = reader.ReadInt16(); // Num of total lines in chunk

            for (int y = firstLine; y < numOfLines; y++)
            {
                var numOfPackets = reader.ReadByte();   // This is used unlike BRUN
                int x = 0;

                for (int packetCnt = 0; packetCnt < numOfPackets; packetCnt++)
                {
                    var colSkip = reader.ReadByte();    // Num of columns to skip
                    int size_type = reader.ReadSByte();
                    x += colSkip;

                    if (size_type > 0)          // This is reversed from byte run - pos == seg. copy, neg == pixel copy
                    {
                        Screen_Copy_Seg(x, y, size_type);
                    }
                    else if (size_type < 0)
                    {
                        size_type = -size_type;
                        Screen_Repeat_One(x, y, size_type);
                    }

                    x += size_type;
                }
            }
        }

        // Set all pixels in buffer to black
        /*void Decode_Black()
        {
            for (int i = 0; i < FrameBuffer.Length; i++)
            {
                FrameBuffer[i] = Color.black;
            }
        }*/

        // Reads count size segment into the buffer
        void Screen_Copy_Seg(int x, int y, int count)
        {
            //int pos = x + (y * header.Width);
            int pos = x + ((header.Height - 1) * header.Width) - (y * header.Width);

            for (int i = 0; i < count; i++)
            {
                var p = reader.ReadByte();
                Color32 color = Palette[p];
                FrameBuffer[pos] = color;
                pos++;
            }
        }

        // Repeats single pixel count times
        void Screen_Repeat_One(int x, int y, int count)
        {
            var p = reader.ReadByte();
            Color32 color = Palette[p];

            //int pos = x + (y * header.Width);
            int pos = x + ((header.Height - 1) * header.Width) - (y * header.Width);

            for (int i = 0; i < count; i++)
            {
                FrameBuffer[pos] = color;
                pos++;
            }
        }

        // Repeats two pixels count times
        void Screen_Repeat_Two(int x, int y, int count)
        {
            var p1 = reader.ReadByte();
            var p2 = reader.ReadByte();
            Color32 color1 = Palette[p1];
            Color32 color2 = Palette[p2];

            //int pos = x + (y * header.Width);
            int pos = x + ((header.Height - 1) * header.Width) - (y * header.Width);

            for (int i = 0; i < count; i++)
            {
                FrameBuffer[pos] = color1;
                FrameBuffer[pos + 1] = color2;
                pos += 2;
            }
        }

        // Reads count size segment into the buffer
        void Screen_Copy_TwoSeg(int x, int y, int count)
        {
            //int pos = x + (y * header.Width);
            int pos = x + ((header.Height - 1) * header.Width) - (y * header.Width);

            for (int i = 0; i < count; i++)
            {
                var p1 = reader.ReadByte();
                var p2 = reader.ReadByte();
                Color32 color1 = Palette[p1];
                Color32 color2 = Palette[p2];
                FrameBuffer[pos] = color1;
                FrameBuffer[pos + 1] = color2;
                pos += 2;
            }
        }

        #endregion

        #region Structures

        [Serializable]
        public struct FLICHeader
        {
            public int FileSize { get; internal set; }
            public int FileID { get; internal set; }
            public int NumOfFrames { get; internal set; } // Total # of animation frames; max 4000; doesn't include ring frame
            public int Width { get; internal set; } // Frame width
            public int Height { get; internal set; } // Frame height
            public int PixelDepth { get; internal set; }
            public int Flags { get; internal set; }// Always 3 or 0
            public int FrameDelay { get; internal set; } // Time in 1/1000 between frames (1/70 in older FLI type files)

            public int DateCreated { get; internal set; } // MS-DOS date stamp
            public int CreatorSN { get; internal set; } // SN or Compiler ID of program used to create animation
            public int LastUpdate { get; internal set; } // MS-DOS date stamp
            public int UpdaterSN { get; internal set; } // SN or Compiler ID of program used to create animation
            public int XAspect { get; internal set; }
            public int YAspect { get; internal set; }

            public int Ext_flags { get; internal set; }
            public int KeyFrames { get; internal set; }
            public int TotalFrames { get; internal set; }
            public int ReqMemory { get; internal set; }
            public int MaxRegions { get; internal set; }
            public int TranspLevels { get; internal set; }
            public int Frame1Offset { get; internal set; } // Offset to the first frame
            public int Frame2Offset { get; internal set; } // Offset to the second frame

        }

        [Serializable]
        public struct FrameHeader
        {
            public int size;
            public ChunkType type;
            public int numSubChunks;

            internal long posInFile;        // Pos in file stream
        }

        [Serializable]
        public struct ChunkHeader
        {
            public int size;
            public ChunkType type;
        }

        public enum FLIC_Format
        {
            FLI = -20719,
            FLIC = -20718
        }

        public enum CinematicSpeed
        {
            FLI = 70,
            FLIC = 1000
        }

        public enum ChunkType
        {
            None = 0,
            CEL_DATA = 3,
            COLOR_256 = 4,      //sub chunk
            DELTA_FLC = 7,      //sub chunk
            COLOR_64 = 11,      //sub chunk
            DELTA_FLI = 12,     //sub chunk
            BLACK = 13,         //sub chunk
            BYTE_RUN = 15,      //sub chunk
            FLI_COPY = 16,      //sub chunk 
            PSTAMP = 18,        //sub chunk - just a thumbnail, should be skipped
            DTA_BRUN = 25,
            DTA_COPY = 26,
            DTA_LC = 27,
            LABEL = 31,
            BMP_MASK = 32,
            MLEV_MASK = 33,
            SEGMENT = 34,
            KEY_IMAGE = 35,
            KEY_PAL = 36,
            REGION = 37,
            WAVE = 38,
            USERSTRING = 39,
            RGN_MASK = 40,
            LABELEX = 41,
            SHIFT = 42,
            PATHMAP = 43,

            PREFIX_TYPE = -3600,
            FRAME_TYPE = -3590,
        }

        #endregion
    }
}
