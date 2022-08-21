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

using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Teleport
    /// </summary>
    public class Teleport : BaseEntityEffect
    {
        public static readonly string EffectKey = "Teleport-Effect";

        #region Fields

        // Constants
        const int teleportOrSetAnchor = 4000;
        const int achorMustBeSet = 4001;

        // Anchor is now stored in PlayerEntity
        // Below field is maintained for backwards compatibility with old save files
        // If non-null this anchor data will be migrated to PlayerEntity on load and the effect will end
        PlayerPositionData_v1 anchorPosition;

        // Volatile references
        SerializablePlayer serializablePlayer = null;
        PlayerEnterExit playerEnterExit = null;

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(43, 255);
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.ShowSpellIcon = false;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("teleport");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1602);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1302);

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            CacheReferences();
            PromptPlayer();
        }

        public override void End()
        {
            anchorPosition = null;
            RoundsRemaining = 0;
            base.End();
        }

        #endregion

        #region Private Methods

        void PromptPlayer()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Target must be player - no effect on other entities
            if (entityBehaviour != GameManager.Instance.PlayerEntityBehaviour)
                return;

            // Prompt for outcome
            DaggerfallMessageBox mb = new DaggerfallMessageBox(DaggerfallUI.Instance.UserInterfaceManager, DaggerfallMessageBox.CommonMessageBoxButtons.AnchorTeleport, teleportOrSetAnchor, DaggerfallUI.Instance.UserInterfaceManager.TopWindow);
            // QoL, does not match classic. No magicka refund, though
            mb.AllowCancel = true;
            mb.OnButtonClick += EffectActionPrompt_OnButtonClick;
            mb.Show();
        }

        void SetAnchor()
        {
            // Validate references
            if (!serializablePlayer || !playerEnterExit)
                return;

            // Get position information
            PlayerPositionData_v1 newAnchorPosition = serializablePlayer.GetPlayerPositionData();
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                newAnchorPosition.exteriorDoors = playerEnterExit.ExteriorDoors;
                newAnchorPosition.buildingDiscoveryData = playerEnterExit.BuildingDiscoveryData;
            }

            // Assign anchor to player entity then end effect
            GameManager.Instance.PlayerEntity.AnchorPosition = newAnchorPosition;
            End();
        }

        void TeleportPlayer()
        {
            // Validate references
            if (!serializablePlayer || !playerEnterExit)
                return;

            // Get anchor position from player entity
            anchorPosition = GameManager.Instance.PlayerEntity.AnchorPosition;

            // Is player in same interior as anchor?
            if (IsSameInterior())
            {
                // Just need to move player
                serializablePlayer.RestorePosition(anchorPosition);
                GameManager.Instance.PlayerEntity.AnchorPosition = null;
            }
            else
            {
                // When teleporting to interior anchor, restore world compensation height early before initworld
                // Ensures exterior world level is aligned with building height at time of anchor
                // Only works with floating origin v3 saves and above with both serialized world compensation and context
                if (anchorPosition.worldContext == WorldContext.Interior)
                    GameManager.Instance.StreamingWorld.RestoreWorldCompensationHeight(anchorPosition.worldCompensation.y);
                else
                    GameManager.Instance.StreamingWorld.RestoreWorldCompensationHeight(0);

                // Cache scene before departing
                if (!playerEnterExit.IsPlayerInside)
                    SaveLoadManager.CacheScene(GameManager.Instance.StreamingWorld.SceneName);      // Player is outside
                else if (playerEnterExit.IsPlayerInsideBuilding)
                    SaveLoadManager.CacheScene(playerEnterExit.Interior.name);                      // Player inside a building
                else // Player inside a dungeon
                    playerEnterExit.TransitionDungeonExteriorImmediate();

                // Need to load some other part of the world again - player could be anywhere
                PlayerEnterExit.OnRespawnerComplete += PlayerEnterExit_OnRespawnerComplete;
                playerEnterExit.RestorePositionHelper(anchorPosition, false, true);

                // Restore building summary data
                if (anchorPosition.insideBuilding)
                    playerEnterExit.BuildingDiscoveryData = anchorPosition.buildingDiscoveryData;

                // When moving anywhere other than same interior trigger a fade so transition appears smoother
                DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack();
            }
        }

        #endregion

        #region Helpers

        // Cache required references
        bool CacheReferences()
        {
            // Get peered SerializablePlayer and PlayerEnterExit
            if (!serializablePlayer)
                serializablePlayer = caster.GetComponent<SerializablePlayer>();

            if (!playerEnterExit)
                playerEnterExit = caster.GetComponent<PlayerEnterExit>();

            if (!serializablePlayer || !playerEnterExit)
            {
                Debug.LogError("Teleport effect could not find both SerializablePlayer and PlayerEnterExit components.");
                return false;
            }

            return true;
        }

        // Checks if player is in same building or dungeon interior as anchor
        bool IsSameInterior()
        {
            // Reject if outside or anchor not set
            if (!playerEnterExit.IsPlayerInside || anchorPosition == null)
                return false;

            // Test depends on if player is inside a building or a dungeon
            if (playerEnterExit.IsPlayerInsideBuilding && anchorPosition.insideBuilding)
            {
                // Compare building key
                if (anchorPosition.buildingDiscoveryData.buildingKey == playerEnterExit.BuildingDiscoveryData.buildingKey)
                {
                    // Also compare map pixel, in case we're unlucky https://forums.dfworkshop.net/viewtopic.php?f=24&t=2018
                    DaggerfallConnect.Utility.DFPosition anchorMapPixel = DaggerfallConnect.Arena2.MapsFile.WorldCoordToMapPixel(anchorPosition.worldPosX, anchorPosition.worldPosZ);
                    DaggerfallConnect.Utility.DFPosition playerMapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
                    if (anchorMapPixel.X == playerMapPixel.X && anchorMapPixel.Y == playerMapPixel.Y)
                        return true;
                }
            }
            else if (playerEnterExit.IsPlayerInsideDungeon && anchorPosition.insideDungeon)
            {
                // Compare map pixel of dungeon (only one dungeon per map pixel allowed)
                DaggerfallConnect.Utility.DFPosition anchorMapPixel = DaggerfallConnect.Arena2.MapsFile.WorldCoordToMapPixel(anchorPosition.worldPosX, anchorPosition.worldPosZ);
                DaggerfallConnect.Utility.DFPosition playerMapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
                if (anchorMapPixel.X == playerMapPixel.X && anchorMapPixel.Y == playerMapPixel.Y)
                {
                    GameManager.Instance.PlayerEnterExit.PlayerTeleportedIntoDungeon = true;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Event Handlers

        private void PlayerEnterExit_OnRespawnerComplete()
        {
            // Must have a caster and it must be the player
            if (caster == null || caster != GameManager.Instance.PlayerEntityBehaviour)
                return;

            // Get peered SerializablePlayer and PlayerEnterExit if they haven't been cached yet
            if (!CacheReferences())
                return;

            // Get anchor position from player entity
            anchorPosition = GameManager.Instance.PlayerEntity.AnchorPosition;

            // Restore final position and unwire event
            serializablePlayer.RestorePosition(anchorPosition);
            PlayerEnterExit.OnRespawnerComplete -= PlayerEnterExit_OnRespawnerComplete;

            // Set "teleported into dungeon" flag when anchor is inside a dungeon
            GameManager.Instance.PlayerEnterExit.PlayerTeleportedIntoDungeon = anchorPosition.insideDungeon;

            // Restore scene cache on arrival
            if (!playerEnterExit.IsPlayerInside)
                SaveLoadManager.RestoreCachedScene(GameManager.Instance.StreamingWorld.SceneName);      // Player is outside
            else if (playerEnterExit.IsPlayerInsideBuilding)
                SaveLoadManager.RestoreCachedScene(playerEnterExit.Interior.name);                      // Player inside a building

            GameManager.Instance.PlayerEntity.AnchorPosition = null;
            End();
        }

        private void EffectActionPrompt_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();

            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Anchor)
            {
                SetAnchor();
            }
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Teleport)
            {
                if (GameManager.Instance.PlayerEntity.AnchorPosition == null)
                {
                    DaggerfallMessageBox mb = new DaggerfallMessageBox(DaggerfallUI.Instance.UserInterfaceManager, DaggerfallUI.Instance.UserInterfaceManager.TopWindow);
                    mb.SetTextTokens(achorMustBeSet);
                    mb.ClickAnywhereToClose = true;
                    mb.Show();
                    return;
                }
                TeleportPlayer();
            }
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public bool anchorSet;
            public PlayerPositionData_v1 anchorPosition;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.anchorPosition = anchorPosition;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            anchorPosition = data.anchorPosition;

            // On resume migrate legacy anchor position to player entity then end effect
            if (anchorPosition != null)
            {
                GameManager.Instance.PlayerEntity.AnchorPosition = anchorPosition;
                End();
            }
        }

        #endregion
    }
}
