// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Class for handling Daedra summoning.
    /// </summary>
    public class DaggerfallDaedraSummoning : UserInterfaceWindow, IMacroContextProvider
    {
        const int NotEnoughGoldId = 454;
        const int SummonNotToday = 480;
        const int SummonAreYouSure = 481;
        const int SummonBefore = 482;
        const int SummonFailed = 483;

        struct DaedraData
        {
            public readonly int factionId;
            public readonly int dayOfYear;
            public readonly string quest;
            public readonly string vidFile;

            public DaedraData(int factionId, string quest, int dayOfYear, string vidFile)
            {
                this.factionId = factionId;
                this.quest = quest;
                this.dayOfYear = dayOfYear;
                this.vidFile = vidFile;
            }
        }

        static DaedraData[] daedraData = new DaedraData[] {
            new DaedraData((int) FactionFile.FactionIDs.Hircine, "X0C00Y00", 155, "HIRCINE.FLC"),  // Restrict to only glenmoril witches?
            new DaedraData((int) FactionFile.FactionIDs.Clavicus_Vile, "V0C00Y00", 1, "CLAVICUS.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Mehrunes_Dagon, "Y0C00Y00", 320, "MEHRUNES.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Molag_Bal, "20C00Y00", 350, "MOLAGBAL.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Sanguine, "70C00Y00", 46, "SANGUINE.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Peryite, "50C00Y00", 99, "PERYITE.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Malacath, "80C00Y00", 278, "MALACATH.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Hermaeus_Mora, "W0C00Y00", 65, "HERMAEUS.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Sheogorath, "60C00Y00", 32, "SHEOGRTH.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Boethiah, "U0C00Y00", 302, "BOETHIAH.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Namira, "30C00Y00", 129, "NAMIRA.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Meridia, "10C00Y00", 13, "MERIDIA.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Vaernima, "90C00Y00", 190, "VAERNIMA.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Nocturnal, "40C00Y00", 248, "NOCTURNA.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Mephala, "Z0C00Y00", 283, "MEPHALA.FLC"),
            new DaedraData((int) FactionFile.FactionIDs.Azura, "T0C00Y00", 81, "AZURA.FLC"),
        };

        FactionFile.FactionData summonerFactionData;

        DaedraData daedraToSummon;

        public DaggerfallDaedraSummoning(IUserInterfaceManager uiManager, int npcFactionId)
            : base(uiManager)
        {
            if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npcFactionId, out summonerFactionData))
                DaedraSummoningService();
            else
                DaggerfallUnity.LogMessage("Error no faction data for NPC FactionId: " + npcFactionId);
        }

        #region Service Handling: Daedra Summoning

        void DaedraSummoningService()
        {
            // Select appropriate Daedra for summoning attempt.
            if (summonerFactionData.id == (int) FactionFile.FactionIDs.The_Glenmoril_Witches)
            {   // Always Hircine at Glenmoril witches.
                daedraToSummon = daedraData[0];
            }
            else if ((FactionFile.FactionTypes) summonerFactionData.type == FactionFile.FactionTypes.WitchesCoven)
            {   // Witches covens summon a random Daedra.
                daedraToSummon = daedraData[Random.Range(1, daedraData.Length)];
            }
            else
            {   // TODO - Sheogorath 5%/15% chance to replace
                int dayOfYear = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.DayOfYear;
                foreach (DaedraData dd in daedraData)
                {
                    if (dd.dayOfYear == dayOfYear)
                    {
                        daedraToSummon = dd;
                        break;
                    }
                }
            }
            // Display message.
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
            if (daedraToSummon.factionId == 0)
            {
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(SummonNotToday);
                messageBox.SetTextTokens(tokens, this);
                messageBox.ClickAnywhereToClose = true;
            }
            else
            {   // Ask player if they really want to risk the summoning.
                messageBox.SetTextTokens(SummonAreYouSure, this);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmSummon_OnButtonClick;
            }
            uiManager.PushWindow(messageBox);
        }

        private void ConfirmSummon_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                int summonCost = FormulaHelper.CalculateDaedraSummoningCost(summonerFactionData.rep);

                Debug.LogFormat("rep: {0}  cost: {1}", summonerFactionData.rep, summonCost);

                if (playerEntity.GetGoldAmount() >= summonCost)
                {
                    playerEntity.DeductGoldAmount(summonCost);

                    DaggerfallUI.MessageBox("summoning ");
                }
                else
                    DaggerfallUI.MessageBox(NotEnoughGoldId);
            }
        }

        #endregion

        #region Macro handling

        public virtual MacroDataSource GetMacroDataSource()
        {
            return new DaedraSummoningMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for quest popup windows.
        /// </summary>
        protected class DaedraSummoningMacroDataSource : MacroDataSource
        {
            private DaggerfallDaedraSummoning parent;
            public DaedraSummoningMacroDataSource(DaggerfallDaedraSummoning daedraSummoning)
            {
                this.parent = daedraSummoning;
            }

            public override string Daedra()
            {
                FactionFile.FactionData factionData;
                if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(parent.daedraToSummon.factionId, out factionData))
                    return factionData.name;
                else
                    return "%dae[error]";
            }
        }

        #endregion
    }
}