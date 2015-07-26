// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

#region Using Statements
using System;
using System.IO;
using System.Text;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores information about a Daggerfall monster.
    /// </summary>
    public struct DFMonster
    {
        public MonsterFile.MonsterCFG MonsterCFG;

        public string Name;
        public byte ResistanceFlags;
        public byte ImmunityFlags;
        public byte LowToleranceFlags;
        public byte CriticalWeaknessFlags;

        public bool AcuteHearing;
        public bool Athleticism;
        public bool AdrenalineRush;
        public bool NoRegenSpellPoints;
        public bool SunDamage;
        public bool HolyDamage;

        public int SpellPointsInDark;
        public int SpellPointsInLight;
        public float SpellPointMultiplier;
    }
}