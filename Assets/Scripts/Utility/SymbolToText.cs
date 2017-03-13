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

using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Resolves symbols like %var, =var_, __var, etc. to text for various objects.
    /// More information: http://www.dfworkshop.net/static_files/questing-source-docs.html#qrcsymbols
    /// Caller will need to supply objects (e.g. quest) as needed.
    /// NOTE: This class will expand over time.
    /// </summary>
    public class SymbolToText
    {
        #region Structs and Enums

        /// <summary>
        /// A symbol to resolve.
        /// </summary>
        struct Symbol
        {
            SymbolTypes type;
            string name;
            string result;
        }

        /// <summary>
        /// The type of symbol expected depends on the characters wrapping the symbol name.
        /// Not all quest resources support all symbol types. A summary is below:
        /// 
        /// Symbol      Item    Person              Place[1]            Foe	            Clock	
        /// _foo_       ring    Zaphod Beeblebrox   Lord Marcus' Arms   Warrior	        NO	
        /// __foo_      NO      Adams residence     Daggerfall City[2]  NO              NO
        /// ___foo_     NO      Gothway Gardens     Ruins of Bethsoft   NO              NO
        /// ____foo_    NO      NO                  Daggerfall          NO              NO	
        /// =foo_       NO      Acrobat             NO                  Arthur Dent	    4 days	
        /// ==foo_      NO      Blades              NO                  Fighter         NO
        ///
        /// [1] Not available for permanent sites. [2] This form is invalid for remote dungeons.
        /// 
        /// </summary>
        enum SymbolTypes
        {
            NameSymbol1,        // _foo_    - replaced with name of foo
            NameSymbol2,        // __foo_   - replaced with name of house/shop where foo is found
            NameSymbol3,        // ___foo_  - replaced with name of town where foo is found
            NameSymbol4,        // ____foo_ - replaced with name of region where foo is found
            DetailsSymbol,      // =foo_    - replaced with detail based on target symbol type (e.g. days remaining on a clock, player class, enemy name)
            FactionSymbol,      // ==foo_   - replaced with faction of target symbol
            MacroSymbol,        // %foo     - replaced with output based on context (pronoun macros relate back to previous NPC/foe symbol in source text)
            BindingSymbol,      // =#foo_   - replaced with current keybind for foo action (Daggerfall Unity only)
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves all possible quest symbols in source text.
        /// </summary>
        /// <param name="text">Source text.</param>
        /// <param name="quest">Quest object. Must contain symbols for objects referenced in source text.</param>
        /// <returns>Text with all possible symbols replaced.</returns>
        string ResolveQuestSymbols(string text, Quest quest)
        {
            return text;
        }

        #endregion

        #region Private Methods
        #endregion
    }
}