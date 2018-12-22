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
    /// Uber-effect used to deliver passive special advantages and disadvantages to player.
    /// More active specials (e.g. critical strike, disallowed armour types) are handled in related systems.
    /// NOTES:
    ///  * This effect is a work in progress and will be added to over time.
    ///  * Could also be assigned to other entities but at this time only using on player.
    /// </summary>
    public class PassiveSpecialsEffect : IncumbentEffect
    {

        #region Fields

        public static readonly string EffectKey = "Passive-Specials";

        int forcedRoundsRemaining = 1;

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
        }

        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is PassiveSpecialsEffect);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            return;
        }

        #endregion

    }
}