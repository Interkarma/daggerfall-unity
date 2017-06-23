// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com), Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;

/*Example patterns:
 * 
 * Item _gold_ gold
 * Item _gold1_ gold range 5 to 25
 * Item talisman talisman
 * Item _book_ book2 anyInfo 1014 used 1014
 * Item _womensclothing_ womens_clothing
 * Item _answer_ letter used 1017
 * Item _artifact_ artifact Ring_of_Khajiit anyInfo 1014
 * TODO: Item _I.06_ item class 17 subclass 13
 */

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A quest item is something used or granted during quest execution.
    /// Can contain tags, for example to show a message when used.
    /// </summary>
    public class Item : QuestResource
    {
        #region Fields

        DaggerfallUnityItem item = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets resultant DaggerfallUnityItem object.
        /// </summary>
        public DaggerfallUnityItem DaggerfallUnityItem
        {
            get { return item; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        public Item(Quest parentQuest)
            : base(parentQuest)
        {
        }

        /// <summary>
        /// Construct an Item from QBN input.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        /// <param name="line">Item definition line from QBN.</param>
        public Item(Quest parentQuest, string line)
            : base(parentQuest)
        {
            SetResource(line);
        }

        #endregion

        #region Overrides

        public override void SetResource(string line)
        {
            base.SetResource(line);

            string declMatchStr = @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) artifact (?<itemName>[a-zA-Z0-9_.-]+)|(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) (?<itemName>[a-zA-Z0-9_.-]+)";

            string optionsMatchStr = @"range (?<rangeLow>\d+) to (?<rangeHigh>\d+)|" +
                                     @"item class (?<itemClass>\d+) subclass (?<itemSubClass>\d+)";

            // Try to match source line with pattern
            string itemName = string.Empty;
            int itemClass = -1;
            int itemSubClass = -1;
            bool isGold = false;
            int rangeLow = -1;
            int rangeHigh = -1;
            string artifactName = string.Empty;
            Match match = Regex.Match(line, declMatchStr);
            if (match.Success)
            {
                // Store symbol for quest system
                Symbol = new Symbol(match.Groups["symbol"].Value);

                // Item or artifact name
                itemName = match.Groups["itemName"].Value;

                // Set gold - this is not in the lookup table
                if (itemName == "gold")
                    isGold = true;

                // Split options from declaration
                string optionsLine = line.Substring(match.Length);

                // Match all options
                MatchCollection options = Regex.Matches(optionsLine, optionsMatchStr);
                foreach (Match option in options)
                {
                    // Range low value
                    Group rangeLowGroup = option.Groups["rangeLow"];
                    if (rangeLowGroup.Success)
                        rangeLow = Parser.ParseInt(rangeLowGroup.Value);

                    // Range high value
                    Group rangeHighGroup = option.Groups["rangeHigh"];
                    if (rangeHighGroup.Success)
                        rangeHigh = Parser.ParseInt(rangeHighGroup.Value);

                    // Item class value
                    Group itemClassGroup = option.Groups["itemClass"];
                    if (itemClassGroup.Success)
                        itemClass = Parser.ParseInt(itemClassGroup.Value);

                    // Item subclass value
                    Group itemSubClassGroup = option.Groups["itemSubClass"];
                    if (itemClassGroup.Success)
                        itemSubClass = Parser.ParseInt(itemSubClassGroup.Value);
                }

                // Create item
                if (!string.IsNullOrEmpty(itemName))
                    item = CreateItem(itemName);                        // Create by name of item in lookup table
                else if (itemClass != -1 && itemSubClass != -1)
                    item = CreateItem(itemClass, itemSubClass);         // Create item by class and subclass (a.k.a ItemGroup and GroupIndex)
                else if (isGold)
                    item = CreateGold(rangeLow, rangeHigh);             // Create gold pieces of amount by level or range values
                else
                    throw new Exception(string.Format("Could not create Item from line {0}", line));
            }
        }

        #endregion

        #region Private Methods

        // Create by item or artifact name
        // This gets class and subclass values from p1 and p2 of items lookup table
        DaggerfallUnityItem CreateItem(string itemName)
        {
            // Get items table
            Table itemsTable = QuestMachine.Instance.ItemsTable;
            if (itemsTable.HasValue(itemName))
            {
                int p1 = Parser.ParseInt(itemsTable.GetValue("p1", itemName));
                int p2 = Parser.ParseInt(itemsTable.GetValue("p2", itemName));
                return CreateItem(p1, p2);
            }
            else
            {
                throw new Exception(string.Format("Could not find Item name {0} in items table", itemName));
            }
        }

        // Create by item class and subclass
        DaggerfallUnityItem CreateItem(int itemClass, int itemSubClass)
        {
            // Validate
            if (itemClass == -1 || itemSubClass == -1)
                throw new Exception(string.Format("Tried to create Item with class {0} subclass {1}", itemClass, itemSubClass));

            // Create item
            DaggerfallUnityItem result = new DaggerfallUnityItem((ItemGroups)itemClass, itemSubClass);
            result.LinkQuestItem(ParentQuest.UID, Symbol);

            return result;
        }

        // Create stack of gold pieces
        DaggerfallUnityItem CreateGold(int rangeLow, int rangeHigh)
        {
            // Get amount
            int amount = 0;
            if (rangeLow == -1 || rangeHigh == -1)
                amount = UnityEngine.Random.Range(rangeLow, rangeHigh + 1);
            else
                amount = GameManager.Instance.PlayerEntity.Level * UnityEngine.Random.Range(90, 110);

            // Create item
            DaggerfallUnityItem result = new DaggerfallUnityItem(ItemGroups.Currency, 0);
            result.stackCount = amount;
            result.LinkQuestItem(ParentQuest.UID, Symbol);

            return result;
        }

        #endregion


        /*
        public const string matchString = @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+)( artifact)? (?<type>\w+)| range (?<r1>\d+) to (?<r2>\d+)| anyInfo (?<info>\d+)| used (?<used>\d+)";

        private DaggerfallUnityItem item;
        private string itemString;
        private int infoID;
        private int useID;
        private int rangeLow    = -1;
        private int rangeHigh   = -1;
        private bool isArtifact = false;

        bool isGold = false;
        int goldAmount = 0;

        public DaggerfallUnityItem DaggerfallUnityItem
        {
            get { return item; }
            set { item = value; }
        }

        public string ItemString
        {
            get { return itemString; }
            set { itemString = value; }
        }

        public int InfoID
        {
            get { return infoID; }
            set { infoID = value; }
        }

        public int UseID
        {
            get { return useID; }
            set { useID = value; }
        }

        public int RangeLow
        {
            get { return rangeLow; }
            set { rangeLow = value; }
        }

        public int RangeHigh
        {
            get { return rangeHigh; }
            set { rangeHigh = value; }
        }

        public bool IsArtifact
        {
            get { return isArtifact; }
        }

        public bool HasRange 
        {
            get { return (rangeLow >= 0 && rangeHigh >= 1) && (rangeHigh > rangeLow); }
        }

        public bool IsGold
        {
            get { return isGold; }
        }

        public int GoldAmount
        {
            get { return goldAmount; }
        }

        #region Constructors

        public Item(Quest parentQuest, string line) : base(parentQuest) 
        {
            try
            {
                SetItem(line);
            }
            catch
            {
                Debug.LogErrorFormat("Could not SetItem() from line: {0}", line);
            }
        }

        #endregion

        #region Overrides

        public override bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            textOut = string.Empty;

            // TEMP: Just hacking in gold value for now
            if (itemString == "gold" && macro == MacroTypes.NameMacro1)
            {
                textOut = goldAmount.ToString();
                return true;
            }

            return false;
        }

        #endregion

        #region public methods

        public void SetItem(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            if (line.Contains("artifact"))
                this.isArtifact = true;

            var matches = Regex.Matches(line, matchString);

            foreach(Match match in matches)
            {
                var symbol  = match.Groups["symbol"];
                var item    = match.Groups["type"];
                var info    = match.Groups["info"];
                var used    = match.Groups["used"];
                var r1      = match.Groups["r1"];
                var r2      = match.Groups["r2"];

                if (symbol.Success)
                    this.Symbol     = new Symbol(symbol.Value);
                if (item.Success)
                    this.itemString = item.Value;
                if (info.Success)
                    this.infoID     = Parser.ParseInt(info.Value);
                if (used.Success)
                    this.useID      = Parser.ParseInt(used.Value);
                if (r1.Success)
                    this.rangeLow   = Parser.ParseInt(r1.Value);
                if (r2.Success)
                    this.rangeHigh  = Parser.ParseInt(r2.Value);
            }

            // Handle item by type
            if (itemString == "gold")
            {
                // Generate gold amount
                int amount = 0;
                if (rangeLow != -1 && rangeHigh != -1)
                {
                    // Gold given is between range values (inclusive)
                    amount = Random.Range(rangeLow, rangeHigh + 1);
                }
                else
                {
                    // Gold given is roughly in the range of 100 gold per player level
                    int goldMultiplier = Random.Range(90, 110);
                    amount = goldMultiplier * GameManager.Instance.PlayerEntity.Level;
                }

                isGold = true;
                goldAmount = amount;
            }
            else
            {
                var table = QuestMachine.Instance.ItemsTable;

                if (table.HasValue(this.itemString))
                {
                    var row = table.GetRow(table.GetRowIndex(itemString));

                    if (row == null || row.Length < 3)
                    {
                        Debug.LogWarning("Failed to create quest item from string: " + itemString);
                        this.item = null;
                        return;
                    }

                    ItemGroups itemGroup = ItemGroups.None;
                    if (isArtifact)
                        itemGroup = ItemGroups.Artifacts;
                    else
                        itemGroup = (ItemGroups)Parser.ParseInt(row[1]);

                    var subType = Parser.ParseInt(row[2]);
                    this.item = new DaggerfallUnityItem(itemGroup, subType);
                    //Debug.Log(string.Format("found item string: {0} type: {1} sub: {2}", row[0], row[1], row[2]));

                    // Link quest Item resource to DaggerfallUnityItem
                    item.LinkQuestItem(ParentQuest.UID, Symbol);
                }
                else
                {
                    Debug.LogWarning("item not found in table: " + itemString);
                }
            }
        }

        #endregion
        */
    }
}