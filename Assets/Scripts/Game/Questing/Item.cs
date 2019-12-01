// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com), Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.FallExe;
using FullSerializer;
using DaggerfallWorkshop.Game.Guilds;

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

        bool artifact = false;
        bool useClicked = false;
        bool actionWatching = false;
        bool allowDrop = false;
        bool playerDropped = false;
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

        /// <summary>
        /// Gets or sets flag when player clicks "Use" on item in inventory.
        /// This is unrelated to using text-only objects like letters.
        /// Example is Sx017 "Wayrest Painting" when player uses painting to trigger a quest task.
        /// This flag is only raised once for any quest actions that consume it.
        /// </summary>
        public bool UseClicked
        {
            get { return useClicked; }
            set { useClicked = value; }
        }

        /// <summary>
        /// Gets or sets value when an action is watching this item for some reason.
        /// Usually waiting for a response to clicks.
        /// This means quest system should have priority to "use" handling on this item.
        /// </summary>
        public bool ActionWatching
        {
            get { return actionWatching; }
            set { actionWatching = value; }
        }

        /// <summary>
        /// This flag determines if quest item can be removed from main item collection.
        /// </summary>
        public bool AllowDrop
        {
            get { return allowDrop; }
            set { allowDrop = value; }
        }

        /// <summary>
        /// Gets or sets flag stating if this item was dropped from inventory.
        /// </summary>
        public bool PlayerDropped
        {
            get { return playerDropped; }
            set { playerDropped = value; }
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

            string declMatchStr = @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) item class (?<itemClass>\d+) subclass (?<itemSubClass>\d+)|"+
                                  @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) (?<artifact>artifact) (?<itemName>[a-zA-Z0-9_.-]+)|"+
                                  @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) (?<itemName>[a-zA-Z0-9_.-]+)";

            string optionsMatchStr = @"range (?<rangeLow>\d+) to (?<rangeHigh>\d+)";

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

                // Item class value
                Group itemClassGroup = match.Groups["itemClass"];
                if (itemClassGroup.Success)
                    itemClass = Parser.ParseInt(itemClassGroup.Value);

                // Item subclass value
                Group itemSubClassGroup = match.Groups["itemSubClass"];
                if (itemClassGroup.Success)
                    itemSubClass = Parser.ParseInt(itemSubClassGroup.Value);

                // Artifact status
                if (!string.IsNullOrEmpty(match.Groups["artifact"].Value))
                    artifact = true;

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
                }

                // Create item
                if (itemClass != -1 && itemSubClass != -1 && !isGold)
                    item = CreateItem(itemClass, itemSubClass);         // Create item by class and subclass (a.k.a ItemGroup and GroupIndex)
                else if (!string.IsNullOrEmpty(itemName) && !isGold)
                    item = CreateItem(itemName);                        // Create by name of item in lookup table
                else if (isGold)
                    item = CreateGold(rangeLow, rangeHigh);             // Create gold pieces of amount by level or range values
                else
                    throw new Exception(string.Format("Could not create Item from line {0}", line));

                // add conversation topics from anyInfo command tag
                AddConversationTopics();
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
                case MacroTypes.DetailsMacro:           // Same as display name?
                    if (artifact)
                        textOut = item.shortName;
                    else
                        textOut = (isGoldPieces) ? item.stackCount.ToString() : GetLongName(item);
                    break;

                default:                                // Macro not supported
                    result = false;
                    break;
            }

            return result;
        }

        public override void Dispose()
        {
            base.Dispose();

            // Remove item if present in player item collections and still marked as a quest item
            if (item != null && item.IsQuestItem)
            {
                Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                playerEntity.Items.RemoveItem(item);
            }
        }

        #endregion

        #region Private Methods

        // Custom long name getter that prevents plant suffix being displayed in quest text
        string GetLongName(DaggerfallUnityItem item)
        {
            return DaggerfallUnity.Instance.ItemHelper.ResolveItemLongName(item, false);
        }

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

            DaggerfallUnityItem result;

            // Handle random magic items
            if (itemClass == (int)ItemGroups.MagicItems)
            {
                Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                result = ItemBuilder.CreateRegularMagicItem(itemSubClass, playerEntity.Level, playerEntity.Gender, playerEntity.Race);
            }
            // Handle books
            else if (itemClass == (int)ItemGroups.Books)
            {
                result = ItemBuilder.CreateRandomBook();
            }
            else
            {
                // Handle random subclass
                if (itemSubClass == -1)
                {
                    Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray((ItemGroups)itemClass);
                    itemSubClass = UnityEngine.Random.Range(0, enumArray.Length);
                }

                // Create item
                result = new DaggerfallUnityItem((ItemGroups)itemClass, itemSubClass);
            }

            // Link item to quest
            result.LinkQuestItem(ParentQuest.UID, Symbol.Clone());

            string name = result.shortName.Replace("%it", result.ItemTemplate.name);
            QuestMachine.LogFormat(
                ParentQuest,
                "Generated \"{0}\" from Class {1} and Subclass {2} for item {3}",
                name,
                itemClass,
                itemSubClass,
                Symbol.Original
            );

             return result;
        }

        // Create stack of gold pieces
        DaggerfallUnityItem CreateGold(int rangeLow, int rangeHigh)
        {
            // Get amount
            int amount = 0;
            if (rangeLow == -1 || rangeHigh == -1)
            {
                Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

                int playerMod = (playerEntity.Level / 2) + 1;
                int factionMod = 50;
                IGuild guild = null;
                if (ParentQuest.FactionId != 0)
                {
                    guild = GameManager.Instance.GuildManager.GetGuild(ParentQuest.FactionId);
                    if (guild != null && !(guild is NonMemberGuild))
                    {
                        // If this is a faction quest, playerMod is (player factionrank + 1) rather than level
                        playerMod = guild.Rank + 1;

                        // If this is a faction quest, factionMod = faction.power rather than 50
                        FactionFile.FactionData factionData;
                        if (playerEntity.FactionData.GetFactionData(ParentQuest.FactionId, out factionData))
                            factionMod = factionData.power;
                    }
                }
                if (playerMod > 10)
                    playerMod = 10;

                PlayerGPS gps = GameManager.Instance.PlayerGPS;
                int regionPriceMod = playerEntity.RegionData[gps.CurrentRegionIndex].PriceAdjustment / 2;
                amount = UnityEngine.Random.Range(150 * playerMod, (200 * playerMod) + 1) * (regionPriceMod + 500) / 1000 * (factionMod + 50) / 100;

                if (guild != null)
                    amount = guild.AlterReward(amount);
            }
            else
                amount = UnityEngine.Random.Range(rangeLow, rangeHigh + 1);

            if (amount < 1)
                amount = 1;

            // Create item
            DaggerfallUnityItem result = new DaggerfallUnityItem(ItemGroups.Currency, 0);
            result.stackCount = amount;
            result.LinkQuestItem(ParentQuest.UID, Symbol.Clone());

            return result;
        }


        void AddConversationTopics()
        {
            List<TextFile.Token[]> anyInfoAnswers = null;
            List<TextFile.Token[]> anyRumorsAnswers = null;
            if (this.InfoMessageID != -1)
            {
                Message message = this.ParentQuest.GetMessage(this.InfoMessageID);
                anyInfoAnswers = new List<TextFile.Token[]>();
                if (message != null)
                {
                    for (int i = 0; i < message.VariantCount; i++)
                    {
                        TextFile.Token[] tokens = message.GetTextTokensByVariant(i, false); // do not expand macros here (they will be expanded just in time by TalkManager class)
                        anyInfoAnswers.Add(tokens);
                    }
                }

                message = this.ParentQuest.GetMessage(this.RumorsMessageID);
                anyRumorsAnswers = new List<TextFile.Token[]>();
                if (message != null)
                {
                    for (int i = 0; i < message.VariantCount; i++)
                    {
                        TextFile.Token[] tokens = message.GetTextTokensByVariant(i, false); // do not expand macros here (they will be expanded just in time by TalkManager class)
                        anyRumorsAnswers.Add(tokens);
                    }
                }                
            }

            string key = this.Symbol.Name;
            GameManager.Instance.TalkManager.AddQuestTopicWithInfoAndRumors(this.ParentQuest.UID, this, key, TalkManager.QuestInfoResourceType.Thing, anyInfoAnswers, anyRumorsAnswers);
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public bool artifact;
            public bool useClicked;
            public bool actionWatching;
            public bool allowDrop;
            public bool playerDropped;
            public ItemData_v1 item;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();

            data.artifact = artifact;
            data.useClicked = useClicked;
            data.actionWatching = actionWatching;
            data.allowDrop = allowDrop;
            data.playerDropped = playerDropped;
            data.item = item.GetSaveData();

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            artifact = data.artifact;
            useClicked = data.useClicked;
            actionWatching = data.actionWatching;
            allowDrop = data.allowDrop;
            playerDropped = data.playerDropped;
            item = new DaggerfallUnityItem(data.item);
        }

        #endregion
    }
}