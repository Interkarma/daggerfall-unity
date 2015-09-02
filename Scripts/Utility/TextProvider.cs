// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
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
        /// Gets text for skill name.
        /// </summary>
        /// <param name="skill">Skill.</param>
        /// <returns>Text for this skill.</returns>
        string GetSkillName(DFClass.Skills skill);
        
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

        public virtual bool OpenBook(string name)
        {
            if (!bookFile.OpenBook(DaggerfallUnity.Instance.Arena2Path, name))
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

        public string GetSkillName(DFClass.Skills skill)
        {
            switch (skill)
            {
                case DFClass.Skills.Medical:
                    return "Medical";
                case DFClass.Skills.Etiquette:
                    return "Etiquette";
                case DFClass.Skills.Streetwise:
                    return "Streetwise";
                case DFClass.Skills.Jumping:
                    return "Jumping";
                case DFClass.Skills.Orcish:
                    return "Orcish";
                case DFClass.Skills.Harpy:
                    return "Harpy";
                case DFClass.Skills.Giantish:
                    return "Giantish";
                case DFClass.Skills.Dragonish:
                    return "Dragonish";
                case DFClass.Skills.Nymph:
                    return "Nymph";
                case DFClass.Skills.Daedric:
                    return "Daedric";
                case DFClass.Skills.Spriggan:
                    return "Spriggan";
                case DFClass.Skills.Centaurian:
                    return "Centaurian";
                case DFClass.Skills.Impish:
                    return "Impish";
                case DFClass.Skills.Lockpicking:
                    return "Lockpicking";
                case DFClass.Skills.Mercantile:
                    return "Mercantile";
                case DFClass.Skills.Pickpocket:
                    return "Pickpocket";
                case DFClass.Skills.Stealth:
                    return "Stealth";
                case DFClass.Skills.Swimming:
                    return "Swimming";
                case DFClass.Skills.Climbing:
                    return "Climbing";
                case DFClass.Skills.Backstabbing:
                    return "Backstabbing";
                case DFClass.Skills.Dodging:
                    return "Dodging";
                case DFClass.Skills.Running:
                    return "Running";
                case DFClass.Skills.Destruction:
                    return "Destruction";
                case DFClass.Skills.Restoration:
                    return "Restoration";
                case DFClass.Skills.Illusion:
                    return "Illusion";
                case DFClass.Skills.Alteration:
                    return "Alteration";
                case DFClass.Skills.Thaumaturgy:
                    return "Thaumaturgy";
                case DFClass.Skills.Mysticism:
                    return "Mysticism";
                case DFClass.Skills.ShortBlade:
                    return "Short Blade";
                case DFClass.Skills.LongBlade:
                    return "Long Blade";
                case DFClass.Skills.HandToHand:
                    return "Hand To Hand";
                case DFClass.Skills.Axe:
                    return "Axe";
                case DFClass.Skills.BluntWeapon:
                    return "Blunt Weapon";
                case DFClass.Skills.Archery:
                    return "Archery";
                case DFClass.Skills.CriticalStrike:
                    return "Critical Strike";
                default:
                    return string.Empty;
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
