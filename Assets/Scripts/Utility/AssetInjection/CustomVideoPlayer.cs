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

using System.IO;
using System.Collections;
using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles playback of custom videos.
    /// </summary>
    public class CustomVideoPlayer : MonoBehaviour
    {
        MovieTexture movieTexture;

        public bool Playing { get; set; }
        
        private void Update()
        {
            // Get end of video
            if ((movieTexture.isReadyToPlay) && (!movieTexture.isPlaying))
                Playing = false;
        }

        /// <summary>
        /// Play custom video with specified name.
        /// </summary>
        /// <param name="name">Name of video.</param>
        public void PlayVideo (string name)
        {
            Playing = true;

            // Import video
            movieTexture = VideoReplacement.GetVideo(name);
            movieTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.VideoFilterMode;

            // Get audio
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = movieTexture.audioClip;

            // Play video and audio
            movieTexture.Play();
            audio.Play();
        }

        /// <summary>
        /// Stop playing video.
        /// </summary>
        public void StopVideo()
        {
            movieTexture.Stop();
            GetComponent<AudioSource>().Stop();
            Destroy(this);
        }
        
        /// <summary>
        /// Draw video on screen.
        /// </summary>
        void OnGUI()
        {
            // Place on top of Daggerfall Unity panels.
            GUI.depth = -1;

            // Draw MovieTexture on GUI.
            if (movieTexture.isPlaying)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movieTexture, ScaleMode.StretchToFill);
        } 
    }
}
