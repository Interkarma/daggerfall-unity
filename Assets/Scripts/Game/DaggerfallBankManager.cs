// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.Utility;

/*
 * Todo
 * Depositing / withdrawing LOC -DONE
 * buying & selling ships/houses
 * events
*/

namespace DaggerfallWorkshop.Game.Banking
{
    //Banking dialogue
    //0282-0299
    public enum TransactionResult
    {
        NONE                    = 0,
        TOO_HEAVY               = 1,
        PURCHASED_HOUSE         = 0282,
        PURCHASED_SHIP          = 0283,
        ALREADY_OWN_SHIP        = 0284,
        NOT_PORT_TOWN           = 0285,
        ALREADY_OWN_HOUSE       = 0286,
        NO_HOUSES_FOR_SALE      = 0287,
        ALREADY_DEFAULTED       = 0288,    // not used in game?
        ALREADY_HAVE_LOAN       = 0289,
        NOT_ENOUGH_ACCOUNT      = 0290,
        DEPOSIT_LOC             = 0291,
        NOT_ENOUGH_ACCOUNT_LOC  = 0292,
        LOC_REQUEST_TOO_SMALL   = 0293,
        OVERPAID_LOAN           = 0294,
        LOAN_REQUEST_TOO_HIGH   = 0295,
        LOAN_REQUEST_TOO_LOW    = 0296,
        BOUNTY_DEFAULT_LOAN     = 0297,   // not used in game
        SELL_HOUSE_OFFER        = 0298,
        SELL_SHIP_OFFER         = 0299,
        NOT_ENOUGH_GOLD         = 0454,
    }

    public enum TransactionType
    {
        None,
        Depositing_gold,
        Withdrawing_gold,
        Withdrawing_Letter,
        Depositing_LOC,
        Repaying_loan,
        Borrowing_loan,
        Buy_house,
        Sell_house,
        Buy_ship,
        Sell_ship,
    }

    public enum ShipType
    {
        None = -1,
        Small = 0,
        Large = 1,
    }

    public static class DaggerfallBankManager
    {
        public const int gold1kg = 400;
        private const float deedSellMult = 0.85f;

        private static int[] shipPrices = new int[] { 1000, 200000 };
        private static DFPosition[] shipCoords = new DFPosition[] { new DFPosition(2, 2), new DFPosition(5, 5) };
        private static string[] shipInteriorSceneNames = new string[] {
            DaggerfallInterior.GetSceneName(1050578, 0),
            DaggerfallInterior.GetSceneName(2102157, 0),
        };
        private static string[] shipExteriorSceneNames = new string[] {
            StreamingWorld.GetSceneName(shipCoords[0].X, shipCoords[0].Y),
            StreamingWorld.GetSceneName(shipCoords[1].X, shipCoords[1].Y),
        };

        private static ShipType ownedShip = ShipType.None;

        public static bool OwnsShip { get { return ownedShip != ShipType.None; } }

        public static ShipType OwnedShip { get { return ownedShip; } set { ownedShip = value; } }

        public static int GetShipPrice(ShipType ship) { return ship >= 0 ? shipPrices[(int)ownedShip] : 0; }

        public static int GetShipSellPrice(ShipType ship) { return (int)(GetShipPrice(ship) * deedSellMult); }

        public static DFPosition GetShipCoords() { return OwnsShip ? shipCoords[(int)ownedShip] : null; }

        private static int loanMaxPerLevel = 50000;

        private static double locCommission = 1.01;

        private static DaggerfallDateTime dateTime;

        private static BankRecordData_v1[] bankAccounts;

        public static BankRecordData_v1[] BankAccounts
        {
            get 
            {
                if (bankAccounts == null)
                    SetupAccounts();
                return bankAccounts; 
            }
            set 
            {
                if (bankAccounts == null)
                    SetupAccounts();
                bankAccounts = value; 
            }
        }

        public static bool OwnsHouse { get { return true; } } //##TODo

        public static bool HasLoan(int regionIndex)
        {
            if (!ValidateRegion(regionIndex))
                return false;

            return BankAccounts[regionIndex].loanTotal > 0;
        }

