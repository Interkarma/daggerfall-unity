// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using System.Collections;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements a VID player.
    /// </summary>
    public class DaggerfallVidPlayerWindow : DaggerfallBaseWindow
    {
        DaggerfallVideo video;
        CustomVideoPlayer customVideo;
        bool useCustomVideo = false;

        bool hideCursor = true;
        bool endOnAnyKey = true;

        public string PlayOnStart { get; set; }

        public bool HideCursor
        {
            get { return hideCursor; }
            set { hideCursor = value; }
        }

        public DaggerfallVideo Video
        {
            get { return video; }
        }

        public CustomVideoPlayer CustomVideo
        {
            get { return customVideo; }
        }

        public bool UseCustomVideo
        {
            get { return useCustomVideo; }
        }

        public DaggerfallVidPlayerWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        public DaggerfallVidPlayerWindow(IUserInterfaceManager uiManager, string playOnStart)
            : base(uiManager)
        {
            PlayOnStart = playOnStart;
        }

        protected override void Setup()
        {
            MovieTexture customMovieTexture;
            if (VideoReplacement.ImportCustomVideo(PlayOnStart, out customMovieTexture))
            {
                // Play custom video
                customVideo = DaggerfallUI.Instance.gameObject.AddComponent<CustomVideoPlayer>();
                customVideo.PlayVideo(customMovieTexture);
                useCustomVideo = true;
            }
            else
            {
                const int nativeScreenWidth = 320;
                const int nativeScreenHeight = 200;

                // Add video player control
                video = new DaggerfallVideo();
                video.HorizontalAlignment = HorizontalAlignment.Center;
                video.Size = new Vector2(nativeScreenWidth, nativeScreenHeight);
                NativePanel.Components.Add(video);

                // Start playing
                if (!string.IsNullOrEmpty(PlayOnStart))
                {
                    video.Open(PlayOnStart);
                    video.Playing = true;
                    Cursor.visible = false;
                }
            }
        }

        public override void Update()
        {
            base.Update();

            // Handle exit any key or end of video
            if (useCustomVideo)
            {
                if (endOnAnyKey && Input.anyKeyDown || 
                    !customVideo.Playing )
                {
                    customVideo.StopVideo();
                    RaiseOnVideoFinishedHandler();
                    CloseWindow();
                }
            }
            else
            {
                if (endOnAnyKey && Input.anyKeyDown ||
                video.VidFile.EndOfFile && video.Playing)
                {
                    video.Playing = false;
                    video.Dispose();
                    RaiseOnVideoFinishedHandler();
                    CloseWindow();
                }
            }
        }

        #region Event Handlers

        // OnVideoFinished
        public delegate void OnVideoFinishedHandler();
        public event OnVideoFinishedHandler OnVideoFinished;
        protected virtual void RaiseOnVideoFinishedHandler()
        {
            if (OnVideoFinished != null)
                OnVideoFinished();
        }

        #endregion
    }
}