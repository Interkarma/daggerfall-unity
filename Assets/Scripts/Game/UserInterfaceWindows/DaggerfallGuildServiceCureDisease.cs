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

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /**
     * Guild service UI for temple cure disease.
     */
    public class DaggerfallGuildServiceCureDisease : DaggerfallMessageBox, IMacroContextProvider
    {
        public const int NoDisease = 30;

        protected IGuild Guild { get; private set; }
        protected int BuildingFactionId { get; private set; }

        protected PlayerEntity playerEntity;

        protected int curingCost = 0;
        protected PlayerGPS.DiscoveredBuilding buildingDiscoveryData;

        public DaggerfallGuildServiceCureDisease(IUserInterfaceManager uiManager, int buildingFactionId, IGuild guild)
            : base(uiManager, uiManager.TopWindow)
        {
            Guild = guild;
            BuildingFactionId = buildingFactionId;  // Not used, provided for mods.

            playerEntity = GameManager.Instance.PlayerEntity;
            buildingDiscoveryData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;

            CureDiseaseService();
        }

        protected virtual TextFile.Token[] GetCureOfferTokens(int msgOffset)
        {
            return DaggerfallUnity.Instance.TextProvider.GetRandomTokens(DaggerfallTradeWindow.TradeMessageBaseId + msgOffset);
        }

        protected virtual void CureDiseaseService()
        {
            int numberOfDiseases = GameManager.Instance.PlayerEffectManager.DiseaseCount;

            if (playerEntity.TimeToBecomeVampireOrWerebeast != 0)
                numberOfDiseases++;

            // Check holidays for free / cheaper curing
            uint minutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            int holidayId = FormulaHelper.GetHolidayId(minutes, GameManager.Instance.PlayerGPS.CurrentRegionIndex);

            if (numberOfDiseases > 0 &&
                (holidayId == (int)DFLocation.Holidays.South_Winds_Prayer ||
                 holidayId == (int)DFLocation.Holidays.First_Harvest ||
                 holidayId == (int)DFLocation.Holidays.Second_Harvest))
            {
                // Just cure and inform player
                GameManager.Instance.PlayerEffectManager.CureAllDiseases();
                playerEntity.TimeToBecomeVampireOrWerebeast = 0;
                SetText(TextManager.Instance.GetLocalizedText("freeHolidayCuring"), this);
                ClickAnywhereToClose = true;
            }
            else if (numberOfDiseases > 0)
            {
                // Get base cost
                int baseCost = 250 * numberOfDiseases;

                // Apply rank-based discount if this is an Arkay temple and member
                baseCost = Guild.ReducedCureCost(baseCost);

                // Apply temple quality and regional price modifiers
                int costBeforeBargaining = FormulaHelper.CalculateCost(baseCost, buildingDiscoveryData.quality);

                // Halve the price on North Winds Prayer holiday
                if (holidayId == (int)DFLocation.Holidays.North_Winds_Festival)
                    costBeforeBargaining /= 2;

                // Apply bargaining to get final price
                curingCost = FormulaHelper.CalculateTradePrice(costBeforeBargaining, buildingDiscoveryData.quality, false);

                // Index correct message
                int msgOffset = 0;
                if (costBeforeBargaining >> 1 <= curingCost)
                {
                    if (costBeforeBargaining - (costBeforeBargaining >> 2) <= curingCost)
                        msgOffset = 2;
                    else
                        msgOffset = 1;
                }

                // Offer curing at the calculated price
                SetTextTokens(GetCureOfferTokens(msgOffset), this);
                AddButton(MessageBoxButtons.Yes);
                AddButton(MessageBoxButtons.No);
                OnButtonClick += ConfirmCuring_OnButtonClick;
            }
            else
            {   // Not diseased
                SetTextTokens(NoDisease, this);
                ClickAnywhereToClose = true;
            }
        }

        protected virtual void ConfirmCuring_OnButtonClick(DaggerfallMessageBox sender, MessageBoxButtons messageBoxButton)
        {
            CloseWindow();
            if (messageBoxButton == MessageBoxButtons.Yes)
            {
                if (playerEntity.GetGoldAmount() >= curingCost)
                {
                    playerEntity.DeductGoldAmount(curingCost);
                    GameManager.Instance.PlayerEffectManager.CureAllDiseases();
                    playerEntity.TimeToBecomeVampireOrWerebeast = 0;
                    DaggerfallUI.MessageBox(TextManager.Instance.GetLocalizedText("curedDisease"));
                }
                else
                    DaggerfallUI.MessageBox(DaggerfallTradeWindow.NotEnoughGoldId);
            }
        }


        #region Macro handling

        public MacroDataSource GetMacroDataSource()
        {
            return new GuildServiceCureMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for guild service cure UI.
        /// </summary>
        private class GuildServiceCureMacroDataSource : MacroDataSource
        {
            private DaggerfallGuildServiceCureDisease parent;
            public GuildServiceCureMacroDataSource(DaggerfallGuildServiceCureDisease guildServiceWindow)
            {
                this.parent = guildServiceWindow;
            }

            public override string Amount()
            {
                return parent.curingCost.ToString();
            }

            public override string God()
            {
                return Temple.GetDivineLocalized(parent.BuildingFactionId);
            }

            public override string GodDesc()
            {
                return Temple.GetDeityDesc((Temple.Divines)parent.BuildingFactionId);
            }

            public override string ShopName()
            {
                return parent.buildingDiscoveryData.displayName;
            }
        }

        #endregion

    }
}