        public static void SetupAccounts()
        {
            bankAccounts = new BankRecordData_v1[DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount];

            for (int i = 0; i < bankAccounts.Length; i++)
            {
                var account = new BankRecordData_v1();
                account.regionIndex = i;
                bankAccounts[i] = account;
            }
        }

        /// <summary>
        /// Gets amount of gold in specified region
        /// </summary>
        /// <param name="regionIndex"></param>
        /// <returns></returns>
        public static long GetAccountTotal(int regionIndex)
        {
            if (!ValidateRegion(regionIndex))
                throw new ArgumentOutOfRangeException();
            else
                return BankAccounts[regionIndex].accountGold;
        }

        /// <summary>
        /// Gets the amount due for loan in specified region
        /// </summary>
        /// <param name="regionIndex"></param>
        /// <returns></returns>
        public static long GetLoanedTotal(int regionIndex)
        {
            if (!ValidateRegion(regionIndex))
                throw new ArgumentOutOfRangeException();
            else
                return BankAccounts[regionIndex].loanTotal;
        }

        /// <summary>
        /// Gets loan due date in classic time format for specified region
        /// </summary>
        /// <param name="regionIndex"></param>
        /// <returns></returns>
        public static long GetLoanDueDate(int regionIndex)
        {
            if (!ValidateRegion(regionIndex))
                throw new ArgumentOutOfRangeException();
            else
                return BankAccounts[regionIndex].loanDueDate;
        }


        public static void MakeTransaction(TransactionType type, int amount, int regionIndex)
        {
            if (regionIndex < 0 || regionIndex >= BankAccounts.Length)
                throw new ArgumentOutOfRangeException();

            TransactionResult result;

            switch (type)
            {
                case TransactionType.None:
                    result = TransactionResult.NONE;
                    break;
                case TransactionType.Depositing_gold:
                    result = DepositGold(amount, regionIndex);
                    break;
                case TransactionType.Withdrawing_gold:
                    result = WithdrawGold(amount, regionIndex);
                    break;
                case TransactionType.Withdrawing_Letter:
                    result = Withdraw_LOC(amount, regionIndex);
                    break;
                case TransactionType.Depositing_LOC:
                    result = DepositAll_LOC(regionIndex);
                    break;
                case TransactionType.Repaying_loan:
                    result = RepayLoan(ref amount, regionIndex);
                    break;
                case TransactionType.Borrowing_loan:
                    result = BorrowLoan(amount, regionIndex);
                    break;
                case TransactionType.Sell_ship:
                    result = SellShip();
                    break;
                default:
                    result = TransactionResult.NONE;
                    break;
            }

            RaiseTransactionEvent(type, result, amount);

        }

        public static TransactionResult DepositGold(int amount, int regionIndex)
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if (amount > playerEntity.GoldPieces)
                return TransactionResult.NOT_ENOUGH_GOLD;

            BankAccounts[regionIndex].accountGold += amount;
            playerEntity.GoldPieces -= (int)amount;
            return TransactionResult.NONE;
        }

        public static TransactionResult WithdrawGold(int amount, int regionIndex)
        {
            if (amount > BankAccounts[regionIndex].accountGold)
                return TransactionResult.NOT_ENOUGH_ACCOUNT;

            // Check weight limit
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if (playerEntity.CarriedWeight + (amount / gold1kg) > playerEntity.MaxEncumbrance)
                return TransactionResult.TOO_HEAVY;

            BankAccounts[regionIndex].accountGold -= amount;
            playerEntity.GoldPieces += amount;
            return TransactionResult.NONE;
        }

        public static TransactionResult DepositAll_LOC(int regionIndex)
        {
            // Remove all LOC from inventory and add sum to account
            ItemCollection playerItems = GameManager.Instance.PlayerEntity.Items;
            while (true)
            {
                DaggerfallUnityItem loc = playerItems.GetItem(ItemGroups.MiscItems, (int)MiscItems.Letter_of_credit);
                if (loc == null)
                    return TransactionResult.NONE;
                BankAccounts[regionIndex].accountGold += loc.value;
                playerItems.RemoveItem(loc);
            }
        }

