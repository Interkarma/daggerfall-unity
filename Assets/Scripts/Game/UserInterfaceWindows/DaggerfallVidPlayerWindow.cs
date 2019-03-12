// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using UnityEngine.Video;
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
        VideoPlayerDrawer customVideo;
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

        public VideoPlayerDrawer CustomVideo
        {
            get { return customVideo; }
        }

        public bool UseCustomVideo
        {
            get { return useCustomVideo; }
        }

        public bool IsPlaying
        {
            get { return useCustomVideo ? customVideo.IsPlaying : video.Playing; }
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
            if (useCustomVideo = VideoReplacement.TryImportMovie(PlayOnStart, out customVideo))
            {
                // Play custom video
                customVideo.Size = new Vector2(Screen.width, Screen.height);
                NativePanel.Components.Add(customVideo);
                customVideo.Play();
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
                    RaiseOnVideoStartGlobalEvent();
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
                    !customVideo.IsPlaying)
                {
                    customVideo.Stop();
                    customVideo.Dispose();
                    customVideo = null;
                    RaiseOnVideoFinishedHandler();
                    RaiseOnVideoEndGlobalEvent();
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
                    RaiseOnVideoEndGlobalEvent();
                    CloseWindow();
                }
            }
        }

        #region Event Handlers

        // OnVideoStart (global)
        public delegate void OnVideoStartEventHandler();
        public static event OnVideoStartEventHandler OnVideoStart;
        protected virtual void RaiseOnVideoStartGlobalEvent()
        {
            if (OnVideoStart != null)
                OnVideoStart();
        }

        // OnVideoEnd (global)
        public delegate void OnVideoEndEventHandler();
        public static event OnVideoEndEventHandler OnVideoEnd;
        protected virtual void RaiseOnVideoEndGlobalEvent()
        {
            if (OnVideoEnd != null)
                OnVideoEnd();
        }

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