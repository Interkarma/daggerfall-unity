// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Formulas;
using Random = UnityEngine.Random;
using Wenzil.Console;
using System.Globalization;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Renders first-person weapons and attack animations.
    /// Recommended for this component to be on its own game object.
    /// Cam modify pitch of audio source for different weapon speed effects.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class FPSWeapon : MonoBehaviour
    {
        public bool ShowWeapon = true;
        public float AttackSpeed;
        public bool FlipHorizontal = false;
        public WeaponTypes WeaponType = WeaponTypes.None;
        public MetalTypes MetalType = MetalTypes.None;
        public ItemHands WeaponHands = ItemHands.None;
        public float Reach = 2.5f;
        public float AttackSpeedScale = 1.0f;
        public float Cooldown = 0.0f;
        public float time;
        public float speed;
        public float offsetposition;
        public SoundClips DrawWeaponSound = SoundClips.DrawWeapon;
        public SoundClips SwingWeaponSound = SoundClips.SwingMediumPitch;

        WeaponTypes currentWeaponType;
        MetalTypes currentMetalType;

        const int nativeScreenWidth = 320;
        const int nativeScreenHeight = 200;

        readonly byte[] leftUnarmedAnims = { 0, 1, 2, 3, 4, 2, 1, 0 };
        int leftUnarmedAnimIndex = 0;

        DaggerfallUnity dfUnity;
        CifRciFile cifFile;
        Texture2D weaponAtlas;
        Rect[] weaponRects;
        RecordIndex[] weaponIndices;
        Rect weaponPosition;
        float weaponScaleX;
        float weaponScaleY;

        DaggerfallAudioSource dfAudioSource;
        WeaponAnimation[] weaponAnims;
        WeaponStates weaponState = WeaponStates.Idle;
        int currentFrame = 0;
        int animTicks = 0;
        public float animTickTime;
        Rect curAnimRect;

        //*COMBAT OVERHAUL ADDITION*//
        //added for combat overhaul mod. Used to calculate and run animation offsetting routines.
        //Also used for debug messages in debug routine.
        float avgFrameRate;
        float lerpRange;
        float maxframeseconds;
        int totalOffsets;
        float timePass;
        float percentagetime;
        string action;
        float bob = 0;
        float posi = 0;
        bool bobSwitch;

        PlayerEntity playerEntity;

        readonly Dictionary<int, Texture2D> customTextures = new Dictionary<int, Texture2D>();
        Texture2D curCustomTexture;

        #region Properties

        public WeaponStates WeaponState { get { return weaponState; } }

        #endregion

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            StartCoroutine(AnimateWeapon());

            //*COMBAT OVERHAUL ADDITION*//
            //starts a debug routine for showing var readouts in a loop
            //adds console commands using the FPSConsoleCommand script. Both are important for debugging.
            StartCoroutine(DebugDisplay());
            FPSConsoleCommands.RegisterCommands();
        }

        void OnGUI()
        {
            GUI.depth = 1;

            //WeaponType = WeaponTypes.Werecreature;

            // Must be ready and not loading the game
            if (!ReadyCheck() || WeaponType == WeaponTypes.None || GameManager.IsGamePaused || SaveLoadManager.Instance.LoadInProgress)
                return;

            // Must have current weapon texture atlas
            if (weaponAtlas == null || WeaponType != currentWeaponType || MetalType != currentMetalType)
            {
                LoadWeaponAtlas();

                if (weaponAtlas == null)
                    return;
                //checks if using bow and updates sprite here to ensure proper bow animation
                //added the if check for combat overhaul.
                if (WeaponType == WeaponTypes.Bow)
                    UpdateWeapon();
            }

            //added if check for any non-bow weapon. This allows the sprite to continually update screen position.            
            if (WeaponType != WeaponTypes.Bow)
                UpdateWeapon();

            if (Event.current.type.Equals(EventType.Repaint) && ShowWeapon)
            {
                // Draw weapon texture behind other HUD elements
                GUI.DrawTextureWithTexCoords(weaponPosition, curCustomTexture ? curCustomTexture : weaponAtlas, curAnimRect);
            }
        }

        public void OnAttackDirection(WeaponManager.MouseDirections direction)
        {
            // Get state based on attack direction
            WeaponStates state;

            switch (direction)
            {
                case WeaponManager.MouseDirections.Down:
                    state = WeaponStates.StrikeDown;
                    break;
                case WeaponManager.MouseDirections.DownLeft:
                    state = WeaponStates.StrikeLeft;
                    break;
                case WeaponManager.MouseDirections.Left:
                    state = WeaponStates.StrikeLeft;
                    break;
                case WeaponManager.MouseDirections.Right:
                    state = WeaponStates.StrikeRight;
                    break;
                case WeaponManager.MouseDirections.DownRight:
                    state = WeaponStates.StrikeRight;
                    break;
                case WeaponManager.MouseDirections.Up:
                    state = WeaponStates.StrikeUp;
                    break;
                default:
                    return;
            }

            // Do not change if already playing attack animation, unless releasing an arrow (bow & state=up->down)
            if (!IsPlayingOneShot() || (WeaponType == WeaponTypes.Bow && weaponState == WeaponStates.StrikeUp && state == WeaponStates.StrikeDown))
                ChangeWeaponState(state);
        }

        //*COMBAT OVERHAUL ADDITION*//
        //switch used to set custom offset distances for each weapon.
        //because each weapon has its own sprites, each one needs slight
        //adjustments to ensure sprites seem as seemless as possible in transition.
        public float GetAnimationOffset()
        {
            if (FPSConsoleCommands.OffsetDistance.offsetDistance == 0)
            {
                WeaponTypes weapon = currentWeaponType;
                switch (weapon)
                {
                    case WeaponTypes.Battleaxe:
                        return .2f;
                    case WeaponTypes.LongBlade:
                        return .252f;
                    case WeaponTypes.Warhammer:
                        return .28f;
                    case WeaponTypes.Werecreature:
                        return .085f;
                    case WeaponTypes.Melee:
                        return .14f;
                    default:
                        return .235f;
                }
            }
            else
            {
                return FPSConsoleCommands.OffsetDistance.offsetDistance;
            }


        }

        public void ChangeWeaponState(WeaponStates state)
        {
            weaponState = state;

            // Only reset frame to 0 for bows if idle state
            if (WeaponType != WeaponTypes.Bow || state == WeaponStates.Idle)
                currentFrame = animTicks = 0;

            UpdateWeapon();
        }

        public bool IsAttacking()
        {
            return IsPlayingOneShot();
        }

        public int GetHitFrame()
        {
            if (WeaponType == WeaponTypes.Bow)
                return 5;
            else
                return 2;
        }

        public int GetCurrentFrame()
        {
            return currentFrame;
        }

        public int SetCurrentFrame(int frame)
        {
            currentFrame = frame;
            return currentFrame;
        }

        public float GetAnimTime()
        {
            return animTicks * animTickTime;
        }

        public void PlayActivateSound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.AudioSource.pitch = 1f;// *AttackSpeedScale;
                dfAudioSource.PlayOneShot(DrawWeaponSound, 0);
            }
        }

        public void PlaySwingSound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.AudioSource.pitch = 1f * AttackSpeedScale;
                dfAudioSource.PlayOneShot(SwingWeaponSound, 0, 1.1f);
            }
        }

        public void PlayAttackVoice(SoundClips customSound = SoundClips.None)
        {
            if (dfAudioSource)
            {
                if (customSound == SoundClips.None)
                {
                    PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                    SoundClips sound = DaggerfallEntity.GetRaceGenderAttackSound(playerEntity.Race, playerEntity.Gender, true);
                    float pitch = dfAudioSource.AudioSource.pitch;
                    dfAudioSource.AudioSource.pitch = pitch + UnityEngine.Random.Range(0, 0.3f);
                    dfAudioSource.PlayOneShot(sound, 0, 1f);
                    dfAudioSource.AudioSource.pitch = pitch;
                }
                else
                {
                    dfAudioSource.PlayOneShot(customSound, 0, 1f);
                }
            }
        }

        #region Private Methods

        private bool IsPlayingOneShot()
        {
            return (weaponState != WeaponStates.Idle);
        }

        private void UpdateWeapon()
        {
            // Do nothing if weapon not ready
            if (weaponAtlas == null || weaponAnims == null ||
                weaponRects == null || weaponIndices == null)
            {
                return;
            }

            // Reset state if weapon not visible
            if (!ShowWeapon || WeaponType == WeaponTypes.None)
            {
                weaponState = WeaponStates.Idle;
                currentFrame = 0;
            }

            // Handle bow with no arrows
            if (!GameManager.Instance.WeaponManager.Sheathed && WeaponType == WeaponTypes.Bow && GameManager.Instance.PlayerEntity.Items.GetItem(Items.ItemGroups.Weapons, (int)Items.Weapons.Arrow) == null)
            {
                GameManager.Instance.WeaponManager.SheathWeapons();
                DaggerfallUI.SetMidScreenText(UserInterfaceWindows.HardStrings.youHaveNoArrows);
            }

            // Store rect and anim
            int weaponAnimRecordIndex;
            if (WeaponType == WeaponTypes.Bow)
                weaponAnimRecordIndex = 0; // Bow has only 1 animation
            else
                weaponAnimRecordIndex = weaponAnims[(int)weaponState].Record;
            WeaponAnimation anim = weaponAnims[(int)weaponState];
            try
            {
                bool isImported = customTextures.TryGetValue(MaterialReader.MakeTextureKey(0, (byte)weaponAnimRecordIndex, (byte)currentFrame), out curCustomTexture);
                if (FlipHorizontal && (weaponState == WeaponStates.Idle || weaponState == WeaponStates.StrikeDown || weaponState == WeaponStates.StrikeUp))
                {
                    // Mirror weapon rect
                    if (isImported)
                    {
                        curAnimRect = new Rect(1, 0, -1, 1);
                    }
                    else
                    {
                        Rect rect = weaponRects[weaponIndices[weaponAnimRecordIndex].startIndex + currentFrame];
                        curAnimRect = new Rect(rect.xMax, rect.yMin, -rect.width, rect.height);
                    }
                }
                else
                {
                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[weaponAnimRecordIndex].startIndex + currentFrame];

                    //*COMBAT OVERHAUL ADDITION*//
                    //added offset checks for individual attacks and weapons. Also, allows for the weapon bobbing effect.
                    //helps smooth out some animaitions by swapping out certain weapon animation attack frames and repositioning.
                    //to line up the 5 animation frame changes with one another. This was critical for certain weapons and attacks.
                    //this is a ridiculous if then loop set. Researching better ways of structuring this, of possible.
                    if (weaponState == WeaponStates.Idle)
                    {
                        //bobbing system. Need to simplify this if then check.
                        if ((InputManager.Instance.HasAction(InputManager.Actions.MoveRight) || InputManager.Instance.HasAction(InputManager.Actions.MoveLeft) || InputManager.Instance.HasAction(InputManager.Actions.MoveForwards) || InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards)))
                        {
                            if (bob >= .10f && bobSwitch)
                                bobSwitch = false;
                            else if (bob <= 0 && !bobSwitch)
                                bobSwitch = true;

                            if (bobSwitch)
                                bob = bob + Random.Range(.0005f, .001f);
                            else
                                bob = bob - Random.Range(.0005f, .001f);
                        }

                        curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[0].startIndex];
                        weaponAnimRecordIndex = 0;
                        anim.Offset = (bob / 1.5f) - .07f;
                        anim.Offsety = (bob * 1.5f) - .15f;
                    }
                    else
                    {
                        //begging of ridiculous if then loops to setup and place each animation frame by frame. Ensures smoothing no matter attack or weapon.
                        curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[weaponAnimRecordIndex].startIndex + currentFrame];
                        if (weaponState == WeaponStates.StrikeLeft)
                        {
                            if (WeaponType == WeaponTypes.Flail || WeaponType == WeaponTypes.Flail_Magic)
                            {
                                if (GetCurrentFrame() <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[3].startIndex + 3];
                                    weaponAnimRecordIndex = 3;
                                    anim.Offset = posi - .65f;
                                }
                                else if (GetCurrentFrame() == 2)
                                {
                                    posi = posi + .002f;
                                    Rect rect = weaponRects[weaponIndices[6].startIndex + 2];
                                    curAnimRect = new Rect(rect.xMax, rect.yMin, -rect.width, rect.height);
                                    weaponAnimRecordIndex = 6;
                                    anim.Offset = posi + .1f;
                                }
                                else
                                {
                                    anim.Offset = posi;
                                    anim.Offsety = (posi / 2) * -1;
                                }
                            }
                            else if (WeaponType == WeaponTypes.Dagger || WeaponType == WeaponTypes.Dagger_Magic)
                            {
                                if (GetCurrentFrame() <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[2].startIndex + 2];
                                    weaponAnimRecordIndex = 2;
                                    anim.Offset = posi - .25f;
                                }
                                else
                                {
                                    anim.Offset = posi;
                                }
                            }
                            else if (WeaponType == WeaponTypes.Melee)
                            {
                                curAnimRect = isImported ? new Rect(0, 0, -1, 1) : weaponRects[weaponIndices[2].startIndex + currentFrame];
                                weaponAnimRecordIndex = 2;
                                if (GetCurrentFrame() <= 2)
                                {
                                    anim.Offset = -.5f;
                                    anim.Offsety = posi - .165f;
                                }
                                else if (GetCurrentFrame() == 3)
                                {
                                    anim.Offset = -.5f;
                                    anim.Offsety = posi * -1;
                                }
                                else if (GetCurrentFrame() == 4)
                                {
                                    anim.Offset = -.5f;
                                    anim.Offsety = posi - .165f;
                                }
                                else if (GetCurrentFrame() == 5)
                                {
                                    anim.Offset = -.5f;
                                    anim.Offsety = posi * -1;
                                }
                            }
                            else
                            {
                                anim.Offset = posi;
                                anim.Offsety = (posi / 6) * -1;
                            }

                        }
                        else if (weaponState == WeaponStates.StrikeRight)
                        {
                            if (WeaponType == WeaponTypes.Flail || WeaponType == WeaponTypes.Flail_Magic)
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[4].startIndex + 3];
                                    weaponAnimRecordIndex = 4;
                                    anim.Offset = posi - .65f;
                                }
                                else if (currentFrame == 2)
                                {
                                    posi = posi + .003f;
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 2];
                                    weaponAnimRecordIndex = 6;
                                    anim.Offset = posi + .075f;
                                    anim.Offsety = (posi / 2) - .1f;
                                }
                                else
                                {
                                    anim.Offset = posi;
                                    anim.Offsety = posi / 2;
                                }
                            }
                            else if (WeaponType == WeaponTypes.Dagger || WeaponType == WeaponTypes.Dagger_Magic)
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[5].startIndex + 2];
                                    weaponAnimRecordIndex = 5;
                                    anim.Offset = posi - .25f;
                                }
                                else
                                {
                                    anim.Offset = posi;
                                }
                            }
                            else if (WeaponType == WeaponTypes.Melee)
                            {
                                if (currentFrame <= 1)
                                {
                                    lerpRange = .03f;
                                    anim.Offset = posi - .15f;
                                    anim.Offsety = (posi / 2) - .15f;
                                }
                                else if (currentFrame == 2)
                                {
                                    anim.Offset = posi - .45f;
                                    anim.Offsety = posi - .24f;
                                }
                                else if (currentFrame == 3)
                                {
                                    anim.Offset = (posi - .45f);
                                    anim.Offsety = ((posi / 2) * -1);
                                }
                                else if (currentFrame == 4)
                                {
                                    anim.Offset = (posi - .45f);
                                    //posi = posi + .004f;
                                    anim.Offsety = ((posi / 2) * -1);
                                }
                            }
                            else if (WeaponType == WeaponTypes.Werecreature)
                            {
                                curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[5].startIndex + currentFrame];
                                weaponAnimRecordIndex = 5;
                                if (currentFrame < 6)
                                    anim.Offsety = (posi / 3) * -1;
                                anim.Offset = (posi * -1) + .3f;
                            }
                            else
                            {
                                anim.Offset = posi;
                                anim.Offsety = (posi / 6) * -1;
                            }
                        }
                        else if (weaponState == WeaponStates.StrikeDown)
                        {
                            if (WeaponType == WeaponTypes.Flail || WeaponType == WeaponTypes.Flail_Magic)
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[1].startIndex + 2];
                                    weaponAnimRecordIndex = 1;
                                    anim.Offset = (posi) - .25f;
                                    anim.Offsety = (posi / 2) * -1;
                                }
                                else if (currentFrame == 2)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 3];
                                    weaponAnimRecordIndex = 6;
                                    anim.Offset = (posi / 3) - .05f;
                                    anim.Offsety = posi * -1;
                                }
                                else if (currentFrame == 3)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 2];
                                    weaponAnimRecordIndex = 6;
                                    anim.Offset = (posi / 3) - .05f;
                                    anim.Offsety = (posi * -1) - .05f;
                                }
                                else
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 1];
                                    weaponAnimRecordIndex = 6;
                                    anim.Offset = (posi / 3) - .05f;
                                    anim.Offsety = (posi * -1) - .1f;
                                }
                            }
                            else if (WeaponType == WeaponTypes.Dagger || WeaponType == WeaponTypes.Dagger_Magic)
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[2].startIndex + 2];
                                    weaponAnimRecordIndex = 2;
                                    anim.Offset = (posi / 2) - .2f;
                                    anim.Offsety = ((posi) * -1) + .05f;
                                }
                                else
                                {
                                    anim.Offset = posi / 4;
                                    anim.Offsety = (posi) * -1;
                                }
                            }
                            else if (WeaponType == WeaponTypes.Battleaxe || WeaponType == WeaponTypes.Battleaxe_Magic)
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 3];
                                    weaponAnimRecordIndex = 1;
                                    anim.Offset = (posi / 4) - .025f;
                                    anim.Offsety = (posi * -1) + .1f;
                                }
                                else if (currentFrame == 2)
                                {
                                    posi = posi + .003f;
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 4];
                                    weaponAnimRecordIndex = 1;
                                    anim.Offset = (posi / 4) + .05f;
                                    anim.Offsety = (posi * -1) - .2f;
                                }
                                else if (currentFrame == 3)
                                {
                                    posi = posi + .003f;
                                    anim.Offset = (posi / 4) + .1f;
                                    anim.Offsety = ((posi) * -1) - .05f;
                                }
                                else
                                {
                                    posi = posi + .003f;
                                    anim.Offset = (posi / 4) + .1f;
                                    anim.Offsety = ((posi) * -1) - .15f;
                                }
                            }
                            else if (WeaponType == WeaponTypes.Werecreature)
                            {
                                curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + currentFrame];
                                weaponAnimRecordIndex = 6;
                                if (currentFrame < 3)
                                    anim.Offsety = posi - .1f;
                                else
                                    anim.Offsety = (posi * -1);
                            }
                            else if (WeaponType == WeaponTypes.Melee)
                            {
                                curAnimRect = isImported ? new Rect(0, 0, -1, 1) : weaponRects[weaponIndices[3].startIndex + currentFrame];
                                weaponAnimRecordIndex = 3;
                                if (currentFrame < 3)
                                    anim.Offsety = posi - .14f;
                                else
                                    anim.Offsety = posi * -1;


                            }
                            else
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 4];
                                    weaponAnimRecordIndex = 1;
                                    anim.Offset = (posi / 4);
                                    anim.Offsety = posi * -1;
                                }
                                else if (currentFrame == 2)
                                {
                                    posi = posi + .003f;
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 3];
                                    weaponAnimRecordIndex = 1;
                                    anim.Offset = (posi / 4) + .075f;
                                    anim.Offsety = (posi * -1) - .15f;
                                }
                                else if (currentFrame == 3)
                                {
                                    posi = posi + .003f;
                                    anim.Offset = (posi / 4) + .2f;
                                    anim.Offsety = (posi * -1) - .05f;
                                }
                                else
                                {
                                    posi = posi + .003f;
                                    anim.Offset = (posi / 4) + .2f;
                                    anim.Offsety = (posi * -1) - .15f;
                                }
                            }
                        }
                        else if (weaponState == WeaponStates.StrikeUp)
                        {
                            if ((WeaponType == WeaponTypes.Flail || WeaponType == WeaponTypes.Flail_Magic) && currentFrame < 4)
                            {
                                anim.Offsety = (posi / 2) - .22f;
                            }
                            else if ((WeaponType == WeaponTypes.Flail || WeaponType == WeaponTypes.Flail_Magic) && currentFrame == 4)
                            {
                                curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + 3];
                                anim.Offsety = (posi / 2) - .11f;
                            }
                            else if (WeaponType == WeaponTypes.Melee)
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[5].startIndex + currentFrame];
                                    weaponAnimRecordIndex = 5;
                                    anim.Offset = posi;
                                    anim.Offsety = posi - .14f;
                                }
                                else if (currentFrame == 2)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[5].startIndex + currentFrame];
                                    weaponAnimRecordIndex = 5;
                                    anim.Offset = posi;
                                    anim.Offsety = posi - .14f;
                                }
                                else if (currentFrame == 3)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[5].startIndex + currentFrame];
                                    weaponAnimRecordIndex = 5;
                                    anim.Offset = posi;
                                    anim.Offsety = (posi * -1);
                                }
                                else if (currentFrame == 4)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[5].startIndex + currentFrame];
                                    weaponAnimRecordIndex = 5;
                                    anim.Offset = posi;
                                    anim.Offsety = (posi * -1);
                                }
                            }
                            else if (WeaponType == WeaponTypes.Werecreature)
                            {
                                curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[1].startIndex + currentFrame];
                                weaponAnimRecordIndex = 1;
                                if (currentFrame < 3)
                                    anim.Offsety = posi - .1f;
                                else
                                    anim.Offsety = (posi * -1);
                            }
                            else if (WeaponType == WeaponTypes.Staff || WeaponType == WeaponTypes.Staff_Magic)
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[0].startIndex];
                                    weaponAnimRecordIndex = 0;
                                    anim.Offset = .25f;
                                    anim.Offsety = (posi * -1) * 2.2f;
                                }
                                else if (currentFrame == 2)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + currentFrame];
                                    weaponAnimRecordIndex = 6;
                                    anim.Offset = .25f;
                                    anim.Offsety = posi - .65f;
                                }
                                else if (currentFrame == 3)
                                {
                                    anim.Offset = .25f;
                                    anim.Offsety = posi - .45f;
                                }
                                else if (currentFrame == 4)
                                {
                                    anim.Offset = .25f;
                                    anim.Offsety = posi - .25f;
                                }
                            }
                            else
                            {
                                if (currentFrame <= 1)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[0].startIndex];
                                    weaponAnimRecordIndex = 0;
                                    anim.Offsety = (posi * -1) * 2.2f;
                                }
                                else if (currentFrame == 2)
                                {
                                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[6].startIndex + currentFrame];
                                    weaponAnimRecordIndex = 6;
                                    anim.Offsety = posi - .65f;
                                }
                                else if (currentFrame == 3)
                                {
                                    anim.Offsety = posi - .45f;
                                }
                                else if (currentFrame == 4)
                                {
                                    anim.Offsety = posi - .25f;
                                }
                            }
                        }
                    }
                }

                //built in check for bow. If player has bow, assigns anime weapon state to ensure default bow mechanics work.
                //originally, this was assigned globally without any of the above offset code or if then check.
                if (WeaponType == WeaponTypes.Bow)
                    anim = weaponAnims[(int)weaponState];

                // Get weapon dimensions
                int width = weaponIndices[weaponAnimRecordIndex].width;
                int height = weaponIndices[weaponAnimRecordIndex].height;

                // Get weapon scale
                weaponScaleX = (float)Screen.width / (float)nativeScreenWidth;
                weaponScaleY = (float)Screen.height / (float)nativeScreenHeight;

                // Adjust scale to be slightly larger when not using point filtering
                // This reduces the effect of filter shrink at edge of display
                if (dfUnity.MaterialReader.MainFilterMode != FilterMode.Point)
                {
                    weaponScaleX *= 1.01f;
                    weaponScaleY *= 1.01f;
                }

                // Source weapon images are designed to overlay a fixed 320x200 display.
                // Some weapons need to align with both top, bottom, and right of display.
                // This means they might be a little stretched on widescreen displays.
                switch (anim.Alignment)
                {
                    case WeaponAlignment.Left:
                        AlignLeft(anim, width, height);
                        break;

                    case WeaponAlignment.Center:
                        AlignCenter(anim, width, height);
                        break;

                    case WeaponAlignment.Right:
                        AlignRight(anim, width, height);
                        break;
                }
                // Set the frame time (attack speed)
                animTickTime = GetAnimTickTime();
            }
            catch (IndexOutOfRangeException)
            {
                DaggerfallUnity.LogMessage("Index out of range exception for weapon animation. Probably due to weapon breaking + being unequipped during animation.");
            }
        }

        //*COMBAT OVERHAUL ADDITION*//
        //all these formulas had offset vars added from the animation file
        //this allows the animation system to adjust the offset by changing
        //the position of the sprite on the screen.
        private void AlignLeft(WeaponAnimation anim, int width, int height)
        {
            weaponPosition = new Rect(
                Screen.width * anim.Offset,
                (Screen.height - height * weaponScaleY) * (1f - anim.Offsety),
                width * weaponScaleX,
                height * weaponScaleY);
        }

        private void AlignCenter(WeaponAnimation anim, int width, int height)
        {
            weaponPosition = new Rect(
                (((Screen.width * (1f - anim.Offset)) / 2f) - (width * weaponScaleX) / 2f),
                Screen.height * (1f - anim.Offsety) - height * weaponScaleY,
                width * weaponScaleX,
                height * weaponScaleY);
        }

        private void AlignRight(WeaponAnimation anim, int width, int height)
        {
            if (FlipHorizontal && (weaponState == WeaponStates.Idle || weaponState == WeaponStates.StrikeDown || weaponState == WeaponStates.StrikeUp))
            {
                // Flip alignment
                AlignLeft(anim, width, height);
                return;
            }

            weaponPosition = new Rect(
                (Screen.width * (1f - anim.Offset) - width * weaponScaleX),
                (Screen.height * (1f - anim.Offsety) - height * weaponScaleY),
                width * weaponScaleX,
                height * weaponScaleY);
        }

        private bool ReadyCheck()
        {
            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("FPSWeapon: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Ensure cif reader is ready
            if (cifFile == null)
            {
                cifFile = new CifRciFile();
                cifFile.Palette.Load(Path.Combine(dfUnity.Arena2Path, cifFile.PaletteName));
            }

            return true;
        }

        IEnumerator AnimateWeapon()
        {
            while (true)
            {
                posi = 0;

                if (weaponAnims != null && ShowWeapon && !GameManager.Instance.WeaponManager.hitobject)
                {
                    int frameBeforeStepping = currentFrame;

                    // Special animation for unarmed attack to left
                    if ((WeaponType == WeaponTypes.Melee || WeaponType == WeaponTypes.Werecreature)
                        && WeaponState == WeaponStates.StrikeLeft)
                    {
                        // Step frame
                        currentFrame = leftUnarmedAnims[leftUnarmedAnimIndex];
                        leftUnarmedAnimIndex++;
                        if (leftUnarmedAnimIndex >= leftUnarmedAnims.Length)
                        {
                            ChangeWeaponState(WeaponStates.Idle);
                            leftUnarmedAnimIndex = 0;
                        }
                    }
                    else if (WeaponType == WeaponTypes.Bow && weaponState == WeaponStates.StrikeUp)
                    {
                        // Step each frame for drawing the bow until reach frame for ready to release arrow
                        if (currentFrame < weaponAnims[(int)weaponState].NumFrames - 1)
                            currentFrame++;
                        // Record animation ticks for drawing and then holding a bow
                        animTicks++;
                    }
                    else
                    {
                        // Step frame
                        currentFrame++;
                        if (currentFrame >= weaponAnims[(int)weaponState].NumFrames)
                        {
                            if (IsPlayingOneShot())
                            {
                                ChangeWeaponState(WeaponStates.Idle);   // If this is a one-shot anim go to queued weapon state
                                if (WeaponType == WeaponTypes.Bow)
                                    ShowWeapon = false;                 // Immediately hide bow so its idle frame doesn't show before it is hidden for its cooldown
                            }
                            else if (WeaponType == WeaponTypes.Bow && !DaggerfallUnity.Settings.BowDrawback)
                                currentFrame = 3;
                            else
                                currentFrame = 0;                       // Otherwise keep looping frames
                        }
                    }

                    // Only update if the frame actually changed
                    if (frameBeforeStepping != currentFrame)
                    {
                        //*COMBAT OVERHAUL ADDITION*//
                        //calculates lerp values for each frame change. When the frame changes,
                        //it grabs the current total animation time, amount of passed time, users fps,
                        //and then uses them to calculate and set the lerp value to ensure proper animation
                        //offsetting no matter users fps or attack speed.


                        //grabs animation time for each frame render of the 5 frames.
                        time = animTickTime;

                        //calculates the current time that has passed
                        timePass = +Time.deltaTime;

                        //insures calculation only happens every frame.
                        if (timePass > 1f)
                        {
                            timePass = 1f;
                        }

                        //how much time has passed in the animation
                        percentagetime = timePass / time;

                        //computes users avg fps.
                        avgFrameRate = Mathf.Round(1 / Time.unscaledDeltaTime);

                        //caps calculation at 60 fps to match engine cap.
                        //Stops weird animation offsetting calculations that happen passed 60fps.
                        if (avgFrameRate > 60)
                            avgFrameRate = 60;

                        //computes the amount of time each frame takes to render in seconds.
                        maxframeseconds = 1 / avgFrameRate;

                        //calculates the totaloffsets possible within the attack animation time by dividing the animation time
                        //by the time a frame would render at players current max fps. Rounds down to ensure doens't render more than possible.
                        //This is critical to calculate and use in the lerp calculations to ensure default attack speeds work no matter user fps.
                        totalOffsets = Mathf.FloorToInt(time / maxframeseconds);

                        //figures out the range the sprite needs to move by taking the total animation time and dividing it by float value.
                        //Float value was randomly choosen based on how clearly it lined up with default sprite animation changes. Unsure
                        //why these base numbers work best, but they work. Need to research this, and see if there is a reason to choose a certain value.
                        //added console trigger to turn on and off smooth animations. Also, added case switch with console float option in GetAnimationOffset().
                        if (FPSConsoleCommands.DisableSmoothAnimations.disableSmoothAnimations == false)
                            lerpRange = GetAnimationOffset();
                        else if (FPSConsoleCommands.DisableSmoothAnimations.disableSmoothAnimations == true)
                            lerpRange = 0;

                        //figures out how long to wait before restarting coroutine based on the number of frame offsets
                        //divided by the total time of the individual frame animation. This ensures attack speeds are not
                        //slowed down by needing to render more offsets than possible in the attack animation time; used in coroutine below.
                        time = time / totalOffsets;

                        //sets the offset range using the mathf.lerp calculation.
                        lerpRange = Mathf.Lerp(0, lerpRange, percentagetime);

                        //default engine update loop starts here.
                        UpdateWeapon();
                    }
                }

                //*COMBAT OVERHAUL ADDITION*//
                //start parry animation if hitobject detected. Begins reversing frames at an increased speed.
                //Goes to idle state at frame 0. This simulates the recoil of hitting an object with a large sword.
                else if (weaponAnims != null && ShowWeapon && GameManager.Instance.WeaponManager.hitobject)
                {
                    int frameBeforeStepping = currentFrame;

                    // Special animation for unarmed attack to left
                    if ((WeaponType == WeaponTypes.Melee || WeaponType == WeaponTypes.Werecreature)
                        && WeaponState == WeaponStates.StrikeLeft)
                    {
                        // Step frame
                        currentFrame = leftUnarmedAnims[leftUnarmedAnimIndex];
                        leftUnarmedAnimIndex--;
                        if (leftUnarmedAnimIndex <= leftUnarmedAnims.Length)
                        {
                            ChangeWeaponState(WeaponStates.Idle);
                            leftUnarmedAnimIndex = 0;
                        }
                    }
                    //reverse attack animation if not on 0/idle frame.
                    else if (currentFrame >= 1)
                    {
                        // Step frame
                        currentFrame--;
                    }
                    //when recoil gets to frame 0, set weapon state to idle to stop attack looping.
                    else
                    {
                        currentFrame = 0;
                        ChangeWeaponState(WeaponStates.Idle);   // If this is a one-shot anim go to queued weapon state
                    }

                    // Only update if the frame actually changed
                    if (frameBeforeStepping != currentFrame)
                        UpdateWeapon();
                }

                //*COMBAT OVERHAUL ADDITION*//
                //added mahf.lerp calculations for animation offsetting to ensure better smoothness no matter speed or framerate.
                if (WeaponState != WeaponStates.Idle && !GameManager.Instance.WeaponManager.hitobject && WeaponType != WeaponTypes.Bow)
                {
                    //begin for loop to loop until the maximum number of offset frames are ran. Ensures animation doesn't go longer than
                    //the calcuated attack speed/time and thus slow down the users attack speed.
                    for (int Offset = 0; Offset < totalOffsets; Offset++)
                    {
                        //moves the sprite using the above mathf.lerp calculation
                        //lerprange is checked and recalculated every animation frame update out of the 5.
                        posi = posi + lerpRange;

                        //waits between offsets and moves to ensure attack speed is not influenced by fps/engine render limits.
                        yield return new WaitForSeconds(time);
                    }
                }
                else if (GameManager.Instance.WeaponManager.hitobject)
                {
                    //added recoil if then check to allow more recoil animation custimization.
                    yield return new WaitForSeconds(animTickTime*1.33f);
                }
                else
                {
                    //added else trigger with combat overhaul mod to ensure
                    //All other animations wait default animation time and renders new frame.
                    yield return new WaitForSeconds(animTickTime);
                }
            }
        }

        //*COMBAT OVERHAUL ADDITION*//
        //added with combat overhaul for coding/debugging assistance.
        IEnumerator DebugDisplay()
        {
            while (false)
            {
                if (WeaponState == WeaponStates.Idle)
                    action = "idle";
                else
                    action = "attacking";

                DaggerfallUI.Instance.PopupMessage("FrameRate: " + avgFrameRate.ToString());
                DaggerfallUI.Instance.PopupMessage("MaxFrames: " + maxframeseconds.ToString());

                DaggerfallUI.Instance.PopupMessage("Total Offsets: " + totalOffsets.ToString());
                DaggerfallUI.Instance.PopupMessage("Current Offsets: " + Offset.ToString());

                DaggerfallUI.Instance.PopupMessage("WeaponState: " + action);

                DaggerfallUI.Instance.PopupMessage("Time Pass: " + timePass.ToString());
                DaggerfallUI.Instance.PopupMessage("% Time: " + percentagetime.ToString());

                DaggerfallUI.Instance.PopupMessage("Lerp Range: " + lerpRange.ToString());

                yield return new WaitForSeconds(2f);
            }
        }

        private float GetAnimTickTime()
        {
            //added to enable on the fly attack speed changes.
            AttackSpeed = (AttackSpeed + FPSConsoleCommands.ChangeAttackSpeed.changeAttackSpeed) / 200;

            PlayerEntity player = GameManager.Instance.PlayerEntity;
            if (WeaponType == WeaponTypes.Bow || player == null)
                return GameManager.classicUpdateInterval;
            else
                //added attack speed modifier. It divides by a large number because base animation speeds are divided by 980 at the end to get MS calculations.             
                return (FormulaHelper.GetMeleeWeaponAnimTime(player, WeaponType, WeaponHands) + AttackSpeed);
        }

        private void LoadWeaponAtlas()
        {
            // Get weapon filename
            string filename = WeaponBasics.GetWeaponFilename(WeaponType);

            // Load the weapon texture atlas
            // Texture is dilated into a transparent coloured border to remove dark edges when filtered
            // Important to use returned UV rects when drawing to get right dimensions
            weaponAtlas = GetWeaponTextureAtlas(filename, MetalType, out weaponRects, out weaponIndices, 2, 2, true);
            weaponAtlas.filterMode = dfUnity.MaterialReader.MainFilterMode;

            // Get weapon anims
            weaponAnims = (WeaponAnimation[])WeaponBasics.GetWeaponAnims(WeaponType).Clone();

            // Store current weapon
            currentWeaponType = WeaponType;
            currentMetalType = MetalType;
        }

        #endregion

        #region Texture Loading

        private Texture2D GetWeaponTextureAtlas(
            string filename,
            MetalTypes metalType,
            out Rect[] rectsOut,
            out RecordIndex[] indicesOut,
            int padding,
            int border,
            bool dilate = false)
        {
            // Load texture file
            cifFile.Load(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);

            // Read every image in archive
            Rect rect;
            List<Texture2D> textures = new List<Texture2D>();
            List<RecordIndex> indices = new List<RecordIndex>();
            customTextures.Clear();
            for (int record = 0; record < cifFile.RecordCount; record++)
            {
                int frames = cifFile.GetFrameCount(record);
                DFSize size = cifFile.GetSize(record);
                RecordIndex ri = new RecordIndex()
                {
                    startIndex = textures.Count,
                    frameCount = frames,
                    width = size.Width,
                    height = size.Height,
                };
                indices.Add(ri);
                for (int frame = 0; frame < frames; frame++)
                {
                    textures.Add(GetWeaponTexture2D(filename, record, frame, metalType, out rect, border, dilate));

                    Texture2D tex;
                    if (TextureReplacement.TryImportCifRci(filename, record, frame, metalType, true, out tex))
                    {
                        tex.filterMode = dfUnity.MaterialReader.MainFilterMode;
                        tex.wrapMode = TextureWrapMode.Mirror;
                        customTextures.Add(MaterialReader.MakeTextureKey(0, (byte)record, (byte)frame), tex);
                    }
                }
            }

            // Pack textures into atlas
            Texture2D atlas = new Texture2D(2048, 2048, TextureFormat.ARGB32, false);
            rectsOut = atlas.PackTextures(textures.ToArray(), padding, 2048);
            indicesOut = indices.ToArray();

            // Shrink UV rect to compensate for internal border
            float ru = 1f / atlas.width;
            float rv = 1f / atlas.height;
            for (int i = 0; i < rectsOut.Length; i++)
            {
                Rect rct = rectsOut[i];
                rct.xMin += border * ru;
                rct.xMax -= border * ru;
                rct.yMin += border * rv;
                rct.yMax -= border * rv;
                rectsOut[i] = rct;
            }

            return atlas;
        }

        private Texture2D GetWeaponTexture2D(
            string filename,
            int record,
            int frame,
            MetalTypes metalType,
            out Rect rectOut,
            int border = 0,
            bool dilate = false)
        {
            // Get source bitmap
            DFBitmap dfBitmap = cifFile.GetDFBitmap(record, frame);

            // Tint based on metal type
            // But not for steel as that is default colour in files
            if (metalType != MetalTypes.Steel && metalType != MetalTypes.None)
                dfBitmap = ImageProcessing.ChangeDye(dfBitmap, ImageProcessing.GetMetalDyeColor(metalType), DyeTargets.WeaponsAndArmor);

            // Get Color32 array
            DFSize sz;
            Color32[] colors = cifFile.GetColor32(dfBitmap, 0, border, out sz);

            // Dilate edges
            if (border > 0 && dilate)
                ImageProcessing.DilateColors(ref colors, sz);

            // Create Texture2D
            Texture2D texture = new Texture2D(sz.Width, sz.Height, TextureFormat.ARGB32, false);
            texture.SetPixels32(colors);
            texture.Apply(true);

            // Shrink UV rect to compensate for internal border
            float ru = 1f / sz.Width;
            float rv = 1f / sz.Height;
            rectOut = new Rect(border * ru, border * rv, (sz.Width - border * 2) * ru, (sz.Height - border * 2) * rv);

            return texture;
        }

        #endregion
    }
}