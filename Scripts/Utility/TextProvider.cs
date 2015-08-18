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
        TextFile textFile = new TextFile();
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
    }
}
