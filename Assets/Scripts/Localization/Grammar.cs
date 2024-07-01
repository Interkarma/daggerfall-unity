// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2024 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Daneel53
// 

using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Localization.LanguageRules;

namespace DaggerfallWorkshop.Localization
{
    public abstract class GrammarRules
    {
        public abstract string ProcessGrammar(string text);
        public abstract void SetHeroGender(Genders Gender);
        public abstract void SetNPCGender(Genders Gender);
    }

    public static class GrammarManager
    {
        public static GrammarRules grammarProcessor = (GrammarRules) new DefaultGrammarRules();
    }
}


