// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Items;

/*Example patterns:
 * 
 * Item _gold_ gold
 * Item _gold1_ gold range 5 to 25
 * Item talisman talisman
 * Item _book_ book2 anyInfo 1014 used 1014
 * Item _womensclothing_ womens_clothing
 * Item _answer_ letter used 1017
 * Item _artifact_ artifact Ring_of_Khajiit anyInfo 1014
 */

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A quest item is something used or granted during quest execution.
    /// Can contain tags, for example to show a message when used.
    /// </summary>
    public class Item : QuestResource
    {
        public const string matchString = @"Item (?<symbol>\w+)( artifact)? (?<type>\w+)| range (?<r1>\d+) to (?<r2>\d+)| anyInfo (?<info>\d+)| used (?<used>\d+)";

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
            SetItem(line);
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

    }
}