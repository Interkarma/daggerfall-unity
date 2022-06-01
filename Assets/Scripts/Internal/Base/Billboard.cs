// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop
{
    [Serializable]
    public struct BillboardSummary
    {
        public Vector2 Size;                                // Size and scale in world units
        public Rect Rect;                                   // Single UV rectangle for non-atlased materials only
        public Rect[] AtlasRects;                           // Array of UV rectangles for atlased materials only
        public RecordIndex[] AtlasIndices;                  // Indices into UV rect array for atlased materials only, supports animations
        public bool AtlasedMaterial;                        // True if material is part of an atlas
        public bool AnimatedMaterial;                       // True if material uses atlas UV animations (always false for non atlased materials)
        public int CurrentFrame;                            // Current animation frame
        public FlatTypes FlatType;                          // Type of flat
        public EditorFlatTypes EditorFlatType;              // Sub-type of flat when editor/marker
        public bool IsMobile;                               // Billboard is a mobile enemy
        public int Archive;                                 // Texture archive index
        public int Record;                                  // Texture record index
        public int Flags;                                   // NPC Flags found in RMB and RDB NPC data
        public int FactionOrMobileID;                       // FactionID for NPCs, Mobile ID for monsters
        public int NameSeed;                                // NPC name seed
        public MobileTypes FixedEnemyType;                  // Type for fixed enemy marker
        public short WaterLevel;                            // Dungeon water level
        public bool CastleBlock;                            // Non-hostile area of main story castle dungeons
        public BillboardImportedTextures ImportedTextures;  // Textures imported from mods
    }

    public abstract class Billboard : MonoBehaviour
    {
        /// <summary>
        /// General frames per second for animation
        /// </summary>
        public abstract int FramesPerSecond { get; set; }

        /// <summary>
        /// Plays animation once then destroys GameObject
        /// </summary>
        public abstract bool OneShot { get; set; }

        /// <summary>
        /// Billboard should also face camera up/down
        /// </summary>
        public abstract bool FaceY { get; set; }

        [SerializeField]
        protected BillboardSummary summary = new BillboardSummary();

        public BillboardSummary Summary
        {
            get { return summary; }
        }
        
        /// <summary>
        /// Sets extended data about people billboard from RMB resource data.
        /// </summary>
        /// <param name="person"></param>
        public abstract void SetRMBPeopleData(DFBlock.RmbBlockPeopleRecord person);

        /// <summary>
        /// Sets people data directly.
        /// </summary>
        /// <param name="factionID">FactionID of person.</param>
        /// <param name="flags">Person flags.</param>
        public abstract void SetRMBPeopleData(int factionID, int flags, long position = 0);

        /// <summary>
        /// Sets extended data about billboard from RDB flat resource data.
        /// </summary>
        public abstract void SetRDBResourceData(DFBlock.RdbFlatResource resource);

        /// <summary>
        /// Sets new Daggerfall material and recreates mesh.
        /// Will use an atlas if specified in DaggerfallUnity singleton.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="archive">Texture archive index.</param>
        /// <param name="record">Texture record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>Material.</returns>
        public abstract Material SetMaterial(int archive, int record, int frame = 0);

        /// <summary>
        /// Sets billboard material with a custom texture.
        /// </summary>
        /// <param name="texture">Texture2D to set on material.</param>
        /// <param name="size">Size of billboard quad in normal units (not Daggerfall units).</param>
        /// <returns>Material.</returns>
        public abstract Material SetMaterial(Texture2D texture, Vector2 size, bool isLightArchive = false);

        /// <summary>
        /// Aligns billboard to centre of base, rather than exact centre.
        /// Must have already set material using SetMaterial() for billboard dimensions to be known.
        /// </summary>
        public abstract void AlignToBase();

    }
}