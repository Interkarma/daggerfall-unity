// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    public enum TransportModes
    {
        Foot,
        Horse,
        Cart,
//        Ship  // (not a real player transport mode)
    }

    public class TransportManager : MonoBehaviour
    {
        public float RidingVolumeScale = 1.0f;  // TODO: Should this be the same setting as PlayerFootsteps.FootstepVolumeScale?

        public TransportModes TransportMode
        {
            get { return mode; }
            set { UpdateMode(value); }
        }

        /// <summary>True when player is on foot.</summary>
        public bool IsOnFoot
        {
            get { return mode == TransportModes.Foot; }
        }

        private TransportModes mode = TransportModes.Foot;

        DaggerfallAudioSource dfAudioSource;
        PlayerMotor playerMotor;
        AudioSource ridingAudioSource;

        Texture2D ridingTexure;
        AudioClip neighClip;
        float neighTime = 0;

        const SoundClips horseSound = SoundClips.AnimalHorse;
        const SoundClips horseRidingSound = SoundClips.HorseClop2;
        const SoundClips cartRidingSound = SoundClips.HorseAndCart;
        const string horseTextureName = "MRED00I0.IMG";
        const string cartTextureName = "MRED01I0.IMG";

        // TODO: Move into ImageHelper? (they're duplicated in FPSWeapon & DaggerfallVidPlayerWindow)
        const int nativeScreenWidth = 320;
        const int nativeScreenHeight = 200;

        // Use this for initialization
        void Start()
        {
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            playerMotor = GetComponent<PlayerMotor>();

            // Use custom audio source as we don't want to affect other sounds.
            ridingAudioSource = gameObject.AddComponent<AudioSource>();
            ridingAudioSource.hideFlags = HideFlags.HideInInspector;
            ridingAudioSource.playOnAwake = false;
            ridingAudioSource.loop = false;
            ridingAudioSource.dopplerLevel = 0f;
            ridingAudioSource.spatialBlend = 0f;
            ridingAudioSource.volume = RidingVolumeScale;

            neighClip = dfAudioSource.GetAudioClip((int) horseSound);
        }

        // Update is called once per frame
        void Update()
        {
            // Handle horse & cart riding sounds
            if (mode == TransportModes.Horse || mode == TransportModes.Cart)
            {
                if (playerMotor.IsStandingStill)
                {
                    ridingAudioSource.Stop();
                }
                else
                {
                    if (!ridingAudioSource.isPlaying) {
                        ridingAudioSource.Play();
                        Debug.Log("Transport manager: playing horse sound");
                     }
                }
                // Time for a whinney?
                if (neighTime < Time.time)
                {
                    dfAudioSource.AudioSource.PlayOneShot(neighClip, RidingVolumeScale);
                    neighTime = Time.time + Random.Range(2, 30);
                    Debug.Log("Transport manager: playing whinney. time next = " + neighTime);
                }
            }

        }

        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint))
            {

                if ((mode == TransportModes.Horse || mode == TransportModes.Cart) && ridingTexure != null)
                {
                    // Draw horse texture behind other HUD elements & weapons.
                    GUI.depth = 2;
                    // Get horse texture scaling factors.
                    float horseScaleX = (float) Screen.width / (float) nativeScreenWidth;
                    float horseScaleY = (float) Screen.height / (float) nativeScreenHeight;
                    // Calculate position for horse texture and draw it.
                    Rect pos = new Rect(
                                    Screen.width / 2f - (ridingTexure.width * horseScaleX) / 2f,
                                    Screen.height - (ridingTexure.height * horseScaleY),
                                    ridingTexure.width * horseScaleX,
                                    ridingTexure.height * horseScaleY);
                    GUI.DrawTexture(pos, ridingTexure);
                }
            }
        }

        private void UpdateMode(TransportModes transportMode)
        {
            // Update the transport mode and stop any riding sounds playing.
            mode = transportMode;
            if (ridingAudioSource.isPlaying)
                ridingAudioSource.Stop();

            if (mode == TransportModes.Horse || mode == TransportModes.Cart)
            {
                // Tell player motor we're riding.
                playerMotor.IsRiding = true;

                // Setup appropriate riding sounds.
                SoundClips sound = (mode == TransportModes.Horse) ? horseRidingSound : cartRidingSound;
                ridingAudioSource.clip = dfAudioSource.GetAudioClip((int) sound);

                // Setup appropriate riding textures.
                string textureName = (mode == TransportModes.Horse) ? horseTextureName : cartTextureName;
                ridingTexure = ImageReader.GetTexture(textureName, 0, 0, true);

                // Initialise neighing timer.
                neighTime = Time.time + Random.Range(1, 5);
            }
            else
            {
                // Tell player motor we're not riding.
                playerMotor.IsRiding = false;
            }
        }
    }
}
