// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{

    /// <summary>
    /// Connects to DAGGER.SND to enumerate and extract sound data.
    /// </summary>
    public class SndFile
    {

        #region Class Variables

        public static int SampleRate = 11025;

        /// <summary>
        /// Auto-discard behaviour enabled or disabled.
        /// </summary>
        private bool autoDiscardValue = true;

        /// <summary>
        /// The last record opened. Used by auto-discard logic.
        /// </summary>
        private int lastSound = -1;

        /// <summary>
        /// The BsaFile representing DAGGER.SND.
        /// </summary>
        private BsaFile bsaFile = new BsaFile();

        /// <summary>
        /// Array of decomposed sound records.
        /// </summary>
        internal SoundRecord[] sounds;

        #endregion

        #region Class Structures

        /// <summary>
        /// Represents a single sound record.
        /// </summary>
        internal struct SoundRecord
        {
            public FileProxy MemoryFile;
            public DFSound DFSound;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// If true then decomposed sound records will be destroyed every time a different sound is fetched.
        ///  If false then decomposed sound records will be maintained until DiscardRecord() or DiscardAllRecords() is called.
        ///  Turning off auto-discard will speed up sound retrieval times at the expense of RAM. For best results, disable
        ///  auto-discard and impose your own caching scheme based on your application needs.
        /// </summary>
        public bool AutoDiscard
        {
            get { return autoDiscardValue; }
            set { autoDiscardValue = value; }
        }

        /// <summary>
        /// Number of BSA records in DAGGER.SND.
        /// </summary>
        public int Count
        {
            get { return bsaFile.Count; }
        }

        /// <summary>
        /// BSA file for sound effects.
        /// </summary>
        public BsaFile BsaFile
        {
            get { return bsaFile; }
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default DAGGER.SND filename.
        /// </summary>
        static public string Filename
        {
            get { return "DAGGER.SND"; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SndFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to DAGGER.SND.</param>
        /// <param name="usage">Determines if the BSA file will read from disk or memory.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public SndFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets index of sound record with specified id.
        /// </summary>
        /// <param name="id">ID of mesh.</param>
        /// <returns>Index of sound if located, or -1 if not found.</returns>
        public int GetRecordIndex(uint id)
        {
            // Otherwise find and store index by searching for id
            for (int i = 0; i < Count; i++)
            {
                if (bsaFile.GetRecordId(i) == id)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Load DAGGER.SND file.
        /// </summary>
        /// <param name="filePath">Absolute path to DAGGER.SND file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            filePath = filePath.ToUpper();
            if (!filePath.EndsWith("DAGGER.SND"))
                return false;

            // Load file
            if (!bsaFile.Load(filePath, usage, readOnly))
                return false;

            // Create records array
            sounds = new SoundRecord[bsaFile.Count];

            return true;
        }

        /// <summary>
        /// Get a sound from index.
        /// </summary>
        /// <param name="sound">SoundEffect.</param>
        /// <param name="soundOut">Sound data out.</param>
        /// <returns>True if successful.</returns>
        public bool GetSound(int sound, out DFSound soundOut)
        {
            soundOut = new DFSound();

            if (sound < 0 || sound >= bsaFile.Count)
                return false;

            // Just return sound if already loaded
            if (sounds[sound].DFSound.WaveHeader != null &&
                sounds[sound].DFSound.WaveData != null)
            {
                soundOut = sounds[sound].DFSound;
                return true;
            }

            // Discard previous sound
            if (AutoDiscard == true && lastSound != -1)
            {
                DiscardSound(lastSound);
            }

            // Load sound data
            sounds[sound].MemoryFile = bsaFile.GetRecordProxy(sound);
            if (sounds[sound].MemoryFile == null)
                return false;

            // Attempt to read sound
            ReadSound(sound);
            soundOut = sounds[sound].DFSound;

            return true;
        }

        /// <summary>
        /// Helper method to get an entire WAV file in a memory stream.
        /// </summary>
        /// <param name="sound">Sound index.</param>
        /// <returns>Wave file in MemoryStream.</returns>
        public MemoryStream GetStream(int sound)
        {
            // Get sound
            DFSound dfSound;
            GetSound(sound, out dfSound);
            if (dfSound.WaveHeader == null ||
                dfSound.WaveData == null)
            {
                return null;
            }

            // Create stream
            byte[] data = new byte[dfSound.WaveHeader.Length + dfSound.WaveData.Length];
            MemoryStream ms = new MemoryStream(data);

            // Write header and data
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(dfSound.WaveHeader);
            writer.Write(dfSound.WaveData);

            // Reset start position in stream
            ms.Position = 0;

            return ms;
        }

        /// <summary>
        /// Discard a sound from memory.
        /// </summary>
        /// <param name="sound">Index of sound to discard.</param>
        public void DiscardSound(int sound)
        {
            // Validate
            if (sound >= bsaFile.Count)
                return;

            // Discard memory files and other data
            if (sounds[sound].MemoryFile != null) sounds[sound].MemoryFile.Close();
            sounds[sound].MemoryFile = null;
            sounds[sound].DFSound = new DFSound();
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read a sound.
        /// </summary>
        /// <param name="sound">Sound index.</param>
        private bool ReadSound(int sound)
        {
            try
            {
                CreatePcmHeader(sound);
                ReadWaveData(sound);
            }
            catch (Exception e)
            {
                DiscardSound(sound);
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads wave data for the sound.
        /// </summary>
        /// <param name="sound">Sound index.</param>
        private void ReadWaveData(int sound)
        {
            // The entire BSA record is just raw sound bytes
            sounds[sound].DFSound.Name = sounds[sound].MemoryFile.FileName;
            sounds[sound].DFSound.WaveData = sounds[sound].MemoryFile.Buffer;
        }

        /// <summary>
        /// Creates a PCM header for the sound, including the DATA prefix preceding raw sound bytes.
        /// </summary>
        /// <param name="sound">Sound index.</param>
        private void CreatePcmHeader(int sound)
        {
            Int32 headerLength = 44;
            Int32 dataLength = sounds[sound].MemoryFile.Length;
            Int32 fileLength = dataLength + 36;

            String sRIFF = "RIFF";
            String sWAVE = "WAVE";
            String sFmtID = "fmt ";
            String sDataID = "data";
            Int32 nFmtLength = 16;
            Int16 nFmtFormat = 1;
            Int16 nFmtChannels = 1;
            Int32 nFmtSampleRate = 11025;
            Int32 nFmtAvgBytesPerSec = 11025;
            Int16 nFmtBlockAlign = 1;
            Int16 nFmtBitsPerSample = 8;

            // Create header bytes
            byte[] header = new byte[headerLength];

            // Create memory stream and writer
            MemoryStream ms = new MemoryStream(header);
            BinaryWriter writer = new BinaryWriter(ms);

            // Write the RIFF tag and file length
            writer.Write(sRIFF.ToCharArray());
            writer.Write((Int32)fileLength);

            // Write the WAVE tag and fmt header
            writer.Write(sWAVE.ToCharArray());
            writer.Write(sFmtID.ToCharArray());

            // Write fmt information
            writer.Write(nFmtLength);
            writer.Write(nFmtFormat);
            writer.Write(nFmtChannels);
            writer.Write(nFmtSampleRate);
            writer.Write(nFmtAvgBytesPerSec);
            writer.Write(nFmtBlockAlign);
            writer.Write(nFmtBitsPerSample);

            // Write PCM data prefix
            writer.Write(sDataID.ToCharArray());
            writer.Write(dataLength);

            // Close writer
            writer.Close();

            // Assign header to sound
            sounds[sound].DFSound.WaveHeader = header;
        }

        #endregion

    }

}
