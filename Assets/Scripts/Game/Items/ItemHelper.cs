// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Numidium
//
// Notes:
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Save;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Helper class for working with items.
    /// </summary>
    public class ItemHelper
    {
        #region Fields

        // This array is in order of ItemEnums.ArtifactsSubTypes
        // Each element in array is the texture record index to use for that artifact in TEXTURE.432, TEXTURE.433
        // The actual equip placement and whether a left and right hand image exist is derived from item group/groupIndex from usual template data
        readonly int[] artifactTextureIndexMappings = { 12, 13, 10, 8, 19, 16, 25, 18, 21, 2, 24, 26, 0, 15, 3, 9, 23, 17, 7, 1, 22, 20, 5 };

        const string itemTemplatesFilename = "ItemTemplates";
        const string magicItemTemplatesFilename = "MagicItemTemplates";
        const string containerIconsFilename = "INVE16I0.CIF";
        const string bookMappingFilename = "books";

        const int artifactMaleTextureArchive = 432;
        const int artifactFemaleTextureArchive = 433;

        // Last template index for vanilla DF items, any higher index is a custom item
        public const int LastDFTemplate = 287;

        public static int WagonKgLimit = 750;

        List<ItemTemplate> itemTemplates = new List<ItemTemplate>();
        List<MagicItemTemplate> allMagicItemTemplates = new List<MagicItemTemplate>();
        List<MagicItemTemplate> artifactItemTemplates = new List<MagicItemTemplate>();
        readonly Dictionary<int, ImageData> itemImages = new Dictionary<int, ImageData>();
        readonly Dictionary<InventoryContainerImages, ImageData> containerImages = new Dictionary<InventoryContainerImages, ImageData>();
        readonly Dictionary<int, String> bookIDNameMapping = new Dictionary<int, String>();
        readonly Dictionary<int, String> localizedBookIDNameMapping = new Dictionary<int, string>();

        public delegate bool ItemUseHandler(DaggerfallUnityItem item, ItemCollection collection);
        Dictionary<int, ItemUseHandler> itemUseHandlers = new Dictionary<int, ItemUseHandler>();

        Dictionary<int, Type> customItemTypes = new Dictionary<int, Type>();
        Dictionary<ItemGroups, List<int>> customItemGroups = new Dictionary<ItemGroups, List<int>>();

        #endregion

        #region Properties

        /// <summary>
        /// Array of all magic item templates including artifact data.
        /// Data is loaded from MagicItemTemplates.txt, a JSON dump of fixed MAGIC.DEF.
        /// </summary>
        public MagicItemTemplate[] MagicItemTemplates => allMagicItemTemplates.ToArray();

        /// <summary>
        /// Array of magic item templates filtered to just artifact data.
        /// </summary>
        public MagicItemTemplate[] ArtifactItemTemplates => artifactItemTemplates.ToArray();

        #endregion

        #region Constructors

        public ItemHelper()
        {
            LoadItemTemplates();
            LoadMagicItemTemplates();
            LoadBookIDNameMapping();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a custom usage handler for a given item template index.
        /// Not required for custom items with implementing classes, override UseItem() instead.
        /// </summary>
        /// <param name="templateIndex">Template index of the item, can be either a classic or custom item</param>
        /// <param name="itemUseHander">Implementation of ItemUseHandler delegate to call when item is used</param>
        public void RegisterItemUseHandler(int templateIndex, ItemUseHandler itemUseHandler)
        {
            DaggerfallUnity.LogMessage("RegisterItemUseHandler: TemplateIndex=" + templateIndex);
            itemUseHandlers[templateIndex] = itemUseHandler;
        }

        public bool GetItemUseHandler(int templateIndex, out ItemUseHandler itemUseHandler)
        {
            return itemUseHandlers.TryGetValue(templateIndex, out itemUseHandler);
        }

        /// <summary>
        /// Registers a custom item defined by an item template entry for in-game generation and with a custom implementation class if required.
        /// </summary>
        /// <param name="templateIndex">Template index to use for the item, must match entry in item templates</param>
        /// <param name="itemGroup">Set to assign the item to a group for purposes of loot generation and shop stocking</param>
        /// <param name="itemClassType">Provide the Type of a custom implementation class to be used, which must extend DaggerfallUnityItem</param>
        public void RegisterCustomItem(int templateIndex, ItemGroups itemGroup = ItemGroups.None, Type itemClassType = null)
        {
            if (templateIndex <= LastDFTemplate)
                throw new Exception("RegisterCustomItem: Template index must not be of an existing DF item.");
            if (itemGroup == ItemGroups.None && itemClassType == null)
                throw new Exception("RegisterCustomItem: Nothing specified to register the item .");

            DaggerfallUnity.LogMessage("RegisterCustomItem: TemplateIndex=" + templateIndex + ", Class=" + itemClassType + ", Group=" + itemGroup, true);

            // Register custom item class
            if (itemClassType != null)
            {
                // Register with item collection for de-serialization, no conflicts allowed here
                string itemClassName = itemClassType.ToString();
                if (!ItemCollection.RegisterCustomItem(itemClassName, itemClassType))
                    throw new Exception("RegisterCustomItem: Unable to register the item for de-serialization.");

                customItemTypes[templateIndex] = itemClassType;
                DaggerfallUnity.LogMessage("RegisterCustomItem: TemplateIndex=" + templateIndex + ", Class=" + itemClassType, true);
            }

            // Register custom item group
            if (itemGroup != ItemGroups.None)
            {
                List<int> itemTemplateIndexes;
                if (!customItemGroups.TryGetValue(itemGroup, out itemTemplateIndexes))
                {
                    itemTemplateIndexes = new List<int>();
                    customItemGroups[itemGroup] = itemTemplateIndexes;
                }
                if (!itemTemplateIndexes.Contains(templateIndex))
                {
                    itemTemplateIndexes.Add(templateIndex);
                }
                DaggerfallUnity.LogMessage("RegisterCustomItem: TemplateIndex=" + templateIndex + ", Class=" + itemClassType + ", Group=" + itemGroup, true);
            }
        }

        public bool GetCustomItemClass(int templateIndex, out Type itemClassType)
        {
            return customItemTypes.TryGetValue(templateIndex, out itemClassType);
        }

        public int[] GetCustomItemsForGroup(ItemGroups itemGroup)
        {
            if (customItemGroups.ContainsKey(itemGroup))
                return customItemGroups[itemGroup].ToArray();
            return new int[0];
        }

        /// <summary>
        /// Gets item template data using group and index.
        /// </summary>
        public ItemTemplate GetItemTemplate(ItemGroups itemGroup, int groupIndex)
        {
            // Items added by mods are after last DF template, with groupIndex == templateIndex
            if (groupIndex > LastDFTemplate)
                return GetItemTemplate(groupIndex);

            Array values = GetEnumArray(itemGroup);
            if (groupIndex < 0 || groupIndex >= values.Length)
            {
                string message = string.Format("Item index out of range: Group={0} Index={1}", itemGroup.ToString(), groupIndex);
                Debug.Log(message);
                return new ItemTemplate();
            }

            int templateIndex = Convert.ToInt32(values.GetValue(groupIndex));

            return itemTemplates[templateIndex];
        }

        /// <summary>
        /// Gets item template from direct template index.
        /// </summary>
        public ItemTemplate GetItemTemplate(int templateIndex)
        {
            // Items added by mods are after last DF template
            if (templateIndex > LastDFTemplate)
            {
                for (int i = LastDFTemplate; i < itemTemplates.Count; i++)
                    if (itemTemplates[i].index == templateIndex)
                        return itemTemplates[i];
            }

            if (templateIndex < 0 || templateIndex >= itemTemplates.Count)
            {
                string message = string.Format("Item template index out of range: TemplateIndex={0}", templateIndex);
                Debug.Log(message);
                return new ItemTemplate();
            }

            return itemTemplates[templateIndex];
        }

        /// <summary>
        /// Gets artifact template from magic item template data.
        /// </summary>
        public MagicItemTemplate GetArtifactTemplate(int artifactIndex)
        {
            if (artifactIndex < 0 || artifactIndex >= artifactItemTemplates.Count)
            {
                string message = string.Format("Artifact template index out of range: ArtifactIndex={0}", artifactIndex);
                Debug.Log(message);
                return new MagicItemTemplate();
            }

            return artifactItemTemplates[artifactIndex];
        }

        /// <summary>
        /// Gets item group index from group and template index.
        /// </summary>
        /// <returns>Item group index, or -1 if not found.</returns>
        public int GetGroupIndex(ItemGroups itemGroup, int templateIndex)
        {
            // Items added by mods are after last DF template, and groupIndex == templateIndex
            if (templateIndex > LastDFTemplate)
                return templateIndex;

            Array values = GetEnumArray(itemGroup);
            for (int i = 0; i < values.Length; i++)
            {
                int checkTemplateIndex = Convert.ToInt32(values.GetValue(i));
                if (checkTemplateIndex == templateIndex)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Resolves full item name using parameters like %it.
        /// </summary>
        public string ResolveItemName(DaggerfallUnityItem item)
        {
            // Get item template
            ItemTemplate template = item.ItemTemplate;

            // Return just the template name if item is unidentified.
            if (!item.IsIdentified)
                return template.name;

            // Return the shortName if item is an artifact
            if (item.IsArtifact)
                return item.shortName;

            // Books are handled differently
            if (item.ItemGroup == ItemGroups.Books)
                return DaggerfallUnity.Instance.ItemHelper.GetBookTitle(item.message, item.shortName);

            // Start with base name
            string result = item.shortName;

            // Resolve %it parameter
            if (!string.IsNullOrEmpty(template.name))
                result = result.Replace("%it", template.name);
            else
                Debug.LogErrorFormat("Item template index {0} has a null template.name", template.index);

            return result;
        }

        /// <summary>
        /// Resolves full item name using parameters like %it and material type.
        /// </summary>
        public string ResolveItemLongName(DaggerfallUnityItem item, bool differentiatePlantIngredients = true)
        {
            string result = ResolveItemName(item);

            // Return result without material prefix if item is unidentified or an Artifact.
            if (!item.IsIdentified || item.IsArtifact)
                return result;

            // Differentiate plant ingredients with 2 variants
            if (differentiatePlantIngredients)
            {
                if (item.ItemGroup == ItemGroups.PlantIngredients1 && item.TemplateIndex < 18)
                    return string.Format("{0} {1}", result, TextManager.Instance.GetLocalizedText("northern"));
                if (item.ItemGroup == ItemGroups.PlantIngredients2 && item.TemplateIndex < 18)
                    return string.Format("{0} {1}", result, TextManager.Instance.GetLocalizedText("southern"));
            }

            // Resolve weapon material
            if (item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex != (int)Weapons.Arrow)
            {
                WeaponMaterialTypes weaponMaterial = (WeaponMaterialTypes)item.nativeMaterialValue;
                string materialName = DaggerfallUnity.Instance.TextProvider.GetWeaponMaterialName(weaponMaterial);
                result = string.Format("{0} {1}", materialName, result);
            }

            // Resolve armor material
            if (item.ItemGroup == ItemGroups.Armor && ArmorShouldShowMaterial(item))
            {
                ArmorMaterialTypes armorMaterial = (ArmorMaterialTypes)item.nativeMaterialValue;
                string materialName = DaggerfallUnity.Instance.TextProvider.GetArmorMaterialName(armorMaterial);
                result = string.Format("{0} {1}", materialName, result);
            }

            // Resolve potion names
            if (item.IsPotion)
                return MacroHelper.GetValue("%po", item);

            // Resolve quest letters, get last 2 lines which should be the signoff
            if (item.ItemGroup == ItemGroups.UselessItems2 && item.TemplateIndex == (int)UselessItems2.Parchment && item.IsQuestItem)
            {
                // Get the Item resource from quest
                Quest quest = QuestMachine.Instance.GetQuest(item.QuestUID);
                if (quest != null)
                {
                    Item questItem = quest.GetItem(item.QuestItemSymbol);
                    if (questItem.UsedMessageID >= 0)
                    {
                        Message msg = quest.GetMessage(questItem.UsedMessageID);
                        TextFile.Token[] tokens = msg.GetTextTokens();
                        string signoff = "";
                        int lines = 0;
                        for (int i = tokens.Length-1; i >= 0; i--)
                        {
                            TextFile.Token token = tokens[i];
                            if (!string.IsNullOrEmpty(token.text))
                            {
                                signoff = token.text.Trim() + " " + signoff;
                                lines++;
                            }
                            if (lines >= 2)
                                return TextManager.Instance.GetLocalizedText("letterPrefix") + signoff;
                        }
                    }
                }
            }

            // Show trapped soul name if any
            if (item.ItemGroup == ItemGroups.MiscItems && item.TemplateIndex == (int)MiscItems.Soul_trap)
            {
                if (item.TrappedSoulType != MobileTypes.None)
                {
                    MobileEnemy soul;
                    if (EnemyBasics.GetEnemy(item.TrappedSoulType, out soul))
                    {
                        MobileEnemy mobileEnemy = GameObjectHelper.EnemyDict[(int)item.TrappedSoulType];
                        result += string.Format(" ({0})", TextManager.Instance.GetLocalizedEnemyName(soul.ID));
                    }
                }
                //else
                //{
                //    // Considering showing (empty) for empty soul traps
                //}
            }

            return result;
        }

        // Gets inventory image
        public ImageData GetInventoryImage(DaggerfallUnityItem item)
        {
            if (item.TemplateIndex == (int)Transportation.Small_cart)
            {
                // Handle small cart - the template image for this is not correct
                // Correct image actually in CIF files
                return GetContainerImage(InventoryContainerImages.Wagon);
            }
            else
            {
                // Get inventory image
                return GetItemImage(item, true, false, true);
            }
        }

        /// <summary>
        /// Gets inventory/equip image for specified item.
        /// Image will be cached based on material and hand for faster subsequent fetches.
        /// Animated item images do not support dyes.
        /// </summary>
        /// <param name="item">Item to fetch image for.</param>
        /// <param name="removeMask">Removes mask index (e.g. around helmets) from final image.</param>
        /// <param name="forPaperDoll">Image is for paper doll.</param>
        /// <param name="allowAnimation">Read animated textures.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetItemImage(DaggerfallUnityItem item, bool removeMask = false, bool forPaperDoll = false, bool allowAnimation = false)
        {
            // Get colour
            int color = (int)item.dyeColor;

            // Get archive and record indices
            int archive = item.InventoryTextureArchive;
            int record = item.InventoryTextureRecord;

            // Paper doll handling
            if (forPaperDoll)
            {
                // 1H Weapons in right hand need record + 1
                if (item.ItemGroup == ItemGroups.Weapons && item.EquipSlot == EquipSlots.RightHand &&
                    ItemEquipTable.GetItemHands(item) == ItemHands.Either)
                    record += 1;
            }
            else
            {
                // Katanas need +1 for inventory image as they use right-hand image instead of left
                if (item.IsOfTemplate(ItemGroups.Weapons, (int)Weapons.Katana))
                    record += 1;
            }

            // Use world texture archive if inventory texture not set
            // Examples are gold pieces and wayrest painting
            if (archive == 0 && record == 0)
            {
                archive = item.ItemTemplate.worldTextureArchive;
                record = item.ItemTemplate.worldTextureRecord;
            }

            // Get unique key
            int key = MakeImageKey(color, archive, record, removeMask);

            // Get existing icon if in cache
            if (itemImages.ContainsKey(key))
                return itemImages[key];

            // Load image data
            string filename = TextureFile.IndexToFileName(archive);
            ImageData data = ImageReader.GetImageData(filename, record, 0, true, false, allowAnimation);
            if (data.type == ImageTypes.None)
                throw new Exception("GetItemImage() could not load image data.");

            // Fix items with known incorrect paper doll offsets
            if (archive == 237 && (record == 52 || record == 54))
            {
                // "Short shirt" template index 202 variants 2 and 5 for human female
                data.offset = new DaggerfallConnect.Utility.DFPosition(237, 43);
            }

            // Get mask texture where alpha 0 is umasked areas of image and alpha 1 are masked areas of image
            Texture2D maskTexture;
            if (TextureReplacement.TryImportTexture(archive, record, 0, item.dyeColor, TextureMap.Mask, out maskTexture))
                data.maskTexture = maskTexture;
            else
                ImageReader.UpdateMaskTexture(ref data);

            Texture2D tex;
            if (TextureReplacement.TryImportTexture(archive, record, 0, item.dyeColor, TextureMap.Albedo, out tex))
            {
                // Assign imported texture
                data.texture = tex;
            }
            else
            {
                // Remove mask if requested
                if (removeMask)
                    data.dfBitmap = ImageProcessing.ChangeMask(data.dfBitmap);

                // Change dye or just update texture
                ItemGroups group = item.ItemGroup;
                DyeColors dye = (DyeColors)color;
                if (group == ItemGroups.Weapons || group == ItemGroups.Armor)
                    data = ChangeDye(data, dye, DyeTargets.WeaponsAndArmor);
                else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing)
                    data = ChangeDye(data, dye, DyeTargets.Clothing);
                else
                    ImageReader.UpdateTexture(ref data);
            }

            // Add to cache
            itemImages.Add(key, data);

            return data;
        }

        /// <summary>
        /// Gets item image with custom mask colour.
        /// </summary>
        /// <param name="item">Item to fetch image for.</param>
        /// <param name="maskColor">New mask colour.</param>
        /// <param name="forPaperDoll">Image is for paper doll.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetItemImage(DaggerfallUnityItem item, Color maskColor, bool forPaperDoll = false)
        {
            // Get base item with mask intact
            ImageData result = GetItemImage(item, false, forPaperDoll);
            ImageReader.UpdateTexture(ref result, maskColor);

            return result;
        }

        /// <summary>
        /// Gets name of artifact.
        /// </summary>
        /// <param name="type">Artifact subtype.</param>
        /// <returns>Artifact name.</returns>
        public string GetArtifactName(ArtifactsSubTypes type)
        {
            return artifactItemTemplates[(int)type].name;
        }

        /// <summary>
        /// Gets artifact texture indices
        /// </summary>
        /// <param name="type">Artifact subtype.</param>
        /// <param name="textureArchiveOut">Texture archive out.</param>
        /// <param name="textureRecordOut">Texture record out.</param>
        public void GetArtifactTextureIndices(ArtifactsSubTypes type, out int textureArchiveOut, out int textureRecordOut)
        {
            textureArchiveOut = (GameManager.Instance.PlayerEntity.Gender == Genders.Male) ? artifactMaleTextureArchive : artifactFemaleTextureArchive;
            textureRecordOut = artifactTextureIndexMappings[(int)type];
        }

        /// <summary>
        /// Gets an artifact sub type from an items' short name. (throws exception if no match)
        /// </summary>
        /// <param name="itemShortName">Item short name</param>
        /// <returns>Artifact sub type.</returns>
        public static ArtifactsSubTypes GetArtifactSubType(string itemShortName)
        {
            itemShortName = itemShortName.Replace("\'", "").Replace(' ', '_');
            foreach (var artifactName in Enum.GetNames(typeof(ArtifactsSubTypes)))
            {
                if (itemShortName.Contains(artifactName))
                    return (ArtifactsSubTypes)Enum.Parse(typeof(ArtifactsSubTypes), artifactName);
            }
            throw new KeyNotFoundException("No match found for: " + itemShortName);
        }

        /// <summary>
        /// Gets the title of a book from its ID.
        /// </summary>
        /// <param name="id">The book's ID</param>
        /// <param name="defaultBookTitle">The name the book should default to if the lookup fails. (Usually the Item's LongName..."Book" or "Parchment")</param>
        /// <returns>The title of the bookd or defaultBookName if no name was found.</returns>
        public string GetBookTitle(int id, string defaultBookTitle)
        {
            // Get cached localized book title if previously read
            if (localizedBookIDNameMapping.ContainsKey(id))
                return localizedBookIDNameMapping[id];

            // Get book title from localized book file as first preference
            // Localized title will be cached so file is only read once
            string filename = GetBookFileName(id);
            LocalizedBook localizedBook = new LocalizedBook();
            if (localizedBook.OpenLocalizedBookFile(filename))
            {
                localizedBookIDNameMapping.Add(id, localizedBook.Title);
                return localizedBook.Title;
            }

            // Fallback to legacy data
            string title;
            return bookIDNameMapping.TryGetValue(id, out title) ? title : defaultBookTitle;
        }

        /// <summary>
        /// Gets the filename for a book (classic or imported).
        /// </summary>
        /// <param name="id">The book's ID</param>
        /// <returns>The filename of the book or null.</returns>
        internal string GetBookFileName(int id)
        {
            // Get name for custom book
            BookMappingEntry entry;
            if (BookReplacement.BookMappingEntries.TryGetValue(id, out entry))
                return entry.Name;

            // Check if classic book and make name from id
            if (bookIDNameMapping.ContainsKey(id))
                return BookFile.messageToBookFilename(id);

            Debug.LogErrorFormat("ID {0} is not assigned to any known book; a mod that provides books was probably removed.", id);
            return null;
        }

        /// <summary>
        /// Obtaining a random book ID is useful for generating books in loot drops, store inventories, etc.
        /// If custom books are available, they are chosen only if specified conditions are met.
        /// </summary>
        /// <returns>A random book ID</returns>
        public int GetRandomBookID()
        {
            const int attempts = 6;

            int[] keys = bookIDNameMapping.Keys.ToArray();

            for (int i = 0; i < attempts; i++)
            {
                int id = keys[UnityEngine.Random.Range(0, keys.Length)];
                if (BookReplacement.BookMeetsConditions(id))
                    return id;
            }

            return keys.First(x => !BookReplacement.BookMappingEntries.ContainsKey(x));
        }

        /// <summary>
        /// Gets icon for a container object, such as the wagon.
        /// </summary>
        /// <param name="type">Container type.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetContainerImage(InventoryContainerImages type)
        {
            // Get existing icon if in cache
            if (containerImages.ContainsKey(type))
                return containerImages[type];

            // Load image data
            ImageData data = ImageReader.GetImageData(containerIconsFilename, (int)type, 0, true, true);

            // Add to cache
            containerImages.Add(type, data);

            return data;
        }

        /// <summary>
        /// Helper to get interior parts of cloak.
        /// This is not cached as only seen on paper doll during refresh.
        /// </summary>
        /// <param name="item">Item - must be a formal or casual cloak.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetCloakInteriorImage(DaggerfallUnityItem item)
        {
            // Must be a formal or casual cloak
            if (!IsCloak(item))
                return new ImageData();

            // Get colour
            int color = (int)item.dyeColor;

            // Cloak interior source is combination of player texture archive index and template record index
            int archive = item.InventoryTextureArchive;
            int record = item.ItemTemplate.playerTextureRecord;

            // Load image data
            string filename = TextureFile.IndexToFileName(archive);
            ImageData data = ImageReader.GetImageData(filename, record, 0, true, false);
            if (data.type == ImageTypes.None)
                throw new Exception("GetCloakBackImage() could not load image data.");

            Texture2D tex;
            if (TextureReplacement.TryImportTexture(archive, record, 0, item.dyeColor, TextureMap.Albedo, out tex))
            {
                // Assign imported texture
                data.texture = tex;
            }
            else
            {
                // Change dye
                data = ChangeDye(data, (DyeColors)color, DyeTargets.Clothing);
            }    

            return data;
        }

        /// <summary>
        /// Checks if this item is a formal or casual cloak.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if formal or casual cloak.</returns>
        public bool IsCloak(DaggerfallUnityItem item)
        {
            // Must be a formal or casual cloak
            switch (item.TemplateIndex)
            {
                case (int)MensClothing.Formal_cloak:
                case (int)MensClothing.Casual_cloak:
                case (int)WomensClothing.Formal_cloak:
                case (int)WomensClothing.Casual_cloak:
                    return true;
                default:
                    return false;
            }
        }

        public static TextFile.Token[] GetItemInfo(DaggerfallUnityItem item, ITextProvider textProvider)
        {
            const int paintingTextId = 250;
            const int armorTextId = 1000;
            const int weaponTextId = 1001;
            const int miscTextId = 1003;
            const int soulTrapTextId = 1004;
            const int letterOfCreditTextId = 1007;
            const int potionTextId = 1008;
            const int bookTextId = 1009;
            const int arrowTextId = 1011;
            const int weaponNoMaterialTextId = 1012;
            const int armorNoMaterialTextId = 1014;
            const int oghmaInfiniumTextId = 1015;
            const int houseDeedTextId = 1073;

            // Handle by item group
            switch (item.ItemGroup)
            {
                case ItemGroups.Armor:
                    if (ArmorShouldShowMaterial(item))
                        return textProvider.GetRSCTokens(armorTextId); // Handle armor showing material
                    else
                        return textProvider.GetRSCTokens(armorNoMaterialTextId); // Handle armor not showing material

                case ItemGroups.Weapons:
                    if (item.TemplateIndex == (int)Weapons.Arrow)
                        return textProvider.GetRSCTokens(arrowTextId);              // Handle arrows
                    else if (item.IsArtifact)
                        return textProvider.GetRSCTokens(weaponNoMaterialTextId);   // Handle artifacts
                    else
                        return textProvider.GetRSCTokens(weaponTextId);             // Handle weapons

                case ItemGroups.Books:
                    if (item.legacyMagic != null && item.legacyMagic[0].type == EnchantmentTypes.SpecialArtifactEffect)
                        return textProvider.GetRSCTokens(oghmaInfiniumTextId);      // Handle Oghma Infinium
                    else
                        return textProvider.GetRSCTokens(bookTextId);               // Handle other books

                case ItemGroups.Paintings:
                    // Show painting. (Uses file paint.dat)
                    return item.InitPaintingInfo(paintingTextId);

                case ItemGroups.MiscItems:
                    // A few items in the MiscItems group have their own text display
                    if (item.IsPotionRecipe)
                        return GetPotionRecipeTokens();                             // Handle potion recipes
                    else if (item.TemplateIndex == (int)MiscItems.House_Deed)
                        return textProvider.GetRSCTokens(houseDeedTextId);          // Handle house deeds
                    else if (item.TemplateIndex == (int)MiscItems.Soul_trap)
                        return textProvider.GetRSCTokens(soulTrapTextId);           // Handle soul traps
                    else if (item.TemplateIndex == (int)MiscItems.Letter_of_credit)
                        return textProvider.GetRSCTokens(letterOfCreditTextId);     // Handle letters of credit
                    else
                        return textProvider.GetRSCTokens(miscTextId);               // Default misc items

                default:
                    // Handle potions in glass bottles
                    // In classic, the check is whether RecordRoot.SublistHead is non-null and of PotionMix type.
                    if (item.IsPotion)
                        return textProvider.GetRSCTokens(potionTextId);

                    // Handle Azura's Star
                    if (item.legacyMagic != null && item.legacyMagic[0].type == EnchantmentTypes.SpecialArtifactEffect && item.legacyMagic[0].param == 9)
                        return textProvider.GetRSCTokens(soulTrapTextId);

                    // Default fallback if none of the above applied
                    return textProvider.GetRSCTokens(miscTextId);
            }
        }

        /// <summary>
        /// Returns whether an armor item should show its material in info popups and tooltips
        /// </summary>
        private static bool ArmorShouldShowMaterial(DaggerfallUnityItem item)
        {
            // HelmAndShieldMaterialDisplay setting for showing material for helms and shields:
            // 0 : Don't show (classic behavior)
            // 1 : Show for all but leather and chain
            // 2 : Show for all but leather
            // 3 : Show for all
            if (item.IsArtifact)
                return false;
            else if (item.IsShield || item.TemplateIndex == (int)Armor.Helm)
            {
                if ((DaggerfallUnity.Settings.HelmAndShieldMaterialDisplay == 1)
                    && ((ArmorMaterialTypes)item.nativeMaterialValue >= ArmorMaterialTypes.Iron))
                    return true;
                else if ((DaggerfallUnity.Settings.HelmAndShieldMaterialDisplay == 2)
                    && ((ArmorMaterialTypes)item.nativeMaterialValue >= ArmorMaterialTypes.Chain))
                    return true;
                else if (DaggerfallUnity.Settings.HelmAndShieldMaterialDisplay == 3)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        // TODO: can this be replaced with a new text RSC entry?
        private static TextFile.Token[] GetPotionRecipeTokens()
        {
            TextFile.Token[] tokens = new TextFile.Token[4];
            tokens[0] = TextFile.CreateTextToken(TextManager.Instance.GetLocalizedText("potionRecipeFor"));
            tokens[1] = TextFile.CreateFormatToken(TextFile.Formatting.JustifyCenter);
            tokens[2] = TextFile.CreateTextToken("Weight: %kg kilograms");
            tokens[3] = TextFile.CreateFormatToken(TextFile.Formatting.JustifyCenter);
            return tokens;
        }

        /// <summary>
        /// Converts Daggerfall weapon to generic API WeaponType.
        /// </summary>
        /// <param name="item">Weapon to convert.</param>
        /// <returns>WeaponTypes.</returns>
        public WeaponTypes ConvertItemToAPIWeaponType(DaggerfallUnityItem item)
        {
            // Must be a weapon
            if (item.ItemGroup != ItemGroups.Weapons)
                return WeaponTypes.None;

            // Find FPS animation set for this weapon type
            // Daggerfall re-uses the same animations for many different weapons

            // Check for a custom item weapon type, if None then continue
            WeaponTypes result = item.GetWeaponType();
            if (result != WeaponTypes.None)
                return result;

            switch (item.TemplateIndex)
            {
                case (int)Weapons.Dagger:
                    result = WeaponTypes.Dagger;
                    break;
                case (int)Weapons.Staff:
                    result = WeaponTypes.Staff;
                    break;
                case (int)Weapons.Tanto:
                case (int)Weapons.Shortsword:
                case (int)Weapons.Wakazashi:
                case (int)Weapons.Broadsword:
                case (int)Weapons.Saber:
                case (int)Weapons.Longsword:
                case (int)Weapons.Katana:
                case (int)Weapons.Claymore:
                case (int)Weapons.Dai_Katana:
                    result = WeaponTypes.LongBlade;
                    break;
                case (int)Weapons.Mace:
                    result = WeaponTypes.Mace;
                    break;
                case (int)Weapons.Flail:
                    result = WeaponTypes.Flail;
                    break;
                case (int)Weapons.Warhammer:
                    result = WeaponTypes.Warhammer;
                    break;
                case (int)Weapons.Battle_Axe:
                case (int)Weapons.War_Axe:
                    result = WeaponTypes.Battleaxe;
                    break;
                case (int)Weapons.Short_Bow:
                case (int)Weapons.Long_Bow:
                    result = WeaponTypes.Bow;
                    break;
                default:
                    return WeaponTypes.None;
            }

            // Handle enchanted weapons
            if (item.IsEnchanted)
            {
                switch (result)
                {
                    case WeaponTypes.Dagger:
                        result = WeaponTypes.Dagger_Magic;
                        break;
                    case WeaponTypes.Staff:
                        result = WeaponTypes.Staff_Magic;
                        break;
                    case WeaponTypes.LongBlade:
                        result = WeaponTypes.LongBlade_Magic;
                        break;
                    case WeaponTypes.Mace:
                        result = WeaponTypes.Mace_Magic;
                        break;
                    case WeaponTypes.Flail:
                        result = WeaponTypes.Flail_Magic;
                        break;
                    case WeaponTypes.Warhammer:
                        result = WeaponTypes.Warhammer_Magic;
                        break;
                    case WeaponTypes.Battleaxe:
                        result = WeaponTypes.Battleaxe_Magic;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts native Daggerfall weapon and armor material types to generic Daggerfall Unity MetalType.
        /// The old metal type enum may be retired as true materials become better integrated.
        /// </summary>
        /// <param name="item">Item to convert material to metal type.</param>
        /// <returns>MetalTypes.</returns>
        public MetalTypes ConvertItemMaterialToAPIMetalType(DaggerfallUnityItem item)
        {
            // Determine metal type
            if (item.ItemGroup == ItemGroups.Weapons)
            {
                // Overrides for artifacts whose dyes do not match their materials
                if (item.IsArtifact)
                {
                    foreach (DaggerfallEnchantment enchantment in item.LegacyEnchantments)
                    {
                        if (enchantment.type == EnchantmentTypes.SpecialArtifactEffect)
                        {
                            switch (enchantment.param)
                            {
                                case (int)ArtifactsSubTypes.Mehrunes_Razor: // Different from classic but Elven matches the paper doll more closely
                                    return MetalTypes.Elven;
                                case (int)ArtifactsSubTypes.Mace_of_Molag_Bal:
                                    return MetalTypes.Ebony;
                                case (int)ArtifactsSubTypes.Wabbajack:
                                    return MetalTypes.Dwarven;
                                default:
                                    break;
                            }
                        }
                    }
                    // Artifact weapons with no unique effects
                    if (item.ItemName == "Chrysamere")
                        return MetalTypes.Mithril;
                    if (item.ItemName == "Staff of Magnus")
                        return MetalTypes.Mithril;
                }
                WeaponMaterialTypes weaponMaterial = (WeaponMaterialTypes)item.nativeMaterialValue;
                switch (weaponMaterial)
                {
                    case WeaponMaterialTypes.Iron:
                        return MetalTypes.Iron;
                    case WeaponMaterialTypes.Steel:
                        return MetalTypes.Steel;
                    case WeaponMaterialTypes.Silver:
                        return MetalTypes.Silver;
                    case WeaponMaterialTypes.Elven:
                        return MetalTypes.Elven;
                    case WeaponMaterialTypes.Dwarven:
                        return MetalTypes.Dwarven;
                    case WeaponMaterialTypes.Mithril:
                        return MetalTypes.Mithril;
                    case WeaponMaterialTypes.Adamantium:
                        return MetalTypes.Adamantium;
                    case WeaponMaterialTypes.Ebony:
                        return MetalTypes.Ebony;
                    case WeaponMaterialTypes.Orcish:
                        return MetalTypes.Orcish;
                    case WeaponMaterialTypes.Daedric:
                        return MetalTypes.Daedric;
                    default:
                        return MetalTypes.None;
                }
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                ArmorMaterialTypes armorMaterial = (ArmorMaterialTypes)item.nativeMaterialValue;
                switch (armorMaterial)
                {
                    case ArmorMaterialTypes.Iron:
                        return MetalTypes.Iron;
                    case ArmorMaterialTypes.Steel:
                        return MetalTypes.Steel;
                    case ArmorMaterialTypes.Silver:
                        return MetalTypes.Silver;
                    case ArmorMaterialTypes.Elven:
                        return MetalTypes.Elven;
                    case ArmorMaterialTypes.Dwarven:
                        return MetalTypes.Dwarven;
                    case ArmorMaterialTypes.Mithril:
                        return MetalTypes.Mithril;
                    case ArmorMaterialTypes.Adamantium:
                        return MetalTypes.Adamantium;
                    case ArmorMaterialTypes.Ebony:
                        return MetalTypes.Ebony;
                    case ArmorMaterialTypes.Orcish:
                        return MetalTypes.Orcish;
                    case ArmorMaterialTypes.Daedric:
                        return MetalTypes.Daedric;
                    default:
                        return MetalTypes.None;
                }
            }
            else
            {
                return MetalTypes.None;
            }
        }

        /// <summary>
        /// Gets dye color automatically from item data.
        /// </summary>
        /// <param name="item">DaggerfallUnityItem.</param>
        /// <returns>DyeColors.</returns>
        public DyeColors GetDyeColor(DaggerfallUnityItem item)
        {

            if (item.ItemGroup == ItemGroups.Weapons)
                return GetWeaponDyeColor((WeaponMaterialTypes)item.nativeMaterialValue);
            else if (item.ItemGroup == ItemGroups.Armor)
                return GetArmorDyeColor((ArmorMaterialTypes)item.nativeMaterialValue);
            else
                return item.dyeColor;
        }

        /// <summary>
        /// Converts weapon material to appropriate dye colour.
        /// </summary>
        /// <param name="material">WeaponMaterialTypes.</param>
        /// <returns>DyeColors.</returns>
        public DyeColors GetWeaponDyeColor(WeaponMaterialTypes material)
        {
            switch (material)
            {
                case WeaponMaterialTypes.Iron:
                    return DyeColors.Iron;
                case WeaponMaterialTypes.Steel:
                    return DyeColors.Steel;
                case WeaponMaterialTypes.Silver:
                    return DyeColors.Silver;
                case WeaponMaterialTypes.Elven:
                    return DyeColors.Elven;
                case WeaponMaterialTypes.Dwarven:
                    return DyeColors.Dwarven;
                case WeaponMaterialTypes.Mithril:
                    return DyeColors.Mithril;
                case WeaponMaterialTypes.Adamantium:
                    return DyeColors.Adamantium;
                case WeaponMaterialTypes.Ebony:
                    return DyeColors.Ebony;
                case WeaponMaterialTypes.Orcish:
                    return DyeColors.Orcish;
                case WeaponMaterialTypes.Daedric:
                    return DyeColors.Daedric;
                default:
                    return DyeColors.Unchanged;

            }
        }

        /// <summary>
        /// Converts armor material to appropriate dye colour.
        /// </summary>
        /// <param name="material">ArmorMaterialTypes.</param>
        /// <returns>DyeColors.</returns>
        public DyeColors GetArmorDyeColor(ArmorMaterialTypes material)
        {
            switch (material)
            {
                case ArmorMaterialTypes.Iron:
                    return DyeColors.Iron;
                case ArmorMaterialTypes.Steel:
                    return DyeColors.Steel;
                case ArmorMaterialTypes.Silver:
                    return DyeColors.Silver;
                case ArmorMaterialTypes.Elven:
                    return DyeColors.Elven;
                case ArmorMaterialTypes.Dwarven:
                    return DyeColors.Dwarven;
                case ArmorMaterialTypes.Mithril:
                    return DyeColors.Mithril;
                case ArmorMaterialTypes.Adamantium:
                    return DyeColors.Adamantium;
                case ArmorMaterialTypes.Ebony:
                    return DyeColors.Ebony;
                case ArmorMaterialTypes.Orcish:
                    return DyeColors.Orcish;
                case ArmorMaterialTypes.Daedric:
                    return DyeColors.Daedric;
                default:
                    return DyeColors.Unchanged;

            }
        }

        ///// <summary>
        ///// Checks legacy item ID against legacy equip index to determine if item is equipped to a slot.
        ///// Must have previously used SetLegacyEquipTable to EntityItems provided.
        ///// </summary>
        ///// <param name="item">Item to test.</param>
        ///// <param name="entityItems">EntityItems with legacy equip table set.</param>
        ///// <returns>Index of item in equip table or -1 if not equipped.</returns>
        //public int GetLegacyEquipIndex(DaggerfallUnityItem item, EntityItems entityItems)
        //{
        //    // Interim use of classic data
        //    //ItemRecord.ItemRecordData itemRecord = item.ItemRecord.ParsedData;
        //    uint[] legacyEquipTable = (GameManager.Instance.PlayerEntity as DaggerfallEntity).Items.LegacyEquipTable;
        //    if (legacyEquipTable == null || legacyEquipTable.Length == 0)
        //        return -1;

        //    // Try to match item RecordID with equipped item IDs
        //    // Item RecordID must be shifted right 8 bits
        //    for (int i = 0; i < legacyEquipTable.Length; i++)
        //    {
        //        if (legacyEquipTable[i] == (item.ItemRecord.RecordRoot.RecordID >> 8))
        //            return i;
        //    }

        //    return -1;
        //}

        /// <summary>
        /// Helps bridge classic item index pair back to item template index.
        /// </summary>
        /// <param name="group">Group enum to retrieve.</param>
        /// <return>Array of group enum values.</return>
        public Array GetEnumArray(ItemGroups group)
        {
            switch (group)
            {
                case ItemGroups.Drugs:
                    return Enum.GetValues(typeof(Drugs));
                case ItemGroups.UselessItems1:
                    return Enum.GetValues(typeof(UselessItems1));
                case ItemGroups.Armor:
                    return Enum.GetValues(typeof(Armor));
                case ItemGroups.Weapons:
                    return Enum.GetValues(typeof(Weapons));
                case ItemGroups.MagicItems:
                    return Enum.GetValues(typeof(MagicItemSubTypes));
                case ItemGroups.Artifacts:
                    return Enum.GetValues(typeof(ArtifactsSubTypes));
                case ItemGroups.MensClothing:
                    return Enum.GetValues(typeof(MensClothing));
                case ItemGroups.Books:
                    return Enum.GetValues(typeof(Books));
                case ItemGroups.Furniture:
                    return Enum.GetValues(typeof(Furniture));
                case ItemGroups.UselessItems2:
                    return Enum.GetValues(typeof(UselessItems2));
                case ItemGroups.ReligiousItems:
                    return Enum.GetValues(typeof(ReligiousItems));
                case ItemGroups.Maps:
                    return Enum.GetValues(typeof(Maps));
                case ItemGroups.WomensClothing:
                    return Enum.GetValues(typeof(WomensClothing));
                case ItemGroups.Paintings:
                    return Enum.GetValues(typeof(Paintings));
                case ItemGroups.Gems:
                    return Enum.GetValues(typeof(Gems));
                case ItemGroups.PlantIngredients1:
                    return Enum.GetValues(typeof(PlantIngredients1));
                case ItemGroups.PlantIngredients2:
                    return Enum.GetValues(typeof(PlantIngredients2));
                case ItemGroups.CreatureIngredients1:
                    return Enum.GetValues(typeof(CreatureIngredients1));
                case ItemGroups.CreatureIngredients2:
                    return Enum.GetValues(typeof(CreatureIngredients2));
                case ItemGroups.CreatureIngredients3:
                    return Enum.GetValues(typeof(CreatureIngredients3));
                case ItemGroups.MiscellaneousIngredients1:
                    return Enum.GetValues(typeof(MiscellaneousIngredients1));
                case ItemGroups.MetalIngredients:
                    return Enum.GetValues(typeof(MetalIngredients));
                case ItemGroups.MiscellaneousIngredients2:
                    return Enum.GetValues(typeof(MiscellaneousIngredients2));
                case ItemGroups.Transportation:
                    return Enum.GetValues(typeof(Transportation));
                case ItemGroups.Deeds:
                    return Enum.GetValues(typeof(Deeds));
                case ItemGroups.Jewellery:
                    return Enum.GetValues(typeof(Jewellery));
                case ItemGroups.QuestItems:
                    return Enum.GetValues(typeof(QuestItems));
                case ItemGroups.MiscItems:
                    return Enum.GetValues(typeof(MiscItems));
                case ItemGroups.Currency:
                    return Enum.GetValues(typeof(Currency));
                default:
                    throw new Exception("Error: Item group not found.");
            }
        }

        /// <summary>
        /// Ensures that a player has a valid spellbook item on load.
        /// </summary>
        public void ValidateSpellbookItem(PlayerEntity playerEntity)
        {
            ItemCollection items = playerEntity.Items;

            // Importing a classic save can result in a spellbook with itemgroup 0
            // This ends up being sorted into ingredients in DFU
            DaggerfallUnityItem badSpellbook = items.GetItem(ItemGroups.MiscItems, 0);
            if (badSpellbook != null)
            {
                // Double-check this is a bad spellbook and not actually a ruby by using texture indices
                if (badSpellbook.InventoryTextureArchive == 209 && badSpellbook.InventoryTextureRecord == 4)
                {
                    // Remove bad spellbook and assign a new one correctly from template
                    items.RemoveItem(badSpellbook);
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook));
                }
            }
        }

        /// <summary>
        /// Gives a new spellbook item to player entity (if they don't already have one).
        /// </summary>
        /// <param name="playerEntity">Player entity to receive spellbook item.</param>
        /// <returns>True if spellbook added, false is player already has a spellbook item.</returns>
        public bool AddSpellbookItem(PlayerEntity playerEntity)
        {
            ItemCollection items = playerEntity.Items;
            DaggerfallUnityItem spellbook = items.GetItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook, allowQuestItem: false);
            if (spellbook == null)
            {
                items.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Assigns basic starting gear to a new character.
        /// </summary>
        public void AssignStartingGear(PlayerEntity playerEntity, CharacterDocument characterDocument)
        {
            // Get references
            ItemCollection items = playerEntity.Items;
            ItemEquipTable equipTable = playerEntity.ItemEquipTable;

            // Starting clothes are gender-specific
            DaggerfallUnityItem shortShirt = null;
            DaggerfallUnityItem casualPants = null;
            if (playerEntity.Gender == Genders.Female)
            {
                shortShirt = ItemBuilder.CreateWomensClothing(WomensClothing.Short_shirt_closed, playerEntity.Race, 0);
                casualPants = ItemBuilder.CreateWomensClothing(WomensClothing.Casual_pants, playerEntity.Race);
            }
            else
            {
                shortShirt = ItemBuilder.CreateMensClothing(MensClothing.Short_shirt, playerEntity.Race, 0);
                casualPants = ItemBuilder.CreateMensClothing(MensClothing.Casual_pants, playerEntity.Race);
            }

            // Randomise shirt dye and pants variant
            shortShirt.dyeColor = ItemBuilder.RandomClothingDye();
            ItemBuilder.RandomizeClothingVariant(casualPants);

            // Add spellbook, all players start with one
            items.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook));

            // Add and equip clothing
            items.AddItem(shortShirt);
            items.AddItem(casualPants);
            equipTable.EquipItem(shortShirt, true, false);
            equipTable.EquipItem(casualPants, true, false);

            if (!characterDocument.isCustom)
            {
                // Add class-specific starting weapon
                Weapons[] StartingWeaponTypesByClass = { Weapons.Shortsword, // Mage
                                                         Weapons.Saber,      // Spellsword
                                                         Weapons.Saber,      // Battlemage
                                                         Weapons.Shortsword, // Sorcerer
                                                         Weapons.Mace,       // Healer
                                                         Weapons.Shortsword, // Nightblade
                                                         Weapons.Shortsword, // Bard
                                                         Weapons.Tanto,      // Burglar
                                                         Weapons.Saber,      // Rogue
                                                         Weapons.Shortsword, // Acrobat
                                                         Weapons.Shortsword, // Thief
                                                         Weapons.Longsword,  // Assassin
                                                         Weapons.Staff,      // Monk
                                                         Weapons.Long_Bow,   // Archer
                                                         Weapons.Battle_Axe, // Ranger
                                                         Weapons.Warhammer,  // Barbarian
                                                         Weapons.Broadsword, // Warrior
                                                         Weapons.Longsword   // Knight
                };
                byte[] StartingWeaponMaterialsByClass = { 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0 }; // 0 = iron, 1 = steel
                int classIndex = characterDocument.classIndex;
                items.AddItem(ItemBuilder.CreateWeapon(StartingWeaponTypesByClass[classIndex], (WeaponMaterialTypes)StartingWeaponMaterialsByClass[classIndex]));

                // Archer also gets a steel battleaxe and some arrows
                const int archerIndex = 13;
                const int archerArrows = 24;
                if (classIndex == archerIndex)
                {
                    items.AddItem(ItemBuilder.CreateWeapon(Weapons.Battle_Axe, WeaponMaterialTypes.Steel));
                    DaggerfallUnityItem arrowPile = ItemBuilder.CreateWeapon(Weapons.Arrow, WeaponMaterialTypes.Iron);
                    arrowPile.stackCount = archerArrows;
                    items.AddItem(arrowPile);
                }
            }
            else
            {
                // Custom classes only get an iron longsword
                items.AddItem(ItemBuilder.CreateWeapon(Weapons.Longsword, WeaponMaterialTypes.Iron));
            }

            // Add some starting gold
            playerEntity.GoldPieces += 100;

            // Add some torches and candles if player torch is from items setting enabled
            if (DaggerfallUnity.Settings.PlayerTorchFromItems)
            {
                for (int i=0; i < 5; i++)
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Torch));
                for (int i=0; i < 2; i++)
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Candle));
            }
        }

        public void AssignEnemyStartingEquipment(PlayerEntity player, EnemyEntity enemyEntity, int variant)
        {
            int itemLevel = player.Level;
            Genders playerGender = player.Gender;
            Races race = player.Race;
            int chance = 0;

            // City watch never have items above iron or steel
            if (enemyEntity.EntityType == EntityTypes.EnemyClass && enemyEntity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                itemLevel = 1;

            if (variant == 0)
            {
                // right-hand weapon
                int item = UnityEngine.Random.Range((int)Weapons.Broadsword, (int)(Weapons.Longsword) + 1);
                DaggerfallUnityItem weapon = ItemBuilder.CreateWeapon((Weapons)item, FormulaHelper.RandomMaterial(itemLevel));
                enemyEntity.ItemEquipTable.EquipItem(weapon, true, false);
                enemyEntity.Items.AddItem(weapon);

                chance = 50;

                // left-hand shield
                item = UnityEngine.Random.Range((int)Armor.Buckler, (int)(Armor.Round_Shield) + 1);
                if (Dice100.SuccessRoll(chance))
                {
                    DaggerfallUnityItem armor = ItemBuilder.CreateArmor(playerGender, race, (Items.Armor)item, FormulaHelper.RandomArmorMaterial(itemLevel));
                    enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                    enemyEntity.Items.AddItem(armor);
                }
                // left-hand weapon
                else if (Dice100.SuccessRoll(chance))
                {
                    item = UnityEngine.Random.Range((int)Weapons.Dagger, (int)(Weapons.Shortsword) + 1);
                    weapon = ItemBuilder.CreateWeapon((Weapons)item, FormulaHelper.RandomMaterial(itemLevel));
                    enemyEntity.ItemEquipTable.EquipItem(weapon, true, false);
                    enemyEntity.Items.AddItem(weapon);
                }
            }
            else
            {
                // right-hand weapon
                int item = UnityEngine.Random.Range((int)Weapons.Claymore, (int)(Weapons.Battle_Axe) + 1);
                DaggerfallUnityItem weapon = ItemBuilder.CreateWeapon((Weapons)item, FormulaHelper.RandomMaterial(itemLevel));
                enemyEntity.ItemEquipTable.EquipItem(weapon, true, false);
                enemyEntity.Items.AddItem(weapon);

                if (variant == 1)
                    chance = 75;
                else if (variant == 2)
                    chance = 90;
            }
            // helm
            if (Dice100.SuccessRoll(chance))
            {
                DaggerfallUnityItem armor = ItemBuilder.CreateArmor(playerGender, race, Armor.Helm, FormulaHelper.RandomArmorMaterial(itemLevel));
                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            // right pauldron
            if (Dice100.SuccessRoll(chance))
            {
                DaggerfallUnityItem armor = ItemBuilder.CreateArmor(playerGender, race, Armor.Right_Pauldron, FormulaHelper.RandomArmorMaterial(itemLevel));
                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            // left pauldron
            if (Dice100.SuccessRoll(chance))
            {
                DaggerfallUnityItem armor = ItemBuilder.CreateArmor(playerGender, race, Armor.Left_Pauldron, FormulaHelper.RandomArmorMaterial(itemLevel));
                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            // cuirass
            if (Dice100.SuccessRoll(chance))
            {
                DaggerfallUnityItem armor = ItemBuilder.CreateArmor(playerGender, race, Armor.Cuirass, FormulaHelper.RandomArmorMaterial(itemLevel));
                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            // greaves
            if (Dice100.SuccessRoll(chance))
            {
                DaggerfallUnityItem armor = ItemBuilder.CreateArmor(playerGender, race, Armor.Greaves, FormulaHelper.RandomArmorMaterial(itemLevel));
                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }
            // boots
            if (Dice100.SuccessRoll(chance))
            {
                DaggerfallUnityItem armor = ItemBuilder.CreateArmor(playerGender, race, Armor.Boots, FormulaHelper.RandomArmorMaterial(itemLevel));
                enemyEntity.ItemEquipTable.EquipItem(armor, true, false);
                enemyEntity.Items.AddItem(armor);
            }

            // Chance for poisoned weapon
            if (player.Level > 1)
            {
                DaggerfallUnityItem weapon = enemyEntity.ItemEquipTable.GetItem(EquipSlots.RightHand);
                if (weapon != null && (enemyEntity.EntityType == EntityTypes.EnemyClass || enemyEntity.MobileEnemy.ID == (int)MobileTypes.Orc
                        || enemyEntity.MobileEnemy.ID == (int)MobileTypes.Centaur || enemyEntity.MobileEnemy.ID == (int)MobileTypes.OrcSergeant))
                {
                    int chanceToPoison = 5;
                    if (enemyEntity.MobileEnemy.ID == (int)MobileTypes.Assassin)
                        chanceToPoison = 60;

                    if (Dice100.SuccessRoll(chanceToPoison))
                    {
                        // Apply poison
                        weapon.poisonType = (Items.Poisons)UnityEngine.Random.Range(128, 135 + 1);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads item templates from JSON file.
        /// This data was exported from Daggerfall's FALL.EXE file.
        /// </summary>
        void LoadItemTemplates()
        {
            try
            {
                TextAsset templates = Resources.Load<TextAsset>(itemTemplatesFilename);
                itemTemplates = SaveLoadManager.Deserialize(typeof(List<ItemTemplate>), templates.text) as List<ItemTemplate>;
                TextAssetReader.Merge(itemTemplates, "ItemTemplates.json", (item, data) => item.index == (int)data["index"].AsInt64);
            }
            catch
            {
                Debug.LogError("Could not load ItemTemplates database from Resources. Check file exists and is in correct format.");
            }
        }

        void LoadMagicItemTemplates()
        {
            try
            {
                // Get full list of magic items
                TextAsset templates = Resources.Load<TextAsset>(magicItemTemplatesFilename);
                allMagicItemTemplates = SaveLoadManager.Deserialize(typeof(List<MagicItemTemplate>), templates.text) as List<MagicItemTemplate>;

                // Create list of just artifact item
                artifactItemTemplates = new List<MagicItemTemplate>();
                for (int i = 0; i < allMagicItemTemplates.Count; i++)
                {
                    if (allMagicItemTemplates[i].type == MagicItemTypes.ArtifactClass1 ||
                        allMagicItemTemplates[i].type == MagicItemTypes.ArtifactClass2)
                    {
                        artifactItemTemplates.Add(allMagicItemTemplates[i]);
                    }
                }
            }
            catch
            {
                Debug.LogError("Could not load MagicItemTemplates database from Resources. Check file exists and is in correct format.");
            }
        }

        /// <summary>
        /// Loads book ID-name mappings from JSON file. This is used whenever you need to look up a title by book ID (message & 0xFF)
        /// It should be called once to initilaize the internal data structures used for book-related helper functions.
        /// This data was obtained by the filenames in ARENA2/BOOKS and the title field.
        /// </summary>
        void LoadBookIDNameMapping()
        {
            try
            {
                TextAsset bookNames = Resources.Load<TextAsset>(bookMappingFilename);
                List<BookMappingTemplate> mappings = SaveLoadManager.Deserialize(typeof(List<BookMappingTemplate>), bookNames.text) as List<BookMappingTemplate>;
                foreach (BookMappingTemplate entry in mappings)
                {
                    bookIDNameMapping.Add(entry.id, entry.title);
                }
            }
            catch
            {
                Debug.Log("Could not load the BookIDName mapping from Resources. Check file exists and is in correct format.");
            }

            if (DaggerfallUnity.Settings.AssetInjection)
                BookReplacement.FindAdditionalBooks(bookIDNameMapping);
        }

        /// <summary>
        /// Makes a unique image key based on item variables.
        /// </summary>
        int MakeImageKey(int color, int archive, int record, bool removeMask)
        {
            int mask = (removeMask) ? 1 : 0;

            // 5 bits for color
            // 9 bits for archive
            // 7 bits for record
            // 1 bits for mask
            return (color << 27) + (archive << 18) + (record << 11) + (mask << 10);
        }

        /// <summary>
        /// Assigns a new Texture2D based on dye colour.
        /// </summary>
        public ImageData ChangeDye(ImageData imageData, DyeColors dye, DyeTargets target)
        {
            imageData.dfBitmap = ImageProcessing.ChangeDye(imageData.dfBitmap, dye, target);
            ImageReader.UpdateTexture(ref imageData);

            return imageData;
        }

        #endregion
    }
}
