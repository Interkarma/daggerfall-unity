// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Displays faces of quest NPCs escorted by player.
    /// Unlike Daggerfall will try to remove face when related quest ends, even if quest script forgets to drop face.
    /// </summary>
    public class EscortingNPCFacePanel : Panel
    {
        #region Fields

        const int maxFaces = 3;

        List<FaceDetails> faces = new List<FaceDetails>();
        List<Panel> facePanels = new List<Panel>();

        #endregion

        #region Constructors

        public EscortingNPCFacePanel()
            : base()
        {
            QuestMachine.OnQuestEnded += QuestMachine_OnQuestEnded;
            Serialization.SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            const int startX = 8;
            const int startY = 36;
            const int spaceY = 40;

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
                pos.y += spaceY;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a face to HUD.
        /// </summary>
        /// <param name="person">Symbol of target Person resource to add.</param>
        public void AddFace(Person person)
        {
            faces.Add(CreateFaceDetails(person));
            RefreshFaces();
        }

        /// <summary>
        /// Drops a face from HUD.
        /// </summary>
        /// <param name="person">Symbol of target Person resource to remove.</param>
        public void DropFace(Person person)
        {
            faces.Remove(CreateFaceDetails(person));
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
            face.faceIndex = person.FaceIndex;

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
                if (face.gender == Genders.Male)
                    faceTexture = DaggerfallUI.GetTextureFromCifRci(raceTemplate.PaperDollHeadsMale, face.faceIndex);
                else if (face.gender == Genders.Female)
                    faceTexture = DaggerfallUI.GetTextureFromCifRci(raceTemplate.PaperDollHeadsFemale, face.faceIndex);

                // Must have a texture by now
                if (faceTexture == null)
                    throw new Exception("RefreshFaces() could not load face texture for Person resource.");

                // Add texture to list
                Panel facePanel = new Panel();
                facePanel.Size = new Vector2(faceTexture.width, faceTexture.height);
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
            FaceDetails[] faceArray = faces.ToArray();
            for (int i = 0; i < faceArray.Length; i++)
            {
                // Remove from list if face belongs to ending quest
                if (faceArray[i].questUID == quest.UID)
                {
                    faces.Remove(faceArray[i]);
                }
            }

            RefreshFaces();
        }

        private void SaveLoadManager_OnStartLoad(Serialization.SaveData_v1 saveData)
        {
            faces.Clear();
            RefreshFaces();
        }

        #endregion
    }
}