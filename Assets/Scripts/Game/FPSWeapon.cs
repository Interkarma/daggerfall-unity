// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut, Kirk.O, Numidium
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;

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
        protected class WeaponAtlas
        {
            public string FileName { get; set; }
            public MetalTypes MetalType { get; set; }
            public Texture2D AtlasTexture { get; set; }
            public Rect[] WeaponRects { get; set; }
            public RecordIndex[] WeaponIndices { get; set; }
        }

        protected class CustomWeaponAnimation
        {
            public string FileName { get; set; }
            public MetalTypes MetalType { get; set; }
            public Dictionary<int, Texture2D> Textures { get; set; }
        }

        public bool ShowWeapon = true;
        public bool FlipHorizontal = false;
        public WeaponTypes WeaponType = WeaponTypes.None;
        public MetalTypes MetalType = MetalTypes.None;
        public ItemHands WeaponHands = ItemHands.None;
        public DaggerfallUnityItem SpecificWeapon = null;
        public float Reach = 2.5f;
        public float AttackSpeedScale = 1.0f;
        public float Cooldown = 0.0f;
        public SoundClips DrawWeaponSound = SoundClips.DrawWeapon;
        public SoundClips SwingWeaponSound = SoundClips.SwingMediumPitch;

        protected WeaponTypes currentWeaponType;
        protected MetalTypes currentMetalType;

        protected const int nativeScreenWidth = 320;
        protected const int nativeScreenHeight = 200;

        protected readonly byte[] leftUnarmedAnims = { 0, 1, 2, 3, 4, 2, 1, 0 };
        protected int leftUnarmedAnimIndex = 0;

        protected DaggerfallUnity dfUnity;
        protected CifRciFile cifFile;
        protected WeaponAtlas weaponAtlas;
        protected readonly Queue<WeaponAtlas> weaponAtlasCache = new Queue<WeaponAtlas>();
        protected readonly Queue<CustomWeaponAnimation> customWeaponAnimationCache = new Queue<CustomWeaponAnimation>();
        protected Rect weaponPosition;
        protected float weaponScaleX;
        protected float weaponScaleY;

        protected DaggerfallAudioSource dfAudioSource;
        protected WeaponAnimation[] weaponAnims;
        protected WeaponStates weaponState = WeaponStates.Idle;
        protected int currentFrame = 0;
        protected int animTicks = 0;
        protected float animTickTime;
        protected Rect curAnimRect;
        protected float weaponOffsetHeight;
        protected Rect screenRect;

        protected Dictionary<int, Texture2D> customTextures = new Dictionary<int, Texture2D>();
        protected Texture2D curCustomTexture;

        // Allows a mod to specify if DFU should use custom HUD animations for weapons
        public static bool moddedWeaponHUDAnimsEnabled = false;

        protected float lastScreenWidth, lastScreenHeight;
        protected bool lastLargeHUDSetting, lastLargeHUDDockSetting;
        protected bool lastSheathed;
        protected float lastWeaponOffsetHeight;

        #region Properties

        public WeaponStates WeaponState { get { return weaponState; } }

        #endregion

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            StartCoroutine(AnimateWeapon());
        }

        protected virtual void OnGUI()
        {
            bool updateWeapon = false;
            GUI.depth = 1;

            if (DaggerfallUI.Instance.CustomScreenRect != null)
                screenRect = DaggerfallUI.Instance.CustomScreenRect.Value;
            else
                screenRect = new Rect(0, 0, Screen.width, Screen.height);

            // Must be ready and not loading the game
            if (!ReadyCheck() || WeaponType == WeaponTypes.None || GameManager.IsGamePaused || SaveLoadManager.Instance.LoadInProgress)
                return;

            // Must have current weapon texture atlas
            if (weaponAtlas == null || WeaponType != currentWeaponType || MetalType != currentMetalType)
            {
                LoadWeaponAtlas();
                if (weaponAtlas == null)
                    return;
                updateWeapon = true;
            }

            // Offset weapon by large HUD height when both large HUD and undocked weapon offset enabled
            // Weapon is forced to offset when using docked HUD else it would appear underneath HUD
            // This helps user avoid such misconfiguration or it might be interpreted as a bug
            weaponOffsetHeight = 0;
            if (DaggerfallUI.Instance.DaggerfallHUD != null &&
                DaggerfallUnity.Settings.LargeHUD &&
                (DaggerfallUnity.Settings.LargeHUDUndockedOffsetWeapon || DaggerfallUnity.Settings.LargeHUDDocked))
            {
                weaponOffsetHeight = (int)DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.ScreenHeight;
            }

            // Update weapon when resolution or large HUD state changes
            if (screenRect.width != lastScreenWidth ||
                screenRect.height != lastScreenHeight ||
                DaggerfallUnity.Settings.LargeHUD != lastLargeHUDSetting ||
                DaggerfallUnity.Settings.LargeHUDDocked != lastLargeHUDDockSetting ||
                GameManager.Instance.WeaponManager.Sheathed != lastSheathed ||
                weaponOffsetHeight != lastWeaponOffsetHeight)
            {
                lastScreenWidth = screenRect.width;
                lastScreenHeight = screenRect.height;
                lastLargeHUDSetting = DaggerfallUnity.Settings.LargeHUD;
                lastLargeHUDDockSetting = DaggerfallUnity.Settings.LargeHUDDocked;
                lastSheathed = GameManager.Instance.WeaponManager.Sheathed;
                lastWeaponOffsetHeight = weaponOffsetHeight;
                updateWeapon = true;
            }

            // Update weapon state only as needed
            if (updateWeapon)
                UpdateWeapon();

            if (Event.current.type.Equals(EventType.Repaint) && ShowWeapon)
            {
                // Draw weapon texture behind other HUD elements
                DaggerfallUI.DrawTextureWithTexCoords(weaponPosition, curCustomTexture ? curCustomTexture : weaponAtlas.AtlasTexture, curAnimRect);
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
                    state = WeaponStates.StrikeDownLeft;
                    break;
                case WeaponManager.MouseDirections.Left:
                    state = WeaponStates.StrikeLeft;
                    break;
                case WeaponManager.MouseDirections.Right:
                    state = WeaponStates.StrikeRight;
                    break;
                case WeaponManager.MouseDirections.DownRight:
                    state = WeaponStates.StrikeDownRight;
                    break;
                case WeaponManager.MouseDirections.Up:
                case WeaponManager.MouseDirections.UpLeft:
                case WeaponManager.MouseDirections.UpRight:
                    state = WeaponStates.StrikeUp;
                    break;
                default:
                    return;
            }

            // Do not change if already playing attack animation, unless releasing an arrow (bow & state=up->down)
            if (!IsPlayingOneShot() || (WeaponType == WeaponTypes.Bow && weaponState == WeaponStates.StrikeUp && state == WeaponStates.StrikeDown))
                ChangeWeaponState(state);
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

        public void TryCacheReadiedWeaponAtlas(MetalTypes metalType, WeaponTypes weaponType)
        {
            var fileName = WeaponBasics.GetWeaponFilename(weaponType);
            if (GetCachedWeaponAtlas(fileName, metalType) == null)
                CacheWeaponAtlas(GetWeaponTextureAtlas(fileName, metalType, 2, 2, true));
        }

        #region Protected Methods

        protected bool IsPlayingOneShot()
        {
            return (weaponState != WeaponStates.Idle);
        }

        protected void UpdateWeapon()
        {
            // Do nothing if weapon not ready
            if (weaponAtlas == null || weaponAnims == null ||
                weaponAtlas.WeaponRects == null || weaponAtlas.WeaponIndices == null)
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
            if (!GameManager.Instance.WeaponManager.Sheathed && WeaponType == WeaponTypes.Bow && GameManager.Instance.PlayerEntity.Items.GetItem(Items.ItemGroups.Weapons, (int)Items.Weapons.Arrow, allowQuestItem: false) == null)
            {
                GameManager.Instance.WeaponManager.SheathWeapons();
                DaggerfallUI.SetMidScreenText(TextManager.Instance.GetLocalizedText("youHaveNoArrows"));
            }

            // Store rect and anim
            int weaponAnimRecordIndex;
            if (WeaponType == WeaponTypes.Bow)
                weaponAnimRecordIndex = 0; // Bow has only 1 animation
            else
                weaponAnimRecordIndex = weaponAnims[(int)weaponState].Record;

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
                        Rect rect = weaponAtlas.WeaponRects[weaponAtlas.WeaponIndices[weaponAnimRecordIndex].startIndex + currentFrame];
                        curAnimRect = new Rect(rect.xMax, rect.yMin, -rect.width, rect.height);
                    }
                }
                else
                {
                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponAtlas.WeaponRects[weaponAtlas.WeaponIndices[weaponAnimRecordIndex].startIndex + currentFrame];
                }
                WeaponAnimation anim = weaponAnims[(int)weaponState];

                // Get weapon dimensions
                int width = weaponAtlas.WeaponIndices[weaponAnimRecordIndex].width;
                int height = weaponAtlas.WeaponIndices[weaponAnimRecordIndex].height;

                // Get weapon scale
                weaponScaleX = (float)screenRect.width / (float)nativeScreenWidth;
                weaponScaleY = (float)screenRect.height / (float)nativeScreenHeight;

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

        protected void AlignLeft(WeaponAnimation anim, int width, int height)
        {
            weaponPosition = new Rect(
                screenRect.x + screenRect.width * anim.Offset,
                screenRect.y + screenRect.height - height * weaponScaleY - weaponOffsetHeight,
                width * weaponScaleX,
                height * weaponScaleY);
        }

        protected void AlignCenter(WeaponAnimation anim, int width, int height)
        {
            weaponPosition = new Rect(
                screenRect.x + screenRect.width / 2f - (width * weaponScaleX) / 2f,
                screenRect.y + screenRect.height - height * weaponScaleY - weaponOffsetHeight,
                width * weaponScaleX,
                height * weaponScaleY);
        }

        protected void AlignRight(WeaponAnimation anim, int width, int height)
        {
            if (FlipHorizontal && (weaponState == WeaponStates.Idle || weaponState == WeaponStates.StrikeDown || weaponState == WeaponStates.StrikeUp))
            {
                // Flip alignment
                AlignLeft(anim, width, height);
                return;
            }

            weaponPosition = new Rect(
                screenRect.x + screenRect.width * (1f - anim.Offset) - width * weaponScaleX,
                screenRect.y + screenRect.height - height * weaponScaleY - weaponOffsetHeight,
                width * weaponScaleX,
                height * weaponScaleY);
        }

        protected bool ReadyCheck()
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

        protected IEnumerator AnimateWeapon()
        {
            while (true)
            {
                if (weaponAnims != null && ShowWeapon)
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

                    // Only update if the frame actually changed & weapon drawn
                    if (frameBeforeStepping != currentFrame)
                        UpdateWeapon();
                }

                yield return new WaitForSeconds(animTickTime);
            }
        }

        protected float GetAnimTickTime()
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            if (WeaponType == WeaponTypes.Bow || player == null)
                return GameManager.classicUpdateInterval;
            else
                return FormulaHelper.GetMeleeWeaponAnimTime(player, WeaponType, WeaponHands);
        }

        protected void LoadWeaponAtlas()
        {
            // Get weapon filename
            string filename = WeaponBasics.GetWeaponFilename(WeaponType);

            // Load the weapon texture atlas
            // Texture is dilated into a transparent coloured border to remove dark edges when filtered
            // Important to use returned UV rects when drawing to get right dimensions
            weaponAtlas = GetWeaponTextureAtlas(filename, MetalType, 2, 2, true);
            weaponAtlas.AtlasTexture.filterMode = dfUnity.MaterialReader.MainFilterMode;

            // Get weapon anims
            weaponAnims = (WeaponAnimation[])WeaponBasics.GetWeaponAnims(WeaponType).Clone();

            // Store current weapon
            currentWeaponType = WeaponType;
            currentMetalType = MetalType;
            animTickTime = GetAnimTickTime();
        }

        #endregion

        #region Texture Loading

        protected WeaponAtlas GetWeaponTextureAtlas(
            string filename,
            MetalTypes metalType,
            int padding,
            int border,
            bool dilate = false)
        {
            // Check caches
            var cachedAtlas = GetCachedWeaponAtlas(filename, metalType);
            var cachedAnimation = GetCachedCustomWeaponAnimation(filename, metalType);
            // Nothing to load if both vanilla atlas and custom texture are currently cached.
            if (cachedAtlas != null && cachedAnimation != null)
            {
                customTextures = cachedAnimation.Textures;
                return cachedAtlas;
            }

            // Load texture file
            cifFile.Load(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);

            // Read every image in archive
            Rect rect;
            List<Texture2D> textures = new List<Texture2D>();
            List<RecordIndex> indices = new List<RecordIndex>();
            customTextures = new Dictionary<int, Texture2D>();
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
                if (cachedAtlas == null) // Load atlas if not already cached.
                {
                    for (int frame = 0; frame < frames; frame++)
                        textures.Add(GetWeaponTexture2D(filename, record, frame, metalType, out rect, border, dilate));
                }

                if (cachedAnimation != null) // No need to load frames if cached.
                    continue;
                for (int frame = 0; frame < frames; frame++)
                {
                    if (moddedWeaponHUDAnimsEnabled && SpecificWeapon != null)
                    {
                        filename = WeaponBasics.GetModdedWeaponFilename(SpecificWeapon);

                        if (string.IsNullOrEmpty(filename))
                            filename = WeaponBasics.GetWeaponFilename(WeaponType); // Possibly make support for custom weapon types for the HUD in the future.
                    }

                    Texture2D tex;
                    if (TextureReplacement.TryImportCifRci(filename, record, frame, metalType, true, out tex))
                    {
                        tex.filterMode = dfUnity.MaterialReader.MainFilterMode;
                        tex.wrapMode = TextureWrapMode.Mirror;
                        customTextures.Add(MaterialReader.MakeTextureKey(0, (byte)record, (byte)frame), tex);
                    }
                }
            }

            if (cachedAnimation == null && customTextures.Count > 0)
                CacheCustomWeaponAnimation(new CustomWeaponAnimation() { FileName = filename, MetalType = metalType, Textures = customTextures });
            if (cachedAtlas != null)
                return cachedAtlas;

            // Pack textures into atlas
            Texture2D atlasTexture = new Texture2D(2048, 2048, TextureFormat.ARGB32, false);
            Rect[] rectsOut = atlasTexture.PackTextures(textures.ToArray(), padding, 2048);
            RecordIndex[] indicesOut = indices.ToArray();

            // Shrink UV rect to compensate for internal border
            float ru = 1f / atlasTexture.width;
            float rv = 1f / atlasTexture.height;
            for (int i = 0; i < rectsOut.Length; i++)
            {
                Rect rct = rectsOut[i];
                rct.xMin += border * ru;
                rct.xMax -= border * ru;
                rct.yMin += border * rv;
                rct.yMax -= border * rv;
                rectsOut[i] = rct;
            }

            return new WeaponAtlas()
            {
                FileName = filename,
                MetalType = metalType,
                AtlasTexture = atlasTexture,
                WeaponRects = rectsOut,
                WeaponIndices = indicesOut
            };
        }

        protected Texture2D GetWeaponTexture2D(
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

        protected WeaponAtlas GetCachedWeaponAtlas(string fileName, MetalTypes metalType)
        {
            foreach (var atlas in weaponAtlasCache)
            {
                if (atlas.FileName == fileName && atlas.MetalType == metalType)
                    return atlas;
            }

            return null;
        }

        protected void CacheWeaponAtlas(WeaponAtlas weaponAtlas)
        {
            const int maxCacheSize = 2; // One for each hand.
            weaponAtlasCache.Enqueue(weaponAtlas);
            if (weaponAtlasCache.Count > maxCacheSize)
                weaponAtlasCache.Dequeue();
        }

        protected CustomWeaponAnimation GetCachedCustomWeaponAnimation(string fileName, MetalTypes metalType)
        {
            foreach (var animation in customWeaponAnimationCache)
            {
                if (animation.FileName == fileName && animation.MetalType == metalType)
                    return animation;
            }

            return null;
        }

        protected void CacheCustomWeaponAnimation(CustomWeaponAnimation animation)
        {
            const int maxCacheSize = 2;
            customWeaponAnimationCache.Enqueue(animation);
            if (customWeaponAnimationCache.Count > maxCacheSize)
                customWeaponAnimationCache.Dequeue();
        }

        #endregion
    }
}