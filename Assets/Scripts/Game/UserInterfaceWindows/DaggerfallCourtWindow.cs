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

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements popup window for when player is in court for a crime.
    /// </summary>
    public class DaggerfallCourtWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "CORT01I0.img";
        const int courtTextID1 = 8050;

        Texture2D nativeTexture;
        Panel courtPanel = new Panel();
        Entity.PlayerEntity playerEntity;
        int regionIndex;
        int punishmentType;
        int fine;
        int daysInPrison;
        int state;

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

            playerEntity = GameManager.Instance.PlayerEntity;
            state = 0;
        }

        public override void Update()
        {
            base.Update();

            if (state == 0)
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

                // TODO: Chance to free player if in Dark Brotherhood or Thieves Guild

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this, false, 105);
                messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(courtTextID1));
                messageBox.ScreenDimColor = new Color32(0, 0, 0, 0);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Guilty);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.NotGuilty);
                messageBox.OnButtonClick += GuiltyNotGuilty_OnButtonClick;
                messageBox.ParentPanel.VerticalAlignment = VerticalAlignment.Bottom;
                uiManager.PushWindow(messageBox);
                state = 1;
            }
        }

        private void GuiltyNotGuilty_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            DaggerfallUI.MessageBox("Not implemented yet. Press ESC to exit.");
        }

        public override void OnPop()
        {
            GameManager.Instance.PlayerEntity.Arrested = false;
            state = 0;
        }
    }
}
