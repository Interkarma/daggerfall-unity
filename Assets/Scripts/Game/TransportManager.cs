// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Banking;

namespace DaggerfallWorkshop.Game
{
    public enum TransportModes
    {
        Foot,
        Horse,
        Cart,
        Ship  // (not a real player transport mode)
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

        /// <summary>True when there's a recorded position before boarding and player is on the ship</summary>
        public bool IsOnShip()
        {
            StreamingWorld world = GameManager.Instance.StreamingWorld;
            DFPosition shipCoords = DaggerfallBankManager.GetShipCoords();
            return boardShipPosition != null && world.MapPixelX == shipCoords.X && world.MapPixelY == shipCoords.Y;
        }

        public PlayerPositionData_v1 BoardShipPosition
        {
            get { return boardShipPosition; }
            set { boardShipPosition = value; }
        }

        private TransportModes mode = TransportModes.Foot;
        private PlayerPositionData_v1 boardShipPosition;    // Holds the player position from before boarding a ship.


        const string horseTextureName = "MRED00I0.CFA";
        const string cartTextureName = "MRED01I0.CFA";
        const float animFrameTime = 0.125f;  // Time between animation frames in seconds.

        const SoundClips horseSound = SoundClips.AnimalHorse;
        const SoundClips horseRidingSound1 = SoundClips.HorseClop;
        const SoundClips horseRidingSound2 = SoundClips.HorseClop2;
        const SoundClips cartRidingSound = SoundClips.HorseAndCart;

        // TODO: Move into ImageHelper? (duplicated in FPSWeapon & DaggerfallVidPlayerWindow)
        const int nativeScreenHeight = 200;

        PlayerMotor playerMotor;
        DaggerfallAudioSource dfAudioSource;
        AudioSource ridingAudioSource;

        ImageData ridingTexure;
        ImageData[] ridingTexures = new ImageData[4];
        float lastFrameTime = 0;
        int frameIdx = 0;

        AudioClip neighClip;
        float neighTime = 0;
        bool wasMovingLessThanHalfSpeed = true;

        // Use this for initialization
        void Start()
        {
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            playerMotor = GetComponent<PlayerMotor>();

            // Use custom audio source as we don't want to affect other sounds while riding.
            ridingAudioSource = gameObject.AddComponent<AudioSource>();
            ridingAudioSource.hideFlags = HideFlags.HideInInspector;
            ridingAudioSource.playOnAwake = false;
            ridingAudioSource.loop = false;
            ridingAudioSource.dopplerLevel = 0f;
            ridingAudioSource.spatialBlend = 0f;
            ridingAudioSource.volume = RidingVolumeScale * DaggerfallUnity.Settings.SoundVolume;

            neighClip = dfAudioSource.GetAudioClip((int) horseSound);

            // Init event listener for transitions.
            PlayerEnterExit.OnPreTransition += new PlayerEnterExit.OnPreTransitionEventHandler(HandleTransition);
        }