        public static TransactionResult Withdraw_LOC(int amount, int regionIndex)
        {
            // Create LOC and deduct from account
            int amountPlusCommission = (int)(amount * locCommission);
            if (amountPlusCommission > BankAccounts[regionIndex].accountGold)
                return TransactionResult.NOT_ENOUGH_ACCOUNT_LOC;
            else if (amount < 100)
                return TransactionResult.LOC_REQUEST_TOO_SMALL;

            BankAccounts[regionIndex].accountGold -= amountPlusCommission;
            DaggerfallUnityItem loc = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Letter_of_credit);
            loc.value = amount;
            GameManager.Instance.PlayerEntity.Items.AddItem(loc, Items.ItemCollection.AddPosition.Front);
            return TransactionResult.NONE;
        }

        //##TODO
        public static TransactionResult PurchaseHouse()
        {
            return TransactionResult.ALREADY_OWN_HOUSE;
        }

        //##TODO
        public static TransactionResult SellHouse()
        {
            return TransactionResult.SELL_HOUSE_OFFER;
        }

        public static TransactionResult PurchaseShip(ShipType shipType, int amount, int regionIndex)
        {
            if (shipType == ShipType.None)
                return TransactionResult.NONE;

            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            var playerGold = playerEntity.GetGoldAmount();
            var accountGold = BankAccounts[regionIndex].accountGold;

            if (amount > playerGold + accountGold)
                return TransactionResult.NOT_ENOUGH_GOLD;

            amount = playerEntity.DeductGoldAmount(amount);
            bankAccounts[regionIndex].loanTotal -= amount;

            // Set player owned ship and add scenes to permanent list
            ownedShip = shipType;
            SaveLoadManager.StateManager.AddPermanentScene(shipExteriorSceneNames[(int)shipType]);
            SaveLoadManager.StateManager.AddPermanentScene(shipInteriorSceneNames[(int)shipType]);

            return TransactionResult.PURCHASED_SHIP;
        }

        public static TransactionResult SellShip()
        {
            int amount = GetShipSellPrice(ownedShip);
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if (playerEntity.CarriedWeight + (amount / gold1kg) > playerEntity.MaxEncumbrance)
                return TransactionResult.TOO_HEAVY;

            playerEntity.GoldPieces += amount;
            SaveLoadManager.StateManager.RemovePermanentScene(shipExteriorSceneNames[(int)ownedShip]);
            SaveLoadManager.StateManager.RemovePermanentScene(shipInteriorSceneNames[(int)ownedShip]);
            ownedShip = ShipType.None;

            return TransactionResult.NONE;
        }

        //unoffical wiki says max possible loan is 1,100,000 but testing indicates otherwise
        //rep. doesn't seem to effect cap, it's just level * 50k
        public static int CalculateMaxLoan()
        {
            return GameManager.Instance.PlayerEntity.Level * loanMaxPerLevel;
        }

        //note - uses inv. gold pieces, account gold & loc
        private static TransactionResult RepayLoan(ref int amount, int regionIndex)
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            var playerGold = playerEntity.GetGoldAmount();
            var accountGold = BankAccounts[regionIndex].accountGold;
            TransactionResult result = TransactionResult.NONE;

            if (!HasLoan(regionIndex))
                return TransactionResult.NONE;
            else if (amount > playerGold + accountGold)
                return TransactionResult.NOT_ENOUGH_GOLD;
            else if (amount > BankAccounts[regionIndex].loanTotal)
            {
                result = TransactionResult.OVERPAID_LOAN;
                amount = BankAccounts[regionIndex].loanTotal;
            }

            bankAccounts[regionIndex].loanTotal -= amount;
            amount = playerEntity.DeductGoldAmount(amount);
            if (amount > 0)     // Should not happen
                bankAccounts[regionIndex].accountGold -= amount;

            if (bankAccounts[regionIndex].loanTotal <= 0)
                bankAccounts[regionIndex].loanDueDate = 0;

            return result;
        }


        private static TransactionResult BorrowLoan(int amount, int regionIndex)
        {
            TransactionResult result = TransactionResult.NONE;
            if (HasLoan(regionIndex))
                result = TransactionResult.ALREADY_HAVE_LOAN;
            else if (amount < 100)
                result = TransactionResult.LOAN_REQUEST_TOO_LOW;
            else if (amount > CalculateMaxLoan())
                result = TransactionResult.LOAN_REQUEST_TOO_HIGH;
            else
            {
                BankAccounts[regionIndex].loanTotal += (int)(amount + amount * .1);
                BankAccounts[regionIndex].accountGold += amount;
                bankAccounts[regionIndex].loanDueDate = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            }
            return result;
        }

        private static bool ValidateRegion(int regionIndex)
        {
            if (regionIndex < 0)
                return false;
            else if (regionIndex >= BankAccounts.Length)
                return false;
            else
                return true;
        }

        public static string GetLoanDueDateString(int regionIndex)
        {
            if (BankAccounts[regionIndex].loanDueDate <= 0)
                return "";
            if (dateTime == null)
                dateTime = new DaggerfallDateTime();
            dateTime.FromClassicDaggerfallTime(BankAccounts[regionIndex].loanDueDate);
            string timeString = dateTime.DateString();
            return timeString;
        }


        public static void ReadNativeBankData(SaveTreeBaseRecord records)
        {
            SetupAccounts();

            if (records == null || records.RecordType != RecordTypes.BankAccount)
                return;


            MemoryStream stream = new MemoryStream(records.RecordData);
            BinaryReader reader = new BinaryReader(stream);

            int count = 0;
            while (reader.BaseStream.Position + 13 < records.RecordLength)
            {
                BankRecordData_v1 record = new BankRecordData_v1();
                record.accountGold      = reader.ReadInt32();
                record.loanTotal        = reader.ReadInt32();
                record.loanDueDate      = reader.ReadUInt32();
                reader.BaseStream.Position++;   //skip over unused byte in each record
                record.regionIndex = count;

                if (record.regionIndex >= BankAccounts.Length)
                {
                    Debug.LogError("error reading bank data from classic save");
                    break;
                }

                BankAccounts[record.regionIndex] = record;
                count++;
            }

            reader.Close();
        }

