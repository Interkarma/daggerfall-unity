// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
        #region Fields & Properties

        readonly AudioSource audioSource;
        MovieTexture movieTexture;
        bool isLoading;

        public bool IsPlaying
        {
            get { return movieTexture.isPlaying || isLoading; }
        }

        #endregion

        #region Constructors

        public CustomVideoPlayer() : base()
        {
            audioSource = DaggerfallUI.Instance.gameObject.GetComponent<AudioSource>();
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            if (isLoading && movieTexture.isReadyToPlay)
            {
                movieTexture.Play();
                audioSource.clip = movieTexture.audioClip;
                audioSource.Play();
                isLoading = false;
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (movieTexture.isPlaying)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movieTexture, ScaleMode.StretchToFill);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set movietexture and play it when it's ready.
        /// </summary>
        /// <param name="movieTexture">Video and audio to play.</param>
        public void Play(MovieTexture movieTexture)
        {
            this.movieTexture = movieTexture;
            movieTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.VideoFilterMode;
            isLoading = true;
        }

        /// <summary>
        /// Stop playing current video.
        /// </summary>
        public void Stop()
        {
            movieTexture.Stop();
            audioSource.Stop();
        }

        #endregion
    }
}
