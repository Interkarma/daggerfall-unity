// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (Lypyl@dfworkshop.net), Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.IO;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class FLCPlayer : Panel
    {
        FlcFile flcFile = new FlcFile();
        Texture2D flcTexture = null;

        float nextFrameTime;
        bool isPlaying = false;

        public bool Loop { get; set; }
        
        public FlcFile FLCFile { get { return flcFile; } }

        public FLCPlayer()
        {
            Loop = true;
            BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
            BackgroundColor = Color.black;
        }

        public void Load(string filename)
        {
            string path = Path.Combine(DaggerfallUnity.Instance.Arena2Path, filename);
            if (!flcFile.Load(path))
                return;

            flcTexture = TextureReader.CreateFromSolidColor(flcFile.Header.Width, flcFile.Header.Height, Color.black, false, false);
            flcTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
        }

        public void Start()
        {
            if (flcFile != null && flcFile.ReadyToPlay)
            {
                flcFile.CurrentFrame = 0;
                nextFrameTime = Time.realtimeSinceStartup + flcFile.FrameDelay;
                BackgroundTexture = flcTexture;
                isPlaying = true;
            }
        }

        public void Stop()
        {
            if (flcFile != null)
            {
                isPlaying = false;
                BackgroundTexture = null;
            }
        }

        public override void Update()
        {
            base.Update();

            // Stop playing if we reach final frame and not looping
            if (isPlaying && !Loop && flcFile.CurrentFrame >= flcFile.Header.NumOfFrames)
                isPlaying = false;

            if (flcFile == null || !flcTexture || !flcFile.ReadyToPlay || !isPlaying)
                return;

            if (Time.realtimeSinceStartup < nextFrameTime)
                return;
            else
                nextFrameTime = Time.realtimeSinceStartup + flcFile.FrameDelay;

            flcTexture.SetPixels32(flcFile.FrameBuffer);
            flcTexture.Apply(false);
            flcFile.BufferNextFrame();
        }
    }
}