// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using UnityEngine.Video;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Draws the output of a VideoPlayer with the Daggerfall Unity UI.
    /// </summary>
    public class VideoPlayerDrawer : BaseScreenComponent
    {
        readonly VideoPlayer videoPlayer;

        bool isLoading;

        /// <summary>
        /// True if the video is playing or preparing to play.
        /// </summary>
        public bool IsPlaying
        {
            get { return videoPlayer.isPlaying || isLoading; }
        }

        /// <summary>
        /// Video scale mode; by default it follows game settings.
        /// </summary>
        public ScaleMode ScaleMode { get; set; }

        private VideoPlayerDrawer()
        {
            videoPlayer = DaggerfallUI.Instance.gameObject.GetComponent<VideoPlayer>();
            if (!videoPlayer)
            {
                videoPlayer = DaggerfallUI.Instance.gameObject.AddComponent<VideoPlayer>();
                videoPlayer.SetTargetAudioSource(0, DaggerfallUI.Instance.AudioSource);
                videoPlayer.renderMode = VideoRenderMode.APIOnly;
                videoPlayer.playOnAwake = false;
            }

            ScaleMode = DaggerfallUnity.Settings.RetroRenderingMode != 0 && DaggerfallUnity.Settings.RetroModeAspectCorrection != 0 ? ScaleMode.StretchToFill : ScaleMode.ScaleToFit;
        }

        /// <summary>
        /// A drawer for a clip with sound and video data.
        /// </summary>
        public VideoPlayerDrawer(VideoClip videoClip)
            : this()
        {
            videoPlayer.clip = videoClip;
        }

        /// <summary>
        /// A drawer for streamed sound and video data.
        /// </summary>
        public VideoPlayerDrawer(string url)
            : this()
        {
            videoPlayer.url = url;
        }

        public override void Draw()
        {
            if (!videoPlayer.isPlaying)
                return;

            base.Draw();

            DaggerfallUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), videoPlayer.texture, ScaleMode);
        }

        /// <summary>
        /// Loads and plays the video.
        /// </summary>
        public void Play()
        {
            isLoading = true;
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += (VideoPlayer videoPlayer) =>
            {
                isLoading = false;
                videoPlayer.Play();
            };
        }

        /// <summary>
        /// Stops the running video.
        /// </summary>
        public void Stop()
        {
            videoPlayer.Stop();
        }
    }
}
