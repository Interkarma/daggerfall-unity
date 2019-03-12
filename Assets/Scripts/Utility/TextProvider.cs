// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Numidium
//
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Interface to a text provider.
    /// Provides common functionality for dealing with various text sources.
    /// </summary>
    public interface ITextProvider
    {
        /// <summary>
        /// Gets tokens from a TEXT.RSC record.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>Text resource tokens.</returns>
        TextFile.Token[] GetRSCTokens(int id);

        /// <summary>
        /// Gets tokens from a randomly selected subrecord.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <param name="dfRand">Use Daggerfall rand() for random selection.</param>
        /// <returns>Text resource tokens.</returns>
        TextFile.Token[] GetRandomTokens(int id, bool dfRand = false);

        /// <summary>
        /// Gets random string from separated token array.
        /// Example would be flavour text variants when finding dungeon exterior.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>String randomly selected from variants.</returns>
        string GetRandomText(int id);

        /// <summary>
        /// Gets name of weapon material type.
        /// </summary>
        /// <param name="material">Material type of weapon.</param>
        /// <returns>String for weapon material name.</returns>
        string GetWeaponMaterialName(WeaponMaterialTypes material);

        /// <summary>
        /// Gets name of armor material type.
        /// </summary>
        /// <param name="material">Material type of armor.</param>
        /// <returns>String for armor material name.</returns>
        string GetArmorMaterialName(ArmorMaterialTypes material);

        /// <summary>
        /// Gets text for skill name.
        /// </summary>
        /// <param name="skill">Skill.</param>
        /// <returns>Text for this skill.</returns>
        string GetSkillName(DFCareer.Skills skill);

        /// <summary>
        /// Gets text for stat name.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Text for this stat.</returns>
        string GetStatName(DFCareer.Stats stat);

        /// <summary>
        /// Gets abbreviated text for stat name.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Abbreviated text for this stat.</returns>
        string GetAbbreviatedStatName(DFCareer.Stats stat);

        /// <summary>
        /// Gets text resource ID of stat description.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Text resource ID.</returns>
        int GetStatDescriptionTextID(DFCareer.Stats stat);

        /// <summary>
        /// Opens a new book based on the internal Daggerfall "message" field, rather than the direct filename
        /// </summary>
        /// <param name="message">The int32 message field that encodes the book's ID</param>
        /// <returns>True if book opened successfully.</returns>
        bool OpenBook(int message);

        /// <summary>
        /// Opens a new book.
        /// </summary>
        /// <param name="name">Filename of book.</param>
        /// <returns>True if book opened successfully.</returns>
        bool OpenBook(string name);

        /// <summary>
        /// Moves book to next page.
        /// </summary>
        /// <returns>True if moved to next page, false if no more pages.</returns>
        bool MoveNextPage();

        /// <summary>
        /// Moves book to previous page.
        /// </summary>
        /// <returns>True if moved to previous page, false if no earlier pages.</returns>
        bool MovePreviousPage();

        /// <summary>
        /// Returns true if a book is currently open.
        /// </summary>
        bool IsBookOpen { get; }

        /// <summary>
        /// Gets or sets current page index.
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// Gets total page count in book.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Gets text tokens for current page.
        /// </summary>
        TextFile.Token[] PageTokens { get; }
    }

    /// <summary>
    /// Implementation of a text provider.
    /// Inherit from this class and override as needed.
    /// </summary>
    public abstract class TextProvider : ITextProvider
    {
        TextFile rscFile = new TextFile();
        BookFile bookFile = new BookFile();
        int currentPage = -1;
        bool isBookOpen = false;

        public TextProvider()
        {
        }

        public virtual bool IsBookOpen
        {
            get { return isBookOpen; }
        }

        public virtual int CurrentPage
        {
            get { return currentPage; }
            set { SetPage(value); }
        }

        public virtual int PageCount
        {
            get { return bookFile.PageCount; }
        }

        public virtual TextFile.Token[] PageTokens
        {
            get { return GetPageTokens(currentPage); }
        }

        public virtual bool OpenBook(int message)
        {
            return OpenBook(BookFile.messageToBookFilename(message));
        }

        public virtual bool OpenBook(string name)
        {
            if (!BookReplacement.TryImportBook(name, bookFile) &&
                !bookFile.OpenBook(DaggerfallUnity.Instance.Arena2Path, name))
                return false;

            isBookOpen = true;
            currentPage = 0;

            return true;
        }

        public virtual bool MoveNextPage()
        {
            if (currentPage + 1 >= bookFile.PageCount)
                return false;

            currentPage++;

            return true;
        }

        public virtual bool MovePreviousPage()
        {
            if (currentPage - 1 < 0)
                return false;

            currentPage--;

            return true;
        }

        public virtual TextFile.Token[] GetPageTokens(int page)
        {
            if (IsBookOpen)
                return bookFile.GetPageTokens(page);
            else
                return null;
        }

        public virtual void SetPage(int index)
        {
            if (index < 0 || index >= bookFile.PageCount)
                throw new Exception("TextProvider: Page index out of range.");

            currentPage = index;
        }

        public virtual TextFile.Token[] GetRSCTokens(int id)
        {
            if (!rscFile.IsLoaded)
                OpenTextRSCFile();

            byte[] buffer = rscFile.GetBytesById(id);
            if (buffer == null)
                return null;

            return TextFile.ReadTokens(ref buffer, 0, TextFile.Formatting.EndOfRecord);
        }

        public virtual TextFile.Token[] GetRandomTokens(int id, bool dfRand = false)
        {
            TextFile.Token[] sourceTokens = GetRSCTokens(id);

            // Build a list of token subrecords
            List<TextFile.Token> currentStream = new List<TextFile.Token>();
            List<TextFile.Token[]> tokenStreams = new List<TextFile.Token[]>();
            for (int i = 0; i < sourceTokens.Length; i++)
            {
                // If we're at end of subrecord then start a new stream
                if (sourceTokens[i].formatting == TextFile.Formatting.SubrecordSeparator)
                {
                    tokenStreams.Add(currentStream.ToArray());
                    currentStream.Clear();
                    continue;
                }

                // Otherwise keep adding to current stream
                currentStream.Add(sourceTokens[i]);
            }

            // Complete final stream
            tokenStreams.Add(currentStream.ToArray());

            // Select a random token stream
            int index = dfRand ? (int)(DFRandom.rand() % tokenStreams.Count) : UnityEngine.Random.Range(0, tokenStreams.Count);

            // Select the next to last item from the array if the length of the last one is zero
            index = (tokenStreams[index].Length == 0 ? index - 1 : index);

            return tokenStreams[index];
        }

        public virtual string GetRandomText(int id)
        {
            // Collect text items
            List<string> textItems = new List<string>();
            TextFile.Token[] tokens = GetRSCTokens(id);
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].formatting == TextFile.Formatting.Text)
                    textItems.Add(tokens[i].text);
            }

            // Validate items
            if (textItems.Count == 0)
                return string.Empty;

            // Select random text item
            int index = UnityEngine.Random.Range(0, textItems.Count);

            return textItems[index];
        }

        public string GetWeaponMaterialName(WeaponMaterialTypes material)
        {
            switch(material)
            {
                case WeaponMaterialTypes.Iron:
                    return "Iron";
                case WeaponMaterialTypes.Steel:
                    return "Steel";
                case WeaponMaterialTypes.Silver:
                    return "Silver";
                case WeaponMaterialTypes.Elven:
                    return "Elven";
                case WeaponMaterialTypes.Dwarven:
                    return "Dwarven";
                case WeaponMaterialTypes.Mithril:
                    return "Mithril";
                case WeaponMaterialTypes.Adamantium:
                    return "Adamantium";
                case WeaponMaterialTypes.Ebony:
                    return "Ebony";
                case WeaponMaterialTypes.Orcish:
                    return "Orcish";
                case WeaponMaterialTypes.Daedric:
                    return "Daedric";
                default:
                    return string.Empty;
            }
        }

        public string GetArmorMaterialName(ArmorMaterialTypes material)
        {
            switch (material)
            {
                case ArmorMaterialTypes.Leather:
                    return "Leather";
                case ArmorMaterialTypes.Chain:
                case ArmorMaterialTypes.Chain2:
                    return "Chain";
                case ArmorMaterialTypes.Iron:
                    return "Iron";
                case ArmorMaterialTypes.Steel:
                    return "Steel";
                case ArmorMaterialTypes.Silver:
                    return "Silver";
                case ArmorMaterialTypes.Elven:
                    return "Elven";
                case ArmorMaterialTypes.Dwarven:
                    return "Dwarven";
                case ArmorMaterialTypes.Mithril:
                    return "Mithril";
                case ArmorMaterialTypes.Adamantium:
                    return "Adamantium";
                case ArmorMaterialTypes.Ebony:
                    return "Ebony";
                case ArmorMaterialTypes.Orcish:
                    return "Orcish";
                case ArmorMaterialTypes.Daedric:
                    return "Daedric";
            }

            // Standard material type value not found.
            // Try again using material value masked to material type only.
            // Some save editors will not write back correct material mask, so we must try to handle this if possible.
            // Clamping range so we don't end up in infinite loop.
            int value = (int)material >> 8;
            value = Mathf.Clamp(value, (int)ArmorMaterialTypes.Iron, (int)ArmorMaterialTypes.Daedric);
            return GetArmorMaterialName((ArmorMaterialTypes)value);
        }

        public string GetSkillName(DFCareer.Skills skill)
        {
            switch (skill)
            {
                case DFCareer.Skills.Medical:
                    return "Medical";
                case DFCareer.Skills.Etiquette:
                    return "Etiquette";
                case DFCareer.Skills.Streetwise:
                    return "Streetwise";
                case DFCareer.Skills.Jumping:
                    return "Jumping";
                case DFCareer.Skills.Orcish:
                    return "Orcish";
                case DFCareer.Skills.Harpy:
                    return "Harpy";
                case DFCareer.Skills.Giantish:
                    return "Giantish";
                case DFCareer.Skills.Dragonish:
                    return "Dragonish";
                case DFCareer.Skills.Nymph:
                    return "Nymph";
                case DFCareer.Skills.Daedric:
                    return "Daedric";
                case DFCareer.Skills.Spriggan:
                    return "Spriggan";
                case DFCareer.Skills.Centaurian:
                    return "Centaurian";
                case DFCareer.Skills.Impish:
                    return "Impish";
                case DFCareer.Skills.Lockpicking:
                    return "Lockpicking";
                case DFCareer.Skills.Mercantile:
                    return "Mercantile";
                case DFCareer.Skills.Pickpocket:
                    return "Pickpocket";
                case DFCareer.Skills.Stealth:
                    return "Stealth";
                case DFCareer.Skills.Swimming:
                    return "Swimming";
                case DFCareer.Skills.Climbing:
                    return "Climbing";
                case DFCareer.Skills.Backstabbing:
                    return "Backstabbing";
                case DFCareer.Skills.Dodging:
                    return "Dodging";
                case DFCareer.Skills.Running:
                    return "Running";
                case DFCareer.Skills.Destruction:
                    return "Destruction";
                case DFCareer.Skills.Restoration:
                    return "Restoration";
                case DFCareer.Skills.Illusion:
                    return "Illusion";
                case DFCareer.Skills.Alteration:
                    return "Alteration";
                case DFCareer.Skills.Thaumaturgy:
                    return "Thaumaturgy";
                case DFCareer.Skills.Mysticism:
                    return "Mysticism";
                case DFCareer.Skills.ShortBlade:
                    return "Short Blade";
                case DFCareer.Skills.LongBlade:
                    return "Long Blade";
                case DFCareer.Skills.HandToHand:
                    return "Hand-to-Hand";
                case DFCareer.Skills.Axe:
                    return "Axe";
                case DFCareer.Skills.BluntWeapon:
                    return "Blunt Weapon";
                case DFCareer.Skills.Archery:
                    return "Archery";
                case DFCareer.Skills.CriticalStrike:
                    return "Critical Strike";
                default:
                    return string.Empty;
            }
        }

        public string GetStatName(DFCareer.Stats stat)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    return "Strength";
                case DFCareer.Stats.Intelligence:
                    return "Intelligence";
                case DFCareer.Stats.Willpower:
                    return "Willpower";
                case DFCareer.Stats.Agility:
                    return "Agility";
                case DFCareer.Stats.Endurance:
                    return "Endurance";
                case DFCareer.Stats.Personality:
                    return "Personality";
                case DFCareer.Stats.Speed:
                    return "Speed";
                case DFCareer.Stats.Luck:
                    return "Luck";
                default:
                    return string.Empty;
            }
        }

        public string GetAbbreviatedStatName(DFCareer.Stats stat)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    return "STR";
                case DFCareer.Stats.Intelligence:
                    return "INT";
                case DFCareer.Stats.Willpower:
                    return "WIL";
                case DFCareer.Stats.Agility:
                    return "AGI";
                case DFCareer.Stats.Endurance:
                    return "END";
                case DFCareer.Stats.Personality:
                    return "PER";
                case DFCareer.Stats.Speed:
                    return "SPD";
                case DFCareer.Stats.Luck:
                    return "LUC";
                default:
                    return string.Empty;
            }
        }

        public int GetStatDescriptionTextID(DFCareer.Stats stat)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    return 0;
                case DFCareer.Stats.Intelligence:
                    return 1;
                case DFCareer.Stats.Willpower:
                    return 2;
                case DFCareer.Stats.Agility:
                    return 3;
                case DFCareer.Stats.Endurance:
                    return 4;
                case DFCareer.Stats.Personality:
                    return 5;
                case DFCareer.Stats.Speed:
                    return 6;
                case DFCareer.Stats.Luck:
                    return 7;
                default:
                    return -1;
            }
        }

        #region Protected Methods

        protected void OpenTextRSCFile()
        {
            rscFile.Load(DaggerfallUnity.Instance.Arena2Path, TextFile.Filename);
        }

        #endregion
    }
}
