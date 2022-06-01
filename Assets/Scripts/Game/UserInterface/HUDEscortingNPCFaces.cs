// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Displays faces of quest NPCs escorted by player.
    /// Unlike Daggerfall will try to remove face when related quest ends, even if quest script forgets to drop face.
    /// </summary>
    public class EscortingNPCFacePanel : Panel
    {
        #region Fields

        const int faceCount = 10;
        const int maxFaces = 3;
        const string factionFaceFile = "FACES.CIF";
        const string factionChildrenFaceFile = "KIDS00I0.CIF";
        const string factionExtraFaceFile = "TFAC00I0.RCI";

        List<FaceDetails> faces = new List<FaceDetails>();
        List<Panel> facePanels = new List<Panel>();

        #endregion

        #region Constructors

        public EscortingNPCFacePanel()
            : base()
        {
            QuestMachine.OnQuestEnded += QuestMachine_OnQuestEnded;
            Serialization.SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            const int startX = 8;
            const int startY = 36;
            const int spaceY = 40;
            const int spaceYSpecial = 50;

            // Align face panels - usually only a single face
            int faceCount = 0;
            Vector2 pos = new Vector2(startX, startY);
            foreach (Panel facePanel in facePanels)
            {
                // Skip if too many faces
                if (faceCount++ >= maxFaces)
                {
                    facePanel.Enabled = false;
                    continue;
                }

                // Setup panel position
                facePanel.Enabled = true;
                facePanel.Position = pos;

                // Space to next face
                if (spaceY < facePanel.Size.y)
                    pos.y += spaceYSpecial;
                else
                    pos.y += spaceY;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a Person face to HUD.
        /// </summary>
        /// <param name="person">Target Person resource to add.</param>
        public void AddFace(Person person)
        {
            faces.Add(CreateFaceDetails(person));
            RefreshFaces();
        }

        /// <summary>
        /// Adds a Foe face to HUD.
        /// Foe faces should always be humanoid as there are no portraits for monstrous enemies.
        /// Always creates a Breton face for now.
        /// </summary>
        /// <param name="foe">Target Foe resource to add.</param>
        public void AddFace(Foe foe)
        {
            faces.Add(CreateFaceDetails(foe));
            RefreshFaces();
        }

        /// <summary>
        /// Drops a Person face from HUD.
        /// </summary>
        /// <param name="person">Target Person resource to remove.</param>
        public void DropFace(Person person)
        {
            faces.RemoveAll(face => face.questUID == person.ParentQuest.UID && person.Symbol.Equals(face.targetPerson));
            RefreshFaces();
        }

        /// <summary>
        /// Drops a Foe face from HUD.
        /// </summary>
        /// <param name="foe">Target Foe resource to remove.</param>
        public void DropFace(Foe foe)
        {
            faces.RemoveAll(face => face.questUID == foe.ParentQuest.UID && foe.Symbol.Equals(face.targetFoe));
            RefreshFaces();
        }

        #endregion

        #region Private Methods

        FaceDetails CreateFaceDetails(Person person)
        {
            FaceDetails face = new FaceDetails();
            face.questUID = person.ParentQuest.UID;
            face.targetPerson = person.Symbol;
            face.targetRace = person.Race;
            face.gender = person.Gender;
            face.isChild = person.FactionData.id == (int)FactionFile.FactionIDs.Children;
            face.faceIndex = person.FaceIndex;
            face.factionFaceIndex = -1;
            
            // Read faction face index for fixed NPCs
            if (person.IsIndividualNPC)
            {
                FactionFile.FactionData fd;
                if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(person.FactionIndex, out fd))
                {
                    face.factionFaceIndex = fd.face;
                }
            }

            // Determine child faction portrait - there are only 2 variants of each gender indexed 0-3
            if (face.isChild)
            {
                UnityEngine.Random.InitState(Time.frameCount);
                int variantOffset = (UnityEngine.Random.Range(0, 2) == 0) ? 0 : 2;
                face.faceIndex = variantOffset + (int)face.gender;
            }

            return face;
        }

        FaceDetails CreateFaceDetails(Foe foe)
        {
            UnityEngine.Random.InitState(Time.frameCount);

            FaceDetails face = new FaceDetails();
            face.questUID = foe.ParentQuest.UID;
            face.targetFoe = foe.Symbol;
            face.targetRace = Races.Breton;
            face.gender = foe.Gender;
            face.faceIndex = UnityEngine.Random.Range(0, faceCount);
            face.factionFaceIndex = -1;

            return face;
        }

        public void ClearFaces()
        {
            // Remove face panels from component list
            foreach (Panel facePanel in facePanels)
            {
                Components.Remove(facePanel);
            }

            // Clear list
            facePanels.Clear();
        }

        public void RefreshFaces()
        {
            const int specialFacePanelWidth = 48;
            const int specialFacePanelHeight = 48;

            // Clear existing faces
            ClearFaces();

            // Load matching texture for each face
            // Only Redguard and Breton supported for now
            // Generally FACTION.TXT only uses these two races - could expand later
            foreach (FaceDetails face in faces)
            {
                // Get race template
                RaceTemplate raceTemplate;
                switch(face.targetRace)
                {
                    case Races.Redguard:
                        raceTemplate = new Redguard();
                        break;

                    default:
                    case Races.Breton:
                        raceTemplate = new Breton();
                        break;
                }

                // Get image for this face
                Texture2D faceTexture = null;
                Panel facePanel = new Panel();
                // Disable until placed
                facePanel.Enabled = false;
                if (face.factionFaceIndex >= 0 && face.factionFaceIndex <= 60)
                {
                    // Use special NPC face set at time face data was added
                    faceTexture = DaggerfallUI.GetTextureFromCifRci(factionFaceFile, face.factionFaceIndex);
                    facePanel.Size = new Vector2(specialFacePanelWidth, specialFacePanelHeight);
                }
                else if (face.factionFaceIndex > 60 && face.factionFaceIndex <= 502)
                {
                    // Use special NPC face set by a mod at time face data was added
                    faceTexture = DaggerfallUI.GetTextureFromCifRci(factionExtraFaceFile, face.factionFaceIndex);
                    facePanel.Size = new Vector2(specialFacePanelWidth, specialFacePanelHeight);
                }
                else
                {
                    string fileName;

                    // Use generic NPC face
                    if (!face.isChild)
                    {
                        // Get a racial portrait
                        fileName = face.gender == Genders.Male ? raceTemplate.PaperDollHeadsMale : raceTemplate.PaperDollHeadsFemale;
                    }
                    else
                    {
                        // Get a child portrait
                        fileName = factionChildrenFaceFile;
                    }

                    // Allow individual factions from mods to use normal race faces
                    ImageData imageData = ImageReader.GetImageData(fileName, face.faceIndex, 0, true, true);
                    faceTexture = imageData.texture;
                    facePanel.Size = new Vector2(imageData.width, imageData.height);
                }

                // Must have a texture by now
                if (faceTexture == null)
                    throw new Exception("RefreshFaces() could not load face texture for Person resource.");

                // Add texture to list
                facePanel.BackgroundTexture = faceTexture;
                facePanels.Add(facePanel);
                Components.Add(facePanel);
            }
        }

        #endregion

        #region Serialization

        public FaceDetails[] GetSaveData()
        {
            return faces.ToArray();
        }

        public void RestoreSaveData(FaceDetails[] faces)
        {
            this.faces.Clear();
            this.faces.AddRange(faces);
            RefreshFaces();
        }

        #endregion

        #region Event Handlers

        private void QuestMachine_OnQuestEnded(Quest quest)
        {
            if (quest == null)
                return;

            // Remove any faces belonging to this quest
            faces.RemoveAll(face => face.questUID == quest.UID);

            RefreshFaces();
        }

        private void SaveLoadManager_OnStartLoad(Serialization.SaveData_v1 saveData)
        {
            faces.Clear();
            RefreshFaces();
        }

        private void StartGameBehaviour_OnNewGame()
        {
            faces.Clear();
            RefreshFaces();
        }

        #endregion
    }
}