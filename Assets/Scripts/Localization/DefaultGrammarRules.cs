// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2024 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Daneel53
// 

namespace GrammarModule.LanguageRules
{
	public class DefaultGrammarRules : GrammarRules
	{
		public override string ProcessGrammar(string text)
		{
			// Process the grammar tokens
			return text;
		}
		public override void SetGenreHero(string Genre)
		{
			// Locally store the genre of the hero so that it can be used
            // by the grammar tokens

		}
	}
}

