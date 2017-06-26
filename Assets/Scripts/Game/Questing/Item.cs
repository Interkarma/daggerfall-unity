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
 * Item _I.06_ item class 17 subclass 13
 * Item _I.06_ item class 17 subclass -1
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
                if (!string.IsNullOrEmpty(itemName) && !isGold)
                    item = CreateItem(itemName);                        // Create by name of item in lookup table
                else if (itemClass != -1 && !isGold)
                    item = CreateItem(itemClass, itemSubClass);         // Create item by class and subclass (a.k.a ItemGroup and GroupIndex)
                else if (isGold)
                    item = CreateGold(rangeLow, rangeHigh);             // Create gold pieces of amount by level or range values
                else
                    throw new Exception(string.Format("Could not create Item from line {0}", line));
            }
        }

        public override bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            // Check if this item is gold pieces
            bool isGoldPieces = false;
            if (item.ItemGroup == ItemGroups.Currency && item.GroupIndex == 0)
                isGoldPieces = true;

            textOut = string.Empty;
            bool result = true;
            switch (macro)
            {
                case MacroTypes.NameMacro1:             // Display name
                    textOut = (isGoldPieces) ? item.stackCount.ToString() : item.LongName;
                    break;

                default:                                // Macro not supported
                    result = false;
                    break;
            }

            return result;
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
            if (itemClass == -1)
                throw new Exception(string.Format("Tried to create Item with class {0}", itemClass));

            // Handle random subclass
            if (itemSubClass == -1)
            {
                Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray((ItemGroups)itemClass);
                itemSubClass = UnityEngine.Random.Range(0, enumArray.Length);
            }

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
                amount = GameManager.Instance.PlayerEntity.Level * UnityEngine.Random.Range(90, 110);
            else
                amount = UnityEngine.Random.Range(rangeLow, rangeHigh + 1);

            // Create item
            DaggerfallUnityItem result = new DaggerfallUnityItem(ItemGroups.Currency, 0);
            result.stackCount = amount;
            result.LinkQuestItem(ParentQuest.UID, Symbol);

            return result;
        }

        #endregion
    }
}