// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Allofich
// Contributors:
//
// Notes:
//

using UnityEngine;
using System;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;


namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements popup window for when player is in court for a crime.
    /// </summary>
    public class DaggerfallCourtWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "CORT01I0.IMG";
        const string nativeImgName2 = "PRIS00I0.IMG";
        const int courtTextTG = 550;
        const int courtTextDB = 551;
        const int courtTextStart = 8050;
        const int courtTextFoundGuilty = 8055;
        const int courtTextExecuted = 8060;
        const int courtTextFreeToGo = 8062;
        const int courtTextBanished = 8063;
        const int courtTextHowConvince = 8064;

        Texture2D nativeTexture;
        Panel courtPanel = new Panel();
        Entity.PlayerEntity playerEntity;
        TextLabel daysUntilFreedomLabel;
        int regionIndex;
        int punishmentType;
        int fine;
        int daysInPrison;
        int daysInPrisonLeft;
        int state;
        bool inPrison;
        bool repositionPlayer;

        float prisonUpdateTimer = 0f;
        float prisonUpdateInterval = 0.3f; // Approximated to classic based on measuring a video recording.

        // From FALL.EXE offset 0x1B34E0
        byte[] PenaltyPerLegalRepPoint  = {  0x05,  0x05,  0x06,  0x06,   0x0A,   0x05,  0x05,  0x03,  0x08,  0x08, 0x00,  0x06,  0x00, 0x00 };
        short[] BasePenaltyAmounts      = { 0x12C,  0xC8, 0x258, 0x3E8, 0x2710,   0xC8, 0x1F4,  0x64, 0x1F4, 0x1F4, 0x00,  0xC8,  0xC8, 0x00 };
        short[] MinimumPenaltyAmounts   = {  0x32,  0x0A,  0x50,  0x0A, 0x2328,   0x00,  0x0A,  0x02,  0x00,  0x00, 0x00,  0x05,  0x05, 0x00 };
        short[] MaximumPenaltyAmounts   = { 0x3E8, 0x320, 0x4B0, 0x5DC, 0x2EE0, 0x2EE0, 0x5DC, 0x2BC,  0x00,  0x00, 0x00, 0x3E8, 0x3E8, 0x00 };

        public int PunishmentType { get { return punishmentType; } }
        public int Fine { get { return fine; } }
        public int DaysInPrison { get { return daysInPrison; } }

        public DaggerfallCourtWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallCourtWindow: Could not load native texture.");

            // Native court panel
            courtPanel.HorizontalAlignment = HorizontalAlignment.Center;
            courtPanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            courtPanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(courtPanel);

            // Cancel any camera recoil
            RaiseOnCourtScreenEvent();

            // Add days until freedom label
            daysUntilFreedomLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(156, 165), string.Empty, NativePanel);
            daysUntilFreedomLabel.TextColor = DaggerfallUI.DaggerfallPrisonDaysUntilFreedomColor;
            daysUntilFreedomLabel.ShadowColor = DaggerfallUI.DaggerfallPrisonDaysUntilFreedomShadowColor;
            daysUntilFreedomLabel.HorizontalAlignment = HorizontalAlignment.Center;

            playerEntity = GameManager.Instance.PlayerEntity;
            AllowCancel = false;
        }

        public override void Update()
        {
            base.Update();

            if (state == 0) // Starting
            {
                regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;

                int crimeType = (int)playerEntity.CrimeCommitted - 1;
                int legalRep = playerEntity.RegionData[regionIndex].LegalRep;

                // Get whether punishment is normal (fine/prison) or a severe punishment (banishment/execution)
                int threshold1 = 0;
                int threshold2 = 0;

                if (legalRep < 0)
                {
                    threshold1 = -legalRep;
                    if (threshold1 > 75)
                        threshold1 = 75;
                    threshold2 = -legalRep / 2;
                    if (threshold2 > 75)
                        threshold2 = 75;
                }
                if (UnityEngine.Random.Range(1, 101) > threshold2 &&
                    UnityEngine.Random.Range(1, 101) > threshold1)
                    punishmentType = 2; // fine/prison
                else
                    punishmentType = 0; // banishment or execution

                // Calculate penalty amount
                int penaltyAmount = 0;

                if (legalRep >= 0)
                    penaltyAmount = PenaltyPerLegalRepPoint[crimeType] * legalRep
                    + BasePenaltyAmounts[crimeType];
                else
                    penaltyAmount = BasePenaltyAmounts[crimeType]
                    - PenaltyPerLegalRepPoint[crimeType] * legalRep;

                if (penaltyAmount > MaximumPenaltyAmounts[crimeType])
                    penaltyAmount = MaximumPenaltyAmounts[crimeType];
                else if (penaltyAmount < MinimumPenaltyAmounts[crimeType])
                    penaltyAmount = MinimumPenaltyAmounts[crimeType];

                penaltyAmount /= 40;

                // Calculate days of prison and fine
                daysInPrison = 0;
                fine = 0;

                for (int i = 0; i < penaltyAmount; i++)
                {
                    if ((DFRandom.rand() & 1) != 0)
                        fine += 40;
                    else
                        daysInPrison += 3;
                }

                // If player can't pay fine, limit fine and add to prison sentence
                int playerGold = playerEntity.GetGoldAmount();
                if (playerGold < fine)
                {
                    daysInPrison += (fine - playerGold) / 40;
                    fine = playerGold;
                }

                DaggerfallMessageBox messageBox;
                if (crimeType == 4 || crimeType == 3) // Assault or murder
                {
                    // If player is a member of the Dark Brotherhood, they may be rescued for a violent crime
                    Guilds.Guild guild = GameManager.Instance.GuildManager.GetGuild((int)FactionFile.FactionIDs.The_Dark_Brotherhood);
                    if (guild.IsMember())
                    {
                        if (guild.Rank >= UnityEngine.Random.Range(0, 20))
                        {
                            messageBox = new DaggerfallMessageBox(uiManager, this, false, 149);
                            messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextDB));
                            messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                            messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                            messageBox.ClickAnywhereToClose = true;
                            uiManager.PushWindow(messageBox);
                            playerEntity.FillVitalSigns();
                            playerEntity.RaiseReputationForDoingSentence();
                            state = 100;
                            return;
                        }
                    }
                }
                if (crimeType <= 2 || crimeType == 11) // Attempted breaking and entering, trespassing, breaking and entering, pickpocketing
                {
                    // If player is a member of the Thieves Guild, they may be rescued for a thieving crime
                    Guilds.Guild guild = GameManager.Instance.GuildManager.GetGuild((int)FactionFile.FactionIDs.The_Thieves_Guild);
                    if (guild.IsMember())
                    {
                        if (guild.Rank >= UnityEngine.Random.Range(0, 20))
                        {
                            messageBox = new DaggerfallMessageBox(uiManager, this, false, 149);
                            messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextTG));
                            messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                            messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                            messageBox.ClickAnywhereToClose = true;
                            messageBox.AllowCancel = false;
                            uiManager.PushWindow(messageBox);
                            playerEntity.FillVitalSigns();
                            playerEntity.RaiseReputationForDoingSentence();
                            state = 100;
                            return;
                        }
                    }
                }

                messageBox = new DaggerfallMessageBox(uiManager, this, false, 105);
                messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextStart));
                messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Guilty);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.NotGuilty);
                messageBox.OnButtonClick += GuiltyNotGuilty_OnButtonClick;
                messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                messageBox.AllowCancel = false;
                uiManager.PushWindow(messageBox);
                state = 1; // Done with initial message
            }
            else if (state == 2) // Found guilty
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this, false, 149);
                messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextFoundGuilty));
                messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                messageBox.ClickAnywhereToClose = true;
                messageBox.AllowCancel = false;
                uiManager.PushWindow(messageBox);

                if (daysInPrison > 0)
                    state = 3;
                else
                {
                    // Give the reputation raise here if no prison time will be served.
                    playerEntity.RaiseReputationForDoingSentence();
                    repositionPlayer = true;
                    playerEntity.FillVitalSigns();
                    ReleaseFromPrison();
                    state = 100;
                }
            }
            else if (state == 3) // Serve prison sentence
            {
                inPrison = true;
                SwitchToPrisonScreen();
                daysInPrisonLeft = daysInPrison;
                playerEntity.RaiseReputationForDoingSentence();
                repositionPlayer = true;
                state = 100;
            }
            else if (state == 4) // Banished
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this, false, 149);
                messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextBanished));
                messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                messageBox.ClickAnywhereToClose = true;
                messageBox.AllowCancel = false;
                uiManager.PushWindow(messageBox);
                playerEntity.RegionData[regionIndex].SeverePunishmentFlags |= 1;
                repositionPlayer = true;
                state = 100;
            }
            // Note: Seems like an execution sentence can't be given in classic. It can't be given here, either.
            else if (state == 5) // Execution
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this, false, 149);
                messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextExecuted));
                messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                messageBox.ClickAnywhereToClose = true;
                messageBox.AllowCancel = false;
                uiManager.PushWindow(messageBox);
                playerEntity.RegionData[regionIndex].SeverePunishmentFlags |= 2;
                state = 6;
            }
            else if (state == 6) // Reposition player at entrance
            {
                repositionPlayer = true;
                state = 100;
            }
            else if (state == 100) // Done
            {
                if (inPrison)
                {
                    if (Input.GetKey(exitKey)) // Speed up prison day countdown. Not in classic.
                        prisonUpdateInterval = 0.001f;
                    else
                        prisonUpdateInterval = 0.3f;

                    if (prisonUpdateTimer == 0)
                        prisonUpdateTimer = Time.realtimeSinceStartup;

                    if (Time.realtimeSinceStartup < prisonUpdateTimer + prisonUpdateInterval)
                        return;
                    else
                    {
                        prisonUpdateTimer = Time.realtimeSinceStartup;
                        UpdatePrisonScreen();
                    }
                }
                else
                    ReleaseFromPrison();
            }
        }

        private void GuiltyNotGuilty_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Guilty)
            {
                if (punishmentType != 0)
                {
                    if (punishmentType == 1)
                        state = 5;
                    else
                    {
                        fine >>= 1;
                        daysInPrison >>= 1;
                        playerEntity.DeductGoldAmount(fine);

                        // Classic gives a legal reputation raise here, probably a bug since it means you get a separate raise
                        // for paying the fine and for serving prison time.
                        if (daysInPrison > 0)
                            state = 3;
                        else
                        {
                            // Give the reputation raise here if no prison time will be served.
                            playerEntity.RaiseReputationForDoingSentence();
                            repositionPlayer = true;
                            // Oversight in classic: Does not refill vital signs when releasing in this case, so player is left with 1 health.
                            playerEntity.FillVitalSigns();
                            ReleaseFromPrison();
                        }
                    }
                }
                else
                    state = 4;
            }
            else // Pleading not guilty
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this, false, 127);
                messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextHowConvince));
                messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Debate);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Lie);
                messageBox.OnButtonClick += DebateLie_OnButtonClick;
                messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                messageBox.AllowCancel = false;
                uiManager.PushWindow(messageBox);
            }
            Update();
        }

        private void DebateLie_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            int playerSkill = 0;
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Debate)
            {
                playerSkill = playerEntity.Skills.GetLiveSkillValue(DFCareer.Skills.Etiquette);
                playerEntity.TallySkill(DFCareer.Skills.Etiquette, 1);
            }
            else
            {
                playerSkill = playerEntity.Skills.GetLiveSkillValue(DFCareer.Skills.Streetwise);
                playerEntity.TallySkill(DFCareer.Skills.Streetwise, 1);
            }

            int chanceToGoFree = playerEntity.RegionData[regionIndex].LegalRep +
                (playerSkill + playerEntity.Stats.GetLiveStatValue(DFCareer.Stats.Personality)) / 2;

            if (chanceToGoFree > 95)
                chanceToGoFree = 95;
            else if (chanceToGoFree < 5)
                chanceToGoFree = 5;

            if (UnityEngine.Random.Range(1, 101) > chanceToGoFree)
            {
                // Banishment
                if (punishmentType == 0)
                    state = 4;
                // Execution
                else if (punishmentType == 1)
                    state = 5;
                // Prison/Fine
                else
                {
                    int roll = playerEntity.RegionData[regionIndex].LegalRep + UnityEngine.Random.Range(1, 101);
                    if (roll < 25)
                        fine *= 2;
                    else if (roll > 75)
                        fine >>= 1;

                    state = 2;
                }
            }
            else
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this, false, 149);
                messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextFreeToGo));
                messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                messageBox.ClickAnywhereToClose = true;
                messageBox.AllowCancel = false;
                uiManager.PushWindow(messageBox);

                // Oversight in classic: Does not refill vital signs when releasing in this case, so player is left with 1 health.
                // Also does not repair reputation.
                playerEntity.FillVitalSigns();
                playerEntity.RaiseReputationForDoingSentence();
                state = 6;
            }
            Update();
        }

        public override void OnPop()
        {
            // Reset variables
            GameManager.Instance.PlayerEntity.Arrested = false;
            state = 0;
            prisonUpdateTimer = 0f;
            inPrison = false;
            repositionPlayer = false;
            daysUntilFreedomLabel.Text = string.Empty;

            // Return native texture to court screen
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallCourtWindow: Could not load native texture.");
            courtPanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            courtPanel.BackgroundTexture = nativeTexture;

            base.CancelWindow();
        }

        public void PositionPlayerAtLocationEntrance()
        {
            DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
            ContentReader.MapSummary mapSummary;
            if (DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
            {
                StreamingWorld world = GameManager.Instance.StreamingWorld;
                StreamingWorld.RepositionMethods reposition = StreamingWorld.RepositionMethods.None;
                reposition = StreamingWorld.RepositionMethods.RandomStartMarker;
                world.TeleportToCoordinates(mapPixel.X, mapPixel.Y, reposition);
            }
        }

        public void UpdatePrisonScreen()
        {
            daysInPrisonLeft--;
            daysUntilFreedomLabel.Text = HardStrings.daysUntilFreedom;
            daysUntilFreedomLabel.Text = daysUntilFreedomLabel.Text.Replace("%d", daysInPrisonLeft.ToString());

            if (daysInPrisonLeft == 0)
            {
                playerEntity.PreventEnemySpawns = true;
                playerEntity.PreventNormalizingReputations = true;
                DaggerfallUnity.WorldTime.DaggerfallDateTime.RaiseTime(daysInPrison * 1440 * 60);
                inPrison = false;
                playerEntity.FillVitalSigns();
            }
        }

        public void ReleaseFromPrison()
        {
            playerEntity.PreventEnemySpawns = true;
            DaggerfallUnity.WorldTime.DaggerfallDateTime.RaiseTime(240 * 60);
            playerEntity.CrimeCommitted = Entity.PlayerEntity.Crimes.None;
            if (repositionPlayer)
                PositionPlayerAtLocationEntrance();
            GameManager.Instance.ClearEnemies();
            CancelWindow();
        }

        // OnCourtSceen. Used for resetting camera recoil.
        public delegate void OnCourtScreenEventHandler();
        public static event OnCourtScreenEventHandler OnCourtScreen;
        protected virtual void RaiseOnCourtScreenEvent()
        {
            if (OnCourtScreen != null)
                OnCourtScreen();
        }

        private void SwitchToPrisonScreen()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName2);
            if (!nativeTexture)
                throw new Exception("DaggerfallCourtWindow: Could not load native texture.");

            // Native court panel
            courtPanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName2);
            courtPanel.BackgroundTexture = nativeTexture;

            daysUntilFreedomLabel.Text = HardStrings.daysUntilFreedom;
            daysUntilFreedomLabel.Text = daysUntilFreedomLabel.Text.Replace("%d", daysInPrison.ToString());
        }
    }
}
