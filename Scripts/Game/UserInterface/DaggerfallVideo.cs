﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Plays a Daggerfall VID file inside a control.
    /// </summary>
    public class DaggerfallVideo : BaseScreenComponent
    {
        const int clipQueueLength = 2;

        VidFile vidFile = new VidFile();
        Texture2D vidTexture;

        GameObject audioPlayer;
        AudioClip[] clips = new AudioClip[clipQueueLength];
        AudioSource[] audioSources = new AudioSource[clipQueueLength];
        int flip = 0;
        double nextEventTime;

        public bool Playing { get; set; }
        public VidFile VidFile { get { return vidFile; } }

        public DaggerfallVideo()
            : base()
        {
            // Setup audio player components
            audioPlayer = new GameObject("VIDAudioPlayer");
            audioPlayer.transform.parent = DaggerfallUI.Instance.gameObject.transform;
            for (int i = 0; i < clipQueueLength; i++)
            {
                audioSources[i] = audioPlayer.AddComponent<AudioSource>();
            }
            nextEventTime = AudioSettings.dspTime;

            // Init empty texture
            vidTexture = TextureReader.CreateFromSolidColor(1, 1, Color.black, false, false);
            vidTexture.wrapMode = TextureWrapMode.Clamp;
            vidTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.VideoFilterMode;
        }

        public void Open(string name)
        {
            string path = Path.Combine(DaggerfallUnity.Instance.Arena2Path, name);
            if (!vidFile.Open(path))
                return;

            vidTexture = TextureReader.CreateFromSolidColor(vidFile.FrameWidth, vidFile.FrameHeight, Color.black, false, false);
            vidTexture.wrapMode = TextureWrapMode.Clamp;
            vidTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.VideoFilterMode;
        }

        public void Play(string name)
        {
            Open(name);
            Playing = true;
        }

        public void Next()
        {
            if (!vidFile.EndOfFile && Playing)
            {
                double time = AudioSettings.dspTime;
                if (time + vidFile.FrameDelay >= nextEventTime)
                {
                    // Read next block or skip over null block
                    vidFile.ReadNextBlock();
                    if (vidFile.LastBlockType == VidBlockTypes.Null)
                        vidFile.ReadNextBlock();

                    if (vidFile.LastBlockType == VidBlockTypes.Audio_StartFrame ||
                        vidFile.LastBlockType == VidBlockTypes.Audio_IncrementalFrame)
                    {
                        // Add empty sample at front and end of clip to prevent clicks and pops
                        int srcLength = vidFile.AudioBuffer.Length;
                        int dstLength = srcLength + 2;
                        int pos = 1;

                        // Create audio clip for this block
                        AudioClip clip;
                        clip = AudioClip.Create(string.Empty, dstLength, 1, vidFile.SampleRate, false);

                        // Fill clip data
                        const float divisor = 1.0f / 128.0f;
                        float[] data = new float[dstLength];
                        for (int i = 0; i < srcLength; i++)
                        {
                            data[pos++] = (vidFile.AudioBuffer[i] - 128) * divisor;
                        }
                        clip.SetData(data, 0);
                        clips[flip] = clip;

                        // Schedule clip
                        audioSources[flip].clip = clips[flip];
                        audioSources[flip].PlayScheduled(nextEventTime);
                        nextEventTime += vidFile.FrameDelay;
                        flip = (clipQueueLength - 1) - flip;
                    }

                    if (vidFile.LastBlockType == VidBlockTypes.Video_StartFrame ||
                        vidFile.LastBlockType == VidBlockTypes.Video_IncrementalFrame ||
                        vidFile.LastBlockType == VidBlockTypes.Video_IncrementalRowOffsetFrame)
                    {
                        // Update video
                        vidTexture.SetPixels32(vidFile.FrameBuffer);
                        vidTexture.Apply(false);
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();

            Next();
        }

        public override void Draw()
        {
            base.Draw();

            if (!vidFile.EndOfFile)
            {
                // Daggerfall VIDs are designed to decode into a frame buffer with 0,0 at top-left.
                // Unity textures have 0,0 at bottom-left.
                // Rather than change decoders (and potentially slow them down) just for Unity,
                // the image is simply drawn upside down by inverting rect Y.
                Rect rect = Rectangle;
                float temp = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = temp;
                GUI.DrawTexture(rect, vidTexture, ScaleMode.StretchToFill);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            GameObject.Destroy(audioPlayer);
        }
    }
}