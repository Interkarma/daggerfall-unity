// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using System;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /**
     * Guild service UI for temple donations.
     */
    public class DaggerfallGuildServiceDonation : DaggerfallInputMessageBox, IMacroContextProvider
    {
        public const int TooGenerousId = 702;
        public const int DonationThanksId = 703;

        // Based on the building faction id rather than guild object so it works for non-members as well
        protected int BuildingFactionId { get; private set; }

        protected PlayerEntity playerEntity;

        public DaggerfallGuildServiceDonation(IUserInterfaceManager uiManager, int buildingFactionId)
            : base(uiManager)
        {
            BuildingFactionId = buildingFactionId;

            playerEntity = GameManager.Instance.PlayerEntity;

            DonationService();
        }

        protected virtual void DonationService()
        {
            SetTextBoxLabel(TextManager.Instance.GetLocalizedText("serviceDonateHowMuch"));
            TextPanelDistanceX = 6;
            TextPanelDistanceY = 6;
            TextBox.Numeric = true;
            TextBox.MaxCharacters = 8;
            TextBox.Text = "1000";
            OnGotUserInput += DonationMsgBox_OnGotUserInput;
        }

        protected virtual void DonationMsgBox_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            int amount = 0;
            if (int.TryParse(input, out amount))
            {
                if (playerEntity.GetGoldAmount() >= amount)
                {
                    // Deduct gold, and apply blessing
                    playerEntity.DeductGoldAmount(amount);
                    int factionId = (int)Temple.GetDivine(BuildingFactionId);

                    // Change reputation
                    int rep = Math.Abs(playerEntity.FactionData.GetReputation(factionId));
                    if (Dice100.SuccessRoll((2 * amount / Math.Max(rep, 1)) + 1))
                        playerEntity.FactionData.ChangeReputation(factionId, 1); // Does not propagate in classic

                    // Show thanks message
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                    messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRandomTokens(DonationThanksId), this);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
                else
                {
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                    messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRandomTokens(TooGenerousId), this);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
            }
        }


        #region Macro handling

        public MacroDataSource GetMacroDataSource()
        {
            return new GuildServiceDonationMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for guild service donation UI.
        /// </summary>
        private class GuildServiceDonationMacroDataSource : MacroDataSource
        {
            private DaggerfallGuildServiceDonation parent;
            public GuildServiceDonationMacroDataSource(DaggerfallGuildServiceDonation guildServiceWindow)
            {
                this.parent = guildServiceWindow;
            }

            public override string God()
            {
                return Temple.GetDivineLocalized(parent.BuildingFactionId);
            }

            public override string GodDesc()
            {
                return Temple.GetDeityDesc((Temple.Divines)parent.BuildingFactionId);
            }
        }

        #endregion

    }
}
