// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Allows an effect to override player's racial display information such as race name and portrait.
    /// Used for vampirism and lycanthropy and possibly could be used for future racial overrides.
    /// Considered a minimal implementation at this time for core game to support vamp/were only.
    /// Only intended to be used on player entity. Will be permanent until removed.
    /// Only a single racial override incumbent effect can be active on player at one time.
    /// </summary>
    public abstract class RacialOverrideEffect : IncumbentEffect
    {
        int forcedRoundsRemaining = 1;

        // Always present at least one round remaining so effect system does not remove
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        // Racial overrides are permanent until removed so we manage our own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is RacialOverrideEffect);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            return;
        }
    }
}