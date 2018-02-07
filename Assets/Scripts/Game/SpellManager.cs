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

using UnityEngine;
using System;
using DaggerfallWorkshop.Game.Effects;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Peered with a DaggerfallEntityBehaviour for spell casting and receiving.
    /// Handles transmission of spells into world and provide for certain effect hooks.
    /// Enemy AI can cast spells through this behaviour into world.
    /// Player also casts spells through this behaviour with some additional coordination needed elsewhere.
    /// </summary>
    public class SpellManager : MonoBehaviour
    {
        #region Fields

        FakeSpell readySpell = null;

        #endregion
    }
}
