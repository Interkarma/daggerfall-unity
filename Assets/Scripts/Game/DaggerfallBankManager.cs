// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    Hazelnut
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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Formulas;

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
        ALREADY_DEFAULTED       = 0288,
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
        Repaying_loan_from_account,
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
        private static float cachedGoldUnitWeightInKg = -1.0f;
        public static float goldUnitWeightInKg
        {
            get
            {
                if (cachedGoldUnitWeightInKg == -1.0f)
                    cachedGoldUnitWeightInKg = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(ItemGroups.Currency, 0).baseWeight;
                return cachedGoldUnitWeightInKg;
            }
        }
        
        private const float deedSellMult = 0.85f;
        private const float housePriceMult = 1280f;
        private const uint loanRepayMinutes = DaggerfallDateTime.DaysPerYear * DaggerfallDateTime.MinutesPerDay;

        #region Ships:

        private static readonly int[] shipPrices = new int[] { 100000, 200000 };
        private static readonly uint[] shipModelIds = new uint[] { 910, 909 };
        private static readonly float[] shipCameraDist = new float[] { -30, -50 };
        private static readonly DFPosition[] shipCoords = new DFPosition[] { new DFPosition(2, 2), new DFPosition(5, 5) };
        private static readonly string[] shipInteriorSceneNames = new string[] {
            DaggerfallInterior.GetSceneName(1050578, BuildingDirectory.buildingKey0),
            DaggerfallInterior.GetSceneName(2102157, BuildingDirectory.buildingKey0),
        };
        private static readonly string[] shipExteriorSceneNames = new string[] {
            StreamingWorld.GetSceneName(shipCoords[0].X, shipCoords[0].Y),
            StreamingWorld.GetSceneName(shipCoords[1].X, shipCoords[1].Y),
        };

        private static ShipType ownedShip = ShipType.None;

        public static bool OwnsShip { get { return ownedShip != ShipType.None; } }

        public static ShipType OwnedShip { get { return ownedShip; } set { ownedShip = value; } }

        public static int GetShipPrice(ShipType ship) { return ship >= 0 ? shipPrices[(int) ship] : 0; }

        public static int GetShipSellPrice(ShipType ship) { return (int)(GetShipPrice(ship) * deedSellMult); }

        public static uint GetShipModelId(ShipType ship) { return ship >= 0 ? shipModelIds[(int)ship] : 0; }

        public static float GetShipCameraDist(ShipType ship) { return ship >= 0 ? shipCameraDist[(int)ship] : 0; }

        public static DFPosition GetShipCoords() { return OwnsShip ? shipCoords[(int)ownedShip] : null; }

        public static void ResetShip() { ownedShip = ShipType.None; }

        #endregion

        #region Houses:

        private static HouseData_v1[] houses;

        public static bool OwnsHouse { get { return Houses[GameManager.Instance.PlayerGPS.CurrentLocation.RegionIndex].buildingKey > 0; } }

        public static int OwnedHouseKey { get { return Houses[GameManager.Instance.PlayerGPS.CurrentLocation.RegionIndex].buildingKey; } }

        public static bool IsHouseOwned(int buildingKey)
        {
            if (buildingKey > 0)
            {
                DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
                return Houses[location.RegionIndex].buildingKey == buildingKey;
            }
            return false;
        }

        public static HouseData_v1[] Houses
        {
            get {
                if (houses == null)
                    SetupHouses();
                return houses;
            }
            set {
                if (houses == null)
                    SetupHouses();
                houses = value;
            }
        }

        public static int GetHousePrice(BuildingSummary house)
        {
            // Get model data and radius which defines price
            ModelData modelData;
            DaggerfallUnity.Instance.MeshReader.GetModelData(house.ModelID, out modelData);
            float houseRadius = modelData.DFMesh.Radius;
            return (int) (houseRadius * housePriceMult);
        }

        public static int GetHouseSellPrice(BuildingSummary house) { return (int)(GetHousePrice(house) * deedSellMult); }

        public static void SetupHouses()
        {
            houses = new HouseData_v1[DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount];
            for (int i = 0; i < houses.Length; i++)
            {
                var house = new HouseData_v1();
                house.regionIndex = i;
                houses[i] = house;
            }
        }

        #endregion

        #region Loans and accounts:

        public static int loanMaxPerLevel = 50000;

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

        public static bool HasLoan(int regionIndex)
        {
            if (!ValidateRegion(regionIndex))
                return false;

            return BankAccounts[regionIndex].loanTotal > 0;
        }

        public static bool HasDefaulted(int regionIndex)
        {
            if (!ValidateRegion(regionIndex))
                return false;

            return BankAccounts[regionIndex].hasDefaulted;
        }

        public static void SetDefaulted(int regionIndex, bool defaulted)
        {
            if (!ValidateRegion(regionIndex))
                return;

            BankAccounts[regionIndex].hasDefaulted = defaulted;
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

        #endregion

        #region Bank transaction methods

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
                    result = RepayLoan(ref amount, false, regionIndex);
                    break;
                case TransactionType.Repaying_loan_from_account:
                    result = RepayLoan(ref amount, true, regionIndex);
                    break;
                case TransactionType.Borrowing_loan:
                    result = BorrowLoan(amount, regionIndex);
                    break;
                case TransactionType.Sell_house:
                    result = SellHouse(regionIndex);
                    break;
                case TransactionType.Sell_ship:
                    result = SellShip(regionIndex);
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
            DaggerfallUnityItem wagonGold = playerEntity.WagonItems.GetItem(ItemGroups.Currency, (int)Currency.Gold_pieces);
            if (amount > playerEntity.GoldPieces + (wagonGold != null ? wagonGold.stackCount : 0))
                return TransactionResult.NOT_ENOUGH_GOLD;

            BankAccounts[regionIndex].accountGold += amount;

            if (amount > playerEntity.GoldPieces && wagonGold != null)
            {
                wagonGold.stackCount -= (amount - playerEntity.GoldPieces);
                playerEntity.GoldPieces = 0;
                if (wagonGold.stackCount < 1)
                    playerEntity.WagonItems.RemoveItem(wagonGold);
            }
            else
            {
                playerEntity.GoldPieces -= amount;
            }
            return TransactionResult.NONE;
        }

        public static TransactionResult WithdrawGold(int amount, int regionIndex)
        {
            if (amount > BankAccounts[regionIndex].accountGold)
                return TransactionResult.NOT_ENOUGH_ACCOUNT;

            // Check weight limit
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if (playerEntity.CarriedWeight + (amount * goldUnitWeightInKg) > playerEntity.MaxEncumbrance)
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

        public static TransactionResult PurchaseHouse(BuildingSummary house, int regionIndex)
        {
            if (house.buildingKey < 1)
                return TransactionResult.NONE;

            int amount = GetHousePrice(house);
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            var playerGold = playerEntity.GetGoldAmount();
            var accountGold = BankAccounts[regionIndex].accountGold;

            if (amount > playerGold + accountGold)
                return TransactionResult.NOT_ENOUGH_GOLD;

            amount = playerEntity.DeductGoldAmount(amount);
            bankAccounts[regionIndex].accountGold -= amount;

            AllocateHouseToPlayer(house, regionIndex);

            return TransactionResult.PURCHASED_HOUSE;
        }

        public static void AllocateHouseToPlayer(BuildingSummary house, int regionIndex)
        {
            // Set player owned house for this region
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            int mapID = location.MapTableData.MapId;
            houses[regionIndex].location = location.Name;
            houses[regionIndex].mapID = mapID;
            houses[regionIndex].buildingKey = house.buildingKey;

            // Ensure building is discovered
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            GameManager.Instance.PlayerGPS.DiscoverBuilding(house.buildingKey, TextManager.Instance.GetLocalizedText("playerResidence").Replace("%s", playerEntity.Name));

            // Add interior scene to permanent list
            SaveLoadManager.StateManager.AddPermanentScene(DaggerfallInterior.GetSceneName(mapID, house.buildingKey));

            // Add note to journal
            playerEntity.Notebook.AddNote(
                TextManager.Instance.GetLocalizedText("houseDeed").Replace("%town", location.Name).Replace("%region", TextManager.Instance.GetLocalizedRegionName(regionIndex)));
        }

        public static TransactionResult SellHouse(int regionIndex)
        {
            BuildingSummary house ;
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (buildingDirectory)
            {
                if (buildingDirectory.GetBuildingSummary(DaggerfallBankManager.OwnedHouseKey, out house))
                {
                    BankAccounts[regionIndex].accountGold += GetHouseSellPrice(house);
                    SaveLoadManager.StateManager.RemovePermanentScene(DaggerfallInterior.GetSceneName(houses[regionIndex].mapID, house.buildingKey));
                    GameManager.Instance.PlayerGPS.UndiscoverBuilding(house.buildingKey);
                    houses[regionIndex] = new HouseData_v1() { regionIndex = regionIndex };
                }
            }
            return TransactionResult.NONE;
        }

        public static TransactionResult PurchaseShip(ShipType shipType, int regionIndex)
        {
            if (shipType == ShipType.None)
                return TransactionResult.NONE;

            int amount = GetShipPrice(shipType);
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            var playerGold = playerEntity.GetGoldAmount();
            var accountGold = BankAccounts[regionIndex].accountGold;

            if (amount > playerGold + accountGold)
                return TransactionResult.NOT_ENOUGH_GOLD;

            amount = playerEntity.DeductGoldAmount(amount);
            bankAccounts[regionIndex].accountGold -= amount;

            AssignShipToPlayer(shipType);

            return TransactionResult.PURCHASED_SHIP;
        }

        public static void AssignShipToPlayer(ShipType shipType)
        {
            // Set player owned ship and add scenes to permanent list
            ownedShip = shipType;
            if (shipType != ShipType.None)
            {
                SaveLoadManager.StateManager.AddPermanentScene(shipExteriorSceneNames[(int)shipType]);
                SaveLoadManager.StateManager.AddPermanentScene(shipInteriorSceneNames[(int)shipType]);
            }
        }

        public static TransactionResult SellShip(int regionIndex)
        {
            BankAccounts[regionIndex].accountGold += GetShipSellPrice(ownedShip);
            SaveLoadManager.StateManager.RemovePermanentScene(shipExteriorSceneNames[(int)ownedShip]);
            SaveLoadManager.StateManager.RemovePermanentScene(shipInteriorSceneNames[(int)ownedShip]);
            ownedShip = ShipType.None;

            return TransactionResult.NONE;
        }

        //note - uses inv. gold pieces, account gold & loc
        private static TransactionResult RepayLoan(ref int amount, bool accountOnly, int regionIndex)
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            var availableGold = BankAccounts[regionIndex].accountGold;
            if (!accountOnly)
            {
                availableGold += playerEntity.GetGoldAmount();
            }

            TransactionResult result = TransactionResult.NONE;

            if (!HasLoan(regionIndex))
                return TransactionResult.NONE;
            else if (amount > availableGold)
                return TransactionResult.NOT_ENOUGH_GOLD;
            else if (amount > BankAccounts[regionIndex].loanTotal)
            {
                result = TransactionResult.OVERPAID_LOAN;
                amount = BankAccounts[regionIndex].loanTotal;
            }

            bankAccounts[regionIndex].loanTotal -= amount;
            if (!accountOnly)
                amount = playerEntity.DeductGoldAmount(amount);
            if (amount > 0)
                bankAccounts[regionIndex].accountGold -= amount;

            if (bankAccounts[regionIndex].loanTotal <= 0)
                bankAccounts[regionIndex].loanDueDate = 0;

            return result;
        }

        private static TransactionResult BorrowLoan(int amount, int regionIndex)
        {
            TransactionResult result = TransactionResult.NONE;
            if (amount < 100)
                result = TransactionResult.LOAN_REQUEST_TOO_LOW;
            else if (amount > FormulaHelper.CalculateMaxBankLoan())
                result = TransactionResult.LOAN_REQUEST_TOO_HIGH;
            else
            {
                BankAccounts[regionIndex].loanTotal += FormulaHelper.CalculateBankLoanRepayment(amount, regionIndex);
                BankAccounts[regionIndex].accountGold += amount;
                bankAccounts[regionIndex].loanDueDate = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() + loanRepayMinutes;
            }
            return result;
        }

        #endregion

        #region Utility methods

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
                record.hasDefaulted     = reader.ReadBoolean();
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

        #endregion

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
