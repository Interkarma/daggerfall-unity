// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2024 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Daneel53
// 

using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Localization.LanguageRules;
using System;

namespace DaggerfallWorkshop.Localization
{
    public abstract class GrammarRules
    {
        public abstract string ProcessGrammar(string text);
        public abstract void SetHeroGenderGetter(Func<Genders> HeroGenderGetter);
        public abstract void SetNPCGenderGetter(Func<Genders> NPCGenderGetter);
    }

    public static class GrammarManager
    {
        public static GrammarRules grammarProcessor = new DefaultGrammarRules();
    }
}


