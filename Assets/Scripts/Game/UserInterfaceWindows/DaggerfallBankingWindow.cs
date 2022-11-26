// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    Hazelnut
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Utility;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class DaggerfallBankingWindow : DaggerfallPopupWindow, IMacroContextProvider
    {
        const string IMGNAME    = "BANK00I0.IMG";

        Panel mainPanel;

        TextLabel accountAmount;
        TextLabel inventoryAmount;
        TextLabel loanAmountDue;
        TextLabel loanDueBy;

        Button depoGoldButton;
        Button drawGoldButton;
        Button depoLOCButton;
        Button drawLOCButton;
        Button loanRepayButton;
        Button loanBorrowButton;
        Button buyHouseButton;
        Button sellHouseButton;
        Button buyShipButton;
        Button sellShipButton;
        Button exitButton;

        TextBox transactionInput;

        PlayerEntity playerEntity;
        TransactionType transactionType = TransactionType.None;

        public int regionIndex = 0;
        int amount = 0;

        public DaggerfallBankingWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            base.Setup();

            var background = DaggerfallUI.GetTextureFromImg(IMGNAME);
            if (background == null)
            {
                Debug.LogError(string.Format("Failed to load background image {0} for Banking pop-up", IMGNAME));
                CloseWindow();
                return;
            }

            ParentPanel.BackgroundColor = ScreenDimColor;

            mainPanel                       = DaggerfallUI.AddPanel(NativePanel, AutoSizeModes.None);
            mainPanel.BackgroundTexture     = background;
            mainPanel.Size                  = new Vector2(225, 181);
            mainPanel.HorizontalAlignment   = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment     = VerticalAlignment.Middle;

            accountAmount               = new TextLabel();
            accountAmount.Position      = new Vector2(150, 14);
            accountAmount.Size          = new Vector2(60, 13);
            accountAmount.Name          = "accnt_total_label";
            accountAmount.MaxCharacters = 13;
            mainPanel.Components.Add(accountAmount);

            inventoryAmount                 = new TextLabel();
            inventoryAmount.Position        = new Vector2(156, 24);
            inventoryAmount.Size            = new Vector2(64, 13);
            inventoryAmount.Name            = "inv_total_label";
            inventoryAmount.MaxCharacters   = 11;
            mainPanel.Components.Add(inventoryAmount);

            loanAmountDue               = new TextLabel();
            loanAmountDue.Position      = new Vector2(96, 34);
            loanAmountDue.Size          = new Vector2(60, 13);
            loanAmountDue.Name          = "amount_due_label";
            loanAmountDue.MaxCharacters = 24;
            mainPanel.Components.Add(loanAmountDue);

            loanDueBy               = new TextLabel();
            loanDueBy.Position      = new Vector2(71, 44);
            loanDueBy.Size          = new Vector2(60, 13);
            loanDueBy.Name          = "loan_by_label";
            loanDueBy.MaxCharacters = 33;
            mainPanel.Components.Add(loanDueBy);

            depoGoldButton          = new Button();
            depoGoldButton.Position = new Vector2(120, 58);
            depoGoldButton.Size     = new Vector2(45, 8);
            depoGoldButton.Name     = "depo_gold_button";
            depoGoldButton.OnMouseClick += DepoGoldButton_OnMouseClick;
            mainPanel.Components.Add(depoGoldButton);

            drawGoldButton          = new Button();
            drawGoldButton.Position = new Vector2(172, 58);
            drawGoldButton.Size     = new Vector2(45, 8);
            drawGoldButton.Name     = "draw_gold_button";
            drawGoldButton.OnMouseClick += DrawGoldButton_OnMouseClick;
            mainPanel.Components.Add(drawGoldButton);

            depoLOCButton           = new Button();
            depoLOCButton.Position  = new Vector2(120, 76);
            depoLOCButton.Size      = new Vector2(45, 8);
            depoLOCButton.Name      = "depo_loc_button";
            depoLOCButton.OnMouseClick += DepoLOCButton_OnMouseClick;
            mainPanel.Components.Add(depoLOCButton);

            drawLOCButton           = new Button();
            drawLOCButton.Position  = new Vector2(172, 76);
            drawLOCButton.Size      = new Vector2(45, 8);
            drawLOCButton.Name      = "draw_LOC_button";
            drawLOCButton.OnMouseClick += DrawLOCButton_OnMouseClick;
            mainPanel.Components.Add(drawLOCButton);

            loanRepayButton         = new Button();
            loanRepayButton.Position= new Vector2(120, 94);
            loanRepayButton.Size    = new Vector2(45, 8);
            loanRepayButton.Name    = "loan_repay_button";
            loanRepayButton.OnMouseClick += LoanRepayButton_OnMouseClick;
            mainPanel.Components.Add(loanRepayButton);

            loanBorrowButton        = new Button();
            loanBorrowButton.Position = new Vector2(172, 94);
            loanBorrowButton.Size   = new Vector2(45, 8);
            loanBorrowButton.Name   = "loan_borrow_button";
            loanBorrowButton.OnMouseClick += LoanBorrowButton_OnMouseClick;
            mainPanel.Components.Add(loanBorrowButton);

            buyHouseButton          = new Button();
            buyHouseButton.Position = new Vector2(120, 112);
            buyHouseButton.Size     = new Vector2(45, 8);
            buyHouseButton.Name     = "buy_house_button";
            buyHouseButton.OnMouseClick += BuyHouseButton_OnMouseClick;
            mainPanel.Components.Add(buyHouseButton);

            sellHouseButton         = new Button();
            sellHouseButton.Position = new Vector2(172, 112);
            sellHouseButton.Size    = new Vector2(45, 8);
            sellHouseButton.Name    = "sell_house_button";
            sellHouseButton.OnMouseClick += SellHouseButton_OnMouseClick;
            mainPanel.Components.Add(sellHouseButton);

            buyShipButton           = new Button();
            buyShipButton.Position  = new Vector2(120, 130);
            buyShipButton.Size      = new Vector2(45, 8);
            buyShipButton.Name      = "buy_ship_button";
            buyShipButton.OnMouseClick += BuyShipButton_OnMouseClick;
            mainPanel.Components.Add(buyShipButton);

            sellShipButton          = new Button();
            sellShipButton.Position = new Vector2(172, 130);
            sellShipButton.Size     = new Vector2(45, 8);
            sellShipButton.Name     = "sell_ship_button";
            sellShipButton.OnMouseClick += SellShipButton_OnMouseClick;
            mainPanel.Components.Add(sellShipButton);

            exitButton              = new Button();
            exitButton.Position     = new Vector2(92, 159);
            exitButton.Size         = new Vector2(40, 19);
            exitButton.Name         = "exit_button";
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            mainPanel.Components.Add(exitButton);

            transactionInput           = new TextBox();
            transactionInput.Position  = new Vector2(113, 146);
            transactionInput.Size      = new Vector2(103, 12);
            transactionInput.Numeric   = true;
            transactionInput.Enabled   = false;
            transactionInput.MaxCharacters = 9;
            mainPanel.Components.Add(transactionInput);

            playerEntity    = GameManager.Instance.PlayerEntity;
            regionIndex     = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            transactionType = TransactionType.None;
            UpdateLabels();
        }

        public override void OnPush()
        {
            base.OnPush();
            DaggerfallBankManager.OnTransaction += this.OnTransactionEventHandler;
            regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
        }

        public override void OnPop()
        {
            base.OnPop();
            DaggerfallBankManager.OnTransaction -= this.OnTransactionEventHandler;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                if (transactionType != TransactionType.None && transactionInput.Enabled)
                {
                    HandleTransactionInput();
                    ToggleTransactionInput(TransactionType.None);
                }
            }

            UpdateButtons();
            UpdateLabels();
        }

        public void GeneratePurchaseShipPopup(ShipType shipType)
        {
            GeneratePopup(DaggerfallBankManager.PurchaseShip(shipType, regionIndex));
        }

        public void GeneratePurchaseHousePopup(BuildingSummary house)
        {
            GeneratePopup(DaggerfallBankManager.PurchaseHouse(house, regionIndex));
        }

        void UpdateLabels()
        {
            inventoryAmount.Text    = playerEntity.GetGoldAmount().ToString();
            if (playerEntity.WagonItems.Contains(ItemGroups.Currency, (int)Currency.Gold_pieces))
            {
                int wagonGold = playerEntity.WagonItems.GetItem(ItemGroups.Currency, (int)Currency.Gold_pieces).stackCount;
                inventoryAmount.Text += " (+" + wagonGold + ")";
            }
            accountAmount.Text      = DaggerfallBankManager.GetAccountTotal(regionIndex).ToString();
            loanAmountDue.Text      = DaggerfallBankManager.GetLoanedTotal(regionIndex).ToString();
            loanDueBy.Text          = DaggerfallBankManager.GetLoanDueDateString(regionIndex);
        }

        void UpdateButtons()
        {
            depoGoldButton.Enabled  = transactionType == TransactionType.None;
            drawGoldButton.Enabled  = transactionType == TransactionType.None;
            depoLOCButton.Enabled   = transactionType == TransactionType.None;
            drawLOCButton.Enabled   = transactionType == TransactionType.None;
            depoLOCButton.Enabled   = transactionType == TransactionType.None;
            loanBorrowButton.Enabled = transactionType == TransactionType.None;
            loanRepayButton.Enabled = (transactionType == TransactionType.None && DaggerfallBankManager.HasLoan(regionIndex));
            buyHouseButton.Enabled  = transactionType == TransactionType.None;
            sellHouseButton.Enabled = transactionType == TransactionType.None;
            buyShipButton.Enabled   = transactionType == TransactionType.None;
            sellShipButton.Enabled  = transactionType == TransactionType.None;
        }


        void ToggleTransactionInput(TransactionType newType)
        {
            if (transactionType == newType)
                return;
            else if (transactionType != TransactionType.None && newType != TransactionType.None)
                return;

            transactionType = newType;
            transactionInput.Text = "";
            transactionInput.Enabled = (transactionType != TransactionType.None);
        }

        void HandleTransactionInput()
        {
            int amount = 0;

            if (string.IsNullOrEmpty(transactionInput.Text))
                return;
            else if (!System.Int32.TryParse(transactionInput.Text, out amount))
            {
                Debug.LogError("Failed to parse input");
                return;
            }
            else if (amount < 1)
                return;
            else
                DaggerfallBankManager.MakeTransaction(transactionType, amount, regionIndex);
        }

        //generates pop-ups, either to indicate failed transaction
        //or to prompt with yes / no option
        void GeneratePopup(TransactionResult result, int amount = 0)
        {
            if (result == TransactionResult.NONE)
                return;

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.ClickAnywhereToClose = true;
            this.amount = amount;

            if (result == TransactionResult.TOO_HEAVY)
                messageBox.SetText(TextManager.Instance.GetLocalizedText("cannotCarryGold"));
            else
                messageBox.SetTextTokens((int)result, this);

            if (result == TransactionResult.DEPOSIT_LOC) //show messagebox window w/ yes no buttons
            {
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.ClickAnywhereToClose = false;
                messageBox.OnButtonClick += DepositLOC_messageBox_OnButtonClick;
            }

            else if (result == TransactionResult.SELL_HOUSE_OFFER) //show messagebox window w/ yes no buttons
            {
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.ClickAnywhereToClose = false;
                messageBox.OnButtonClick += SellHouse_messageBox_OnButtonClick;
            }
            else if (result == TransactionResult.SELL_SHIP_OFFER)
            {
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.ClickAnywhereToClose = false;
                messageBox.OnButtonClick += SellShip_messageBox_OnButtonClick;
            }

            messageBox.Show();
        }


        #region event handlers

        //handles button clicks from Deposit LOC message box
        void DepositLOC_messageBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                DaggerfallBankManager.MakeTransaction(TransactionType.Depositing_LOC, 0, regionIndex);

            sender.CloseWindow();
        }

        //handles button clicks from Sell house message box
        void SellHouse_messageBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                DaggerfallBankManager.MakeTransaction(TransactionType.Sell_house, 0, regionIndex);

            sender.CloseWindow();
        }

        //handles button clicks from Sell ship message box
        void SellShip_messageBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                DaggerfallBankManager.MakeTransaction(TransactionType.Sell_ship, 0, regionIndex);
            sender.CloseWindow();
        }

        //bank window button handlers
        void DepoGoldButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ToggleTransactionInput(TransactionType.Depositing_gold);
        }

        void DrawGoldButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ToggleTransactionInput(TransactionType.Withdrawing_gold);
        }

        void DepoLOCButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            GeneratePopup(TransactionResult.DEPOSIT_LOC);
        }

        void DrawLOCButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ToggleTransactionInput(TransactionType.Withdrawing_Letter);
        }

        void LoanRepayButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ToggleTransactionInput(TransactionType.Repaying_loan);
        }

        void LoanBorrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (DaggerfallBankManager.HasDefaulted(regionIndex))
            {
                GeneratePopup(TransactionResult.ALREADY_DEFAULTED);
                ToggleTransactionInput(TransactionType.None);
            }
            else if (DaggerfallBankManager.HasLoan(regionIndex))
            {
                GeneratePopup(TransactionResult.ALREADY_HAVE_LOAN);
                ToggleTransactionInput(TransactionType.None);
            }
            else
            {
                ToggleTransactionInput(TransactionType.Borrowing_loan);
            }
        }

        void BuyHouseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (DaggerfallBankManager.OwnsHouse)
                GeneratePopup(TransactionResult.ALREADY_OWN_HOUSE);
            else
            {
                BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
                if (buildingDirectory)
                {
                    List<BuildingSummary> housesForSale = buildingDirectory.GetHousesForSale();
                    // If houses are for sale, show them
                    if (housesForSale.Count > 0)
                        uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.BankPurchasePopup, new object[] { uiManager, this, housesForSale }));
                    else
                        GeneratePopup(TransactionResult.NO_HOUSES_FOR_SALE);
                }
                else
                    GeneratePopup(TransactionResult.NO_HOUSES_FOR_SALE);
            }
        }

        void SellHouseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (DaggerfallBankManager.OwnsHouse)
            {
                BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
                if (buildingDirectory)
                {
                    BuildingSummary house;
                    if (buildingDirectory.GetBuildingSummary(DaggerfallBankManager.OwnedHouseKey, out house))
                        GeneratePopup(TransactionResult.SELL_HOUSE_OFFER, DaggerfallBankManager.GetHouseSellPrice(house));
                }
            }
        }

        void BuyShipButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (DaggerfallBankManager.OwnsShip)
                GeneratePopup(TransactionResult.ALREADY_OWN_SHIP);
            else if (GameManager.Instance.PlayerGPS.CurrentLocation.Exterior.ExteriorData.PortTownAndUnknown == 0)
                GeneratePopup(TransactionResult.NOT_PORT_TOWN);
            else    // Show ships for sale
                uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.BankPurchasePopup, new object[] { uiManager, this, null }));
        }

        void SellShipButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (DaggerfallBankManager.OwnsShip)
                GeneratePopup(TransactionResult.SELL_SHIP_OFFER, DaggerfallBankManager.GetShipSellPrice(DaggerfallBankManager.OwnedShip));
        }

        void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (!transactionInput.Enabled)
                CloseWindow();
        }

        public void OnTransactionEventHandler(TransactionType type, TransactionResult result, int amount)
        {
            GeneratePopup(result, amount);
        }

        #endregion

        #region Macro handling

        public MacroDataSource GetMacroDataSource()
        {
            return new BankingMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for banking window.
        /// </summary>
        private class BankingMacroDataSource : MacroDataSource
        {
            private DaggerfallBankingWindow parent;
            public BankingMacroDataSource(DaggerfallBankingWindow bankingWindow)
            {
                this.parent = bankingWindow;
            }

            public override string Amount()
            {
                return parent.amount.ToString();
            }
            public override string MaxLoan()
            {
                return FormulaHelper.CalculateMaxBankLoan().ToString();
            }

        }

        #endregion

        #region banking status box

        public static DaggerfallMessageBox CreateBankingStatusBox(IUserInterfaceWindow previous = null)
        {
            DaggerfallMessageBox bankingBox = new DaggerfallMessageBox(DaggerfallUI.Instance.UserInterfaceManager, previous);
            bankingBox.SetHighlightColor(DaggerfallUI.DaggerfallUnityStatDrainedTextColor);
            List<TextFile.Token> messages = new List<TextFile.Token>();
            bool found = false;
            messages.AddRange(GetLoansLine(
                TextManager.Instance.GetLocalizedText("region"),
                TextManager.Instance.GetLocalizedText("account"),
                TextManager.Instance.GetLocalizedText("loan"),
                TextManager.Instance.GetLocalizedText("dueDate")));
            messages.Add(TextFile.NewLineToken);
            for (int regionIndex = 0; regionIndex < DaggerfallBankManager.BankAccounts.Length; regionIndex++)
            {
                if (DaggerfallBankManager.GetAccountTotal(regionIndex) > 0 || DaggerfallBankManager.HasLoan(regionIndex))
                {
                    TextFile.Formatting formatting = DaggerfallBankManager.HasDefaulted(regionIndex) ? TextFile.Formatting.TextHighlight : TextFile.Formatting.Text;
                    messages.AddRange(GetLoansLine(ShortenName(TextManager.Instance.GetLocalizedRegionName(regionIndex), 12), DaggerfallBankManager.GetAccountTotal(regionIndex).ToString(), DaggerfallBankManager.GetLoanedTotal(regionIndex).ToString(), DaggerfallBankManager.GetLoanDueDateString(regionIndex), formatting));
                    found = true;
                }
            }
            if (!found)
            {
                TextFile.Token noneToken = TextFile.CreateTextToken(TextManager.Instance.GetLocalizedText("noAccount"));
                messages.Add(noneToken);
                messages.Add(TextFile.NewLineToken);
            }
            bankingBox.SetTextTokens(messages.ToArray());
            bankingBox.ClickAnywhereToClose = true;
            return bankingBox;
        }

        private static string ShortenName(string name, int maxLength)
        {
            if (name.Length <= maxLength)
                return name;
            return name.Substring(0, maxLength - 1) + "...";
        }

        private static List<TextFile.Token> GetLoansLine(string region, string account, string loan, string duedate, TextFile.Formatting formatting = TextFile.Formatting.Text)
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();

            TextFile.Token positioningToken = TextFile.TabToken;

            tokens.Add(new TextFile.Token(formatting, region));
            positioningToken.x = 60;
            tokens.Add(positioningToken);
            tokens.Add(new TextFile.Token(formatting, account));
            positioningToken.x = 120;
            tokens.Add(positioningToken);
            tokens.Add(new TextFile.Token(formatting, loan));
            positioningToken.x = 180;
            tokens.Add(positioningToken);
            tokens.Add(new TextFile.Token(formatting, duedate));
            tokens.Add(TextFile.NewLineToken);
            return tokens;
        }

        #endregion
    }
}
