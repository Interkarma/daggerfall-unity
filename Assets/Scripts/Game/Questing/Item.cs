// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com), Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut
// 
// Notes:
//

using System;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallConnect.Arena2;
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
 * Item _modItem1_ item class 0 template 538
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
        bool madePermanent = false;
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

        /// <summary>
        /// Gets flag stating if this virtual quest item was previously made permanent.
        /// </summary>
        public bool MadePermanent
        {
            get { return madePermanent; }
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
                                  @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) item class (?<itemClass>\d+) template (?<itemTemplate>\d+)|" +
                                  @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) (?<artifact>artifact) (?<itemName>[a-zA-Z0-9_.-]+)|"+
                                  @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) (?<itemName>[a-zA-Z0-9_.-]+) key (?<itemKey>\d+)|"+
                                  @"(Item|item) (?<symbol>[a-zA-Z0-9_.-]+) (?<itemName>[a-zA-Z0-9_.-]+)";

            string optionsMatchStr = @"range (?<rangeLow>\d+) to (?<rangeHigh>\d+)";

            // Try to match source line with pattern
            string itemName = string.Empty;
            int itemKey = -1;
            int itemClass = -1;
            int itemSubClass = -1;
            int itemTemplate = -1;
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
                if (itemSubClassGroup.Success)
                    itemSubClass = Parser.ParseInt(itemSubClassGroup.Value);

                // Item template value
                Group itemTemplateGroup = match.Groups["itemTemplate"];
                if (itemTemplateGroup.Success)
                    itemTemplate = Parser.ParseInt(itemTemplateGroup.Value);

                // Artifact status
                if (!string.IsNullOrEmpty(match.Groups["artifact"].Value))
                    artifact = true;

                // Item id (for books and potions)
                Group itemKeyGroup = match.Groups["itemKey"];
                if (itemKeyGroup.Success)
                    itemKey = Parser.ParseInt(itemKeyGroup.Value);

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
                    item = CreateItem(itemClass, itemSubClass);                         // Create item by class and subclass (a.k.a ItemGroup and GroupIndex)
                else if (itemClass != -1 && itemTemplate > 0 && !isGold)
                    item = ItemBuilder.CreateItem((ItemGroups)itemClass, itemTemplate); // Create item by class and template index (used for modded items)
                else if (!string.IsNullOrEmpty(itemName) && !isGold)
                    item = CreateItem(itemName, itemKey);                               // Create by name of item in lookup table
                else if (isGold)
                    item = CreateGold(rangeLow, rangeHigh);                             // Create gold pieces of amount by level or range values
                else
                    throw new Exception(string.Format("Could not create Item from line {0}", line));

            }
        }

        public override bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            // Check if this item is gold pieces
            bool isGoldPieces = item.IsOfTemplate(ItemGroups.Currency, (int)Currency.Gold_pieces);

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

        #region Public Methods

        /// <summary>
        /// Makes both virtual and current instance of item permanent.
        /// If DaggerfallUnityItem is subsequently reinstantiated then new DaggerfallUnityItem must also be made permanent.
        /// </summary>
        public void MakePermanent()
        {
            // Flag this virtual Item as permanent
            madePermanent = true;

            // Set current DaggerfallUnityItem instance as permanent
            if (DaggerfallUnityItem != null)
                DaggerfallUnityItem.MakePermanent();
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
        DaggerfallUnityItem CreateItem(string itemName, int itemKey)
        {
            // Get items table
            Table itemsTable = QuestMachine.Instance.ItemsTable;
            if (itemsTable.HasValue(itemName))
            {
                int p1 = Parser.ParseInt(itemsTable.GetValue("p1", itemName));
                int p2 = Parser.ParseInt(itemsTable.GetValue("p2", itemName));
                return CreateItem(p1, p2, itemKey);
            }
            else
            {
                throw new Exception(string.Format("Could not find Item name {0} in items table", itemName));
            }
        }

        // Create by item class and subclass
        DaggerfallUnityItem CreateItem(int itemClass, int itemSubClass, int itemKey = -1)
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
                result = (itemKey != -1) ? ItemBuilder.CreateBook(itemKey) : ItemBuilder.CreateRandomBook();
            }
            // Handle potions
            else if (itemClass == (int)ItemGroups.UselessItems1 && itemSubClass == 1)
            {
                result = (itemKey != -1) ? ItemBuilder.CreatePotion(itemKey) : ItemBuilder.CreateRandomPotion();
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

            // Randomise clothing dye
            if (result.IsClothing)
                result.dyeColor = ItemBuilder.RandomClothingDye();

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
            public bool madePermanent;
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
            data.madePermanent = madePermanent;
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
            madePermanent = data.madePermanent;
            item = new DaggerfallUnityItem(data.item);
        }

        #endregion
    }
}
