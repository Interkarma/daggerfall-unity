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
        [SerializeField]
        string original;
        [SerializeField]
        string name;

        /// <summary>
        /// Gets original symbol at time it was set.
        /// </summary>
        public string Original
        {
            get { return original; }
            set { original = value; }
        }

        /// <summary>
        /// Gets name of symbol stripped to inner symbol name only.
        /// This is what should be used by quest system.
        /// </summary>
        public string Name
        {
            get { return name; }
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
        /// Restore symbol constructor.
        /// </summary>
        /// <param name="original">Symbol original string.</param>
        /// <param name="name">Symbol inner string.</param>
        public Symbol(string original, string name)
        {
            this.original = original;
            this.name = name;
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

        /// <summary>
        /// Clone this symbol.
        /// </summary>
        /// <returns>New Symbol with same details as this one.</returns>
        public Symbol Clone()
        {
            return new Symbol(original, name);
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