        // Handle interior/exterior transition events by setting transport mode to Foot.
        void HandleTransition(PlayerEnterExit.TransitionEventArgs args)
        {
            if (args.TransitionType == PlayerEnterExit.TransitionType.ToBuildingInterior ||
                args.TransitionType == PlayerEnterExit.TransitionType.ToDungeonInterior)
            {
                UpdateMode(TransportModes.Foot);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Handle horse & cart riding animation & sounds.
            if (mode == TransportModes.Horse || mode == TransportModes.Cart)
            {
                // refresh audio volume to reflect global changes
                ridingAudioSource.volume = RidingVolumeScale * DaggerfallUnity.Settings.SoundVolume;
                if (playerMotor.IsStandingStill || !playerMotor.IsGrounded || GameManager.IsGamePaused)
                {   // Stop animation frames and sound playing.
                    lastFrameTime = 0;
                    ridingTexure = ridingTexures[0];
                    ridingAudioSource.Stop();
                }
                else
                {   // Update Animation frame?
                    if (lastFrameTime == 0)
                    {
                        lastFrameTime = Time.time;
                    }
                    else if (Time.time > lastFrameTime + animFrameTime)
                    {
                        lastFrameTime = Time.time;
                        frameIdx = (frameIdx == 3) ? 0 : frameIdx + 1;
                        ridingTexure = ridingTexures[frameIdx];
                    }
                    // Get appropriate hoof sound for horse
                    if (mode == TransportModes.Horse)
                    {
                        if (!wasMovingLessThanHalfSpeed && playerMotor.IsMovingLessThanHalfSpeed)
                        {
                            wasMovingLessThanHalfSpeed = true;
                            ridingAudioSource.clip = dfAudioSource.GetAudioClip((int)horseRidingSound1);
                        }
                        else if (wasMovingLessThanHalfSpeed && !playerMotor.IsMovingLessThanHalfSpeed)
                        {
                            wasMovingLessThanHalfSpeed = false;
                            ridingAudioSource.clip = dfAudioSource.GetAudioClip((int)horseRidingSound2);
                        }
                    }

                    if (!ridingAudioSource.isPlaying)
                    {
                        ridingAudioSource.volume = RidingVolumeScale * DaggerfallUnity.Settings.SoundVolume;
                        ridingAudioSource.Play();
                    }
                }
                // Time for a whinney?
                if (neighTime < Time.time)
                {
                    dfAudioSource.AudioSource.PlayOneShot(neighClip, RidingVolumeScale * DaggerfallUnity.Settings.SoundVolume);
                    neighTime = Time.time + Random.Range(2, 40);
                }
            }
        }

        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint) && !GameManager.IsGamePaused)
            {
                if ((mode == TransportModes.Horse || mode == TransportModes.Cart) && ridingTexure.texture != null)
                {
                    // Draw horse texture behind other HUD elements & weapons.
                    GUI.depth = 2;
                    // Get horse texture scaling factor. (only use height to avoid aspect ratio issues like fat horses)
                    float horseScaleY = (float) Screen.height / (float) nativeScreenHeight;
                    // Calculate position for horse texture and draw it.
                    Rect pos = new Rect(
                                    Screen.width / 2f - (ridingTexure.width * horseScaleY) / 2f,
                                    Screen.height - (ridingTexure.height * horseScaleY),
                                    ridingTexure.width * horseScaleY,
                                    ridingTexure.height * horseScaleY);
                    GUI.DrawTexture(pos, ridingTexure.texture);
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
                // Tell player motor we're riding. TODO: Change to event system so other classes can listen for transport changes.
                playerMotor.IsRiding = true;

                // Setup appropriate riding sounds.
                SoundClips sound = (mode == TransportModes.Horse) ? horseRidingSound2 : cartRidingSound;
                ridingAudioSource.clip = dfAudioSource.GetAudioClip((int) sound);

                // Setup appropriate riding textures.
                string textureName = (mode == TransportModes.Horse) ? horseTextureName : cartTextureName;
                for (int i = 0; i < 4; i++)
                    ridingTexures[i] = ImageReader.GetImageData(textureName, 0, i, true, true);
                ridingTexure = ridingTexures[0];

                // Initialise neighing timer.
                neighTime = Time.time + Random.Range(1, 5);
            }
            else
            {
                // Tell player motor we're not riding.
                playerMotor.IsRiding = false;
            }

            if (mode == TransportModes.Ship)
            {
                GameManager.Instance.PlayerMotor.CancelMovement = true;
                SerializablePlayer serializablePlayer = GetComponent<SerializablePlayer>();
                DaggerfallUI.Instance.FadeBehaviour.SmashHUDToBlack();
                StreamingWorld world = GameManager.Instance.StreamingWorld;
                DFPosition shipCoords = DaggerfallBankManager.GetShipCoords();

                // Is there recorded position before boarding and is player on the ship?
                if (IsOnShip())
                {
                    // Check for terrain sampler changes. (so don't fall through floor)
                    StreamingWorld.RepositionMethods reposition = StreamingWorld.RepositionMethods.None;
                    if (boardShipPosition.terrainSamplerName != DaggerfallUnity.Instance.TerrainSampler.ToString() ||
                        boardShipPosition.terrainSamplerVersion != DaggerfallUnity.Instance.TerrainSampler.Version)
                    {
                        reposition = StreamingWorld.RepositionMethods.RandomStartMarker;
                        if (DaggerfallUI.Instance.DaggerfallHUD != null)
                            DaggerfallUI.Instance.DaggerfallHUD.PopupText.AddText("Terrain sampler changed. Repositioning player.");
                    }
                    // Restore player position from before boarding ship, caching ship scene first.
                    SaveLoadManager.CacheScene(world.SceneName);    // TODO: Should this should move into teleport to support other teleports? Issue only if inside. (e.g. recall)
                    DFPosition mapPixel = MapsFile.WorldCoordToMapPixel(boardShipPosition.worldPosX, boardShipPosition.worldPosZ);
                    world.TeleportToCoordinates(mapPixel.X, mapPixel.Y, reposition);
                    serializablePlayer.RestorePosition(boardShipPosition);
                    boardShipPosition = null;
                    // Restore cached scene (ship is special case, cache will not be cleared)
                    SaveLoadManager.RestoreCachedScene(world.SceneName);
                }
                else
                {
                    // Record current player position before boarding ship, and cache scene. (ship is special case, cache will not be cleared)
                    boardShipPosition = serializablePlayer.GetPlayerPositionData();
                    SaveLoadManager.CacheScene(world.SceneName);
                    // Teleport to the players ship, restoring cached scene.
                    world.TeleportToCoordinates(shipCoords.X, shipCoords.Y, StreamingWorld.RepositionMethods.RandomStartMarker);
                    SaveLoadManager.RestoreCachedScene(world.SceneName);
                }
                DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack();
                mode = TransportModes.Foot;
            }
        }
    }
}
