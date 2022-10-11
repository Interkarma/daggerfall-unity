using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallConnect.Arena2;

public class _new_game
{

    [UnityTest]
    public static IEnumerator game_starts ()
    {
        SceneManager.LoadScene( SceneControl.StartupSceneIndex );
        yield return null;
        
        SceneManager.LoadScene( SceneControl.GameSceneIndex );
        yield return null;

        //
        // code based on StartGameBehaviour::StartNewCharacter()
        //

        GameManager.Instance.PauseGame(true);

        StartGameBehaviour sgb = GameObject.FindObjectOfType<StartGameBehaviour>();
        sgb.StartMethod = StartGameBehaviour.StartMethods.NewCharacter;

        // Assign character sheet
        // NewCharacterCleanup();
        CharacterDocument characterDocument = new CharacterDocument();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        DaggerfallEntityBehaviour dfe = player.GetComponent<DaggerfallEntityBehaviour>();
        PlayerEntity playerEntity = (PlayerEntity) dfe.Entity;
        playerEntity.AssignCharacter( characterDocument );

        // Set game time
        DaggerfallUnity.Instance.WorldTime.Now.SetClassicGameStartTime();

        // Set time tracked in playerEntity
        playerEntity.LastGameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

        // Get start parameters
        DFPosition mapPixel = new DFPosition( DaggerfallUnity.Settings.StartCellX , DaggerfallUnity.Settings.StartCellY );

        // Read location if any
        DFLocation location = default;
        bool hasLocation = DaggerfallUnity.Instance.ContentReader.HasLocation( mapPixel.X , mapPixel.Y , out var mapSummary );
        if (hasLocation)
        {
            if (!DaggerfallUnity.Instance.ContentReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex, out location))
                hasLocation = false;
        }

        PlayerEnterExit playerEnterExit = player.GetComponent<PlayerEnterExit>();
        
        // Start at specified location
        StreamingWorld streamingWorld = GameObject.FindObjectOfType<StreamingWorld>();
        if (hasLocation && DaggerfallUnity.Settings.StartInDungeon && location.HasDungeon)
        {
            if (streamingWorld)
            {
                streamingWorld.TeleportToCoordinates(mapPixel.X, mapPixel.Y);
                streamingWorld.suppressWorld = true;
            }
            playerEnterExit.EnableDungeonParent();
            playerEnterExit.StartDungeonInterior(location);
        }
        else
        {
            playerEnterExit.EnableExteriorParent();
            if (streamingWorld)
            {
                streamingWorld.TeleportToCoordinates(mapPixel.X, mapPixel.Y);
                streamingWorld.SetAutoReposition(StreamingWorld.RepositionMethods.Origin, Vector3.zero);
                streamingWorld.suppressWorld = false;
            }
        }

        // Apply biography effects to player entity
        BiogFile.ApplyEffects(characterDocument.biographyEffects, playerEntity);

        // Assign starting gear to player entity
        sgb.AssignStartingEquipment(playerEntity, characterDocument);
        
        // Assign starting spells to player entity
        sgb.AssignStartingSpells(playerEntity, characterDocument);

        // Assign starting level up skill sum
        playerEntity.SetCurrentLevelUpSkillSum();
        playerEntity.StartingLevelUpSkillSum = playerEntity.CurrentLevelUpSkillSum;

        // Setup bank accounts and houses
        DaggerfallBankManager.SetupAccounts();
        DaggerfallBankManager.SetupHouses();

        // Initialize region data
        playerEntity.InitializeRegionData();

        // Randomize weathers
        GameManager.Instance.WeatherManager.SetClimateWeathers();

        // Start game
        GameManager.Instance.PauseGame(false);
        DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack();
        DaggerfallUI.PostMessage( sgb.PostStartMessage );

        // lastStartMethod = StartMethods.NewCharacter;

        // Start main quest
        QuestMachine.Instance.StartQuest("_TUTOR__");
        QuestMachine.Instance.StartQuest("_BRISIEN");

        // Launch startup optional quest
        if( !string.IsNullOrEmpty(sgb.LaunchQuest) )
        {
            QuestMachine.Instance.StartQuest( sgb.LaunchQuest );
            sgb.LaunchQuest = string.Empty;
        }

        // Launch any InitAtGameStart quests
        GameManager.Instance.QuestListsManager.InitAtGameStartQuests();
        
        if (StartGameBehaviour.OnStartGame != null)
            StartGameBehaviour.OnStartGame(sgb, null);

        for( int i=0 ; i<10 ; i++ )
            yield return null;
    }

}
