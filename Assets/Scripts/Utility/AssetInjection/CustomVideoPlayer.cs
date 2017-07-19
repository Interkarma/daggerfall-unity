// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles playback of custom videos.
    /// </summary>
    public class CustomVideoPlayer : BaseScreenComponent
    {
        MovieTexture movieTexture;
        readonly AudioSource audio;

        public bool Playing { get; set; }

        public CustomVideoPlayer() : base()
        {
            this.audio = DaggerfallUI.Instance.gameObject.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Play specified video.
        /// </summary>
        /// <param name="movieTexture">Video.</param>
        public void PlayVideo (MovieTexture video)
        {
            Playing = true;

            // Set filtermode
            movieTexture = video;
            movieTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.VideoFilterMode;

            // Get audio
            audio.clip = movieTexture.audioClip;

            // Play video and audio
            movieTexture.Play();
            audio.Play();
        }

        public override void Update()
        {
            base.Update();

            // Get end of video
            if ((movieTexture.isReadyToPlay) && (!movieTexture.isPlaying))
                Playing = false;
        }

        /// <summary>
        /// Draw video on screen.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            if (movieTexture.isPlaying)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movieTexture, ScaleMode.StretchToFill);
        }

        /// <summary>
        /// Stop playing video.
        /// </summary>
        public void StopVideo()
        {
            movieTexture.Stop();
            audio.Stop();
            Dispose();
        }
    }
}