#region events

        public delegate void Transaction(TransactionType type, TransactionResult result, int amount);
        public static event Transaction OnTransaction;
        public static event Transaction OnDepositGold;
        public static event Transaction OnWithdrawGold;
        public static event Transaction OnRepayLoan;
        public static event Transaction OnBorrowLoan;
        public static event Transaction OnDepositLOC;
        public static event Transaction OnWithdrawLOC;
        public static event Transaction OnBuyHouse;
        public static event Transaction OnSellHouse;
        public static event Transaction OnBuyShip;
        public static event Transaction OnSellShip;


        public static void RaiseTransactionEvent(TransactionType type, TransactionResult result, int amount)
        {
            if (OnTransaction != null)
                OnTransaction(type, result, amount);

            switch (type)
            {
                case TransactionType.None:
                    break;
                case TransactionType.Depositing_gold:
                    if (OnDepositGold != null)
                        OnDepositGold(type, result, amount);
                    break;
                case TransactionType.Withdrawing_gold:
                    if(OnWithdrawGold != null)
                        OnWithdrawGold(type, result, amount);
                    break;
                case TransactionType.Withdrawing_Letter:
                    if (OnWithdrawLOC != null)
                        OnWithdrawLOC(type, result, amount);
                    break;
                case TransactionType.Depositing_LOC:
                    if (OnDepositLOC != null)
                        OnDepositLOC(type, result, amount);
                    break;
                case TransactionType.Repaying_loan:
                    if (OnRepayLoan != null)
                        OnRepayLoan(type, result, amount);
                    break;
                case TransactionType.Borrowing_loan:
                    if (OnBorrowLoan != null)
                        OnBorrowLoan(type, result, amount);
                    break;
                case TransactionType.Buy_house:
                    if (OnBuyHouse != null)
                        OnBuyHouse(type, result, amount);
                    break;
                case TransactionType.Sell_house:
                    if (OnSellHouse != null)
                        OnSellHouse(type, result, amount);
                    break;
                case TransactionType.Buy_ship:
                    if (OnBuyShip != null)
                        OnBuyShip(type, result, amount);
                    break;
                case TransactionType.Sell_ship:
                    if (OnSellShip != null)
                        OnSellShip(type, result, amount);
                    break;
                default:
                    break;
            }

        }

#endregion

    }

}
