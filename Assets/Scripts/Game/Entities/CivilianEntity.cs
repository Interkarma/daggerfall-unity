// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

namespace DaggerfallWorkshop.Game.Entity
{
    public class CivilianEntity : DaggerfallEntity
    {
        public CivilianEntity(DaggerfallEntityBehaviour entityBehaviour)
            : base(entityBehaviour)
        {
        }

        /// <summary>
        /// Assigns default entity settings.
        /// </summary>
        public override void SetEntityDefaults()
        {
            MaxHealth = 1;
            CurrentHealth = 1;
        }
    }
}
