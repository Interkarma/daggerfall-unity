// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using System.Collections;
using System.Text.RegularExpressions;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Manages a text symbol used by quest system.
    /// Symbols are used by resources, tasks, and text replacement.
    /// </summary>
    public class Symbol : IComparable
    {
        string original;
        string name;

        /// <summary>
        /// Gets original symbol at time it was set.
        /// </summary>
        public string Original
        {
            get { return original; }
        }

        /// <summary>
        /// Gets name of symbol stripped to inner symbol name only.
        /// This is what should be used by quest system.
        /// </summary>
        public string Name
        {
            get { return original; }
            set { Set(value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Symbol()
        {
        }

        /// <summary>
        /// Set symbol constructor.
        /// </summary>
        /// <param name="symbol">Symbol text from source.</param>
        public Symbol(string symbol)
        {
            Set(symbol);
        }

        /// <summary>
        /// Set symbol.
        /// </summary>
        /// <param name="symbol">Symbol text from source.</param>
        public void Set(string symbol)
        {
            original = symbol;
            name = Parser.GetInnerSymbolName(symbol);
        }

        #region IComparable

        /// <summary>
        /// Compare value with other symbol.
        /// </summary>
        /// <param name="other">Other symbol to compare. Case sensitive.</param>
        /// <returns>Compare result. 0 means equal.</returns>
        public int CompareTo(object other)
        {
            return string.Compare(name, (other as Symbol).Name);
        }

        #endregion
    }
}